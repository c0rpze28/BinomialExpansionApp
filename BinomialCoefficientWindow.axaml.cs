using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using System;
using System.Text;
using System.Numerics;

namespace BiCal
{
    public partial class BinomialCoefficientWindow : Window
    {
        private TextBox? _powerInput = new TextBox();
        private TextBox? _exponentInput = new TextBox();
        private TextBlock? _resultBlock;
        public BinomialCoefficientWindow()
        {
            InitializeComponent();
            _powerInput = this.FindControl<TextBox>("PowerInput") ?? throw new InvalidOperationException("PowerInput not found");
            _exponentInput = this.FindControl<TextBox>("ExponentInput") ?? throw new InvalidOperationException("ExponentInput not found");
            _resultBlock = this.FindControl<TextBlock>("CoefficientTextBlock") ?? new TextBlock();
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        private void OnBackClick(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
        private void OnClearClick(object sender, RoutedEventArgs e){
            _powerInput?.Clear();
            _exponentInput?.Clear();
            if(_resultBlock != null)
                _resultBlock.Text = "Result will be shown here";
        }
        private void OnCalculateClick(object sender, RoutedEventArgs e)
        {
            int n, k;
            if(int.TryParse(_powerInput?.Text, out n) && int.TryParse(_exponentInput?.Text, out k))
            {
                if (k>n)
                {
                    if(_resultBlock != null){
                        _resultBlock.Text = "Exponent of x cannot be greater than the total power";
                    }
                    return;
                }

                BigInteger coefficient = CaclulateBinomialCoefficient(n,k);
                if(_resultBlock != null){
                    _resultBlock.Text = $"The coefficient is: {coefficient}";
                }
                
            }
            else{
                if(_resultBlock != null)
                {
                    _resultBlock.Text="Invalid Input";
                }
            }
        }
        
        private BigInteger CaclulateBinomialCoefficient(int n, int k)
        {
            return Factorial(n) / (Factorial(k)*Factorial(n-k));
        }

        private BigInteger Factorial(int n){
            if (n == 0 || n==1){
                return 1;
            }
            
            BigInteger result = 1;
            for(int i=1; i<=n;i++){
                result *= i;
            }
            return result;
            
        }
    }
}