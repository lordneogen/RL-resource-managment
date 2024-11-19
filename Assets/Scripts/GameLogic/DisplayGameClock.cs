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
            Generator.Instance.Builds.Add(this);
        }

        public void Tick_activation()
        {
            hour++;
            labelTimeOfDay.text = "Day: " + (hour / 24).ToString() + ", Hour: " + (hour % 24).ToString();
        }

        public float The_building_consumes_energy_in_the_amount()
        {
            return 0f;
        }

        public void The_generator_gives_energy_in_size(float power)
        {
            return;
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
            this.hour=hour;
        }

        public int Get_type()
        {
            return 0;
        }
    }
}
