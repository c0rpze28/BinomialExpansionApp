using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;

namespace BiCal;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnExpansionClick(object sender, RoutedEventArgs e)
    {
        var binomialExpansionWindow= new BinomialExpansionWindow();
        binomialExpansionWindow.Show();
        this.Close();
    }
    private void OnCoefficientClick(object sender, RoutedEventArgs e)
    {
        var binomialCoefficientWindow = new BinomialCoefficientWindow();
        binomialCoefficientWindow.Show();
        this.Close();
    }
}