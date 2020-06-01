using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Hover.Core.Items.Types;
using UnityEngine;
using UnityEngine.UI;

public class TrussBarPanel : MonoBehaviour
{
 public bool isHover = true;
    public Slider radius;
    public GameObject radiusHover;
    private bool _updateRadius = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void StartPanel(string[] data)
    {
        print("Starting Truss Bar Panel");
        print("Values: " + data[4]);

        if (isHover)
        {
            SetHoverInitialValues(radiusHover, (float) Math.Round(Convert.ToDecimal(data[4], CultureInfo.InvariantCulture), 3),(float) 10.000, (float) 0.100);
        }
        else
        {
            data[4] = Math.Round(Convert.ToDecimal(data[4], CultureInfo.InvariantCulture), 3)
                .ToString("0.000", CultureInfo.InvariantCulture);
            SetInitialValues(radius, (float) Math.Round(Convert.ToDecimal(data[4], CultureInfo.InvariantCulture), 3),
                (float) 10.000, (float) 0.100);
        }

        _updateRadius = false;
    }
    
    public void SetHoverInitialValues(GameObject slider, float initial, float max, float min)
    {
        slider.GetComponent<HoverItemDataSlider>().Value = initial;
        slider.GetComponent<HoverItemDataSlider>().RangeMax = max;
        slider.GetComponent<HoverItemDataSlider>().RangeMin = min;
    }
    
    void SetInitialValues(Slider s, float initial, float max, float min)
    {
        //default rounding value is set to 3
        s.value = initial;
        s.maxValue = max;
        s.minValue = min;
    }
    
    public void Reset()
    {
        _updateRadius = false;
    }
    
    public void UpdateThickness()
    {
        print("New thickness");
        _updateRadius = true;
    }
    
    public string[] GetCurrentValues()
    {
        var values = new List<string>();

        if (!isHover)
        {
            if (_updateRadius)
            {
                values.Add("radius");
                values.Add(radius.value.ToString());
            }
        }
        else
        {
            if (_updateRadius) { values.Add("thickness"); values.Add(radiusHover.GetComponent<HoverItemDataSlider>().Value.ToString()); }
        }
        return values.ToArray();

    }
}
