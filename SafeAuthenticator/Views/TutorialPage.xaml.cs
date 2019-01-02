using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TutorialPage : ContentPage
    {
        public TutorialPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<TutorialViewModel>(
                this,
                MessengerConstants.NavCreateAcctPage,
                async _ =>
                {
                    if (!App.IsPageValid(this))
                    {
                        MessageCenterUnsubscribe();
                        return;
                    }
                    Navigation.InsertPageBefore(new CreateAcctPage(), this);
                    await Navigation.PopAsync();
                });

            MessagingCenter.Subscribe<TutorialViewModel>(
                this,
                MessengerConstants.NavLoginPage,
                async _ =>
                {
                    if (!App.IsPageValid(this))
                    {
                        MessageCenterUnsubscribe();
                        return;
                    }
                    Navigation.InsertPageBefore(new LoginPage(), this);
                    await Navigation.PopAsync();
                });
        }

        private void MessageCenterUnsubscribe()
        {
            MessagingCenter.Unsubscribe<TutorialViewModel>(this, MessengerConstants.NavCreateAcctPage);
            MessagingCenter.Unsubscribe<TutorialViewModel>(this, MessengerConstants.NavLoginPage);
        }
    }
}
