using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using System.Resources;
using AppSalas.Model;
using AppSalas.Services;
using System.Text.RegularExpressions;
using PhoneNumbers;
using CommunityToolkit.Maui.Alerts;

namespace AppSalas.ViewModel
{
    public partial class RegisterViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Country> Countries { get; } = new();

        private Country? _selectedCountry;

        private readonly PhoneNumberUtil _phoneUtil = PhoneNumberUtil.GetInstance();

        public Country? SelectedCountry
        {
            get => _selectedCountry;
            set
            {
                if (_selectedCountry != value)
                {
                    _selectedCountry = value;
                    OnPropertyChanged();
                    DialCode = _selectedCountry?.DialCode ?? string.Empty;
                    UpdateCanRegister();
                }
            }
        }

        private string _dialCode = string.Empty;
        public string DialCode
        {
            get => _dialCode;
            set { _dialCode = value; OnPropertyChanged(); }
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); UpdateCanRegister(); }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); UpdateCanRegister(); }
        }

        private string _confirmPassword = string.Empty;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set { _confirmPassword = value; OnPropertyChanged(); UpdateCanRegister(); }
        }

        private string _phoneNumber = string.Empty;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set { _phoneNumber = value; OnPropertyChanged(); UpdateCanRegister(); }
        }

        private bool _termsAccepted;
        public bool TermsAccepted
        {
            get => _termsAccepted;
            set { _termsAccepted = value; OnPropertyChanged(); UpdateCanRegister(); }
        }

        public string CreateAccount { get; private set; } = string.Empty;
        public string AccountDescription { get; private set; } = string.Empty;
        public string EmailPlaceholder { get; private set; } = string.Empty;
        public string PasswordPlaceholder { get; private set; } = string.Empty;
        public string ConfirmPasswordPlaceholder { get; private set; } = string.Empty;
        public string PhonePlaceholder { get; private set; } = string.Empty;
        public string TermsLabel { get; private set; } = string.Empty;
        public string SignupButton { get; private set; } = string.Empty;
        public string AlreadyAccount { get; private set; } = string.Empty;
        public string SelectCountryCode => "Seleccione país";

        private bool _canRegister;
        public bool CanRegister
        {
            get => _canRegister;
            set { _canRegister = value; OnPropertyChanged(); }
        }

        public ICommand BackCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand TermsCommand { get; }
        public ICommand NavigateToLoginCommand { get; }

        public RegisterViewModel()
        {
            BackCommand = new Command(async () => await Shell.Current.GoToAsync("//MainPage"));
            RegisterCommand = new Command(async () => await ExecuteRegisterAsync());
            TermsCommand = new Command(async () => await ShowTermsAsync());
            NavigateToLoginCommand = new Command(async () => await Shell.Current.GoToAsync("//Log_InPage"));

            UpdateLocalizedTexts();
            LocalizationService.LanguageChanged += (s, e) => UpdateLocalizedTexts();

            LoadCountriesFromJson();
        }


        private void LoadCountriesFromJson()
        {
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                var resourceName = asm.GetManifestResourceNames()
                    .FirstOrDefault(n => n.EndsWith("languages.json", StringComparison.OrdinalIgnoreCase));

                if (resourceName is null)
                {
                    Console.WriteLine("No se encontró el recurso languages.json");
                    return;
                }

                using var stream = asm.GetManifestResourceStream(resourceName);
                if (stream is null)
                {
                    Console.WriteLine("No se pudo obtener el stream del recurso.");
                    return;
                }

                using var reader = new StreamReader(stream);
                var json = reader.ReadToEnd();

                var list = JsonConvert.DeserializeObject<Country[]>(json) ?? Array.Empty<Country>();

                Countries.Clear();
                foreach (var c in list)
                    Countries.Add(c);

                SelectedCountry = Countries.FirstOrDefault(c => c.Name == "Spain")
                                  ?? Countries.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cargando países embebidos: {ex.Message}");
            }
        }

        private void UpdateLocalizedTexts()
        {
            var rm = new ResourceManager(
                "AppSalas.Resources.Language.AppResources",
                Assembly.GetExecutingAssembly());

            CreateAccount = rm.GetString("CreateAccount") ?? string.Empty;
            AccountDescription = rm.GetString("AccountDescription") ?? string.Empty;
            EmailPlaceholder = rm.GetString("EmailPlaceholder") ?? string.Empty;
            PasswordPlaceholder = rm.GetString("PasswordPlaceholder") ?? string.Empty;
            ConfirmPasswordPlaceholder = rm.GetString("ConfirmPasswordPlaceholder") ?? string.Empty;
            PhonePlaceholder = rm.GetString("PhonePlaceholder") ?? string.Empty;
            TermsLabel = rm.GetString("TermsLabel") ?? string.Empty;
            SignupButton = rm.GetString("SignupButton") ?? string.Empty;
            AlreadyAccount = rm.GetString("AlreadyAccount") ?? string.Empty;

            OnPropertyChanged(nameof(CreateAccount));
            OnPropertyChanged(nameof(AccountDescription));
            OnPropertyChanged(nameof(EmailPlaceholder));
            OnPropertyChanged(nameof(PasswordPlaceholder));
            OnPropertyChanged(nameof(ConfirmPasswordPlaceholder));
            OnPropertyChanged(nameof(PhonePlaceholder));
            OnPropertyChanged(nameof(TermsLabel));
            OnPropertyChanged(nameof(SignupButton));
            OnPropertyChanged(nameof(AlreadyAccount));
        }

        private void UpdateCanRegister()
        {
            CanRegister =
                !string.IsNullOrWhiteSpace(Email) &&
                !string.IsNullOrWhiteSpace(Password) &&
                Password == ConfirmPassword &&
                !string.IsNullOrWhiteSpace(PhoneNumber) &&
                SelectedCountry != null &&
                TermsAccepted;
        }

        private static string GetVerificationUrl()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
                return "http://10.0.2.2:5050/api/mail/send-verification";
            else
                return "http://192.168.1.112:5050/api/mail/send-verification";
        }
        public bool ValidarCorreo(string correo)
        {
            string patron = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(patron);
            return regex.IsMatch(correo);
        }

        public bool ValidarNumber(string number)
        {
            try
            {
                // Como ya tienes +34, no necesitas pasar region
                var parsedNumber = _phoneUtil.Parse(number, null);
                return _phoneUtil.IsValidNumber(parsedNumber);
            }
            catch (NumberParseException)
            {
                return false;
            }
        }


        private async Task ExecuteRegisterAsync()
        {
            UpdateCanRegister();
            if (!CanRegister) return;

            var fullPhone = $"{DialCode}{PhoneNumber}";
            var url = GetVerificationUrl();

            Console.WriteLine($"POST a {url} con Email: {Email}");

            try
            {
                using var client = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(20) // Aumentar timeout por si tarda
                };



                if (Password.Length < 8)
                {
                    await Toast.Make("La contraseña debe tener almenos 8 carácteres").Show();
                    return;
                }

                if (!ValidarCorreo(Email))
                {
                    await Toast.Make("El correo no es válido").Show();
                    return;
                }

                if (!ValidarNumber(fullPhone))
                {
                    await Toast.Make("El número de teléfono no es válido.").Show();
                    return;
                }

                var payload = JsonConvert.SerializeObject(Email);
                Console.WriteLine($"Payload: {payload}");

                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var resp = await client.PostAsync(url, content);

                if (resp.IsSuccessStatusCode)
                {
                    await Application.Current.MainPage.DisplayAlert("Éxito", "Código enviado", "OK");
                    var route = $"//EmailVerificationPage?email={Email}&password={Password}&phoneNumber={fullPhone}&dif=registro";
                    await Shell.Current.GoToAsync(route);
                }
                else
                {
                    var err = await resp.Content.ReadAsStringAsync();
                    Console.WriteLine($"Respuesta del servidor: {err}");
                    await Application.Current.MainPage.DisplayAlert("Error del servidor", err, "OK");
                }
            }
            catch (HttpRequestException httpEx)
            {
                var details = new StringBuilder();
                details.AppendLine("Error al conectar con el servidor.");
                details.AppendLine($"Mensaje: {httpEx.Message}");

                if (httpEx.InnerException != null)
                {
                    details.AppendLine("Detalles internos:");
                    details.AppendLine(httpEx.InnerException.Message);
                }

                Console.WriteLine(details.ToString());
                await Application.Current.MainPage.DisplayAlert("HttpRequestException", details.ToString(), "OK");
            }
            catch (TaskCanceledException)
            {
                var msg = "La solicitud ha tardado demasiado en responder. Verifica tu conexión o la IP del servidor.";
                Console.WriteLine(msg);
                await Application.Current.MainPage.DisplayAlert("Timeout", msg, "OK");
            }
            catch (Exception ex)
            {
                var msg = $"Excepción inesperada:\n{ex.Message}\n{ex.StackTrace}";
                Console.WriteLine(msg);
                await Application.Current.MainPage.DisplayAlert("Excepción inesperada", msg, "OK");
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

        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
