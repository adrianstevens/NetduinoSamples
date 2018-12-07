using System;
using Microsoft.SPOT;
using System.IO.Ports;
using System.Threading;

namespace GPS
{
    public class GpsReader
    {
        private readonly object _lock = new object();
        private readonly SerialPort _serialPort;

        private readonly int _timeOut;
        private readonly double _minDistanceBetweenPoints;

        private bool _isStarted;
        private Thread _processor;

        public delegate void LineProcessor(string line);

        public delegate void GpsDataProcessor(GpsPoint gpsPoint);

        public event LineProcessor RawLine;
        public event GpsDataProcessor GpsData;

        public bool IsStarted { get { return _isStarted; } }

        public GpsReader(SerialPort serialPort) : this(serialPort, 250, 0.0)
        { }

        public GpsReader(SerialPort serialPort, int timeOutBetweenReadsInMilliseconds, double minDistanceInMilesBetweenPoints)
        {
            _serialPort = serialPort;
            _timeOut = timeOutBetweenReadsInMilliseconds;
            _minDistanceBetweenPoints = minDistanceInMilesBetweenPoints;
        }

        public bool Start()
        {
            if (_isStarted)
                return false;

            lock (_lock)
            {
                _isStarted = true;
                _processor = new Thread(ThreadProc);
                _processor.Start();
            }
            return true;
        }

        public bool Stop()
        {
            lock (_lock)
            {
                if (!_isStarted)
                    return false;

                _isStarted = false;

                if (!_processor.Join(5000))
                    _processor.Abort();

                return true;
            }
        }

        private void ThreadProc()
        {
            Debug.Print("GPS thread started...");

            if (!_serialPort.IsOpen)
            {
                _serialPort.Open();
            }
            while (_isStarted)
            {
                int bytesToRead = _serialPort.BytesToRead;

                if (bytesToRead > 0)
                {
                    var buffer = new byte[bytesToRead];
                    _serialPort.Read(buffer, 0, buffer.Length);

                    try
                    {
                        string gpsData = new string(System.Text.Encoding.UTF8.GetChars(buffer));
                        ProcessBytes(gpsData);
                    }
                    catch (Exception ex)
                    {
                        // only process lines we can parse.
                        Debug.Print(ex.ToString());
                    }
                }

                Thread.Sleep(_timeOut);
            }
            Debug.Print("GPS thread stopped...");
        }

        private string _data = string.Empty;
        private GpsPoint _lastPoint;
        private DateTime _lastDateTime = DateTime.Now;

        private void ProcessBytes(string gpsData)
        {
            while (gpsData.IndexOf('\n') != -1)
            {
                var parts = gpsData.Split('\n');
                _data += parts[0];
                _data = _data.Trim();
                if (_data != string.Empty)
                {
                    if(_data.IndexOf("$GPRMS") != 0)
                    {
                        int g = 9;
                    }

                    if (_data.IndexOf("$GPRMC") == 0)
                    {
                      //  Debug.Print("GOT $GPRMC LINE");
                        if (GpsData != null)
                        {
                            var gpsPoint = GprmcParser.Parse(_data);

                            if (gpsPoint == null)
                                continue;

                            var isOk = true;

                            if (_lastPoint != null)
                            {
                                var distance = GeoDistanceCalculator.GetDistanceInMiles(gpsPoint.Latitude, gpsPoint.Longitude,
                                                                                _lastPoint.Latitude, _lastPoint.Longitude);
                                double distInFeet = distance * 5280;
                                //  Debug.Print("distance = " + distance + " mi (" + distInFeet + " feet)");
                                if (distance < _minDistanceBetweenPoints)
                                {
                                    // Too close to the last point....don't raise the event
                                    isOk = false;
                                }

                                var timeDelta = (DateTime.Now - _lastDateTime);

                                if (timeDelta.Seconds > 60)
                                {
                                    // A minute has gone by, so update
                                    isOk = true;
                                    _lastDateTime = DateTime.Now;
                                }
                            }
                            _lastPoint = gpsPoint;

                            // Raise the event
                            if (isOk)
                            {
                                GpsData(gpsPoint);
                            }
                        }
                    }

                    RawLine?.Invoke(_data);
                }
                gpsData = parts[1];
                _data = string.Empty;
            }
            _data += gpsData;
        }
    }
}