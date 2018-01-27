using System;

namespace SurfioAud.Waves
{
    class Microwave : IWave
    {
        private const double AmplitudeReduction = 1.25;
        private const double DistanceFromPlayer = 250;
        private const int BufferSize = 3000;
        
        private readonly Microphone _microphone;
        private double _unreadTime;
        private readonly double[] _buffer;
        private readonly double[] _buffer2;
        private readonly double[] _buffer3;


        public Microwave()
        {
            _microphone = new Microphone();
            _unreadTime = 0;
            _buffer = new double[BufferSize];
            _buffer2 = new double[BufferSize];
            _buffer3 = new double[BufferSize];
        }

        public void Update(double dt, double playerPosition)
        {
            double mul = 1 / Math.Pow(AmplitudeReduction, dt);
            for (int i = 0; i < BufferSize; i++)
            {
                _buffer[i] *= mul;
            }

            bool gotChanges = false;
            _unreadTime += 1 / 60.0;// dt;
            while (_unreadTime > 1 / 60.0)
            {
                gotChanges = true;
                _unreadTime -= 1 / 60.0;
                playerPosition %= _buffer.Length;
                int pos = (int)Math.Round(playerPosition - DistanceFromPlayer);
                _buffer[(pos % _buffer.Length + _buffer.Length) % _buffer.Length] = _microphone.GetTickValue();
                pos -= 500;
                _buffer[(pos % _buffer.Length + _buffer.Length) % _buffer.Length] = 0;
            }
            
            if (gotChanges)
            {
                Smooth();
            }
        }

        public void MakeSplash(double position) { }

        private void Smooth()
        {
            const int smoothDistance = 50;

            for (int i = 0; i < BufferSize; i++)
            {
                _buffer2[i] = 0;
                for (int j = 0; j < smoothDistance; j++)
                {
                    _buffer2[i] = Math.Max(_buffer2[i], GetAt(_buffer, i - j));
                }
            }

            for (int i = 0; i < BufferSize; i++)
            {
                _buffer3[i] = 0;
                for (int j = 0; j < smoothDistance; j++)
                {
                    _buffer3[i] += GetAt(_buffer2, i - j);
                }
                _buffer3[i] /= smoothDistance;
            }
        }
        
        public double GetHeight(double x)
        {
            x %= _buffer.Length;
            int first = (int)Math.Floor(x) % _buffer.Length;
            int next = (first + 1) % _buffer.Length;
            double step = x - first;
            step = Math.Min(1, Math.Max(0, step));
            double a = GetAt(_buffer3, first), b = GetAt(_buffer3, next);
            return a * (1 - step) + b * step;
        }

        private T GetAt<T>(T[] array, int index)
        {
            return array[(index % array.Length + array.Length) % array.Length];
        }
    }
}
