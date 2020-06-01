using System.Collections;
using System.Collections.Generic;
using KhepriUnity;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class AddPanel : MonoBehaviour
{
    public GameObject currentPanel;
    public GameObject menu;
    public GameObject chair;
    public GameObject table;
    public GameObject light;
    private bool selected;
    
    // Start is called before the first frame update
    void Start()
    {
        selected = false;
        currentPanel = menu;
        menu.SetActive(true);
        chair.SetActive(false);
        table.SetActive(false);
        light.SetActive(false);
    }
    
    void OnEnable()
    {
        selected = false;
        currentPanel = menu;
        menu.SetActive(true);
        chair.SetActive(false);
        table.SetActive(false);
        light.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenChair()
    {
        chair.SetActive(true);
        selected = true;
    }

    public void OpenLight()
    {
        currentPanel.SetActive(false);
        light.SetActive(true);
        currentPanel = light;
        AddObject script = (AddObject) light.GetComponent(typeof(AddObject));
        script.StartPanel(0);
        selected = true;
    }

    public void Cancel()
    {
        if (selected)
        {
            AddObject script = (AddObject) currentPanel.GetComponent(typeof(AddObject));
            script.Cancel();
            currentPanel.SetActive(false);
            currentPanel = menu;
            currentPanel.SetActive(true);
            selected = false;
        } else
        {
            this.gameObject.SetActive(false);
        }
    }

    public void Save()
    {
        if (selected)
        {
            AddObject script = (AddObject) light.GetComponent(typeof(AddObject));
            string result = script.getCurrentValues();
            print(result);
            Primitives.AddToQueue(result);
            currentPanel.SetActive(false);
            currentPanel = menu;
            currentPanel.SetActive(true);
        }

    }
}
