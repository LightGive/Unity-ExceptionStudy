using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StringCalculator
{
	public class StringCalculatorSymbolList
	{
		#region 算術演算子
		/// <summary>
		/// 加算
		/// </summary>
		public class SymbolAdd : ParserSymbolOperator
		{
			public override string ComparisonStr => "+";
			public override int Priority => 0;

			public override float Calc(float val1, float val2) => val1 + val2;
		}
		/// <summary>
		/// 減算
		/// </summary>
		public class SymbolSub : ParserSymbolOperator
		{
			public override string ComparisonStr => "-";
			public override int Priority => 0;
			public override float Calc(float val1, float val2) => val1 - val2;
		}
		/// <summary>
		/// 乗算
		/// </summary>
		public class SymbolMulti : ParserSymbolOperator
		{
			public override string ComparisonStr => "*";
			public override int Priority => 1;
			public override float Calc(float val1, float val2) => val1 * val2;
		}
		/// <summary>
		/// 除算
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
		/// べき乗
		/// </summary>
		public class SymbolPow : ParserSymbolOperator
		{
			public override string ComparisonStr => "^";
			public override int Priority => 2;
			public override float Calc(float val1, float val2) => Mathf.Pow(val1, val2);
		}
		#endregion

		#region 定数
		public class PI : ParserSymbolConstant
		{
			public override float ConstValue => Mathf.PI;
			public override string ComparisonStr => "pi";
		}

		//新たに定数を加えたい場合はここに追加する

		#endregion
	}
}