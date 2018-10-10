namespace Luminator.UIFramework.ResourceLibrary.Converters
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public class EnumStringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return Visibility.Visible;
            }

            if (targetType != typeof(Visibility))
                throw new ArgumentOutOfRangeException("targetType", "VisibilityConverter can only convert to Visibility");

            Visibility visibility = Visibility.Visible;

            visibility = value != null && value.Equals(parameter) ? Visibility.Visible : Visibility.Collapsed;

            return visibility;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Visibility.Collapsed;
        }
    }
}
