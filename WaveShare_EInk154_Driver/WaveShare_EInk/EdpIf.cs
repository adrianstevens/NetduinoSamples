using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System.Threading;

namespace WaveShare_EInk
{
    public class EdpIf
    {
        SPI spi;

        public OutputPort CSPin { get; set; }
        public OutputPort ResetPin { get; set; }
        public OutputPort DCPin { get; set; }
        public OutputPort BusyPin { get; set; }
        

        public EdpIf()
        {
        }


        public void DelayMs(int millseconds)
        {
            Thread.Sleep(millseconds);
        }

        public void SpiTransfer (byte[] data)
        {
            CSPin.Write(false);
            spi.Write(data);
            CSPin.Write(true);
        }

        public void Init ()
        {
            var spiConfig = new SPI.Configuration(
                ChipSelect_Port: Pins.GPIO_PIN_D4,      // Chip select is digital 4
                ChipSelect_ActiveState: false,          // Chip select is active low
                ChipSelect_SetupTime: 0,                // Amount of time between selection and the clock starting
                ChipSelect_HoldTime: 0,                 // Amount of time the device must be active after the data has been read
                Clock_Edge: true,                       // Sample on the rising edge
                Clock_IdleState: false,                 // Clock is idle when low
                Clock_RateKHz: 2000,                    // 2Mhz clock speed 
                SPI_mod: SPI_Devices.SPI1               // Only setting avaliabe
            );

            //MSBFirst, SPI0: polarity 0, phase 0, edge 1

            spi = new SPI(spiConfig);
        }
    }
}