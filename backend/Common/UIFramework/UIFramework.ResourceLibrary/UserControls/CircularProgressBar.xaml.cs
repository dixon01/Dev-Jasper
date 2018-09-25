// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CircularProgressBar.xaml.cs" company="Luminator USA">
//   Copyright (c) 2013
//   All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.UIFramework.ResourceLibrary.UserControls
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Animation;

    /// <summary>
    ///     Provides a circular progress bar
    /// </summary>
    public partial class CircularProgressBar : UserControl
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="CircularProgressBar" /> class.
        /// </summary>
        static CircularProgressBar()
        {
            // Use a default Animation Frame rate of 30, which uses less CPU time
            // than the standard 50 which you get out of the box
            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 30 });
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CircularProgressBar" /> class.
        /// </summary>
        public CircularProgressBar()
        {
            this.InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
            }
        }

        #endregion
    }
}