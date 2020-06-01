using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Hover.Core.Items.Types;
using UnityEngine;
using UnityEngine.UI;

public class SlabPanel : MonoBehaviour
{
    
    public Slider thickness;
    public Slider coating_thickness;

    public GameObject familyButton;
    public GameObject thicknessHover;
    public GameObject cthicknessHover;
    
    private bool _updateThickness = false;
    private bool _updateCoatingThickness = false;
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
        print("Starting Slab Panel");
        print("Values: " + data[2] + ", " + data[4]);
        
        
        setInitialValues(thickness, float.Parse(data[4], CultureInfo.InvariantCulture.NumberFormat),10, (float) 0.1);
        setInitialValues(coating_thickness, float.Parse(data[6], CultureInfo.InvariantCulture.NumberFormat), 10, 0);
        _updateThickness = false;
        _updateCoatingThickness = false;
    }
    
    public void StartHoverPanel(string[] data)
    {
        print("Starting Wall Panel");
        print("Values: " + data[2]);
        
        //data[4] = Math.Round(Convert.ToDecimal(data[2], CultureInfo.InvariantCulture), 3).ToString("0.000", CultureInfo.InvariantCulture);

        //Name family data[2]
        SetHoverInitalValues(thicknessHover, (float) Math.Round(Convert.ToDecimal(data[4], CultureInfo.InvariantCulture), 3),(float) 10.000, (float) 0.1);
        SetHoverInitalValues(cthicknessHover, (float) Math.Round(Convert.ToDecimal(data[6], CultureInfo.InvariantCulture), 3),(float) 10.000, (float) 0);
        _updateThickness = false;
        _updateCoatingThickness = false;
    }

    void setInitialValues(Slider s, float initial, float max, float min)
    {
        //default rounding value is set to 3
        s.value = (float) Math.Round(Convert.ToDecimal(initial.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture), 3);
        s.maxValue = max;
        s.minValue = min;
    }

    
    
    public void UpdateThickness()
    {
        print("updatethickness");
        _updateThickness = true;
    }

    public void UpdateCoatingThickness()
    {
        print("update coating");

        _updateCoatingThickness = true;
    }

    public void Reset()
    {
        _updateThickness = false;
        _updateCoatingThickness = false;
    }

    public string[] GetCurrentValues()
    {
        var values = new List<string>();
        if (_updateThickness) { values.Add("thickness"); values.Add(thickness.value.ToString()); }
        if(_updateCoatingThickness) { values.Add("coating_thickness"); values.Add(coating_thickness.value.ToString()); }
        return values.ToArray();
    }
    
    public string[] GetHoverCurrentValues()
    {
        var values = new List<string>();
        if (_updateThickness) { values.Add("thickness"); values.Add(thicknessHover.GetComponent<HoverItemDataSlider>().Value.ToString()); }
        if (_updateCoatingThickness) { values.Add("coating_thickness"); values.Add(cthicknessHover.GetComponent<HoverItemDataSlider>().Value.ToString()); }

        return values.ToArray();
    }
    public void SetHoverInitalValues(GameObject slider, float initial, float max, float min)
    {
        slider.GetComponent<HoverItemDataSlider>().Value = initial;
        slider.GetComponent<HoverItemDataSlider>().RangeMax = max;
        slider.GetComponent<HoverItemDataSlider>().RangeMin = min;
    }
}
