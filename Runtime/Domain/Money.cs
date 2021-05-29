using System;
using System.Collections.Generic;
using System.Linq;

namespace Kalendra.Idle.Runtime
{
    public readonly partial struct Money : IEquatable<Money>, IComparable<Money>, IComparable
    {
        readonly Dictionary<string, int> factors;

        #region Constructors
        Money(double amount, string prefixSymbol) : this(amount * Prefix.From(prefixSymbol)) { }
        
        Money(double amount)
        {
            if(amount < 0)
                throw new ArgumentOutOfRangeException();
            
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
        #endregion

        public double Reduce()
        {
            if(!factors.Any())
                return 0;

            return factors.Sum(factor => factor.Value * Prefix.From(factor.Key));
        }

        public IReadOnlyCollection<Money> Factorize()
        {
            var resultFactors = new List<Money>();

            foreach(var factor in factors)
                resultFactors.Add(From(factor.Value, factor.Key));

            return resultFactors;
        }

        public Money Round()
        {
            if(!factors.Any())
                return Zero;
            
            var maxPrefix = factors.Max(p => Prefix.From(p.Key));
            return new Money(maxPrefix * factors[maxPrefix]);
        }

        #region Factory Methods/Properties
        public static Money Zero => From(0);
        public static Money MaxValue => From(double.MaxValue);
        
        public static Money From(double reduction) => new Money(reduction);
        public static Money From(double reduction, string symbol) => new Money(reduction, symbol);

        public static Money From(string serialized) => Parser.Deserialize(serialized);
        public override string ToString() => Parser.Serialize(this);
        #endregion

        #region Operator overloading
        public static Money operator +(Money m1, Money m2)
        {
            return From(m1.Reduce() + m2.Reduce());
        }
        
        public static Money operator -(Money m1, Money m2)
        {
            if(m2 > m1)
                throw new InvalidOperationException($"Cannot substract a greater money: {m1} < {m2}");
            
            return From(m1.Reduce() - m2.Reduce());
        }
        
        public static Money operator *(Money m, double factor)
        {
            if(factor < 0 && m > Zero)
                throw new InvalidOperationException($"Cannot multiply positive money by negative factor: {factor}");
            
            return From(m.Reduce() * factor);
        }
        #endregion
        
        #region Equality/Comparation
        public bool Equals(Money other) => other.Reduce().Equals(Reduce());
        public override bool Equals(object other) => other is Money o && Equals(o);
        public override int GetHashCode() => Reduce().GetHashCode();

        public static bool operator ==(Money m1, Money m2) => m1.Equals(m2);
        public static bool operator !=(Money m1, Money m2) => !(m1 == m2);
        
        public int CompareTo(Money other) => Reduce().CompareTo(other.Reduce());
        public int CompareTo(object other) => Reduce().CompareTo(other);

        public static bool operator >(Money m1, Money m2) => m1.Reduce() > m2.Reduce();
        public static bool operator <(Money m1, Money m2) => !(m1 > m2 || m1 == m2);
        public static bool operator >=(Money m1, Money m2) => !(m1 < m2);
        public static bool operator <=(Money m1, Money m2) => !(m1 > m2);
        #endregion
    }
}