using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfSplashScreen.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;

            //MANAGE false visibility state
            Visibility falseVisibilityParam = Visibility.Collapsed;
            if (parameter != null)
            {
                if (!Enum.TryParse((string)parameter, true, out falseVisibilityParam))
                {
                    falseVisibilityParam = Visibility.Collapsed;
                }
            }

            return (bool)value ? Visibility.Visible : falseVisibilityParam;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BooleanToVisibilityHiddenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Hidden;

            //MANAGE false visibility state
            Visibility falseVisibilityParam = Visibility.Hidden;
            if (parameter != null)
            {
                if (!Enum.TryParse((string)parameter, true, out falseVisibilityParam))
                {
                    falseVisibilityParam = Visibility.Hidden;
                }
            }

            return (bool)value ? Visibility.Visible : falseVisibilityParam;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}