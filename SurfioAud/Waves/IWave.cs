namespace SurfioAud.Waves
{
    interface IWave
    {
        void Update(double dt);
        double GetHeight(double x);
    }
}
