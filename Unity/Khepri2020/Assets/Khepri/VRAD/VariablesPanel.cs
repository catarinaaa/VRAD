using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Hover.Core.Items.Types;
using KhepriUnity;
using UnityEngine;
using UnityEngine.UI;

public class VariablesPanel : MonoBehaviour
{
    public GameObject slider;
    public List<string> updated;
    public Dictionary<string, GameObject> sliderDict;

    public void Start() {}

    public void Update() {}
    
    public void StartPanel(string[] data)
    {
        updated = new List<string>();
        sliderDict = new Dictionary<string, GameObject>();
        GameObject g = slider;
        for(int i = 0; i < data.Length; i+=3)
        {
            //TODO: bool
            
            GameObject _new = Instantiate(slider, g.transform.parent);
            _new.transform.position += (Vector3.down * 0.05f * (i/3) + Vector3.down * 0.056f);
            _new.GetComponentInChildren<Text>().text = data[i];
            _new.GetComponent<HoverItemDataSlider>().Label = data[i];
            if (data[i + 1].Contains("Int"))
            {
                _new.GetComponent<HoverItemDataSlider>().Snaps = (int) _new.GetComponent<HoverItemDataSlider>().RangeMax + 1;
            }
            print(float.Parse(data[i+2], CultureInfo.InvariantCulture));
            _new.GetComponent<HoverItemDataSlider>().Value = float.Parse(data[i+2], CultureInfo.InvariantCulture);
            print( _new.GetComponent<HoverItemDataSlider>().Value);
            _new.GetComponent<HoverItemDataSlider>().RangeMax = float.Parse(data[i+2], CultureInfo.InvariantCulture) * 2;
            _new.GetComponent<HoverItemDataSlider>().RangeMin = 0;
            _new.GetComponent<HoverItemDataSlider>().OnValueChangedEvent.AddListener(UpdateVariable);
            _new.SetActive(true);
            sliderDict.Add(data[i], _new);
        }
    }


    
    public void UpdateVariable(IItemDataSelectable<float> value)
    {
        print(value.Label);
        if(!updated.Contains(value.Label))
            updated.Add(value.Label);
    }

    public void Save()
    {
        string result = "updateGlobalVariables";
        foreach (string name in updated)
        {
            result += "#" + name + "#" + (sliderDict[name].GetComponent<HoverItemDataSlider>().Value * 10).ToString().Replace(",",".");
        }
        Primitives.AddToQueue(result);
    }
}
