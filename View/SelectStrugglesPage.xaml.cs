using AppSalas.Model;
using AppSalas.ViewModel;


namespace AppSalas.View
{
    public partial class SelectStrugglesPage : ContentPage
    {
        public SelectStrugglesPage(UserInfoModel UserInfo)
        {
            InitializeComponent();
            var viewModel = new SelectStrugglesViewModel(UserInfo);

            // 🔗 Suscribimos al evento para reaccionar cuando se presione "Submit"
            viewModel.OnNavigateToDisponibilityPage += NavigateToDisponibilityPage;

            BindingContext = viewModel;
        }

        private async void NavigateToDisponibilityPage(UserInfoModel userInfo)
        {
            await Navigation.PushAsync(new DisponibilityPage(userInfo));
        }


    }
}
