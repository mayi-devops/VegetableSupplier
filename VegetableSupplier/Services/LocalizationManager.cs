using System.Globalization;

namespace VegetableSupplier.Services;

public class LocalizationManager
{
    public static LocalizationManager Instance { get; } = new LocalizationManager();

    public event EventHandler CultureChanged;

    private LocalizationManager()
    {
        // Load saved language or use system default
        var savedLanguage = Preferences.Default.Get("AppLanguage", string.Empty);
        if (!string.IsNullOrEmpty(savedLanguage))
        {
            UpdateCulture(new CultureInfo(savedLanguage));
        }
    }

    public void UpdateCulture(CultureInfo culture)
    {
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        CultureChanged?.Invoke(this, EventArgs.Empty);
    }
}