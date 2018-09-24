// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormulaNavigationEditorPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The formula navigation editor prompt.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Interaction
{
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// The formula navigation editor prompt.
    /// </summary>
    public class FormulaNavigationEditorPrompt : FormulaEditorPrompt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormulaNavigationEditorPrompt"/> class.
        /// </summary>
        /// <param name="shell">
        /// The shell.
        /// </param>
        /// <param name="dataValue">
        /// The data value.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public FormulaNavigationEditorPrompt(
            IMediaShell shell,
            IDynamicDataValue dataValue,
            ICommandRegistry commandRegistry)
            : base(shell, dataValue, commandRegistry)
        {
        }
    }
}