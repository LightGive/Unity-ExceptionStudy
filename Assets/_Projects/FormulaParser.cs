using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class FormulaParser
{
	const char ParenthesisStart = '(';
	const char ParenthesisEnd = ')';

	Dictionary<char, OperatorBase> _stringGroupByOperatorName = null;

	public FormulaParser()
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
				Assert.IsTrue(false, "�����񂪐ݒ肳��Ă��܂���");
				return;
			}
			_stringGroupByOperatorName.Add(att.OperatorName, (OperatorBase)Activator.CreateInstance(t));
		}
	}

	public float Calc(string baseStr, int priority = 0)
	{
		if(priority > 20) { throw new Exception(); }

		//�X�y�[�X��������������ɂ��Ă���
		var noSpaceStr = baseStr.Replace(" ", "");

		//���Z�q�ƒl�����݂ɓ���郊�X�g
		List<StringGroup> stringGroupList = new List<StringGroup>();

		//�P����������
		for (var i = 0; i < noSpaceStr.Length; i++)
		{
			var targetChar = noSpaceStr[i];

			//�J�b�R�J�n�̃`�F�b�N
			if (targetChar == ParenthesisStart)
			{
				var endIdx = GetParenthesisEndIndex(noSpaceStr, i);
				var extractStr = noSpaceStr.Substring(i + 1, endIdx - i);
				var v = Calc(extractStr, priority + 1);
				//Debug.Log(
				//	$"�����o�����J�b�R�܂ރJ�b�R���̕�����: {extractStr}\n" +
				//	$"�J�b�R���̌v�Z����: {v}");
				//TODO:���������_���Ή�
				noSpaceStr.Replace(extractStr, v.ToString("0"));
				i--;
			}
			else if (char.IsNumber(targetChar))
			{
				var endIdx = GetNumericalEndIndex(noSpaceStr, i);
				var extractStr = noSpaceStr.Substring(i, endIdx - i + 1);
				var v = float.Parse(extractStr);
				stringGroupList.Add(new NumericalString(extractStr, v));
				i = endIdx;
			}
			else if (_stringGroupByOperatorName.Keys.Contains(targetChar))
			{
				stringGroupList.Add(new OperatorString(targetChar.ToString(),_stringGroupByOperatorName[targetChar]));
			}
		}

		if (stringGroupList.Count == 0)
		{
			//TODO:������O�����
			throw new SystemException();
		}

		StringGroup preStringGroup = null;
		if (stringGroupList.Last().StringType == StringType.Operator ||
			stringGroupList[0].StringType == StringType.Operator)
		{
			//�ŏ����Ōオ���Z�q�ŏI����Ă���
			throw new NumericalOperatorOrder();
		}

		float result = 0.0f;
		for (var i = 0; i < stringGroupList.Count; i++)
		{
			var stringGroup = stringGroupList[i];
			if (preStringGroup != null && stringGroup.StringType == preStringGroup.StringType)
			{
				//���Z�q�Ɛ��l���A�����Ă���
				throw new NumericalOperatorOrder();
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
	///  (���J�n���ďI������܂ł̔ԍ���Ԃ�
	/// </summary>
	/// <param name="baseStr"></param>
	/// <param name="parenthesisStartIdx"></param>
	/// <returns></returns>
	public int GetParenthesisEndIndex(string baseStr, int parenthesisStartIdx)
	{
		if (baseStr[parenthesisStartIdx] != ParenthesisStart)
		{
			Assert.IsTrue(false, "�J�n�ʒu�̕������J�b�R����Ȃ�");
			return -1;
		}

		var nest = 0;
		var startIdx = parenthesisStartIdx + 1;
		for (var i = startIdx; i < baseStr.Length; i++)
		{
			var targetStr = baseStr[i];
			if (targetStr == ParenthesisEnd)
			{
				if (nest != 0)
				{
					nest--;
					continue;
				}
				//����ɏI��
				return i - 1;
			}
			else if (targetStr == ParenthesisStart)
			{
				nest++;
			}
		}

		//�J�b�R�������ɏI������ꍇ
		throw new ParenthesisException();
	}

	public int GetNumericalEndIndex(string baseStr,int numericalStartIdx)
	{
		if (!char.IsNumber(baseStr[numericalStartIdx]))
		{
			Assert.IsTrue(false, "�J�n�ʒu�̕��������l����Ȃ�");
			return -1;
		}

		var isPeriod = false;
		for (var i = numericalStartIdx + 1; i < baseStr.Length; i++)
		{
			if (char.IsNumber(baseStr[i]))
			{
				continue;
			}
			else if(baseStr[i] == '.')
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
	/// �J�b�R���Ⴄ
	/// </summary>
	public class ParenthesisException : Exception { }

	/// <summary>
	/// �s���I�h������
	/// </summary>
	public class MorePeriodException : Exception { }

	/// <summary>
	/// ���l�Ɖ��Z�q�̏��Ԃ��Ⴄ
	/// </summary>
	public class NumericalOperatorOrder : Exception { }
}

/// <summary>
/// �^�O�I�u�W�F�N�g��\���N���X�ɕt���鑮��
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class OperatorAttribute : Attribute
{
	/// <summary>
	/// �C���^�O��
	/// </summary>
	public char OperatorName { get; private set; }
	public OperatorAttribute(char operatorName) { this.OperatorName = operatorName; }
}

public abstract class OperatorBase
{
	public abstract float Calc(float val1, float val2);
}

[Operator('+')]
public class OperatorAddition : OperatorBase { public override float Calc(float val1, float val2) => val1 + val2; }
[Operator('-')]
public class OperatorSubtraction : OperatorBase { public override float Calc(float val1, float val2) => val1 - val2; }
[Operator('*')]
public class OperatorMultiplication : OperatorBase { public override float Calc(float val1, float val2) => val1 * val2; }
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
	/// ���Z�q
	/// </summary>
	Operator,
	/// <summary>
	/// ���l
	/// </summary>
	Numerical,
}


public abstract class StringGroup
{
	public abstract StringType StringType { get; }
	public string Str = "";
	public int Priority = 0;
	public StringGroup(string baseStr)
	{
		Str = baseStr;
	}
}

/// <summary>
/// ���l
/// </summary>
public class NumericalString : StringGroup
{
	public NumericalString(string baseStr, float value) : base(baseStr)
	{
		Value = value;
	}

	public float Value { get; private set; } = 0.0f;
	public override StringType StringType => StringType.Numerical;
}

/// <summary>
/// ���Z�q
/// </summary>
public class OperatorString : StringGroup
{
	public override StringType StringType => StringType.Operator;
	public OperatorBase Operator { get; private set; }
	public OperatorString(string baseStr, OperatorBase operatorBase) : base(baseStr) 
	{
		Operator = operatorBase;
	}
}