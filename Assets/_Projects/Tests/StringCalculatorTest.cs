using NUnit.Framework;
using UnityEngine;

namespace StringCalculator.Test
{
	public class StringCalculatorTest
	{
		[Test]
		public void StringCalculatorTestSample()
		{
			Calculator.Init();
			Assert.That(Calc("1"), Is.EqualTo(1));
			Assert.That(Calc("(1)"), Is.EqualTo(1));
			Assert.That(Calc("((1))"), Is.EqualTo(1));
			Assert.That(Calc("1+2"), Is.EqualTo(3));
			Assert.That(Calc("1+2+3"), Is.EqualTo(6));
			Assert.That(Calc("(1)+(2)+(3)"), Is.EqualTo(6));
			Assert.That(Calc("(1+2+3)"), Is.EqualTo(6));
			Assert.That(Calc("1+2*3"), Is.EqualTo(7));
			Assert.That(Calc("(1+2)*3"), Is.EqualTo(9));
			Assert.That(Calc("(1+1)*(2+2)*(3+3)"), Is.EqualTo(48));
			Assert.That(Calc("(1+(2*(3+3)))"), Is.EqualTo(13));
			Assert.That(Calc("0.1+0.2+0.3"), Is.EqualTo(0.6f));
			Assert.That(Calc(".1+.2+.3"), Is.EqualTo(0.6f));
			Assert.That(Calc("0.1+0.2*0.3"), Is.EqualTo(0.16f));
			Assert.That(Calc("3^3"), Is.EqualTo(27));
			Assert.That(Calc("2^2^2"), Is.EqualTo(16));
			Assert.That(Calc("pi"), Is.EqualTo(Mathf.PI));
		}

		float Calc(string str)
		{
			Debug.Log("--------------------------------------");
			var result = Calculator.Calc(str);
			Debug.Log($"{str}={result}");
			return result;
		}
	}
}