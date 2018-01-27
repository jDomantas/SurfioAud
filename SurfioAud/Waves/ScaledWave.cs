namespace SurfioAud.Waves
{
    class ScaledWave : IWave
    {
        private readonly IWave _baseWave;
        private readonly double _scale;

        public ScaledWave(IWave baseWave, double scale)
        {
            _baseWave = baseWave;
            _scale = scale;
        }

        public void Update(double dt, double playerPosition)
        {
            _baseWave.Update(dt, playerPosition);
        }

        public double GetHeight(double x)
        {
            return _baseWave.GetHeight(x) * _scale;
        }
    }
}
