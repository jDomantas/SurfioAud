using System;

namespace SurfioAud.Waves
{
    class SinWave : IWave
    {
        private readonly double _period;

        public SinWave(double period)
        {
            _period = period;
        }

        public void Update(double dt, double playerPosition)
        {
        }

        public double GetHeight(double x)
        {
            return Math.Sin(x * Math.PI * 2 / _period);
        }
    }
}
