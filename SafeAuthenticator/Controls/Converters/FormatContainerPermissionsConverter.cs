using System;
using System.Globalization;
using SafeAuthenticator.Models;
using Xamarin.Forms;

namespace SafeAuthenticator.Controls.Converters
{
    class FormatContainerPermissionsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // if (targetType != typeof(string))
            // {
            //    throw new InvalidOperationException("The target must be a string");
            // }
            var access = value as PermissionSetModel;
            var result = string.Empty;

            if (access.Read)
                result += "Read";
            if (access.Insert)
                result += result == string.Empty ? "Insert" : ", Insert";
            if (access.Update)
                result += result == string.Empty ? "Update" : ", Update";
            if (access.Delete)
                result += result == string.Empty ? "Delete" : ", Delete";
            if (access.ManagePermissions)
                result += result == string.Empty ? "ManagePermissions" : ", ManagePermissions";
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
