using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using AppSalas.Model;

namespace AppSalas.ViewModel
{
    // ViewModel para manejar la disponibilidad (fecha y horarios)
    public class DisponibilityViewModel : INotifyPropertyChanged
    {
        // Propiedad para la fecha seleccionada, inicializada a hoy.
        private DateTime selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get => selectedDate;
            set
            {
                if (selectedDate != value)
                {
                    selectedDate = value;
                    OnPropertyChanged();
                }
            }
        }

        // Propiedad para la hora de inicio; se simplifica la creación con target-typed new.
        private TimeSpan startTime = new(9, 0, 0);
        public TimeSpan StartTime
        {
            get => startTime;
            set
            {
                if (startTime != value)
                {
                    startTime = value;
                    OnPropertyChanged();
                }
            }
        }

        // Propiedad para la hora de término; se usa target-typed new.
        private TimeSpan endTime = new(17, 0, 0);
        public TimeSpan EndTime
        {
            get => endTime;
            set
            {
                if (endTime != value)
                {
                    endTime = value;
                    OnPropertyChanged();
                }
            }
        }

        // Comando para enviar o procesar la disponibilidad.
        public ICommand SubmitCommand { get; }
        public ICommand BackCommand { get; }
        public UserInfoModel UserInfo { get; set; }
        // Constructor: asigna el comando.
        public DisponibilityViewModel(UserInfoModel Userinfo)
        {
            SubmitCommand = new Command(async () => await OnSubmit());
            UserInfo = Userinfo;
            BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));

        }

        private List<Dictionary<string, int>> MapSpecialtiesToIds(List<string> specialties)
        {
            var map = new Dictionary<string, int>
            {
                ["Depression"] = 1,
                ["Anxiety"] = 2,
                ["Behavior Change"] = 3,
                ["Family Therapy"] = 4,
                ["Sleeping Difficulty"] = 5,
                ["PTSD"] = 6,
                ["Loneliness"] = 7,
                ["Retirement"] = 8,
                ["Companionship"] = 9,
                ["Declining Health"] = 10,
                ["Declining Mortality"] = 11
            };

            return specialties
                .Where(map.ContainsKey)
                .Select(name => new Dictionary<string, int> { ["specialtyId"] = map[name] })
                .ToList();
        }

        private List<Dictionary<string, int>> MapStrugglesToIds(List<string> struggles)
        {
            var map = new Dictionary<string, int>
            {
                ["Depression"] = 1,
                ["Anxiety"] = 2,
                ["Behavior Change"] = 3,
                ["Family Therapy"] = 4,
                ["Sleeping Difficulty"] = 5,
                ["PTSD"] = 6,
                ["Loneliness"] = 7,
                ["Retirement"] = 8,
                ["Companionship"] = 9,
                ["Declining Health"] = 10,
                ["Declining Mortality"] = 11
            };

            return struggles?
                .Where(map.ContainsKey)
                .Select(name => new Dictionary<string, int> { ["struggleId"] = map[name] })
                .ToList() ?? new List<Dictionary<string, int>>();
        }

        // Método que se invoca al ejecutar el comando Submit.
        private async Task OnSubmit()
        {

            UserInfo.StartTime = StartTime;

            UserInfo.EndTime = EndTime;

            object finalPayload;

            if (UserInfo.UserType == "therapist")
            {


                // var parsedDate = DateTime.ParseExact(UserInfo.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);


                finalPayload = new
                {
                    username = $"{UserInfo.firstName.ToLower()}_{UserInfo.lastName.ToLower()}",
                    firstName = UserInfo.firstName,
                    lastName = UserInfo.lastName,
                    gender = UserInfo.gender,
                    age = UserInfo.age,
                    dni = UserInfo.dni,
                    country = UserInfo.country,
                    state = UserInfo.state,
                    address = UserInfo.address,
                    startTime = UserInfo.StartTime,
                    endTime = UserInfo.EndTime,
                    additionalData = "Disponible para sesiones",
                    therapistProfile = new
                    {

                        experienceYears = UserInfo.AniosExperiencia,
                        license = "ESP-000000",
                        degree = UserInfo.Degree,
                        institution = UserInfo.Institution,
                        graduationYear = UserInfo.EndDate.Year,
                        specialties = MapSpecialtiesToIds(UserInfo.Specialties)
                    }
                };
            }
            else // cliente
            {
                finalPayload = new
                {
                    username = $"{UserInfo.firstName.ToLower()}_{UserInfo.lastName.ToLower()}",
                    firstName = UserInfo.firstName,
                    lastName = UserInfo.lastName,
                    gender = UserInfo.gender,
                    age = UserInfo.age,
                    dni = UserInfo.dni,
                    country = UserInfo.country,
                    state = UserInfo.state,
                    startTime = UserInfo.StartTime,
                    endTime = UserInfo.EndTime,
                    address = UserInfo.address,
                    additionalData = "Disponible para sesiones",
                    clientProfile = new
                    {
                        struggles = MapStrugglesToIds(UserInfo.Struggles)
                    }
                };
            }

            int idurl = UserInfo.userId;
            Debug.WriteLine($"USERID: {idurl}");
            string adress = UserInfo.address;
            Debug.WriteLine($"address: {adress}");

            string jsonPayload = JsonSerializer.Serialize(finalPayload);
            HttpContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            Debug.WriteLine($"Enviando datos a la API: {jsonPayload}");

            // Aquí haces el POST a la API con httpClient.SendAsync si deseas

            string apiUrl = "http://10.0.2.2:5050/api/profiles/" + idurl;

            using var httpClient = new HttpClient();

            try
            {
                var response = await httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("✅ Datos enviados correctamente.");

                    await Application.Current.MainPage.DisplayAlert("Perfil", "Perfil Completado", "OK");


                    var responseEMAIL = await httpClient.GetAsync("http://10.0.2.2:5050/api/users/mailbyid/" + idurl);

                    if (responseEMAIL.IsSuccessStatusCode)
                    {
                        var email = await responseEMAIL.Content.ReadAsStringAsync();
                        Console.WriteLine($"Email recibido: {email}");

                        var contentaux = new StringContent($"\"{email}\"", Encoding.UTF8, "application/json");
                        var responseaux = await httpClient.PostAsync("http://10.0.2.2:5050/api/mail/send-welcome", contentaux);
                        await Shell.Current.GoToAsync($"///MainPage");
                    }
                    else
                    {
                        Console.WriteLine($"Error: {responseEMAIL.StatusCode}");
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"❌ Error al enviar: {response.StatusCode}");
                    Debug.WriteLine($"📦 Contenido del error: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"🚨 Excepción en el POST: {ex.ToString()}");
            }

        }

        // Implementación de INotifyPropertyChanged.
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
