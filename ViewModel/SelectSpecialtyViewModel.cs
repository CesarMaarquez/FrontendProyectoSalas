using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AppSalas.ViewModel
{
    public partial class SelectSpecialtyViewModel : INotifyPropertyChanged
    {
        private bool isPatientSelected;
        public bool IsPatientSelected
        {
            get => isPatientSelected;
            set
            {
                if (isPatientSelected != value)
                {
                    isPatientSelected = value;
                    // Al seleccionar Patient (que interpretamos como "cliente"), se deselecciona Therapist.
                    if (isPatientSelected)
                        IsTherapistSelected = false;
                    OnPropertyChanged();
                }
            }
        }

        private bool isTherapistSelected;
        public bool IsTherapistSelected
        {
            get => isTherapistSelected;
            set
            {
                if (isTherapistSelected != value)
                {
                    isTherapistSelected = value;
                    // Al seleccionar Therapist, se deselecciona Patient.
                    if (isTherapistSelected)
                        IsPatientSelected = false;
                    OnPropertyChanged();
                }
            }
        }

        // Comandos para seleccionar cada opción.
        public ICommand SelectPatientCommand { get; }
        public ICommand SelectTherapistCommand { get; }

        // Comando para hacer submit y navegar a la siguiente pantalla.
        public ICommand SubmitCommand { get; }
        private readonly int userId;
        public SelectSpecialtyViewModel(int userId)
        {
            SelectPatientCommand = new Command(() => { IsPatientSelected = true; });
            SelectTherapistCommand = new Command(() => { IsTherapistSelected = true; });

            // En ExecuteSubmitCommand pasamos el parámetro "specialty" a la siguiente página.
            SubmitCommand = new Command(async () => await ExecuteSubmitCommand());
            this.userId = userId;
        }

        private async Task ExecuteSubmitCommand()
        {
            string specialty = string.Empty;

            if (IsPatientSelected)
                specialty = "cliente";
            else if (IsTherapistSelected)
                specialty = "therapist";

            await Shell.Current.GoToAsync("///PersonalInfoPage", new Dictionary<string, object>
{
    { "userType", specialty },
    { "userId", userId.ToString() } // ← ¡muy importante pasarlo como string!
});

        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
