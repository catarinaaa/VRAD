using System.Collections;
using System.Collections.Generic;
using Hover.Core.Items.Types;
using UnityEngine;

public class HoverAddLight : MonoBehaviour
{
    private GameObject created = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnEnable()
    {
        created = new GameObject("PointLight");
        Light light = created.AddComponent<Light>();
        created.transform.parent = GameObject.Find("MainObject").transform;
        light.type = LightType.Point;
        light.color = Color.white;
        light.range = 4;         // How far the light is emitted from the center of the object
        light.intensity = 2;     // Brightness of the light
        created.transform.localPosition = GameObject.FindWithTag("Player").transform.localPosition;
        
    }

    
    public void AdjustValueX(float value)
    {
        created.transform.position = new Vector3(value, created.transform.localPosition.y, created.transform.localPosition.z);
    }
    
    public void AdjustValueY(float value)
    {
        created.transform.position = new Vector3(created.transform.localPosition.x, value, created.transform.localPosition.z);

    }
    
    public void AdjustValueZ(float value)
    {
        created.transform.position = new Vector3(created.transform.localPosition.x, created.transform.localPosition.y, value);
    }

    public void AdjustRange(IItemDataSelectable<float> value)
    {
        created.GetComponent<Light>().range = value.Value;
    }

    public void AdjustIntensity(IItemDataSelectable<float> value)
    {
        print(value.Value);
        created.GetComponent<Light>().intensity = value.Value;
    }

    public void Cancel()
    {
        Destroy(created);
    }

    public string getCurrentValues()
    {
        string result = "";
        {
            result = "createLight#" + (created.transform.localPosition.x).ToString().Replace(",", ".") + "#" + (created.transform.localPosition.y).ToString().Replace(",", ".") +
                     "#" + (created.transform.localPosition.z).ToString().Replace(",", ".");
        }
        return result;
    }
}
