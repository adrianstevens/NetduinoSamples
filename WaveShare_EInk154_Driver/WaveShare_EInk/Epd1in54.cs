using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace WaveShare_EInk
{
    public class Epd1in54 : EdpIf
    {
        // Display resolution
        static byte EPD_WIDTH = 200;
        static byte EPD_HEIGHT = 200;

        // EPD1IN54 commands
        static byte DRIVER_OUTPUT_CONTROL = 0x01;
        static byte BOOSTER_SOFT_START_CONTROL = 0x0C;
        static byte GATE_SCAN_START_POSITION = 0x0F;
        static byte DEEP_SLEEP_MODE = 0x10;
        static byte DATA_ENTRY_MODE_SETTING = 0x11;
        static byte SW_RESET = 0x12;
        static byte TEMPERATURE_SENSOR_CONTROL = 0x1A;
        static byte MASTER_ACTIVATION = 0x20;
        static byte DISPLAY_UPDATE_CONTROL_1 = 0x21;
        static byte DISPLAY_UPDATE_CONTROL_2 = 0x22;
        static byte WRITE_RAM = 0x24;
        static byte WRITE_VCOM_REGISTER = 0x2C;
        static byte WRITE_LUT_REGISTER = 0x32;
        static byte SET_DUMMY_LINE_PERIOD = 0x3A;
        static byte SET_GATE_TIME = 0x3B;
        static byte BORDER_WAVEFORM_CONTROL = 0x3C;
        static byte SET_RAM_X_ADDRESS_START_END_POSITION = 0x44;
        static byte SET_RAM_Y_ADDRESS_START_END_POSITION = 0x45;
        static byte SET_RAM_X_ADDRESS_COUNTER = 0x4E;
        static byte SET_RAM_Y_ADDRESS_COUNTER = 0x4F;
        static byte TERMINATE_FRAME_READ_WRITE = 0xFF;

        public static readonly byte[] LUT_Full_Update =
        {
            0x02, 0x02, 0x01, 0x11, 0x12, 0x12, 0x22, 0x22,
            0x66, 0x69, 0x69, 0x59, 0x58, 0x99, 0x99, 0x88,
            0x00, 0x00, 0x00, 0x00, 0xF8, 0xB4, 0x13, 0x51,
            0x35, 0x51, 0x51, 0x19, 0x01, 0x00
        };

        public static readonly byte[] LUT_Partial_Update =
        {
            0x10, 0x18, 0x18, 0x08, 0x18, 0x18, 0x08, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x13, 0x14, 0x44, 0x12,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        public Epd1in54(Cpu.Pin chipSelectPin, Cpu.Pin dcPin, Cpu.Pin resetPin, Cpu.Pin busyPin, SPI.SPI_module spiModule = SPI.SPI_module.SPI1, uint speedKHz = (uint)9500)
            : base(chipSelectPin, dcPin, resetPin, busyPin, spiModule, speedKHz)
        {


        }

        public void Init(byte[] lut) //something init string (defined below)
        {
        

            Reset();

            SendCommand(DRIVER_OUTPUT_CONTROL);
            SendData(199);
            SendData(199 >> 8);
            SendData(0x00);                     // GD = 0; SM = 0; TB = 0;

            SendCommand(BOOSTER_SOFT_START_CONTROL);
            SendData(0xD7);
            SendData(0xD6);
            SendData(0x9D);

            SendCommand(WRITE_VCOM_REGISTER);
            SendData(0xA8);                     // VCOM 7C

            SendCommand(SET_DUMMY_LINE_PERIOD);
            SendData(0x1A);                     // 4 dummy lines per gate

            SendCommand(SET_GATE_TIME);
            SendData(0x08);                     // 2us per line

            SendCommand(DATA_ENTRY_MODE_SETTING);
            SendData(0x03);                     // X increment; Y increment

            SetLookUpTable(lut);



            /*      SendCommand(DRIVER_OUTPUT_CONTROL);
                  SendData((EPD_HEIGHT - 1) & 0xFF);
                  SendData(((EPD_HEIGHT - 1) >> 8) & 0xFF);
                  SendData(0x00);                     // GD = 0; SM = 0; TB = 0;
                  SendCommand(BOOSTER_SOFT_START_CONTROL);
                  SendData(0xD7);
                  SendData(0xD6);
                  SendData(0x9D);
                  SendCommand(WRITE_VCOM_REGISTER);
                  SendData(0xA8);                     // VCOM 7C
                  SendCommand(SET_DUMMY_LINE_PERIOD);
                  SendData(0x1A);                     // 4 dummy lines per gate
                  SendCommand(SET_GATE_TIME);
                  SendData(0x08);                     // 2us per line
                  SendCommand(DATA_ENTRY_MODE_SETTING);
                  SendData(0x03);                     // X increment; Y increment
                  SetLookUpTable(lut);*/

            /* EPD hardware init end */
        }

        void SendCommand(byte command)
        {
            dataCommandPort.Write(Command);
            Write(command);
        }

        void SendData(int data)
        {
            SendData((byte)data);
        }

        void SendData(byte data)
        {
            dataCommandPort.Write(Data);
            Write(data);
        }

        void SendData(byte[] data)
        {
            dataCommandPort.Write(Data);
            spi.Write(data);
        }

        protected void Write(byte data)
        {
            spi.Write(new byte[] { data });
        }


        void WaitUntilIdle()
        {
            while (busyPort.Read() == true)
            {
                DelayMs(50);
            }
        }

        void Reset()
        {
            resetPort.Write(false);
            DelayMs(200);
            resetPort.Write(true);
            DelayMs(200);
        }

        void SetLookUpTable(byte[] lookupTableData)
        {
            SendCommand(WRITE_LUT_REGISTER);
            SendData(lookupTableData);

        /*    for(int i = 0; i < 30; i++)
            {
                SendData(lookupTableData[i]);
            }*/
        }

        public void SetFrameMemory(byte[] image_buffer,
                                int x,
                                int y,
                                int image_width,
                                int image_height)
        {
            int x_end;
            int y_end;

            if ( image_buffer == null ||
                x< 0 || image_width < 0 ||
                y< 0 || image_height < 0 )
            {
                return;
            }
            /* x point must be the multiple of 8 or the last 3 bits will be ignored */
            x &= 0xF8;
            image_width &= 0xF8;
            if (x + image_width >= EPD_WIDTH)
            {
                x_end = EPD_WIDTH - 1;
            }
            else
            {
                x_end = x + image_width - 1;
            }
            if (y + image_height >= EPD_HEIGHT)
            {
                y_end = EPD_HEIGHT - 1;
            }
            else
            {
                y_end = y + image_height - 1;
            }
            SetMemoryArea(x, y, x_end, y_end);
            SetMemoryPointer(x, y);
            SendCommand(WRITE_RAM);
            /* send the image data */
            for (int j = 0; j < y_end - y + 1; j++)
            {
                for (int i = 0; i < (x_end - x + 1) / 8; i++)
                {
                    SendData(image_buffer[i + j * (image_width / 8)]);
                }
            }
        }

        public void SetFrameMemory(byte[] image_buffer) 
        {
            SetMemoryArea(0, 0, EPD_WIDTH - 1, EPD_HEIGHT - 1);
            SetMemoryPointer(0, 0);
            SendCommand(WRITE_RAM);
            /* send the image data */
            for (int i = 0; i< EPD_WIDTH / 8 * EPD_HEIGHT; i++)
            {
                SendData(image_buffer[i]);
            }
        }

        public void ClearFrameMemory(byte color)
        {
            SetMemoryArea(0, 0, EPD_WIDTH - 1, EPD_HEIGHT - 1);
            SetMemoryPointer(0, 0);
            SendCommand(WRITE_RAM);
            /* send the color data */
            for (int i = 0; i < EPD_WIDTH / 8 * EPD_HEIGHT; i++)
            {
                SendData(color);
            }
        }

        public void DisplayFrame()
        {
            SendCommand(DISPLAY_UPDATE_CONTROL_2);
            SendData(0xC4);
            SendCommand(MASTER_ACTIVATION);
            SendCommand(TERMINATE_FRAME_READ_WRITE);
            WaitUntilIdle();
        }

        void SetMemoryArea(int x_start, int y_start, int x_end, int y_end)
        {
            SendCommand(SET_RAM_X_ADDRESS_START_END_POSITION);
            /* x point must be the multiple of 8 or the last 3 bits will be ignored */
            SendData((x_start >> 3) & 0xFF);
            SendData((x_end >> 3) & 0xFF);
            SendCommand(SET_RAM_Y_ADDRESS_START_END_POSITION);
            SendData(y_start & 0xFF);
            SendData((y_start >> 8) & 0xFF);
            SendData(y_end & 0xFF);
            SendData((y_end >> 8) & 0xFF);
        }

        void SetMemoryPointer(int x, int y)
        {
            SendCommand(SET_RAM_X_ADDRESS_COUNTER);
            /* x point must be the multiple of 8 or the last 3 bits will be ignored */
            SendData((x >> 3) & 0xFF);
            SendCommand(SET_RAM_Y_ADDRESS_COUNTER);
            SendData(y & 0xFF);
            SendData((y >> 8) & 0xFF);
            WaitUntilIdle();
        }

        void Sleep()
        {
            SendCommand(DEEP_SLEEP_MODE);
            WaitUntilIdle();
        }
    }
}
