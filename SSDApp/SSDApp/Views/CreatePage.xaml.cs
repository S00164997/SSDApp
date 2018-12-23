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
using System.Text.RegularExpressions;



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
                Regex NameReg = new Regex("[A-Za-z]$");         //regex for input
                Regex AgeReg = new Regex("[0-9]*$");
                Regex PPSNReg = new Regex("(/d{7})([A-Z]{1})$");
                var a = IsValid(FirstName.Text.ToString(), NameReg.ToString());
                var b = IsValid(LastName.Text.ToString(), NameReg.ToString());
                var bb = IsValid(Age.Text.ToString(), AgeReg.ToString());
                var d = IsValid(PPSN.Text.ToString(), PPSNReg.ToString());
                var E = IsPPSValid(PPSN.Text.ToString());
                await DisplayAlert("hh", a.ToString(), "ok");

                bool IsValid(string value, string Reg)
                {
                    return Regex.IsMatch(value,Reg);
                }
        

                bool IsPPSValid(string Value)   //real world ppsn checker users %23
                {
                   // string charValue = " ";
                    char charValue = ' ';
                    //string[] characters = Value;
                    char[] characters = Value.ToCharArray();
                    charValue = characters[7];//set the last index of the array to the variable charValue
                    string charaVal = charValue.ToString().ToUpper();
                    
                    Array.Resize(ref characters, 7);//remove the charachter from the array
                    
                    //char[] array = { '1', '2', '3', '4' };
                    int[] PPSNumbers = characters.Select(c => Convert.ToInt32(c.ToString())).ToArray();//convert the charArray to an int array

                    int[] arr1 = new int[] {8,7,6,5,4,3,2};
                    
                    int sum = 0;

                    for (int i = 0; i < arr1.Length; i++)
                    {
                        //foreach (var pps in PPSNumbers)
                        //{
                        sum +=  PPSNumbers[i] * arr1[i] ;
                    //}

                }

                decimal alphabetIndex = 0;
                    int mod23 = 23;
                    alphabetIndex = sum % mod23;
                    //decimal REALINDEX = Math.Round(alphabetIndex,3);
                    string letter=" ";

                    switch (alphabetIndex)
                    {
                        case 1:
                            letter = "A";
                            break;
                        case 2:
                            letter = "B";
                            break;
                        case 3:
                            letter = "C";
                            break;
                        case 4:
                            letter = "D";
                            break;
                        case 5:
                            letter = "E";
                            break;
                        case 6:
                            letter = "F";
                            break;
                        case 7:
                            letter = "G";
                            break;
                        case 8:
                            letter = "H";
                            break;
                        case 9:
                            letter = "I";
                            break;
                        case 10:
                            letter = "J";
                            break;
                        case 11:
                            letter = "K";
                            break;
                        case 12:
                            letter = "L";
                            break;
                        case 13:
                            letter = "M";
                            break;
                        case 14:
                            letter = "N";
                            break;
                        case 15:
                            letter = "O";
                            break;
                        case 16:
                            letter = "P";
                            break;
                        case 17:
                            letter = "Q";
                            break;
                        case 18:
                            letter = "R";
                            break;
                        case 19:
                            letter = "S";
                            break;
                        case 20:
                            letter = "T";
                            break;
                        case 21:
                            letter = "U";
                            break;
                        case 22:
                            letter = "V";
                            break;
                        case 23:
                            letter = "W";//PPS LETTERS STOP AT W    
                            break;
                        default:
                            DisplayAlert("Warning","Invalid PPSN","OK");
                            break;

                           

                    }
                    if (charaVal.ToString() == letter)//check letters are equal
                    {
                        return true;
                    }
                    else
                    {
                        DisplayAlert("Warning", "Invalid PPSN", "OK");
                        return false;
                    }

                }

                if (FirstName.Text == null || LastName.Text == null || Age.Text == null || PPSN.Text == null || CardNumber.Text == null)
                {
                    await DisplayAlert("Alert", "Not all fields have been entered", "Ok");
                }
                if (a != true)
                {
                    WarningLabel.Text = "First Name is not in the correct format";
                }
                else if (b != true)
                {
                    WarningLabel.Text = "Last Name is not in the correct format";
                }
                else if (bb != true)
                {
                    WarningLabel.Text = "PPS number is not in the correct format";
                }
                if (FirstName.Text == null || LastName.Text == null || Age.Text == null || PPSN.Text == null || CardNumber.Text == null)
                {
                    await DisplayAlert("Alert", "Not all fields have been entered", "Ok");
                }
                else 
                {
                    string ageEntry = (Age.Text);
                    string cardEntry =(CardNumber.Text);
                    string id = Guid.NewGuid().ToString();
                    //string producturl = ImageUrl.ToString();
                    //Quantity = Convert.ToInt32(Quantity);
                    Person personOne = new Person(id, FirstName.Text, LastName.Text, ageEntry, PPSN.Text, cardEntry);

                    await ExecuteDatabaseCommandAsync(personOne.FirstName, personOne.LastName, personOne.Age, personOne.PPSN, personOne.CardNumber);//sends to db service
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



        public async Task ExecuteDatabaseCommandAsync(string firstName, string lastName, string age, string ppsn, string cardNumber)
        {

            string enteredFirstName = FirstName.Text;
            string enteredLastName = LastName.Text;
            string enteredAge = (Age.Text);
            string enteredPPSN = PPSN.Text;
            string enteredCardN = (CardNumber.Text);

            azureDataServicePerson = new AzureDataServicePerson();
           //send accepted inputs to db
            

            if (IsBusy)
                return;
            bool exists;
            try
            {
                IsBusy = true;
                exists = await azureDataServicePerson.CheckPPSN(enteredPPSN);// check ppsn as its unique
               
                if (exists == true)
                {
                    await DisplayAlert("Alert", "Name: " + firstName + " already exists, Please check person table", "Ok");
                }
                else if (exists == false)
                {//if not in db insert

                    var coffee = await azureDataServicePerson.AddPerson(firstName, lastName, age, ppsn, cardNumber);
                    Coffees.Add(coffee);
                    await DisplayAlert("Alert", "Person Added", "Ok");
                     Navigation.PopAsync();

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("da error: " + ex);
                //http://pmhsilva.org/validate-irish-ppsn-with-javascript-jquery/
            }
            finally
            {
                IsBusy = false;
            }

        }
    }
}