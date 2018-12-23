using SSDApp.Models;
using SSDApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSDApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ReadPage : ContentPage
	{
        public static ObservableCollection<Person> ListViewPersons { get; } = new ObservableCollection<Person>();

        public ReadPage ()
		{
            ListViewPersons.Clear();
            InitializeComponent();
            LoadPersons();                              ///this page loads the person object onto the page
            listView.ItemsSource = ListViewPersons;
        }

        public void LoadPersons()
        {
            AzureDataServicePerson azureDataServicePerson;
            azureDataServicePerson = new AzureDataServicePerson();

            try
            {
                azureDataServicePerson.LoadPersons();
                //azureDataServiceProduct.GetIProducts();
            }
            catch (Exception er)
            {
                DisplayAlert("Alert", "Could not load products" + er, "Ok");
            }
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }
    }
}