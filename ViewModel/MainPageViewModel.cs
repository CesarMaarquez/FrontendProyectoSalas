using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Newtonsoft.Json;
using AppSalas.Model;

namespace AppSalas.ViewModel
{
    // VIEWMODEL DE LA PÁGINA PRINCIPAL, IMPLEMENTA INotifyPropertyChanged PARA ACTUALIZAR LA INTERFAZ
    public class MainPageViewModel : INotifyPropertyChanged
    {
        // LISTA DE PAÍSES DISPONIBLES PARA SELECCIÓN
        public List<Country> Countries { get; set; } = new();
        private Country _selectedCountry;

        // PROPIEDAD QUE REPRESENTA EL PAÍS SELECCIONADO
        public Country SelectedCountry
        {
            get => _selectedCountry;
            set
            {
                if (_selectedCountry != value)
                {
                    _selectedCountry = value;
                    OnPropertyChanged(); // NOTIFICAMOS CAMBIO EN LA PROPIEDAD
                    SetLanguage(value); // AJUSTAMOS EL IDIOMA SEGÚN EL PAÍS SELECCIONADO
                }
            }
        }

        // COMANDOS PARA ACCIONES DE REGISTRO Y SELECCIÓN DE PAÍS
        public ICommand SignUpCommand { get; }

        public ICommand LogInCommand { get; }

        public ICommand TermsCommand { get; }
        public ICommand SelectCountryCommand { get; }

        // CADENAS LOCALIZADAS PARA MOSTRAR TEXTOS EN LA INTERFAZ
        public string LoginText { get; set; } = string.Empty;
        public string SignupText { get; set; } = string.Empty;
        public string TitleText { get; set; } = string.Empty;
        public string ContinueWithText { get; set; } = string.Empty;
        public string DescriptionText { get; set; } = string.Empty;

        // CONSTRUCTOR DEL VIEWMODEL
        public MainPageViewModel()
        {
            LoadCountriesFromJson(); // CARGAMOS LOS DATOS DE PAÍSES DESDE UN ARCHIVO JSON

            // SELECCIONAMOS POR DEFECTO "SPAIN" O, SI NO, EL PRIMERO DE LA LISTA
            _selectedCountry = Countries.Find(c => c.Name == "Spain") ?? Countries.FirstOrDefault() ?? new Country { Name = "Spain", FlagEmoji = "🇪🇸" };
            SelectedCountry = _selectedCountry;

            SetLanguage(SelectedCountry); // CONFIGURAMOS EL IDIOMA INICIAL SEGÚN EL PAÍS SELECCIONADO

            // DEFINIMOS EL COMANDO DE REGISTRO, QUE NAVEGA A LA PÁGINA DE REGISTRO
            SignUpCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("//RegisterPage");
            });

            TermsCommand = new Command(async () => await ShowTermsAsync());

            LogInCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("//Log_InPage");
            });

            // COMANDO PARA CAMBIAR EL PAÍS SELECCIONADO
            SelectCountryCommand = new Command<Country>(country =>
            {
                if (country != null)
                {
                    SelectedCountry = country;
                }
            });
        }

        // MÉTODO PARA CARGAR LOS PAÍSES DESDE EL ARCHIVO JSON INCRUSTADO EN LOS RECURSOS
        private void LoadCountriesFromJson()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(name => name.EndsWith("languages.json"));

                if (resourceName == null)
                {
                    Console.WriteLine("ERROR: NO SE ENCONTRÓ EL ARCHIVO LANGUAGES.JSON");
                    return;
                }

                using Stream? stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    Console.WriteLine("ERROR: NO SE PUDO CARGAR EL RECURSO LANGUAGES.JSON");
                    return;
                }

                using var reader = new StreamReader(stream);
                string json = reader.ReadToEnd();
                Countries = JsonConvert.DeserializeObject<List<Country>>(json) ?? new List<Country>();

                Console.WriteLine($"JSON CARGADO CORRECTAMENTE. {Countries.Count} PAÍSES ENCONTRADOS.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR CARGANDO JSON: {ex}");
            }
        }

        // MÉTODO PARA CONFIGURAR EL IDIOMA SEGÚN EL PAÍS SELECCIONADO
        private void SetLanguage(Country country)
        {
            try
            {
                // DETERMINAMOS EL CÓDIGO DE IDIOMA BASADO EN EL NOMBRE DEL PAÍS
                string languageCode = country?.Name switch
                {
                    "Spain" => "es",
                    "United States" => "en",
                    _ => "es"
                };

                // CONFIGURAMOS LA CULTURA PARA LA INTERFAZ
                CultureInfo culture = new(languageCode);
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;

                // RECUPERAMOS LAS CADENAS LOCALIZADAS UTILIZANDO EL RESOURCE MANAGER
                ResourceManager rm = new ResourceManager("AppSalas.Resources.Language.AppResources", Assembly.GetExecutingAssembly());

                LoginText = rm.GetString("LoginButton", culture) ?? "Iniciar sesión";
                SignupText = rm.GetString("SignupButton", culture) ?? "Registrarse";
                TitleText = rm.GetString("Title", culture) ?? "Bienvenido";
                ContinueWithText = rm.GetString("ContinueWith", culture) ?? "Continuar con";
                DescriptionText = rm.GetString("Description", culture) ?? "Tu viaje comienza aquí";

                // NOTIFICAMOS A LA INTERFAZ QUE LAS PROPIEDADES HAN CAMBIADO
                OnPropertyChanged(nameof(LoginText));
                OnPropertyChanged(nameof(SignupText));
                OnPropertyChanged(nameof(TitleText));
                OnPropertyChanged(nameof(ContinueWithText));
                OnPropertyChanged(nameof(DescriptionText));

                Console.WriteLine($"IDIOMA CAMBIADO A {languageCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR CARGANDO IDIOMA: {ex}");
            }
        }
        private async Task ShowTermsAsync()
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("TermsAndConditions.md");
                using var reader = new StreamReader(stream);
                var md = await reader.ReadToEndAsync();
                await Application.Current.MainPage.DisplayAlert("Términos y Condiciones", md, "Cerrar");
            }
            catch
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudieron cargar los términos.", "OK");
            }
        }

        // IMPLEMENTACIÓN DEL EVENTO PROPERTYCHANGED PARA ACTUALIZAR LA INTERFAZ
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
