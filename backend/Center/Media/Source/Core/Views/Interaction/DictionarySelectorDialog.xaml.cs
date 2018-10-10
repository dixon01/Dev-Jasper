// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DictionarySelectorDialog.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for DictionarySelectorDialog.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Interaction
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.Interaction;

    /// <summary>
    /// Interaction logic for DictionarySelectorDialog.xaml
    /// </summary>
    public partial class DictionarySelectorDialog
    {
        /// <summary>
        /// The is led canvas property.
        /// </summary>
        public static readonly DependencyProperty IsLedCanvasProperty = DependencyProperty.Register(
            "IsLedCanvas",
            typeof(bool),
            typeof(DictionarySelectorDialog),
            new PropertyMetadata(default(bool)));

        private const int MaxRecentDictionaryItems = 5;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionarySelectorDialog"/> class.
        /// </summary>
        public DictionarySelectorDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets a value indicating whether is led canvas.
        /// </summary>
        public bool IsLedCanvas
        {
            get
            {
                return (bool)this.GetValue(IsLedCanvasProperty);
            }

            set
            {
                this.SetValue(IsLedCanvasProperty, value);
            }
        }

        /// <summary>
        /// the handler for close
        /// </summary>
        /// <param name="e">the event arguments</param>
        protected override void OnClose(EventArgs e)
        {
            base.OnClose(e);

            if (!(this.DataContext is DictionarySelectorPrompt selectorPrompt))
            {
                return;
            }

            this.AddRecentDictionaryValue(selectorPrompt.SelectedDictionaryValue);
        }

        private void AddRecentDictionaryValue(DictionaryValueDataViewModel newElement)
        {
            if (!(this.DataContext is DictionarySelectorPrompt dictionarySelectorPrompt))
            {
                return;
            }

            if (newElement.Table == null)
            {
                return;
            }

            Func<DictionaryValueDataViewModel, bool> predicate =
                v =>
                v.Table.Name == newElement.Table.Name && v.Table.Index == newElement.Table.Index
                && v.Column.Name == newElement.Column.Name && v.Column.Index == newElement.Column.Index;

            if (dictionarySelectorPrompt.RecentDictionaryValues.Any(predicate))
            {
                dictionarySelectorPrompt.RecentDictionaryValues.Remove(
                    dictionarySelectorPrompt.RecentDictionaryValues.First(predicate));
            }

            dictionarySelectorPrompt.RecentDictionaryValues.Insert(0, newElement);
            for (var i = dictionarySelectorPrompt.RecentDictionaryValues.Count - 1; i >= MaxRecentDictionaryItems; i--)
            {
                dictionarySelectorPrompt.RecentDictionaryValues.RemoveAt(i);
            }
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            InteractionAction.SkipNextMouseUp = true;
            this.Close();
        }
    }
}
