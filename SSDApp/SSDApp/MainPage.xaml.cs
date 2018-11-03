using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SSDApp.Views;

namespace SSDApp
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

        private async void CreateClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreatePage() );
        }

        private async void ReadClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ReadPage());
        }

        private async void UpdateClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListViewForUpdatePage());
        }

        private async void DeleteClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DeletePage());
        }

        
    }
}
