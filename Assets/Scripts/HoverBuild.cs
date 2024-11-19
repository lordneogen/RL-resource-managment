using System;
using UnityEngine;

public class HoverBuild : MonoBehaviour
{
    private Outline_main _outlineMain;
    private Camera mainCamera;
    private bool hover = false;
    private Build _build;

    void Start()
    {
        _outlineMain = this.gameObject.GetComponent<Outline_main>();
        _build = this.gameObject.GetComponent<Build>();
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
            Generator.Instance.windowGraph.ShowGraph(_build.Electricity_usage_relative_to_time);
            Generator.Instance.windowGraph_Bank.ShowGraph(_build.Electricity_storage_usage_relative_to_time);
            Generator.Instance.windowGraph.showInfo(_build.GetResourceInfo());
        }
    }
}