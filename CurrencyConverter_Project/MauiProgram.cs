using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using CommunityToolkit.Maui;

namespace CurrencyConverter_Project
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .ConfigureMauiHandlers(handlers =>
                {
                    handlers.AddHandler<Entry, CustomEntryHandler>();
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
    public class CustomEntryHandler : EntryHandler
    {
        public CustomEntryHandler()
        {
            Mapper.AppendToMapping("NoUnderline", (handler, view) =>
            {

#if ANDROID
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#endif
            });
        }
    }
}
