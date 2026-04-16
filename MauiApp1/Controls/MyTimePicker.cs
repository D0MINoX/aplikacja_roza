namespace MauiApp1.Controls;

public class MyTimePicker : TimePicker
{
    public static readonly BindableProperty UnderlineColorProperty =
        BindableProperty.Create(
            nameof(UnderlineColor),
            typeof(Color),
            typeof(MyTimePicker),
            Colors.Red);

    public Color UnderlineColor
    {
        get => (Color)GetValue(UnderlineColorProperty);
        set => SetValue(UnderlineColorProperty, value);
    }
}
