using System.Windows.Input;
using SafeAuthenticator.Helpers;
using Xamarin.Forms;

namespace SafeAuthenticator.ViewModels
{
    class SettingsViewModel : BaseViewModel
    {
        public ICommand LogoutCommand { get; }

        private string _accountStorageInfo;

        public string AccountStorageInfo
        {
            get => _accountStorageInfo;
            set => SetProperty(ref _accountStorageInfo, value);
        }

        public bool AuthReconnect
        {
            get => Authenticator.AuthReconnect;
            set
            {
                if (Authenticator.AuthReconnect != value)
                {
                    Authenticator.AuthReconnect = value;
                }

                OnPropertyChanged();
            }
        }

        public SettingsViewModel()
        {
            LogoutCommand = new Command(OnLogout);
            GetAccountInfo();
        }

        private async void GetAccountInfo()
        {
            var acctStorageTuple = await Authenticator.GetAccountInfoAsync();
            AccountStorageInfo = $"{acctStorageTuple.Item1} / {acctStorageTuple.Item2}";
        }

        private async void OnLogout()
        {
            if (await Application.Current.MainPage.DisplayAlert(
                "Confirm Logout",
                "Are you sure you want to logout?",
                "Logout",
                "Cancel"))
            {
                await Authenticator.LogoutAsync();
                MessagingCenter.Send(this, MessengerConstants.NavLoginPage);
            }
        }
    }
}
