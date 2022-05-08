using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StringCalculator
{
	/// <summary>
	/// �����v�Z����N���X
	/// </summary>
	public static class Calculator
	{
		static ParserFormula Parser = null;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void Init()
		{
			Parser = new ParserFormula();
		}

		/// <summary>
		/// ������̎����v�Z����
		/// </summary>
		/// <param name="baseStr">��</param>
		/// <returns>��</returns>
		public static float Calc(string baseStr)
		{
			if(Parser == null) { throw new StringCalculatorException.ParserNotInitialized(); }
			var formula = Parser.GetFormula(baseStr);
			return CalcFormula(formula).Value;
		}

		static FormulaSymbolNumerical CalcFormula(List<FormulaSymbol> formulaList)
		{
			//���̏����������������`�F�b�N
			var isNumericalOperatorOrder = formulaList.Count % 2 == 1 &&
				formulaList
				.Select((s, i) => new { Content = s, Index = i })
				.Where(x =>
					x.Index % 2 == 0 && !(x.Content is FormulaSymbolNumerical) ||
					x.Index % 2 == 1 && !(x.Content is FormulaSymbolString))
				.Count() > 0;

			if (isNumericalOperatorOrder)
			{
				throw new StringCalculatorException.NumericalOperatorOrderException();
			}

			var isAllSamePriority = formulaList
				.Select(x => x.Priority)
				.Distinct()
				.Count() == 1;

			if (isAllSamePriority)
			{
				var nextPriority = Mathf.Clamp(formulaList[0].Priority - 1, 0, int.MaxValue);

				//���Z�q�̗D��x�̐��l���X�g�i�~���j
				var priorityList = formulaList
					.Where(x => x is FormulaSymbolString)
					.Select(x => ((FormulaSymbolString)x).Parser.Priority)
					.OrderByDescending(x => x)
					.Distinct();

				foreach (var currentPriority in priorityList)
				{
					var idx = 1;
					while (formulaList.Count >= 3 && idx < formulaList.Count)
					{
						var symbolString = (FormulaSymbolString)formulaList[idx];
						if (symbolString.Parser.Priority != currentPriority)
						{
							idx += 2;
							continue;
						}

						var numValLeft = (FormulaSymbolNumerical)formulaList[idx - 1];
						var numValRight = (FormulaSymbolNumerical)formulaList[idx + 1];
						var calcVal = symbolString.Parser.Calc(numValLeft.Value, numValRight.Value);
						formulaList.RemoveRange(idx - 1, 3);
						formulaList.Insert(idx - 1, new FormulaSymbolNumerical(nextPriority, calcVal));
					}
				}
				return new FormulaSymbolNumerical(nextPriority, ((FormulaSymbolNumerical)formulaList[0]).Value);
			}

			while (formulaList.Count != 1)
			{
				var maxPriority = formulaList.Select(x => x.Priority).Max();
				for (var i = 0; i < formulaList.Count; i++)
				{
					var targetFormula = formulaList[i];
					if (targetFormula.Priority != maxPriority) { continue; }
					var li = ExtractSamePriorityFormula(formulaList, i);
					var sameSymbolCount = li.Count;
					var num = CalcFormula(li);
					formulaList.RemoveRange(i, sameSymbolCount);
					formulaList.Insert(i, num);
				}
			}
			var result = (FormulaSymbolNumerical)formulaList[0];
			return result;
		}

		/// <summary>
		/// �w��ʒu����Priority�������܂ł�FormulaSymbol�̃��X�g��Ԃ�
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

		static void DisplayLog(FormulaSymbol[] symbols)
		{
			var str = "";
			for(var i = 0; i < symbols.Length; i++)
			{
				switch (symbols[i].GetType().Name)
				{
					case nameof(FormulaSymbolNumerical):
						var numerical = (FormulaSymbolNumerical)symbols[i];
						str += $"�y{numerical.Priority}:{numerical.Value}�z";
						break;
					case nameof(FormulaSymbolString):
						var symbolString = (FormulaSymbolString)symbols[i];
						str += $"�y{symbolString.Priority}:{symbolString.Parser.ComparisonStr}�z";
						break;
				}
			}
			Debug.Log(str);
		}
	}
}