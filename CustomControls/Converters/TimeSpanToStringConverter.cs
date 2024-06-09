using System.Globalization;
using System.Windows.Data;

namespace CustomControls.Converters;

class TimeSpanToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TimeSpan timeSpan)
        {
            // 格式化为 "xx时xx分xx秒"，去除前导零
            var hours = timeSpan.Hours > 0 ? timeSpan.Hours.ToString() + "时" : "";
            var minutes = timeSpan.Minutes > 0 ? timeSpan.Minutes.ToString() + "分" : "";
            var seconds = timeSpan.Seconds >= 0 ? timeSpan.Seconds.ToString() + "秒" : "";

            // 如果有时，就不显示秒
            if (timeSpan.Hours > 0)
            {
                return $"{hours}{minutes}";
            }
            return $"{minutes}{seconds}";
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
