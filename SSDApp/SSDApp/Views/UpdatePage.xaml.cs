using SSDApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SSDApp.Services;

namespace SSDApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UpdatePage : ContentPage
	{
        public Models.Person editSelection = new Person();
        public UpdatePage (Models.Person selection)// acceps a person object
		{
			InitializeComponent ();         // this page updates the person object

            FirstName.Text = selection.FirstName;
            LastName.Text = selection.LastName;
            Age.Text = selection.Age.ToString();
            PPSN.Text = selection.PPSN;
            CardNumber.Text = selection.CardNumber.ToString();


            editSelection.Id = selection.Id;
            editSelection.FirstName = selection.FirstName;
            editSelection.LastName = selection.LastName;
            editSelection.Age = selection.Age;
            editSelection.PPSN = selection.PPSN;
            editSelection.CardNumber = selection.CardNumber;

            
        }



        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }

        private async Task updatePerson(object sender, EventArgs e)
        {

            editSelection.FirstName = FirstName.Text;
            editSelection.LastName = LastName.Text;
            editSelection.Age = (Age.Text);
            editSelection.PPSN = PPSN.Text;
            editSelection.CardNumber = (CardNumber.Text);

            AzureDataServicePerson azureDataServicePerson;
            azureDataServicePerson = new AzureDataServicePerson();

            await azureDataServicePerson.UpdatePersons(editSelection);
           await  DisplayAlert("Person Updated", "You have updated " + editSelection.FirstName+" "+editSelection.LastName, "ok");
            await Navigation.PushAsync(new MainPage());

        }
    }
}