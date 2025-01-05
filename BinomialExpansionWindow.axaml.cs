using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using System;
using System.Text;
using System.Numerics;
using System.Text.RegularExpressions;


namespace BiCal
{
    public partial class BinomialExpansionWindow : Window
    {
        private TextBox? _expressionInput = new TextBox();
        private TextBlock? _expandedFormTextBlock;
        private TextBlock? _pascalsTriangleTextBlock;
        public BinomialExpansionWindow()
        {
            InitializeComponent();
            _expressionInput = this.FindControl<TextBox>("ExpressionInput") ?? throw new InvalidOperationException("ExpressionInput not found");
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
            if(_expandedFormTextBlock != null && _pascalsTriangleTextBlock != null)
            {
                _expandedFormTextBlock.Text = "Result will be shown here";
                _pascalsTriangleTextBlock.Text="";
            }
        }       

        private void OnCalculateClick(object sender, RoutedEventArgs e)
        {
            
            string expression = _expressionInput?.Text ?? string.Empty;
            int exponent = ExtractExponent(expression);
            string expandForm = GetBinomialExpansion(expression);
            string pascalsTriangle = GetPascalsTriangle(exponent);
            Console.WriteLine(GetBinomialExpansion(expression));
            if(_expandedFormTextBlock != null && _pascalsTriangleTextBlock != null){
                _expandedFormTextBlock.Text = expandForm;
                _pascalsTriangleTextBlock.Text = pascalsTriangle;
            }
            
        }
        private void OnCopyClick(object sender, RoutedEventArgs e){
            var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
            if(!string.IsNullOrWhiteSpace(_expandedFormTextBlock?.Text)){
                clipboard?.SetTextAsync(_expandedFormTextBlock.Text);
            }
        }
        private string GetBinomialExpansion(string expression)
        {
            (int a, int b, string var1, string var2) = ExtractComponents(expression);
            int power = ExtractExponent(expression);
    
            if (string.IsNullOrEmpty(var1)) throw new ArgumentException("Expression must contain at least one variable.");

            StringBuilder expansion = new StringBuilder();

            for (int i = 0; i <= power; i++)
            {
                // BigInteger is used for handling large numbers
                BigInteger coefficient = GetPascalCoefficient(power, i) * (int)Math.Pow(a, power - i) * (int)Math.Pow(b, i);
        
                if (coefficient == 0) continue;

                // Formatting the sign of the terms
                if (expansion.Length > 0 && coefficient > 0) expansion.Append(" + ");
                if (coefficient < 0) expansion.Append(" - ");

                // Append coefficient if it's not 1 or -1
                if (BigInteger.Abs(coefficient) != 1 || (i == power && string.IsNullOrEmpty(var1)))
                    expansion.Append(BigInteger.Abs(coefficient));

                // Append the first variable (if any) with appropriate exponent
                if (!string.IsNullOrEmpty(var1) && power - i > 0)
                {
                    expansion.Append(var1);
                    if (power - i > 1) expansion.Append($"^{power - i}");
                }

                // Append the second variable (if any) with appropriate exponent
                if (!string.IsNullOrEmpty(var2) && i > 0)
                {
                    expansion.Append(var2);
                    if (i > 1) expansion.Append($"^{i}");
                }
                if (i==power && coefficient < 2 && string.IsNullOrEmpty(var2)){
                    expansion.Append(coefficient);
                }
                System.Console.WriteLine($"var1:{var1} var2:{var2} coefficient{coefficient} i={i}");
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
                    BigInteger coefficient = GetPascalCoefficient(row,col);
                    rowString.Append(coefficient + " ");
                }
                string rowFormatted = rowString.ToString().Trim();
                int spacesToPad = Math.Max(0, (maxWidth - rowFormatted.Length) / 2);
                triangle.AppendLine(new string(' ', spacesToPad) + rowFormatted);
            }

            return triangle.ToString();
        }
        private  BigInteger GetPascalCoefficient(int row, int col)
        {
            if(col == 0 || col == row)
            {
                return 1;
            }
            return GetPascalCoefficient(row - 1, col - 1 ) + GetPascalCoefficient(row - 1, col);
        }
        private (int, int, string, string) ExtractComponents(string expression){
            var matches = Regex.Match(expression, @"\(\s*(?<a>[+-]?\d*)?(?<var1>[a-z])?\s*(?<b>[+-]?\d*)?(?<var2>[a-z])?\s*\)");


            if (!matches.Success)
            {
                Console.WriteLine($"Expression: {expression}");
                throw new ArgumentException("Invalid expression format.");
            }

            // Extract base components
            int a = string.IsNullOrEmpty(matches.Groups["a"].Value) ? 1 : int.Parse(matches.Groups["a"].Value);
            string var1 = matches.Groups["var1"].Value;

            string bValue = matches.Groups[3].Value.Trim();
    int b = string.IsNullOrEmpty(bValue) ? 0 : int.Parse(bValue);
            string var2 = matches.Groups["var2"].Value;

            return (a, b, var1, var2);
        }
         private int ExtractExponent(string expression){
            var match = Regex.Match(expression, @"\^\s*(?<exponent>\d+)");
            if (match.Success){
                return int.Parse(match.Groups["exponent"].Value);
            }
            throw new ArgumentException("Invalid binomial expression. Ensure it is in the form (x + y)^n.");
        }
    }
}
