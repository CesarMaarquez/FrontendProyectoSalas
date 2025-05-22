
using AppSalas.Model;
using AppSalas.ViewModel;

namespace AppSalas.View
{
    [QueryProperty(nameof(UserType), "userType")]
    [QueryProperty(nameof(UserIdString), "userId")]

    public partial class PersonalInfoPage : ContentPage, IQueryAttributable
    {
        private PersonalInfoViewModel viewModel;



        private string userType;
        private int userId;

        // Propiedad UserType
        public string UserType
        {
            set
            {
                userType = value;
                TryInitializeViewModel();
            }
        }

        // Propiedad UserId
        public string UserIdString
        {
            set
            {
                System.Diagnostics.Debug.WriteLine($"[PersonalInfoPage] LLEGÓ userId (string): {value}");

                if (int.TryParse(value, out int id))
                {
                    userId = id;
                    TryInitializeViewModel();
                }
            }
        }

        // Método para inicializar el ViewModel solo cuando tengamos ambos parámetros
        private void TryInitializeViewModel()
        {
            if (!string.IsNullOrEmpty(userType) && userId != 0 && viewModel == null)
            {
                // Aquí se pasan tanto userType como userId al ViewModel
                viewModel = new PersonalInfoViewModel(userType, userId);
                viewModel.OnNavigateToEducationalPage += NavigateToEducationalInfoPage;
                viewModel.OnNavigateToStrugglesPage += OnNavigateToStrugglesPage;
                BindingContext = viewModel;
            }
        }

        public PersonalInfoPage()
        {
            InitializeComponent();

        }
        private async void NavigateToEducationalInfoPage(UserInfoModel userInfo)
        {
            await Navigation.PushAsync(new EducationalInfoPage(userInfo));
        }

        private async void OnNavigateToStrugglesPage(UserInfoModel userInfo)
        {
            await Navigation.PushAsync(new SelectStrugglesPage(userInfo));
        }

        // Método que recibe los parámetros de navegación
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("specialty", out object specialtyValue))
            {
                string specialty = specialtyValue?.ToString();
                if (BindingContext is PersonalInfoViewModel viewModel)
                {
                    // Oculta los campos profesionales si "specialty" es "cliente"
                    viewModel.ShowProfessionalFields = !string.Equals(specialty, "cliente", StringComparison.OrdinalIgnoreCase);
                }
            }
        }
    }
}
