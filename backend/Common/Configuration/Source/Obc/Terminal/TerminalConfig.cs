// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TerminalConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TerminalConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Terminal
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Obc.Common;

    /// <summary>
    /// The terminal configuration.
    /// </summary>
    public class TerminalConfig
    {
        /// <summary>
        /// Gets the version.
        /// </summary>
        [XmlAttribute("Version")]
        public int Version
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets or sets the GSM config.
        /// </summary>
        public GsmConfig GsmConfig { get; set; }

        /// <summary>
        /// Gets or sets the alarm input.
        /// </summary>
        public ConfigItem<string> AlarmInput { get; set; }

        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Gets or sets the icenter validation.
        /// </summary>
        public ConfigItem<bool> ICenterValidation { get; set; }

        /// <summary>
        /// Gets or sets the driver pin code.
        /// </summary>
        public ConfigItem<bool> DriverPinCode { get; set; }

        /// <summary>
        /// Gets or sets the driver block.
        /// </summary>
        public ConfigItem<bool> DriverBlock { get; set; }

        /// <summary>
        /// Gets or sets the driver block auto completion.
        /// </summary>
        public ConfigItem<bool> DriverBlock_AutoCompletion { get; set; }

        /// <summary>
        /// Gets or sets the DB button1.
        /// </summary>
        public ConfigItem<string> DB_Btn1 { get; set; }

        /// <summary>
        /// Gets or sets the DB button2.
        /// </summary>
        public ConfigItem<string> DB_Btn2 { get; set; }

        /// <summary>
        /// Gets or sets the DB button3.
        /// </summary>
        public ConfigItem<string> DB_Btn3 { get; set; }

        /// <summary>
        /// Gets or sets the DB button4.
        /// </summary>
        public ConfigItem<string> DB_Btn4 { get; set; }

        /// <summary>
        /// Gets or sets the DB button5.
        /// </summary>
        public ConfigItem<string> DB_Btn5 { get; set; }

        /// <summary>
        /// Gets or sets the DB button6.
        /// </summary>
        public ConfigItem<string> DB_Btn6 { get; set; }

        /// <summary>
        /// Gets or sets the DB button7.
        /// </summary>
        public ConfigItem<string> DB_Btn7 { get; set; }

        /// <summary>
        /// Gets or sets the DB button8.
        /// </summary>
        public ConfigItem<string> DB_Btn8 { get; set; }

        /// <summary>
        /// Gets or sets the DB button1 short name.
        /// </summary>
        public ConfigItem<string> DB_Btn1ShortName { get; set; }

        // pour raison de compatibilité ascendante, difficile de modifier ca

        /// <summary>
        /// Gets or sets the DB button2 short name.
        /// </summary>
        public ConfigItem<string> DB_Btn2ShortName { get; set; }

        /// <summary>
        /// Gets or sets the DB button3 short name.
        /// </summary>
        public ConfigItem<string> DB_Btn3ShortName { get; set; }

        /// <summary>
        /// Gets or sets the DB button4 short name.
        /// </summary>
        public ConfigItem<string> DB_Btn4ShortName { get; set; }

        /// <summary>
        /// Gets or sets the DB button5 short name.
        /// </summary>
        public ConfigItem<string> DB_Btn5ShortName { get; set; }

        /// <summary>
        /// Gets or sets the DB button6 short name.
        /// </summary>
        public ConfigItem<string> DB_Btn6ShortName { get; set; }

        /// <summary>
        /// Gets or sets the DB button7 short name.
        /// </summary>
        public ConfigItem<string> DB_Btn7ShortName { get; set; }

        /// <summary>
        /// Gets or sets the DB button8 short name.
        /// </summary>
        public ConfigItem<string> DB_Btn8ShortName { get; set; }

        /// <summary>
        /// Gets or sets the third party PS.
        /// </summary>
        public ConfigItem<bool> ThirdPartyPS { get; set; }

        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Gets or sets the handle GPRS flag.
        /// </summary>
        public ConfigItem<bool> HandleGprs { get; set; }

        /// <summary>
        /// Gets or sets the speech type.
        /// </summary>
        [XmlElement(ElementName = "SpeechType")]
        public ConfigItem<SpeechType> SpeechType { get; set; }

        /// <summary>
        /// Gets or sets the languages.
        /// </summary>
        public ConfigItem<List<string>> Languages { get; set; }

        /// <summary>
        /// Gets or sets the announcement.
        /// </summary>
        [XmlElement(ElementName = "Announcment")]
        public ConfigItem<AnnouncementType> Announcement { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return "SpeechType:   " + this.SpeechType + "\n" + "Handle GPRS:  " + this.HandleGprs + "\n"
                   + "Alarm input:  " + this.AlarmInput + "\n" + "Languages:    "
                   + string.Join(", ", this.Languages.Value.ToArray()) + "\n" + "Announcement: " + this.Announcement
                   + "\n" + "GSM:\n" + this.GsmConfig;
        }
    }
}