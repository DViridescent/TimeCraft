using System.Globalization;
using System.Windows.Data;

namespace CustomControls.Converters;

class UniformGridColumnConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType == typeof(int))
        {
            return (int)value switch
            {
                5 or 6 or 9 => 3,
                <= 4 or 7 or 8 or _ => 4,
            };
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}
