namespace Luminator.PeripheralDimmer.Processor
{
    using System;
    using System.Xml.Serialization;
    
    using Luminator.PeripheralDimmer.Processor.Interfaces;

    /// <summary>
    /// Tuning parameters to control when to switch to next or previous range scale.
    /// </summary>
    [Serializable]
    public class DimmerProcessorTuningParams : IDimmerProcessorTuningParams
    {
        #region Public Properties

        /// <summary>
        /// The upper limit value in range 1 to switch to range 2
        /// </summary>
        [XmlElement]
        public ushort Range1Upper { get; set; }

        /// <summary>
        /// The lower limit value in range 2 to switch to range 1
        /// </summary>
        [XmlElement]
        public ushort Range2Lower { get; set; }

        /// <summary>
        /// The upper limit value in range 2 to switch to range 3
        /// </summary>

        [XmlElement]
        public ushort Range2Upper { get; set; }

        /// <summary>
        /// The lower limit value in range 3 to switch to range 2
        /// </summary>
        [XmlElement]
        public ushort Range3Lower { get; set; }

        /// <summary>
        /// The upper limit value in range 3 to switch to range 4
        /// </summary>
        [XmlElement]
        public ushort Range3Upper { get; set; }

        /// <summary>
        /// The lower limit value in range 4 to switch to range 3
        /// </summary>
        [XmlElement]
        public ushort Range4Lower { get; set; }

        /// <summary>
        /// The number of brightness steps to generate
        /// </summary>
        [XmlElement]
        public int Steps { get; set; }

        /// <summary>
        /// The number of milliseconds to delay between each set brightness
        /// </summary>
        [XmlElement]
        public int IntervalDelay { get; set; }

        /// <summary>
        /// Generally higher brightness value = higher brightness, this indicates the inverse behavior of the TFT
        /// </summary>
        [XmlElement]
        public bool InvertBrightness { get; set; }

        /// <summary>
        /// Puts the dimmer processor into debug mode
        /// </summary>
        [XmlElement]
        public bool DimmerProcessorDebugMode { get; set; }

        #endregion

        #region Constructors

        public DimmerProcessorTuningParams()
        {
            Range2Lower = 0x4ccc;
            Range3Lower = 0x4ccc;
            Range4Lower = 0x4ccc;

            Range1Upper = 0xcccc;
            Range2Upper = 0xcccc;
            Range3Upper = 0xcccc;

            Steps = 8;
            IntervalDelay = 25;
            InvertBrightness = false;
        }

        #endregion

        #region Public Methods and Operators
        #endregion

        #region Member Methods and Operators
        #endregion
    }
}
