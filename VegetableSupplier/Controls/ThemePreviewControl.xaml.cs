using VegetableSupplier.Themes;

namespace VegetableSupplier.Controls;

public partial class ThemePreviewControl : ContentView
{
    public static readonly BindableProperty ThemeProperty =
        BindableProperty.Create(nameof(Theme), typeof(Theme), typeof(ThemePreviewControl), null,
            propertyChanged: OnThemeChanged);

    public Theme Theme
    {
        get => (Theme)GetValue(ThemeProperty);
        set => SetValue(ThemeProperty, value);
    }

    public string ThemeName => Theme?.Name;
    public Color PrimaryColor => GetThemeColor("Primary");
    public Color SecondaryColor => GetThemeColor("Secondary");
    public Color TertiaryColor => GetThemeColor("Tertiary");
    public Color TextColor => GetThemeColor("Text");
    public Color TextSecondaryColor => GetThemeColor("TextSecondary");

    public ThemePreviewControl()
    {
        InitializeComponent();
        BindingContext = this;
    }

    private Color GetThemeColor(string key)
    {
        return Theme?.GetColors(false)[key] ?? Colors.Transparent;
    }

    private static void OnThemeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ThemePreviewControl)bindable;
        control.OnPropertyChanged(nameof(ThemeName));
        control.OnPropertyChanged(nameof(PrimaryColor));
        control.OnPropertyChanged(nameof(SecondaryColor));
        control.OnPropertyChanged(nameof(TertiaryColor));
        control.OnPropertyChanged(nameof(TextColor));
        control.OnPropertyChanged(nameof(TextSecondaryColor));
    }
}