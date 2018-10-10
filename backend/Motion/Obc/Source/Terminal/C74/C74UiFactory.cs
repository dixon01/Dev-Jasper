// --------------------------------------------------------------------------------------------------------------------
// <copyright file="C74UiFactory.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the C74UiFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.C74
{
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The UI factory for the Thoreb C74 OBU.
    /// </summary>
    public class C74UiFactory : UiFactory
    {
        /// <summary>
        /// Creates the root.
        /// </summary>
        /// <returns>
        /// The <see cref="IUiRoot"/> implementation.
        /// </returns>
        public override IUiRoot CreateRoot()
        {
            return new RootForm();
        }
    }
}
