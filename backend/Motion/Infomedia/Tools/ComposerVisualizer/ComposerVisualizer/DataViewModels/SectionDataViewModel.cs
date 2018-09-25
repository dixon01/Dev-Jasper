// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectionDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// Section properties data view model
    /// </summary>
    [DisplayName(@"Section Properties")]
    public class SectionDataViewModel : ConfigBaseDataViewModel
    {
        private EvaluatorBaseDataViewModel enabled;

        private bool nodeVisible;

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionDataViewModel"/> class.
        /// </summary>
        public SectionDataViewModel()
        {
            this.LayoutNode = new ObservableCollection<LayoutDataViewModel>();
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
        /// Gets or sets the duration.
        /// </summary>
        [Category("Configuration")]
        [ReadOnly(true)]
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets or sets the layout.
        /// </summary>
        [Category("Configuration")]
        [ReadOnly(true)]
        public string Layout { get; set; }

        /// <summary>
        /// Gets or sets the name of the section (section type).
        /// </summary>
        [Browsable(false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether section node is visible.
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
        /// Gets or sets the layout for the section.
        /// </summary>
        [Browsable(false)]
        public ObservableCollection<LayoutDataViewModel> LayoutNode { get; set; }
    }
}
