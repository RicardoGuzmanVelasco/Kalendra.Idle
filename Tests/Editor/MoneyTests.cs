using FluentAssertions;
using Kalendra.Idle.Runtime;
using NUnit.Framework;

namespace Kalendra.Idle.Tests.Editor
{
    public class MoneyTests
    {
        [Test, TestCase(0), TestCase(2)]
        public void Money_Reduces_ToDouble(double reduction)
        {
            var sut = new Money((int)reduction);

            var result = sut.Reduce();

            result.Should().Be(reduction);
        }

        [Test]
        public void Money_FromDouble_ReducesToDouble()
        {
            var sut = Money.From(1.0);

            var result = sut.Reduce();

            result.Should().Be(1.0);
        }
    }
}