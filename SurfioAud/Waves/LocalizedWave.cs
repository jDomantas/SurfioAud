﻿using System;

namespace SurfioAud.Waves
{
    class LocalizedWave : IWave
    {
        private readonly IWave _wave;
        private readonly double _start;
        private readonly double _end;

        public LocalizedWave(IWave wave, double start, double end)
        {
            _wave = wave;
            _start = start;
            _end = end;
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
            double halfFidth = (_end - _start) / 2;
            double middle = (_start + _end) / 2;
            double dist = Math.Abs(x - middle);
            return S(4 - dist / halfFidth * 4) * _wave.GetHeight(x);
        }

        private double S(double x)
        {
            return Math.Exp(x) / (Math.Exp(x) + 1);
        }
    }
}
