using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Diagnostics;
using AppSalas.Model;
using CommunityToolkit.Maui.Alerts;


namespace AppSalas.ViewModel
{
    public partial class EducationalInfoViewModel : INotifyPropertyChanged
    {
        // Comando para guardar la información y navegar a SelectSpecialtiesPage
        public ICommand SaveInformationCommand { get; }
        // Comando para subir el certificado
        public ICommand UploadCertificateCommand { get; }
        public ICommand BackCommand { get; }


        public UserInfoModel UserInfo;
        public EducationalInfoViewModel(UserInfoModel userinfo)
        {
            UserInfo = userinfo;
            SaveInformationCommand = new Command(async () => await OnSaveInformation());
            UploadCertificateCommand = new Command(OnUploadCertificate);
            BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));

        }

        public string Degree { get; set; }
        public string Institution { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today;

        public event Action<UserInfoModel> OnNavigateToSpecialitiesPage;

        public event Action<UserInfoModel> OnNavigateToStrugglesPage;



        private async Task OnSaveInformation()
        {
            // Navega a SelectSpecialtiesPage usando Shell.Current. No se usa datos de instancia.


            if (Institution == null || Degree == null)
            {
                await Toast.Make("Por favor, completa todos los campos").Show();
                return;


            }


            UserInfo.Institution = Institution;
            UserInfo.Degree = Degree;
            UserInfo.StartDate = StartDate;
            UserInfo.EndDate = EndDate;



            Debug.WriteLine("ENDDATE:" + UserInfo.EndDate);


            if (UserInfo.UserType == "therapist")
            {
                OnNavigateToSpecialitiesPage?.Invoke(UserInfo);
            }
            else
            {
                OnNavigateToStrugglesPage?.Invoke(UserInfo);
            }


        }

        private void OnUploadCertificate()
        {
            // Lógica para subir el certificado (por ejemplo: abrir un selector de archivos)
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
