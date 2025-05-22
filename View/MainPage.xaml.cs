using AppSalas.ViewModel;

namespace AppSalas.View
{
    // CONSTRUCTOR DE LA PÁGINA PRINCIPAL
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent(); // INICIALIZAMOS LOS COMPONENTES DE LA INTERFAZ
            BindingContext = new MainPageViewModel(); // ASIGNAMOS EL VIEWMODEL COMO CONTEXTO DE DATOS
        }

    }
}
