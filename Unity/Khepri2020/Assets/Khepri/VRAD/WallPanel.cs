using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Hover.Core.Items.Types;
using UnityEngine;
using UnityEngine.UI;

public class WallPanel : MonoBehaviour
{
    
    public Slider thickness;
    public GameObject thicknessHover;
    public GameObject lthicknessHover;
    public GameObject rthicknessHover;
    
    private bool _updateThickness = false;
    private bool _updateLThickness = false;
    private bool _updateRThickness = false;
    
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
        
    }
    
    public void StartPanel(string[] data)
    {
        print("Starting Wall Panel");
        print("Values: " + data[4]);
        
        //thickness.onValueChanged.AddListener();
        data[4] = Math.Round(Convert.ToDecimal(data[4], CultureInfo.InvariantCulture), 3)
            .ToString("0.000", CultureInfo.InvariantCulture);
        setInitialValues(thickness, (float) Math.Round(Convert.ToDecimal(data[4], CultureInfo.InvariantCulture), 3),(float) 10.000, (float) 0.100);
        _updateThickness = false;
        _updateLThickness = false;
        _updateRThickness = false;
    }
    
    public void StartHoverPanel(string[] data)
    {
        print("Starting Wall Panel");
        print("Values: " + data[2]);
        
        //data[4] = Math.Round(Convert.ToDecimal(data[2], CultureInfo.InvariantCulture), 3).ToString("0.000", CultureInfo.InvariantCulture);

        //Name family data[2]
        SetHoverInitalValues(thicknessHover, (float) Math.Round(Convert.ToDecimal(data[4], CultureInfo.InvariantCulture), 3),(float) 10.000, (float) 0.100);
        SetHoverInitalValues(lthicknessHover, (float) Math.Round(Convert.ToDecimal(data[6], CultureInfo.InvariantCulture), 3),(float) 10.000, (float) 0);
        SetHoverInitalValues(rthicknessHover, (float) Math.Round(Convert.ToDecimal(data[8], CultureInfo.InvariantCulture), 3),(float) 10.000, (float) 0);
        _updateThickness = false;
        _updateLThickness = false;
        _updateRThickness = false;
    }
    
    void setInitialValues(Slider s, float initial, float max, float min)
    {
        //default rounding value is set to 3
        s.value = initial;
        s.maxValue = max;
        s.minValue = min;
    }
    
    public void Reset()
    {
        _updateThickness = false;
        _updateLThickness = false;
        _updateRThickness = false;
    }

    public string[] GetCurrentValues()
    {
        var values = new List<string>();
        if (_updateThickness) { values.Add("thickness"); values.Add(thickness.value.ToString()); }
        return values.ToArray();
    }

    public string[] GetHoverCurrentValues()
    {
        var values = new List<string>();
        if (_updateThickness) { values.Add("thickness"); values.Add(thicknessHover.GetComponent<HoverItemDataSlider>().Value.ToString()); }
        if (_updateLThickness) { values.Add("left_coating_thickness"); values.Add(lthicknessHover.GetComponent<HoverItemDataSlider>().Value.ToString()); }
        if (_updateRThickness) { values.Add("right_coating_thickness"); values.Add(rthicknessHover.GetComponent<HoverItemDataSlider>().Value.ToString()); }

        return values.ToArray();
    }
    
    public void UpdateThickness()
    {
        print("New thickness");
        _updateThickness = true;
    }
    
    public void UpdateRThickness()
    {
        print("New right thickness");
        _updateRThickness = true;
    }
    
    public void UpdateLThickness()
    {
        print("New left thickness");
        _updateLThickness = true;
    }

    public void SetHoverInitalValues(GameObject slider, float initial, float max, float min)
    {
        slider.GetComponent<HoverItemDataSlider>().Value = initial;
        slider.GetComponent<HoverItemDataSlider>().RangeMax = max;
        slider.GetComponent<HoverItemDataSlider>().RangeMin = min;
    }
}