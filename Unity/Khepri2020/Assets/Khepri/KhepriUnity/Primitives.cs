using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using Parabox.CSG;
using UnityEditor;
using UnityMeshSimplifier;

namespace KhepriUnity {
    public class Primitives : KhepriBase.Primitives {

        public static Primitives Instance { get; set; }

        GameObject currentParent;
        List<GameObject> parents;
        GameObject sun;
        bool editorLastSetSun; // Used to determine if the SetSun method was last called by the editor or Julia client. 
        bool editorLastSetInteractive;
        bool interactiveMode;
        int numRequests;
        public static List<GameObject> SelectedGameObjects { get; set; }
        public bool InSelectionProcess { get; set; }
        public bool SelectingManyGameObjects { get; set; }
        //Dictionary<GameObject, Color> highlighted;
        List<GameObject> highlighted;
        Outline.Mode highlightMode;
        Color highlightColor;
        float highlightWidth;

        //VRAD
        public static Queue<string> poolFunctions = new Queue<string>();
        public VRADHandler VradHandler;

        
        Transform playerTransform;
        Camera mainCamera;
        Material currentMaterial;
        Material defaultMaterial;

        bool applyMaterials;
        bool applyColliders;
        bool enableLights;
        bool enablePointlightShadows;
        private List<GameObject> lights;
        bool enableMergeParent;
        bool bakedLights;
        bool applyLOD;
        LODLevel[] lodLevels;
        SimplificationOptions lodSimplificationOptions;
        bool makeUVs;
        bool makeStatic;

        Channel channel;
        Processor<Channel, Primitives> processor;

        public Primitives(GameObject mainObject) : this(null, mainObject) { }

        public Primitives(Channel channel, GameObject mainObject) {
            this.channel = channel;
            this.currentParent = mainObject;
            this.parents = new List<GameObject> { mainObject };
            this.sun = GameObject.FindWithTag("Sun");
            this.editorLastSetSun = true;
            this.editorLastSetInteractive = true;
            this.interactiveMode = false;
            this.numRequests = Int32.MaxValue;
            Primitives.SelectedGameObjects = new List<GameObject>();
            this.InSelectionProcess = false;
            this.highlighted = new List<GameObject>();//new Dictionary<GameObject, Color> { };
            this.highlightMode = Outline.Mode.OutlineAll;
            this.highlightColor = new Color(1, 0.45f, 0);
            this.highlightWidth = 4;
            this.mainCamera = Camera.main;
            this.playerTransform = GameObject.FindWithTag("Player").transform;
            this.defaultMaterial = new Material(Shader.Find("Standard"));
            this.defaultMaterial.enableInstancing = true;
            this.currentMaterial = defaultMaterial;
            this.applyMaterials = true;
            this.applyColliders = true;
            this.enableLights = true;
            this.enablePointlightShadows = true;
            this.bakedLights = false;
            this.applyLOD = true;
            this.makeUVs = false;
            this.makeStatic = true;
            this.enableMergeParent = false;
            this.lights = new List<GameObject>();
            
            this.lodLevels = new LODLevel[]
            {
                new LODLevel(0.5f, 1f)
                {
                    CombineMeshes = false,
                    CombineSubMeshes = false,
                    SkinQuality = SkinQuality.Auto,
                    ShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ReceiveShadows = true,
                    SkinnedMotionVectors = true,
                    LightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes,
                    ReflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.BlendProbes,
                },
                new LODLevel(0.17f, 0.65f)
                {
                    CombineMeshes = true,
                    CombineSubMeshes = false,
                    SkinQuality = SkinQuality.Auto,
                    ShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ReceiveShadows = true,
                    SkinnedMotionVectors = true,
                    LightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes,
                    ReflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Simple
                },
                new LODLevel(0.02f, 0.4225f)
                {
                    CombineMeshes = true,
                    CombineSubMeshes = true,
                    SkinQuality = SkinQuality.Bone2,
                    ShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    ReceiveShadows = false,
                    SkinnedMotionVectors = false,
                    LightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off,
                    ReflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off
                }
            };
            Instance = this;
        }

        #region Setters
        public void SetChannel(Channel channel) => this.channel = channel;

        public void SetProcessor(Processor<Channel, Primitives> processor) {
            this.processor = processor;
            this.processor.MaxRepeated = numRequests;
        } 
        
        public GameObject SetCurrentParent(GameObject newParent) {
            GameObject prevParent = currentParent;
            currentParent = newParent;
            return prevParent;
        }
        
        public void SetApplyMaterials(bool apply) => applyMaterials = apply;
        public void SetApplyColliders(bool apply) => applyColliders = apply;
        public void SetApplyLOD(bool apply) => applyLOD = apply;
        public void SetLODLevels(LODLevel[] lodLevels) => this.lodLevels = lodLevels;
        public void SetLODSimplificationOptions(SimplificationOptions options) =>
            this.lodSimplificationOptions = options;
        public void SetEnableMergeParent(bool enable) => enableMergeParent = enable;
        public void SetCalculateUV(bool apply) => makeUVs = apply;
        
        public void SetEnableLights(bool apply) {
            if (apply != enableLights) {
                enableLights = apply;
                // Update all current lights retroatively to enableLights value
                UpdateEnableLights();
            }
        }
        
        public void SetEnablePointLightsShadow(bool apply) {
            if (apply != enablePointlightShadows) {
                enablePointlightShadows = apply;
                // Update all current lights retroatively to enableLights value
                UpdateEnablePointLightsShadow();
            }
        }
        
        #if UNITY_EDITOR
        public void SetBakedLights(bool apply) {
            if (apply != bakedLights) {
                bakedLights = apply;
                // Update all current lights retroatively to enableLights value
                UpdateBakedLights();
            }
        }
        #endif

        private void FindPointLights() {
            if (lights.Count == 0) {
                foreach (Transform child in currentParent.transform) {
                    if (child.name == "PointLight")
                        lights.Add(child.gameObject);
                }
            }
        }
        
        private void UpdateEnableLights() {
            FindPointLights();
            foreach (var light in lights) { // FIXME this can be parallelized
                light.GetComponent<Light>().enabled = enableLights; 
            }
        }

        #if UNITY_EDITOR
        private void UpdateBakedLights() {
            FindPointLights();
            foreach (var light in lights) { 
                light.GetComponent<Light>().lightmapBakeType = bakedLights ? LightmapBakeType.Baked : LightmapBakeType.Realtime;
            }
        }
        #endif

        private void UpdateEnablePointLightsShadow() {
            FindPointLights();
            foreach (var light in lights) { 
                light.GetComponent<Light>().shadows = enablePointlightShadows ? LightShadows.Hard : LightShadows.None;
            }
        }
        
        public void MakeStaticGameObjects(bool val) {
            makeStatic = val;
        }

        public void SetHighlightMode(Outline.Mode mode) {
            highlightMode = mode;
            UpdateHighlights();
        }
        public void SetHighlightColor(Color color) {
            highlightColor = color;
            UpdateHighlights();
        }
        public void SetHighlightWidth(float width) {
            highlightWidth = width;
            UpdateHighlights();
        }
        private void UpdateHighlights() {
            foreach (GameObject obj in highlighted) {
                Outline outline = obj.GetComponent<Outline>();
                outline.OutlineMode = highlightMode;
                outline.OutlineColor = highlightColor;
                outline.OutlineWidth = highlightWidth;
            }
        }

        public void SetSun(float x, float y, float z) {
            SetSunEditor(x, y, z, false);
        }

        // Khepri does not allow this to have the same name as the above...
        public void SetSunEditor(float x, float y, float z, bool isEditor) {
            if (sun == null)
                sun = GameObject.FindWithTag("Sun");
            sun.transform.rotation = Quaternion.Euler(x, y, z);
            editorLastSetSun = isEditor;
        }

        public void SetEditorLastSun(bool b) {
            editorLastSetSun = b;
        }

        public void SetEditorLastInteractive(bool b) {
            editorLastSetInteractive = b;
        }

        #endregion

        #region Getters
        public GameObject CurrentParent() => currentParent;
        
        #if UNITY_EDITOR
        // Analysis
        public string GetRenderResolution() {
            return UnityStats.screenRes;
        }
        public float GetCurrentFPS() {
            return 1 / UnityStats.frameTime;
        }

        public int GetViewTriangleCount() {
            return UnityStats.triangles;
        }
        
        public int GetViewVertexCount() {
            return UnityStats.vertices;
        }
        #endif

        public Vector3 GetSunRotation() {
            return sun.transform.rotation.eulerAngles;
        }

        public bool GetLastSunEdit() {
            return editorLastSetSun; // true if editor was the last to SetSun or false if it was Julia client. Used by the UI.
        }

        public bool GetLastInteractiveEdit() {
            return editorLastSetInteractive; // true if editor was the last to set the interactive mode or false if it was Julia client. Used by the UI.
        }
        
        public bool GetInteractiveMode() {
            return interactiveMode; // true if #requests != maxint
        }

        public int GetNumRequests() {
            return numRequests;
        }
        #endregion

        #region Auxiliary
        public void SetActive(GameObject obj, bool state) => obj.SetActive(state);
        
        GameObject ApplyCollider(GameObject obj, Mesh mesh) {
            if (applyColliders) {
                MeshCollider meshCollider = obj.GetComponent<MeshCollider>();
                if (meshCollider == null)
                    meshCollider = obj.AddComponent<MeshCollider>();
                
                meshCollider.sharedMesh = mesh;
            }
            return obj;
        }

        GameObject ApplyCollider(GameObject obj) {
            if (applyColliders) {
                Collider collider = obj.GetComponent<Collider>();
                if (collider == null) {
                    MeshCollider meshCollider = obj.AddComponent<MeshCollider>();
                    meshCollider.sharedMesh = obj.GetComponent<MeshFilter>().sharedMesh;
                }
            } else {
                Collider collider = obj.GetComponent<Collider>();
                if (collider != null) {
                    Component.DestroyImmediate(collider);
                }
            }
            return obj;
        }
        
        public GameObject ApplyLOD(GameObject g) {
            if (!applyLOD)
                return g;
            Mesh mesh = g.GetComponent<MeshFilter>()?.sharedMesh;
            if (mesh.vertexCount < 30) // HACK! So it doesn't simplify already simple meshes
                return g;

            LODGenerator.GenerateLODGroup(g, lodLevels, true, lodSimplificationOptions);
            return g;
        }

        public GameObject ApplyCurrentMaterial(GameObject obj) =>
            ApplyMaterial(obj, currentMaterial);
        
        public GameObject ApplyMaterial(GameObject obj, Material material) {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (applyMaterials) {
                renderer.sharedMaterial = material;
            }
            else {
                renderer.sharedMaterial = defaultMaterial;
            }
            return obj;
        }
        
        // This helps improve performance 
        public GameObject MakeStatic(GameObject s) {
            if (makeStatic) {
                s.isStatic = true;
                foreach (Transform trans in s.transform) {
                    MakeStatic(trans.gameObject);
                }
            }
            return s;
        }
        
        Vector3 PlaneNormal(Vector3[] pts) {
            Vector3 pt = pts[0];
            Vector3 sum = Vector3.zero;
            for (int i = 1; i < pts.Length - 1; i++) {
                if (pts[i] == pt || pts[i + 1] == pt) continue;
                sum += Vector3.Cross(pts[i] - pt, pts[i + 1] - pt);
            }
            sum.Normalize();
            return sum;
        }

        Vector3[] ReverseIfNeeded(Vector3[] pts, Vector3 normal) {
            Vector3 normalPts = PlaneNormal(pts);
            return (Vector3.Dot(normalPts, normal) > 0) ? pts : pts.Reverse().ToArray();
        }
        
        Mesh CreatePolygonMesh(Vector3[] ps) {
            Poly2Mesh.Polygon polygon = new Poly2Mesh.Polygon();
            polygon.outside = new List<Vector3>(ps);
            return Poly2Mesh.CreateMesh(polygon);
        }
        
        Mesh CreatePolygonMeshWithHoles(Vector3[] ps, Vector3[][] holes) {
            Poly2Mesh.Polygon polygon = new Poly2Mesh.Polygon();
            polygon.outside = new List<Vector3>(ps);
            polygon.holes =
                new List<List<Vector3>>(
                    new List<Vector3[]>(holes).Select(e => new List<Vector3>(e)));
            return Poly2Mesh.CreateMesh(polygon);
        }
        
        Mesh CreateTrigMesh(Vector3[] ps, Vector3 q) {
            Vector3[] vertices = new Vector3[ps.Length + 1];
            Array.Copy(ps, vertices, ps.Length);
            vertices[ps.Length] = q;
            int[] triangles = new int[ps.Length * 3];
            int k = 0;
            for (int i = 0; i < ps.Length - 1; i++) {
                triangles[k++] = i;
                triangles[k++] = i + 1;
                triangles[k++] = ps.Length;
            }
            triangles[k++] = ps.Length - 1;
            triangles[k++] = 0;
            triangles[k++] = ps.Length;
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            //Unwrapping.GenerateSecondaryUVSet(mesh); 
            return mesh;
        }
        
        Mesh CreateQuadMesh(Vector3[] ps, Vector3[] qs) {
            Vector3[] vertices = new Vector3[ps.Length * 2];
            Array.Copy(ps, vertices, ps.Length);
            Array.Copy(qs, 0, vertices, ps.Length, qs.Length);
            int[] triangles = new int[ps.Length * 2 * 3];
            int k = 0;
            for (int i = 0, j = ps.Length; i < ps.Length - 1; i++, j++) {
                triangles[k++] = i;
                triangles[k++] = i + 1;
                triangles[k++] = j + 1;
                triangles[k++] = i;
                triangles[k++] = j + 1;
                triangles[k++] = j;
            }
            triangles[k++] = ps.Length - 1;
            triangles[k++] = 0;
            triangles[k++] = ps.Length;
            triangles[k++] = ps.Length - 1;
            triangles[k++] = ps.Length;
            triangles[k++] = 2 * ps.Length - 1;
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            //Unwrapping.GenerateSecondaryUVSet(mesh); 
            return mesh;
        }
        
        Mesh CombineMesh(Mesh[] meshes) {
            Mesh mainMesh = new Mesh();
            List<CombineInstance> combineInstances = new List<CombineInstance>();

            for (int i = 0; i < meshes.Length; i++) {
                CombineInstance combineInstance = new CombineInstance();
                combineInstance.subMeshIndex = 0;
                combineInstance.mesh = meshes[i];
                combineInstance.transform = Matrix4x4.identity;
                combineInstances.Add(combineInstance);
            }
            mainMesh.CombineMeshes(combineInstances.ToArray());
            mainMesh.Optimize();
            //CalculateUVs(mainMesh);
            return mainMesh;
        }

        #endregion
        
        #region Khepri Operations
        // ||||||||||||||||||||||||||| Layer |||||||||||||||||||||||||||
        public GameObject CreateParent(String name) {
            GameObject newParent = MakeStatic(new GameObject(name));
            SetActive(newParent, false);
            parents.Add(newParent);
            return newParent;
        }
        
        public void SwitchToParent(GameObject newParent) {
            SetActive(currentParent, false);
            SetActive(newParent, true);
            currentParent = newParent;
        }

        public void OptimizeParent() {
            if (enableMergeParent)
                MergeParent();
            StaticBatchingUtility.Combine(currentParent.gameObject);
        }

        public void MergeParent() {
            if (currentParent.GetComponent<MeshFilter>() != null || currentParent.transform.childCount == 0) return; // Return if the current parent has already merged its mesh
            Renderer[] renderers = LODGenerator.GetChildRenderersForLOD(currentParent);
            var meshRenderers = (from renderer in renderers
                where renderer.enabled && renderer as MeshRenderer != null 
                select renderer as MeshRenderer).ToArray();

            Material[] materials;
            Mesh mesh = MeshCombiner.CombineMeshes(currentParent.transform, meshRenderers, out materials);
            mesh.Optimize();
            MeshRenderer meshRenderer = currentParent.AddComponent<MeshRenderer>();
            meshRenderer.materials = materials;
            MeshFilter meshFilter = currentParent.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;
            
            foreach (var renderer in renderers) {
                renderer.enabled = false;
            }
        }
        
        // ||||||||||||||||||||||||||| Delete |||||||||||||||||||||||||||
        public void DeleteMany(GameObject[] objs) {
            int count = objs.Length;
            for (int i = 0; i < count; i++) {
                lights.Remove(objs[0]);
                highlighted.Remove(objs[0]);
                GameObject.DestroyImmediate(objs[0]);
            }
        }
        
        public void DeleteAllInParent(GameObject parent) {
            int count = parent.transform.childCount;
            for (int i = 0; i < count; i++) {
                lights.Remove(parent.transform.GetChild(0).gameObject);
                highlighted.Remove(parent.transform.GetChild(0).gameObject);
                GameObject.DestroyImmediate(parent.transform.GetChild(0).gameObject);
            }
        }
        
        public void DeleteAll() {
            foreach (GameObject parent in parents) {
                DeleteAllInParent(parent);
            }
        }
        
        // ||||||||||||||||||||||||||| Geometric transformations |||||||||||||||||||||||||||
        public void Move(GameObject s, Vector3 v) {
            s.transform.localPosition += v;
        }

        public void Scale(GameObject s, Vector3 p, float scale) {
            Vector3 sp = s.transform.localPosition;
            s.transform.localScale *= scale;
            s.transform.localPosition = p + (sp - p) * scale;
        }

        public void Rotate(GameObject s, Vector3 p, Vector3 n, float a) {
            Vector3 pw = s.transform.parent.TransformPoint(p);
            Vector3 nw = s.transform.parent.TransformVector(n);
            s.transform.RotateAround(pw, nw, -a * Mathf.Rad2Deg);
        }
        
        // ||||||||||||||||||||||||||| Boolean operations |||||||||||||||||||||||||||
        // FIXME Maybe we shouldn't apply LOD on the result of boolean operations... EDIT: REMOVED LOD ON THIS
        // LOD on simple resulted objects can lead to weird results for the human eye
        public GameObject Unite(GameObject s0, GameObject s1) {
            Mesh m = CSG.Union(s0, s1);
            GameObject composite = new GameObject();
            composite.transform.parent = currentParent.transform;
            composite.AddComponent<MeshFilter>().sharedMesh = m;
            composite.AddComponent<MeshRenderer>().sharedMaterial = s0.GetComponent<MeshRenderer>().sharedMaterial;
            composite.name = "Union";
            ApplyCollider(composite, m);
            //ApplyLOD(composite);
            GameObject.DestroyImmediate(s0);
            GameObject.DestroyImmediate(s1);
            return MakeStatic(composite);
        }

        public GameObject Subtract(GameObject s0, GameObject s1) {
            Mesh m = CSG.Subtract(s0, s1);
            GameObject composite = new GameObject();
            composite.transform.parent = currentParent.transform;
            composite.AddComponent<MeshFilter>().sharedMesh = m;
            composite.AddComponent<MeshRenderer>().sharedMaterial = s0.GetComponent<MeshRenderer>().sharedMaterial;
            composite.name = "Subtraction";
            ApplyCollider(composite, m);
            //ApplyLOD(composite);
            GameObject.DestroyImmediate(s0);
            GameObject.DestroyImmediate(s1);
            return MakeStatic(composite);
        }

        public GameObject Intersect(GameObject s0, GameObject s1) {
            Mesh m = CSG.Intersect(s0, s1);
            GameObject composite = new GameObject();
            composite.transform.parent = currentParent.transform;
            composite.AddComponent<MeshFilter>().sharedMesh = m;
            composite.AddComponent<MeshRenderer>().sharedMaterial = s0.GetComponent<MeshRenderer>().sharedMaterial;
            composite.name = "Intersection";
            ApplyCollider(composite, m);
            //ApplyLOD(composite);
            GameObject.DestroyImmediate(s0);
            GameObject.DestroyImmediate(s1);
            return MakeStatic(composite);
        }

        public void SubtractFrom(GameObject s0, GameObject s1) {
            Mesh m = CSG.Subtract(s0, s1);
            s0.GetComponent<MeshFilter>().sharedMesh = m;
            ApplyCollider(s0, m);
            GameObject.DestroyImmediate(s1);
            s0.name += " Subtraction";
        }
        
        T EnsureNonNull<T>(T arg) {
            if (arg == null) throw new NullReferenceException();
            return arg;
        }
        
        // ||||||||||||||||||||||||||| Interactiveness |||||||||||||||||||||||||||
        public int SetMaxNonInteractiveRequests(int n) {
            int prev = numRequests;
            numRequests = n;
            if (processor != null)
                processor.MaxRepeated = n;
            this.interactiveMode = true;
            return prev;
        }

        public void SetNonInteractiveRequests() {
            SetMaxNonInteractiveRequests(Int32.MaxValue);
            this.interactiveMode = false;
        }
        
        public void SetInteractiveRequests() => SetMaxNonInteractiveRequests(0);

        public int SetMaxNonInteractiveRequestsEditor(int n) {
            editorLastSetInteractive = true;
            return SetMaxNonInteractiveRequests(n);
        }
        public void SetNonInteractiveRequestEditor() {
            editorLastSetInteractive = true;
            SetNonInteractiveRequests();
        }
        
        public void SetInteractiveRequestEditor() {
            editorLastSetInteractive = true;
            SetInteractiveRequests();
        }


        // ||||||||||||||||||||||||||| Resources |||||||||||||||||||||||||||
        public Material LoadMaterial(String name) => EnsureNonNull(Resources.Load<Material>(name));
        public void SetCurrentMaterial(Material material) => currentMaterial = material;
        public Material CurrentMaterial() => currentMaterial;
        public GameObject LoadResource(String name) => Resources.Load<GameObject>(name);
        
        // ||||||||||||||||||||||||||| Simple Geometry |||||||||||||||||||||||||||
        public GameObject PointLight(Vector3 position, Color color, float range, float intensity) {
            GameObject pLight = new GameObject("PointLight");
            Light light = pLight.AddComponent<Light>();
            light.enabled = enableLights;
            pLight.transform.parent = currentParent.transform;
            light.type = LightType.Point;
            light.color = color;
            light.range = range;         // How far the light is emitted from the center of the object
            light.intensity = intensity; // Brightness of the light
            light.shadows = enablePointlightShadows ? LightShadows.Hard : LightShadows.None;
            #if UNITY_EDITOR
            light.lightmapBakeType = bakedLights ? LightmapBakeType.Baked : LightmapBakeType.Realtime;
            #endif
            pLight.transform.localPosition = position;
            lights.Add(pLight);
            return MakeStatic(pLight);
        }
        
        public GameObject Window(Vector3 position, Quaternion rotation, float dx, float dy, float dz) {
            GameObject s = GameObject.CreatePrimitive(PrimitiveType.Cube);
            s.name = "Window";
            s.transform.parent = currentParent.transform;
            s.transform.localScale = new Vector3(Math.Abs(dx), Math.Abs(dy), Math.Abs(dz));
            s.transform.localRotation = rotation;
            s.transform.localPosition = position + rotation * new Vector3(dx / 2, dy / 2, dz / 2);
            ApplyMaterial(s, Resources.Load<Material>("Default/Materials/Glass"));
            ApplyCollider(s);
            return MakeStatic(s);
        }
        
        public GameObject Box(Vector3 position, Vector3 vx, Vector3 vy, float dx, float dy, float dz) {
            Quaternion rotation = Quaternion.LookRotation(vx, vy);
            GameObject s = GameObject.CreatePrimitive(PrimitiveType.Cube);
            s.name = "Box";
            s.transform.parent = currentParent.transform;
            s.transform.localScale = new Vector3(Math.Abs(dx), Math.Abs(dy), Math.Abs(dz));
            s.transform.localRotation = rotation;
            s.transform.localPosition = position + rotation * new Vector3(dx / 2, dy / 2, dz / 2);
            ApplyCurrentMaterial(s);
            ApplyCollider(s);
            return MakeStatic(s);
        }
        
        public GameObject RightCuboidNamed(String name, Vector3 position, Vector3 vx, Vector3 vy, float dx, float dy, float dz, float angle, Material material) {
            Quaternion rotation = Quaternion.LookRotation(vx, vy);
            GameObject s = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rotation = rotation * Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle);
            s.name = name;
            s.transform.parent = currentParent.transform;
            s.transform.localScale = new Vector3(Math.Abs(dx), Math.Abs(dy), Math.Abs(dz));
            s.transform.localRotation = rotation;
            s.transform.localPosition = position + rotation * new Vector3(0, 0, dz / 2);
            ApplyCollider(s);
            ApplyMaterial(s, material);
            return MakeStatic(s);
        }
        public GameObject RightCuboidWithMaterial(Vector3 position, Vector3 vx, Vector3 vy, float dx, float dy, float dz, float angle, Material material) =>
	    RightCuboidNamed("RightCuboid", position, vx, vy, dx, dy, dz, angle, material);
        public GameObject RightCuboid(Vector3 position, Vector3 vx, Vector3 vy, float dx, float dy, float dz, float angle) =>
	    RightCuboidWithMaterial(position, vx, vy, dx, dy, dz, angle, currentMaterial);

        public GameObject SphereNamed(String name, Vector3 center, float radius, Material material) {
            GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.name = name;
            s.transform.parent = currentParent.transform;
            s.transform.localScale = new Vector3(2 * radius, 2 * radius, 2 * radius);
            s.transform.localPosition = center;
            ApplyCollider(s);
            ApplyMaterial(s, material);
            ApplyLOD(s);
            return MakeStatic(s);
        }
        public GameObject SphereWithMaterial(Vector3 center, float radius, Material material) =>
            SphereNamed("Sphere", center, radius, material);
        public GameObject Sphere(Vector3 center, float radius) =>
            SphereWithMaterial(center, radius, currentMaterial);
        public Vector3 SphereCenter(GameObject s) => s.transform.localPosition;
        public float SphereRadius(GameObject s) => s.transform.localScale.x / 2;

        public GameObject CylinderNamed(String name, Vector3 bottom, float radius, Vector3 top, Material material) {
            GameObject s = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            s.name = name;
            float d = Vector3.Distance(bottom, top);
            s.transform.parent = currentParent.transform;
            s.transform.localScale = new Vector3(2 * radius, d / 2, 2 * radius);
            s.transform.localRotation = Quaternion.FromToRotation(Vector3.up, top - bottom);
            s.transform.localPosition = bottom + (top - bottom) / 2;
            ApplyCollider(s);
            ApplyMaterial(s, material);
            ApplyLOD(s);
            return MakeStatic(s);
        }
        public GameObject CylinderWithMaterial(Vector3 bottom, float radius, Vector3 top, Material material) =>
            CylinderNamed("Cylinder", bottom, radius, top, material);
        public GameObject Cylinder(Vector3 bottom, float radius, Vector3 top) =>
            CylinderWithMaterial(bottom, radius, top, currentMaterial);


        public string ShapeType(GameObject s) => s.name;

        // ||||||||||||||||||||||||||| Complex Geometry |||||||||||||||||||||||||||
        public GameObject PyramidNamed(String name, Vector3[] ps, Vector3 q, Material material) {
            ps = ReverseIfNeeded(ps, ps[0] - q);
            GameObject s = new GameObject(name);
            s.transform.parent = currentParent.transform;
            
            Mesh botMesh = CreatePolygonMesh(ps);
            Array.Reverse(ps);
            Mesh exteriorMesh = CreateTrigMesh(ps, q);
            MeshFilter meshFilter = s.AddComponent<MeshFilter>();
            Mesh[] allMeshes = {botMesh, exteriorMesh};
            meshFilter.sharedMesh = CombineMesh(allMeshes);
            s.AddComponent<MeshRenderer>();
            
            ApplyMaterial(s, material);
            ApplyCollider(s);
            ApplyLOD(s);
            return MakeStatic(s);
        }

        public GameObject PyramidWithMaterial(Vector3[] ps, Vector3 q, Material material) =>
	    PyramidNamed("Pyramid", ps, q, material);
        public GameObject Pyramid(Vector3[] ps, Vector3 q) =>
	    PyramidWithMaterial(ps, q, currentMaterial);

        public GameObject PyramidFrustumNamed(String name, Vector3[] ps, Vector3[] qs, Material material) {
            ps = ReverseIfNeeded(ps, qs[0] - ps[0]);
            qs = ReverseIfNeeded(qs, qs[0] - ps[0]);
            GameObject s = new GameObject(name);
            s.transform.parent = currentParent.transform;
            Array.Reverse(ps);
            Mesh botMesh = CreatePolygonMesh(ps);
            Mesh topMesh = CreatePolygonMesh(qs);
            
            Array.Reverse(ps);
            Mesh exteriorMesh = CreateQuadMesh(ps, qs);
            
            MeshFilter meshFilter = s.AddComponent<MeshFilter>();
            Mesh[] allMeshes = { botMesh, topMesh, exteriorMesh };
            meshFilter.sharedMesh = CombineMesh(allMeshes);
            s.AddComponent<MeshRenderer>();
            
            ApplyMaterial(s, material);
            ApplyCollider(s);
            ApplyLOD(s);
            return MakeStatic(s);
        }
        public GameObject PyramidFrustumWithMaterial(Vector3[] ps, Vector3[] qs, Material material) =>
            PyramidFrustumNamed("PyramidFrustum", ps, qs, material);
        public GameObject PyramidFrustum(Vector3[] ps, Vector3[] qs) =>
            PyramidFrustumWithMaterial(ps, qs, currentMaterial);

        public GameObject ExtrudeContour(Vector3[] contour, Vector3[][] holes, Vector3 v, Material material) {
            contour = ReverseIfNeeded(contour, -v);
            GameObject s = new GameObject("Slab");
            s.transform.parent = currentParent.transform;
            
            Mesh botMesh = CreatePolygonMeshWithHoles(contour.ToArray(), holes);
            
            Mesh topMesh = new Mesh();
            topMesh.vertices = botMesh.vertices.Select(e => e + v).ToArray();
            topMesh.triangles = botMesh.triangles.Reverse().ToArray();
            topMesh.RecalculateNormals();
            topMesh.RecalculateBounds();
            
            Vector3[] topContour = contour.Select(e => e + v).ToArray();
            Mesh exteriorMesh = CreateQuadMesh(topContour, contour);

            List<Mesh> holeMeshes = new List<Mesh>();
            foreach (Vector3[] hole in holes) {
                Vector3[] topHole = hole.Select(e => e + v).ToArray();
                holeMeshes.Add(CreateQuadMesh(hole.ToArray(), topHole));
            }

            List<Mesh> allMeshes = new List<Mesh>(holeMeshes);
            allMeshes.Add(topMesh);
            allMeshes.Add(botMesh);
            allMeshes.Add(exteriorMesh);
            
            MeshFilter meshFilter = s.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = CombineMesh(allMeshes.ToArray());
            s.AddComponent<MeshRenderer>();
            
            ApplyMaterial(s, material);
            ApplyCollider(s);
            ApplyLOD(s);
            return MakeStatic(s);
        }

        public GameObject SurfaceFromGridNamed(string name, int m, int n, Vector3[] pts, bool closedM, bool closedN, int level, Material material) {
            GameObject s = new GameObject(name);
            s.transform.parent = currentParent.transform;
            s.AddComponent<MeshRenderer>();
            MeshFilter filter = s.AddComponent<MeshFilter>();
            Vector3[] vertices = pts;
            int[] triangles = new int[pts.Length * 2 * 3];
            int k = 0;
            int rm = closedM ? m : m - 1;
            int rn = closedN ? n : n - 1;
            for (int i = 0; i < rm; i++) {
                for (int j = 0; j < rn; j++) {
                    int i11 = i * n + j;
                    int i12 = i * n + (j + 1) % n;
                    int i22 = ((i + 1) % m) * n + (j + 1) % n;
                    int i21 = ((i + 1) % m) * n + j;
                    triangles[k++] = i11;
                    triangles[k++] = i22;
                    triangles[k++] = i12;
                    triangles[k++] = i11;
                    triangles[k++] = i21;
                    triangles[k++] = i22;
                }
            }
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.Optimize();
            //Unwrapping.GenerateSecondaryUVSet(mesh); 
            filter.sharedMesh = mesh;
            ApplyMaterial(s, material);
            ApplyCollider(s, mesh);
            ApplyLOD(s);
            return MakeStatic(s);
        }
        public GameObject SurfaceFromGridWithMaterial(int m, int n, Vector3[] pts, bool closedM, bool closedN, int level, Material material) =>
            SurfaceFromGridNamed("SurfaceGrid", m, n, pts, closedM, closedN, level, material);
        public GameObject SurfaceFromGrid(int m, int n, Vector3[] pts, bool closedM, bool closedN, int level) =>
            SurfaceFromGridWithMaterial(m, n, pts, closedM, closedN, level, currentMaterial);


        public void CreateTerrain() {
            GameObject terrain = new GameObject("Terrain");
            terrain.transform.parent = currentParent.transform;
            TerrainData terrainData = new TerrainData();
            terrainData.size = new Vector3(10, 600, 10);
            terrainData.heightmapResolution = 512;
            terrainData.baseMapResolution = 1024;
            terrainData.SetDetailResolution(1024, 16);

            TerrainCollider collider = terrain.AddComponent<TerrainCollider>();
            Terrain terrain_aux = terrain.AddComponent<Terrain>();
            collider.terrainData = terrainData;
            terrain_aux.terrainData = terrainData;
            MakeStatic(terrain);
        }
        
        // Text
        public GameObject Text(string txt, Vector3 pos, Vector3 vx, Vector3 vy, string fontName, int fontSize) {
            GameObject s = new GameObject("Text");
            TextMesh textMesh = s.AddComponent<TextMesh>();
            textMesh.anchor = TextAnchor.LowerLeft;
            //            textMesh.font = (Font)Resources.GetBuiltinResource(typeof(Font), fontName);
            textMesh.font = Resources.Load<Font>(fontName);
            textMesh.fontSize = 100 * fontSize;
            textMesh.text = txt;
            s.transform.parent = currentParent.transform;
            s.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            s.transform.localRotation = Quaternion.LookRotation(vx, vy);
            s.transform.localPosition = pos;
            return s;
        }
        
        // Mesh Combination
        public GameObject Canonicalize(GameObject s) {
            MeshFilter[] meshFilters = s.GetComponentsInChildren<MeshFilter>();
            if (meshFilters.Length == 0) {
                return s;
            }
            Dictionary<Material, List<MeshFilter>> materialMeshFilters = new Dictionary<Material, List<MeshFilter>>();
            foreach (MeshFilter meshFilter in meshFilters) {
                Material[] materials = meshFilter.GetComponent<MeshRenderer>().sharedMaterials;
                if (materials != null) {
                    if (materials.Length > 1 || materials[0] == null) {
                        return s;
                    } else if (materialMeshFilters.ContainsKey(materials[0])) {
                        materialMeshFilters[materials[0]].Add(meshFilter);
                    } else {
                        materialMeshFilters.Add(materials[0], new List<MeshFilter>() { meshFilter });
                    }
                }
            }
            if (materialMeshFilters.Count == 0) {
                return s;
            } else {
                List<GameObject> combinedObjects = new List<GameObject>();
                foreach (KeyValuePair<Material, List<MeshFilter>> entry in materialMeshFilters) {
                    Material material = entry.Key;
                    List<MeshFilter> meshes = entry.Value;
                    string materialName = material.name; // ToString().Split(' ')[0];
                    CombineInstance[] combine = new CombineInstance[meshes.Count];
                    for (int i = 0; i < meshes.Count; i++) {
                        combine[i].mesh = meshes[i].sharedMesh;
                        combine[i].transform = meshes[i].transform.localToWorldMatrix;
                    }
                    Mesh combinedMesh = new Mesh();
                    combinedMesh.CombineMeshes(combine);
                    combinedMesh.Optimize();
                    //Unwrapping.GenerateSecondaryUVSet(combinedMesh); 
                    GameObject combinedObject = new GameObject(materialName);
                    MeshFilter filter = combinedObject.AddComponent<MeshFilter>();
                    filter.sharedMesh = combinedMesh;
                    MeshRenderer renderer = combinedObject.AddComponent<MeshRenderer>();
                    renderer.sharedMaterial = material;
                    combinedObjects.Add(combinedObject);
                }
                //remove old children
                DeleteAllInParent(s);
                //Add new ones
                foreach (GameObject combinedObject in combinedObjects) {
                    combinedObject.transform.parent = s.transform;
                }
                return s;
            }
        }
        
        // ||||||||||||||||||||||||||| Blocks |||||||||||||||||||||||||||
        //We could use Prefabs for this but they can only be used with the Unity Editor and I'm not convinced we want to depend on it.

        //Creating instances
        public GameObject CreateBlockInstance(GameObject block, Vector3 position, Vector3 vx, Vector3 vy, float scale) {
            GameObject obj = InstantiateFamily(block, position, vx, vy, scale);
            obj.SetActive(true);
            return MakeStatic(obj);
            //return obj;
        }

        //Creating blocks
        public GameObject CreateBlockFromFunc(String name, Func<List<GameObject>> f) =>
            CreateBlockFromShapes(name, f().ToArray());

        public GameObject CreateBlockFromShapes(String name, GameObject[] objs) {
            GameObject block = new GameObject(name);
            block.SetActive(false);
            foreach (GameObject child in objs) {
                child.transform.parent = block.transform;
            }
            return block;
        }
        
        // ||||||||||||||||||||||||||| BIM |||||||||||||||||||||||||||
        public GameObject InstantiateFamily(GameObject family, Vector3 pos, Vector3 vx, Vector3 vy, float scale) {
            Quaternion rotation = Quaternion.LookRotation(vx, vy);
            GameObject s = GameObject.Instantiate(family);
            s.transform.parent = currentParent.transform;
            s.transform.localRotation = rotation * s.transform.localRotation;
            s.transform.localPosition = pos;
            s.transform.localScale *= scale;
            return MakeStatic(s);
        }

        public GameObject InstantiateBIMElement(GameObject family, Vector3 pos, float angle) {
            GameObject s = GameObject.Instantiate(family);
            s.transform.parent = currentParent.transform;
            s.transform.localRotation = Quaternion.Euler(0, Mathf.Rad2Deg * angle, 0) * s.transform.localRotation;
            s.transform.localPosition += pos;
            return MakeStatic(s);
        }

        public GameObject Slab(Vector3[] contour, Vector3[][] holes, float h, Material material) =>
            ExtrudeContour(contour, holes, new Vector3(0, h, 0), material);

        public GameObject BeamRectSection(Vector3 position, Vector3 vx, Vector3 vy, float dx, float dy, float dz, float angle, Material material) =>
            RightCuboidNamed("Beam", position, vx, vy, dx, dy, dz, angle, material);

        public GameObject BeamCircSection(Vector3 bot, float radius, Vector3 top, Material material) =>
            CylinderNamed("Beam", bot, radius, top, material);

        public GameObject Panel(Vector3[] pts, Vector3 n, Material material) {
            Vector3[] bps = ReverseIfNeeded(pts, -n);
            Vector3[] tps = bps.Select(p => p + n).Reverse().ToArray();
            GameObject s = new GameObject("Panel");
            s.transform.parent = currentParent.transform;
            Mesh botMesh = CreatePolygonMesh(bps);
            Mesh topMesh = CreatePolygonMesh(tps);
            Array.Reverse(bps);
            Mesh sideMesh = CreateQuadMesh(bps, tps);
            MeshFilter meshFilter = s.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = CombineMesh(new Mesh[] { botMesh, topMesh, sideMesh });
            s.AddComponent<MeshRenderer>();
            ApplyMaterial(s, material);
            ApplyCollider(s);
            ApplyLOD(s);
            return MakeStatic(s);
        }
        
        // ||||||||||||||||||||||||||| View operations |||||||||||||||||||||||||||
        public void SetView(Vector3 position, Vector3 target, float lens) {
            playerTransform.position = position - mainCamera.gameObject.transform.localPosition;
            //mainCameraTransform.rotation = Quaternion.FromToRotation(mainCameraTransform.forward, target - position);
            mainCamera.transform.LookAt(target);
            mainCamera.focalLength = lens;
            Canvas.ForceUpdateCanvases();
        }
        public Vector3 ViewCamera() => mainCamera.transform.position;
        public Vector3 ViewTarget() => mainCamera.transform.position + mainCamera.transform.forward;
        public float ViewLens() => mainCamera.focalLength;

        public void SetResolution(int width, int height) {
            Screen.SetResolution(width, height, false);
        }

        public void ScreenShot(String path) {
            ScreenCapture.CaptureScreenshot(path);
        }
        
        // ||||||||||||||||||||||||||| Object selection |||||||||||||||||||||||||||
        public void SelectGameObjects(GameObject[] objs) {
            //Deselect all
            foreach (GameObject obj in highlighted) {
                Outline outline = obj.GetComponent<Outline>();
                if (outline != null)
                    outline.enabled = false;
            }
            highlighted.Clear();

            foreach (GameObject obj in objs) {
                highlighted.Add(obj);
                Outline outline = obj.GetComponent<Outline>();
                if (outline == null) {
                    outline = obj.AddComponent<Outline>();
                }

                outline.OutlineMode = highlightMode;
                outline.OutlineColor = highlightColor;
                outline.OutlineWidth = highlightWidth;
                outline.enabled = true;
            }
        }

        public void StartSelectingGameObject() {
            SelectedGameObjects.Clear();
            InSelectionProcess = true;
            SelectingManyGameObjects = false;
        }
        public void StartSelectingGameObjects() {
            StartSelectingGameObject();
            SelectingManyGameObjects = true;
        }
        public bool EndedSelectingGameObjects() => InSelectionProcess == false;

        public void ToggleSelectedGameObject(GameObject obj) {
            if (!SelectedGameObjects.Remove(obj)) {
                SelectedGameObjects.Add(obj);
            }
        }

        public int IndexedSelfOrParent(GameObject obj, List<GameObject> objs) {
            int idx = objs.IndexOf(obj);
            if (idx >= 0) {
                return idx;
            } else {
                if (obj.transform == null) {
                    return -1;
                } else {
                    return IndexedSelfOrParent(obj.transform.parent.gameObject, objs);
                }
            }
        }

        public int SelectedGameObjectId(bool existing) {
            if (SelectedGameObjects.Count > 0) {
                List<GameObject> shapes = channel.shapes;
                GameObject obj = SelectedGameObjects[0];
                if (existing) {
                    int idx = IndexedSelfOrParent(obj, shapes);
                    if (idx >= 0) {
                        return idx;
                    } else {
                        return -2;
                    }
                } else {
                    shapes.Add(obj);
                    return shapes.Count - 1;
                }
            } else {
                return -2;
            }
        }
        public int[] SelectedGameObjectsIds(bool existing) {
            List<int> idxs = new List<int>();
            List<GameObject> shapes = channel.shapes;
            foreach (GameObject obj in SelectedGameObjects) {
                if (existing) {
                    int idx = IndexedSelfOrParent(obj, shapes);
                    if (idx >= 0) {
                        idxs.Add(idx);
                    }
                } else {
                    shapes.Add(obj);
                    idxs.Add(shapes.Count - 1);
                }
            }
            return idxs.ToArray();
        }
        /**************************** VRAD *********************************/
        // maybe change place
        //string comparison sucks,change
        public string CallJulia()
        {
            if (poolFunctions.Count == 0)
                return "empty";
            return poolFunctions.Dequeue();
            //return "true";
        }

        public static void AddToQueue(string function)
        {
            poolFunctions.Enqueue(function);
        }

        public void EnableVRAD()
        {
            //VradHandler = GameObject.AddComponent<VRADHandler>();
            //TODO: Create object editing
            GameObject editing = GameObject.Find("Editing");
            editing.SetActive(true);
            VradHandler = (VRADHandler) editing.GetComponent(typeof(VRADHandler));
        }

        public void ChangeState(int state)
        {
            if (VradHandler == null)
            {
                GameObject editing = GameObject.Find("Editing");
                editing.SetActive(true);
                VradHandler = (VRADHandler) editing.GetComponent(typeof(VRADHandler));
            }
            VradHandler.ChangeState(state);
        }

        public void StoreFamilyData(string[] data)
        {
            VradHandler.SetFamilyData(data);
        }

        public void OpenAddPanel()
        {
            VradHandler.OpenAddPanel();
        }

        public void StoreGlobalVariables(string[] data)
        {
            VradHandler.OpenVariablePanel(data);
        }
        #endregion
        
        
    }
}