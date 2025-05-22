using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using AppSalas.Model;
using CommunityToolkit.Maui.Alerts;
using Newtonsoft.Json;

namespace AppSalas.ViewModel
{
    public class EmailVerificationViewModel : INotifyPropertyChanged
    {
        private readonly string email;
        private readonly string password;
        private readonly string phoneNumber;
        private readonly string dif;

        public string Code1 { get => _code1; set { _code1 = value; OnPropertyChanged(nameof(Code1)); } }
        public string Code2 { get => _code2; set { _code2 = value; OnPropertyChanged(nameof(Code2)); } }
        public string Code3 { get => _code3; set { _code3 = value; OnPropertyChanged(nameof(Code3)); } }
        public string Code4 { get => _code4; set { _code4 = value; OnPropertyChanged(nameof(Code4)); } }
        public string Code5 { get => _code5; set { _code5 = value; OnPropertyChanged(nameof(Code5)); } }

        private string _code1, _code2, _code3, _code4, _code5;

        public ICommand VerifyCommand { get; }
        public ICommand BackCommand { get; }

        public ICommand ReSendCommand { get; }


        public event Action<UsuarioModel> OnNavigateToSetPasswordPage;

        public EmailVerificationViewModel(
            string email,
            string password,
            string phoneNumber,
            string dif)
        {
            this.email = email;
            this.password = password;
            this.phoneNumber = phoneNumber;
            this.dif = dif;

            Debug.WriteLine($"[DEBUG] Email recibido en EmailVerificationViewModel: {email}");

            VerifyCommand = new Command(async () => await VerifyCodeAsync());
            ReSendCommand = new Command(async () => await ReSendAsync());
            BackCommand = new Command(async () => await Shell.Current.GoToAsync("//RegisterPage"));

        }
        private async Task VerifyCodeAsync()
        {
            string code = $"{Code1}{Code2}{Code3}{Code4}{Code5}";
            if (code.Length != 5)
            {
                await Toast.Make("Introduce un código de 5 dígitos").Show();
                return;
            }

            var model = new { email = email, code = code };
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            HttpResponseMessage verifyResponse;
            try
            {
                verifyResponse = await client.PostAsync(GetVerifyUrl(), content);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Exception", ex.Message, "OK");
                return;
            }

            if (!verifyResponse.IsSuccessStatusCode)
            {
                var error = await verifyResponse.Content.ReadAsStringAsync();
                await Application.Current.MainPage.DisplayAlert("Error", $"Code verification failed: {error}", "OK");
                return;
            }

            await Application.Current.MainPage.DisplayAlert("Verificado", "Verificado correctamente", "OK");

            // Validar de forma robusta si es "registro"
            if (dif?.Trim().ToLower() == "registro")
            {
                var user = new
                {
                    email = email,
                    password = password,
                    phoneNumber = phoneNumber
                };
                var userJson = JsonConvert.SerializeObject(user);
                var userContent = new StringContent(userJson, Encoding.UTF8, "application/json");

                var regResponse = await client.PostAsync(GetRegisterUrl(), userContent);
                var regBody = await regResponse.Content.ReadAsStringAsync();
                Debug.WriteLine($"[Registro] {regResponse.StatusCode} — {regBody}");

                if (!regResponse.IsSuccessStatusCode)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Failed to register user: {regBody}", "OK");
                    return;
                }
                // Aquí puedes manejar la respuesta del registro si es necesario
                await Application.Current.MainPage.DisplayAlert("Registro", "Usuario registrado correctamente", "OK");

                var registeredUser = JsonConvert.DeserializeObject<ObtainedUser>(regBody);
                int userId = registeredUser.UserId;

                Debug.WriteLine("EL USER ID ES :" + userId);
                // Ir a MainPage después del proceso
                await Shell.Current.GoToAsync($"///SelectSpecialtyPage?userId={userId}");
            }
            else if (dif?.Trim().ToLower() == "cambio")
            {

                var urlget = GetUserUrl();
                urlget = urlget + email;

                var userResponse = await client.GetAsync(urlget);

                Debug.WriteLine("la response es :" + userResponse.ToString());

                //setnewspassowrd

                if (userResponse.IsSuccessStatusCode)
                {

                    var userJson = await userResponse.Content.ReadAsStringAsync();
                    var userObject = JsonConvert.DeserializeObject<UsuarioModel>(userJson);

                    OnNavigateToSetPasswordPage?.Invoke(userObject);

                    // Ahora puedes usar userObject.Id, userObject.Email, etc.
                }
                else
                {
                    var error = userResponse.Content.ToString();
                    await Application.Current.MainPage.DisplayAlert("Error", error, "OK");
                }
            }
        }
        private async Task ReSendAsync()
        {
            try
            {
                using var client = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(20) // Aumentar timeout por si tarda
                };

                string url = GetSendUrl();
                var payload = JsonConvert.SerializeObject(email);
                Debug.WriteLine("EL CORREO ES:" + email);
                Console.WriteLine($"Payload: {payload}");

                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var resp = await client.PostAsync(url, content);

                if (resp.IsSuccessStatusCode)
                {
                    await Application.Current.MainPage.DisplayAlert("Éxito", "Código enviado de nuevo", "OK");

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

        private string GetSendUrl() =>
            DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:5050/api/mail/send-verification"
                : " http://192.168.128.1:5050/api/mail/send-verification";

        private string GetVerifyUrl() =>
            DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:5050/api/mail/verify-code"
                : "http://192.168.128.1:5050/api/mail/verify-code";

        private string GetUserUrl()
        {
            return DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:5050/api/users/mail/"
                : "http://192.168.128.1:5050/api/users/mail/";
        }

        private string GetRegisterUrl() =>
            DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:5050/api/users"
                : "http://192.168.128.1:5050/api/users";

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
