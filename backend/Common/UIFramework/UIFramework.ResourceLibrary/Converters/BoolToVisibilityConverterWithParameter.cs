namespace Luminator.UIFramework.ResourceLibrary.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public class BoolToVisibilityConverterWithParameter : IValueConverter
        {
            #region Implementation of IValueConverter
            object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (targetType != typeof(Visibility))
                    throw new ArgumentOutOfRangeException("targetType", "VisibilityConverter can only convert to Visibility");

                Visibility visibility = Visibility.Visible;

                if (value == null)
                    visibility = Visibility.Collapsed;

                if (value is bool)
                    visibility = (bool)value ? Visibility.Visible : Visibility.Collapsed;

                if (value is string)
                    visibility = String.IsNullOrEmpty((string)value) ? Visibility.Collapsed : Visibility.Visible;

                if (parameter != null && parameter.ToString().Equals("Inverse"))
                {

                    visibility = (visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
                }
                return visibility;
            }

            object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (!(value is Visibility))
                    throw new ArgumentOutOfRangeException("value", "VisibilityConverter can only convert from Visibility");

                if (targetType == typeof(bool))
                    return ((Visibility)value == Visibility.Visible) ? true : false;

                throw new ArgumentOutOfRangeException("targetType", "VisibilityConverter can only convert to Boolean");
            }
            #endregion
        }
}
