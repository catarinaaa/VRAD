using System.Collections.Generic;
using UnityEngine;

public class StaticOverview : MonoBehaviour {

    private class Trans {
        public Vector3 position = Vector3.zero;
        public Quaternion rotation  = Quaternion.identity;
    }
    private List<Trans> overviewsList = new List<Trans>();
    private Transform thisTransform;
    private Transform cameraTransform;
    private Movement movementScript;
    
    void Start() {
        thisTransform = transform;
        cameraTransform = Camera.main.transform;
        movementScript = GameObject.FindWithTag("Player").GetComponent<Movement>();
        
        for (int i = 0; i < 10; i++)
            overviewsList.Add(new Trans());
        
        // FIXME 
        overviewsList[0] = new Trans()
            {position = new Vector3(-91, 126, 79), rotation = Quaternion.Euler(39f, 135f, 0f)};
        overviewsList[9] = new Trans()
            {position = new Vector3(-56, 19.5f, 15.7f), rotation = Quaternion.Euler(9, 122, 0f)};
    }
    
    void Update() {
        if (movementScript.GetCursorMode())
            return;
        
        if (Input.GetKey(KeyCode.LeftShift)) {
            if (Input.GetKeyDown(KeyCode.Alpha0)) {
                Debug.Log("Overview position bound to 0.");
                overviewsList[0].position = thisTransform.position;
                overviewsList[0].rotation = cameraTransform.rotation;
            } else if (Input.GetKeyDown(KeyCode.Alpha1)) {
                Debug.Log("Overview position bound to 1.");
                overviewsList[1].position = thisTransform.position;
                overviewsList[1].rotation = cameraTransform.rotation;
            } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
                Debug.Log("Overview position bound to 2.");
                overviewsList[2].position = thisTransform.position;
                overviewsList[2].rotation = cameraTransform.rotation;
            } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
                Debug.Log("Overview position bound to 3.");
                overviewsList[3].position = thisTransform.position;
                overviewsList[3].rotation = cameraTransform.rotation;
            } else if (Input.GetKeyDown(KeyCode.Alpha4)) {
                Debug.Log("Overview position bound to 4.");
                overviewsList[4].position = thisTransform.position;
                overviewsList[4].rotation = cameraTransform.rotation;
            } else if (Input.GetKeyDown(KeyCode.Alpha5)) {
                Debug.Log("Overview position bound to 5.");
                overviewsList[5].position = thisTransform.position;
                overviewsList[5].rotation = cameraTransform.rotation;
            } else if (Input.GetKeyDown(KeyCode.Alpha6)) {
                Debug.Log("Overview position bound to 6.");
                overviewsList[6].position = thisTransform.position;
                overviewsList[6].rotation = cameraTransform.rotation;
            } else if (Input.GetKeyDown(KeyCode.Alpha7)) {
                Debug.Log("Overview position bound to 7.");
                overviewsList[7].position = thisTransform.position;
                overviewsList[7].rotation = cameraTransform.rotation;
            } else if (Input.GetKeyDown(KeyCode.Alpha8)) {
                Debug.Log("Overview position bound to 8.");
                overviewsList[8].position = thisTransform.position;
                overviewsList[8].rotation = cameraTransform.rotation;
            } else if (Input.GetKeyDown(KeyCode.Alpha9)) {
                Debug.Log("Overview position bound to 9.");
                overviewsList[9].position = thisTransform.position;
                overviewsList[9].rotation = cameraTransform.rotation;
            }
        }

        else {
            if (Input.GetKeyUp(KeyCode.Alpha0)) {
                thisTransform.position = overviewsList[0].position;
                cameraTransform.rotation = overviewsList[0].rotation;
            } else if (Input.GetKeyUp(KeyCode.Alpha1)) {
                thisTransform.position = overviewsList[1].position;
                cameraTransform.rotation = overviewsList[1].rotation;
            } else if (Input.GetKeyUp(KeyCode.Alpha2)) {
                thisTransform.position = overviewsList[2].position;
                cameraTransform.rotation = overviewsList[2].rotation;
            } else if (Input.GetKeyUp(KeyCode.Alpha3)) {
                thisTransform.position = overviewsList[3].position;
                cameraTransform.rotation = overviewsList[3].rotation;
            } else if (Input.GetKeyUp(KeyCode.Alpha4)) {
                thisTransform.position = overviewsList[4].position;
                cameraTransform.rotation = overviewsList[4].rotation;
            } else if (Input.GetKeyUp(KeyCode.Alpha5)) {
                thisTransform.position = overviewsList[5].position;
                cameraTransform.rotation = overviewsList[5].rotation;
            } else if (Input.GetKeyUp(KeyCode.Alpha6)) {
                thisTransform.position = overviewsList[6].position;
                cameraTransform.rotation = overviewsList[6].rotation;
            } else if (Input.GetKeyUp(KeyCode.Alpha7)) {
                thisTransform.position = overviewsList[7].position;
                cameraTransform.rotation = overviewsList[7].rotation;
            } else if (Input.GetKeyUp(KeyCode.Alpha8)) {
                thisTransform.position = overviewsList[8].position;
                cameraTransform.rotation = overviewsList[8].rotation;
            } else if (Input.GetKeyUp(KeyCode.Alpha9)) {
                thisTransform.position = overviewsList[9].position;
                cameraTransform.rotation = overviewsList[9].rotation;
            }
        }
    }
}
