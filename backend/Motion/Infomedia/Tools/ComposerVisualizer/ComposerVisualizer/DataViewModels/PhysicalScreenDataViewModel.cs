// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhysicalScreenDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// Physical screen properties data view model
    /// </summary>
    [DisplayName(@"Physical Screen Properties")]
    public class PhysicalScreenDataViewModel : ConfigBaseDataViewModel
    {
        private EvaluatorBaseDataViewModel visible;

        private bool nodeVisible;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhysicalScreenDataViewModel"/> class.
        /// </summary>
        public PhysicalScreenDataViewModel()
        {
            this.MasterCycles = new ObservableCollection<MasterCycleDataViewModel>();
            this.MasterEventCycles = new ObservableCollection<MasterEventCycleDataViewModel>();
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Category("Configuration")]
        [ReadOnly(true)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [Category("Configuration")]
        [ReadOnly(true)]
        public PhysicalScreenType Type { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [Category("Configuration")]
        [ReadOnly(true)]
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        [Category("Configuration")]
        [ReadOnly(true)]
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        [Category("Configuration")]
        [ReadOnly(true)]
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether visible.
        /// </summary>
        [Category("Configuration")]
        [ReadOnly(true)]
        [ExpandableObject]
        [DisplayName(@"Visible*")]
        public EvaluatorBaseDataViewModel Visible
        {
            get
            {
                return this.visible;
            }

            set
            {
                if (this.SetProperty(ref this.visible, value, () => this.Visible))
                {
                    this.RaisePropertyChanged(() => this.Visible);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether node visible.
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
        /// Gets or sets the master cycle.
        /// </summary>
        [Browsable(false)]
        public ObservableCollection<MasterCycleDataViewModel> MasterCycles { get; set; }

        /// <summary>
        /// Gets or sets the master event cycle.
        /// </summary>
        [Browsable(false)]
        public ObservableCollection<MasterEventCycleDataViewModel> MasterEventCycles { get; set; }
    }
}
