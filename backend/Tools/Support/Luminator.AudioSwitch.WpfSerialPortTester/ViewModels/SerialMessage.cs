using System;

namespace Luminator.AudioSwitch.WpfSerialPortTester.ViewModels
{
    public class SerialMessage
    {
        private int Id { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Name { get; set; }

        public object ObjectData { get; set; }

        public SerialMessage()
        {
            TimeStamp = DateTime.Now;
        }
    }
}