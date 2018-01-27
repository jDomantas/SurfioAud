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

        public void Update(double dt)
        {
            _baseWave.Update(dt);
        }

        public double GetHeight(int x)
        {
            return _baseWave.GetHeight(x) * _scale;
        }
    }
}
