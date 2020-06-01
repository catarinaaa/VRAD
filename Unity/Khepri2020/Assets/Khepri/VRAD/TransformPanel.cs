using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformPanel : MonoBehaviour
{
    public GameObject movePanel;
    public GameObject scalePanel;
    public GameObject reflectPanel;
    public GameObject rotatePanel;
    // Start is called before the first frame update
    void Start()
    { 
        movePanel.SetActive(true);
        scalePanel.SetActive(false);
        rotatePanel.SetActive(false);
        reflectPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenMove()
    {
        movePanel.SetActive(true);
        scalePanel.SetActive(false);
        rotatePanel.SetActive(false);
        reflectPanel.SetActive(false);
    }

    public void OpenRotate()
    {
        movePanel.SetActive(false);
        scalePanel.SetActive(false);
        rotatePanel.SetActive(true);
        reflectPanel.SetActive(false);
    }

    public void OpenScale()
    {
        movePanel.SetActive(false);
        scalePanel.SetActive(true);
        rotatePanel.SetActive(false);
        reflectPanel.SetActive(false);
    }

    public void OpenReflection()
    {
        movePanel.SetActive(false);
        scalePanel.SetActive(false);
        rotatePanel.SetActive(false);
        reflectPanel.SetActive(true);
    }
}
