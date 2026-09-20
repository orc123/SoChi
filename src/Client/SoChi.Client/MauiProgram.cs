using Microsoft.Extensions.Logging;

using SoChi.Client.Services;
using SoChi.Client.ViewModels;
using SoChi.Client.Views;

namespace SoChi.Client;

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
            });

        builder.Services.AddTransient<OverviewViewModel>();
        builder.Services.AddTransient<CategoriesViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<StatisticsViewModel>();
        builder.Services.AddTransient<TransactionsViewModel>();
        builder.Services.AddTransient<TransactionFormViewModel>();

        builder.Services.AddTransient<OverviewPage>();
        builder.Services.AddTransient<CategoriesPage>();
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<StatisticsPage>();
        builder.Services.AddTransient<TransactionsPage>();
        builder.Services.AddTransient<TransactionFormPage>();

        builder.Services.AddSingleton<ITransactionRepository, TransactionRepository>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}