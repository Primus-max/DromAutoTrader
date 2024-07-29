using System.Globalization;
using System.Windows.Data;

namespace DromAutoTrader.Views.Converters
{
    public class ListViewHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double windowHeight)
            {
                // Установите желаемый коэффициент или оставьте его как есть
                double coefficient = 0.65;

                return windowHeight * coefficient;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
