// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericUsageDS021BaseEditorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GenericUsageDS021BaseEditorViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors
{
    using System;
    using System.ComponentModel;

    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;

    /// <summary>
    /// View model for the editor of the generic coordinates
    /// with an additional block selection for DS021 based telegrams.
    /// </summary>
    public class GenericUsageDS021BaseEditorViewModel : GenericUsageEditorViewModel
    {
        private const int DefaultBlock = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericUsageDS021BaseEditorViewModel"/> class.
        /// </summary>
        public GenericUsageDS021BaseEditorViewModel()
        {
            this.BlockSelection = new SelectionEditorViewModel
                {
                    Options =
                        {
                            new SelectionOptionViewModel(
                                AdminStrings.UnitConfig_Ibis_DS021Base_DefaultBlock, DefaultBlock),
                            new SelectionOptionViewModel("1", 1),
                            new SelectionOptionViewModel("2", 2),
                            new SelectionOptionViewModel("3", 3),
                            new SelectionOptionViewModel("4", 4),
                        }
                };

            this.BlockSelection.SelectValue(DefaultBlock);

            this.BlockSelection.PropertyChanged += this.BlockSelectionOnPropertyChanged;
            this.PropertyChanged += this.OnPropertyChanged;

            this.BlockSelection.IsEnabled = this.IsEditorEnabled;
        }

        /// <summary>
        /// Gets or sets the <see cref="GenericUsageDS021Base"/>.
        /// </summary>
        public GenericUsageDS021Base GenericUsageDS021Base
        {
            get
            {
                var usage = this.GenericUsage;
                if (usage == null)
                {
                    return null;
                }

                var extended = new GenericUsageDS021Base
                                   {
                                       Language = usage.Language,
                                       Table = usage.Table,
                                       Column = usage.Column,
                                       Row = usage.Row,
                                       RowOffset = usage.RowOffset,
                                       FromBlock = (int)this.BlockSelection.SelectedValue
                                   };
                return extended;
            }

            set
            {
                this.GenericUsage = value;

                this.BlockSelection.SelectValue(value == null ? DefaultBlock : value.FromBlock);
            }
        }

        /// <summary>
        /// Gets the block selection editor.
        /// </summary>
        public SelectionEditorViewModel BlockSelection { get; private set; }

        /// <summary>
        /// Clears the <see cref="DirtyViewModelBase.IsDirty"/> flag.
        /// </summary>
        public override void ClearDirty()
        {
            base.ClearDirty();
            this.BlockSelection.ClearDirty();
        }

        private void BlockSelectionOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty" && this.BlockSelection.IsDirty)
            {
                this.MakeDirty();
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsEditorEnabled")
            {
                this.BlockSelection.IsEnabled = this.IsEditorEnabled;
            }
        }
    }
}
