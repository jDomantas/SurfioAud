namespace SurfioAud.Waves
{
    class ConstantWave : IWave
    {
        private readonly double _value;

        public ConstantWave(double value)
        {
            _value = value;
        }

        public void Update(double dt, double playerPosition) { }

        public void MakeSplash(double potition) { }

        public double GetHeight(double x)
        {
            return _value;
        }
    }
}
