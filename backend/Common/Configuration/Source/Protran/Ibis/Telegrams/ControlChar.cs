// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControlChar.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The control char.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis.Telegrams
{
    /// <summary>
    /// Specifies the type of control character that arrives in the telegram
    /// </summary>
    public enum ControlChar
    {
        /// <summary>
        /// The default option, which means we handle DS003A normally
        /// </summary>
        None,

        /// <summary>
        /// The telegram is from Siemens and has control characters from it.
        /// </summary>
        Siemens,

        /// <summary>
        /// The telegram is from Kruger and has control characters from it.
        /// </summary>
        Krüger,

        /// <summary>
        /// The telegram is from AVL Luxembourg and has control characters from it.
        /// </summary>
        AvlLuxembourg,

        /// <summary>
        /// The telegram has control characters for connection format known as Master.
        /// </summary>
        ConnectionFormatMaster,

        /// <summary>
        /// The telegram is from SYNTUS and has control characters from it.
        /// </summary>
        Syntus,

        /// <summary>
        /// The telegram is from Arriva and has control characters from it.
        /// </summary>
        Arriva
    }
}