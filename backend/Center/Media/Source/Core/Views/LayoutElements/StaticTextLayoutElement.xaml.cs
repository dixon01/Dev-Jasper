// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StaticTextLayoutElement.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for StaticTextLayoutElement.xaml
// </summary>
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
    /// Interaction logic for StaticTextLayoutElement.xaml
    /// </summary>
    public partial class StaticTextLayoutElement
    {
        /// <summary>
        /// The static text element property.
        /// </summary>
        public static readonly DependencyProperty ElementProperty = DependencyProperty.Register(
            "Element",
            typeof(StaticTextElementDataViewModel),
            typeof(StaticTextLayoutElement),
            new PropertyMetadata(default(StaticTextElementDataViewModel)));

        /// <summary>
        /// The horizontal text alignment
        /// </summary>
        public static readonly DependencyProperty HorizontalTextAlignmentProperty =
            DependencyProperty.Register(
                "HorizontalTextAlignment",
                typeof(HorizontalAlignment),
                typeof(StaticTextLayoutElement),
                new PropertyMetadata(default(HorizontalAlignment)));

        /// <summary>
        /// The vertical text alignment
        /// </summary>
        public static readonly DependencyProperty VerticalTextAlignmentProperty =
            DependencyProperty.Register(
                "VerticalTextAlignment",
                typeof(VerticalAlignment),
                typeof(StaticTextLayoutElement),
                new PropertyMetadata(default(VerticalAlignment)));

        /// <summary>
        /// The is in edit mode property.
        /// </summary>
        public static readonly DependencyProperty IsInEditModeProperty = DependencyProperty.Register(
            "IsInEditMode",
            typeof(bool),
            typeof(StaticTextLayoutElement),
            new PropertyMetadata(default(bool)));

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticTextLayoutElement"/> class.
        /// </summary>
        public StaticTextLayoutElement()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the element.
        /// </summary>
        public StaticTextElementDataViewModel Element
        {
            get
            {
                return (StaticTextElementDataViewModel)this.GetValue(ElementProperty);
            }

            set
            {
                this.SetValue(ElementProperty, value);
            }
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

            var staticText = (StaticTextElementDataViewModel)sourceObject;
            var oldOne = (StaticTextElementDataViewModel)staticText.Clone();
            oldOne.Value.Value = oldName;
            staticText.Value.Value = newName;
            var mediaShell = (MediaShell)ServiceLocator.Current.GetInstance(typeof(IMediaShell));
            var parameters = new UpdateEntityParameters(
                new List<DataViewModelBase> { oldOne },
                new List<DataViewModelBase> { (StaticTextElementDataViewModel)staticText.Clone() },
                mediaShell.Editor.Elements);
            ((EditorViewModelBase)mediaShell.Editor).UpdateElementCommand.Execute(parameters);
        }
    }
}
