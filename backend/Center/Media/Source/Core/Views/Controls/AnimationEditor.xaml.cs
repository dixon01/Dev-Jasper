// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnimationEditor.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The AnimationEditor.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Controls
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Configuration.Infomedia.Common;

    using NLog;

    /// <summary>
    /// Interaction logic for AnimationEditor.xaml
    /// </summary>
    public partial class AnimationEditor
    {
        /// <summary>
        /// the FormulaTypeListProperty
        /// </summary>
        public static readonly DependencyProperty AnimationTypeListProperty = DependencyProperty.Register(
            "AnimationTypeList",
            typeof(ExtendedObservableCollection<AnimationTypeDefinition>),
            typeof(AnimationEditor),
            new PropertyMetadata(default(ExtendedObservableCollection<AnimationTypeDefinition>)));

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationEditor"/> class.
        /// </summary>
        public AnimationEditor()
        {
            this.InitializeComponent();
            this.Loaded += this.OnLoaded;
            this.DataContextChanged += this.OnDataContextChanged;

            this.AnimationTypeList = new ExtendedObservableCollection<AnimationTypeDefinition>();
            var animationTypes = (PropertyChangeAnimationType[])Enum.GetValues(typeof(PropertyChangeAnimationType));
            foreach (var animationType in animationTypes.Where(at => at != PropertyChangeAnimationType.None))
            {
                this.AnimationTypeList.Add(new AnimationTypeDefinition(animationType));
            }

            this.AnimationTypeComboBox.SelectedItem = this.AnimationTypeList.First();
        }

        /// <summary>
        /// Gets the Formula type list
        /// </summary>
        public ExtendedObservableCollection<AnimationTypeDefinition> AnimationTypeList
        {
            get
            {
                return (ExtendedObservableCollection<AnimationTypeDefinition>)this.GetValue(AnimationTypeListProperty);
            }

            private set
            {
                this.SetValue(AnimationTypeListProperty, value);
            }
        }

        /// <summary>
        /// The refresh function
        /// </summary>
        public void Refresh()
        {
            var context = (AnimationEditorPrompt)DataContext;
            if (context != null)
            {
                var animationType = ((AnimationDataViewModel)context.DataValue.Animation).Type.Value;
                var definition = this.AnimationTypeList.FirstOrDefault(at => at.AnimationType == animationType);
                this.AnimationTypeComboBox.SelectedItem = definition;
            }
        }

        /// <summary>
        /// handles key up events
        /// </summary>
        /// <param name="e">the key event</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            // only close formular editor on escape
            if (e.Key != Key.Escape)
            {
                e.Handled = true;
            }
        }

        private void OnDataContextChanged(
            object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            this.Refresh();
        }

        /// <summary>
        /// sets the index on load time
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="routedEventArgs">the routed event args</param>
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            this.Refresh();
        }

        private void OnAnimationTypeSelected(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext != null)
            {
                var animationTypeDefinition = (AnimationTypeDefinition)AnimationTypeComboBox.SelectedItem;

                var context = (AnimationEditorPrompt)DataContext;
                ((AnimationDataViewModel)context.DataValue.Animation).Type.Value = animationTypeDefinition.AnimationType;
            }
        }
    }
}
