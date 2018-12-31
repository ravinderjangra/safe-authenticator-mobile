using SafeAuthenticator.Helpers;
using SafeAuthenticator.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<SettingsViewModel>(
                this,
                MessengerConstants.NavLoginPage,
                async _ =>
                {
                    MessageCenterUnsubscribe();
                    if (!App.IsPageValid(this))
                    {
                        return;
                    }

                    Navigation.InsertPageBefore(new LoginPage(), this);
                    await Navigation.PopAsync();
                });
        }

        private void MessageCenterUnsubscribe()
        {
            MessagingCenter.Unsubscribe<HomeViewModel>(this, MessengerConstants.NavLoginPage);
        }
    }
}
