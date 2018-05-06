namespace Netduino.Fountation.Sensors.Camera
{
    public interface ICamera
    {
        bool IsInitialized { get; set; }

        bool TakePicture(string path);

        void Initialize();

        string GetVersion();
    }
}
