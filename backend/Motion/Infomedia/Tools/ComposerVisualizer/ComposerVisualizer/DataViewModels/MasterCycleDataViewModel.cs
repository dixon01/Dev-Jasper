// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MasterCycleDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MasterCycleDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// The master cycle data view model.
    /// </summary>
    [DisplayName(@"Master Cycle Properties")]
    public class MasterCycleDataViewModel : ConfigBaseDataViewModel
    {
        private EvaluatorBaseDataViewModel enabled;

        private bool nodeVisible;

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterCycleDataViewModel"/> class.
        /// </summary>
        public MasterCycleDataViewModel()
        {
            this.MasterSections = new ObservableCollection<MasterSectionDataViewModel>();
        }

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
        /// Gets or sets the name of the master cycle.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether master cycle is visible.
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
        /// Gets or sets the master sections.
        /// </summary>
        [Browsable(false)]
        public ObservableCollection<MasterSectionDataViewModel> MasterSections { get; set; }
    }
}