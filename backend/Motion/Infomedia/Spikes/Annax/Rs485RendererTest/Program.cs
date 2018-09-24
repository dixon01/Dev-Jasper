namespace Rs485RendererTest
{
    using System;
    using System.IO;
    using System.IO.Ports;
    using System.Threading;

    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Infomedia.AnnaxRendererTest.Renderer;
    using Gorba.Motion.Infomedia.RendererBase;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;
    using Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol;
    using Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol.Commands;
    using Gorba.Motion.Infomedia.Spikes.Annax.Rs485RendererTest.Properties;

    using NLog;

    class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static IApplicationLayer app;

        public static void Main(string[] args)
        {
            var buffer = new byte[517];

            var phy = new PhysicalLayer();
            var net = new AscNetworkLayer();
            app = new ApplicationLayer();

            var pcq = new ProducerConsumerQueue<CommandBase>(SendCommand, 1000);

            net.SetPhysicalLayer(phy);
            app.SetNetworkLayer(net);

            StartTestDataProducer(pcq);

            using (var stream = OpenPort())
            {
                // ReSharper disable AccessToDisposedClosure
                phy.DataReady += (s, e) =>
                    {
                        stream.Write(e.Data, e.Offset, e.Count);
                        stream.Flush();
                    };

                pcq.StartConsumer();
                var offset = 0;
                while (true)
                {
                    var read = stream.Read(buffer, offset, buffer.Length - offset);
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

                pcq.StopConsumer();
                // ReSharper restore AccessToDisposedClosure
            }

            Console.WriteLine("Press <enter> to exit");
            Console.ReadLine();
        }

        private static void StartTestDataProducer(ProducerConsumerQueue<CommandBase> pcq)
        {
            var thread = new Thread(() =>
                {
                    Thread.Sleep(2000);
                    pcq.Enqueue(new ClearWindowCommand { WindowNumber = 2 });
                    Thread.Sleep(2000);
                    pcq.Enqueue(new ClearBitmapCommand { BitmapNumber = 2 });
                    Thread.Sleep(2000);
                    pcq.Enqueue(new ClearAllCommand());
                    Thread.Sleep(2000);
                    var text = new byte[10];
                    for (int i = 0; i < text.Length - 1; i++)
                    {
                        text[i] = (byte)('A' + i);
                    }

                    pcq.Enqueue(
                        new TextCommand
                            {
                                BitmapNumber = 12,
                                Width = 120,
                                Height = 10,
                                Spacing = 0,
                                TextAttribute = TextAttr.Center,
                                FontNumber = -9,
                                TextData = text
                            });

                    Thread.Sleep(2000);

                    pcq.Enqueue(
                        new WindowCommand
                        {
                            WindowNumber = 2,
                            X = 0,
                            Y = 20,
                            Width = 120,
                            Height = 10,
                            StartX = 0,
                            StartY = 0,
                            BitmapNumber = 1,
                            BaseAttribute = BaseAttr.Synchronize,
                            DurationAttribute = DurationAttr.Endless,
                            DisplayAttribute = DisplayAttr.Normal,
                            Timing = 0,
                            Counting = 0
                        });
                });
            thread.IsBackground = true;
            thread.Start();
        }

        private static void SendCommand(CommandBase command)
        {
            app.WriteData(NetworkServiceType.Ackd, command);

            // TODO: wait for ACK
            Thread.Sleep(50);
        }

        private static Stream OpenPort()
        {
            var portName = Settings.Default.ComPort;
            Logger.Info("Opening {0} with 9600 baud", portName);
            var port = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
            port.Open();
            return port.BaseStream;
        }
    }

    public class RenderManagerFactory : RenderManagerFactoryBase<IAnnaxRenderContext>
    {
        public RenderManagerFactory()
        {
        }

        protected override ITextRenderEngine<IAnnaxRenderContext> CreateEngine(
            TextRenderManager<IAnnaxRenderContext> manager)
        {
            var engine = new TextRenderEngine(manager);
            //engine.Prepare(this.serialPort);
            return engine;
        }
    }
}

