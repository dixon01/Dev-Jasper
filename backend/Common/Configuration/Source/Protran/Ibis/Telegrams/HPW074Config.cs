// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HPW074Config.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HPW074Config type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis.Telegrams
{
    using System;
    using System.ComponentModel;

    using Gorba.Common.Configuration.Protran.Common;

    /// <summary>
    /// Configuration for the special text reference telegram.
    /// </summary>
    [Serializable]
    public class HPW074Config : TelegramConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HPW074Config"/> class.
        /// </summary>
        public HPW074Config()
        {
            this.SpecialTextFile = @"D:\infomedia\layout\specialtext.csv";
            this.Encoding = "UTF-8";
        }

        /// <summary>
        /// Gets or sets the location of the special text CSV file.
        /// Default value is: <code>D:\infomedia\layout\specialtext.csv</code>
        /// </summary>
        public string SpecialTextFile { get; set; }

        /// <summary>
        /// Gets or sets the encoding of the CSV file.
        /// Default value is: UTF-8.
        /// </summary>
        public string Encoding { get; set; }

        /// <summary>
        /// Gets or sets the usage of this telegram.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public override GenericUsage UsedFor { get; set; }

        /// <summary>
        /// Gets or sets the XML element called Answer.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public override Answer Answer { get; set; }
    }
}
