using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace ReadyOrNotOpenMic.UI.Converters
{
    public partial class BoolToInvisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value is bool hidden && hidden) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is Visibility visibility && visibility == Visibility.Visible;
        }
    }
}
