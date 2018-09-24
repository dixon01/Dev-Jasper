// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatePickerEx.cs" company="">
//   Copyright (c) 2013
//   Luminator Technology Group
//   All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.UIFramework.ResourceLibrary.DatePicker
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    using Luminator.UIFramework.Common.MVVM.ViewModelHelpers;

    using Calendar = System.Windows.Controls.Calendar;

    /// <summary>
    ///     A specialized DatePicker that allows for Up key/Down key date changes
    ///     and also supports "Go To Today" and tooltips for BlackOut dates
    /// </summary>
    public class DatePickerEx : DatePicker
    {
        #region Static Fields

        /// <summary>
        ///     AlternativeCalendarStyle Dependency Property
        /// </summary>
        public static readonly DependencyProperty AlternativeCalendarStyleProperty = DependencyProperty.Register(
            "AlternativeCalendarStyle", typeof(Style), typeof(DatePickerEx), new FrameworkPropertyMetadata(null, OnAlternativeCalendarStyleChanged));

        #endregion

        #region Fields

        /// <summary>
        ///     The go to today button.
        /// </summary>
        private Button gotoTodayButton;

        /// <summary>
        ///     The go to today command.
        /// </summary>
        private ICommand gotoTodayCommand;

        /// <summary>
        ///     The popup.
        /// </summary>
        private Popup popup;

        /// <summary>
        ///     The text box.
        /// </summary>
        private DatePickerTextBox textBox;

      #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the alternative calendar style.
        /// </summary>
        public Style AlternativeCalendarStyle
        {
            get
            {
                return (Style)this.GetValue(AlternativeCalendarStyleProperty);
            }

            set
            {
                this.SetValue(AlternativeCalendarStyleProperty, value);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The on apply template.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.textBox = this.GetTemplateChild("PART_TextBox") as DatePickerTextBox;
            this.popup = this.GetTemplateChild("PART_Popup") as Popup;
         
            if (this.AlternativeCalendarStyle != null)
            {
                Popup popup1 = this.popup;
                if (popup1 != null)
                {
                    var calendar = popup1.Child as Calendar;

                    if (calendar != null)
                    {
                        calendar.Style = this.AlternativeCalendarStyle;
                        calendar.ApplyTemplate();

                        this.gotoTodayButton = calendar.Template.FindName("PART_CalenderButton", calendar) as Button;
                    }
                }

                if (this.gotoTodayButton != null)
                {
                    this.gotoTodayCommand = new DelegateCommand(this.ExecuteGoToTodayCommand, this.CanExecuteGoToTodayCommand);
                    this.gotoTodayButton.Command = this.gotoTodayCommand;
                }
            }

            this.textBox.PreviewKeyDown -= this.DatePickerTextBoxPreviewKeyDown; // unhook
            this.textBox.PreviewKeyDown += this.DatePickerTextBoxPreviewKeyDown; // hook
        }
        #endregion

        #region Methods

        /// <summary>
        /// The on alternative calendar style changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnAlternativeCalendarStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (DatePickerEx)d;
            target.ApplyTemplate();
        }

        /// <summary>
        /// The can execute go to today command.
        /// </summary>
        /// <param name="param">
        /// The param.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CanExecuteGoToTodayCommand(object param)
        {
            return this.gotoTodayButton != null; // && !this.BlackoutDates.Contains(DateTime.Today);
        }

        /// <summary>
        /// The date picker text box_ preview key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DatePickerTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.Down)
            {
                int direction = e.Key == Key.Up ? 1 : -1;

                DateTime result;
                if (!DateTime.TryParse(this.Text, out result))
                {
                    return;
                }

                char delimeter = ' ';

                switch (this.SelectedDateFormat)
                {
                    case DatePickerFormat.Short: // dd/mm/yyyy
                        delimeter = '/';
                        break;
                    case DatePickerFormat.Long: // day month  year
                        delimeter = ' ';
                        break;
                }

                int index = 3;
                bool foundIt = false;
                for (int i = this.Text.Length - 1; i >= 0; i--)
                {
                    if (this.Text[i] == delimeter)
                    {
                        --index;
                        if (this.textBox.CaretIndex > i)
                        {
                            foundIt = true;
                            break;
                        }
                    }
                }

                if (!foundIt)
                {
                    index = 0;
                }

                switch (index)
                {
                    case 0: // Day
                        result = result.AddDays(direction);
                        break;
                    case 1: // Month
                        result = result.AddMonths(direction);
                        break;
                    case 2: // Year
                        result = result.AddYears(direction);
                        break;
                }

                while (this.BlackoutDates.Contains(result))
                {
                    result = result.AddDays(direction);
                }

                DateTimeFormatInfo dtfi = DateTimeHelper.GetCurrentDateFormat();
                switch (this.SelectedDateFormat)
                {
                    case DatePickerFormat.Short:
                        this.Text = string.Format(CultureInfo.CurrentCulture, result.ToString(dtfi.ShortDatePattern, dtfi));
                        break;
                    case DatePickerFormat.Long:
                        this.Text = string.Format(CultureInfo.CurrentCulture, result.ToString(dtfi.LongDatePattern, dtfi));
                        break;
                }

                switch (index)
                {
                    case 1:
                        this.textBox.CaretIndex = this.Text.IndexOf(delimeter) + 1;
                        break;
                    case 2:
                        this.textBox.CaretIndex = this.Text.LastIndexOf(delimeter) + 1;
                        break;
                }
            }
        }

        /// <summary>
        /// The execute go to today command.
        /// </summary>
        /// <param name="param">
        /// The param.
        /// </param>
        private void ExecuteGoToTodayCommand(object param)
        {
            this.SelectedDate = null;
            Mouse.Capture(null);
        }

        #endregion
    }
}