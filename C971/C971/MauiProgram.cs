using C971.Database;
using C971.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;

namespace C971
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                // Initialize the .NET MAUI Community Toolkit by adding the below line of code
                .UseMauiCommunityToolkit()
                .UseLocalNotification()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<DatabaseHandler>();
            builder.Services.AddTransient<TermPage>();
            builder.Services.AddTransient<AddTermPage>();
            builder.Services.AddTransient<EditTermPage>();
            builder.Services.AddTransient<CoursePage>();
            builder.Services.AddTransient<AddCoursePage>();
            builder.Services.AddTransient<EditCoursePage>();
            builder.Services.AddTransient<AssessmentPage>();
            builder.Services.AddTransient<AddAssessmentPage>();
            builder.Services.AddTransient<EditAssessmentPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
