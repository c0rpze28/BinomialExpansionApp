using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace BiCal
{
    public partial class BinomialExpansionWindow : Window
    {
        private TextBox? _expressionInput = new TextBox();
        private TextBox? _powerInput = new TextBox();
        private TextBlock? _expandedFormTextBlock;
        private TextBlock? _pascalsTriangleTextBlock;
        public BinomialExpansionWindow()
        {
            InitializeComponent();
            _expressionInput = this.FindControl<TextBox>("ExpressionInput") ?? throw new InvalidOperationException("ExpressionInput not found");
            _powerInput = this.FindControl<TextBox>("PowerInput") ?? throw new InvalidOperationException("PowerInput not found");
            _expandedFormTextBlock = this.FindControl<TextBlock>("ExpandedFormTextBlock")  ?? new TextBlock();
            _pascalsTriangleTextBlock = this.FindControl<TextBlock>("PascalsTriangleTextBlock") ?? new TextBlock();
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
        private void OnClearClick(object sender, RoutedEventArgs e)
        {
            _expressionInput?.Clear();
            _powerInput?.Clear();
            if(_expandedFormTextBlock != null && _pascalsTriangleTextBlock != null)
            {
                _expandedFormTextBlock.Text = "Result will be shown here";
                _pascalsTriangleTextBlock.Text="";
            }
        }       

        private void OnCalculateClick(object sender, RoutedEventArgs e)
        {
            
            string expression = _expressionInput?.Text ?? string.Empty;
            int power = int.Parse(_powerInput?.Text ?? "0");

            string expandForm = GetBinomialExpansion(expression, power);
            string pascalsTriangle = GetPascalsTriangle(power);

            if(_expandedFormTextBlock != null && _pascalsTriangleTextBlock != null){
                _expandedFormTextBlock.Text = expandForm;
                _pascalsTriangleTextBlock.Text = pascalsTriangle;
            }
            
        }

        private string GetBinomialExpansion(string expression, int power)
        {

            var match = Regex.Match(expression, @"(?<a>[+-]?\d*)?(?<var1>[a-z])?(?<b>[+-]\d*)?(?<var2>[a-z])?");
            if (!match.Success) throw new ArgumentException("Invalid expression format.");

            int a = string.IsNullOrEmpty(match.Groups["a"].Value) ? 1 : int.Parse(match.Groups["a"].Value);
            string var1 = match.Groups["var1"].Value;
            int b = string.IsNullOrEmpty(match.Groups["b"].Value) ? 0 : int.Parse(match.Groups["b"].Value);
            string var2 = match.Groups["var2"].Value;

             if (string.IsNullOrEmpty(var1)) throw new ArgumentException("Expression must contain at least one variable.");
            // Pascal's triangle coefficient to generate binomial expansion

            StringBuilder expansion = new StringBuilder();

            for (int i = 0; i<=power;i++)
            {
                 long coefficient = GetPascalCoefficient(power, i) * (int)Math.Pow(a, power - i) * (int)Math.Pow(b, i);
                if (coefficient == 0) continue;

                if (expansion.Length > 0 && coefficient > 0) expansion.Append(" + ");
                if (coefficient < 0) expansion.Append(" - ");

                if (Math.Abs(coefficient) != 1 || (i == power && string.IsNullOrEmpty(var1)))
                    expansion.Append(Math.Abs(coefficient));

                if (!string.IsNullOrEmpty(var1) && power - i > 0)
                {
                    expansion.Append(var1);
                    if (power - i > 1) expansion.Append($"^{power - i}");
                }

                if (!string.IsNullOrEmpty(var2) && i > 0)
                {
                    expansion.Append(var2);
                    if (i > 1) expansion.Append($"^{i}");
                }
            }

            return expansion.ToString().TrimEnd(' ', '+');
        }

        private string GetPascalsTriangle(int power)
        {
            StringBuilder triangle = new StringBuilder();
            int maxWidth = (power + 1) * 4;
            for(int row = 0; row <= power; row++)
            {
                StringBuilder rowString = new StringBuilder();

                for(int col = 0; col <= row; col++)
                {
                    int coefficient = GetPascalCoefficient(row,col);
                    rowString.Append(coefficient + " ");
                }
                string rowFormatted = rowString.ToString().Trim();
                int spacesToPad = Math.Max(0, (maxWidth - rowFormatted.Length) / 2);
                triangle.AppendLine(new string(' ', spacesToPad) + rowFormatted);
            }

            return triangle.ToString();
        }
        private  int GetPascalCoefficient(int row, int col)
        {
            if(col == 0 || col == row)
            {
                return 1;
            }
            return GetPascalCoefficient(row - 1, col - 1 ) + GetPascalCoefficient(row - 1, col);
        }
    }
}
