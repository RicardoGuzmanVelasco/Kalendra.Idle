using System;
using System.Globalization;
using System.Linq;

namespace Kalendra.Idle.Runtime
{
    public readonly partial struct Money
    {
        static class Parser
        {
            const int MaxSerializedDigits = 3;
            
            public static string Serialize(Money money)
            {
                var prefix = GetMaxPrefix(money);
            
                var number = ReduceNumberToPrefix(money, prefix);
                number = LimitToMaxDigits(number);
            
                number = CleanTailingFloatingPoint(number);
                number = CleanTailingFloatingZero(number);

                return number + prefix;
            }

            #region Support methods
            static Prefix GetMaxPrefix(Money money)
            {
                return money.factors.Keys.Max(Prefix.From);
            }

            static string ReduceNumberToPrefix(Money money, Prefix symbol)
            {
                return (money.Reduce() / symbol).ToString(CultureInfo.InvariantCulture);
            }

            static string LimitToMaxDigits(string number)
            {
                while(number.Replace(".", "").Length > MaxSerializedDigits)
                    number = number.Remove(number.Length - 1);
                return number;
            }

            static string CleanTailingFloatingZero(string number)
            {
                if(number.Contains('.') && number.Last() == '0')
                    number = number.Remove(number.Length - 1);
                return number;
            }

            static string CleanTailingFloatingPoint(string number)
            {
                if(number.Last() == '.')
                    number = number.Remove(number.Length - 1);
                return number;
            }
            #endregion

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