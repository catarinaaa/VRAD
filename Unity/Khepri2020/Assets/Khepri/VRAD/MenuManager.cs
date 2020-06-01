using System.Collections;
using System.Collections.Generic;
using KhepriUnity;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;


/*
 * Class for handling the menu interactions
 */
public class MenuManager : MonoBehaviour
{
    public GameObject addPanel;
    public GameObject familyPanel;
    public GameObject variablesPanel;
    public GameObject historyPanel;

    //TODO: make a panel history to go back
    //private List<Panel> panelHistory = new List<Panel>();
    // Start is called before the first frame update
    void Start()
    {
        addPanel.SetActive(false);
        familyPanel.SetActive(false);
        variablesPanel.SetActive(false);
        historyPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update(){}

    // Add object panel
    public void OpenAddPanel()
    {
        print("Open add panel ");
        addPanel.SetActive(true);
    }

    public void CloseAddPanel()
    {
        print("Close!!!");
       addPanel.SetActive(false);
    }

    // Families panel
    public void OpenFamilyPanel(string[] data)
    {
        Transform playerTransform = GameObject.FindWithTag("Player").transform;
        FamilyPanel script = (FamilyPanel) familyPanel.GetComponent(typeof(FamilyPanel));
        script.StartPanel(data);
        familyPanel.SetActive(true);
        print("Open family panel ");
    }
    
    public void OpenHoverFamilyPanel(string[] data)
    {
        HoverFamilyPanel script = (HoverFamilyPanel) familyPanel.GetComponent(typeof(HoverFamilyPanel));
        script.StartPanel(data);
        familyPanel.SetActive(true);
    }
    
    public void CloseFamilyPanel()
    {
        familyPanel.SetActive(false);
    }

    
    // Variables Panel
    public void OpenVariablesPanel(string[] data)
    {
        VariablesPanel script = (VariablesPanel) variablesPanel.GetComponent(typeof(VariablesPanel));
        script.StartPanel(data);
        variablesPanel.SetActive(true);
    }

    public void CloseVariablesPanel()
    {
        variablesPanel.SetActive(false);
    }
    
    
}
