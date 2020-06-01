using UnityEngine;
using Valve.VR;

public class VRMovement : MonoBehaviour, Movement {
    private float flySpeed = 2f;
    private float walkSpeed = 1f;
    private float lookSpeed = 10f;
    private float gravityMultiplier = -1.05f;
    private float maxFallSpeed = -6f;
    private float jumpHeight = 2f;
    private float playerHeight = 2f;
    private float slopeLimit = 45f;
    private float stepOffset = 0.3f;
    private float playerRadius = 0.5f;

    private float laserWidth = 0.002f;
    private Color laserColor = Color.red;
    public GameObject leftLaserPointer;
    private GameObject leftLaserBeam;
    public GameObject rightLaserPointer;
    private GameObject rightLaserBeam;
    private Material laserMaterial;
    
    public SteamVR_Action_Vector2 moveValue;
    public SteamVR_Action_Vector2 cameraValue;
    public SteamVR_Action_Boolean jumpValue;
    public SteamVR_Action_Boolean downValue;
    public SteamVR_Action_Boolean boostValue;
    public SteamVR_Action_Boolean leftToogleSelectValue;
    public SteamVR_Action_Boolean rightToogleSelectValue;
    public SteamVR_Action_Boolean changeModeValue;
    public SteamVR_Action_Boolean leftToogleLaserValue;
    public SteamVR_Action_Boolean rightToogleLaserValue;

    private float verticalSpeed = 1;
    private CharacterController characterController;
    private Transform cameraRig;
    private Transform head;
    private Mode playerMode = Mode.FreeFly;
    private bool cursorMode = true;

    private void Awake() {
        characterController = GetComponent<CharacterController>();
        characterController.slopeLimit = slopeLimit;
        characterController.stepOffset = stepOffset;
        characterController.radius = playerRadius;
        moveValue.actionSet.Activate();
        leftLaserBeam = leftLaserPointer.transform.GetChild(0).gameObject;
        rightLaserBeam = rightLaserPointer.transform.GetChild(0).gameObject;
        laserMaterial = leftLaserBeam.GetComponent<Renderer>().sharedMaterial;
        leftLaserBeam.SetActive(false);
        rightLaserBeam.SetActive(false);
        UpdateLaserSettings(laserWidth, laserColor);
    }

    private void Start() {
        cameraRig = SteamVR_Render.Top().origin;
        head = SteamVR_Render.Top().head;
    }

    private void Update() {
        KhepriUnity.Primitives khepri = KhepriUnity.Primitives.Instance;

        HandleHead();
        HandleHeight();
        CalculateMovement();

        rightLaserBeam.SetActive(rightToogleLaserValue.state);
        leftLaserBeam.SetActive(leftToogleLaserValue.state);

        if (khepri.InSelectionProcess) {
            if (Input.GetMouseButtonDown(0)) {
                ToggleSelectedObject(khepri, Camera.main.ScreenPointToRay(Input.mousePosition));
            } else if (leftToogleLaserValue.state && leftToogleSelectValue.state) {
                Vector3 origin = leftLaserPointer.transform.position + (0.1f * leftLaserPointer.transform.forward);
                Vector3 direction = leftLaserPointer.transform.forward;
                ToggleSelectedObject(khepri, new Ray(origin, direction));
            } else if (rightToogleLaserValue.state && rightToogleSelectValue.state) {
                Vector3 origin = rightLaserPointer.transform.position + (0.1f * rightLaserPointer.transform.forward);
                Vector3 direction = rightLaserPointer.transform.forward;
                ToggleSelectedObject(khepri, new Ray(origin, direction));
            }
        }
        
        if (changeModeValue.stateDown || changeModeValue.stateUp || (!cursorMode && Input.GetKeyUp(KeyCode.M))) {
            SwitchPlayerMode();
        }
        
        if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.Mouse1)) 
            ToggleCursorMode();
        
        if (!cursorMode && Input.GetKeyUp(KeyCode.K)) 
            ToggleKhepri();
        
        if (!cursorMode && Input.GetKeyUp(KeyCode.Q)) 
            Application.Quit();
    }

    private void HandleHead() {
        // Store current rotation
        Vector3 oldPosition = cameraRig.position;
        Quaternion oldRotation = cameraRig.rotation;
        
        // Rotate Character Controller
        transform.eulerAngles = new Vector3(0f, head.rotation.eulerAngles.y, 0f);

        // Restore position
        cameraRig.position = oldPosition;
        cameraRig.rotation = oldRotation;
    }

    private void CalculateMovement() {
        float delta = Time.deltaTime * 10;
        float speedMul = boostValue.state || (!cursorMode && Input.GetKey(KeyCode.LeftShift)) ? 4 : 1;
        Vector3 movement = Vector3.zero;
        float rotationY = 0f; //((transform.eulerAngles.y + 90) % 360) - 90;
        if (!cursorMode)
            rotationY += Input.GetAxis("Mouse X") * lookSpeed * delta; // Mouse rotation
        rotationY += cameraValue.axis.x * lookSpeed * delta; // VR controller rotation

        switch (playerMode) {
            case Mode.FreeFly: {
                movement = flySpeed * speedMul * delta *
                           (transform.right * moveValue.axis.x + transform.forward * moveValue.axis.y); // VR controller movement
                if (!cursorMode)
                    movement += flySpeed * speedMul * delta *
                                (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical")); // Keyboard movement
            
                transform.position += movement;

                if (jumpValue.state || (!cursorMode && Input.GetKey(KeyCode.Space))) 
                    transform.position += flySpeed * speedMul * delta * Vector3.up;
					
                if (downValue.state || (!cursorMode && Input.GetKey(KeyCode.LeftAlt)))
                    transform.position -= flySpeed * speedMul * delta * Vector3.up;
                break;
            }
            case Mode.Walk: {
                movement = walkSpeed * speedMul * delta * 
                           (transform.right * moveValue.axis.x + transform.forward * moveValue.axis.y);
                
                if (!cursorMode)
                    movement +=  walkSpeed * speedMul * delta * 
                                 (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical"));
               
                if (characterController.isGrounded && (Input.GetKey(KeyCode.Space) || jumpValue.state))
                    verticalSpeed = jumpHeight;
                else if (characterController.isGrounded)
                    verticalSpeed = 0;
                else {
                    verticalSpeed += gravityMultiplier * delta;
                    verticalSpeed = Mathf.Clamp(verticalSpeed, maxFallSpeed, jumpHeight * 2);
                }
                Vector3 gravityVector = new Vector3(0, verticalSpeed, 0);
                movement += gravityVector * delta;
            
                characterController.Move(movement);
                break;
            }
        }
        
        transform.RotateAround(head.position, Vector3.up, rotationY);
    }

    private void HandleHeight() {
        // Input the correct height of the character controller according to the height of the player
        float headHeight = Mathf.Clamp(head.localPosition.y, 1, 2);
        characterController.height = headHeight;
        
        RecenterCharacterController();
    }

    private void RecenterCharacterController() {
        // Recenter the character controller (because pivot is in the middle and not on the top)
        Vector3 center = Vector3.zero;
        center.y = characterController.height / 2;
        center.y += characterController.skinWidth;

        // Move and rotate capsule (bounding box) according to the headset position
        //center.x = head.localPosition.x;
        //center.z = head.localPosition.z;
        center.x = head.position.x - transform.position.x;
        center.z = head.position.z - transform.position.z;

        center = Quaternion.Euler(0, -transform.eulerAngles.y, 0) * center;
        characterController.center = center;
    }
    
    void SwitchPlayerMode() {
        switch (playerMode) {
            case Mode.FreeFly:
                playerMode = Mode.Walk;
                break;
            case Mode.Walk:
                playerMode = Mode.FreeFly;
                break;
        }
    }
    
    void ToggleCursorMode() {
        if (Cursor.lockState == CursorLockMode.Locked) {
            Cursor.lockState = CursorLockMode.None;
            cursorMode = true;
            Cursor.visible = true;
        } else {
            Cursor.lockState = CursorLockMode.Locked;
            cursorMode = false;
            Cursor.visible = false;
        }
    }

    void ToggleSelectedObject(KhepriUnity.Primitives khepri, Ray ray) {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            khepri.ToggleSelectedGameObject(hit.collider.gameObject);
            if (!khepri.SelectingManyGameObjects) {
                khepri.InSelectionProcess = false;
            }
        }
    }

    void ToggleKhepri() {
        SceneLoad.visualizing = ! SceneLoad.visualizing;
    }

    public void UpdateMovementSettings(float flySpeed, float walkSpeed, float lookSpeed, float gravityMultiplier,
        float maxFallSpeed, float playerRadius) {
        this.flySpeed = flySpeed;
        this.walkSpeed = walkSpeed;
        this.lookSpeed = lookSpeed;
        this.gravityMultiplier = gravityMultiplier;
        this.maxFallSpeed = maxFallSpeed;
        this.playerRadius = playerRadius;

        if (characterController != null)
            characterController.radius = playerRadius;
    }

    public bool GetCursorMode() {
        return cursorMode;
    }

    public void UpdateLaserSettings(float width, Color color) {
        laserWidth = width;
        laserColor = color;
        
        if (leftLaserBeam != null)
            leftLaserBeam.transform.localScale = new Vector3(width, width, leftLaserBeam.transform.localScale.z);
        if (rightLaserBeam != null)
            rightLaserBeam.transform.localScale = new Vector3(width, width, rightLaserBeam.transform.localScale.z);
        if (laserMaterial != null)
            laserMaterial.color = color;
    }

    public float PlayerHeight() => playerHeight;
}
