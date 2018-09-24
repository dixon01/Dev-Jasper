// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.ViewModels
{
    using System.Collections.ObjectModel;

    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Items;

    /// <summary>
    /// View model for the layout view
    /// </summary>
    public class LayoutViewModel : WpfCore.ViewModelBase
    {
        private string tabName;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutViewModel"/> class.
        /// </summary>
        /// <param name="tabName">
        /// The tab Name.
        /// </param>
        public LayoutViewModel(string tabName)
        {
            this.TabName = tabName;
            this.LayoutItems = new ObservableCollection<ItemBaseDataViewModel>();
        }

        /// <summary>
        /// Gets or sets the layout view root.
        /// </summary>
        public ObservableCollection<ItemBaseDataViewModel> LayoutItems { get; set; }

        /// <summary>
        /// Gets or sets the tab name of the layout view.
        /// </summary>
        public string TabName
        {
            get
            {
                return this.tabName;
            }

            set
            {
                if (this.SetProperty(ref this.tabName, value, () => this.TabName))
                {
                    this.RaisePropertyChanged(() => this.TabName);
                }
            }
        }
    }
}
