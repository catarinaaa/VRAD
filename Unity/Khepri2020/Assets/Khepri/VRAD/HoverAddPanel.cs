using System.Collections;
using System.Collections.Generic;
using Hover.Core.Items.Types;
using KhepriUnity;
using UnityEngine;

public class HoverAddPanel : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject main;
    public GameObject activePanel;

    public GameObject created;
    
    void Start()
    { 
       // selected = false;
        main.SetActive(true);
        activePanel = main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    public void returnMain()
    {
        activePanel.SetActive(false);
        main.SetActive(true);
    }

    public void createLight()
    {
        created = new GameObject("PointLight");
        Light light = created.AddComponent<Light>();
        created.transform.parent = GameObject.Find("MainObject").transform;
        light.type = LightType.Point;
        light.color = Color.white;
        light.range = 4;     // How far the light is emitted from the center of the object
        light.intensity = 2; // Brightness of the light
        created.transform.localPosition = GameObject.FindWithTag("Player").transform.localPosition; // TODO: fix
        
        activePanel = gameObject.transform.Find("CreatePointlight").gameObject;
        activePanel.SetActive(true);
        setPositionSlider(activePanel.transform.Find("Position").gameObject);
        activePanel.transform.Find("Intensity").gameObject.GetComponent<HoverItemDataSlider>().OnValueChangedEvent.AddListener(AdjustIntensity);
        activePanel.transform.Find("Range").gameObject.GetComponent<HoverItemDataSlider>().OnValueChangedEvent.AddListener(AdjustRange);

    }

    public void createTable()
    {
        //created = Primitives.InstantiateBIMElement(Resources.Load("Default/Prefabs/Table") as GameObject, new Vector3(0,1,1), 0);
        created = Instantiate(Resources.Load("Default/Prefabs/Table") as GameObject);
        //created.transform.parent = currentParent.transform;
        //created.transform.localRotation = Quaternion.Euler(0, Mathf.Rad2Deg * angle, 0) * s.transform.localRotation;
        //created.transform.localPosition += 0;
    }

    public void createChair()
    {
        created = Instantiate(Resources.Load("Default/Prefabs/Chair") as GameObject);
        //apply outline
    }

    public void createWindow()
    {
        //send to julia select object
        
    }

    public void setPositionSlider(GameObject position)
    {
        position.transform.Find("X").gameObject.GetComponent<HoverItemDataSlider>().OnValueChangedEvent.AddListener(AdjustValueX);
        position.transform.Find("Y").gameObject.GetComponent<HoverItemDataSlider>().OnValueChangedEvent.AddListener(AdjustValueY);
        position.transform.Find("Z").gameObject.GetComponent<HoverItemDataSlider>().OnValueChangedEvent.AddListener(AdjustValueZ);
    }
    
    
    public void AdjustValueX(IItemDataSelectable<float> value)
    {
        created.transform.position = new Vector3(value.Value * 10, created.transform.localPosition.y, created.transform.localPosition.z);
    }
    
    public void AdjustValueY(IItemDataSelectable<float> value)
    {
        created.transform.position = new Vector3(created.transform.localPosition.x, value.Value * 10, created.transform.localPosition.z);

    }
    
    public void AdjustValueZ(IItemDataSelectable<float> value)
    {
        created.transform.position = new Vector3(created.transform.localPosition.x, created.transform.localPosition.y, value.Value * 10);
    }

    public void AdjustIntensity(IItemDataSelectable<float> value)
    {
        created.GetComponent<Light>().intensity = value.Value * 10;
    }

    public void AdjustRange(IItemDataSelectable<float> value)
    {
        created.GetComponent<Light>().range = value.Value * 10;

    }


    
    public void Save()
    {
        string data = "";
        
        Primitives.AddToQueue(data);
       
        Destroy(created); // Removes preview after reload
    }

}


