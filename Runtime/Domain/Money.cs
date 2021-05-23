namespace Kalendra.Idle.Runtime
{
    public struct Money
    {
        readonly double value;
        
        Money(int amount) => value = amount;

        public double Reduce()
        {
            return value;
        }

        #region Factory Methods/Properties
        public static Money From(double reduction)
        {
            return new Money((int)reduction);
        }

        public static Money Zero => From(0);
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

        #region Formatting members
        public override string ToString() => $"[{value}]";
        #endregion
    }
}