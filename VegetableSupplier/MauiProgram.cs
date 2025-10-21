using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using VegetableSupplier.Services;
using VegetableSupplier.ViewModels;
using VegetableSupplier.Views;

namespace VegetableSupplier;

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
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Register services
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<GoogleService>();
        builder.Services.AddSingleton<AuthenticationService>();
        builder.Services.AddSingleton<LocalizationManager>();
        builder.Services.AddSingleton<InventoryService>();
        builder.Services.AddSingleton<AnalyticsService>();
        builder.Services.AddSingleton<CustomerService>();

        // Register ViewModels
        builder.Services.AddTransient<LoginPageViewModel>();
        builder.Services.AddTransient<DashboardPageViewModel>();
        builder.Services.AddTransient<VendorsPageViewModel>();
        builder.Services.AddTransient<VendorDetailPageViewModel>();
        builder.Services.AddTransient<VegetablesPageViewModel>();
        builder.Services.AddTransient<VegetableEditViewModel>();
        builder.Services.AddTransient<SettingsPageViewModel>();
        builder.Services.AddTransient<CreateInvoicePageViewModel>();
        builder.Services.AddTransient<AnalyticsDashboardViewModel>();
        builder.Services.AddTransient<CustomerManagementViewModel>();
        builder.Services.AddTransient<InventoryDashboardViewModel>();

        // Register Views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<VendorsPage>();
        builder.Services.AddTransient<VendorDetailPage>();
        builder.Services.AddTransient<VegetablesPage>();
        builder.Services.AddTransient<VegetableEditPage>();
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<CreateInvoicePage>();
        builder.Services.AddTransient<AnalyticsDashboardPage>();
        builder.Services.AddTransient<CustomerManagementPage>();
        builder.Services.AddTransient<InventoryDashboardPage>();

        return builder.Build();
    }
}