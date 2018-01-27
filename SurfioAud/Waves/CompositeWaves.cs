namespace SurfioAud.Waves
{
    class CompositeWaves : IWaves
    {
        private readonly IWaves[] _parts;

        public CompositeWaves(params IWaves[] parts)
        {
            _parts = parts;
        }

        public void Update(double dt)
        {
            for (int i = 0; i < _parts.Length; i++)
            {
                _parts[i].Update(dt);
            }
        }

        public double GetHeight(int x)
        {
            double total = 0;
            for (int i = 0; i < _parts.Length; i++)
            {
                total += _parts[i].GetHeight(x);
            }
            return total;
        }
    }
}
