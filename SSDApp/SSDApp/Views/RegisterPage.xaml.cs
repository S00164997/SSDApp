using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Firebase.Auth;

namespace SSDApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();                  // register's a user with firebase
        }

        private async void registerClicked(object sender, EventArgs e)
        {
            try
            {

           
            string emails = email.Text;
            string passs = password.Text;
            var g = await FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(emails, passs);// command to register
             await   DisplayAlert("Hi","Register :"+g.ToString(),"ok");
            }
            catch (Firebase.Auth.FirebaseAuthInvalidCredentialsException h)
            {
                h.ToString();
                throw;
            }
        }

    }
}