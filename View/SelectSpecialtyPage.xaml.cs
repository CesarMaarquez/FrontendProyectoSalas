using Microsoft.Maui.Controls;
using AppSalas.ViewModel;

namespace AppSalas.View
{
    [QueryProperty(nameof(UserId), "userId")]
    public partial class SelectSpecialtyPage : ContentPage
    {
        private int userId;
        public int UserId
        {
            get => userId;
            set
            {
                userId = value;
                BindingContext = new SelectSpecialtyViewModel(userId);
            }
        }

        public SelectSpecialtyPage()
        {
            InitializeComponent();
        }
    }

}
