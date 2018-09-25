// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VirtualDisplayDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    /// <summary>
    /// Virtual display properties data view model
    /// </summary>
    [DisplayName("Virtual Display Properties")]
    public class VirtualDisplayDataViewModel : ConfigBaseDataViewModel
    {
        private bool nodeVisible;

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualDisplayDataViewModel"/> class.
        /// </summary>
        public VirtualDisplayDataViewModel()
        {
            this.StandardCycles = new ObservableCollection<CycleDataViewModel>();
            this.EventCycles = new ObservableCollection<EventCycleDataViewModel>();
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Category("Configuration")]
        [ReadOnly(true)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the cycle package.
        /// </summary>
        [Category("Configuration")]
        [ReadOnly(true)]
        public string CyclePackage { get; set; }

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
        /// Gets or sets a value indicating whether virtual display node is visible.
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
        /// Gets or sets the standard cycles list.
        /// </summary>
        [Browsable(false)]
        public ObservableCollection<CycleDataViewModel> StandardCycles { get; set; }

        /// <summary>
        /// Gets or sets the event cycles list.
        /// </summary>
        [Browsable(false)]
        public ObservableCollection<EventCycleDataViewModel> EventCycles { get; set; }
    }
}
