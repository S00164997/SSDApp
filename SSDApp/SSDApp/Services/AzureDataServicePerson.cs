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
using System.Security.Cryptography;
using System.IO;

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

        public async Task<Person> AddPerson(string firstName, string lastName, string age, string ppsn, string cardNumber)
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

            string ENFirstN = await Encrypt(newPerson.FirstName.ToString());
            string ENLastN = await Encrypt(newPerson.LastName.ToString());
            string ENAge = await Encrypt(newPerson.Age.ToString());
            string ENPPSN = await Encrypt(newPerson.PPSN.ToString());
            string ENCardNumber = await Encrypt(newPerson.CardNumber.ToString());

            string DeName = await Decrypt(ENFirstN.ToString());
            ////////////////////// send person
            //var encPerson = Encrypt(newPerson);
            

            var encPerson = new Person
            {

                FirstName = ENFirstN,
                LastName = ENLastN,
                Age = ENAge,
                PPSN = ENPPSN,
                CardNumber = ENCardNumber
            };

            ////foreach (var item in encList)
            ////{
            ////  encPerson.FirstName=  Encrypt(item.FirstName.ToString());
            ////}

            await personTable.InsertAsync(encPerson);

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
                    string FN =  await Decrypt(x.FirstName);
                    string LN = await Decrypt(x.LastName);
                    string Age = await Decrypt(x.Age);
                    string PPS = await Decrypt(x.PPSN);
                    string CN = await Decrypt(x.CardNumber);
                    Person one = new Person(x.Id, FN.ToString(), LN.ToString(), Age.ToString(),PPS.ToString(),CN.ToString());
                    //Person one = new Person(x.Id, x.FirstName, x.LastName, x.Age, x.PPSN, x.CardNumber);

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


        /*****************************************************************
               ENCRYPTION DATA
        ****************************************************************/
        public async Task<string> Encrypt(string EncryptPerson)
        {
            string EncryptPerson2;
            using (AesManaged myAes = new AesManaged())
            {
                myAes.Padding = PaddingMode.PKCS7;
                myAes.KeySize = 128;          // in bits
                myAes.Key = new byte[128 / 8];  // 16 bytes for 128 bit encryption
                myAes.IV = new byte[128 / 8];   // AES needs a 16-byte IV
                byte[] encrypted = EncryptStringToBytes_Aes(EncryptPerson, myAes.Key, myAes.IV);
                EncryptPerson2 = (Convert.ToBase64String(encrypted));
            }
            
            //Console.WriteLine("String to encrypt");
            //string x = Console.ReadLine();//sgdfhgfdghfg  READ PRESON

            

            

             byte[] EncryptStringToBytes_Aes(string EncryptPerson3, byte[] Key, byte[] IV)//encrypting
            {

                byte[] encrypted;

                // Create an AesManaged object
                // with the specified key and IV.
                using (AesManaged aesAlg = new AesManaged())
                {
                    aesAlg.Padding = PaddingMode.PKCS7;
                    aesAlg.KeySize = 128;          // in bits
                    aesAlg.Key = new byte[128 / 8];  // 16 byte for 128 bit encryption
                    aesAlg.IV = new byte[128 / 8];   // AES needs a 16-byte Inilitasation Vector
                    aesAlg.Key = Key;
                    aesAlg.IV = IV;

                    // Create an encryptor to perform the stream transform.
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for encryption.
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                //Write all data to the stream.
                                swEncrypt.Write(EncryptPerson);
                            }
                            encrypted = msEncrypt.ToArray();
                        }
                    }
                }
                GC.Collect();
                return  encrypted;
            }
            return EncryptPerson2;
        }


        /*****************************************************************
               DECRYPTION DATA
        ****************************************************************/
        public async Task<string> Decrypt(string EncryptPerson)
        {//method for decryption
            string DecryptPerson2;

            byte[] bytes = System.Convert.FromBase64String(EncryptPerson);
            //byte[] y2 = Encoding.UTF8.GetBytes(y);
            //byte[] y22 = Encoding.UTF8.GetEncoder(y2);

            using (AesManaged myAes = new AesManaged())
            {
                myAes.Padding = PaddingMode.PKCS7;
                myAes.KeySize = 128;          // in bits
                myAes.Key = new byte[128 / 8];  // 16 bytes for 128 bit encryption
                myAes.IV = new byte[128 / 8];   // AES needs a 16-byte IV
                string dec = DecryptStringFromBytes_Aes(bytes, myAes.Key, myAes.IV);//decrypt file
                DecryptPerson2 = dec;
            }

            //Console.WriteLine("String to encrypt");
            //string x = Console.ReadLine();//sgdfhgfdghfg  READ PRESON
             string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)//decrypt file
            {
                // Check arguments.

                // Declare the string used to hold
                // the decrypted text.
                string plaintext = null;

                // Create an AesManaged object
                // with the specified key and IV.
                using (AesManaged aesAlg = new AesManaged())
                {
                    aesAlg.Padding = PaddingMode.PKCS7;
                    aesAlg.KeySize = 128;          // in bits
                    aesAlg.Key = new byte[128 / 8];  // 16 bytes for 128 bit encryption
                    aesAlg.IV = new byte[128 / 8];   // AES needs a 16-byte IV
                    aesAlg.Key = Key;
                    aesAlg.IV = IV;

                    // Create a decryptor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for decryption.
                    using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {

                                // Read the decrypted bytes from the decrypting stream
                                // and place them in a string.
                                plaintext = srDecrypt.ReadToEnd();
                                
                            }
                        }
                    }
                    GC.Collect();
                    return plaintext;
                }
                
                
            }


            return DecryptPerson2;


           // return EncryptPerson2;// = Convert.ToBase64String(encrypted);
        }
    }
}



