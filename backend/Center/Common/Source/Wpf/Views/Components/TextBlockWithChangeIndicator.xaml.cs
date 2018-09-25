// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextBlockWithChangeIndicator.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextBlockWithChangeIndicator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Views.Components
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for TextBoxWithChangeIndicator.xaml
    /// </summary>
    public partial class TextBlockWithChangeIndicator
    {
        /// <summary>
        /// the dependency Text property
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(TextBlockWithChangeIndicator),
            new PropertyMetadata(default(string)));

        /// <summary>
        /// the dependency "is dirty" Property indicating that the current Object has changes
        /// </summary>
        public static readonly DependencyProperty IsDirtyProperty = DependencyProperty.Register(
            "IsDirty",
            typeof(bool),
            typeof(TextBlockWithChangeIndicator),
            new PropertyMetadata(default(bool)));

        /// <summary>
        /// the dependency property for text trimming
        /// </summary>
        public static readonly DependencyProperty TextTrimmingProperty = DependencyProperty.Register(
            "TextTrimming",
            typeof(TextTrimming),
            typeof(TextBlockWithChangeIndicator),
            new PropertyMetadata(default(TextTrimming)));

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBlockWithChangeIndicator" /> class.
        /// </summary>
        public TextBlockWithChangeIndicator()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the Text
        /// </summary>
        public string Text
        {
            get
            {
                return (string)this.GetValue(TextProperty);
            }

            set
            {
                this.SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the TextBlock is dirty
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return (bool)this.GetValue(IsDirtyProperty);
            }

            set
            {
                this.SetValue(IsDirtyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text trimming
        /// </summary>
        public TextTrimming TextTrimming
        {
            get
            {
                return (TextTrimming)this.GetValue(TextTrimmingProperty);
            }

            set
            {
                this.SetValue(TextTrimmingProperty, value);
            }
        }
    }
}
