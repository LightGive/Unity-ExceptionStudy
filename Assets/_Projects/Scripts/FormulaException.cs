using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FormulaException
{
	public class ExceptionZero : Exception { }
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
}
