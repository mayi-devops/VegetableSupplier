using VegetableSupplier.Services;

namespace VegetableSupplier.Views;

public partial class SplashPage : ContentPage
{
    private readonly AuthenticationService _authService;

    public SplashPage(AuthenticationService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        
        // Delay for a minimum splash screen display time
        await Task.Delay(2000);
        
        // Check authentication status
        if (await _authService.ValidateCurrentUserAsync())
        {
            await Shell.Current.GoToAsync("///main");
        }
        else
        {
            await Shell.Current.GoToAsync("///login");
        }
    }
}