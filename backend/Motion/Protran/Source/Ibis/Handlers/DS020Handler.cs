// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS020Handler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS020Handler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Handlers
{
    using Gorba.Common.Protocols.Vdv300.Telegrams;

    /// <summary>
    /// Dummy handler for DS020.
    /// This class doesn't actually do anything, but is just here so
    /// every telegram has its handler.
    /// </summary>
    public class DS020Handler : TelegramHandler<DS020>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS020Handler"/> class.
        /// </summary>
        public DS020Handler()
            : base(10)
        {
        }

        /// <summary>
        /// Handles the telegram and generates Ximple if needed.
        /// This method needs to be implemented by subclasses to
        /// create the Ximple object for the given telegram.
        /// </summary>
        /// <param name="telegram">
        /// The telegram.
        /// </param>
        protected override void HandleInput(DS020 telegram)
        {
        }
    }
}
