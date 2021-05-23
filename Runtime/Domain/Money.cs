using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace Kalendra.Idle.Runtime
{
    public readonly struct Money : IEquatable<Money>
    {
        readonly Dictionary<string, int> factors;
        
        Money(double amount)
        {
            factors = new Dictionary<string, int>();
            Factorize(amount);
        }

        void Factorize(double amount)
        {
            if(amount < 1)
                return;
            
            var closestPrefix = Prefix.ClosestLowerThan(amount);
            var factor = (int) (amount / closestPrefix);
            factors[closestPrefix] = factor;
            
            Factorize(amount - closestPrefix * factor);
        }

        public double Reduce()
        {
            if(!factors.Any())
                return 0;

            return factors.Sum(factor => factor.Value * Prefix.From(factor.Key));
        }

        #region Factory Methods/Properties
        public static Money Zero => From(0);
        
        public static Money From(double reduction) => new Money(reduction);
        #endregion

        #region Operator overloading
        public static Money operator +(Money m1, Money m2)
        {
            return From(m1.Reduce() + m2.Reduce());
        }
        
        public static Money operator *(Money m, double factor)
        {
            return From(m.Reduce() * factor);
        }
        #endregion
        
        #region Equality
        public bool Equals(Money other) => other.Reduce().Equals(Reduce());
        public override bool Equals(object other) => other is Money o && Equals(o);
        public override int GetHashCode() => Reduce().GetHashCode();

        public static bool operator ==(Money m1, Money m2) => m1.Equals(m2);
        public static bool operator !=(Money m1, Money m2) => !(m1 == m2);
        #endregion

        #region Formatting members
        public override string ToString() => $"[{Reduce()}]";
        #endregion

        #region Prefixes management
        readonly struct Prefix
        {
            readonly string symbol;
            readonly int base10;

            Prefix(string symbol)
            {
                Debug.Assert(Symbols.Contains(symbol));
                
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
                    if(Prefix.From(symbol) > number)
                        return Prefix.From(lastPrefix);
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
        #endregion
    }
}