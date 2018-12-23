using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Firebase;
using Firebase.Auth;

namespace SSDApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void loginClicked(object sender, EventArgs e)
        {

            try
            {


                string pat = password.Text;
               
                string em = email.Text;
                
                var res = await FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(em, pat);
               //send login data to firebase to auth

                await DisplayAlert("Success", "Login Successful!!", "ok");
                await Navigation.PushAsync(new MainPage());
                
            }
            catch (Firebase.Auth.FirebaseAuthInvalidCredentialsException InvCre)
            {
                await DisplayAlert("Invalid", InvCre.ErrorCode.ToString(), "ok");
                await Navigation.PopAsync();
                //throw;
            }
            catch (Firebase.FirebaseNetworkException NetEx)
            {
                await DisplayAlert("Invalid", NetEx.Message.ToString(), "ok");
                await Navigation.PopAsync();
            }
        }

        private async void Register(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
        }
    }
}
