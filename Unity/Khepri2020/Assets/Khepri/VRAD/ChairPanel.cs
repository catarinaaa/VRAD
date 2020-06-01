using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Hover.Core.Items.Types;
using UnityEngine;
using UnityEngine.UI;

public class ChairPanel : MonoBehaviour
{
    public bool isHover = true;
    public Slider length;
    public Slider width;
    public Slider height;
    public Slider seatHeight;
    public Slider thickness;

    public GameObject lenghtHover;
    public GameObject widthHover;
    public GameObject heightHover;
    public GameObject seatHeightHover;
    public GameObject thicknessHover;
    
    private bool _updateLength = false;
    private bool _updateWidth = false;
    private bool _updateHeight = false;
    private bool _updateSeatHeight = false;
    private bool _updateThickness = false;


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
            SetHoverInitalValues(seatHeightHover, (float) Math.Round(Convert.ToDecimal(data[10], CultureInfo.InvariantCulture), 3),(float) 10.000, (float) 0.100);
            SetHoverInitalValues(thicknessHover, (float) Math.Round(Convert.ToDecimal(data[12], CultureInfo.InvariantCulture), 3),(float) 10.000, (float) 0.100);

        }
        else
        {
            data[4] = Math.Round(Convert.ToDecimal(data[4], CultureInfo.InvariantCulture), 3).ToString("0.000", CultureInfo.InvariantCulture);
            setInitialValues(length, (float) Math.Round(Convert.ToDecimal(data[4], CultureInfo.InvariantCulture), 3),(float) 10.000, (float) 0.100);
            setInitialValues(width, (float) Math.Round(Convert.ToDecimal(data[6], CultureInfo.InvariantCulture), 3),(float) 10.000, (float) 0.100);
            setInitialValues(height, (float) Math.Round(Convert.ToDecimal(data[8], CultureInfo.InvariantCulture), 3),(float) 10.000, (float) 0.100);
            setInitialValues(seatHeight, (float) Math.Round(Convert.ToDecimal(data[10], CultureInfo.InvariantCulture), 3),(float) 10.000, (float) 0.100);
            setInitialValues(thickness, (float) Math.Round(Convert.ToDecimal(data[12], CultureInfo.InvariantCulture), 3),(float) 10.000, (float) 0.100);
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
        _updateSeatHeight = false;
        _updateThickness = false;
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
    
    public void UpdateSeatHeight()
    {
        print("New seat height");
        _updateSeatHeight = true;
    }
    
    public void UpdateThickness()
    {
        print("New thickness");
        _updateThickness = true;
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
        if (_updateSeatHeight)
        {
            values.Add("seat_height");
            if (isHover) values.Add(seatHeightHover.GetComponent<HoverItemDataSlider>().Value.ToString());
            else values.Add(seatHeight.value.ToString());
        }
        if (_updateThickness)
        {
            values.Add("thickness");
            if (isHover) values.Add(thicknessHover.GetComponent<HoverItemDataSlider>().Value.ToString());
            else values.Add(thickness.value.ToString());
        }

        return values.ToArray();

    }
}
