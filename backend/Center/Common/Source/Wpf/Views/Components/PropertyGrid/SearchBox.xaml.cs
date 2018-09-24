// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchBox.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The SearchBox.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Views.Components.PropertyGrid
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for SearchBox
    /// </summary>
    public partial class SearchBox
    {
        /// <summary>
        /// The text property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(SearchBox), new PropertyMetadata(default(string)));

        /// <summary>
        /// The help text property.
        /// </summary>
        public static readonly DependencyProperty HelpTextProperty = DependencyProperty.Register(
            "HelpText", typeof(string), typeof(SearchBox), new PropertyMetadata(default(string)));

        /// <summary>
        /// The has searched property.
        /// </summary>
        public static readonly DependencyProperty HasSearchedProperty = DependencyProperty.Register(
            "HasSearched", typeof(bool), typeof(SearchBox), new PropertyMetadata(default(bool)));

        /// <summary>
        /// The m_ search box text changed timer.
        /// </summary>
        private readonly DispatcherTimer searchBoxTextChangedTimer;

        /// <summary>
        /// The m_ last search term.
        /// </summary>
        private string lastSearchTerm;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchBox"/> class.
        /// </summary>
        public SearchBox()
        {
            this.InitializeComponent();

            this.searchBoxTextChangedTimer = new DispatcherTimer(DispatcherPriority.Send);
            this.searchBoxTextChangedTimer.Tick += (s, e) =>
            {
                this.searchBoxTextChangedTimer.Stop();

                this.OnSearched();
            };
            this.searchBoxTextChangedTimer.Interval = TimeSpan.FromMilliseconds(500);
            this.searchBoxTextChangedTimer.Start();

            this.GotFocus += this.OnGotFocus;
        }

        /// <summary>
        /// The searched.
        /// </summary>
        public event EventHandler<PropertyGridSearchEventArgs> Searched;

        /// <summary>
        /// The clear search event.
        /// </summary>
        public event EventHandler ClearSearch;

        /// <summary>
        /// Gets or sets the text.
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
        /// Gets or sets the help text.
        /// </summary>
        public string HelpText
        {
            get
            {
                return (string)this.GetValue(HelpTextProperty);
            }

            set
            {
                this.SetValue(HelpTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether has searched.
        /// </summary>
        public bool HasSearched
        {
            get
            {
                return (bool)this.GetValue(HasSearchedProperty);
            }

            set
            {
                this.SetValue(HasSearchedProperty, value);
            }
        }

        /// <summary>
        /// The on searched.
        /// </summary>
        public void OnSearched()
        {
            var searchtext = this.ItemSearchBox.Text;
            if (!string.IsNullOrEmpty(searchtext))
            {
                this.HasSearched = true;
                this.lastSearchTerm = searchtext;
                var handler = this.Searched;
                if (handler != null)
                {
                    var e = new PropertyGridSearchEventArgs { Text = searchtext };
                    handler(this, e);
                }
            }
            else
            {
                this.OnClearSearch();
            }
        }

        /// <summary>
        /// The on clear search.
        /// </summary>
        public void OnClearSearch()
        {
            this.HasSearched = false;
            this.lastSearchTerm = null;

            this.ItemSearchBox.Text = string.Empty;
            var handler = this.ClearSearch;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        /// <summary>
        /// The on search box key up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnSearchBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.OnSearched();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                this.OnClearSearch();
                e.Handled = true;
            }
        }

        /// <summary>
        /// The on search box key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnSearchBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
            }
            else if (e.Key == Key.Return)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// The on search box text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnSearchBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            this.HasSearched = this.ItemSearchBox.Text == this.lastSearchTerm;

            this.searchBoxTextChangedTimer.Stop();
            this.searchBoxTextChangedTimer.Start();
        }

        /// <summary>
        /// The on search button click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnSearchButtonClick(object sender, RoutedEventArgs e)
        {
            this.OnSearched();
        }

        /// <summary>
        /// The on clear search button click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnClearSearchButtonClick(object sender, RoutedEventArgs e)
        {
            this.OnClearSearch();
        }

        /// <summary>
        /// The on got focus.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="routedEventArgs">
        /// The routed event args.
        /// </param>
        private void OnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            this.ItemSearchBox.Focus();
        }

        /// <summary>
        /// The property grid search event args.
        /// </summary>
        public class PropertyGridSearchEventArgs : EventArgs
        {
            /// <summary>
            /// Gets or sets the text.
            /// </summary>
            public string Text { get; set; }
        }
    }
}
