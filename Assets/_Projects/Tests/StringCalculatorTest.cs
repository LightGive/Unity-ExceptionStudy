using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using StringCalculator;

public class StringCalculatorTest
{
	[Test]
	public void StringCalculatorTestSample()
	{
		var parser = new ParserFormula();
		var stringCalculator = new StringCalculatorOld();
		{
			var val = Calculator.Calc("1", parser);
			Assert.That(val, Is.EqualTo(1));
		}
		{
			var val = Calculator.Calc("(1)", parser);
			Assert.That(val, Is.EqualTo(1));
		}
		{
			var val = Calculator.Calc("((1))", parser);
			Assert.That(val, Is.EqualTo(1));
		}
		{
			var val = Calculator.Calc("1+2", parser);
			Assert.That(val, Is.EqualTo(3));
		}
		{
			var val = Calculator.Calc("1+2+3", parser);
			Assert.That(val, Is.EqualTo(6));
		}
		{
			var val = Calculator.Calc("(1)+(2)+(3)", parser);
			Assert.That(val, Is.EqualTo(6));
		}
		{
			var val = Calculator.Calc("(1+2+3)", parser);
			Assert.That(val, Is.EqualTo(6));
		}
		{
			var val = Calculator.Calc("(1+1)*(2+2)*(3+3)", parser);
			Assert.That(val, Is.EqualTo(48));
		}
		{
			var val = Calculator.Calc("(1+(2*(3+3)))", parser);
			Assert.That(val, Is.EqualTo(13));
		}
		{
			var val = Calculator.Calc("0.1+0.2+0.3", parser);
			Assert.That(val, Is.EqualTo(0.6f));
		}
		{
			var val = Calculator.Calc(".1+.2*.3", parser);
			Assert.That(val, Is.EqualTo(0.09f));
		}
	}
}
