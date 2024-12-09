using System;
using System.Collections.Generic;
using UnityEngine;
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

    private int hour;

    public bool Train { get; set; }

    [SerializeField]
    private GameObject EnableLook;
    [SerializeField]
    private GameObject DisableLook;

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
        Factory2 = 10,
        Placeholder = 11
    }

    private void Start()
    {
        Electricity_consumed = 0f;
        Electricity_usage_relative_to_time = new List<float>();
        Electricity_storage_usage_relative_to_time = new List<float>();
        On = true;

        // Если здание используется в тренировке, добавить в список
        if (!Train && Generator.Instance.agentWithGenerator != null)
        {
            Generator.Instance.agentWithGenerator.UseTickInterfaces.Add(this);
        }

        // Установка начальных значений для типа здания
        SetInitialElectricityValues();
    }

    public void Tick_activation()
    {
        GetBatteryIn();
        Electricity_storage_capacity += Input_electricity;

        if (EnableLook != null && DisableLook != null)
        {
            EnableLook.SetActive(On);
            DisableLook.SetActive(!On);
        }

        Electricity_storage_capacity -= Electricity_consumed;
        Electricity_storage_capacity = Mathf.Clamp(Electricity_storage_capacity, 0, Electricity_storage_capacity_MAX);
        On = Electricity_storage_capacity > 0;
    }

    private void SetInitialElectricityValues()
    {
        switch (Type)
        {
            case "House":
                Electricity_storage_capacity = 10f;
                Electricity_storage_capacity_MAX = 100f;
                break;
            case "Office":
                Electricity_storage_capacity = 10f;
                Electricity_storage_capacity_MAX = 500f;
                break;
            case "Mall":
                Electricity_storage_capacity = 10f;
                Electricity_storage_capacity_MAX = 800f;
                break;
            case "Factory":
                Electricity_storage_capacity = 10f;
                Electricity_storage_capacity_MAX = 1200f;
                break;
            case "School":
                Electricity_storage_capacity = 10f;
                Electricity_storage_capacity_MAX = 300f;
                break;
            case "Restaurant":
                Electricity_storage_capacity = 10f;
                Electricity_storage_capacity_MAX = 200f;
                break;
            case "Hotel":
                Electricity_storage_capacity = 10f;
                Electricity_storage_capacity_MAX = 600f;
                break;
            case "SportCenter":
                Electricity_storage_capacity = 10f;
                Electricity_storage_capacity_MAX = 500f;
                break;
            case "Hospital":
                Electricity_storage_capacity = 10f;
                Electricity_storage_capacity_MAX = 900f;
                break;
            case "Factory2":
                Electricity_storage_capacity = 10f;
                Electricity_storage_capacity_MAX = 1300f;
                break;
            case "Placeholder":
                Electricity_storage_capacity = 0f;
                Electricity_storage_capacity_MAX = 0f;
                break;
            default:
                Electricity_storage_capacity = 0f;
                Electricity_storage_capacity_MAX = 200f;
                break;
        }
    }

    public void GetBatteryIn()
    {
        if (!Generator.Instance.agentWithGenerator.IsTraining)
        {
            Electricity_usage_relative_to_time.Add(Electricity_consumed);
            Electricity_storage_usage_relative_to_time.Add(Electricity_storage_capacity);
        }

        float hourNormalized = Generator.Instance.agentWithGenerator.CurrentHour / 24f;

        switch (Type)
        {
            case "House":
                Electricity_consumed = Mathf.Abs(Mathf.Sin((hour - 6f) / 12f * Mathf.PI)) * Random.Range(2f, 4f);
                break;
            case "Office":
                Electricity_consumed = Mathf.Abs(Mathf.Sin(hourNormalized * Mathf.PI)) * Random.Range(3f, 6f);
                break;
            case "Mall":
                Electricity_consumed = Mathf.Max(0, Mathf.Sin(hourNormalized * Mathf.PI * 2 - Mathf.PI / 3)) * Random.Range(5f, 8f);
                break;
            case "Factory":
                Electricity_consumed = Mathf.Clamp01(Mathf.Sin(hourNormalized * Mathf.PI - Mathf.PI / 2)) * Random.Range(6f, 10f);
                break;
            case "School":
                Electricity_consumed = Mathf.Clamp01(Mathf.Sin((hour - 8f) / 8f * Mathf.PI)) * Random.Range(4f, 7f);
                break;
            case "Restaurant":
                Electricity_consumed = Mathf.Clamp01(Mathf.Sin((hour - 17f) / 7f * Mathf.PI)) * Random.Range(3f, 6f);
                break;
            case "Hotel":
                Electricity_consumed = (0.5f + 0.5f * Mathf.Cos(hourNormalized * Mathf.PI * 2)) * Random.Range(4f, 5f);
                break;
            case "SportCenter":
                Electricity_consumed = Mathf.Clamp01(Mathf.Sin((hour - 10f) / 12f * Mathf.PI)) * Random.Range(5f, 9f);
                break;
            case "Hospital":
                Electricity_consumed = (1f + 0.2f * Mathf.Sin(hourNormalized * Mathf.PI)) * Random.Range(5f, 6f);
                break;
            case "Factory2":
                Electricity_consumed = Mathf.Abs(Mathf.Sin(hourNormalized * Mathf.PI - Mathf.PI / 3)) * Random.Range(7f, 11f);
                break;
            case "Placeholder":
                Electricity_consumed = 0f;
                break;
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

    public bool Is_active()
    {
        return Electricity_storage_capacity>0;
    }

    public bool Is_need()
    {
        return true;
    }

    public void Set_hour(int newHour)
    {
        hour = newHour;
        SetInitialElectricityValues();
    }

    public int Get_type()
    {
        if (Enum.TryParse(Type, out BuildingType buildingType))
        {
            return (int)buildingType;
        }
        return 0; // Если тип не определен
    }

    public string GetResourceInfo()
    {
        return $"TYPE: {Type}\n" +
               $"MAX BANK: {Electricity_storage_capacity_MAX}\n" +
               $"CURR BANK: {Electricity_storage_capacity}\n" +
               $"Electricity Consumed: {Electricity_consumed}\n" +
               $"Electricity Received: {Input_electricity}";
    }
}
