using System.Collections.Generic;
using Microsoft.Maui.Controls;
using AppSalas.ViewModel;
using AppSalas.Model;

namespace AppSalas.View
{
    public partial class EducationalInfoPage : ContentPage, IQueryAttributable
    {
        private EducationalInfoViewModel viewModel;
        public EducationalInfoPage(UserInfoModel userinfo)
        {
            InitializeComponent();
           
            //  1. Guardas el ViewModel en una variable
            viewModel = new EducationalInfoViewModel(userinfo);

            //  2. Te suscribes al evento
            viewModel.OnNavigateToSpecialitiesPage += NavigateToSpecialtiesPage;

            viewModel.OnNavigateToStrugglesPage += NavigateToStrugglesPage;

            //  3. Lo conectas al BindingContext
            BindingContext = viewModel;

        }

        private async void NavigateToSpecialtiesPage(UserInfoModel userInfo)
        {
            // Aquí navegas y pasas el modelo a la siguiente página
            await Navigation.PushAsync(new SelectSpecialtiesPage(userInfo));
        }

        private async void NavigateToStrugglesPage(UserInfoModel userInfo)
        {
            // Aquí navegas y pasas el modelo a la siguiente página
            await Navigation.PushAsync(new SelectStrugglesPage(userInfo));
        }


        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            // Aquí puedes manejar parámetros entrantes si fuese necesario.
        }

    }
}
