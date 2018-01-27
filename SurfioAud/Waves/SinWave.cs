using System;

namespace SurfioAud.Waves
{
    class SinWave : IWave
    {
        private readonly double _period;
        private readonly double _speed;
        private double _offset;

        public SinWave(double period, double speed)
        {
            _period = period;
            _speed = speed;
            _offset = 0;
        }

        public void Update(double dt)
        {
            _offset += _speed * dt;
        }

        public double GetHeight(int x)
        {
            return Math.Sin((_offset + x) * Math.PI * 2 / _period);
        }
    }
}
