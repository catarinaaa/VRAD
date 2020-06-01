using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Configuration;
using System.Text.RegularExpressions;
using Hover.Core.Items.Types;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

public class BeamPanel : MonoBehaviour
{

    public bool isHover = true;

    public GameObject profilePanel;
    public bool _updateProfile = false;

    private string profile = null;
    public GameObject profileButton;
    public GameObject radiusButton;
    public GameObject widthButton;
    public GameObject heightButton;
    public GameObject circularButton;
    public GameObject rectangularButton;
    
    public float radius;
    public float width;
    public float height;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void StartPanel(string[] data) {
        print("Starting Beam Panel");
        print("Values: " + data[4]);

        string[] operands = Regex.Split(data[4], @"Khepri\.|\(|\)|\s");  
        foreach (string op in operands)
        {
            print(op);
        }
        if (isHover)
        {
            profileButton.GetComponent<HoverItemDataSelector>().Label = operands[1];
        }
        StartProfilePanel(operands[1], operands[5], (operands[1].Equals("CircularPath")) ? null : operands[7]);

        _updateProfile = false;
    }

    public void StartProfilePanel(string name, string w, string h)
    {
        if (name.Equals("CircularPath"))
        {
            circularButton.GetComponent<HoverItemDataRadio>().Select();
            radius = (float) Math.Round(Convert.ToDecimal(w, CultureInfo.InvariantCulture), 3);
            radiusButton.GetComponent<HoverItemDataSlider>().Value = radius;
            widthButton.GetComponent<HoverItemDataSlider>().Value = 0;
            heightButton.GetComponent<HoverItemDataSlider>().Value = 0;
            radiusButton.GetComponent<HoverItemDataSlider>().IsEnabled = true;
            widthButton.GetComponent<HoverItemDataSlider>().IsEnabled = false;
            heightButton.GetComponent<HoverItemDataSlider>().IsEnabled = false;
        }
        else
        {
            rectangularButton.GetComponent<HoverItemDataRadio>().Select();
            width = (float) Math.Round(Convert.ToDecimal(w, CultureInfo.InvariantCulture), 3);
            height = (float) Math.Round(Convert.ToDecimal(h, CultureInfo.InvariantCulture), 3);
            radiusButton.GetComponent<HoverItemDataSlider>().IsEnabled = false;
            widthButton.GetComponent<HoverItemDataSlider>().IsEnabled = true;
            heightButton.GetComponent<HoverItemDataSlider>().IsEnabled = true;
            widthButton.GetComponent<HoverItemDataSlider>().Value = width;
            heightButton.GetComponent<HoverItemDataSlider>().Value = height;
            radiusButton.GetComponent<HoverItemDataSlider>().Value = 0;
        }
    }
    
    public void UpdateProfile()
    {
        _updateProfile = true;
    }

    public void CircularProfile()
    {
        print("Circular");
        profile = "circular_profile";
        //this.GetComponent<HoverItemDataRadio>().Select();
        radiusButton.GetComponent<HoverItemDataSlider>().IsEnabled = true;
        widthButton.GetComponent<HoverItemDataSlider>().IsEnabled = false;
        heightButton.GetComponent<HoverItemDataSlider>().IsEnabled = false;
        UpdateProfile();
    }

    public void RectangularProfile()
    {
        print("Rectangular");
        profile = "rectangular_profile";
        //this.GetComponent<HoverItemDataRadio>().Select();
        radiusButton.GetComponent<HoverItemDataSlider>().IsEnabled = false;
        widthButton.GetComponent<HoverItemDataSlider>().IsEnabled = true;
        heightButton.GetComponent<HoverItemDataSlider>().IsEnabled = true;
        UpdateProfile();
    }
    
    public string[] GetCurrentValues()
    {
        var values = new List<string>();
        //if (_updateProfile) { 
            values.Add("profile");
            if (profile.Equals("circular_profile"))
            {
                values.Add(profile + "(" + radius + ")");
            }
            else
            {
                values.Add(profile + "(" + width + "," + height + ")");
            }
        //}
        
        return values.ToArray();
    }

    public void Reset()
    {
        _updateProfile = false;
    }
}
