using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class StringCalculatorTest
{
	[Test]
	public void StringCalculatorTestSample()
	{
		var stringCalculator = new StringCalculator();
		{
			var val = stringCalculator.Calc("1");
			Assert.That(val, Is.EqualTo(1));
		}
		{
			var val = stringCalculator.Calc("(1)");
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
			var val = stringCalculator.Calc("(1+1)*(2+2)*(3+3)");
			Assert.That(val, Is.EqualTo(48));
		}
		{
			var val = stringCalculator.Calc("(1+(2*(3+3)))");
			Assert.That(val, Is.EqualTo(13));
		}
	}
}
