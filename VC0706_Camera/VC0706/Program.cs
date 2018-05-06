using Microsoft.SPOT;
using Microsoft.SPOT.IO;
using System.IO;
using Netduino.Fountation.Sensors.Camera;

namespace Camera_VC0706
{
    public class Program
    {
        public static VC0706 Camera = new VC0706();

        public static void Main()
        {
            Debug.Print("hello");

            var volume = new VolumeInfo("SD");

            RecurseFolders(new DirectoryInfo("SD"));

            Camera.Initialize("COM1", VC0706.PortSpeed.Baud38400, VC0706.Resolution._640x480);

            Camera.SetTVOut(false);

            ShowCameraConfigTest();

            Camera.Initialize("COM1", VC0706.PortSpeed.Baud115200, VC0706.Resolution._160x120);
            TestTakePictures(@"SD\StressTestSmall", 100);

            Camera.SetTVOut(true);
        }

        static void RecurseFolders(DirectoryInfo directory)
        {
            if (directory == null)
                return;

            foreach (var file in directory.GetFiles())
            {
                Debug.Print(file.FullName);
            }

            foreach(var subDir in directory.GetDirectories())
            {
                RecurseFolders(subDir);
            }
        }

        public static void ShowCameraConfigTest()
        {
            Debug.Print("Camera version: " + Camera.GetVersion());
            Debug.Print("Compression: " + Camera.GetCompression());
            Debug.Print("Image size: " + Camera.GetImageSize());
            Debug.Print("Motion detection activation: " + Camera.GetMotionDetectionCommStatus());

            ushort width = 0;
            ushort height = 0;
            ushort zoomWidth = 0;
            ushort zoomHeight = 0;
            ushort pan = 0;
            ushort tilt = 0;

            Camera.GetPanTiltZoom(out width, out height, out zoomWidth, out zoomHeight, out pan, out tilt);

            Debug.Print("PTZ width: " + width.ToString());
            Debug.Print("PTZ height: " + height.ToString());
            Debug.Print("PTZ zoomWidth: " + zoomWidth.ToString());
            Debug.Print("PTZ zoomHeight: " + zoomHeight.ToString());
            Debug.Print("PTZ pan: " + pan.ToString());
            Debug.Print("PTZ tilt: " + tilt.ToString());

            //ShowCameraColorControlMode();
        }

        public static void TestTakePictures(string path, int maxCount = 100)
        {
            Directory.CreateDirectory(path);
            for (var count = 0; count < maxCount; count++)
            {
                var imgPath = Path.Combine(path, "Pic_" + count + ".jpg");
                Debug.Print("Working on " + imgPath);

                Camera.TakePicture(imgPath);
            }
        }
    }
}