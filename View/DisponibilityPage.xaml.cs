using AppSalas.Model;
using AppSalas.ViewModel;  

namespace AppSalas.View
{
    public partial class DisponibilityPage : ContentPage
    {
        public DisponibilityPage(UserInfoModel UserInfo)
        {
            InitializeComponent();
            BindingContext = new DisponibilityViewModel(UserInfo);
        }
    }
}
