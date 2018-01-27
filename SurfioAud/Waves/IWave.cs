namespace SurfioAud.Waves
{
    interface IWave
    {
        void Update(double dt, double playerPosition);
        void MakeSplash(double position);
        double GetHeight(double x);
    }
}
