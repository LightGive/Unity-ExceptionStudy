using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StringCalculator
{
	public class StringCalculatorSymbolList
	{
		#region �Z�p���Z�q
		/// <summary>
		/// ���Z
		/// </summary>
		public class SymbolAdd : ParserSymbolOperator
		{
			public override string ComparisonStr => "+";
			public override int Priority => 0;

			public override float Calc(float val1, float val2) => val1 + val2;
		}
		/// <summary>
		/// ���Z
		/// </summary>
		public class SymbolSub : ParserSymbolOperator
		{
			public override string ComparisonStr => "-";
			public override int Priority => 0;
			public override float Calc(float val1, float val2) => val1 - val2;
		}
		/// <summary>
		/// ��Z
		/// </summary>
		public class SymbolMulti : ParserSymbolOperator
		{
			public override string ComparisonStr => "*";
			public override int Priority => 1;
			public override float Calc(float val1, float val2) => val1 * val2;
		}
		/// <summary>
		/// ���Z
		/// </summary>
		public class SymbolDiv : ParserSymbolOperator
		{
			public override string ComparisonStr => "/";
			public override int Priority => 1;
			public override float Calc(float val1, float val2)
			{
				if (val2 == 0) { throw new DivideByZeroException(); }
				return val1 / val2;
			}
		}
		/// <summary>
		/// �ׂ���
		/// </summary>
		public class SymbolPow : ParserSymbolOperator
		{
			public override string ComparisonStr => "^";
			public override int Priority => 2;
			public override float Calc(float val1, float val2) => Mathf.Pow(val1, val2);
		}
		#endregion

		#region �萔
		public class PI : ParserSymbolConstant
		{
			public override float ConstValue => Mathf.PI;
			public override string ComparisonStr => "pi";
		}

		//�V���ɒ萔�����������ꍇ�͂����ɒǉ�����

		#endregion
	}
}