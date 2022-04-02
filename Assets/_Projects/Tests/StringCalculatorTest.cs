using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class StringCalculatorTest
{
	[Test]
	public void StringCalculatorTestSample()
	{
		var stringCalculator = new StringCalculatorOld();
		{
			var val = stringCalculator.Calc("1");
			Assert.That(val, Is.EqualTo(1));
		}
		{
			var val = stringCalculator.Calc("(1)");
			Assert.That(val, Is.EqualTo(1));
		}
		{
			var val = stringCalculator.Calc("((1))");
			Assert.That(val, Is.EqualTo(1));
		}
		{
			var val = stringCalculator.Calc("1+2");
			Assert.That(val, Is.EqualTo(3));
		}
		{
			var val = stringCalculator.Calc("1+2+3");
			Assert.That(val, Is.EqualTo(6));
		}
		{
			var val = stringCalculator.Calc("(1)+(2)+(3)");
			Assert.That(val, Is.EqualTo(6));
		}
		{
			var val = stringCalculator.Calc("(1+2+3)");
			Assert.That(val, Is.EqualTo(6));
		}
		{
			var val = stringCalculator.Calc("(1+1)*(2+2)*(3+3)");
			Assert.That(val, Is.EqualTo(48));
		}
		{
			var val = stringCalculator.Calc("(1+(2*(3+3)))");
			Assert.That(val, Is.EqualTo(13));
		}
		{
			var val = stringCalculator.Calc("0.1+0.2+0.3");
			Assert.That(val, Is.EqualTo(0.6f));
		}
		{
			var val = stringCalculator.Calc(".1+.2*.3");
			Assert.That(val, Is.EqualTo(0.09f));
		}
	}
}
