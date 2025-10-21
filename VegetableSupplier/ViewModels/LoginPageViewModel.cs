using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VegetableSupplier.Services;

namespace VegetableSupplier.ViewModels;

public partial class LoginPageViewModel : ObservableObject
{
    private readonly AuthenticationService _authService;
    
    [ObservableProperty]
    private bool isBusy;

    public bool IsNotBusy => !IsBusy;

    public LoginPageViewModel(AuthenticationService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    async Task SignIn()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            var success = await _authService.SignInWithGoogleAsync();
            
            if (success)
            {
                await Shell.Current.GoToAsync("///main");
            }
            else
            {
                await Shell.Current.DisplayAlert(
                    "Access Denied",
                    "You are not authorized to use this application. Please contact the administrator.",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                "An error occurred while signing in. Please try again.",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}