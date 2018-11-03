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
	public partial class DeletePage : ContentPage
	{
		public DeletePage ()
		{
			InitializeComponent ();
            LoadProducts();

            deleteView.ItemsSource = ReadPage.ListViewPersons;
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
                var selection = e.SelectedItem as Person;
                var res =await DisplayAlert("Delete Person!!!", "Are you sure you want to delete: " + selection.FirstName, "ok", "Cancel");
                if (res ==true)
                {
                    AzureDataServicePerson azureDataServicePerson;
                    azureDataServicePerson = new AzureDataServicePerson();


                    await azureDataServicePerson.DeletePersons(selection);
                  await  DisplayAlert("Person Deleted", "Person Deleted", "ok");
                    await Navigation.PopAsync();
                }

                
            }
        }
    }
}