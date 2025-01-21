namespace AltitudeMapGenerator;

public interface IProgressor
{
    public void Reset(int total);

    public void Progress();
}
