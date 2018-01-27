namespace SurfioAud.Waves
{
    interface IWave
    {
        void Update(double dt);
        double GetHeight(int x);
    }
}
