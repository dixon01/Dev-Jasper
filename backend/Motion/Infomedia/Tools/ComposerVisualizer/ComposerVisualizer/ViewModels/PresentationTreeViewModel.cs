// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PresentationTreeViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.ViewModels
{
    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels;

    using ViewModelBase = Gorba.Motion.Infomedia.Tools.ComposerVisualizer.WpfCore.ViewModelBase;

    /// <summary>
    /// View model for the tree view
    /// </summary>
    public class PresentationTreeViewModel : ViewModelBase
    {
        private PresentationsDataViewModel treeViewRoot;

        /// <summary>
        /// Gets or sets the tree view root.
        /// </summary>
        public PresentationsDataViewModel TreeViewRoot
        {
            get
            {
                return this.treeViewRoot;
            }

            set
            {
                if (this.SetProperty(ref this.treeViewRoot, value, () => this.TreeViewRoot))
                {
                    this.RaisePropertyChanged(() => this.TreeViewRoot);
                }
            }
        }
    }
}
