namespace Kalendra.Idle.Runtime
{
    public class Money
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
    }
}