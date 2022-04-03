using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace StringCalculator
{
	/// <summary>
	/// 最終的な式を構成する項目
	/// </summary>
	public class FormulaSymbol
	{
		public int Priority { get; private set; }
		public FormulaSymbol(int priority)
		{
			Priority = priority;
		}
	}

	/// <summary>
	/// 演算子
	/// </summary>
	public class FormulaSymbolString : FormulaSymbol
	{
		public string Key { get; private set; }
		public FormulaSymbolString(int priority, string key) : base(priority)
		{
			Key = key;
		}
	}

	/// <summary>
	/// 数値
	/// </summary>
	public class FormulaSymbolNumerical : FormulaSymbol
	{
		public float Value { get; private set; }
		public FormulaSymbolNumerical(int priority, float val) : base(priority)
		{
			Value = val;
		}
	}
}