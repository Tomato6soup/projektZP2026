using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace ResearchHub.Services
{
    public class StatusToBgConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isOtwarta)
            {
                // Zwraca zielony (Success) jeśli true, czerwony (Danger) jeśli false
                return isOtwarta
                    ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#166534"))
                    : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC2626"));
            }
            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
