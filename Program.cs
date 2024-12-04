using System;
using System.Text.RegularExpressions;
using Gtk;

class BinomialExpansionApp : Window
{
    private Entry entryBinomial;
    private SpinButton spinPower;
    private TextView textViewOutput;

    public BinomialExpansionApp() : base("Binomial Expansion Calculator")
    {
        // Window settings
        SetDefaultSize(600, 400);
        SetPosition(WindowPosition.Center);

        // Main layout
        VBox vbox = new VBox(false, 5);

        // Binomial input
        HBox hboxBinomial = new HBox(false, 5);
        hboxBinomial.PackStart(new Label("Binomial (e.g., 2x+3, 2x+5y):"), false, false, 0);
        entryBinomial = new Entry();
        hboxBinomial.PackStart(entryBinomial, true, true, 0);
        vbox.PackStart(hboxBinomial, false, false, 0);

        // Power input
        HBox hboxPower = new HBox(false, 5);
        hboxPower.PackStart(new Label("Power:"), false, false, 0);
        spinPower = new SpinButton(0, 100, 1);
        spinPower.Value = 1; // Default value
        hboxPower.PackStart(spinPower, true, true, 0);
        vbox.PackStart(hboxPower, false, false, 0);

        // Buttons
        HBox hboxButtons = new HBox(false, 5);
        Button btnCalculate = new Button("Calculate");
        btnCalculate.Clicked += OnCalculateClicked;
        hboxButtons.PackStart(btnCalculate, false, false, 0);

        Button btnClear = new Button("Clear All");
        btnClear.Clicked += OnClearClicked;
        hboxButtons.PackStart(btnClear, false, false, 0);

        vbox.PackStart(hboxButtons, false, false, 0);

        // Output area
        textViewOutput = new TextView();
        textViewOutput.Editable = false;
        textViewOutput.WrapMode = WrapMode.Word;
        vbox.PackStart(new ScrolledWindow { textViewOutput }, true, true, 0);

        Add(vbox);
        ShowAll();
    }

    private void OnCalculateClicked(object sender, EventArgs e)
    {
        string binomial = entryBinomial.Text.Trim();
        int power = (int)spinPower.Value;

        try
        {
            // Parse the binomial to handle either "ax+b" or "ax+by"
            string expansion = string.Empty;
            var singleVariableRegex = new Regex(@"(?<coef1>[-+]?\d*\.?\d*)(?<var1>[a-z])(?<sign>[-+])(?<coef2>\d+)");
            var multiVariableRegex = new Regex(@"(?<coef1>[-+]?\d*\.?\d*)(?<var1>[a-z])(?<sign>[-+])(?<coef2>\d*\.?\d*)(?<var2>[a-z])");

            if (multiVariableRegex.IsMatch(binomial))
            {
                var match = multiVariableRegex.Match(binomial);
                double coef1 = string.IsNullOrWhiteSpace(match.Groups["coef1"].Value) ? 1 : double.Parse(match.Groups["coef1"].Value);
                double coef2 = string.IsNullOrWhiteSpace(match.Groups["coef2"].Value) ? 1 : double.Parse(match.Groups["coef2"].Value);
                if (match.Groups["sign"].Value == "-") coef2 = -coef2;

                string var1 = match.Groups["var1"].Value;
                string var2 = match.Groups["var2"].Value;

                // Generate Pascal's Triangle
                int[,] pascalTriangle = GeneratePascalsTriangle(power + 1);
                string pascalTriangleStr = FormatPascalsTriangle(pascalTriangle, power);

                // Compute Expansion
                expansion = ComputeMultiVariableExpansion(pascalTriangle, power, coef1, coef2, var1, var2);

                // Display output
                textViewOutput.Buffer.Text = $"Binomial: ({binomial})^{power}\n\nPascal's Triangle:\n{pascalTriangleStr}\n\nExpansion:\n{expansion}";
            }
            else if (singleVariableRegex.IsMatch(binomial))
            {
                var match = singleVariableRegex.Match(binomial);
                double coef1 = string.IsNullOrWhiteSpace(match.Groups["coef1"].Value) ? 1 : double.Parse(match.Groups["coef1"].Value);
                double coef2 = double.Parse(match.Groups["coef2"].Value);
                if (match.Groups["sign"].Value == "-") coef2 = -coef2;

                string var1 = match.Groups["var1"].Value;

                // Generate Pascal's Triangle
                int[,] pascalTriangle = GeneratePascalsTriangle(power + 1);
                string pascalTriangleStr = FormatPascalsTriangle(pascalTriangle, power);

                // Compute Expansion
                expansion = ComputeSingleVariableExpansion(pascalTriangle, power, coef1, coef2, var1);

                // Display output
                textViewOutput.Buffer.Text = $"Binomial: ({binomial})^{power}\n\nPascal's Triangle:\n{pascalTriangleStr}\n\nExpansion:\n{expansion}";
            }
            else
            {
                throw new Exception("Invalid binomial format. Use 'ax+b' or 'ax+by'.");
            }
        }
        catch (Exception ex)
        {
            textViewOutput.Buffer.Text = $"Error: {ex.Message}";
        }
    }

    private void OnClearClicked(object sender, EventArgs e)
    {
        entryBinomial.Text = string.Empty;
        spinPower.Value = 1;
        textViewOutput.Buffer.Text = string.Empty;
    }

    private int[,] GeneratePascalsTriangle(int rows)
    {
        int[,] triangle = new int[rows, rows];
        for (int n = 0; n < rows; n++)
        {
            triangle[n, 0] = 1;
            for (int k = 1; k <= n; k++)
            {
                triangle[n, k] = triangle[n - 1, k - 1] + triangle[n - 1, k];
            }
        }
        return triangle;
    }

    private string FormatPascalsTriangle(int[,] triangle, int rows)
    {
        string result = string.Empty;
        for (int i = 0; i <= rows; i++)
        {
            result += new string(' ', (rows - i) * 2); // Center align
            for (int j = 0; j <= i; j++)
            {
                result += $"{triangle[i, j]} ";
            }
            result += "\n";
        }
        return result;
    }

    private string ComputeSingleVariableExpansion(int[,] pascalTriangle, int n, double a, double b, string var1)
    {
        string expansion = string.Empty;
        for (int i = 0; i <= n; i++)
        {
            int coefficient = pascalTriangle[n, i];
            double termCoefficient = coefficient * Math.Pow(a, n - i) * Math.Pow(b, i);

            if (i > 0 && termCoefficient > 0)
                expansion += " + ";
            else if (termCoefficient < 0)
                expansion += " - ";

            expansion += Math.Abs(termCoefficient);

            if (n - i > 0)
            {
                expansion += var1;
                if (n - i > 1)
                    expansion += $"^{n - i}";
            }
        }

        return expansion;
    }

    private string ComputeMultiVariableExpansion(int[,] pascalTriangle, int n, double a, double b, string var1, string var2)
    {
        string expansion = string.Empty;
        for (int i = 0; i <= n; i++)
        {
            int coefficient = pascalTriangle[n, i];
            double termCoefficient = coefficient * Math.Pow(a, n - i) * Math.Pow(b, i);

            if (i > 0 && termCoefficient > 0)
                expansion += " + ";
            else if (termCoefficient < 0)
                expansion += " - ";

            expansion += Math.Abs(termCoefficient);

            if (n - i > 0)
            {
                expansion += var1;
                if (n - i > 1)
                    expansion += $"^{n - i}";
            }

            if (i > 0)
            {
                expansion += var2;
                if (i > 1)
                    expansion += $"^{i}";
            }
        }

        return expansion;
    }

    public static void Main()
    {
        Application.Init();
        new BinomialExpansionApp();
        Application.Run();
    }
}
