using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Hover.Core.Items.Types;
using UnityEngine;
using UnityEngine.UI;

public class DoorPanel : MonoBehaviour
{
    public Slider width;
    public Slider height;
    public Slider thickness;
    
    public GameObject widthHover;
    public GameObject heightHover;
    public GameObject thicknessHover;
    
    private bool _updateThickness = false;
    private bool _updateWidth = false;
    private bool _updateHeight = false;

    public bool isHover = true;
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
        print("Starting Door Panel");
        print("Values: " + data[2] + ", " + data[4]);
        if (isHover)
        {
            //TODO check values
            SetHoverInitialValues(thicknessHover, float.Parse(data[4], CultureInfo.InvariantCulture.NumberFormat), 10,
                (float) 0.1);
            SetHoverInitialValues(widthHover, float.Parse(data[6], CultureInfo.InvariantCulture.NumberFormat), 10, 0);
            SetHoverInitialValues(heightHover, float.Parse(data[8], CultureInfo.InvariantCulture.NumberFormat), 10, 0);
        }
        else
        {
            SetInitialValues(thickness, float.Parse(data[4], CultureInfo.InvariantCulture.NumberFormat), 10,
                (float) 0.1);
            SetInitialValues(width, float.Parse(data[6], CultureInfo.InvariantCulture.NumberFormat), 10, 0);
            
            SetInitialValues(height, float.Parse(data[8], CultureInfo.InvariantCulture.NumberFormat), 10, 0);
        }
        Reset();
    }
    
    void SetInitialValues(Slider s, float initial, float max, float min)
    {
        s.value = initial;
        s.maxValue = max;
        s.minValue = min;
    }

    public void SetHoverInitialValues(GameObject slider, float initial, float max, float min)
    {
        slider.GetComponent<HoverItemDataSlider>().Value = initial;
        slider.GetComponent<HoverItemDataSlider>().RangeMax = max;
        slider.GetComponent<HoverItemDataSlider>().RangeMin = min;
    }
    
    public void UpdateThickness()
    {
        _updateThickness = true;
    }

    public void UpdateWidth()
    {
        _updateWidth = true;
    }

    public void UpdateHeight()
    {
        _updateWidth = true;
    }
    public void Reset()
    {
        _updateThickness = false;
        _updateWidth = false;
        _updateHeight = false;
    }

    public string[] GetCurrentValues()
    {
        var values = new List<string>();
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
        if (_updateThickness)
        {
            values.Add("thickness");
            if (isHover) values.Add(thicknessHover.GetComponent<HoverItemDataSlider>().Value.ToString());
            else values.Add(thickness.value.ToString());
        }
        
        return values.ToArray();
    }
}
