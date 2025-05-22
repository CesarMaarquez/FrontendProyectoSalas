using AppSalas.Model;
using AppSalas.ViewModel;


namespace AppSalas.View
{
    public partial class SelectSpecialtiesPage : ContentPage
    {
        public SelectSpecialtiesPage(UserInfoModel UserInfo)
        {
            InitializeComponent();
            var viewModel = new SelectSpecialtiesViewModel(UserInfo);

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
