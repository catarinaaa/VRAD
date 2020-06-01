using System.Collections;
using System.Collections.Generic;
using KhepriUnity;
using UnityEngine;
using UnityEngine.UI;
using UnityScript.Lang;
using System;
using System.Linq;
using Array = System.Array;
using System.IO;
using UnityEditorInternal;

public class FamilyPanel : MonoBehaviour
{
    private bool on = false;
    
    public Text type = null;
    public Dropdown drop = null;
    public Dropdown material = null;
    public List<string> funcNames = null;
    public List<string> materials = null;
    public Dictionary<string,string> materialsDict = null;
    
    private string currentMaterial = null;
    private string funcToSave = null;
    private bool _updateMaterial = false;
    
    public GameObject currentPanel = null;
    public GameObject slab = null;
    public GameObject beam = null;
    public GameObject wall = null;
    public GameObject door = null;

    void Start(){}
    void Update() {}
    void onReset()
    {
        on = false;
        funcNames = null;
        funcToSave = null;
        materials = null;
        currentMaterial = null;
        materialsDict = null;
        _updateMaterial = false;
        CloseAllPanels();
    }
    void OnEnable()
    {
        on = true;
        //currentPanel.SetActive(true);
    }

    void OnDisable()
    {
        onReset();
    }

    public void StartPanel(string[] data)
    {
        CloseAllPanels();
        type.text = data[0];
        string[] funcData = null;
        switch (data[0])
        {
            //case "RoofFamily":
            case "SlabFamily":
                currentPanel = slab;
                currentPanel.SetActive(true);
                SlabPanel script1 = (SlabPanel) currentPanel.GetComponent(typeof(SlabPanel));
                script1.StartPanel(data);
                funcData = new string[data.Length - 5];
                Array.Copy(data, 5, funcData, 0, data.Length - 5);
                break;
            //case "PanelFamily":
            case "WallFamily":
                print("Opening wall panel");
                currentPanel = wall;
                currentPanel.SetActive(true);
                WallPanel script2 = (WallPanel) currentPanel.GetComponent(typeof(WallPanel));
                script2.StartPanel(data);
                funcData = new string[data.Length - 3];
                Array.Copy(data, 3, funcData, 0, data.Length - 3);
                break;
            //case "WindowFamily":
            case "DoorFamily":
                currentPanel = door;
                currentPanel.SetActive(true);
                DoorPanel script3 = (DoorPanel) currentPanel.GetComponent(typeof(DoorPanel));
                script3.StartPanel(data);
                funcData = new string[data.Length - 7];
                Array.Copy(data, 7, funcData, 0, data.Length - 7);

                break;
            case "BeamFamily":
                print("Not implemented yet");
                break;
            default:
                print("No family found");
                break;
        }

        funcNames = new List<string>(funcData);
        drop.ClearOptions();
        drop.AddOptions(funcNames);
        ChangeFunctionSaved(0);
        drop.onValueChanged.AddListener(ChangeFunctionSaved);
        startMaterials();
        
    }

    private void CloseAllPanels()
    {
        slab.SetActive(false);
        wall.SetActive(false);
        beam.SetActive(false);
        door.SetActive(false);
        //add more
    }

    public string[] GetCurrentValues()
    {
        string[] values = null;
        if (currentPanel.name.Equals("SlabPanel"))
        {
            SlabPanel script = (SlabPanel) currentPanel.GetComponent(typeof(SlabPanel));
            values = script.GetCurrentValues();
            script.Reset();
        }
        if (currentPanel.name.Equals("WallPanel"))
        {
            WallPanel script = (WallPanel) currentPanel.GetComponent(typeof(WallPanel));
            values = script.GetCurrentValues();
            script.Reset();
        }

        if (currentPanel.name.Equals("DoorPanel"))
        {
            DoorPanel script = (DoorPanel) currentPanel.GetComponent(typeof(DoorPanel));
            values = script.GetCurrentValues();
            script.Reset();
        }

/*        if (currentPanel.name.Equals("BeamPanel"))
        {
            BeamPanel script = (BeamPanel) currentPanel.GetComponent(typeof(BeamPanel));
            values = script.GetCurrentValues();
            script.Reset();
        }*/

        return values;
    }

    public void Save()
    {
        string[] values = GetCurrentValues();
        string result = "updateFamilyValues";
        foreach (string s in values)
        {
            result += "#" + s.Replace(",", ".");
        }

        if (_updateMaterial) result += "#material#" + materialsDict[currentMaterial].Replace(Directory.GetCurrentDirectory() + "\\Assets\\Resources\\", "").Replace("\\", "/").Replace(".mat", "");
        _updateMaterial = false;
        result += "#" + funcToSave;
        print("Sent: " + result);
        Primitives.AddToQueue(result);
    }

    // Function for dropdown menu
    public void ChangeFunctionSaved(int value)
    {
        funcToSave = funcNames[value];
    }
    
    public void ChangeMaterialSaved(int value)
    {
        currentMaterial = materials[value];
    }

    public void updateMaterial(int value)
    {
        _updateMaterial = true;
    }

    public void startMaterials()
    {
        materialsDict = new Dictionary<string,string>();
        string[] paths = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Assets\\Resources", "*.mat", SearchOption.AllDirectories);
        foreach (string path in paths)
        {
            if(!materialsDict.ContainsKey(Path.GetFileNameWithoutExtension(path)))
                materialsDict.Add(Path.GetFileNameWithoutExtension(path), path);
        }

        materials = new List<string>(materialsDict.Keys);
        material.ClearOptions();
        material.AddOptions(materials);
        ChangeMaterialSaved(0);
        material.onValueChanged.AddListener(ChangeMaterialSaved);
        material.onValueChanged.AddListener(updateMaterial);
    }
}
