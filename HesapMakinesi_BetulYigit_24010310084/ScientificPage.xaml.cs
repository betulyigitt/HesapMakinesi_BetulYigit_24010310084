
namespace HesapMakinesi_BetulYigit_24010310084;

public partial class ScientificPage : ContentPage
{
    private string _expression = "";
    private bool _resultShown = false;

    public ScientificPage()
    {
        InitializeComponent();
    }

    private void OnButtonClicked(object sender, EventArgs e)
    {
        if (sender is not Button button)
            return;

        string value = button.Text;

        switch (value)
        {
            case "C":
                _expression = "";
                _resultShown = false;
                break;

            case "⌫":
                if (_expression.Length > 0)
                    _expression = _expression[..^1];

                _resultShown = false;
                break;

            case "=":
                Calculate();
                return;

            case "sin":
            case "cos":
            case "tan":
            case "log":
            case "ln":
            case "abs":
                PrepareForNewInput();
                _expression += value + "(";
                break;

            case "√":
                PrepareForNewInput();
                _expression += "sqrt(";
                break;

            case "π":
                PrepareForNewInput();
                _expression += "pi";
                break;

            case "e":
                PrepareForNewInput();
                _expression += "e";
                break;

            case "÷":
                AddOperator("/");
                break;

            case "×":
                AddOperator("*");
                break;

            case "−":
                AddOperator("-");
                break;

            case "+":
            case "^":
                AddOperator(value);
                break;

            case ".":
                PrepareForNewInput();
                AddDecimalPoint();
                break;

            default:
                PrepareForNewInput();
                _expression += value;
                break;
        }

        UpdateDisplay();
    }

    private void PrepareForNewInput()
    {
        if (_resultShown)
        {
            _expression = "";
            _resultShown = false;
        }
    }

    private void AddOperator(string op)
    {
        if (_resultShown)
            _resultShown = false;

        if (_expression.Length == 0)
        {
            if (op == "-")
                _expression = "-";

            return;
        }

        char last = _expression[^1];

        if ("+-*/^".Contains(last))
        {
            if (op == "-" && last != '-')
                _expression += op;
            else
                _expression = _expression[..^1] + op;
        }
        else
        {
            _expression += op;
        }
    }

    private void AddDecimalPoint()
    {
        int lastOperator = Math.Max(
            Math.Max(_expression.LastIndexOf('+'),
                     _expression.LastIndexOf('-')),
            Math.Max(_expression.LastIndexOf('*'),
                     _expression.LastIndexOf('/')));

        lastOperator = Math.Max(
            lastOperator, _expression.LastIndexOf('^'));

        string currentNumber = _expression[(lastOperator + 1)..];

        if (!currentNumber.Contains('.'))
            _expression += ".";
    }

    private void Calculate()
    {
        try
        {
            double result = CalculatorEngine.Evaluate(_expression);

            _expression = result.ToString(
                "G15",
                System.Globalization.CultureInfo.InvariantCulture);

            _resultShown = true;
            UpdateDisplay();
        }
        catch (Exception ex)
        {
            DisplayEntry.Text = ex.Message;
            _expression = "";
            _resultShown = false;
        }
    }

    private void UpdateDisplay()
    {
        DisplayEntry.Text = string.IsNullOrEmpty(_expression)
            ? "0"
            : _expression;
    }
}
