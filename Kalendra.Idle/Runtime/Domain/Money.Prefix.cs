using System;

namespace Kalendra.Idle.Runtime
{
    public readonly partial struct Money
    {
        readonly partial struct Prefix : IEquatable<Prefix>, IComparable<Prefix>, IComparable
        {
            readonly PrefixSymbol symbol;
            readonly int base10;

            Prefix(string symbol)
            {
                this.symbol = PrefixSymbol.From(symbol);
                base10 = PrefixSymbol.IndexOf(this.symbol) * 3;
            }

            #region Factory methods
            public static Prefix From(string symbol) => new Prefix(symbol);

            public static Prefix ClosestLowerThan(double number)
            {
                var prefixSymbol = PrefixSymbol.Empty;

                while(From(prefixSymbol) <= number)
                    prefixSymbol = prefixSymbol.Next();

                return From(prefixSymbol.Prev());
            }
            #endregion

            #region Conversion operators
            public static implicit operator double(Prefix p) => Math.Round(Math.Pow(10, p.base10));
            public static implicit operator string(Prefix p) => p.ToString();
            #endregion

            #region Equality
            public bool Equals(Prefix other) => symbol == other.symbol;
            public override bool Equals(object other) => other is Prefix o && Equals(o);
            public override int GetHashCode() => symbol != null ? symbol.GetHashCode() : 0;

            public static bool operator ==(Prefix p1, Prefix p2) => p1.Equals(p2);
            public static bool operator !=(Prefix p1, Prefix p2) => !(p1 == p2);

            public int CompareTo(Prefix other) => base10.CompareTo(other.base10);
            public int CompareTo(object other) => base10.CompareTo(other);
            #endregion

            #region Formatting members
            public override string ToString() => symbol;
            #endregion
        }
    }
}