// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILayerEditorController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ILayerEditorController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    /// <summary>
    /// The LayerEditorController interface.
    /// </summary>
    public interface ILayerEditorController
    {
        /// <summary>
        /// The rename layout element.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        void RenameLayoutElement(UpdateEntityParameters parameter);
    }
}