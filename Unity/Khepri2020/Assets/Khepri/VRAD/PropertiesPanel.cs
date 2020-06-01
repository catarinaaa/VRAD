using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KhepriUnity;
public class PropertiesPanel : MonoBehaviour
{
    public Text type = null;
    public Text positionX = null;
    public Text positionY  = null;
    public Text positionZ  = null;
    public GameObject panel = null;

    private GameObject currentPanel = null;
    public GameObject panelSphere;
    public GameObject panelBox;
    
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
        UpdatePanel();
    }

    void OnDisable()
    {
        currentPanel.SetActive(false);
    }
    public void UpdatePanel()
    {
        String objectType = KhepriUnity.Primitives.SelectedGameObjects[0].name;
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