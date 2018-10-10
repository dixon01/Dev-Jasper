// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlPartViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts
{
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;

    /// <summary>
    /// The xml part view model.
    /// </summary>
    public class XmlPartViewModel : SingleEditorPartViewModelBase<XmlEditorViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlPartViewModel"/> class.
        /// </summary>
        /// <param name="editor">
        /// The editor.
        /// </param>
        public XmlPartViewModel(XmlEditorViewModel editor)
            : base(editor)
        {
        }
    }
}
