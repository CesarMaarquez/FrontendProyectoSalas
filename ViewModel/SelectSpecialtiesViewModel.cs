using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AppSalas.Model;
using System.Diagnostics;
using CommunityToolkit.Maui.Alerts;

namespace AppSalas.ViewModel
{
    // Clase para cada especialidad.
    public partial class SpecialtyItem : INotifyPropertyChanged
    {
        private bool isSelected;
        public string Name { get; set; } = string.Empty;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // ViewModel para la selección múltiple de especialidades.
    public partial class SelectSpecialtiesViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<SpecialtyItem> Specialties { get; set; }
        public ICommand ToggleSpecialtyCommand { get; }
        public ICommand SubmitCommand { get; }
        public ICommand BackCommand { get; }

        private UserInfoModel UserInfo;
        public event Action<UserInfoModel> OnNavigateToDisponibilityPage;
        public SelectSpecialtiesViewModel(UserInfoModel UserInfo)
        {
            this.UserInfo = UserInfo;

            Specialties = new ObservableCollection<SpecialtyItem>
            {
                new() { Name = "Depression" },
                new() { Name = "Anxiety" },
                new() { Name = "Behavior Change" },
                new() { Name = "Family Therapy" },
                new() { Name = "Sleeping Difficulty" },
                new() { Name = "PTSD" },
                new() { Name = "Loneliness" },
                new() { Name = "Retirement" },
                new() { Name = "Companionship" },
                new() { Name = "Declining Health" },
                new() { Name = "Declining Mortality" }
            };

            ToggleSpecialtyCommand = new Command<SpecialtyItem>(ToggleSpecialty);
            SubmitCommand = new Command(Submit);
            BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        }

        private void ToggleSpecialty(SpecialtyItem item)
        {
            if (item != null)
                item.IsSelected = !item.IsSelected;
        }

        private async void Submit()
        {
            var selected = Specialties.Where(s => s.IsSelected).Select(s => s.Name).ToList();

            if (selected.Count <= 0)
            {
                await Toast.Make("Selecciona alguna Especialidad").Show();
                return;
            }
            string result = string.Join(", ", selected);



            UserInfo.Specialties = selected;

            foreach (var speciality in UserInfo.Specialties)
            {
                Debug.WriteLine("struggle:" + speciality);
            }

            await Shell.Current.DisplayAlert("Especialidades Seleccionadas", result, "OK");

            OnNavigateToDisponibilityPage?.Invoke(UserInfo);

        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
