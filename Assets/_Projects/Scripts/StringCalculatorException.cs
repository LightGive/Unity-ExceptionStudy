using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StringCalculator
{
	public static class StringCalculatorException
	{
		/// <summary>
		/// ピリオドが多い
		/// </summary>
		public class MorePeriodException : Exception { }
		/// <summary>
		/// カッコが違う
		/// </summary>
		public class ParenthesisException : Exception { }
		/// <summary>
		/// 無効な文字列
		/// </summary>
		public class InvalidStringException : Exception { }
		/// <summary>
		/// 式が空
		/// </summary>
		public class FormulaEmptyException : Exception { }
		/// <summary>
		/// 数値と演算子の順番が違う
		/// </summary>
		public class NumericalOperatorOrderException : Exception { }
		/// <summary>
		/// 定数、関数、演算子に使用できない文字列が含まれている
		/// </summary>
		public class CantUsedStringSymbolChar : Exception { }
		/// <summary>
		/// 初期化されていない
		/// </summary>
		public class ParserNotInitialized : Exception { }
	}
}