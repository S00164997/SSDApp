using MvvmHelpers;
using SSDApp.Models;
using SSDApp.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;



namespace SSDApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CreatePage : ContentPage
	{
		public CreatePage ()
		{
			InitializeComponent ();
		}

        private async void createPerson(object sender, EventArgs e)
        {
            try
            {
                if (FirstName.Text == null || LastName.Text == null || Age.Text == null || PPSN.Text == null || CardNumber.Text == null)
                {
                    await DisplayAlert("Alert", "Not all fields have been entered", "Ok");
                }
                else
                {
                    int ageEntry = Convert.ToInt32(Age.Text);
                    int cardEntry = Convert.ToInt32(CardNumber.Text);
                    string id = Guid.NewGuid().ToString();
                    //string producturl = ImageUrl.ToString();
                    //Quantity = Convert.ToInt32(Quantity);
                    Person personOne = new Person(id, FirstName.Text, LastName.Text, ageEntry, PPSN.Text, cardEntry);

                    await ExecuteDatabaseCommandAsync(personOne.FirstName, personOne.LastName, personOne.Age, personOne.PPSN, personOne.CardNumber);
                }
            }
            catch (Exception ee)
            {
                ee.Message.ToString();
                ee.Source.ToString();
                throw;
            }
        }
        AzureDataServicePerson azureDataServicePerson;
        public ObservableRangeCollection<Person> Coffees { get; } = new ObservableRangeCollection<Person>();



        public async Task ExecuteDatabaseCommandAsync(string firstName, string lastName, int age, string ppsn, int cardNumber)
        {

            string enteredFirstName = FirstName.Text;
            string enteredLastName = LastName.Text;
            int enteredAge = Convert.ToInt32(Age.Text);
            string enteredPPSN = PPSN.Text;
            int enteredCardN = Convert.ToInt32(CardNumber.Text);

            azureDataServicePerson = new AzureDataServicePerson();
            //this.FindByName<Entry>("John");
            

            if (IsBusy)
                return;
            bool exists;
            try
            {
                IsBusy = true;
                exists = await azureDataServicePerson.CheckPPSN(enteredPPSN);
                //exists = await azureDataServiceProduct.CheckBarcode(barcode);
                if (exists == true)
                {
                    await DisplayAlert("Alert", "Name: " + firstName + " already exists, Please check person table", "Ok");
                }
                else if (exists == false)
                {

                    var coffee = await azureDataServicePerson.AddPerson(firstName, lastName, age, ppsn, cardNumber);
                    Coffees.Add(coffee);
                    await DisplayAlert("Alert", "Person Added", "Ok");
                     Navigation.PopAsync();

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("da error: " + ex);

            }
            finally
            {
                IsBusy = false;
            }

        }
    }
}