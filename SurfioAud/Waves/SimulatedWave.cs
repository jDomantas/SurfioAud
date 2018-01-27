using System;

namespace SurfioAud.Waves
{
    class SimulatedWave : IWave
    {
        private const int Width = 2000;
        private const double Dampening = 0.011;
        private const double Tension = 0.01;
        private const double Spread = 0.4;

        private readonly double[] _position;
        private readonly double[] _speed;
        private readonly double[] _lDeltas, _rDeltas;

        private double _unsimulated;
        
        public SimulatedWave()
        {
            _position = new double[Width];
            _speed = new double[Width];
            _lDeltas = new double[Width];
            _rDeltas = new double[Width];
            _unsimulated = 0;
        }

        public double GetHeight(double x)
        {
            int index = (int)Math.Round(x);
            index = (index % Width + Width) % Width;
            return _position[index];
        }
        
        public void Update(double dt, double playerPosition)
        {
            _unsimulated += dt;

            const int PassesPerSecond = 10 * 60;

            while (_unsimulated > 1.0 / PassesPerSecond)
            {
                _unsimulated -= 1.0 / PassesPerSecond;
                // do some passes where columns pull on their neighbours
                for (int i = 0; i < Width; i++)
                {
                    double x = -_position[i];
                    _speed[i] += (Tension * x - _speed[i] * Dampening) / 4;
                    _position[i] += _speed[i] / 4;
                }

                for (int i = 0; i < Width; i++)
                {
                    _lDeltas[i] = _rDeltas[i] = 0;
                }

                for (int i = 0; i < Width; i++)
                {
                    int prev = (i - 1 + _position.Length) % _position.Length;
                    int next = (i + 1) % _position.Length;
                    _lDeltas[i] = Spread * (_position[i] - _position[prev]);
                    _speed[prev] += _lDeltas[i];
                    _rDeltas[i] = Spread * (_position[i] - _position[next]);
                    _speed[next] += _rDeltas[i];
                }

                for (int i = 0; i < Width; i++)
                {
                    int prev = (i - 1 + _position.Length) % _position.Length;
                    int next = (i + 1) % _position.Length;
                    _position[prev] += _lDeltas[i] * 0.1;
                    _position[next] += _rDeltas[i] * 0.1;
                }
            }
        }

        public void MakeSplash(double position)
        {
            int index = (int)Math.Round(position);
            index = (index % Width + Width) % Width;
            _speed[index] += 15;
        }
    }
}
