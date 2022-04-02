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
	public class FormulaSymbol : MonoBehaviour
	{

	}

	/// <summary>
	/// 演算子
	/// </summary>
	public class FormulaSymbolOperator : FormulaSymbol
	{
		public int Priority { get; }
	}

	/// <summary>
	/// 数値
	/// </summary>
	public class FormulaSymbolNumerical : FormulaSymbol
	{
		public float Value { get; private set; }
		public FormulaSymbolNumerical(float val) 
		{
			Value = val; 
		}
	}
}