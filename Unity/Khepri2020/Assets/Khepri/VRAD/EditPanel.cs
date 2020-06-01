using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KhepriUnity;
public class EditPanel : MonoBehaviour
{
    public Text type = null;

    private GameObject currentPanel = null;
    public GameObject panelSphere;
    public GameObject panelBox;
    private GameObject currentObject;
    
    // Start is called before the first frame update
    void Start()
    {
        //panel = GameObject.Find("Editing/Canvas2D/Panel_Properties");
        // positionX = GameObject.Find("Editing/Canvas2D/Panel_Properties/PositionX");
        //panel.SetActive(false);
        panelBox.SetActive(false);
        panelSphere.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        currentObject = KhepriUnity.Primitives.SelectedGameObjects[0];
        UpdatePanel();
    }

    void OnDisable()
    {
        currentPanel.SetActive(false);
    }
    public void UpdatePanel()
    {
        String objectType = currentObject.name;
        type.text = objectType;
        //TODO: dynamic generation?
        //TODO: get info from Julia and not Unity
        //Primitives.AddToQueue("getShapeInfo");
        if (objectType.Equals("Sphere"))
        {
            currentPanel = panelSphere;
        }

        if (objectType.Equals("Box"))
        {
            currentPanel = panelBox;
        }
        currentPanel.SetActive(true);
    }
    
}