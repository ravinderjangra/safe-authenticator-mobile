using System.Reflection;
using Xamarin.Forms;

namespace SafeAuthenticator.Controls
{
    public static class AccessExtensions
    {
        public static object Call(this object o, string methodName, params object[] args)
        {
            var mi = o.GetType().GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (mi != null)
            {
                return mi.Invoke(o, args);
            }
            return null;
        }
    }

    public static class XamarinFormsExtensions
    {
        public static T GetInternalField<T>(this BindableObject element, string propertyKeyName)
            where T : class
        {
            // reflection stinks, but hey, what can you do?
            var pi = element.GetType().GetField(propertyKeyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var key = (T)pi?.GetValue(element);

            return key;
        }
    }
}
