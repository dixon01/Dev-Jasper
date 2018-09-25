namespace Gorba.Motion.Infomedia.EdLtnRendererTest.Protocol
{
    using System;
    using System.IO.Ports;
    using System.Threading;

    using Gorba.Common.Utility.Core;

    using NLog;

    public class LtnSerialPort
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly object locker = new object();

        private readonly SerialPort serialPort;

        private readonly ProducerConsumerQueue<Telegram> sendQueue; 

        public LtnSerialPort(string portName)
        {
            this.serialPort = new SerialPort(portName);
            this.sendQueue = new ProducerConsumerQueue<Telegram>(this.SendTelegram, 1000);
        }

        public void Open()
        {
            this.serialPort.Open();
            this.sendQueue.StartConsumer();
        }

        public void Close()
        {
            this.sendQueue.StopConsumer();
            this.serialPort.Close();
        }

        public void EnqueueTelegram(Telegram telegram)
        {
            this.sendQueue.Enqueue(telegram);
        }

        private void SendTelegram(Telegram telegram)
        {
            Logger.Debug("Sending telegram: {0}", telegram);
            var data = telegram.ToByteArray();

            Logger.Trace(() => "Telegram content: " + BitConverter.ToString(data));

            try
            {
                this.serialPort.Write(data, 0, data.Length);

                var read = this.serialPort.ReadByte();
                if (read == 0x06)
                {
                    read = this.serialPort.ReadByte();
                    if (read == telegram.Code)
                    {
                        Logger.Debug("Got ACK");
                    }
                    else
                    {
                        Logger.Warn("Got wrong ACK: {0:X2}", read);
                    }
                }
                else if (read == 0x15)
                {
                    Logger.Warn("Got NAK");
                }
                else
                {
                    Logger.Warn("Got unknown return code: {0}", read);
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Exception while reading from serial port", ex);
                ThreadPool.QueueUserWorkItem(s =>
                {
                    this.Close();
                    this.Open();
                });
            }
        }
    }
}
