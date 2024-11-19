using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Build : MonoBehaviour, IUseTick
{
    public bool On;
    public float Electricity_storage_capacity;
    public float Electricity_storage_capacity_MAX;
    public float Input_electricity;
    public float Electricity_consumed;
    public string Type;
    public List<float> Electricity_usage_relative_to_time;
    public List<float> Electricity_storage_usage_relative_to_time;
    //
    private int hour;
    //
    [HideInInspector] public bool train;
    //
    [SerializeField]
    private GameObject EnableLook;
    [SerializeField]
    private GameObject DisableLook;
    //

    public enum BuildingType
    {
        House = 1,
        Office = 2,
        Mall = 3,
        Factory = 4,
        School = 5,
        Restaurant = 6,
        Hotel = 7,
        SportCenter = 8,
        Hospital = 9,
        Factory2 = 10
    }

    public void Tick_activation()
    {
        GetBatteryIn();
        Electricity_storage_capacity += Input_electricity;

        if (EnableLook!=null || DisableLook!=null)
        {
            if (On)
            {
                EnableLook.SetActive(true);
                DisableLook.SetActive(false);
            }
            else
            {
                EnableLook.SetActive(false);
                DisableLook.SetActive(true);
            }
        }

        Electricity_storage_capacity -= Electricity_consumed;
        Electricity_storage_capacity = Mathf.Min(Electricity_storage_capacity, Electricity_storage_capacity_MAX);
        if (Electricity_storage_capacity < 0)
        {
            On = false;
        }
        else
        {
            On = true;
        }
    }

    //
    private void Start()
    {
        Electricity_consumed = 0f;
        Electricity_usage_relative_to_time = new List<float>();
        Electricity_storage_usage_relative_to_time = new List<float>();
        On = true;
        Generator.Instance.Builds.Add(this);

        // Установка начальных значений для каждого типа здания
        SetInitialElectricityValues();
    }

    private void SetInitialElectricityValues()
    {
        switch (Type)
        {
            case "House":
                Electricity_storage_capacity = 50f;
                Electricity_storage_capacity_MAX = 100f;
                break;
            case "Office":
                Electricity_storage_capacity = 200f;
                Electricity_storage_capacity_MAX = 500f;
                break;
            case "Mall":
                Electricity_storage_capacity = 300f;
                Electricity_storage_capacity_MAX = 800f;
                break;
            case "Factory":
                Electricity_storage_capacity = 500f;
                Electricity_storage_capacity_MAX = 1200f;
                break;
            case "School":
                Electricity_storage_capacity = 150f;
                Electricity_storage_capacity_MAX = 300f;
                break;
            case "Restaurant":
                Electricity_storage_capacity = 80f;
                Electricity_storage_capacity_MAX = 200f;
                break;
            case "Hotel":
                Electricity_storage_capacity = 250f;
                Electricity_storage_capacity_MAX = 600f;
                break;
            case "SportCenter":
                Electricity_storage_capacity = 200f;
                Electricity_storage_capacity_MAX = 500f;
                break;
            case "Hospital":
                Electricity_storage_capacity = 400f;
                Electricity_storage_capacity_MAX = 900f;
                break;
            case "Factory2":
                Electricity_storage_capacity = 600f;
                Electricity_storage_capacity_MAX = 1300f;
                break;
            default:
                Electricity_storage_capacity = 100f;
                Electricity_storage_capacity_MAX = 200f;
                break;
        }
    }

    public void GetBatteryIn()
    {
        Electricity_usage_relative_to_time.Add(Electricity_consumed);
        Electricity_storage_usage_relative_to_time.Add(Electricity_storage_capacity);

        if (Type == "House")
        {
            Electricity_consumed = (Mathf.Abs(Mathf.Sin((Generator.Instance.Houre - 6f) / 12f * Mathf.PI))
                         + Mathf.Abs(Mathf.Sin((Generator.Instance.Houre - 18f) / 12f * Mathf.PI))) * Random.Range(2f, 4f);
        }
        else if (Type == "Office")
        {
            Electricity_consumed = Mathf.Abs(Mathf.Sin(Generator.Instance.Houre / 12f * Mathf.PI)) * Random.Range(3f, 6f);
        }
        else if (Type == "Mall")
        {
            Electricity_consumed = Mathf.Max(0, Mathf.Sin(Generator.Instance.Houre / 24f * Mathf.PI * 2 - Mathf.PI / 3)) * Random.Range(5f, 8f);
        }
        else if (Type == "Factory")
        {
            Electricity_consumed = Mathf.Clamp01(Mathf.Sin(Generator.Instance.Houre / 12f * Mathf.PI - Mathf.PI / 2)) * Random.Range(6f, 10f);
        }
        else if (Type == "School")
        {
            Electricity_consumed = Mathf.Clamp01(Mathf.Sin((Generator.Instance.Houre - 8f) / 8f * Mathf.PI)) * Random.Range(4f, 7f);
        }
        else if (Type == "Restaurant")
        {
            Electricity_consumed = Mathf.Clamp01(Mathf.Sin((Generator.Instance.Houre - 17f) / 7f * Mathf.PI)) * Random.Range(3f, 6f);
        }
        else if (Type == "Hotel")
        {
            Electricity_consumed = (0.5f + 0.5f * Mathf.Cos(Generator.Instance.Houre / 24f * Mathf.PI * 2)) * Random.Range(4f, 5f);
        }
        else if (Type == "SportCenter")
        {
            Electricity_consumed = Mathf.Clamp01(Mathf.Sin((Generator.Instance.Houre - 10f) / 12f * Mathf.PI)) * Random.Range(5f, 9f);
        }
        else if (Type == "Hospital")
        {
            Electricity_consumed = (1f + 0.2f * Mathf.Sin(Generator.Instance.Houre / 12f * Mathf.PI)) * Random.Range(5f, 6f);
        }
        else if (Type == "Factory2")
        {
            Electricity_consumed = Mathf.Abs(Mathf.Sin(Generator.Instance.Houre / 6f * Mathf.PI - Mathf.PI / 3)) * Random.Range(7f, 11f);
        }
    }

    public void The_generator_gives_energy_in_size(float power)
    {
        Input_electricity = power;
    }

    public float The_building_consumes_energy_in_the_amount()
    {
        return Electricity_consumed;
    }
    
    public bool Is_need()
    {
        return true;
    }

    public bool Is_active()
    {
        return On;
    }

    public void Set_hour(int hour)
    {
        SetInitialElectricityValues();
        this.hour = hour;
    }

    public int Get_type()
    {
        if (Enum.TryParse(Type, out BuildingType buildingType))
        {
            return (int)buildingType;
        }
        return 0; // значение по умолчанию, если тип не найден
    }
    
    public string GetResourceInfo()
    {
        string info = $"TYPE: {Type}\n" +
                      $"MAX BANK: {Electricity_storage_capacity_MAX}\n" +
                      $"CURR BANK: {Electricity_storage_capacity}\n" +
                      $"Electricity Consumed: {Electricity_consumed}\n" +
                      $"Electricity Received: {Input_electricity}";
        return info;
    }
}
