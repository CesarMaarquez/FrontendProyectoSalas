using System.Text;
using AppSalas.View;
using AppSalas.ViewModel;

namespace AppSalas.View
{
    public partial class ForgotPassword : ContentPage
    {

        private ForgotPasswordViewModel viewModel;
        public ForgotPassword()
        {
            InitializeComponent();


            viewModel = new ForgotPasswordViewModel();

            //  2. Te suscribes al evento
            viewModel.OnNavigateToVerificationPage += OnNavigateToVerificationPage;


            //  3. Lo conectas al BindingContext
            BindingContext = viewModel;
        }

        private async void OnClickedCreateAcc(object sender, EventArgs e)
        {
            //	await Navigation.PushAsync(new Register());
        }

        private async void OnNavigateToVerificationPage(string email, string username, string password, string dif)
        {

            await Shell.Current.GoToAsync($"//EmailVerificationPage?email={email}&password=&phoneNumber=&dif={dif}");

        }

    }
}

