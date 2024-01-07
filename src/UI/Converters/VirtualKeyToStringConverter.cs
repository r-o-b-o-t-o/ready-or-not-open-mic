using Microsoft.UI.Xaml.Data;
using System;
using Windows.System;

namespace ReadyOrNotOpenMic.UI.Converters
{
    public partial class VirtualKeyToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return VirtualKeyToString(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return NameToVirtualKey(value);
        }

        private static string VirtualKeyToString(object value)
        {
            if (value is not VirtualKey key)
            {
                return null;
            }

            return Utils.AddSpaceToCamelCase(Enum.GetName(typeof(VirtualKey), key) ?? "");
        }

        private static VirtualKey NameToVirtualKey(object value)
        {
            throw new NotImplementedException();
        }
    }
}
