using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using Avalonia.Input.Platform;
using System;
using System.Text;
using System.Numerics;
using System.Text.RegularExpressions;

namespace BiCal
{
    public partial class BinomialCoefficientWindow : Window
    {
        private TextBox? _expressionInput = new TextBox();
        private TextBox? _termInput = new TextBox();
        private TextBlock? _resultBlock;
        public BinomialCoefficientWindow()
        {
            InitializeComponent();
            _expressionInput = this.FindControl<TextBox>("ExpressionInput") ?? throw new InvalidOperationException("ExpressionInput not found");
            _termInput = this.FindControl<TextBox>("TermInput") ?? throw new InvalidOperationException("TermInput not found");
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
            _expressionInput?.Clear();
            _termInput?.Clear();
            if(_resultBlock != null)
                _resultBlock.Text = "Result will be shown here";
        }

        private void OnCopyClick(object sender, RoutedEventArgs e){
            var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
            if(!string.IsNullOrWhiteSpace(_resultBlock?.Text)){
                clipboard?.SetTextAsync(_resultBlock.Text);
            }
        }
         private void OnCalculateClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string binomialExpression = _expressionInput?.Text ?? string.Empty;
                string termExpression = _termInput?.Text ?? string.Empty;

                int totalPower = ExtractPower(binomialExpression);
                (int exponentX, int exponentY) = ExtractExponents(termExpression);

                // Validate exponents
                if (exponentX + exponentY != totalPower)
                {
                    if (_resultBlock != null)
                    {
                        _resultBlock.Text = "The sum of exponents of x and y must equal the total power.";
                    }
                    return;
                }

                BigInteger coefficient = CalculateCombination(totalPower, exponentX);

                if (_resultBlock != null)
                {
                    _resultBlock.Text = $"The coefficient is: {coefficient}";
                }
            }
            catch (Exception)
            {
                if (_resultBlock != null)
                {
                    _resultBlock.Text = "Invalid Input";
                }
            }
        }
        
        private BigInteger CalculateCombination(int n, int k)
        {
            return Factorial(n) / (Factorial(k)*Factorial(n-k));
        }

        private int ExtractPower(string expression)
        {
            var match = Regex.Match(expression, @"\^\s*(\d+)");
            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }
            throw new ArgumentException("Invalid binomial expression. Ensure it is in the form (x + y)^n.");
        }
         private (int, int) ExtractExponents(string expression)
        {
            var matches = Regex.Matches(expression, @"x\^\s*(\d+)|y\^\s*(\d+)");
            if (matches.Count == 2)
            {
                int exponentX = int.Parse(matches[0].Groups[1].Value);
                int exponentY = int.Parse(matches[1].Groups[2].Value);
                return (exponentX, exponentY);
            }
            throw new ArgumentException("Invalid term expression. Ensure it is in the form x^k y^j.");
        }
         private BigInteger Factorial(int n)
        {
            if (n == 0 || n == 1)
            {
                return 1;
            }

            BigInteger result = 1;
            for (int i = 1; i <= n; i++)
            {
                result *= i;
            }
            return result;
        }
    }
}