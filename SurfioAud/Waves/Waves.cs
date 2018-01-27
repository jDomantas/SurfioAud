namespace SurfioAud.Waves
{
    class SimulatedWave : IWave
    {
        private const int Width = 800;
        private const double Dampening = 0.011;
        private const double Tension = 0.015;
        private const double Spread = 0.6;

        private readonly double[] _position;
        private readonly double[] _speed;
        private readonly double[] _lDeltas, _rDeltas;
        
        public SimulatedWave()
        {
            _position = new double[Width];
            _speed = new double[Width];
            _lDeltas = new double[Width];
            _rDeltas = new double[Width];
        }

        public double GetHeight(double x)
        {
            int temp = (int) x;
            return _position[temp];
        }

        public void Update(double _)
        {
            const int passes = 4;

            // do some passes where columns pull on their neighbours
            for (int j = 0; j < passes; j++)
            {
                for (int i = 0; i < Width; i++)
                {
                    double x = -_position[i];
                    _speed[i] += (Tension * x - _speed[i] * Dampening) / passes;
                    _position[i] += _speed[i] / passes;
                }

                for (int i = 0; i < Width; i++)
                {
                    _lDeltas[i] = _rDeltas[i] = 0;
                }

                for (int i = 0; i < Width; i++)
                {
                    if (i > 0)
                    {
                        _lDeltas[i] = Spread * (_position[i] - _position[i - 1]);
                        _speed[i - 1] += _lDeltas[i];
                    }
                    if (i < Width - 1)
                    {
                        _rDeltas[i] = Spread * (_position[i] - _position[i + 1]);
                        _speed[i + 1] += _rDeltas[i];
                    }
                }

                for (int i = 0; i < Width; i++)
                {
                    if (i > 0)
                        _position[i - 1] += _lDeltas[i] * 0.1;
                    if (i < Width - 1)
                        _position[i + 1] += _rDeltas[i] * 0.1;
                }
            }
        }

        public void Splash()
        {
            _speed[Width / 4] += 15;
        }
    }
}
