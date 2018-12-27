using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticator.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthorisationPopup : PopupPage
    {
        public List<Container> ContainerList { get; set; }

        public AuthorisationPopup()
        {
            InitializeComponent();
            ContainerListView.ItemTapped += UnSelectSelectedItem;

            ContainerList = new List<Container>
            {
                new Container { Title = "Public Names", Details = "Read, Insert, Update, Delete, Manage Permissions", Image = "publicName" },
                new Container { Title = "Public", Details = "Read, Insert, Update, Delete, Manage Permissions", Image = "publics" },
                new Container { Title = "Documents", Details = "Read, Insert, Update, Delete, Manage Permissions", Image = "document" },
                new Container { Title = "Music", Details = "Read, Insert, Update, Delete, Manage Permissions", Image = "music" },
            };
        }

        private void UnSelectSelectedItem(object sender, ItemTappedEventArgs e)
        {
            var listView = sender as ListView;
            listView.SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            SetData();
        }

        private void SetData()
        {
            ContainerListView.ItemsSource = ContainerList;
        }
    }

    public class Container
    {
        public string Title { get; set; }

        public string Details { get; set; }

        public string Image { get; set; }
    }
}
