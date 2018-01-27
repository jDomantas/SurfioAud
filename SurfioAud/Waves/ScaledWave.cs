namespace SurfioAud.Waves
{
    class ScaledWave : IWave
    {
        private readonly IWave _wave;
        private readonly double _scale;

        public ScaledWave(IWave baseWave, double scale)
        {
            _wave = baseWave;
            _scale = scale;
        }

        public void Update(double dt, double playerPosition)
        {
            _wave.Update(dt, playerPosition);
        }

        public void MakeSplash(double position)
        {
            _wave.MakeSplash(position);
        }

        public double GetHeight(double x)
        {
            return _wave.GetHeight(x) * _scale;
        }
    }
}
