using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AppSalas.Model;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Maui.Alerts;

namespace AppSalas.ViewModel
{
    public partial class PersonalInfoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string name = string.Empty;
        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool showProfessionalFields = true;
        public bool ShowProfessionalFields
        {
            get => showProfessionalFields;
            set
            {
                if (showProfessionalFields != value)
                {
                    showProfessionalFields = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SaveInformationCommand { get; }
        public ICommand UploadCVCommand { get; }
        public ICommand BackCommand { get; }

        public PersonalInfoViewModel(string usertype, int userId)
        {
            UserType = usertype;
            this.userId = userId;
            ShowProfessionalFields = UserType == "therapist";

            SaveInformationCommand = new Command(async () => await OnSaveInformation());
            UploadCVCommand = new Command(OnUploadCV);
            BackCommand = new Command(async () => await Shell.Current.GoToAsync("//SelectSpecialtyPage"));
            CargarPaisesYEstadosDesdeJson();
        }

        public event Action<UserInfoModel> OnNavigateToEducationalPage;
        public event Action<UserInfoModel> OnNavigateToStrugglesPage;

        public int userId { get; set; }
        public string UserType { get; set; }

        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Dni { get; set; }

        // Cambiados a string para que Entry funcione correctamente
        private string edad = string.Empty;
        public string Edad
        {
            get => edad;
            set
            {
                if (edad != value)
                {
                    edad = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Genero { get; set; }
        //public string Pais { get; set; }
        //public string Estado { get; set; }
        public string Direccion { get; set; }

        private string aniosExperiencia = string.Empty;
        public string AniosExperiencia
        {
            get => aniosExperiencia;
            set
            {
                if (aniosExperiencia != value)
                {
                    aniosExperiencia = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Especialidad { get; set; }

        private async Task OnSaveInformation()
        {
            if (UserType == "therapist")
            {
                if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Apellidos) ||
                string.IsNullOrWhiteSpace(Dni) || string.IsNullOrWhiteSpace(Edad) ||
                string.IsNullOrWhiteSpace(Genero) || string.IsNullOrWhiteSpace(Pais) ||
                string.IsNullOrWhiteSpace(Estado) || string.IsNullOrWhiteSpace(Direccion) || string.IsNullOrWhiteSpace(aniosExperiencia))
                {
                    await Toast.Make("Completa todos los campos").Show();
                    return;
                }

            }
            else
            {
                if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Apellidos) ||
                string.IsNullOrWhiteSpace(Dni) || string.IsNullOrWhiteSpace(Edad) ||
                string.IsNullOrWhiteSpace(Genero) || string.IsNullOrWhiteSpace(Pais) ||
                string.IsNullOrWhiteSpace(Estado) || string.IsNullOrWhiteSpace(Direccion))
                {
                    await Toast.Make("Completa todos los campos").Show();
                    return;
                }

            }

            var userInfo = new UserInfoModel
            {
                firstName = Nombre,
                lastName = Apellidos,
                dni = Dni,
                age = int.TryParse(Edad, out var edadInt) ? edadInt : 0,
                gender = Genero,
                country = Pais,
                state = Estado,
                address = Direccion,
                AniosExperiencia = int.TryParse(AniosExperiencia, out var expInt) ? expInt : 0,
                Especialidad = Especialidad,
                UserType = this.UserType,
                userId = this.userId
            };

            Debug.WriteLine($"USERID: {userInfo.userId} | Name: {userInfo.firstName}");

            if (userInfo.UserType == "therapist")
            {
                OnNavigateToEducationalPage?.Invoke(userInfo);
            }
            else
            {
                OnNavigateToStrugglesPage?.Invoke(userInfo);
            }
        }
        public ObservableCollection<string> Paises { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> Estados { get; set; } = new ObservableCollection<string>();

        private string paisSeleccionado;
        public string Pais
        {
            get => paisSeleccionado;
            set
            {
                if (paisSeleccionado != value)
                {
                    paisSeleccionado = value;
                    OnPropertyChanged();
                    ActualizarEstados(); // cada vez que cambie el país, actualiza estados
                }
            }
        }

        private string estadoSeleccionado;
        public string Estado
        {
            get => estadoSeleccionado;
            set
            {
                if (estadoSeleccionado != value)
                {
                    estadoSeleccionado = value;
                    OnPropertyChanged();
                }
            }
        }

        private Dictionary<string, List<string>> paisesConEstados = new();

        private async Task CargarPaisesYEstadosDesdeJson()
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("paises_ciudades.json");
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();

                paisesConEstados = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);

                Paises.Clear();
                foreach (var pais in paisesConEstados.Keys)
                    Paises.Add(pais);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al cargar JSON: {ex.Message}");
            }
        }

        private void ActualizarEstados()
        {
            Estados.Clear();
            if (paisesConEstados.TryGetValue(Pais, out var listaEstados))
            {
                foreach (var estado in listaEstados)
                    Estados.Add(estado);
            }
        }


        private void OnUploadCV()
        {
            // Lógica para subir CV
        }
    }
}
