// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CycleDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// Cycle properties data view model
    /// </summary>
    [DisplayName(@"Cycle Properties")]
    public class CycleDataViewModel : ConfigBaseDataViewModel
    {
        private EvaluatorBaseDataViewModel enabled;

        private bool nodeVisible;

        /// <summary>
        /// Initializes a new instance of the <see cref="CycleDataViewModel"/> class.
        /// </summary>
        public CycleDataViewModel()
        {
            this.Sections = new ObservableCollection<SectionDataViewModel>();
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Category("Configuration")]
        [ReadOnly(true)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enabled.
        /// </summary>
        [Category("Configuration")]
        [ReadOnly(true)]
        [ExpandableObject]
        [DisplayName(@"Enabled*")]
        public EvaluatorBaseDataViewModel Enabled
        {
            get
            {
                return this.enabled;
            }

            set
            {
                if (this.SetProperty(ref this.enabled, value, () => this.Enabled))
                {
                    this.RaisePropertyChanged(() => this.Enabled);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether cycle is visible.
        /// </summary>
        [Browsable(false)]
        public bool NodeVisible
        {
            get
            {
                return this.nodeVisible;
            }

            set
            {
                if (this.SetProperty(ref this.nodeVisible, value, () => this.NodeVisible))
                {
                    this.RaisePropertyChanged(() => this.NodeVisible);
                }
            }
        }

        /// <summary>
        /// Gets or sets the sections.
        /// </summary>
        [Browsable(false)]
        public ObservableCollection<SectionDataViewModel> Sections { get; set; }
    }
}
