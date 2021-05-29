using System;
using System.Globalization;
using System.Linq;

namespace Kalendra.Idle.Runtime
{
    public readonly partial struct Money
    {
        static class Parser
        {
            public static string Serialize(Money money)
            {
                var symbol = money.factors.Keys.Max(Prefix.From);
            
                var number = (money.Reduce() / symbol).ToString(CultureInfo.InvariantCulture);

                while(number.Replace(".", "").Length > 3)
                    number = number.Remove(number.Length - 1);
            
                if(number.Last() == '.')
                    number = number.Remove(number.Length - 1);
            
                if(number.Contains('.') && number.Last() == '0')
                    number = number.Remove(number.Length - 1);

                return number + symbol;
            }

            public static Money Deserialize(string serialized)
            {
                var number = DeserializeNumber(serialized);
                var symbol = DeserializeSymbol(serialized);

                return From(number, symbol);
            }

            #region Support methods
            static double DeserializeNumber(string source)
            {
                var symbolPart = DeserializeSymbol(source);
            
                var numberPart = symbolPart.Any() ? source.Replace(symbolPart, "") : source;
                if(IsWrongNumberFormat(numberPart))
                    throw new FormatException($"{numberPart} is not a well-formatted number");

                return double.Parse(numberPart, CultureInfo.InvariantCulture);
            }

            static bool IsWrongNumberFormat(string numberPart)
            {
                return numberPart.Count(char.IsPunctuation) > 1 ||
                       numberPart.Any(c => !char.IsDigit(c) && !char.IsPunctuation(c));
            }

            static string DeserializeSymbol(string source)
            {
                var symbol = "";

                for(var i = source.Length - 1; i >= 0 && !char.IsDigit(source[i]); i--)
                    symbol = source[i] + symbol;

                return symbol;
            }
            #endregion
        }
    }
}