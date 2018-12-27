using System;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using SafeAuthenticator.Models;
using SafeAuthenticator.Native;
using SafeAuthenticator.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RequestDetailPage : PopupPage
    {
        public event EventHandler CompleteRequest;

        private readonly RequestDetailViewModel _viewModel;

        public RequestDetailPage(IpcReq req)
        {
            InitializeComponent();
            _viewModel = new RequestDetailViewModel(req);
            BindingContext = _viewModel;
        }

        private void Send_Response(object sender, EventArgs e)
        {
            if (sender == AllowButton)
            {
                CompleteRequest?.Invoke(this, new ResponseEventArgs(true));
            }
            else if (sender == DenyButton)
            {
                CompleteRequest?.Invoke(this, new ResponseEventArgs(false));
            }
            PopupNavigation.Instance.PopAsync(true);
        }

        private void Unselect_Item(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
            {
                return;
            }

            if (sender is ListView lv)
            {
                lv.SelectedItem = null;
            }
        }
    }
}
