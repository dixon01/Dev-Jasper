// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS036Parser.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS036Parser type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers
{
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Protran.Ibis.Parsers.Verification;

    /// <summary>
    /// Special parser for DS036 which has a special status code.
    /// </summary>
    public class DS036Parser : AnswerWithDS130Parser<DS036>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS036Parser"/> class.
        /// </summary>
        public DS036Parser()
            : base("hP", new Digit(4))
        {
        }

        /// <summary>
        /// Creates the IBIS telegram as answer for the incoming telegram.
        /// </summary>
        /// <param name="telegram">The telegram including header, marker and checksum.</param>
        /// <param name="status">The status which is ignored.</param>
        /// <returns>This method always returns null.</returns>
        public override byte[] CreateAnswer(byte[] telegram, Status status)
        {
            // we have a special answer status here, not the system status (currently only "OK")
            return base.CreateAnswer(telegram, Status.Ok);
        }
    }
}