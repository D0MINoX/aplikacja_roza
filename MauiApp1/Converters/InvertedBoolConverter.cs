using System.Globalization;

namespace MauiApp1.Converters
{
    public class InvertedBoolConverter : IValueConverter
    {
        // Zmieniono CultureCollection na CultureInfo
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool booleanValue)
                return !booleanValue;

            return false;
        }

        // Zmieniono CultureCollection na CultureInfo
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}