using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace SafeAuthenticator.Controls.Converters
{
    class ContainerNameToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // if (targetType != typeof(string))
            // {
            //    throw new InvalidOperationException("The target must be a string");
            // }
            var text = value.ToString();
            switch (text)
            {
                case "_documents":
                    return "documents";
                case "_downloads":
                    return "downloads";
                case "_music":
                    return "music";
                case "_pictures":
                    return "pictures";
                case "_videos":
                    return "videos";
                case "_public":
                    return "publicContainer";
                case "_publicNames":
                    return "publicNames";
                case "App Container":
                    return "publicNames";
                default:
                    throw new Exception();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
