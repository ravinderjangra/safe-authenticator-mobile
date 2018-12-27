using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace SafeAuthenticator.Controls.Converters
{
    class FormatContainerNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // if (targetType != typeof(string))
            // {
            //    throw new InvalidOperationException("The target must be a string");
            // }
            var text = value.ToString();
            return text == "App Container" ?
                "App's own container" :
                string.Format("{0}{1}", text.Substring(1, 1).ToUpper(), text.Substring(2));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
