namespace Gorba.Motion.Infomedia.Spikes.Annax.AscSpy
{
    using System;
    using System.IO;
    using System.IO.Ports;

    using Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol;
    using Gorba.Motion.Infomedia.Spikes.Annax.AscSpy.Properties;

    using NLog;

    public class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly MemoryStream TestMemory =
            new MemoryStream(
                new byte[]
                    {
                        // telegram 0
                        0x01, 0x15, 0x00, 0xFE, 0x10, 0x21, 0xC0, 0x00, 0x02, 0x00, 0x0D, 0x20, 0x02, 0x00, 0x07, 0x00,
                        0x15, 0x00, 0x0A, 0x02, 0xF8, 0x32, 0x27, 0x00, 0xB5, 0xDD,

                        // telegram 1
                        0x01, 0x07, 0x00, 0x10, 0x21, 0xFE, 0x90, 0x00, 0x02, 0x00, 0x04, 0x65, 0x55
                    });

        public static void Main(string[] args)
        {
            var buffer = new byte[517];

            var phy = new PhysicalLayer();
            var net = new AscNetworkLayer();
            var app = new ApplicationLayer();

            net.SetPhysicalLayer(phy);
            app.SetNetworkLayer(net);

            using (var input = OpenPort())
            {
                var offset = 0;
                while (true)
                {
                    var read = input.Read(buffer, offset, buffer.Length - offset);
                    if (read <= 0)
                    {
                        Logger.Warn("End of Stream");
                        break;
                    }

                    offset += read;
                    while (true)
                    {
                        var decoded = phy.Decode(buffer, 0, offset);
                        offset -= decoded;
                        if (decoded <= 0 || offset <= 0)
                        {
                            break;
                        }

                        Array.Copy(buffer, decoded, buffer, 0, offset);
                    }
                }
            }

            Console.WriteLine("Press <enter> to exit");
            Console.ReadLine();
        }

        private static Stream OpenPort()
        {
            //return TestMemory;
            var portName = Settings.Default.ComPort;
            Logger.Info("Opening {0} with 9600 baud", portName);
            var port = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
            port.Open();
            return port.BaseStream;
        }
    }
}
