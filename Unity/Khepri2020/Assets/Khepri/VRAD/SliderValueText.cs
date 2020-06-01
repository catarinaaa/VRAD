using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueText : MonoBehaviour
{
    // Start is called before the first frame update
    Text textComponent;
    Slider sliderComponent;
    //TODO: replace this
    public GameObject settingsPanel;
    public int rounding = 3;
    void Start()
    {
        sliderComponent = ((gameObject.transform.parent.parent.parent).gameObject).GetComponent<Slider>();
        textComponent = gameObject.GetComponent<Text>();
        textComponent.text = Math.Round(Convert.ToDecimal(sliderComponent.value.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture), rounding).ToString(CultureInfo.InvariantCulture);
        SetValue(sliderComponent.value);
        print(textComponent.text);
    }

    // Update is called once per frame
    public void SetValue(float value)
    {
        if (textComponent == null) textComponent = gameObject.GetComponent<Text>();
        if (sliderComponent == null) sliderComponent = ((gameObject.transform.parent.parent.parent).gameObject).GetComponent<Slider>();
        sliderComponent.value = (float) Math.Round(Convert.ToDecimal(sliderComponent.value.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture), rounding);
        textComponent.text = Math.Round(Convert.ToDecimal(value.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture), rounding).ToString(CultureInfo.InvariantCulture);
    }

    public void SetMin(string value)
    {
        if (sliderComponent == null) sliderComponent = ((gameObject.transform.parent.parent.parent).gameObject).GetComponent<Slider>();
        sliderComponent.minValue = (float) Math.Round(Decimal.Parse(value, CultureInfo.InvariantCulture.NumberFormat), rounding);
    }

    public void SetMax(string value)
    {
        if (sliderComponent == null) sliderComponent = ((gameObject.transform.parent.parent.parent).gameObject).GetComponent<Slider>();
        sliderComponent.maxValue = (float) Math.Round(Decimal.Parse(value, CultureInfo.InvariantCulture.NumberFormat), rounding);
    }

    public void Cancel()
    {
        settingsPanel.SetActive(false);
    }

    public void Open()
    {
        settingsPanel.SetActive(true);
    }

    public void ChangeRounding(float value)
    {
        rounding = (int) value;
        SetValue(sliderComponent.value);
    }
}
