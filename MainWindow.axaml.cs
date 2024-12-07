using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BiCal;

public partial class MainWindow : Window
{
    private int _clickCount = 0;
    public MainWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnButtonClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _clickCount++;

        var greetingText = this.FindControl<TextBlock>("greetingText");
        if (greetingText != null)
        {
            greetingText.Text = $"Button Clicked! {_clickCount}";
        }
        
    }
}