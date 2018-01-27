namespace SurfioAud.Waves
{
    interface IWave
    {
        void Update(double dt, double playerPosition);
        double GetHeight(double x);
    }
}
