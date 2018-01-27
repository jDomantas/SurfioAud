using System.Linq;

namespace SurfioAud.Waves
{
    class CompositeWave : IWave
    {
        private readonly IWave[] _parts;

        public CompositeWave(params IWave[] parts)
        {
            _parts = parts;
        }

        public void Update(double dt, double playerPosition)
        {
            foreach (IWave wave in _parts)
            {
                wave.Update(dt, playerPosition);
            }
        }

        public void MakeSplash(double position)
        {
            foreach (IWave wave in _parts)
            {
                wave.MakeSplash(position);
            }
        }

        public double GetHeight(double x)
        {
            return _parts.Sum(t => t.GetHeight(x));
        }
    }
}
