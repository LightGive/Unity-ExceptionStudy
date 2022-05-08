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
	public class FormulaSymbol
	{
		public int Priority { get; private set; }
		public FormulaSymbol(int priority)
		{
			Priority = priority;
		}
	}

	/// <summary>
	/// ���Z�q
	/// </summary>
	public class FormulaSymbolString : FormulaSymbol
	{
		public ParserSymbolOperator Parser { get; private set; }
		public FormulaSymbolString(int priority, ParserSymbolOperator parser) : base(priority)
		{
			Parser = parser;
		}
	}

	/// <summary>
	/// ���l
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