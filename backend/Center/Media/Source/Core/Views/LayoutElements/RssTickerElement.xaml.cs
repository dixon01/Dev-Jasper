// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RssTickerElement.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.LayoutElements
{
    using System.Collections.Generic;
    using System.Windows;

    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Interaction logic for RSSTickerElement.xaml
    /// </summary>
    public partial class RssTickerElement
    {
        /// <summary>
        /// The horizontal text alignment
        /// </summary>
        public static readonly DependencyProperty HorizontalTextAlignmentProperty =
            DependencyProperty.Register(
                "HorizontalTextAlignment",
                typeof(HorizontalAlignment),
                typeof(RssTickerElement),
                new PropertyMetadata(default(HorizontalAlignment)));

        /// <summary>
        /// The vertical text alignment
        /// </summary>
        public static readonly DependencyProperty VerticalTextAlignmentProperty =
            DependencyProperty.Register(
                "VerticalTextAlignment",
                typeof(VerticalAlignment),
                typeof(RssTickerElement),
                new PropertyMetadata(default(VerticalAlignment)));

        /// <summary>
        /// The is in edit mode property.
        /// </summary>
        public static readonly DependencyProperty IsInEditModeProperty = DependencyProperty.Register(
            "IsInEditMode",
            typeof(bool),
            typeof(RssTickerElement),
            new PropertyMetadata(default(bool)));

        /// <summary>
        /// Initializes a new instance of the <see cref="RssTickerElement"/> class.
        /// </summary>
        public RssTickerElement()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the horizontal text alignment
        /// </summary>
        public HorizontalAlignment HorizontalTextAlignment
        {
            get
            {
                return (HorizontalAlignment)this.GetValue(HorizontalTextAlignmentProperty);
            }

            set
            {
                this.SetValue(HorizontalTextAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the vertical text alignment
        /// </summary>
        public VerticalAlignment VerticalTextAlignment
        {
            get
            {
                return (VerticalAlignment)this.GetValue(VerticalTextAlignmentProperty);
            }

            set
            {
                this.SetValue(VerticalTextAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is in edit mode.
        /// </summary>
        public bool IsInEditMode
        {
            get
            {
                return (bool)this.GetValue(IsInEditModeProperty);
            }

            set
            {
                this.SetValue(IsInEditModeProperty, value);
            }
        }

        /// <summary>
        /// the method to enter the edit mode
        /// </summary>
        protected override void EnterEditMode()
        {
            base.EnterEditMode();
            this.IsInEditMode = true;
        }

        /// <summary>
        /// The exit edit mode.
        /// </summary>
        protected override void ExitEditMode()
        {
            base.ExitEditMode();
            this.IsInEditMode = false;
        }

        private void OnTextChanged(string oldName, string newName, object sourceObject)
        {
            if (oldName == newName)
            {
                return;
            }

            var rssTicker = (RssTickerElementDataViewModel)sourceObject;
            var oldOne = (RssTickerElementDataViewModel)rssTicker.Clone();
            oldOne.TestData.Value = oldName;
            rssTicker.TestData.Value = newName;
            var mediaShell = (MediaShell)ServiceLocator.Current.GetInstance(typeof(IMediaShell));
            var parameters = new UpdateEntityParameters(
                new List<DataViewModelBase> { oldOne },
                new List<DataViewModelBase> { (RssTickerElementDataViewModel)rssTicker.Clone() },
                mediaShell.Editor.Elements);
            ((EditorViewModelBase)mediaShell.Editor).UpdateElementCommand.Execute(parameters);
        }
    }
}
