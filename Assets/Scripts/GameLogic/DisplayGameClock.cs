using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CityBuilder
{
    public class DisplayGameClock : MonoBehaviour,IUseTick
    {
        //
        // Configurable Parameters
        //
        public TextMeshProUGUI labelTimeOfDay = null;

        //
        // Cached References
        //
        private TimeKeeperSystem timeController = null;
        private int hour=0;

        // void Start()
        // {
        //     timeController = FindObjectOfType<TimeKeeperSystem>();
        //     if (timeController != null)
        //         timeController.OnEventMinutesChanged += () => {
        //             labelTimeOfDay.text = timeController.GetHours().ToString("D2") + " : " + timeController.GetMinutes().ToString("D2");
        //         };
        // }

        private void Start()
        {
            Generator.Instance.builds.Add(this);
        }

        public void Use()
        {
            hour++;
            labelTimeOfDay.text = "Day: " + (hour / 24).ToString() + ", Hour: " + (hour % 24).ToString();
        }

        public float GetBatteryOut()
        {
            return 0f;
        }

        public void SetBatteryIn(float power)
        {
            return;
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
}
