namespace VegetableSupplier.Helpers;

public static class AccessibilityHelper
{
    public static void SetupAccessibility(this VisualElement element, string label, string hint = null)
    {
        SemanticProperties.SetDescription(element, label);
        if (!string.IsNullOrEmpty(hint))
        {
            SemanticProperties.SetHint(element, hint);
        }
        AutomationProperties.SetIsInAccessibleTree(element, true);
        AutomationProperties.SetName(element, label);
    }

    public static void SetupHeading(this Label element)
    {
        SemanticProperties.SetHeadingLevel(element, SemanticHeadingLevel.Level1);
        AutomationProperties.SetIsInAccessibleTree(element, true);
    }

    public static void SetupButton(this Button button, string description)
    {
        SemanticProperties.SetDescription(button, description);
        AutomationProperties.SetIsInAccessibleTree(button, true);
        AutomationProperties.SetName(button, button.Text);
    }

    public static void SetupListItem(this VisualElement element, string label, string value)
    {
        SemanticProperties.SetDescription(element, $"{label}: {value}");
        AutomationProperties.SetIsInAccessibleTree(element, true);
        AutomationProperties.SetName(element, label);
    }

    public static void SetupImage(this Image image, string description)
    {
        SemanticProperties.SetDescription(image, description);
        AutomationProperties.SetIsInAccessibleTree(image, true);
        AutomationProperties.SetName(image, description);
    }

    public static void SetupEntry(this Entry entry, string label, string placeholder = null)
    {
        SemanticProperties.SetDescription(entry, label);
        AutomationProperties.SetIsInAccessibleTree(entry, true);
        AutomationProperties.SetName(entry, label);
        AutomationProperties.SetHelpText(entry, placeholder ?? entry.Placeholder);
    }

    public static void SetupPicker(this Picker picker, string label)
    {
        SemanticProperties.SetDescription(picker, label);
        AutomationProperties.SetIsInAccessibleTree(picker, true);
        AutomationProperties.SetName(picker, label);
    }

    public static void SetupTab(this VisualElement element, string label)
    {
        SemanticProperties.SetDescription(element, label);
        AutomationProperties.SetIsInAccessibleTree(element, true);
        AutomationProperties.SetName(element, label);
        AutomationProperties.SetAccessibilityView(element, AccessibilityView.Control);
    }
}