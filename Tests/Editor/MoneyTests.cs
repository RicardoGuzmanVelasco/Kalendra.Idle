using FluentAssertions;
using Kalendra.Idle.Runtime;
using NUnit.Framework;

namespace Kalendra.Idle.Tests.Editor
{
    public class MoneyTests
    {
        [Theory, TestCase(0), TestCase(2)]
        public void Money_Reduces_ToDouble(double source)
        {
            var sut = Money.From(source);

            var result = sut.Reduce();

            result.Should().Be(source);
        }

        [Test]
        public void Money_FromDouble_ReducesToDouble()
        {
            var sut = Money.From(1.0);

            var result = sut.Reduce();

            result.Should().Be(1.0);
        }

        [Test]
        public void Money_AddingOperator_ReturnsMoney()
        {
            var sut1 = Money.From(1);
            var sut2 = Money.From(2);

            var result = sut1 + sut2;

            result.Should().Be(Money.From(3));
        }

        [Test]
        public void Money_MultiplyByNumber_ReturnsMoney()
        {
            var sut = Money.From(5);

            var result = sut * 2;

            result.Should().Be(Money.From(10));
        }

        [Theory, TestCase(0), TestCase(2), TestCase(5)]
        public void Money_AddingTwice_EqualsMultiplyBy2(double source)
        {
            var sut = Money.From(source);

            var result = sut + sut;
            
            result.Should().Be(sut * 2);
        }
        
        [Theory, TestCase(0), TestCase(2), TestCase(5)]
        public void Money_MultiplyByZero_IsZero(double source)
        {
            var sut = Money.From(source);

            var result = sut * 0;
            
            result.Should().Be(Money.Zero);
        }
        
        [Theory, TestCase(0), TestCase(2), TestCase(5)]
        public void MoneyZero_MultiplyByAnything_IsZero(double factor)
        {
            var sut = Money.Zero;

            var result = sut * factor;
            
            result.Should().Be(Money.Zero);
        }

        [Test]
        public void Money_Ignore_Decimals()
        {
            var sut = Money.From(0.1f);

            var result = sut.Reduce();

            result.Should().Be(0);
        }

        [Test]
        public void Money_GroupBy_Prefixes()
        {
            var sut = Money.From(1400);

            var result = sut.Factors;

            result.Should().HaveCount(2);
        }
    }
}