using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

namespace AppSalas.ViewModel
{
    public class CalendarViewModel : INotifyPropertyChanged
    {
        private DateTime _shownDate = DateTime.Today;
        private DateTime _selectedDate = DateTime.Today;

        public ObservableCollection<DayItem> Days { get; }
        public string MonthYear { get; private set; }

        /// <summary>
        /// Fecha que el calendario muestra (mes/año). Cambios aquí disparan recarga.
        /// </summary>
        public DateTime ShownDate
        {
            get => _shownDate;
            set
            {
                if (_shownDate != value)
                {
                    _shownDate = value;
                    OnPropertyChanged(nameof(ShownDate));
                    LoadCalendar(_shownDate);
                }
            }
        }

        /// <summary>
        /// Fecha seleccionada dentro del mes.
        /// </summary>
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged(nameof(SelectedDate));
                    // Aquí puedes añadir lógica extra al cambiar la selección.
                }
            }
        }

        public CalendarViewModel()
        {
            Days = new ObservableCollection<DayItem>();
            LoadCalendar(_shownDate);
        }

        private void LoadCalendar(DateTime date)
        {
            Days.Clear();
            MonthYear = date.ToString("MMMM yyyy", CultureInfo.GetCultureInfo("es-ES")).ToUpper();
            OnPropertyChanged(nameof(MonthYear));

            // Calcula el offset para que lunes = 0
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            int startDay = ((int)firstDayOfMonth.DayOfWeek + 6) % 7;

            // Días vacíos antes del primer día
            for (int i = 0; i < startDay; i++)
                Days.Add(new DayItem { DayNumber = "", IsToday = false });

            int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
            for (int day = 1; day <= daysInMonth; day++)
            {
                var current = new DateTime(date.Year, date.Month, day);
                Days.Add(new DayItem
                {
                    DayNumber = day.ToString(),
                    IsToday = current.Date == DateTime.Today
                });
            }

            OnPropertyChanged(nameof(Days));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class DayItem
    {
        public string DayNumber { get; set; }
        public bool IsToday { get; set; }
    }
}
