using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Hover.Core.Items.Types;
using UnityEngine;
using UnityEngine.UI;

public class TablePanel : MonoBehaviour
{
    public bool isHover = true;
    public Slider length;
    public Slider width;
    public Slider height;
    public Slider topthickness;
    public Slider legthickness;

    public GameObject lenghtHover;
    public GameObject widthHover;
    public GameObject heightHover;
    public GameObject topthicknessHover;
    public GameObject legthicknessHover;
    
    private bool _updateLength = false;
    private bool _updateWidth = false;
    private bool _updateHeight = false;
    private bool _updateTopThickness = false;
    private bool _updateLegThickness = false;


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
        print("Starting Table Panel");
        print("Values: " + data[4]);

        if (isHover)
        {
            SetHoverInitalValues(lenghtHover, (float) Math.Round(Convert.ToDecimal(data[4], CultureInfo.InvariantCulture), 3),(float) 10.000, (float) 0.100);
            SetHoverInitalValues(widthHover, (float) Math.Round(Convert.ToDecimal(data[6], CultureInfo.InvariantCulture), 3),(float) 10.000, (float) 0.100);
            SetHoverInitalValues(heightHover, (float) Math.Round(Convert.ToDecimal(data[8], CultureInfo.InvariantCulture), 3),(float) 10.000, (float) 0.100);
            SetHoverInitalValues(topthicknessHover, (float) Math.Round(Convert.ToDecimal(data[10], CultureInfo.InvariantCulture), 3),(float) 10.000, (float) 0.100);
            SetHoverInitalValues(legthicknessHover, (float) Math.Round(Convert.ToDecimal(data[12], CultureInfo.InvariantCulture), 3),(float) 10.000, (float) 0.100);

        }
        else
        {
            data[4] = Math.Round(Convert.ToDecimal(data[4], CultureInfo.InvariantCulture), 3).ToString("0.000", CultureInfo.InvariantCulture);
            setInitialValues(length, (float) Math.Round(Convert.ToDecimal(data[4], CultureInfo.InvariantCulture), 3),(float) 10.000, (float) 0.100);
            setInitialValues(width, (float) Math.Round(Convert.ToDecimal(data[6], CultureInfo.InvariantCulture), 3),(float) 10.000, (float) 0.100);
            setInitialValues(height, (float) Math.Round(Convert.ToDecimal(data[8], CultureInfo.InvariantCulture), 3),(float) 10.000, (float) 0.100);
            setInitialValues(topthickness, (float) Math.Round(Convert.ToDecimal(data[10], CultureInfo.InvariantCulture), 3),(float) 10.000, (float) 0.100);
            setInitialValues(legthickness, (float) Math.Round(Convert.ToDecimal(data[12], CultureInfo.InvariantCulture), 3),(float) 10.000, (float) 0.100);
        }
        Reset();
    }
    
    public void SetHoverInitalValues(GameObject slider, float initial, float max, float min)
    {
        slider.GetComponent<HoverItemDataSlider>().Value = initial;
        slider.GetComponent<HoverItemDataSlider>().RangeMax = max;
        slider.GetComponent<HoverItemDataSlider>().RangeMin = min;
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
        _updateLength = false;
        _updateWidth = false;
        _updateHeight = false;
        _updateTopThickness = false;
        _updateLegThickness = false;
    }
    
    public void UpdateLength()
    {
        print("New length");
        _updateLength = true;
    }
    public void UpdateHeight()
    {
        print("New height");
        _updateHeight = true;
    }
    public void UpdateWidth()
    {
        print("New width");
        _updateWidth = true;
    }
    
    public void UpdateTopThickness()
    {
        print("New top thickness");
        _updateTopThickness = true;
    }
    
    public void UpdateLegThickness()
    {
        print("New leg thickness");
        _updateLegThickness = true;
    }
    
    public string[] GetCurrentValues()
    {
        var values = new List<string>();
        if (_updateLength)
        {
            values.Add("length");
            if (isHover) values.Add(lenghtHover.GetComponent<HoverItemDataSlider>().Value.ToString());
            else values.Add(length.value.ToString());
        }
        if (_updateWidth)
        {
            values.Add("width");
            if (isHover) values.Add(widthHover.GetComponent<HoverItemDataSlider>().Value.ToString());
            else values.Add(width.value.ToString());
        }
        if (_updateHeight)
        {
            values.Add("height");
            if (isHover) values.Add(heightHover.GetComponent<HoverItemDataSlider>().Value.ToString());
            else values.Add(height.value.ToString());
        }
        if (_updateTopThickness)
        {
            values.Add("top_thickness");
            if (isHover) values.Add(topthicknessHover.GetComponent<HoverItemDataSlider>().Value.ToString());
            else values.Add(topthickness.value.ToString());
        }
        if (_updateLegThickness)
        {
            values.Add("leg_thickness");
            if (isHover) values.Add(legthicknessHover.GetComponent<HoverItemDataSlider>().Value.ToString());
            else values.Add(legthickness.value.ToString());
        }

        return values.ToArray();

    }
}
