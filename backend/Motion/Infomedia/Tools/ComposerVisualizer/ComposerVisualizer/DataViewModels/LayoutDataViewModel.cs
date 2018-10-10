// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LayoutDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels
{
    using System.ComponentModel;

    /// <summary>
    /// The layout data view model.
    /// </summary>
    [DisplayName(@"Layout Properties")]
    public class LayoutDataViewModel : ConfigBaseDataViewModel
    {
        private bool nodeVisible;

        /// <summary>
        /// Gets or sets the name of the layout.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether layout is visible.
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
    }
}