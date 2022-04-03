using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StringCalculator
{
	/// <summary>
	/// 式を計算するクラス
	/// </summary>
	public static class Calculator
	{
		public static float Calc(string baseStr, FormulaParser parser)
		{
			var formula = parser.GetFormula(baseStr);
			var maxPriority = formula.Select(x => x.Priority).Max();

			for (var priority = maxPriority; priority >= 0; priority--)
			{
				//一番最初の文字列が数字かチェック
				if (!(formula[0] is FormulaSymbolNumerical))
				{
					throw new System.Exception();
				}
				float? result = null;
				for (var i = 0; i < formula.Count; i++)
				{
					if (formula[i].Priority != priority) { continue; }
					if (formula[i] is FormulaSymbolString)
					{
						var symbolString = (FormulaSymbolString)formula[i];
						var key = symbolString.GetType().Name;
						var parserSymbol = parser.DictionaryParseSymbol[key];
						switch (parserSymbol.GetType().Name)
						{
							case nameof(ParserSymbolConstant):
								var constant = (ParserSymbolConstant)parserSymbol;
								formula.RemoveAt(i);
								formula.Insert(i, new FormulaSymbolNumerical(priority, constant.Calc()));
								break;
							case nameof(ParserSymbolOperator):
								var ope = (ParserSymbolOperator)parserSymbol;
								var val2 = (FormulaSymbolNumerical)formula[i + 1];
								if (!result.HasValue || val2.Priority != priority)
								{
									throw new FormulaException.NumericalOperatorOrderException();
								}
								result = ope.Calc(result.Value, val2.Value);
								break;
						}
					}

					if (result == null && formula[i] is FormulaSymbolNumerical)
					{
						var num = (FormulaSymbolNumerical)formula[i];
						result = num.Value;
					}
				}
			}
			return 0.0f;
		}
	}
}