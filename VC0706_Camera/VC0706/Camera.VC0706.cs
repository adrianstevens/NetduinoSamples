using System;
using Microsoft.SPOT;
using System.IO.Ports;
using System.Threading;

namespace Camera_VC0706
{
    public class CameraVC0706 : ICamera, IDisposable
    {
        #region enums

        public enum ColorControl : byte
        {
            Automatic,
            Color,
            BlackWhite,
        }

        
        public enum ImageSize : byte
        {
            Res640x480 = 0x00,
            Res320x240 = 0x11,
            Res160x120 = 0x22,
        }

        public enum ComPortSpeed
        {
            Baud9600 = 0xAEC8,
            Baud19200 = 0x56E4,
            Baud38400 = 0x2AF2,
            Baud57600 = 0x1C4C,
            Baud115200 = 0x0DA6
        }

        #endregion

        private SerialPort _comPort;

        private VC0706Core _vc0706;

        private int _motionImageStartId;
        private string _motionStoragePath;
        private Thread _motionThread;
        private bool _motionStop;

        public delegate bool MotionDetectedHandler(Exception e, int imageSequenceNumber, string imagePath);
        private MotionDetectedHandler _motionDetectionHandler;

        public CameraVC0706 ()
        {
            _vc0706 = new VC0706Core();
        }

        public void Initialize(string comPort, ComPortSpeed baudRate = ComPortSpeed.Baud38400, ImageSize imageSize = ImageSize.Res640x480)
        {
            DetectBaudRate(comPort);
            SetImageSize(imageSize);
            DetectBaudRate(comPort);
            SetPortSpeed(baudRate);

            _vc0706.Initialize(_comPort);
        }

        public void TakePicture (string path)
        {
            _vc0706.TakePicture(path);
        }

        protected void DetectBaudRate(string port)
        {
            var supportedBaudRates = new int[] { 115200, 57600, 38400, 19200, 9600 };

            foreach (int rate in supportedBaudRates)
            {
                OpenComPort(port, rate);

                try
                {
                    _vc0706.SetComPort(_comPort);
                    GetImageSize();

                    Debug.Print("BaudRate detected: " + rate.ToString());
                    return;
                }
                catch (Exception e)
                {

                }
                CloseComPort();
            }
            throw new ApplicationException("BaudRate detection failed - is your camera connected?");
        }

        protected void OpenComPort(string port = "COM1", int baudRate = 38400)
        {
            CloseComPort();

            _comPort = new SerialPort(port, baudRate, Parity.None, 8, StopBits.One);

            _comPort.ReadTimeout = 10 * 2;
            _comPort.WriteTimeout = 10 * 2;

            _comPort.Open();
        }

        protected void SetPortSpeed(ComPortSpeed speed)
        {
            _vc0706.SetPortSpeed((short)speed);

            var comPortName = _comPort.PortName;
            switch (speed)
            {
                case ComPortSpeed.Baud9600:
                    OpenComPort(comPortName, 9600);
                    break;
                case ComPortSpeed.Baud19200:
                    OpenComPort(comPortName, 19200);
                    break;
                case ComPortSpeed.Baud38400:
                    OpenComPort(comPortName, 38400);
                    break;
                case ComPortSpeed.Baud57600:
                    OpenComPort(comPortName, 57600);
                    break;
                case ComPortSpeed.Baud115200:
                    OpenComPort(comPortName, 115200);
                    break;
            }
            Thread.Sleep(100);
        }

        public void SetTVOutput(bool enabled)
        {
            _vc0706.TvOutput(enabled);
        }

        public string GetVersion ()
        {
            return _vc0706.GetVersion();
        }

        public int GetCompression ()
        {
            return _vc0706.GetCompression();
        }

        public bool GetMotionDetectionCommStatus()
        {
            return _vc0706.GetMotionDetectionStatus();
        }

        public ColorControl GetColorStatus ()
        {
            return (ColorControl)_vc0706.GetColorStatus();
        }

        public void SetColorStatus(ColorControl colorControl)
        {
            _vc0706.SetColorControl((byte)colorControl);
        }

        public ImageSize GetImageSize ()
        {
            return (ImageSize)_vc0706.GetImageSize();
        }

        public void SetImageSize (ImageSize imageSize)
        {
            _vc0706.SetImageSize((byte)imageSize);
        }

        public bool IsMotionDetectionEnabled()
        {
            return _vc0706.GetMotionDetectionStatus();
        }

        public void StartAutoMotionDetection(int imageStartId, string storagePath, MotionDetectedHandler motionDetectionHandler)
        {
            if (_motionThread != null)
                throw new ApplicationException("_autoMotionThread");

            if (motionDetectionHandler == null)
                throw new ApplicationException("motionDetectionHandler");

            _motionStop = false;
            _motionImageStartId = imageStartId;
            _motionStoragePath = storagePath;
            _motionDetectionHandler = motionDetectionHandler;
            _motionThread = new Thread(MotionDetectionThread);
            _motionThread.Start();
        }

        public void StopMotionDetection()
        {
            if (_motionThread != null)
            {
                _motionStop = true;
                _motionThread.Join();
                _motionThread = null;
            }
        }

        private void MotionDetectionThread()
        {
            _vc0706.SetMotionDetection(true);
            string imagePath = "";

            while (!_motionStop)
            {
                try
                {
                    if (_vc0706.IsMotionDetectionEnabled())
                    {
                        imagePath = _motionStoragePath + @"\Motion_" + _motionImageStartId.ToString() + ".jpg";
                        if (_motionDetectionHandler(null, _motionImageStartId, imagePath))
                        {
                            TakePicture(imagePath);
                            _motionImageStartId++;
                        }
                    }
                }
                catch (Exception e)
                {
                    _motionDetectionHandler(e, _motionImageStartId, imagePath);
                }
            }
            _vc0706.SetMotionDetection(false);
        }

        void CloseComPort ()
        {
            if (_comPort != null)
            {
                _comPort.Flush();
                _comPort.Close();
                _comPort.Dispose();
                _comPort = null;
            }
        }

        public void Dispose()
        {
            CloseComPort();
            StopMotionDetection();
        }
    }
}
