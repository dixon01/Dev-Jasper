// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Action.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Action type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    /// <summary>
    /// Delegate for simple messages.
    /// This is only needed in CF &lt; 3.5 since Action without template parameters is only defined in CF 3.5.
    /// </summary>
    public delegate void Action();
}