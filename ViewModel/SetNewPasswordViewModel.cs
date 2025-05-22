using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using AppSalas.Model;
using Newtonsoft.Json;


namespace AppSalas.ViewModel
{
    public class SetNewPasswordViewModel
    {

        UsuarioModel user;

        public ICommand OnSignInCommand { get; }
        public ICommand BackCommand { get; }


        public SetNewPasswordViewModel(UsuarioModel user)
        {
            this.user = user;
            OnSignInCommand = new Command(OnSignIn);
            BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));

        }

        public string Password { get; set; }
        public string Confirm { get; set; }



        private async void OnSignIn()
        {

            using var client = new HttpClient();


            if (Confirm != Password)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Passwords do not match.", "OK");
                return;

            }




            var updatedUser = new
            {
                userid = user.userid,
                email = user.email,
                password = Password, // nueva contraseña
                phoneNumber = user.phoneNumber,
                registrationDate = user.registrationDate
            };

            var urlput = GetUpdateUrl();

            urlput = urlput + user.userid;

            string updatedUserJson = JsonConvert.SerializeObject(updatedUser);
            var putContent = new StringContent(updatedUserJson, Encoding.UTF8, "application/json");

            var putrequest = await client.PutAsync(urlput, putContent);

            if (putrequest.IsSuccessStatusCode)
            {
                Application.Current.MainPage.DisplayAlert("Success", "Password updated!", "OK");
                await Shell.Current.GoToAsync("//MainPage");

            }
            else
            {
                var putError = await putrequest.Content.ReadAsStringAsync();
                Application.Current.MainPage.DisplayAlert("Error", $"Failed to update: {putError}", "OK");


            }

        }

        private string GetUpdateUrl()
        {
            return DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:5050/api/users/"
                : "http://192.168.128.1:5050/api/users/";
        }
    }
}