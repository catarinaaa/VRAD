using System;
using System.Collections;
using System.Collections.Generic;
using KhepriUnity;
using UnityEngine;
using UnityEngine.UI;

public class AddObject : MonoBehaviour
{
    public Camera camera;
    public GameObject created;
    public Slider sliderX;
    public Slider sliderY;
    public Slider sliderZ;

    private int _objectType;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEnable()
    {

        
    }

    public void StartPanel(int obj)
    {
        _objectType = obj;
        if (obj == 0)
        {
           //created = Primitives.PointLight(new Vector3(camera.transform.localPosition.x, camera.transform.localPosition.y, camera.transform.localPosition.z ), Color.white, 4,4);
           created = new GameObject("PointLight");
           Light light = created.AddComponent<Light>();
           created.transform.parent = GameObject.Find("MainObject").transform;
           light.type = LightType.Point;
           light.color = Color.white;
           light.range = 4;         // How far the light is emitted from the center of the object
           light.intensity = 2; // Brightness of the light
           created.transform.localPosition = GameObject.FindWithTag("Player").transform.localPosition;
        }
    }
    
    public void AdjustValueX(float value)
    {
        created.transform.position = new Vector3(value, created.transform.localPosition.y, created.transform.localPosition.z);
    }
    
    public void AdjustValueY(float value)
    {
        created.transform.position = new Vector3(created.transform.localPosition.x, value, created.transform.localPosition.z);

    }
    
    public void AdjustValueZ(float value)
    {
        created.transform.position = new Vector3(created.transform.localPosition.x, created.transform.localPosition.y, value);
    }

    public void Cancel()
    {
        Destroy(created);
    }

    public string getCurrentValues()
    {
        string result = "";
        if (_objectType == 0)
        {
            result = "createLight#" + (created.transform.localPosition.x).ToString().Replace(",", ".") + "#" + (created.transform.localPosition.y).ToString().Replace(",", ".") +
                        "#" + (created.transform.localPosition.z).ToString().Replace(",", ".");
        }
        return result;
    }
}
