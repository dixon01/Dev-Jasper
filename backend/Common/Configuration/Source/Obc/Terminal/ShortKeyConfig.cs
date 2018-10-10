// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShortKeyConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Represent the actions which should be started by pressing a shortkey
//   For each DFA.State may have an own ShortKeyConfig
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Terminal
{
    using System.Text;
    using System.Xml.Serialization;

    using Gorba.Motion.Obc.CommonEmb;

    /// <summary>
    ///   Represent the actions which should be started by pressing a short key
    ///   For each DFA.State may have an own ShortKeyConfig
    /// </summary>
    public class ShortKeyConfig
    {
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        [XmlAttribute("Version")]
        public int Version
        {
            get
            {
                return 1;
            }

            // ReSharper disable once ValueParameterNotUsed
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [XmlAttribute("Description")]
        public string Description
        {
            get
            {
                return "Possible action values: " + EnumUtil.GetAllEnumValues<CommandName>();
            }

            // ReSharper disable once ValueParameterNotUsed
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the F1 long action.
        /// </summary>
        public CommandConfigItem F1LongAction { get; set; }

        /// <summary>
        /// Gets or sets the F1 short action.
        /// </summary>
        public CommandConfigItem F1ShortAction { get; set; }

        /// <summary>
        /// Gets or sets the F2 long action.
        /// </summary>
        public CommandConfigItem F2LongAction { get; set; }

        /// <summary>
        /// Gets or sets the F2 short action.
        /// </summary>
        public CommandConfigItem F2ShortAction { get; set; }

        /// <summary>
        /// Gets or sets the F3 long action.
        /// </summary>
        public CommandConfigItem F3LongAction { get; set; }

        /// <summary>
        /// Gets or sets the F3 short action.
        /// </summary>
        public CommandConfigItem F3ShortAction { get; set; }

        /// <summary>
        /// Gets or sets the F4 long action.
        /// </summary>
        public CommandConfigItem F4LongAction { get; set; }

        /// <summary>
        /// Gets or sets the F4 short action.
        /// </summary>
        public CommandConfigItem F4ShortAction { get; set; }

        /// <summary>
        /// Gets or sets the F5 long action.
        /// </summary>
        public CommandConfigItem F5LongAction { get; set; }

        /// <summary>
        /// Gets or sets the F5 short action.
        /// </summary>
        public CommandConfigItem F5ShortAction { get; set; }

        /// <summary>
        /// Gets or sets the F6 short action.
        /// </summary>
        public CommandConfigItem F6ShortAction { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            // ReSharper disable PossiblyMistakenUseOfParamsMethod
            var sb = new StringBuilder();
            sb.AppendFormat(null, "F1ShortAction:\t{0}\n", this.F1ShortAction);
            sb.AppendFormat(null, "F1LongAction:\t{0}\n", this.F1LongAction);
            sb.AppendFormat(null, "F2ShortAction:\t{0}\n", this.F2ShortAction);
            sb.AppendFormat(null, "F2LongAction:\t{0}\n", this.F2LongAction);
            sb.AppendFormat(null, "F3ShortAction:\t{0}\n", this.F3ShortAction);
            sb.AppendFormat(null, "F3LongAction:\t{0}\n", this.F3LongAction);
            sb.AppendFormat(null, "F4ShortAction:\t{0}\n", this.F4ShortAction);
            sb.AppendFormat(null, "F4LongAction:\t{0}\n", this.F4LongAction);
            sb.AppendFormat(null, "F5ShortAction:\t{0}\n", this.F5ShortAction);
            sb.AppendFormat(null, "F5LongAction:\t{0}", this.F5LongAction);
            sb.AppendFormat(null, "F6ShortAction:\t{0}\n", this.F6ShortAction);
            return sb.ToString();
            // ReSharper restore PossiblyMistakenUseOfParamsMethod
        }
    }
}