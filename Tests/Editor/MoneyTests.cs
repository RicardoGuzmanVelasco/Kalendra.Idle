using System;
using System.Linq;
using FluentAssertions;
using Kalendra.Idle.Runtime;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Kalendra.Idle.Tests.Editor
{
    public class MoneyTests
    {
        #region Construction/Preconditions
        [Test]
        public void Money_FromNegative_ThrowsException()
        {
            Action act = () => Money.From(-1);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        [TestCase(1, "k", 1e3)]
        [TestCase(9, "", 9)]
        [TestCase(3.54, "M", 3e6+540e3)]
        [TestCase(8.66, "", 8)]
        public void Money_FromPrefix(double reductionSource, string prefix, double reductionExpected)
        {
            var sut = Money.From(reductionSource, prefix);

            var result = sut.Reduce();

            result.Should().Be(reductionExpected);
        }

        [Test]
        [TestCase(" ")]
        [TestCase("x")]
        [TestCase("b")]
        public void Money_FromWrongPrefix_ThrowsException(string wrongPrefix)
        {
            Action act = () => Money.From(1, wrongPrefix);

            act.Should().Throw<ArgumentException>();
        }
        #endregion
        
        #region Reduction to double
        [Theory, TestCase(0), TestCase(50 + 20000 + 2e10)]
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
        #endregion

        #region Arithmetics (+, - and *)
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
        public void Money_Adding_ReturnsSameReduction()
        {
            var sut = Money.From(1000) + Money.From(400);

            var result = sut.Reduce();

            result.Should().BeApproximately(Money.From(1400).Reduce(), double.Epsilon);
        }

        [Test, TestCase(3, 2), TestCase(2e15, 17e11)]
        public void Money_Substract_SameAsReductionsSubstraction(double m1, double m2)
        {
            var sut1 = Money.From(m1);
            var sut2 = Money.From(m2);

            var result = sut1 - sut2;

            result.Should().Be(Money.From(m1 - m2));
        }

        [Test, TestCase(0), TestCase(1), TestCase(87e9)]
        public void Money_SubstractSame_IsMoneyZero(double m)
        {
            var sut = Money.From(m);

            var result = sut - sut;

            result.Should().Be(Money.Zero);
        }

        [Test]
        public void Money_AdditionAndMultiply_Precedence()
        {
            var expected = (Money.From(1000) + Money.From(600)) * 2;
            
            var sut = Money.From(1000) + Money.From(600);
            sut *= 2;

            sut.Should().Be(expected);
            sut.Should().Be(Money.From(3200));
        }

        [Test]
        public void Money_Arithmetics_Association()
        {
            var expected = Money.From(1400) * 2;

            var sut = Money.From(1000) * 2 + Money.From(400) * 2;

            sut.Should().Be(expected);
            sut.Should().Be(Money.From(2800));
        }

        [Test]
        public void Money_SubstractGreater_ThrowsException()
        {
            var sut1 = Money.From(1);
            var sut2 = Money.From(2);

            Money result;
            Action act = () => result = sut1 - sut2;

            act.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void MoneyNotZero_MultiplyByNegative_ThrowsException()
        {
            var sut = Money.From(1);

            Money result;
            Action act = () => result = sut * -1;

            act.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void MoneyZero_MultiplyByNegative_IsMoneyZero()
        {
            var result = Money.Zero * -1;

            result.Should().Be(Money.Zero);
        }
        #endregion
        
        #region Equality
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

        [Test]
        public void Money_Arithmethic_Equality()
        {
            var sut = Money.From(1000) + Money.From(400);

            var result = sut == Money.From(1400);

            result.Should().BeTrue();
        }
        #endregion
        
        #region Factorization
        [Test, TestCase(1), TestCase(2e3), TestCase(20e12)]
        public void Factorize_OneFactorMoney_IsSameMoney(double m)
        {
            var sut = Money.From(m);

            var result = sut.Factorize();

            result.Single().Should().Be(sut);
        }

        [Test]
        public void Factorize_ResultSum_IsSameMoney()
        {
            var sut = Money.From(2135403541);

            var sum = sut.Factorize().Sum(money => money.Reduce());
            var result = Money.From(sum);

            result.Should().Be(sut);
        }
        #endregion
        
        #region Rounding
        [Test]
        public void Round_MoneyZero_IsMoneyZero()
        {
            var sut = Money.Zero;

            var result = sut.Round();

            result.Should().Be(Money.Zero);
        }
        
        [Test, TestCase(1e3), TestCase(700e18)]
        public void Round_OneFactorMoney_IsSameMoney(double oneFactor)
        {
            var sut = Money.From(oneFactor);

            var result = sut.Round();

            result.Should().Be(sut);
        }
        
        [Test]
        public void Round_MoneyWithOnlyTwoAdjacentFactors_PurguesSmallestFactor()
        {
            var sut = Money.From(1000) + Money.From(809);

            var result = sut.Round();

            result.Should().Be(Money.From(1000));
        }

        [Test]
        public void Round_MoneyWithNonAdjacentFactors_PurguesSmallestFactors()
        {
            var sut = Money.From(45e6) + Money.From(23e3) + Money.From(7);

            var result = sut.Round();

            result.Should().Be(Money.From(45e6));
        }
        #endregion
    }
}