// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUpdateable.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IUpdateable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control
{
    /// <summary>
    ///   Implement this interface if your object should be updated automatically. You can register it on
    ///   Updater.AddUpdateable(IUpdateable)
    /// </summary>
    public interface IUpdateable
    {
        /// <summary>
        ///   Gets the interval time in seconds to be called.
        ///   If return value is 0 or negative, no Update() will be called
        /// </summary>
        /// <value>Update interval in seconds</value>
        int IntervalTime { get; }

        /// <summary>
        ///   Will be called from the Updater when the interval time is expired
        ///   The updater will run ever in the same thread.
        ///   But if the Update method can be called from an other part you may think about
        ///   locking your implementation.
        ///   Make sure that this object will handle fast the Update() method. Otherwise it may block the hole system!
        /// </summary>
        void Update();
    }
}