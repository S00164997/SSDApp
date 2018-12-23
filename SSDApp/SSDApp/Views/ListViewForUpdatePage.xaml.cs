using SSDApp.Models;
using SSDApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSDApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ListViewForUpdatePage : ContentPage
	{
		public ListViewForUpdatePage ()
		{
			InitializeComponent ();
            LoadProducts();
            editlistView.ItemsSource = ReadPage.ListViewPersons;
        }

        

        public void LoadProducts()
        {
            AzureDataServicePerson azureDataServicePerson;
            azureDataServicePerson = new AzureDataServicePerson();

            try
            {
                azureDataServicePerson.LoadPersons();
            }
            catch (Exception er)
            {
                DisplayAlert("Alert", "Could not load products" + er, "Ok");
            }
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var selection = e.SelectedItem as Person;//load produts
                // await DisplayAlert("mm", "Product: " + selection.Id, "ok");
                await Navigation.PushAsync(new UpdatePage(selection));//pass details to the update page
            }
        }
    }
}