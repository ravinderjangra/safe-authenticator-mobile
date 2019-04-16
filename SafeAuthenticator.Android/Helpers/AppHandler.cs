using System.Threading.Tasks;
using Android.Content;
using SafeAuthenticator.Droid.Helpers;
using SafeAuthenticator.Services;

[assembly: Xamarin.Forms.Dependency(typeof(AppHandler))]
namespace SafeAuthenticator.Droid.Helpers
{
    class AppHandler : IAppHandler
    {
        public Task<bool> LaunchApp(string uri)
        {
            bool result = false;

            try
            {
                var aUri = Android.Net.Uri.Parse(uri.ToString());
                var intent = new Intent(Intent.ActionView, aUri);
                MainActivity.AppContext.StartActivity(intent);
                result = true;
            }
            catch (ActivityNotFoundException)
            {
                result = false;
            }

            return Task.FromResult(result);
        }
    }
}
