using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Kastelo.Controls
{
    public class ModeToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var retVal = Visibility.Collapsed;
            if ((string)value == "Done")
                retVal = Visibility.Visible;

            if ((string)parameter == "LABEL")
            {
                retVal = retVal == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            }

            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
