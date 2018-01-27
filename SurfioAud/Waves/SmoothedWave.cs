using System;

namespace SurfioAud.Waves
{
    class SmoothedWave : IWave
    {
        private readonly IWave _wave;
        private readonly int _distance;

        public SmoothedWave(IWave wave, int d)
        {
            _wave = wave;
            _distance = d;
        }
        
        public void Update(double dt, double playerPosition)
        {
            _wave.Update(dt, playerPosition);
        }

        public double GetHeight(double x)
        {
            double total = 0;
            for (int i = 0; i < _distance; i++)
            {
                double prev = _wave.GetHeight(x - i);
                double next = _wave.GetHeight(x - i + _distance);
                double step = (double)i / _distance;
                step = -6 + 12 * step;
                double e = Math.Exp(step);
                double s = e / (e + 1);
                total += prev * (1 - s) + next * s;
            }
            return total / _distance;
        }
    }
}
