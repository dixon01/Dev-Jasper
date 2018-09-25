// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GuiFactory.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GuiFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Gui
{
    using Gorba.Motion.Obc.Terminal.Core;
    using Gorba.Motion.Obc.Terminal.Gui.Form;

    /// <summary>
    /// The graphical UI factory.
    /// </summary>
    public class GuiFactory : UiFactory
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
