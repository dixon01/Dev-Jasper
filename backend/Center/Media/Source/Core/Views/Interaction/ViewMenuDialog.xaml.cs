// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewMenuDialog.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ViewMenuDialog type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Interaction
{
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Media.Core.Interaction;

    /// <summary>
    /// Interaction logic for ShowMenuDialog.xaml
    /// </summary>
    public partial class ViewMenuDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewMenuDialog"/> class.
        /// </summary>
        public ViewMenuDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the show simulation toggle command wrapper.
        /// </summary>
        public ICommand ShowSimulationToggleCommandWrapper
        {
            get
            {
                return new RelayCommand(this.ShowSimulation);
            }
        }

        /// <summary>
        /// Gets the use edge snap toggle command wrapper.
        /// </summary>
        public ICommand UseEdgeSnapToggleCommandWrapper
        {
            get
            {
                return new RelayCommand(this.UseEdgeSnapToggle);
            }
        }

        private void ShowSimulation()
        {
            if (!(this.DataContext is ViewMenuPrompt context))
            {
                return;
            }

            context.ShowSimulationToggleCommand.Execute(null);
            this.Close();
        }

        private void UseEdgeSnapToggle()
        {
            if (!(this.DataContext is ViewMenuPrompt context))
            {
                return;
            }

            context.UseEdgeSnapToggleCommand.Execute(null);
            this.Close();
        }
    }
}
