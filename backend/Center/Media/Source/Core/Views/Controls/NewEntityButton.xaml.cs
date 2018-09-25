// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewEntityButton.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The NewEntityButton.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Controls
{
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for NewEntityButton.xaml
    /// </summary>
    public partial class NewEntityButton
    {
        /// <summary>
        /// the create new entity property
        /// </summary>
        public static readonly DependencyProperty CreateNewEntityCommandProperty =
            DependencyProperty.Register(
                "CreateNewEntityCommand",
                typeof(ICommand),
                typeof(NewEntityButton),
                new PropertyMetadata(default(ICommand)));

        /// <summary>
        /// the new button text property
        /// </summary>
        public static readonly DependencyProperty NewButtonTextProperty = DependencyProperty.Register(
            "NewButtonText",
            typeof(string),
            typeof(NewEntityButton),
            new PropertyMetadata(default(string)));

        /// <summary>
        /// Initializes a new instance of the <see cref="NewEntityButton"/> class.
        /// </summary>
        public NewEntityButton()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the create new Entity Command
        /// </summary>
        public ICommand CreateNewEntityCommand
        {
            get
            {
                return (ICommand)this.GetValue(CreateNewEntityCommandProperty);
            }

            set
            {
                this.SetValue(CreateNewEntityCommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text to be displayed on the button
        /// </summary>
        public string NewButtonText
        {
            get
            {
                return (string)this.GetValue(NewButtonTextProperty);
            }

            set
            {
                this.SetValue(NewButtonTextProperty, value);
            }
        }
    }
}
