using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KhepriUnity;
using UnityEngine.UI;

/*
 * Script for testing, enabled VR doesn't really enables VR (I think)
 * Old Khepri version
 */
public class VRADHandler : MonoBehaviour
{
    //STATES
    public enum State
    {
        Idle = 0,
        InSelection = 1,
        Selected = 2,
        Add = 3,
        ChangeVariables = 4
    }
    public State currentState = 0;
    private bool on = false;
    private bool vr = false;
    public GameObject pointer;
    public GameObject pointer2;
    
    public GameObject canvas2D;
    public GameObject canvas3D;
    private GameObject _activeCanvas = null;
    private MenuManager _menuManager;

    private string[] familyData = null;
    //public static GameObject propertiesPanel;
     
    // Start is called before the first frame update
    void Start()
    {
        currentState = State.Idle;
        //set to true if not debugging
        vr = true;
        if (vr)
        {
            _activeCanvas = canvas3D;
            _activeCanvas.SetActive(true);
            _menuManager = (MenuManager) _activeCanvas.GetComponent(typeof(MenuManager));
        }
        else
        {
            _activeCanvas = canvas2D;
            _activeCanvas.SetActive(true);
            _menuManager = (MenuManager) _activeCanvas.GetComponent(typeof(MenuManager));
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Start Editing mode if E pressed
        if (Input.GetKeyDown(KeyCode.E))
        {
            on = !on;
            if (on) ChangeState(1);
            else
            {
                _menuManager.CloseFamilyPanel();
                ChangeState(0);
            }
            print("Editing mode:" + on.ToString());
        } 

        //Start Variables menu
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (currentState == State.Idle)
            {
                print("Variables!!!");
                ChangeState(4);
                Primitives.AddToQueue("sendGlobalVariables");
            }
            else if (currentState == State.ChangeVariables)
            {
                _menuManager.CloseVariablesPanel();
                ChangeState(0);
            }
            
        }
        
        //Start Add Menu
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (currentState == State.Idle)
            {
                print("Objects!!!");
                ChangeState(3); 
                _menuManager.OpenAddPanel();
            }

            else if (currentState == State.Add)
            {
                _menuManager.CloseAddPanel();
                ChangeState(0);
            }
        }
    }
    public void ChangeState(int state)
    {
        if (state == 0)
        {
            StopInSelection();
            currentState = State.Idle;
        }
        if (state == 1)
        {
            StartInSelection();
            currentState = State.InSelection;
        }
        if (state == 2)
        {
            print("Current state: Selected");
            StartSelected();
            currentState = State.Selected;
        }

        if (state == 3)
        {
            currentState = State.Add;
        }

        if (state == 4)
        {
            currentState = State.ChangeVariables;
        }
    }
    
    private void StartInSelection()
    {
        //if (vr) pointer.SetActive(true);
        //else pointer2.SetActive(true);
        //Debug.Log("Editing is on");
        Primitives.AddToQueue("startInSelection");
    }

    private void StopInSelection()
    {
        //if (vr) pointer.SetActive(false);
        //else pointer2.SetActive(false);
        //Debug.Log("Editing is off");
        Primitives.AddToQueue("stopEditingMode");
    }
    

    public void StartSelected()
    {
        print("Selected state is active");
        //open properties panel
        //_menuManager.OpenPropertiesPanel();
        //_menuManager.OpenTransformPanel();
        print("Request family data");
        Primitives.AddToQueue("sendFamilyData");
    }

    public void SetFamilyData(string[] data)
    {
        print("Family data received: " + data[0]);
        familyData = data;
        if (vr)
        {
            _menuManager.OpenHoverFamilyPanel(data);
        }
        else
        {
            _menuManager.OpenFamilyPanel(data);
        }
    }

    
    public void OpenAddPanel()
    {
        Transform playerTransform = GameObject.FindWithTag("Player").transform;
        canvas2D.transform.position = new Vector3(playerTransform.transform.position.x+(float)1.8,playerTransform.transform.position.y,playerTransform.transform.position.z+1);
        canvas2D.transform.localScale = new Vector3((float) 0.001, (float) 0.001, (float) 0.001);
        _menuManager.OpenAddPanel();
    }

    public void OpenVariablePanel(string[] data)
    {
        _menuManager.OpenVariablesPanel(data);
    }
}
