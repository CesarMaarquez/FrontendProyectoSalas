using System.Globalization;
using System.Resources;

namespace AppSalas.Services
{
    // SERVICIO DE LOCALIZACIÓN: ADMINISTRA LOS RECURSOS LOCALIZADOS Y CAMBIOS DE CULTURA
    public static class LocalizationService
    {
        // RESOURCE MANAGER PARA ACCEDER A LOS ARCHIVOS DE RECURSOS LOCALIZADOS
        private static ResourceManager resourceManager = new ResourceManager("AppSalas.Resources.Language.AppResources", typeof(LocalizationService).Assembly);

        // EVENTO QUE NOTIFICA CUANDO CAMBIA EL IDIOMA DE LA APLICACIÓN
        public static event EventHandler LanguageChanged;

        // MÉTODO PARA OBTENER LA CADENA LOCALIZADA BASÁNDONOS EN UNA CLAVE
        public static string GetString(string key)
        {
            return resourceManager.GetString(key, CultureInfo.CurrentCulture) ?? string.Empty;
        }

        // MÉTODO PARA CAMBIAR LA CULTURA ACTUAL Y NOTIFICAR A LOS SUSCRIPTORES
        public static void ChangeCulture(CultureInfo culture)
        {
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            Preferences.Set("app_language", culture.Name); // GUARDAMOS EL IDIOMA SELECCIONADO
            LanguageChanged?.Invoke(null, EventArgs.Empty); // DISPARAMOS EL EVENTO DE CAMBIO DE IDIOMA
        }
    }
}
