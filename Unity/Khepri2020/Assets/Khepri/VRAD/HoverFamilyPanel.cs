using System;
using System.Collections;
using System.Collections.Generic;
using Hover.Core.Items.Types;
using KhepriUnity;
using UnityEditorInternal;
using UnityEngine;

public class HoverFamilyPanel : MonoBehaviour
{

    public GameObject wall = null;
    public GameObject slab;
    public GameObject roof;
    public GameObject panel;
    public GameObject table;
    public GameObject chair;
    public GameObject door;
    public GameObject trussbar;
    public GameObject beam;
    public GameObject profileChange;
    public GameObject currentPanel;

    public GameObject functionsLayout;
    public GameObject functionButton;
    public List<GameObject> functionsList;
    public string functionToSave = null;
    
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
        CloseAllPanels();
        //type.text = data[0];
        functionsList = new List<GameObject>();
        string[] funcData = null;
        switch (data[0])
        {
            case "SlabFamily":
                currentPanel = slab;
                currentPanel.SetActive(true);
                funcData = new string[data.Length - 7];
                Array.Copy(data, 7, funcData, 0, data.Length - 7);
                SlabPanel script1 = (SlabPanel) currentPanel.GetComponent(typeof(SlabPanel));
                script1.StartHoverPanel(data);
                break;
            case "WallFamily":
                currentPanel = wall;
                currentPanel.SetActive(true);
                funcData = new string[data.Length - 9];
                Array.Copy(data, 9, funcData, 0, data.Length - 9);
                WallPanel script2 = (WallPanel) currentPanel.GetComponent(typeof(WallPanel));
                script2.StartHoverPanel(data);
                break;
            case "RoofFamily":
                currentPanel = roof;
                currentPanel.SetActive(true);
                funcData = new string[data.Length - 7];
                Array.Copy(data, 7, funcData, 0, data.Length - 7);
                SlabPanel script3 = (SlabPanel) currentPanel.GetComponent(typeof(SlabPanel));
                script3.StartHoverPanel(data);
                break;
            case "PanelFamily":
                currentPanel = panel;
                currentPanel.SetActive(true);
                funcData = new string[data.Length - 5];
                Array.Copy(data, 5, funcData, 0, data.Length - 5);
                PanelPanel script4 = (PanelPanel) currentPanel.GetComponent(typeof(PanelPanel));
                script4.StartPanel(data);
                break;
            case "TableFamily":
                currentPanel = table;
                currentPanel.SetActive(true);
                funcData = new string[data.Length - 11];
                Array.Copy(data, 11, funcData, 0, data.Length - 11);
                TablePanel script5 = (TablePanel) currentPanel.GetComponent(typeof(TablePanel));
                script5.StartPanel(data);
                break;
            case "ChairFamily":
                currentPanel = chair;
                currentPanel.SetActive(true);
                funcData = new string[data.Length - 11];
                Array.Copy(data, 11, funcData, 0, data.Length - 11);
                ChairPanel script6 = (ChairPanel) currentPanel.GetComponent(typeof(ChairPanel));
                script6.StartPanel(data);
                break;
            case "TableAndChairFamily":
                break;
            case "BeamFamily":
                currentPanel = beam;
                currentPanel.SetActive(true);
                funcData = new string[data.Length - 5];
                Array.Copy(data, 5, funcData, 0, data.Length - 5);
                BeamPanel script7 = (BeamPanel) currentPanel.GetComponent(typeof(BeamPanel));
                script7.StartPanel(data);
                break;
            case "ColumnFamily":
                currentPanel = beam;
                currentPanel.SetActive(true);
                funcData = new string[data.Length - 5];
                Array.Copy(data, 5, funcData, 0, data.Length - 5);
                BeamPanel script8 = (BeamPanel) currentPanel.GetComponent(typeof(BeamPanel));
                script8.StartPanel(data);
                break;
            case "CurtainWallFamily":
                break;
            case "CurtainWallFrameFamily":
                break;
            case "WindowFamily":
                currentPanel = door;
                currentPanel.SetActive(true);
                funcData = new string[data.Length - 9];
                Array.Copy(data, 9, funcData, 0, data.Length - 9);
                DoorPanel script12 = (DoorPanel) currentPanel.GetComponent(typeof(DoorPanel));
                script12.StartPanel(data);
                break;
            case "DoorFamily":
                currentPanel = door;
                currentPanel.SetActive(true);
                funcData = new string[data.Length - 9];
                Array.Copy(data, 9, funcData, 0, data.Length - 9);
                DoorPanel script13 = (DoorPanel) currentPanel.GetComponent(typeof(DoorPanel));
                script13.StartPanel(data);
                break;
            case "TrussBarFamily":
                currentPanel = trussbar;
                currentPanel.SetActive(true);
                funcData = new string[data.Length - 5];
                Array.Copy(data, 5, funcData, 0, data.Length - 5);
                TrussBarPanel script14 = (TrussBarPanel) currentPanel.GetComponent(typeof(TrussBarPanel));
                script14.StartPanel(data);
                break;
            case "TrussNodeFamily":
                break;
            
        }
        StartFunctions(funcData);

    }

    private void CloseAllPanels()
        {
            slab.SetActive(false);
            wall.SetActive(false);
            roof.SetActive(false);
            //add more
        }

    private void StartFunctions(string[] data)
    {
        //foreach (GameObject obj in functionsList)
        //{
          //  Destroy(obj);
        //}
        for (int i = 0; i < functionsList.Count; i++)
        {
            functionsList[i].SetActive(false);
        }
        functionsList = new List<GameObject>();
        bool first = true;
        foreach(string name in data)
        {
            print(name);
            functionsList.Add(Instantiate(functionButton, functionsLayout.transform));
            functionsList[functionsList.Count-1].GetComponent<HoverItemDataRadio>().Label = name;
            if (first)
            {
                functionsList[functionsList.Count-1].GetComponent<HoverItemDataRadio>().Select();
                first = false;
            }
            //functionsList.Add(_new);
        }
    }
    
    
    public string[] GetCurrentValues()
    {
        string[] values = null;
        if (currentPanel.name.Equals("SlabFamily"))
        {
            SlabPanel script = (SlabPanel) currentPanel.GetComponent(typeof(SlabPanel));
            values = script.GetHoverCurrentValues();
            script.Reset();
        }
        if (currentPanel.name.Equals("WallFamily"))
        {
            WallPanel script = (WallPanel) currentPanel.GetComponent(typeof(WallPanel));
            values = script.GetHoverCurrentValues();
            script.Reset();
        }

        if (currentPanel.name.Equals("DoorFamily") || currentPanel.name.Equals("WindowFamily"))
        {
            DoorPanel script = (DoorPanel) currentPanel.GetComponent(typeof(DoorPanel));
            values = script.GetCurrentValues();
            script.Reset();
        }
        if (currentPanel.name.Equals("PanelFamily"))
        {
            PanelPanel script = (PanelPanel) currentPanel.GetComponent(typeof(PanelPanel));
            values = script.GetCurrentValues();
            script.Reset();
        }
        if (currentPanel.name.Equals("TableFamily"))
        {
            TablePanel script = (TablePanel) currentPanel.GetComponent(typeof(TablePanel));
            values = script.GetCurrentValues();
            script.Reset();
        }
        if (currentPanel.name.Equals("ChairFamily"))
        {
            ChairPanel script = (ChairPanel) currentPanel.GetComponent(typeof(ChairPanel));
            values = script.GetCurrentValues();
            script.Reset();
        }
        if (currentPanel.name.Equals("BeamFamily"))
        {
            BeamPanel script = (BeamPanel) currentPanel.GetComponent(typeof(BeamPanel));
            values = script.GetCurrentValues();
            script.Reset();
        }
        return values;
    }
    
    public void Save()
    {
        string[] values = GetCurrentValues();
        string result = "updateFamilyValues";
        if(values == null) print("Nothing to save!");
        foreach (string s in values)
        {
            result += "#" + s.Replace(",", ".");
        }

        foreach (GameObject obj in functionsList)
        {
            if (obj.GetComponent<HoverItemDataRadio>().Value)
            {
                functionToSave = obj.GetComponent<HoverItemDataRadio>().Label;
            }
        }
        result += "#" + functionToSave;
        print("Sent: " + result);
        Primitives.AddToQueue(result);
    }

    public void StartProfilePanel()
    {
        currentPanel.SetActive(false);
        profileChange.SetActive(true);
        currentPanel = profileChange;
        functionsLayout.SetActive(false);
    }

    public void DisableProfilePanel()
    {
        currentPanel.SetActive(false);
        currentPanel = beam;
        currentPanel.SetActive(true);
        functionsLayout.SetActive(true);
    }
}

