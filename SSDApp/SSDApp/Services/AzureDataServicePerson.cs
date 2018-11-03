using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Collections;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SSDApp.Models;
using SSDApp.Views;
using System.Diagnostics;
using MvvmHelpers;
using Xamarin.Forms;
using System.Linq;

namespace SSDApp.Services
{
    public class AzureDataServicePerson : ContentPage
    {
        public MobileServiceClient MobileService { get; set; } = null;
        IMobileServiceSyncTable<Person> personTable;

        bool isInitialised;

        /*****************************************************************
            *INITIALIZE METHOD - CONNECTS TO THE DATABASE
        ****************************************************************/

        public async Task Intialize()
        {
            if (isInitialised)
            {
                return;
            }

            //Create our client
            MobileService = new MobileServiceClient("https://ssds00164997.azurewebsites.net");

            const string path = "Person.db";
            //setup our local sqlite store and intialize our table
            var store = new Microsoft.WindowsAzure.MobileServices.SQLiteStore.MobileServiceSQLiteStore(path);
            store.DefineTable<Person>();
            await MobileService.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());

            //Get our sync table that will call out to azure
            personTable = MobileService.GetSyncTable<Person>();

            isInitialised = true;
        }

        /*****************************************************************
        *METHOD TO Add a NEW Person
        ****************************************************************/

        public async Task<Person> AddPerson(string firstName, string lastName, int age, string ppsn, int cardNumber)
        {
            await Intialize();
            //create and insert coffee
            var newPerson = new Person
            {

                FirstName = firstName,
                LastName = lastName,
                Age = age,
                PPSN = ppsn,
                CardNumber = cardNumber
            };

            await personTable.InsertAsync(newPerson);

            //Synchronize coffee
            await SyncPersons();
            return newPerson;
        }

        /*****************************************************************
        *METHOD TO CHECK IF A PPSN IS ALREADY IN DATABASE
        ****************************************************************/
        public async Task<bool> CheckPPSN(string PPSN)
        {
            await Intialize();
            await SyncPersons();
            bool answer = false;
            //System.Diagnostics.Debug.WriteLine((await App.MobileService.GetTable<User>().LookupAsync(1) as User).firstName);
            //User item = await coffeeTable.LookupAsync("6cc1aca348714a26af9c1d9d1757d0c2");

            List<Person> item = await personTable
            .Where(todoItem => todoItem.PPSN == PPSN)
            .ToListAsync();

            // DisplayAlert("ITEM CONTENT",item.ToString(),"Cancel");

            foreach (var x in item)
            {

                string daName = x.PPSN;

                if (item.Count() > 0)
                {
                   

                    answer = true;
                    break;
                }
                else
                {
                    answer = false;
                    break;
                }

            }
            return answer;

        }


        /*****************************************************************
            *METHOD TO SYNC THE DATABSE TO THE APP
        ****************************************************************/

        public async Task SyncPersons()
        {

            try
            {
                await personTable.PullAsync("allpersons", personTable.CreateQuery());
                await MobileService.SyncContext.PushAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to sync coffees, that is alright as we have offline capabilities: " + ex);
            }
        }

        public async Task<IEnumerable> GetIProducts()
        {
            await Intialize();
            await LoadPersons();
            return await personTable.OrderBy(c => c.LastName).ToEnumerableAsync();
        }

        /*****************************************************************
                *METHOD TO LOAD THE people ONTO the PAGE
         ****************************************************************/
        public async Task<string> LoadPersons()
        {
            await Intialize();
            await SyncPersons();
            string answer = "false";

            try
            {
                List<Person> item = await personTable
             .Where(todoItem => todoItem.FirstName != null)
             .OrderBy(todoItem => todoItem.LastName)
             .ToListAsync();
                ReadPage.ListViewPersons.Clear();
                
                foreach (var x in item)
                {
                    Person one = new Person(x.Id, x.FirstName, x.LastName, x.Age, x.PPSN, x.CardNumber);
                    ReadPage.ListViewPersons.Add(one);

                    answer = "true";
                }

                return answer;
            }

            catch (Exception er)
            {
                await DisplayAlert("Alert", "da error: " + er, "Ok");
                return answer;

            }


        }



        /*****************************************************************
               *METHOD TO uPDATE THE A PERSON
        ****************************************************************/
        public async Task<Person> UpdatePersons(Person editSelection)
        {
            await Intialize();
            // await SyncProducts();
            string answer = "false";
            //System.Diagnostics.Debug.WriteLine((await App.MobileService.GetTable<User>().LookupAsync(1) as User).firstName);
            //User item = await coffeeTable.LookupAsync("6cc1aca348714a26af9c1d9d1757d0c2");

            var Prod = await personTable
                .Where(p => p.Id == editSelection.Id)
                .ToListAsync();

            //Then change the properties you want and after that call UpdateAsync
            var editPerson = new Person
            {
                Id = editSelection.Id,
                FirstName = editSelection.FirstName,
                LastName = editSelection.LastName,
                Age = editSelection.Age,
                PPSN = editSelection.PPSN,
                CardNumber = editSelection.CardNumber

            };

            // var inserted = await productTable.UpdateAsync(jo);
            // editSelection.ProductName =
            //Debug.WriteLine("the error: " + editSelection.Id.ToString());

            await personTable.UpdateAsync(editPerson);
            await SyncPersons();

            return editSelection;
        }

        /*****************************************************************
               *METHOD TO DElete A PERSON
        ****************************************************************/
        public async Task<Person> DeletePersons(Person editSelection)
        {
            await Intialize();
           

            var Prod = await personTable
                .Where(p => p.Id == editSelection.Id)
                .ToListAsync();
            await personTable.DeleteAsync(editSelection);
            await SyncPersons();

            return editSelection;
        }

    }
}

