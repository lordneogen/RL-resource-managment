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
    private static Generator _instance;
    public float Energy_issued=100f;
    [SerializeField]
    private float Energy_current=100f;
    public int Tick_in_houre=10;
    [HideInInspector]
    public int Tick_count=0;
    public int Houre = 0;
    public List<IUseTick> Builds = new List<IUseTick>();
    [SerializeField]
    private int buildsSize;
    public CityGenerator cityGenerator;
    public Window_Graph windowGraph;
    public Window_Graph_Bank windowGraph_Bank;
    public float StaticEnergyFloatHouse;
    public bool train;
    public static Generator Instance
    {
        
        // Time.timeScale = 0.01f;
        get
        {
            // Если экземпляр еще не создан, создаем его
            if (_instance == null)
            {
                _instance = new Generator();
            }
            return _instance;
        }
        set { _instance = value; }
    }

    private void Update()
    {
        Tick_count++;
        if (Tick_count == Tick_in_houre)
        {

            if (!train)
            {
                Houre++;
                buildsSize = 0;
                Energy_current = Energy_issued;
                for (int i = 0; i < Builds.Count; i++)
                {
                    if (Builds[i].Is_need())
                    {
                        Energy_current -= StaticEnergyFloatHouse;
                        Builds[i].The_generator_gives_energy_in_size(StaticEnergyFloatHouse);
                        buildsSize++;
                    }
                }

                Tick_count = 0;
                for (int i = 0; i < Builds.Count; i++)
                {
                    Builds[i].Tick_activation();
                }
            }
        }
    }

    private void Awake()
    {
        Generator.Instance = this;
        Builds.Add(this);
    }

    public void Tick_activation()
    {
        Tick_count++;
    }
    public void The_generator_gives_energy_in_size(float batteryOut)
    {
        return;
    }

    public float The_building_consumes_energy_in_the_amount()
    {
        return 0f;
    }

    public bool Is_need()
    {
        return false;
    }

    public bool Is_active()
    {
        return true;
    }

    public void Set_hour(int hour)
    {
        hour = hour;
    }

    public int Get_type()
    {
        return 0;
    }
}