public interface IUseTick
    {
        public void Use();
        public void SetBatteryIn(float power);

        public int Type();
        public float GetBatteryOut();

        public bool IsNeed();
    }