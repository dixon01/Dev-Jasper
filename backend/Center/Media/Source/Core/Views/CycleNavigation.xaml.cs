// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CycleNavigation.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for CycleNavigation.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;

    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;

    /// <summary>
    /// Interaction logic for CycleNavigation.xaml
    /// </summary>
    public partial class CycleNavigation
    {
        /// <summary>
        /// the cycle package property
        /// </summary>
        public static readonly DependencyProperty CyclePackageProperty = DependencyProperty.Register(
            "CyclePackage",
            typeof(CyclePackageConfigDataViewModel),
            typeof(CycleNavigation),
            new PropertyMetadata(default(CyclePackageConfigDataViewModel)));

        /// <summary>
        /// The cycle property
        /// </summary>
        public static readonly DependencyProperty CycleProperty = DependencyProperty.Register(
            "Cycle",
            typeof(CycleConfigDataViewModelBase),
            typeof(CycleNavigation),
            new PropertyMetadata(default(CycleConfigDataViewModelBase)));

        /// <summary>
        /// the cycle item property
        /// </summary>
        public static readonly DependencyProperty SectionProperty = DependencyProperty.Register(
            "Section",
            typeof(SectionConfigDataViewModelBase),
            typeof(CycleNavigation),
            new PropertyMetadata(default(SectionConfigDataViewModelBase)));

        private readonly DispatcherTimer popupTimer;

        private object currentPopupContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="CycleNavigation"/> class.
        /// </summary>
        public CycleNavigation()
        {
            this.InitializeComponent();

            this.popupTimer = new DispatcherTimer();
            this.popupTimer.Tick += this.PopupTimerOnTick;
            this.popupTimer.Interval = TimeSpan.FromMilliseconds(400);
        }

        /// <summary>
        /// the open cycle package navigation event
        /// </summary>
        public event Action<CycleNavigation> OpenCyclePackageNavigation;

        /// <summary>
        /// the open cycle navigation event
        /// </summary>
        public event Action<CycleNavigation> OpenCycleNavigation;

        /// <summary>
        /// the open cycle item navigation event
        /// </summary>
        public event Action<CycleNavigation> OpenSectionNavigation;

        /// <summary>
        /// Gets or sets the cycle package
        /// </summary>
        public CyclePackageConfigDataViewModel CyclePackage
        {
            get
            {
                return (CyclePackageConfigDataViewModel)this.GetValue(CyclePackageProperty);
            }

            set
            {
                this.SetValue(CyclePackageProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the cycle
        /// </summary>
        public CycleConfigDataViewModelBase Cycle
        {
            get
            {
                return (CycleConfigDataViewModelBase)this.GetValue(CycleProperty);
            }

            set
            {
                this.SetValue(CycleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the section
        /// </summary>
        public SectionConfigDataViewModelBase Section
        {
            get
            {
                return (SectionConfigDataViewModelBase)this.GetValue(SectionProperty);
            }

            set
            {
                this.SetValue(SectionProperty, value);
            }
        }

        /// <summary>
        /// selects the cycle navigation
        /// </summary>
        public void SelectCycleNavigationItem()
        {
            CycleToggleButton.IsChecked = true;
        }

        /// <summary>
        /// selects the section navigation
        /// </summary>
        public void SelectSectionNavigationItem()
        {
            SectionToggleButton.IsChecked = true;
        }

        private void PopupTimerOnTick(object sender, EventArgs eventArgs)
        {
            this.OpenPopup();
        }

        private void CyclePackageMouseEnter(object sender, MouseEventArgs e)
        {
            this.currentPopupContent = this.CyclePackage;
            this.popupTimer.Start();
        }

        private void CyclePackageMouseLeave(object sender, MouseEventArgs e)
        {
            this.popupTimer.Stop();
            this.ClosePopup();
        }

        private void CycleMouseEnter(object sender, MouseEventArgs e)
        {
            this.currentPopupContent = this.Cycle;
            this.popupTimer.Start();
        }

        private void CycleMouseLeave(object sender, MouseEventArgs e)
        {
            this.popupTimer.Stop();
            this.ClosePopup();
        }

        private void CycleItemMouseEnter(object sender, MouseEventArgs e)
        {
            this.currentPopupContent = this.Section;
            this.popupTimer.Start();
        }

        private void CycleItemMouseLeave(object sender, MouseEventArgs e)
        {
            this.popupTimer.Stop();
            this.ClosePopup();
        }

        private void OpenPopup()
        {
            // TODO
        }

        private void ClosePopup()
        {
            // TODO
        }

        private void OnShowCyclePackageNavigationClick(object sender, RoutedEventArgs e)
        {
            if (this.OpenCyclePackageNavigation != null)
            {
                this.OpenCyclePackageNavigation(this);
            }
        }

        private void OnShowCycleNavigationClick(object sender, RoutedEventArgs e)
        {
            if (this.OpenCycleNavigation != null)
            {
                this.OpenCycleNavigation(this);
            }
        }

        private void OnShowCycleItemNavigationClick(object sender, RoutedEventArgs e)
        {
            if (this.OpenSectionNavigation != null)
            {
                this.OpenSectionNavigation(this);
            }
        }
    }
}
