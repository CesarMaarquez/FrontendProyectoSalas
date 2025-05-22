using AppSalas.ViewModel;

namespace AppSalas.View
{
    // PÁGINA DE REGISTRO: INICIALIZA LOS COMPONENTES Y ASIGNA EL VIEWMODEL DE REGISTRO COMO CONTEXTO DE DATOS
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent(); // INICIALIZAMOS LOS COMPONENTES DE LA INTERFAZ
            BindingContext = new RegisterViewModel(); // SETEAMOS EL VIEWMODEL DE REGISTRO
        }
    }
}
