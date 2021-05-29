using System;
using System.Collections.Generic;
using System.Linq;

namespace Kalendra.Idle.Runtime
{
    public readonly partial struct Money
    {
        readonly struct Prefix : IEquatable<Prefix>, IComparable<Prefix>, IComparable
        {
            readonly string symbol;
            readonly int base10;

            Prefix(string symbol)
            {
                if(!Symbols.Contains(symbol))
                    throw new ArgumentException($"Unknown prefix: {symbol}", nameof(symbol));

                this.symbol = symbol;

                var prefixes = Symbols.TakeWhile(p => p != symbol);
                base10 = prefixes.Count() * 3;
            }

            #region Factory methods
            public static Prefix From(string symbol) => new Prefix(symbol);

            public static Prefix ClosestLowerThan(double number)
            {
                var lastPrefix = "";
                foreach(var symbol in Symbols)
                    if(From(symbol) > number)
                        return From(lastPrefix);
                    else
                        lastPrefix = symbol;

                throw new ArgumentOutOfRangeException();
            }
            #endregion

            #region Conversion operators
            public static implicit operator double(Prefix p) => Math.Pow(10, p.base10);
            public static implicit operator string(Prefix p) => p.symbol;
            #endregion

            #region Equality
            public bool Equals(Prefix other) => symbol == other.symbol;
            public override bool Equals(object other) => other is Prefix o && Equals(o);
            public override int GetHashCode() => symbol != null ? symbol.GetHashCode() : 0;

            public static bool operator ==(Prefix p1, Prefix p2) => p1.Equals(p2);
            public static bool operator !=(Prefix p1, Prefix p2) => !(p1 == p2);

            public int CompareTo(Prefix other) => base10.CompareTo((int) other.base10);
            public int CompareTo(object other) => base10.CompareTo(other);
            #endregion

            #region Formatting members
            public override string ToString() => symbol;
            #endregion

            static IEnumerable<string> Symbols
            {
                get
                {
                    yield return "";
                    yield return "k";
                    yield return "M";
                    yield return "B";
                    yield return "T";
                    yield return "aa";
                    yield return "ab";
                    yield return "ac";
                    yield return "ad";
                }
            }
        }
    }
}