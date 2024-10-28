using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using UnityEngine.Serialization;

public class Generator:MonoBehaviour,IUseTick
{
    private static Generator instance;
    public float batteryRes=100f;
    [SerializeField]
    private float currBattery=100f;
    public int tickTime=10;
    [HideInInspector]
    public int tickCount=0;
    [FormerlySerializedAs("day")] public int houre = 0;
    public List<IUseTick> builds = new List<IUseTick>();
    [SerializeField]
    private int buildsSize;
    public CityGenerator cityGenerator;
    public Window_Graph windowGraph;
    public Window_Graph_Bank windowGraph_Bank;
    public float StaticFloatHouse;
    public static Generator Instance
    {
        get
        {
            // Если экземпляр еще не создан, создаем его
            if (instance == null)
            {
                instance = new Generator();
            }
            return instance;
        }
        set { instance = value; }
    }

    private void Update()
    {
        tickCount++;
        if (tickCount == tickTime)
        {
            houre++;
            buildsSize = 0;
            currBattery = batteryRes;
            for (int i = 0; i < builds.Count; i++)
            {
                if (builds[i].IsNeed())
                {
                    currBattery -= StaticFloatHouse;
                    builds[i].SetBatteryIn(StaticFloatHouse);
                    buildsSize++;
                }
            }
            tickCount = 0;
            for (int i = 0; i < builds.Count; i++)
            {
                builds[i].Use();
            }
        }
    }

    private void Awake()
    {
        Generator.Instance = this;
        builds.Add(this);
    }

    public void Use()
    {
        tickCount++;
    }
    public void SetBatteryIn(float batteryOut)
    {
        return;
    }

    public float GetBatteryOut()
    {
        return 0f;
    }

    public bool IsNeed()
    {
        return false;
    }

    public int Type()
    {
        return 0;
    }
}