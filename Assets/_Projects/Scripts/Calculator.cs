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
			return CalcFormula(formula, parser).Value;
		}

		static FormulaSymbolNumerical CalcFormula(List<FormulaSymbol> formulaList,ParserFormula parser)
		{
			DisplayLog(formulaList.ToArray());

			var isNumericalOperatorOrder = formulaList
				.Select((s, i) => new { Content = s, Index = i })
				.Where(x =>
					x.Index % 2 == 0 && !(x.Content is FormulaSymbolNumerical) ||
					x.Index % 2 == 1 && !(x.Content is FormulaSymbolString))
				.Count() > 0;

			if (isNumericalOperatorOrder)
			{
				//最初か最後が数字じゃない、もしくは演算子と数字が交互になっていない
				throw new FormulaException.NumericalOperatorOrderException();
			}

			while (formulaList.Count != 1)
			{
				var isAllSamePriority = formulaList
					.Select(x => x.Priority)
					.Distinct()
					.Count() == 1;

				if (isAllSamePriority)
				{
					//優先度が全て同じ時
					var result = CalcSameFormulaSymbolList(formulaList, formulaList[0].Priority - 1);
					formulaList.RemoveRange(0, formulaList.Count);
					formulaList.Add(result);
					Debug.Log($"formulaList{formulaList.Count}");

				}
				else
				{
					var maxPriority = formulaList.Select(x => x.Priority).Max();
					Debug.Log($"maxPriority{maxPriority}");
					for (var i = 0; i < formulaList.Count; i++)
					{
						var targetFormula = formulaList[i];
						if (targetFormula.Priority != maxPriority) { continue; }
						var li = ExtractSamePriorityFormula(formulaList, i);
						var count = li.Count;
						var num = CalcFormula(li, parser);
						formulaList.RemoveRange(i, count);
						formulaList.Insert(i, num);
					}
				}
			}
			return (FormulaSymbolNumerical)formulaList[0];
		}

		/// <summary>
		/// 指定位置からPriorityが同じまでのFormulaSymbolのリストを返す
		/// </summary>
		/// <param name="formulaList"></param>
		/// <param name="startIdx"></param>
		/// <returns></returns>
		static List<FormulaSymbol> ExtractSamePriorityFormula(List<FormulaSymbol> formulaList, int startIdx)
		{
			List<FormulaSymbol> tmpList = new List<FormulaSymbol>();
			var targetPriority = formulaList[startIdx].Priority;
			for (var i = startIdx; i < formulaList.Count; i++)
			{
				if (formulaList[i].Priority != targetPriority) { break; }
				tmpList.Add(formulaList[i]);
				continue;
			}
			return tmpList;
		}

		static FormulaSymbolNumerical CalcSameFormulaSymbolList(List<FormulaSymbol> formulaList,int priority)
		{
			while (formulaList.Count > 1)
			{
				var maxPriority = formulaList
					.Where(x => x is FormulaSymbolString)
					.Select(x => ((FormulaSymbolString)x).Parser.Priority).Max();

				for (var i = 0; i < formulaList.Count; i++)
				{
					if (!(formulaList[i] is FormulaSymbolString)) { continue; }
					var symbolString = (FormulaSymbolString)formulaList[i];
					if(symbolString.Parser.Priority != maxPriority) { continue; }

					//範囲外チェック
					if (i + 1 >= formulaList.Count || i - 1 < 0)
					{
						throw new System.Exception();
					}
					//演算子と数字の順番チェック
					if (!(formulaList[i - 1] is FormulaSymbolNumerical) ||
						!(formulaList[i + 1] is FormulaSymbolNumerical))
					{
						throw new FormulaException.NumericalOperatorOrderException();
					}

					var val1 = (FormulaSymbolNumerical)formulaList[i - 1];
					var val2 = (FormulaSymbolNumerical)formulaList[i + 1];
					var result = symbolString.Parser.Calc(val1.Value, val2.Value);
					formulaList.RemoveRange(i - 1, 3);
					formulaList.Insert(i - 1, new FormulaSymbolNumerical(priority, result));
				}
			}
			return (FormulaSymbolNumerical)formulaList[0];
		}

		static void DisplayLog(FormulaSymbol[] symbols)
		{
			var str = "";
			for(var i = 0; i < symbols.Length; i++)
			{
				switch (symbols[i].GetType().Name)
				{
					case nameof(FormulaSymbolNumerical):
						var numerical = (FormulaSymbolNumerical)symbols[i];
						str += $"【{numerical.Priority}:{numerical.Value}】";
						break;
					case nameof(FormulaSymbolString):
						var symbolString = (FormulaSymbolString)symbols[i];
						str += $"【{symbolString.Priority}:{symbolString.Parser.ComparisonStr}】";
						break;
				}
			}
			Debug.Log(str);
		}
	}
}