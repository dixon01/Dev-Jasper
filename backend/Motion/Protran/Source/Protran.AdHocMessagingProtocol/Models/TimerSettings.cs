namespace Luminator.Motion.Protran.AdHocMessagingProtocol.Models
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    using Luminator.Motion.Protran.AdHocMessagingProtocol.Interfaces;

    [Serializable]
    public class TimerSettings : ITimerSettings
    {
        public TimerSettings()
        {
            this.Interval = TimeSpan.Zero;
        }

        public TimerSettings(TimeSpan interval)
        {
            this.Interval = interval;
        }

        public TimerSettings(int seconds)
        {
            if (seconds < 0)
            {
                throw new ArgumentException("TIme in seconds must be greater than or equal to zero!");
            }

            this.Interval = seconds == 0 ? TimeSpan.Zero : TimeSpan.FromSeconds(seconds);
        }

        [XmlIgnore]
        public TimeSpan Interval { get; set; }

        [XmlElement("Interval", DataType = "duration")]
        public string IntervalXml
        {
            get => XmlConvert.ToString(this.Interval);

            set => this.Interval = XmlConvert.ToTimeSpan(value);
        }

        /// <summary>Test if the TimeSpan is valid non-null and not TimeSpan.Zero.</summary>
        public bool IsValid => this.Interval != null && this.Interval != TimeSpan.Zero;
    }
}