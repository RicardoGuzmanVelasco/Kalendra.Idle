namespace Kalendra.Idle.Runtime
{
    public struct Money
    {
        readonly double value;
        
        public Money(int amount)
        {
            value = amount;
        }

        public double Reduce()
        {
            return value;
        }

        public static Money From(double reduction)
        {
            return new Money((int)reduction);
        }

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

        #region Formatting members
        public override string ToString() => $"[{value}]";
        #endregion
    }
}