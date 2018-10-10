namespace Luminator.AudioSwitch.EnableAudio
{
    public class SerialPortInfo 
    {
        #region Public Properties

        public SerialPortInfo(string name = "COM4",
                              int baudRate = 115200,
                              int dataBits = 8,
                              string parity = "None",
                              string stopBits = "None",
                              bool dtrControl = false,
                              bool rtsControl = false)
        {
            this.Name = name;
            this.BaudRate = baudRate;
            this.DataBits = dataBits;
            this.Parity = parity;
            this.StopBits = stopBits;
            this.DtrControl = dtrControl;
            this.RtsControl = rtsControl;
        }

        public string Name { get; set; }
        public int BaudRate { get; set; }

        public int DataBits { get; set; }

        public string Parity { get; set; }

        public string StopBits { get; set; }

        public bool DtrControl { get; set; }

        public bool RtsControl { get; set; }

        #endregion
    }
}