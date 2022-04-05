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
		public static float Calc(string baseStr, ParserFormula parser)
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
					if (formula[i].Priority != priority) 
					{

						continue;
					}

					var startPriority = i;
					(int startIdx, int count) range = (i, 0);
					result = null;
					for (var j = i; j < formula.Count && formula[j].Priority == priority; j++)
					{
						if (formula[j] is FormulaSymbolString)
						{
							var symbolString = (FormulaSymbolString)formula[j];
							var key = symbolString.GetType().Name;
							var parserSymbol = parser.DictionaryParseSymbol[key];
							switch (parserSymbol.GetType().Name)
							{
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
						range.count++;
					}

					if (result == null && formula[i] is FormulaSymbolNumerical)
					{
						var num = (FormulaSymbolNumerical)formula[i];
						result = num.Value;
					}

					formula.RemoveRange(i, range.count);
					formula.Insert(range.startIdx,new FormulaSymbolNumerical(priority - 1, result.Value));
				}
			}
			return 0.0f;
		}
	}
}