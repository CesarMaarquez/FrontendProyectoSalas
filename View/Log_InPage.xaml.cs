using AppSalas.ViewModel;

namespace AppSalas.View;

public partial class Log_InPage : ContentPage
{
	public Log_InPage()
	{
		InitializeComponent();
        BindingContext = new Log_InViewModel();
    }


	private async void OnClickedCreateAcc(object sender, EventArgs e)
	{

       // await Navigation.PushAsync(new Register());
    }

    private async void OnclickedForgot(object sender, EventArgs e)
    {

      //  await Navigation.PushAsync(new ForgotPassword());
    }

    private async void OnSignInClicked(object sender, EventArgs e)
    {

      //  await Navigation.PushAsync(new Register());
    }

   


}