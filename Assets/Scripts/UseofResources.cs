using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UseofResources:MonoBehaviour,IUseTick
{
    public bool On;
    public float bankbattery;
    public float bankbatteryMax;
    public float batteryIn;
    public float batteryOut;
    public string type;
    public List<float> timebatteryuse;
    public List<float> timebatterybank;
    [SerializeField]
    private GameObject EnableLook;
    [SerializeField]
    private GameObject DisableLook;
    //
    public void Use()
    {
        GetBatteryIn();
        bankbattery+=batteryIn;
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
        bankbattery-=batteryOut;
        bankbattery=Mathf.Min(bankbattery, bankbatteryMax);
        if (bankbattery < 0)
        {
            On=false;
        }
    }

    //
    private void Start()
    {
        batteryOut = 0f;
        timebatteryuse = new List<float>();
        On = true;
        Generator.Instance.builds.Add(this);
    }
    //
public void GetBatteryIn()
{
    timebatteryuse.Add(batteryOut);
    timebatterybank.Add(bankbattery);

    if (type == "House")
    {
        // Жилой дом
        batteryOut = (Mathf.Abs(Mathf.Sin((Generator.Instance.houre - 6f) / 12f * Mathf.PI)) 
                     + Mathf.Abs(Mathf.Sin((Generator.Instance.houre - 18f) / 12f * Mathf.PI))) * Random.Range(2f, 4f);
    }
    else if (type == "Office")
    {
        // Офисное здание
        batteryOut = Mathf.Abs(Mathf.Sin(Generator.Instance.houre / 12f * Mathf.PI)) * Random.Range(3f, 6f);
    }
    else if (type == "Mall")
    {
        // Торговый центр
        batteryOut = Mathf.Max(0, Mathf.Sin(Generator.Instance.houre / 24f * Mathf.PI * 2 - Mathf.PI / 3)) * Random.Range(5f, 8f);
    }
    else if (type == "Factory")
    {
        // Завод
        batteryOut = Mathf.Clamp01(Mathf.Sin(Generator.Instance.houre / 12f * Mathf.PI - Mathf.PI / 2)) * Random.Range(6f, 10f);
    }
    else if (type == "School")
    {
        // Школа
        batteryOut = Mathf.Clamp01(Mathf.Sin((Generator.Instance.houre - 8f) / 8f * Mathf.PI)) * Random.Range(4f, 7f);
    }
    else if (type == "Restaurant")
    {
        // Ресторан
        batteryOut = Mathf.Clamp01(Mathf.Sin((Generator.Instance.houre - 17f) / 7f * Mathf.PI)) * Random.Range(3f, 6f);
    }
    else if (type == "Hotel")
    {
        // Гостиница
        batteryOut = (0.5f + 0.5f * Mathf.Cos(Generator.Instance.houre / 24f * Mathf.PI * 2)) * Random.Range(4f, 5f);
    }
    else if (type == "SportCenter")
    {
        // Спортивный центр
        batteryOut = Mathf.Clamp01(Mathf.Sin((Generator.Instance.houre - 10f) / 12f * Mathf.PI)) * Random.Range(5f, 9f);
    }
    else if (type == "Hospital")
    {
        // Больница
        batteryOut = (1f + 0.2f * Mathf.Sin(Generator.Instance.houre / 12f * Mathf.PI)) * Random.Range(5f, 6f);
    }
    else if (type == "Factory2")
    {
        // Фабрика (еще один тип фабрики)
        batteryOut = Mathf.Abs(Mathf.Sin(Generator.Instance.houre / 6f * Mathf.PI - Mathf.PI / 3)) * Random.Range(7f, 11f);
    }
    // Добавьте дополнительные типы зданий по аналогии ниже
    // Например:
    // else if (type == "Warehouse") { ... }
    // else if (type == "Theater") { ... }
    // ...

    // Повторите этот шаблон для всех 25 типов зданий
}


    public void SetBatteryIn(float power)
    {
        batteryIn = power;
    }

    public float GetBatteryOut()
    {
        return batteryOut;
    }
    
    public bool IsNeed()
    {
        return true;
    }

    public int Type()
    {
        if(type=="House") return 1;
        else return 0;
    }
    
    public string GetResourceInfo()
    {
        string info = $"TYPE: {type}\n" +
                      $"MAX BANK: {bankbatteryMax}\n" +
                      $"CURR BANK: {bankbattery}\n" +
                      $"Electricity Consumed: {batteryOut}\n" +
                      $"Electricity Received: {batteryIn}";
        return info;
    }
}