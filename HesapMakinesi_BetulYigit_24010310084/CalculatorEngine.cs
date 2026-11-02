
using System.Globalization;

namespace HesapMakinesi_BetulYigit_24010310084;

/// <summary>
/// Matematiksel ifadeleri ayrıştırır ve hesaplar.
/// +, -, *, /, ^, parantez ve bilimsel fonksiyonları destekler.
/// Trigonometrik işlemler derece cinsindendir.
/// </summary>
public static class CalculatorEngine
{
    public static double Evaluate(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            throw new ArgumentException("Lütfen bir işlem girin.");

        var parser = new ExpressionParser(expression);
        double result = parser.Parse();

        if (double.IsNaN(result) || double.IsInfinity(result))
            throw new ArithmeticException("Geçersiz matematiksel sonuç.");

        return result;
    }

    private sealed class ExpressionParser
    {
        private readonly string _text;
        private int _position;

        public ExpressionParser(string text)
        {
            _text = text.Replace(',', '.');
        }

        public double Parse()
        {
            double result = ParseExpression();
            SkipWhiteSpace();

            if (_position < _text.Length)
                throw new FormatException("İfade doğru biçimde değil.");

            return result;
        }

        private double ParseExpression()
        {
            double value = ParseTerm();

            while (true)
            {
                SkipWhiteSpace();

                if (Match('+'))
                    value += ParseTerm();
                else if (Match('-'))
                    value -= ParseTerm();
                else
                    return value;
            }
        }

        private double ParseTerm()
        {
            double value = ParseUnary();

            while (true)
            {
                SkipWhiteSpace();

                if (Match('*'))
                {
                    value *= ParseUnary();
                }
                else if (Match('/'))
                {
                    double divisor = ParseUnary();

                    if (Math.Abs(divisor) < 1e-15)
                        throw new DivideByZeroException(
                            "Sıfıra bölme işlemi yapılamaz.");

                    value /= divisor;
                }
                else
                {
                    return value;
                }
            }
        }

        private double ParseUnary()
        {
            SkipWhiteSpace();

            if (Match('+'))
                return ParseUnary();

            if (Match('-'))
                return -ParseUnary();

            return ParsePower();
        }

        private double ParsePower()
        {
            double value = ParsePrimary();

            SkipWhiteSpace();

            if (Match('^'))
                value = Math.Pow(value, ParseUnary());

            return value;
        }

        private double ParsePrimary()
        {
            SkipWhiteSpace();

            if (Match('('))
            {
                double value = ParseExpression();

                SkipWhiteSpace();

                if (!Match(')'))
                    throw new FormatException("Kapanış parantezi eksik.");

                return value;
            }

            if (_position < _text.Length &&
                (char.IsDigit(_text[_position]) ||
                 _text[_position] == '.'))
            {
                return ParseNumber();
            }

            if (_position < _text.Length &&
                char.IsLetter(_text[_position]))
            {
                string name = ParseIdentifier().ToLowerInvariant();

                if (name == "pi")
                    return Math.PI;

                if (name == "e")
                    return Math.E;

                SkipWhiteSpace();

                if (!Match('('))
                    throw new FormatException(
                        "Fonksiyon için parantez açılmalı.");

                double argument = ParseExpression();

                SkipWhiteSpace();

                if (!Match(')'))
                    throw new FormatException(
                        "Fonksiyon parantezi kapatılmalı.");

                return ApplyFunction(name, argument);
            }

            throw new FormatException("Geçersiz ifade.");
        }

        private double ParseNumber()
        {
            int start = _position;
            bool hasDot = false;

            while (_position < _text.Length)
            {
                char c = _text[_position];

                if (char.IsDigit(c))
                {
                    _position++;
                }
                else if (c == '.' && !hasDot)
                {
                    hasDot = true;
                    _position++;
                }
                else
                {
                    break;
                }
            }

            string numberText = _text[start.._position];

            if (!double.TryParse(
                numberText,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double value))
            {
                throw new FormatException("Sayı okunamadı.");
            }

            return value;
        }

        private string ParseIdentifier()
        {
            int start = _position;

            while (_position < _text.Length &&
                   char.IsLetter(_text[_position]))
            {
                _position++;
            }

            return _text[start.._position];
        }

        private double ApplyFunction(string name, double x)
        {
            return name switch
            {
                "sqrt" when x >= 0 => Math.Sqrt(x),
                "sqrt" => throw new ArithmeticException(
                    "Negatif sayının gerçek karekökü alınamaz."),

                "sin" => Math.Sin(ToRadians(x)),
                "cos" => Math.Cos(ToRadians(x)),
                "tan" => Math.Abs(Math.Cos(ToRadians(x))) < 1e-12
                    ? throw new ArithmeticException(
                        "Bu açı için tanjant tanımsızdır.")
                    : Math.Tan(ToRadians(x)),

                "log" when x > 0 => Math.Log10(x),
                "ln" when x > 0 => Math.Log(x),

                "log" or "ln" => throw new ArithmeticException(
                    "Logaritma için sayı sıfırdan büyük olmalıdır."),

                "abs" => Math.Abs(x),

                _ => throw new FormatException(
                    $"Bilinmeyen fonksiyon: {name}")
            };
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        private bool Match(char expected)
        {
            if (_position < _text.Length &&
                _text[_position] == expected)
            {
                _position++;
                return true;
            }

            return false;
        }

        private void SkipWhiteSpace()
        {
            while (_position < _text.Length &&
                   char.IsWhiteSpace(_text[_position]))
            {
                _position++;
            }
        }
    }
}
