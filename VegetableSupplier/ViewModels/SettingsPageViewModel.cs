using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Globalization;
using VegetableSupplier.Services;

namespace VegetableSupplier.ViewModels;

public partial class SettingsPageViewModel : ObservableObject
{
    private readonly AuthenticationService _authService;
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private ObservableCollection<LanguageOption> availableLanguages;

    [ObservableProperty]
    private LanguageOption selectedLanguage;

    [ObservableProperty]
    private string userEmail;

    [ObservableProperty]
    private string userName;

    [ObservableProperty]
    private string appVersion;

    [ObservableProperty]
    private string buildNumber;

    [ObservableProperty]
    private string databaseSize;

    public SettingsPageViewModel(AuthenticationService authService, DatabaseService databaseService)
    {
        _authService = authService;
        _databaseService = databaseService;

        InitializeLanguages();
        LoadUserInfo();
        LoadAppInfo();
        LoadStorageInfo();
    }

    private void InitializeLanguages()
    {
        AvailableLanguages = new ObservableCollection<LanguageOption>
        {
            new LanguageOption { Code = "en", Name = "English" },
            new LanguageOption { Code = "hi", Name = "हिंदी" },
            new LanguageOption { Code = "te", Name = "తెలుగు" },
            // Add more languages as needed
        };

        var currentCulture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        SelectedLanguage = AvailableLanguages.FirstOrDefault(l => l.Code == currentCulture) 
            ?? AvailableLanguages.First();
    }

    private async Task LoadUserInfo()
    {
        var preferences = Preferences.Default;
        var currentEmail = preferences.Get("CurrentUserEmail", string.Empty);
        if (!string.IsNullOrEmpty(currentEmail))
        {
            var user = await _databaseService.GetUserAsync(currentEmail);
            if (user != null)
            {
                UserEmail = user.Email;
                UserName = user.DisplayName;
            }
        }
    }

    private void LoadAppInfo()
    {
        AppVersion = AppInfo.VersionString;
        BuildNumber = AppInfo.BuildString;
    }

    private async Task LoadStorageInfo()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "VegetableSupplier.db");
        var fileInfo = new FileInfo(dbPath);
        DatabaseSize = FormatFileSize(fileInfo.Length);
    }

    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = bytes;

        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:0.##} {sizes[order]}";
    }

    partial void OnSelectedLanguageChanged(LanguageOption value)
    {
        if (value == null)
            return;

        // Change the app's culture
        var culture = new CultureInfo(value.Code);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        // Save the selected language
        Preferences.Default.Set("AppLanguage", value.Code);

        // Notify that the language has changed
        LocalizationManager.Instance.UpdateCulture(culture);
    }

    [RelayCommand]
    private async Task SignOut()
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Sign Out",
            "Are you sure you want to sign out?",
            "Yes", "No");

        if (confirm)
        {
            await _authService.SignOutAsync();
            await Shell.Current.GoToAsync("///login");
        }
    }

    [RelayCommand]
    private async Task ClearCache()
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Clear Cache",
            "This will clear all cached files. Your data will not be affected. Continue?",
            "Yes", "No");

        if (confirm)
        {
            try
            {
                var cacheDir = FileSystem.CacheDirectory;
                Directory.Delete(cacheDir, true);
                Directory.CreateDirectory(cacheDir);

                await Shell.Current.DisplayAlert(
                    "Success",
                    "Cache cleared successfully",
                    "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    "Failed to clear cache",
                    "OK");
            }
        }
    }
}

public class LanguageOption
{
    public string Code { get; set; }
    public string Name { get; set; }
}