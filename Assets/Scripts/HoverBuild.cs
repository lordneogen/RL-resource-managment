using System;
using UnityEngine;

public class HoverBuild : MonoBehaviour
{
    private Outline_main _outlineMain;
    private Camera mainCamera;
    private bool hover = false;
    private UseofResources useofResources;

    void Start()
    {
        _outlineMain = this.gameObject.GetComponent<Outline_main>();
        useofResources = this.gameObject.GetComponent<UseofResources>();
        _outlineMain.enabled = false;
        mainCamera = Camera.main;
    }

    
    private void OnMouseEnter()
    {
        hover = true;
        _outlineMain.enabled = true;
    }

    private void OnMouseExit()
    {
        hover = false;
        _outlineMain.enabled = false;
    }

    private void Update()
    {
        if (hover)
        {
            Generator.Instance.windowGraph.ShowGraph(useofResources.timebatteryuse);
            Generator.Instance.windowGraph_Bank.ShowGraph(useofResources.timebatterybank);
            Generator.Instance.windowGraph.showInfo(useofResources.GetResourceInfo());
        }
    }
}