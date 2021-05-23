using FluentAssertions;
using Kalendra.Idle.Runtime;
using NUnit.Framework;

namespace Kalendra.Idle.Tests.Editor
{
    public class MoneyTests
    {
        [Theory, TestCase(0), TestCase(1500)]
        public void Money_Reduces_ToDouble(double source)
        {
            var sut = Money.From(source);

            var result = sut.Reduce();

            result.Should().Be(source);
        }

        [Test, TestCase(1.0), TestCase(2.1)]
        public void Money_FromDouble_ReducesToDouble(double source)
        {
            var sut = Money.From(source);

            var result = sut.Reduce();

            result.Should().Be((int)source);
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
        public void MoneyZero_AbsorbingElement(double factor)
        {
            var sut = Money.Zero;

            var result = sut * factor;
            
            result.Should().Be(Money.Zero);
        }
        
        [Theory, TestCase(0), TestCase(2), TestCase(5)]
        public void MoneyZero_IdentityElement(int multipland)
        {
            var sut = Money.Zero;

            var result = sut + Money.From(multipland);
            
            result.Should().Be(Money.From(multipland));
        }

        [Test]
        public void Money_Ignore_Decimals()
        {
            var sut = Money.From(0.1f);

            var result = sut.Reduce();

            result.Should().Be(0);
        }

        [Test]
        public void Money_Equality()
        {
            var sut1 = Money.From(1);
            var sut2 = Money.From(1);

            var result = sut1 == sut2;

            result.Should().BeTrue();
        }

        [Test]
        public void Money_Equality_ToZero()
        {
            var sut = Money.From(0);

            var result = sut == Money.Zero;

            result.Should().BeTrue();
        }
    }
}