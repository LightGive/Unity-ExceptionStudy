using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StringCalculator
{
	/// <summary>
	/// Ž®‚ðŒvŽZ‚·‚éƒNƒ‰ƒX
	/// </summary>
	public static class Calculator
	{
		public static float Calc(string baseStr, FormulaParser parser)
		{
			var formula = parser.GetFormula(baseStr);
			for(var i = 0; i < formula.Count; i++)
			{

			}
			return 0.0f;
		}
	}
}