
using AppSalas.View;
using AppSalas.ViewModel;

namespace AppSalas.View
{
    public partial class EmailVerificationPage : ContentPage, IQueryAttributable
    {

        public EmailVerificationPage()
        {
            InitializeComponent();
        }

        private EmailVerificationViewModel viewModel;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            query.TryGetValue("email", out var emailObj);
            query.TryGetValue("password", out var passwordObj);
            query.TryGetValue("phoneNumber", out var phoneObj);
            query.TryGetValue("dif", out var difObj);

            var email = emailObj?.ToString() ?? "";
            var password = passwordObj?.ToString() ?? "";
            var phoneNumber = phoneObj?.ToString() ?? "";
            var dif = difObj?.ToString() ?? "";

            viewModel = new EmailVerificationViewModel(email, password, phoneNumber, dif);
            viewModel.OnNavigateToSetPasswordPage += OnNavigateToSetPasswordPage;
            BindingContext = viewModel;
        }
        private void OnNavigateToSetPasswordPage(UsuarioModel user)
        {
            {
                Navigation.PushAsync(new SetNewPasswordPage(user));
            }

        }
    }
}
