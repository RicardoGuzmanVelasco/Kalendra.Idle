using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace Kalendra.Idle.Runtime
{
    public readonly struct Money : IEquatable<Money>
    {
        readonly Dictionary<string, int> factors;
        public IReadOnlyDictionary<string, int> Factors => factors;
        
        Money(double amount)
        {
            factors = new Dictionary<string, int>();
            Factor(amount);
        }

        void Factor(double amount)
        {
            if(amount < 1)
                return;
            
            var closestPrefix = Prefixes.ClosestLowerThan(amount);
            factors[closestPrefix] = Prefixes.ToUnits(amount, closestPrefix);
            
            Factor(amount - Reduce());
        }

        public double Reduce()
        {
            if(!factors.Any())
                return 0;

            return factors.Sum(factor => Prefixes.ToNumber(factor.Key) * factor.Value);
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
        static class Prefixes
        {
            static IEnumerable<string> OrderedPrefixes
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
            
            internal static string ClosestLowerThan(double number)
            {
                var lastPrefix = "";
                foreach(var prefix in OrderedPrefixes)
                    if(ToNumber(prefix) > number)
                        return lastPrefix;
                    else
                        lastPrefix = prefix;

                throw new ArgumentOutOfRangeException();
            }

            internal static int ToUnits(double amount, string prefix)
            {
                Debug.Assert(amount / ToNumber(prefix) <= int.MaxValue);

                return (int) (amount / ToNumber(prefix));
            }

            internal static double ToNumber(string prefix)
            {
                Debug.Assert(OrderedPrefixes.Contains(prefix));

                var prefixes = OrderedPrefixes.TakeWhile(p => p != prefix);
                return Math.Pow(10, prefixes.Count() * 3);
            }
        }
        #endregion
    }
}