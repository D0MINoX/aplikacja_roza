namespace MauiApp1.Controls;

public class MyEntry : Entry
{
    public static readonly BindableProperty UnderlineColorProperty =
        BindableProperty.Create(
            nameof(UnderlineColor),
            typeof(Color),
            typeof(MyEntry),
            Colors.Red);

    public Color UnderlineColor
    {
        get => (Color)GetValue(UnderlineColorProperty);
        set => SetValue(UnderlineColorProperty, value);
    }
}