using System;
using System.Globalization;
using System.Windows.Data;

namespace ReactiveEditor.Converters
{
    public class BaseDataTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
          CultureInfo culture)
        {
            return value?.GetType().BaseType;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
          CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}