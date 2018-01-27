namespace SurfioAud.Waves
{
    class ScaledWaves : IWaves
    {
        private readonly IWaves _baseWaves;
        private readonly double _scale;

        public ScaledWaves(IWaves baseWaves, double scale)
        {
            _baseWaves = baseWaves;
            _scale = scale;
        }

        public void Update(double dt)
        {
            _baseWaves.Update(dt);
        }

        public double GetHeight(int x)
        {
            return _baseWaves.GetHeight(x) * _scale;
        }
    }
}
