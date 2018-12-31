using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebPage : ContentPage
    {
        private string _url = "https://invite.maidsafe.net/";

        protected override void OnAppearing()
        {
            base.OnAppearing();
            WebView.Source = _url;
        }

        public WebPage()
        {
            InitializeComponent();
        }
    }
}
