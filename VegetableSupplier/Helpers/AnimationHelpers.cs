using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using System.Threading.Tasks;
using System;

namespace VegetableSupplier.Helpers;

public static class AnimationHelpers
{
    public static async Task AnimateBackgroundColor(this VisualElement element, Color fromColor, Color toColor, uint length = 250, Easing easing = null)
    {
        var animation = new Animation();
        animation.Add(0, 1, new Animation(v => element.BackgroundColor = Color.FromRgba(
            fromColor.Red + (toColor.Red - fromColor.Red) * v,
            fromColor.Green + (toColor.Green - fromColor.Green) * v,
            fromColor.Blue + (toColor.Blue - fromColor.Blue) * v,
            fromColor.Alpha + (toColor.Alpha - fromColor.Alpha) * v
        ), 0, 1, easing ?? Easing.Linear));

        await animation.Commit(element, "ColorTo", 16, length);
    }

    public static async Task Shake(this VisualElement element, uint duration = 500, double scale = 1.2)
    {
        await element.ScaleTo(scale, duration / 2);
        await element.ScaleTo(1, duration / 2);
    }

    public static async Task<bool> RotateIn(this VisualElement element, uint length = 250, double rotation = 360)
    {
        element.Rotation = 0;
        element.IsVisible = true;
        await element.FadeIn(length / 2);
        return await element.RotateTo(rotation, length);
    }

    public static async Task<bool> RotateOut(this VisualElement element, uint length = 250, double rotation = 360)
    {
        await element.RotateTo(rotation, length);
        return await element.FadeOut(length / 2);
    }

    public static async Task RippleEffect(this VisualElement element, uint duration = 500)
    {
        var originalScale = element.Scale;
        var originalOpacity = element.Opacity;

        await Task.WhenAll(
            element.ScaleTo(originalScale * 1.2, duration / 2),
            element.FadeTo(0.7, duration / 2)
        );

        await Task.WhenAll(
            element.ScaleTo(originalScale, duration / 2),
            element.FadeTo(originalOpacity, duration / 2)
        );
    }

    public static async Task Bounce(this VisualElement element, uint duration = 800)
    {
        var animation = new Animation();
        animation.Add(0, 0.5, new Animation(v => element.TranslationY = v, 0, -30, Easing.SpringOut));
        animation.Add(0.5, 1, new Animation(v => element.TranslationY = v, -30, 0, Easing.SpringIn));
        await animation.Commit(element, "Bounce", 16, duration, Easing.Linear);
    }

    public static async Task PulseScale(this VisualElement element, uint duration = 500, double scale = 1.1)
    {
        await element.ScaleTo(scale, duration / 2, Easing.Linear);
        await element.ScaleTo(1, duration / 2, Easing.Linear);
    }
    public static async Task<bool> FadeIn(this VisualElement element, uint length = 250, double opacity = 1.0)
    {
        element.Opacity = 0;
        element.IsVisible = true;
        return await element.FadeTo(opacity, length);
    }

    public static async Task<bool> FadeOut(this VisualElement element, uint length = 250)
    {
        var result = await element.FadeTo(0, length);
        element.IsVisible = false;
        return result;
    }

    public static async Task<bool> ScaleIn(this VisualElement element, uint length = 250, double scale = 1.0)
    {
        await element.FadeIn(length / 2);
        return await element.ScaleTo(scale, length);
    }

    public static async Task<bool> ScaleOut(this VisualElement element, uint length = 250)
    {
        await element.ScaleTo(0, length);
        return await element.FadeOut(length / 2);
    }

    public static async Task<bool> SlideIn(this VisualElement element, uint length = 250, double offset = 50)
    {
        element.TranslationY = offset;
        element.IsVisible = true;
        await element.FadeIn(length);
        return await element.TranslateTo(0, 0, length, Easing.SpringOut);
    }

    public static async Task<bool> SlideOut(this VisualElement element, uint length = 250, double offset = 50)
    {
        await element.TranslateTo(0, offset, length, Easing.SpringIn);
        return await element.FadeOut(length);
    }
}