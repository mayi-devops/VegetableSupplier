using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;

namespace VegetableSupplier.Themes;

public abstract class Theme
{
    public abstract string Name { get; }
    public abstract Dictionary<string, Color> GetColors(bool isDarkMode);
}

public class DefaultTheme : Theme
{
    public override string Name => "Default";

    public override Dictionary<string, Color> GetColors(bool isDarkMode)
    {
        return new Dictionary<string, Color>
        {
            { "Primary", Color.Parse("#2B5D4F") },
            { "PrimaryDark", Color.Parse("#1E3F35") },
            { "Secondary", Color.Parse("#7EB693") },
            { "Tertiary", Color.Parse("#EFD372") },
            { "Background", isDarkMode ? Color.Parse("#121212") : Colors.White },
            { "Surface", isDarkMode ? Color.Parse("#1E1E1E") : Colors.White },
            { "Text", isDarkMode ? Colors.White : Colors.Black },
            { "TextSecondary", isDarkMode ? Color.Parse("#B3B3B3") : Color.Parse("#666666") }
        };
    }
}

public class NatureTheme : Theme
{
    public override string Name => "Nature";

    public override Dictionary<string, Color> GetColors(bool isDarkMode)
    {
        return new Dictionary<string, Color>
        {
            { "Primary", Color.Parse("#4CAF50") },
            { "PrimaryDark", Color.Parse("#388E3C") },
            { "Secondary", Color.Parse("#8BC34A") },
            { "Tertiary", Color.Parse("#FFC107") },
            { "Background", isDarkMode ? Color.Parse("#1B2415") : Colors.White },
            { "Surface", isDarkMode ? Color.Parse("#2A361F") : Colors.White },
            { "Text", isDarkMode ? Colors.White : Colors.Black },
            { "TextSecondary", isDarkMode ? Color.Parse("#B3B3B3") : Color.Parse("#666666") }
        };
    }
}

public class OceanTheme : Theme
{
    public override string Name => "Ocean";

    public override Dictionary<string, Color> GetColors(bool isDarkMode)
    {
        return new Dictionary<string, Color>
        {
            { "Primary", Color.Parse("#0288D1") },
            { "PrimaryDark", Color.Parse("#01579B") },
            { "Secondary", Color.Parse("#03A9F4") },
            { "Tertiary", Color.Parse("#00BCD4") },
            { "Background", isDarkMode ? Color.Parse("#102027") : Colors.White },
            { "Surface", isDarkMode ? Color.Parse("#1C313A") : Colors.White },
            { "Text", isDarkMode ? Colors.White : Colors.Black },
            { "TextSecondary", isDarkMode ? Color.Parse("#B3B3B3") : Color.Parse("#666666") }
        };
    }
}

public class SunsetTheme : Theme
{
    public override string Name => "Sunset";

    public override Dictionary<string, Color> GetColors(bool isDarkMode)
    {
        return new Dictionary<string, Color>
        {
            { "Primary", Color.Parse("#FF5722") },
            { "PrimaryDark", Color.Parse("#E64A19") },
            { "Secondary", Color.Parse("#FF9800") },
            { "Tertiary", Color.Parse("#FFC107") },
            { "Background", isDarkMode ? Color.Parse("#251816") : Colors.White },
            { "Surface", isDarkMode ? Color.Parse("#3E2723") : Colors.White },
            { "Text", isDarkMode ? Colors.White : Colors.Black },
            { "TextSecondary", isDarkMode ? Color.Parse("#B3B3B3") : Color.Parse("#666666") }
        };
    }
}

public class CyberTheme : Theme
{
    public override string Name => "Cyber";

    public override Dictionary<string, Color> GetColors(bool isDarkMode)
    {
        return new Dictionary<string, Color>
        {
            { "Primary", Color.Parse("#00FF9F") },
            { "PrimaryDark", Color.Parse("#00CC7F") },
            { "Secondary", Color.Parse("#FF00FF") },
            { "Tertiary", Color.Parse("#00FFFF") },
            { "Background", isDarkMode ? Color.Parse("#0D0D0D") : Colors.White },
            { "Surface", isDarkMode ? Color.Parse("#1A1A1A") : Colors.White },
            { "Text", isDarkMode ? Colors.White : Colors.Black },
            { "TextSecondary", isDarkMode ? Color.Parse("#B3B3B3") : Color.Parse("#666666") }
        };
    }
}

public class DesertTheme : Theme
{
    public override string Name => "Desert";

    public override Dictionary<string, Color> GetColors(bool isDarkMode)
    {
        return new Dictionary<string, Color>
        {
            { "Primary", Color.Parse("#D4AC0D") },
            { "PrimaryDark", Color.Parse("#B7950B") },
            { "Secondary", Color.Parse("#E67E22") },
            { "Tertiary", Color.Parse("#CB4335") },
            { "Background", isDarkMode ? Color.Parse("#1C1611") : Color.Parse("#F9E4B7") },
            { "Surface", isDarkMode ? Color.Parse("#2C241B") : Color.Parse("#FDF2E9") },
            { "Text", isDarkMode ? Colors.White : Colors.Black },
            { "TextSecondary", isDarkMode ? Color.Parse("#B3B3B3") : Color.Parse("#666666") }
        };
    }
}

public class MonochromeTheme : Theme
{
    public override string Name => "Monochrome";

    public override Dictionary<string, Color> GetColors(bool isDarkMode)
    {
        return new Dictionary<string, Color>
        {
            { "Primary", isDarkMode ? Color.Parse("#FFFFFF") : Color.Parse("#000000") },
            { "PrimaryDark", isDarkMode ? Color.Parse("#E0E0E0") : Color.Parse("#1A1A1A") },
            { "Secondary", isDarkMode ? Color.Parse("#CCCCCC") : Color.Parse("#333333") },
            { "Tertiary", isDarkMode ? Color.Parse("#999999") : Color.Parse("#666666") },
            { "Background", isDarkMode ? Color.Parse("#000000") : Colors.White },
            { "Surface", isDarkMode ? Color.Parse("#1A1A1A") : Color.Parse("#F5F5F5") },
            { "Text", isDarkMode ? Colors.White : Colors.Black },
            { "TextSecondary", isDarkMode ? Color.Parse("#B3B3B3") : Color.Parse("#666666") }
        };
    }
}

public class VintageTheme : Theme
{
    public override string Name => "Vintage";

    public override Dictionary<string, Color> GetColors(bool isDarkMode)
    {
        return new Dictionary<string, Color>
        {
            { "Primary", Color.Parse("#8B4513") },
            { "PrimaryDark", Color.Parse("#6B3410") },
            { "Secondary", Color.Parse("#DEB887") },
            { "Tertiary", Color.Parse("#CD853F") },
            { "Background", isDarkMode ? Color.Parse("#1C1611") : Color.Parse("#F5E6D3") },
            { "Surface", isDarkMode ? Color.Parse("#2C241B") : Color.Parse("#FFF5E6") },
            { "Text", isDarkMode ? Colors.White : Colors.Black },
            { "TextSecondary", isDarkMode ? Color.Parse("#B3B3B3") : Color.Parse("#666666") }
        };
    }
}