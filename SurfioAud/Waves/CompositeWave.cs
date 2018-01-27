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

        public void Update(double dt)
        {
            foreach (IWave wave in _parts)
            {
                wave.Update(dt);
            }
        }

        public double GetHeight(double x)
        {
            return _parts.Sum(t => t.GetHeight(x));
        }
    }
}
