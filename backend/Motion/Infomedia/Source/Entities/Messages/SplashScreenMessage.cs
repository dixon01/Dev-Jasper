// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="SplashScreenMessage.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Motion.Infomedia.Entities.Messages
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>The splash screen message.</summary>
    [Serializable]
    public class SplashScreenMessage
    {
        #region Fields

        private TimeSpan duration;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="SplashScreenMessage"/> class.</summary>
        /// <param name="show">The show.</param>
        public SplashScreenMessage(bool show)
        {
            this.Show = show;
            this.duration = TimeSpan.FromSeconds(60);
        }

        /// <summary>Initializes a new instance of the <see cref="SplashScreenMessage" /> class.</summary>
        public SplashScreenMessage()
            : this(true)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the duration of time to show the view.</summary>
        [XmlIgnore]
        public TimeSpan Duration
        {
            get
            {
                return this.duration;
            }

            set
            {
                if (value != TimeSpan.Zero)
                {
                    this.duration = value;
                }
            }
        }

        /// <summary>Gets or sets the duration xml.</summary>
        [XmlAttribute("Duration", DataType = "duration")]
        public string DurationXml
        {
            get
            {
                return XmlConvert.ToString(this.Duration);
            }

            set
            {
                this.Duration = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>Gets or sets a value indicating whether show.</summary>
        public bool Show { get; set; }

        #endregion
    }
}