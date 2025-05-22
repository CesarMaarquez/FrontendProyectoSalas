using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AppSalas.Model;
using CommunityToolkit.Maui.Alerts;

namespace AppSalas.ViewModel
{
    // Clase para cada struggle.
    public class StruggleItem : INotifyPropertyChanged
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

    // ViewModel para la selección de struggles (problemas del paciente).
    public class SelectStrugglesViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<StruggleItem> Struggles { get; set; }
        public ICommand ToggleStruggleCommand { get; }
        public ICommand SubmitCommand { get; }
        public ICommand BackCommand { get; }

        private UserInfoModel UserInfo;
        public event Action<UserInfoModel> OnNavigateToDisponibilityPage;

        public SelectStrugglesViewModel(UserInfoModel userInfo)
        {
            UserInfo = userInfo;

            Struggles = new ObservableCollection<StruggleItem>
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

            ToggleStruggleCommand = new Command<StruggleItem>(ToggleStruggle);
            SubmitCommand = new Command(Submit);
            BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        }

        private void ToggleStruggle(StruggleItem item)
        {
            if (item != null)
                item.IsSelected = !item.IsSelected;
        }

        private async void Submit()
        {
            var selected = Struggles.Where(s => s.IsSelected).Select(s => s.Name).ToList();

            if (selected.Count <= 0)
            {
                await Toast.Make("Selecciona alguna Struggle").Show();
                return;
            }
            string result = string.Join(", ", selected);

            UserInfo.Struggles = selected;

            await Shell.Current.DisplayAlert("Struggles Seleccionadas", result, "OK");

            foreach (var struggle in UserInfo.Struggles)
            {
                Debug.WriteLine("struggle:" + struggle);
            }

            Debug.WriteLine("USSSSERINFO:" + UserInfo.userId + " el nombre" + UserInfo.firstName);



            OnNavigateToDisponibilityPage?.Invoke(UserInfo);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
