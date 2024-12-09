using System.Collections.Generic;
using CityBuilder;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.SideChannels;
using UnityEngine;



public class AgentWithGenerator : Agent
{
// --- Energy Management ---
    [Header("Energy Management")]
    public float EnergyUsedPerHour;
    public float MaxBatteryCharge;
    public float RandomMaxBatteryCharge;
    public float RandomMinBatteryCharge;
    public float CurrentEnergyUsed;
    public float EnergyUsageCoefficient;
    public float CoefMax;
    public float MaxEnergyPerBuilding;

// --- Building and City Management ---
    [Header("Building and City Management")]
    public CityGenerator CityGenerator;
    public int CityGridSize;
    public int MaxBuildingCount;
    public int MinBuildingCount;
    public int CurrentBuildingCount;
    public List<Build> Buildings = new List<Build>();
    public bool EndBecoseEnergy = false;

// --- Time and Training ---
    [Header("Time and Training")]
    public bool IsTraining;
    public int StartHour;
    public int EndHour;
    public int EndHourMax;
    public int CurrentHour;
    public bool IsTimeIncreasing;
    public GameObject TrainingObject;
    public GameObject TrainingObjectCopy;
    public GameObject TrainingObjectParent;
    public DisplayGameClock TrainingClock;

// --- Error Handling ---
    [Header("Error Handling")]
    public int CurrentErrorCount;
    public int MinErrorCount;
    public int MaxErrorCount;
    public bool ErrorCountDecrise;
    public List<float> Errors;
    public Window_Graph_Bank ErrorGraph;

// --- Graphs and Display ---
    [Header("Graphs and Display")]
    public Window_Graph BuildingGraph;
    public Window_Graph_Bank EnergyGraph;

// --- Custom Metrics ---
    [Header("Custom Metrics")]
    private float customMetric = 0.0f;

// --- Interfaces ---
    [Header("Interfaces")]
    [HideInInspector]
    public List<IUseTick> UseTickInterfaces = new List<IUseTick>();

    
    private readonly List<(string buildingName, float probability)> buildingProbabilities = new List<(string, float)>
    {
        ("House", 0.8f), // 50% for House
        ("Mall", 0.1f),  // 30% for Mall
        ("Factory", 0.1f) // 20% for Factory
    };

// Helper function to select building type based on probabilities
    private string GetRandomBuildingType()
    {
        float randomValue = Random.Range(0f, 1f);
        float cumulativeProbability = 0f;

        // Iterate over the building types and their probabilities
        foreach (var building in buildingProbabilities)
        {
            cumulativeProbability += building.probability;
            if (randomValue <= cumulativeProbability)
            {
                return building.buildingName;
            }
        }

        return buildingProbabilities[0].buildingName; // Default case (fallback)
    }

    public override void OnEpisodeBegin()
    {
        CityGenerator.GRID_SIZE = CityGridSize;
        CurrentErrorCount = MaxErrorCount;
        CurrentBuildingCount = Random.Range(MinBuildingCount, MaxBuildingCount);
        CurrentEnergyUsed = Random.Range(RandomMinBatteryCharge, RandomMaxBatteryCharge);
        MaxBatteryCharge = CurrentEnergyUsed;
        CurrentHour = StartHour;
        TrainingClock.Set_hour(CurrentHour);
        TrainingClock.epoch++;
        EnergyUsedPerHour = MaxBatteryCharge;

        if (IsTraining)
        {
            Destroy(TrainingObjectCopy);
            GameObject trGameObject = Instantiate(TrainingObject, TrainingObjectParent.transform);
            TrainingObjectCopy = trGameObject;

            for (int i = 0; i < MaxBuildingCount; i++)
            {
                if (i < CurrentBuildingCount)
                {
                    // Assign a building type based on probabilities
                    string buildingType = GetRandomBuildingType();

                    Build build = TrainingObjectCopy.AddComponent<Build>();

                    build.Type = buildingType;

                    build.Set_hour(CurrentHour);
                    if (TrainingClock.epoch > 1)
                    {
                        Buildings[i] = build;
                    }
                    else
                    {
                        Buildings.Add(build);
                    }
                }
                else
                {
                    Build build = TrainingObjectCopy.AddComponent<Build>();
                    build.Type = "Placeholder";
                    build.Set_hour(CurrentHour);
                    if (TrainingClock.epoch > 1)
                    {
                        Buildings[i] = build;
                    }
                    else
                    {
                        Buildings.Add(build);
                    }
                }
            }
        }
        else
        {
            CurrentBuildingCount = 0;
            Destroy(TrainingObjectCopy);
            GameObject trGameObject = Instantiate(TrainingObject, TrainingObjectParent.transform);
            TrainingObjectCopy = trGameObject;

            CityGenerator.ResetGrid();
            CurrentHour = StartHour;

            for (int i = 0; i < MaxBuildingCount; i++)
            {
                if (i < CityGenerator.currentHouseCount)
                {
                    if (Buildings.Count < MaxBuildingCount)
                    {
                        Buildings.Add(UseTickInterfaces[i] as Build);
                    }
                    else
                    {
                        Buildings[i] = UseTickInterfaces[i] as Build;
                    }
                }
                else
                {
                    Build build = TrainingObjectCopy.AddComponent<Build>();
                    build.Type = "Placeholder";
                    build.Set_hour(CurrentHour);
                    if (Buildings.Count < MaxBuildingCount)
                    {
                        Buildings[i] = build;
                    }
                    else
                    {
                        Buildings.Add(build);
                    }
                }
            }

            CurrentBuildingCount = Buildings.Count;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Buildings.RemoveAll(item => item == null);
        sensor.AddObservation(StartHour);
        sensor.AddObservation(EndHour);
        sensor.AddObservation(EnergyUsedPerHour);
        sensor.AddObservation(MaxBatteryCharge);
        sensor.AddObservation(Buildings.Count);
        sensor.AddObservation(CurrentHour);
        sensor.AddObservation(MaxBuildingCount);
        sensor.AddObservation(CurrentErrorCount);
        sensor.AddObservation(MaxErrorCount);
        sensor.AddObservation(MinErrorCount);
        sensor.AddObservation(CurrentBuildingCount);
        sensor.AddObservation(CurrentEnergyUsed);

        if (Buildings.Count < MaxBuildingCount)
        {
            while (Buildings.Count < MaxBuildingCount)
            {
                Build placeholderBuild = new GameObject("Placeholder").AddComponent<Build>();
                placeholderBuild.Type = "Placeholder";
                Buildings.Add(placeholderBuild);
            }
        }

        for (int i = 0; i < Buildings.Count && i < MaxBuildingCount; i++)
        {
            sensor.AddObservation(Buildings[i].Get_type());
            sensor.AddObservation(Buildings[i].The_building_consumes_energy_in_the_amount());
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        
        float performance = GetCumulativeReward(); 
        float newLearningRate = Mathf.Clamp(0.1f - performance * 0.01f, 0.001f, 0.1f); 
        Academy.Instance.StatsRecorder.Add("LearningRate", newLearningRate);
        
        int offset = 2;
        float totalEnergyConsumed = 0;
        TrainingClock.Tick_activation();
        var continuousActions = actions.ContinuousActions;
        float totalAllocatedEnergy = 0f;

        EnergyUsageCoefficient = (actions.ContinuousActions[0]+1)/2;

        EnergyUsedPerHour = CurrentEnergyUsed/EndHour*CoefMax* EnergyUsageCoefficient;

        MaxBatteryCharge -= EnergyUsedPerHour;
        MaxBatteryCharge = Mathf.Max(MaxBatteryCharge, 0);

        if (MaxBatteryCharge <= 0 & EndBecoseEnergy)
        {
            SetReward(-50f);
            // Adjusting the error calculation
            Errors.Add(GetCumulativeReward() / CurrentBuildingCount);
            ErrorGraph.ShowGraph(Errors);
            // Add the error to the metric
            customMetric = GetCumulativeReward() / CurrentBuildingCount;
            Academy.Instance.StatsRecorder.Add("Error", customMetric);
            EndEpisode();
        }

        for (int i = offset; i < MaxBuildingCount + offset; i++)
        {
            if (totalAllocatedEnergy >= EnergyUsedPerHour)
            {
                SetReward(-5f);
                break;
            }
            else
            {
                float allocatedEnergy = Mathf.Clamp(
                    EnergyUsedPerHour / CurrentBuildingCount * (actions.ContinuousActions[i] + 1) / 2,
                    0,
                    Mathf.Min(EnergyUsedPerHour - totalAllocatedEnergy, MaxEnergyPerBuilding)
                );
                // allocatedEnergy = Mathf.Clamp(allocatedEnergy, 0, Mathf.Min(EnergyUsedPerHour - totalAllocatedEnergy, MaxEnergyPerBuilding));
                totalAllocatedEnergy += allocatedEnergy;

                Buildings[i - offset]?.The_generator_gives_energy_in_size(allocatedEnergy);
            }
        }

        if (!IsTraining)
        {

            for (int i = 0; i < MaxBuildingCount; i++)
            {
                if (!Buildings[i].Is_active()&Buildings[i].Type!="Placeholder")
                {
                    CurrentErrorCount--;
                    if (CurrentErrorCount < 0)
                    {
                        Debug.Log("Training error");
                        SetReward(-50f);
                        // Adjusting the error calculation
                        Errors.Add(GetCumulativeReward() / Buildings.Count); // Используем фактическое количество зданий
                        ErrorGraph.ShowGraph(Errors);
                        // Add the error to the metric
                        customMetric = GetCumulativeReward() / Buildings.Count;
                        Academy.Instance.StatsRecorder.Add("Error", customMetric);
                        EndEpisode();
                    }

                    SetReward(-1.0f);
                }
                else
                {
                    SetReward(20.0f);
                }

                Buildings[i]?.Tick_activation();
                totalEnergyConsumed += Buildings[i].Electricity_consumed;
            }
        }
        else
        {
            for (int i = 0; i < MaxBuildingCount; i++)
            {
                if (!Buildings[i].Is_active()&Buildings[i].Type!="Placeholder")
                {
                    CurrentErrorCount--;
                    if (CurrentErrorCount < 0)
                    {
                        Debug.Log("Episode ended due to excessive errors.");
                        SetReward(-50f);
                        Errors.Add(GetCumulativeReward() / Buildings.Count);
                        ErrorGraph.ShowGraph(Errors);
                        customMetric = GetCumulativeReward() / Buildings.Count;
                        Academy.Instance.StatsRecorder.Add("Error", customMetric);
                        EndEpisode();
                    }


                    SetReward(-1.0f);
                }
                else
                {
                    SetReward(20.0f);
                }

                Buildings[i]?.Tick_activation();
                totalEnergyConsumed += Buildings[i].Electricity_consumed;
            }
        }

        CurrentHour++;

        SetReward((-EnergyUsedPerHour + totalEnergyConsumed) * -0.1f);
        // SetReward(-Mathf.Abs( EnergyUsedPerHour - totalEnergyConsumed) * 10);

        if (totalAllocatedEnergy <= EnergyUsedPerHour)
        {
            SetReward(2.0f);
            EnergyUsedPerHour -= totalAllocatedEnergy;
        }
        else
        {
            SetReward(-1.0f);
        }

        if (CurrentHour >= EndHour)
        {
            if (IsTimeIncreasing & EndHour<EndHourMax)
            {
                EndHour++;
            }

            if (ErrorCountDecrise & MaxErrorCount>MinErrorCount)
            {
                MaxErrorCount--;
            }

            SetReward(200f);
            // Adjusting the error calculation
            Errors.Add(GetCumulativeReward() / CurrentBuildingCount);
            ErrorGraph.ShowGraph(Errors);
            // Add the error to the metric
            customMetric = GetCumulativeReward() / CurrentBuildingCount;
            Academy.Instance.StatsRecorder.Add("Error", customMetric);
            EndEpisode();
        }
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;

        for (int i = 0; i < continuousActionsOut.Length; i++)
        {
            continuousActionsOut[i] = 5f; // Примерное значение для теста
        }
    }
}
