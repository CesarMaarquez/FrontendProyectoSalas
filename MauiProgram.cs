using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Platform;

namespace AppSalas;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Poppins-Bold.ttf", "PoppinsBold");
                fonts.AddFont("Poppins-Medium.ttf", "PoppinsMedium");
                fonts.AddFont("Poppins-Regular.ttf", "PoppinsRegular");
                fonts.AddFont("Poppins-SemiBold.ttf", "PoppinsSemiBold");
                fonts.AddFont("Manrope-Regular.ttf", "ManropeRegular");
                fonts.AddFont("Manrope-Bold.ttf", "ManropeBold");
                fonts.AddFont("Manrope-Medium.ttf", "ManropeMedium");
                fonts.AddFont("Manrope-Light.ttf", "ManropeLight");
                fonts.AddFont("Manrope-ExtraBold.ttf", "ManropeExBold");
            });

        // Mapeo para los Entries sin línea inferior
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("Placeholder", (h, v) =>
        {
#if ANDROID
            h.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(
                Microsoft.Maui.Graphics.Colors.Transparent.ToPlatform());
#endif
#if IOS
            h.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
        });

        // Mapeo para los Pickers sin línea inferior
        Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("NoBorder", (handler, view) =>
        {
#if ANDROID
            handler.PlatformView.Background = null;
#endif
#if IOS
            handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
        });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
