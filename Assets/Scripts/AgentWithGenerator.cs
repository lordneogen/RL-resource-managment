using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class AgentWithGenerator : Agent, IUseTick
{
    // Параметры генератора
    public float Energy_issued = 100f;
    [SerializeField]
    private float Energy_current = 100f;
    public int Tick_in_houre = 10;
    [HideInInspector]
    public int Tick_count = 0;
    public int Houre = 0;
    public List<IUseTick> Builds = new List<IUseTick>();
    [SerializeField]
    private int buildsSize;
    public float StaticEnergyFloatHouse;
    public bool train;
    
    // Параметры агента
    public int hour_start;
    public int hour_end;
    public int hour;
    public List<Build> builds = new List<Build>();
    public float Energy_issued_max = 300f;
    public float Energy_AI_max;
    public int maxObservations = 30;
    
    public GameObject trainObject;
    public GameObject trainCopyObject;
    public GameObject parentTrain;

    // Методы для генератора
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

    // Начало эпизода
    public override void OnEpisodeBegin()
    {
        hour = hour_start;
        Energy_issued = Energy_issued_max;
        builds.Clear();
        
        if (train)
        {
            GameObject trGameObject = Instantiate(trainObject, parentTrain.transform);
            trainCopyObject = trGameObject;
            
            for (int i = 0; i < maxObservations; i++)
            {
                int randomBuildingType = Random.Range(0, 3); // 0 - Industrial, 1 - Residential, 2 - Office
                
                Build build = trainCopyObject.AddComponent<Build>();
                
                switch (randomBuildingType)
                {
                    case 0:
                        build.Type = "House";
                        break;
                    case 1:
                        build.Type = "Mall";
                        break;
                    case 2:
                        build.Type = "Factory";
                        break;
                }
                build.Set_hour(hour);
                builds.Add(build);
            } 
        }
        else
        {
            // Реинициализация генератора
            Houre = hour_start;
            Energy_issued = Energy_issued;
            foreach (var v in Builds)
            {
                try
                {
                    builds.Add(v as Build);
                }
                finally
                {
                    
                }
            }
        }
    }

    // Сбор наблюдений для агента
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(hour_start);
        sensor.AddObservation(hour_end);
        sensor.AddObservation(Energy_issued);
        sensor.AddObservation(builds.Count);
        sensor.AddObservation(hour);
        
        int buildingsObserved = 0;
        foreach (var building in builds)
        {
            if (buildingsObserved >= maxObservations) break;

            sensor.AddObservation(building.Get_type());
            sensor.AddObservation(building.The_building_consumes_energy_in_the_amount());
            buildingsObserved++;
        }

        // Заполняем нулями, если зданий меньше maxObservations
        for (int i = buildingsObserved; i < maxObservations; i++)
        {
            sensor.AddObservation(0);
            sensor.AddObservation(0f);
        }
    }

    // Получение действий от агента
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (train)
        {
            Houre = hour;
            var continuousActions = actions.ContinuousActions;
            Energy_issued = 100f;
            float totalAllocatedEnergy = 0f;
            int buildingsProcessed = Mathf.Min(builds.Count, maxObservations);

            for (int i = 0; i < buildingsProcessed; i++)
            {
                if (totalAllocatedEnergy >= Energy_issued)
                {
                    SetReward(-1.0f);
                    break;
                }
                else
                {
                    float allocatedEnergy = continuousActions[i] * Energy_AI_max;
                    allocatedEnergy = Mathf.Max(0, allocatedEnergy); // Энергия на здание
                    totalAllocatedEnergy += allocatedEnergy;

                    builds[i].The_generator_gives_energy_in_size(allocatedEnergy);
                    if (!builds[i].Is_active())
                    {
                        SetReward(-1.0f);
                    }

                    builds[i].Tick_activation();
                }
            }

            hour++;

            if (totalAllocatedEnergy <= Energy_issued)
            {
                SetReward(1.0f);
                Energy_issued -= totalAllocatedEnergy;
            }
            else
            {
                SetReward(-1.0f);
            }

            if (hour >= hour_end)
            {
                EndEpisode();
            }
        }
    }

    // Тестовый режим управления
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;

        for (int i = 0; i < continuousActionsOut.Length; i++)
        {
            continuousActionsOut[i] = 5f; // Примерное значение для теста
        }
    }
}
