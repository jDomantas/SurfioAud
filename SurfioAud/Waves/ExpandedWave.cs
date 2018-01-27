using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfioAud.Waves
{
    class ExpandedWave : IWave
    {
        private readonly IWave _wave;
        private readonly double _scale;

        public ExpandedWave(IWave wave, double scale)
        {
            _wave = wave;
            _scale = scale;
        }

        public void Update(double dt, double playerPosition)
        {
            _wave.Update(dt, playerPosition * _scale);
        }

        public double GetHeight(double x)
        {
            return _wave.GetHeight(x / _scale);
        }
    }
}
