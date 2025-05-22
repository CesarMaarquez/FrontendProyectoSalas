using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using AppSalas.Model;
using Newtonsoft.Json;


namespace AppSalas.ViewModel
{
    public class Log_InViewModel
    {
        private string _email;
        private string _password;

        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        public ICommand OnClickedForgot { get; }
        public ICommand OnClickedCreateAcc { get; }
        public ICommand BackCommand { get; }
        public ICommand SignInCommand { get; }

        public Log_InViewModel()
        {
            OnClickedForgot = new Command(async () => await Shell.Current.GoToAsync("//ForgotPasswordPage"));
            OnClickedCreateAcc = new Command(async () => await Shell.Current.GoToAsync("//RegisterPage"));
            BackCommand = new Command(async () => await Shell.Current.GoToAsync("//MainPage"));
            SignInCommand = new Command(async () => await SignIn());
        }

        private async Task SignIn()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Please enter both email and password.", "OK");
                return;
            }

            try
            {
                // Aquí iría la lógica real para autenticar, por ejemplo con un API.
                // Simulamos éxito:
                Debug.WriteLine($"Attempting login with Email: {Email} and Password: {Password}");
                var user = new UserModel
                {
                    username = Email,
                    password = Password
                };

                var json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {


                    var response = await httpClient.PostAsync("http://10.0.2.2:5050/api/auth/login", content);
                    if (response.IsSuccessStatusCode)
                    {




                        var responseContent = await response.Content.ReadAsStringAsync();

                        // Deserializa para obtener solo el token
                        var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

                        // Guarda solo el valor del token
                        await SecureStorage.SetAsync("auth_token", tokenResponse.Token);
                        Debug.WriteLine($"Login successful: {responseContent}");

                        // Navegar a la página principal o hacer algo con el token
                        await App.Current.MainPage.DisplayAlert("Success", "Login successful!", "OK");
                        await Shell.Current.GoToAsync("//HomePage");
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Debug.WriteLine($"Login failed: {errorContent}");
                        await App.Current.MainPage.DisplayAlert("Error", "Invalid email or password.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Login failed: {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Error", "Login failed. Please try again.", "OK");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class TokenResponse
        {
            public string Token { get; set; }
        }
    }
}