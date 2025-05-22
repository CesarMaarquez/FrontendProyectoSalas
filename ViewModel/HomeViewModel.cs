using System.ComponentModel;
using System.Windows.Input;

namespace AppSalas.ViewModel
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        // Comandos
        public ICommand NavigateCommand { get; }
        public ICommand LogoutCommand { get; }

        // Evento para INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        // Constructor
        public HomeViewModel()
        {
            NavigateCommand = new Command<string>(NavigateTo);
            LogoutCommand = new Command(Logout);
        }

        // Método de navegación
        private async void NavigateTo(string page)
        {
            string targetRoute = page switch
            {
                "Calendar" => "//CalendarPage",
                _ => "//HomePage"
            };

            await Shell.Current.GoToAsync(targetRoute);
        }

        // Método de logout
        private void Logout()
        {
            // Lógica de logout (por ejemplo, limpiar datos de sesión, redirigir a la página de login)
        }

        // Método para disparar el evento PropertyChanged
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
