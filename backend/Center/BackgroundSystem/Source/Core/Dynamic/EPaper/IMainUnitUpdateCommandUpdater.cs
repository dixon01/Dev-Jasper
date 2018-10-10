// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMainUnitUpdateCommandUpdater.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IMainUnitUpdateCommandUpdater type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Dynamic.EPaper
{
    /// <summary>
    /// Defines the component that updates the main unit configuration according to enqueued changes for display
    /// contents.
    /// </summary>
    public interface IMainUnitUpdateCommandUpdater
    {
        /// <summary>
        /// Enqueues a content change for a display unit.
        /// </summary>
        /// <param name="mainUnitId">
        /// The main unit id.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="hash">
        /// The hash of the content.
        /// </param>
        void EnqueueDisplayContent(int mainUnitId, int index, string hash);
    }
}