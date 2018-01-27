using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfioAud.Waves
{
    class LocalizedWave : IWave
    {
        private readonly IWave _wave;
        private readonly double _start;
        private readonly double _end;

        public LocalizedWave(IWave wave, double start, double end)
        {
            _wave = wave;
            _start = start;
            _end = end;
        }

        public void Update(double dt)
        {
            _wave.Update(dt);
        }

        public double GetHeight(double x)
        {
            if (x <= _start || x >= _end)
            {
                return 0;
            }
            return _wave.GetHeight(x);
        }
    }
}
