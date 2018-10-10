// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WizardWindow.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WizardWindow type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Wizard
{
    using System.ComponentModel.Composition;

    using Gorba.Center.Common.Wpf.Framework.Views;

    /// <summary>
    /// Interaction logic for WizardWindow.xaml
    /// </summary>
    [Export]
    public partial class WizardWindow : IDialogView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WizardWindow"/> class.
        /// </summary>
        public WizardWindow()
        {
            this.InitializeComponent();
        }
    }
}
