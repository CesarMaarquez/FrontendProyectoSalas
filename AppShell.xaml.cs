namespace AppSalas
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Navigated += AppShell_Navigated;

            // Esperamos hasta que la navegación esté lista
            if (Shell.Current != null)
            {
                // Esperamos a que la navegación se haya inicializado antes de realizar cualquier acción
                Shell.Current.Navigating += (sender, e) =>
                {
                    if (Shell.Current.CurrentItem == null)
                    {
                        Shell.Current.GoToAsync("//MainPage");  // Navegar a la MainPage
                    }
                };
            }
        }

        private void AppShell_Navigated(object sender, ShellNavigatedEventArgs e)
        {
            var hideMenuRoutes = new[]
            {
        "MainPage", "RegisterPage", "EmailVerificationPage", "SelectSpecialtyPage",
        "PersonalInfoPage", "EducationalInfoPage", "SelectSpecialtiesPage",
        "DisponibilityPage", "Log_InPage", "ForgotPasswordPage", "SetNewPasswordPage"
    };

            var location = Shell.Current?.CurrentState?.Location?.ToString();

            FlyoutBehavior = hideMenuRoutes.Any(route => location.Contains(route))
                ? FlyoutBehavior.Disabled
                : FlyoutBehavior.Flyout;
        }

    }


}
