using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class StringCalculator
{
	const char ParenthesisStart = '(';
	const char ParenthesisEnd = ')';
	const char Period = '.';

	Dictionary<char, OperatorBase> _stringGroupByOperatorName = null;

	public StringCalculator()
	{
		InitializeDictionary();
	}

	void InitializeDictionary()
	{
		_stringGroupByOperatorName = new Dictionary<char, OperatorBase>();
		var stringGroupTypes = Assembly.GetAssembly(typeof(OperatorBase))
			.GetTypes()
			.Where(x => x.IsSubclassOf(typeof(OperatorBase)) && !x.IsAbstract)
			.ToArray();

		foreach (var t in stringGroupTypes)
		{
			var att = (OperatorAttribute)t.GetCustomAttribute(typeof(OperatorAttribute), false);
			if (att == null)
			{
				Assert.IsTrue(false, "文字列が設定されていません");
				return;
			}
			_stringGroupByOperatorName.Add(att.OperatorName, (OperatorBase)Activator.CreateInstance(t));
		}
	}

	public float Calc(string baseStr)
	{
		//スペースを除いた文字列にしておく
		var noSpaceStr = baseStr.Replace(" ", "");

		//演算子と値を交互に入れるリスト
		List<StringGroup> stringGroupList = new List<StringGroup>();

		//１文字ずつ処理
		for (var i = 0; i < noSpaceStr.Length; i++)
		{
			var targetChar = noSpaceStr[i];

			//カッコ開始のチェック
			if (targetChar == ParenthesisStart)
			{
				var endIdx = GetParenthesisEndIndex(noSpaceStr, i);
				//カッコ内の文字列を抜き出す
				var extractStr = noSpaceStr.Substring(i + 1, endIdx - i - 1);
				//カッコ内の文字列を再帰
				var v = Calc(extractStr);
				stringGroupList.Add(new NumericalString(v));
				i = endIdx;
			}
			//数字チェック
			else if (char.IsNumber(targetChar) || targetChar == Period)
			{
				var endIdx = GetNumericalEndIndex(noSpaceStr, i);
				var extractStr = noSpaceStr.Substring(i, endIdx - i + 1);
				stringGroupList.Add(new NumericalString(float.Parse(extractStr)));
				i = endIdx;
			}
			else if (_stringGroupByOperatorName.Keys.Contains(targetChar))
			{
				stringGroupList.Add(new OperatorString(_stringGroupByOperatorName[targetChar]));
			}
		}

		//式が空かどうかをチェック
		if (stringGroupList.Count == 0)
		{
			throw new FormulaEmptyException();
		}

		//偶数番号が数値、奇数番号が演算子かどうか
		var isNumericalOperatorOrder = stringGroupList
			.Select((s, i) => new { Content = s, Index = i })
			.Where(x =>
				x.Index % 2 == 0 && x.Content.StringType != StringType.Numerical ||
				x.Index % 2 == 1 && x.Content.StringType != StringType.Operator)
			.Count() > 0;

		if (isNumericalOperatorOrder)
		{
			//最初か最後が演算子で終わっている
			throw new NumericalOperatorOrderException();
		}

		float result = 0.0f;
		StringGroup preStringGroup = null;
		for (var i = 0; i < stringGroupList.Count; i++)
		{
			var stringGroup = stringGroupList[i];
			if (preStringGroup != null && stringGroup.StringType == preStringGroup.StringType)
			{
				//演算子と数値が連続している
				throw new NumericalOperatorOrderException();
			}

			switch (stringGroup.StringType)
			{
				case StringType.Numerical:
					if (preStringGroup == null)
					{
						result = ((NumericalString)stringGroup).Value;
					}
					break;

				case StringType.Operator:
					var ope = (OperatorString)stringGroup;
					var nextNum = (NumericalString)stringGroupList[i + 1];
					result = ope.Operator.Calc(result, nextNum.Value);
					break;
			}
			preStringGroup = stringGroup;
		}

		return result;
	}

	/// <summary>
	///  カッコ終わりの文字番号を返す
	/// </summary>
	/// <param name="baseStr">元の文字列</param>
	/// <param name="parenthesisStartIdx">括弧の開始文字番号</param>
	/// <returns></returns>
	public int GetParenthesisEndIndex(string baseStr, int parenthesisStartIdx)
	{
		if (baseStr[parenthesisStartIdx] != ParenthesisStart)
		{
			Assert.IsTrue(false, "開始位置の文字がカッコじゃない");
			return -1;
		}

		var nest = 0;
		var startIdx = parenthesisStartIdx + 1;
		for (var i = startIdx; i < baseStr.Length; i++)
		{
			var targetChar = baseStr[i];
			if (targetChar == ParenthesisEnd)
			{
				if (nest != 0)
				{
					nest--;

					continue;
				}
				//正常に終了
				return i;
			}
			else if (targetChar == ParenthesisStart)
			{
				nest++;
			}
		}

		//カッコが閉じずに終わった場合
		throw new ParenthesisException();
	}

	/// <summary>
	/// 数字が連続している時、終わりの文字番号を返す
	/// </summary>
	/// <param name="baseStr">元の文字列</param>
	/// <param name="numericalStartIdx">数字の開始文字番号</param>
	/// <returns></returns>
	public int GetNumericalEndIndex(string baseStr,int numericalStartIdx)
	{
		if (!char.IsNumber(baseStr[numericalStartIdx]) && baseStr[numericalStartIdx] != Period)
		{
			Assert.IsTrue(false, "開始位置の文字が数値じゃない");
			return -1;
		}

		var isPeriod = false;
		for (var i = numericalStartIdx + 1; i < baseStr.Length; i++)
		{
			if (char.IsNumber(baseStr[i]))
			{
				continue;
			}
			else if(baseStr[i] == Period)
			{
				if (isPeriod)
				{
					throw new MorePeriodException();
				}
				isPeriod = true;
				continue;
			}
			else
			{
				return i - 1;
			}
		}
		return baseStr.Length - 1;
	}


	/// <summary>
	/// カッコが違う
	/// </summary>
	public class ParenthesisException : Exception { }
	/// <summary>
	/// ピリオドが多い
	/// </summary>
	public class MorePeriodException : Exception { }
	/// <summary>
	/// 数値と演算子の順番が違う
	/// </summary>
	public class NumericalOperatorOrderException : Exception { }
	/// <summary>
	/// 式が空
	/// </summary>
	public class FormulaEmptyException : Exception { }
}

/// <summary>
/// タグオブジェクトを表すクラスに付ける属性
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class OperatorAttribute : Attribute
{
	/// <summary>
	/// 修飾タグ名
	/// </summary>
	public char OperatorName { get; private set; }
	public OperatorAttribute(char operatorName) { this.OperatorName = operatorName; }
}

/// <summary>
/// 演算子のベース
/// </summary>
public abstract class OperatorBase
{
	public abstract float Calc(float val1, float val2);
}

/// <summary>
/// 加算
/// </summary>
[Operator('+')]
public class OperatorAddition : OperatorBase { public override float Calc(float val1, float val2) => val1 + val2; }

/// <summary>
/// 減算
/// </summary>
[Operator('-')]
public class OperatorSubtraction : OperatorBase { public override float Calc(float val1, float val2) => val1 - val2; }

/// <summary>
/// 乗算
/// </summary>
[Operator('*')]
public class OperatorMultiplication : OperatorBase { public override float Calc(float val1, float val2) => val1 * val2; }

/// <summary>
/// 除算
/// </summary>
[Operator('/')]
public class OperatorDivision : OperatorBase 
{
	public override float Calc(float val1, float val2)
	{
		if (val2 == 0.0f)
		{
			throw new DivideByZeroException();
		}
		return val1 / val2;
	}
}


public enum StringType
{
	/// <summary>
	/// 演算子
	/// </summary>
	Operator,
	/// <summary>
	/// 数値
	/// </summary>
	Numerical,
}


public abstract class StringGroup
{
	public abstract StringType StringType { get; }
}

/// <summary>
/// 数値
/// </summary>
public class NumericalString : StringGroup
{
	public override StringType StringType => StringType.Numerical;
	public float Value { get; private set; } = 0.0f;

	public NumericalString(float value)
	{
		Value = value;
	}
}

/// <summary>
/// 演算子
/// </summary>
public class OperatorString : StringGroup
{
	public override StringType StringType => StringType.Operator;
	public OperatorBase Operator { get; private set; }
	public OperatorString(OperatorBase operatorBase)
	{
		Operator = operatorBase;
	}
}