// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="XimpleTableActionContext.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.Motion.Protran.XimpleProtocol
{
    using System.Collections.Generic;

    using Gorba.Common.Protocols.Ximple;

    /// <summary>The ximple table action context.</summary>
    internal class XimpleTableActionContext
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="XimpleTableActionContext"/> class.</summary>
        /// <param name="socketState">The socket state.</param>
        /// <param name="ximpleCells">The ximple cells.</param>
        public XimpleTableActionContext(SocketState socketState, IEnumerable<XimpleCell> ximpleCells)
        {
            this.SocketState = socketState;
            this.XimpleCells = ximpleCells;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the socket state.</summary>
        public SocketState SocketState { get; set; }

        /// <summary>Gets or sets the ximple cells.</summary>
        public IEnumerable<XimpleCell> XimpleCells { get; set; }

        #endregion
    }
}