using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SphereEditPanel : MonoBehaviour
{
    public Slider sliderX;
    public Slider sliderY;
    public Slider sliderZ;
    public Slider sliderRadius;

    public GameObject selected;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        selected = KhepriUnity.Primitives.SelectedGameObjects[0];
        setInitialValues(sliderX, selected.transform.localPosition.x, 50, -50);
        setInitialValues(sliderY, selected.transform.localPosition.y, 50, -50);
        setInitialValues(sliderZ, selected.transform.localPosition.z, 50, -50);
        setInitialValues(sliderRadius, (selected.transform.localScale.x / 2), 10, 0);
    }

    void setInitialValues(Slider s, float initial, float max, float min)
    {
        s.value = initial;
        s.maxValue = max;
        s.minValue = min;
    }

    //TODO: move the functions below to another script
    public void AdjustValueRadius(float value)
    {
        selected.transform.localScale = new Vector3(value * 2, value * 2, value * 2);
    }
    
    public void AdjustValueX(float value)
    {
        selected.transform.position = new Vector3(value, selected.transform.localPosition.y, selected.transform.localPosition.z);
    }
    
    public void AdjustValueY(float value)
    {
        selected.transform.position = new Vector3(selected.transform.localPosition.x, value, selected.transform.localPosition.z);

    }
    
    public void AdjustValueZ(float value)
    {
        selected.transform.position = new Vector3(selected.transform.localPosition.x, selected.transform.localPosition.y, value);

    }
}
