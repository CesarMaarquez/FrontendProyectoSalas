using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using AppSalas.Model;
using Newtonsoft.Json;


namespace AppSalas.ViewModel
{
    public class ForgotPasswordViewModel : INotifyPropertyChanged
    {
        public ICommand OnSendClickedCommand { get; }


        public ForgotPasswordViewModel()
        {
            OnSendClickedCommand = new Command(OnSendClicked);

        }


        public string email { get; set; }


        public event Action<string, string, string, string> OnNavigateToVerificationPage;
        public event PropertyChangedEventHandler? PropertyChanged;

        private async void OnSendClicked()
        {




            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var apiUrl = GetVerificationUrl(); // endpoint de enviar código
                    var content = new StringContent($"\"{email}\"", Encoding.UTF8, "application/json");

                    //Llamada a la Api

                    var response = await client.PostAsync(apiUrl, content);
                    string dif = "cambio";

                    if (response.IsSuccessStatusCode)
                    {
                        await Application.Current.MainPage.DisplayAlert("Éxito", "Se envió el codigo de verificación", "OK");


                        OnNavigateToVerificationPage?.Invoke(email, null, null, dif);

                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        await Application.Current.MainPage.DisplayAlert("Error", error, "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Excepción: {ex.Message}", "OK");
            }
        }

        // Este método lo usas para obtener la URL de verificación
        private static string GetVerificationUrl()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                return "http://10.0.2.2:5050/api/mail/send-verification";
            }
            else
            {
                return "http://192.168.128.1:5050/api/mail/send-verification";
            }
        }
    }
}