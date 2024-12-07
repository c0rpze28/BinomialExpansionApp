using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
namespace BiCal
{
    public partial class BinomialExpansionWindow : Window
    {
        public BinomialExpansionWindow()
        {
            InitializeComponent();
        }
         private void OnBackClick(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
