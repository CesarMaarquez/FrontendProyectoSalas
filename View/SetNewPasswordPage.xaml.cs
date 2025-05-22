using System.Runtime.CompilerServices;
using Newtonsoft.Json;
//using static Java.Util.Jar.Attributes;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using Microsoft.Maui.Devices;
using AppSalas.ViewModel;

namespace AppSalas.View
{



public partial class SetNewPasswordPage : ContentPage


    
{
    UsuarioModel user;

   
    public SetNewPasswordPage(UsuarioModel user)
	{
		InitializeComponent();
        this.user= user;

            Shell.SetBackButtonBehavior(this, new BackButtonBehavior
            {
                IsVisible = false,
                IsEnabled = false
            });


            BindingContext = new SetNewPasswordViewModel(user);

        
	}

    private async void Ontapped(object sender, EventArgs e)
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("TermsAndConditions.md"); //cuando nos pase el txt
            using var reader = new StreamReader(stream);
            string Terminos = await reader.ReadToEndAsync();


            await DisplayAlert("Terms and Conditions",
        Terminos,
         "Close");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar los términos: {ex.Message}");
            await DisplayAlert("Error", "No se pudieron cargar los términos y condiciones,intentelo mas tarde", "OK");
        }


    }


    private async void OnClickedAlrAcc(object sender, EventArgs e)
    {
     //   await Navigation.PushAsync(new Log_in());
    }

   
   

    





    }
}


