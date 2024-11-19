
using System;

public interface IUseTick
    {
        public void Tick_activation();
        public void The_generator_gives_energy_in_size(float power);

        public int Get_type();
        public float The_building_consumes_energy_in_the_amount();

        public bool Is_need();

        public bool Is_active();
        public void Set_hour(int hour);
    }