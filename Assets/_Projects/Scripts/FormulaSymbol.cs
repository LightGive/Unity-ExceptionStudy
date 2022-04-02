using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace StringCalculator
{
	/// <summary>
	/// �ŏI�I�Ȏ����\�����鍀��
	/// </summary>
	public class FormulaSymbol : MonoBehaviour
	{

	}

	/// <summary>
	/// ���Z�q
	/// </summary>
	public class FormulaSymbolOperator : FormulaSymbol
	{
		public int Priority { get; }
	}

	/// <summary>
	/// ���l
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