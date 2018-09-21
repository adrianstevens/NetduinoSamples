using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System.Threading;

namespace WaveShare_EInk
{
    public class EdpIf
    {
        protected SPI spi;

        protected OutputPort resetPort { get; set; }
        protected OutputPort dataCommandPort { get; set; }
        protected InputPort busyPort { get; set; }


        protected const bool Data = true;
        protected const bool Command = false;

        public EdpIf()
        {
        }

        public EdpIf(Cpu.Pin chipSelectPin, Cpu.Pin dcPin, Cpu.Pin resetPin, Cpu.Pin busyPin, SPI.SPI_module spiModule = SPI.SPI_module.SPI1, uint speedKHz = (uint)9500)
        {

            dataCommandPort = new OutputPort(dcPin, false);
            resetPort = new OutputPort(resetPin, true);
            busyPort = new InputPort(busyPin, true, Port.ResistorMode.Disabled);

            var spiConfig = new SPI.Configuration(
                SPI_mod: spiModule,
                ChipSelect_Port: chipSelectPin,
                ChipSelect_ActiveState: false,
                ChipSelect_SetupTime: 0,
                ChipSelect_HoldTime: 0,
                Clock_IdleState: false,
                Clock_Edge: true,
                Clock_RateKHz: speedKHz);

            spi = new SPI(spiConfig);

            Initialize();
        }


        public void DelayMs(int millseconds)
        {
            Thread.Sleep(millseconds);
        }

        public void SpiTransfer (byte[] data)
        {
       //     CSPin.Write(false);
            spi.Write(data);
       //     CSPin.Write(true);
        }

        void Initialize ()
        {


        }
    }
}