namespace SurfioAud.Waves
{
    class MovingWave : IWave
    {
        private readonly IWave _wave;
        private readonly double _speed;
        private double _offset;

        public MovingWave(IWave wave, double speed)
        {
            _wave = wave;
            _speed = speed;
            _offset = 0;
        }

        public void Update(double dt)
        {
            _offset += _speed * dt;
            _wave.Update(dt);
        }

        public double GetHeight(double x)
        {
            return _wave.GetHeight(_offset + x);
        }
    }
}
