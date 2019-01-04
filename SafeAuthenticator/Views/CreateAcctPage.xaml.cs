using System.Diagnostics;
using System.Linq;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateAcctPage : ContentPage, ICleanup
    {
        private readonly CreateAcctViewModel _createAcctViewModel;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _createAcctViewModel.InitializeStrengthChecker();
        }

        public CreateAcctPage()
        {
            InitializeComponent();
            _createAcctViewModel = new CreateAcctViewModel();
            BindingContext = _createAcctViewModel;

            MessagingCenter.Subscribe<CreateAcctViewModel>(
                this,
                MessengerConstants.NavHomePage,
                async _ =>
                {
                    MessageCenterUnsubscribe();
                    if (!App.IsPageValid(this))
                    {
                        return;
                    }

                    var rootPage = Navigation.NavigationStack.FirstOrDefault();
                    if (rootPage == null)
                    {
                        return;
                    }
                    Navigation.InsertPageBefore(new HomePage(), rootPage);
                    await Navigation.PopToRootAsync();
                });
        }

        public void MessageCenterUnsubscribe()
        {
            MessagingCenter.Unsubscribe<CreateAcctViewModel>(this, MessengerConstants.NavHomePage);
        }
    }
}
