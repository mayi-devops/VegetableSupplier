using System.ComponentModel;

namespace VegetableSupplier.Themes;

public class ThemeManager : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private static ThemeManager _instance;
    public static ThemeManager Instance => _instance ??= new ThemeManager();

    private Theme _currentTheme;
    private bool _isDarkMode;

    public bool IsDarkMode
    {
        get => _isDarkMode;
        set
        {
            if (_isDarkMode != value)
            {
                _isDarkMode = value;
                ApplyTheme();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDarkMode)));
            }
        }
    }

    public Theme CurrentTheme
    {
        get => _currentTheme;
        set
        {
            if (_currentTheme != value)
            {
                _currentTheme = value;
                ApplyTheme();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTheme)));
            }
        }
    }

    private ThemeManager()
    {
        LoadSavedTheme();
    }

    private void LoadSavedTheme()
    {
        var preferences = Preferences.Default;
        IsDarkMode = preferences.Get("IsDarkMode", false);
        var themeName = preferences.Get("CurrentTheme", "Default");
        CurrentTheme = GetTheme(themeName);
    }

    private Theme GetTheme(string themeName)
    {
        return themeName switch
        {
            "Nature" => new NatureTheme(),
            "Ocean" => new OceanTheme(),
            "Sunset" => new SunsetTheme(),
            _ => new DefaultTheme()
        };
    }

    private void ApplyTheme()
    {
        if (CurrentTheme == null) return;

        var resources = Application.Current.Resources;
        var theme = CurrentTheme.GetColors(IsDarkMode);

        foreach (var (key, value) in theme)
        {
            if (resources.ContainsKey(key))
                resources[key] = value;
            else
                resources.Add(key, value);
        }

        // Save theme preferences
        var preferences = Preferences.Default;
        preferences.Set("IsDarkMode", IsDarkMode);
        preferences.Set("CurrentTheme", CurrentTheme.Name);
    }

    public IEnumerable<Theme> GetAvailableThemes()
    {
        return new Theme[]
        {
            new DefaultTheme(),
            new NatureTheme(),
            new OceanTheme(),
            new SunsetTheme()
        };
    }
}