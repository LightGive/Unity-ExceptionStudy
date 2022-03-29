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
				Assert.IsTrue(false, "�����񂪐ݒ肳��Ă��܂���");
				return;
			}
			_stringGroupByOperatorName.Add(att.OperatorName, (OperatorBase)Activator.CreateInstance(t));
		}
	}

	public float Calc(string baseStr)
	{
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
				//�J�b�R���̕�����𔲂��o��
				var extractStr = noSpaceStr.Substring(i + 1, endIdx - i - 1);
				//�J�b�R���̕�������ċA
				var v = Calc(extractStr);
				stringGroupList.Add(new NumericalString(v));
				i = endIdx;
			}
			//�����`�F�b�N
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

		//�����󂩂ǂ������`�F�b�N
		if (stringGroupList.Count == 0)
		{
			throw new FormulaEmptyException();
		}

		//�����ԍ������l�A��ԍ������Z�q���ǂ���
		var isNumericalOperatorOrder = stringGroupList
			.Select((s, i) => new { Content = s, Index = i })
			.Where(x =>
				x.Index % 2 == 0 && x.Content.StringType != StringType.Numerical ||
				x.Index % 2 == 1 && x.Content.StringType != StringType.Operator)
			.Count() > 0;

		if (isNumericalOperatorOrder)
		{
			//�ŏ����Ōオ���Z�q�ŏI����Ă���
			throw new NumericalOperatorOrderException();
		}

		float result = 0.0f;
		StringGroup preStringGroup = null;
		for (var i = 0; i < stringGroupList.Count; i++)
		{
			var stringGroup = stringGroupList[i];
			if (preStringGroup != null && stringGroup.StringType == preStringGroup.StringType)
			{
				//���Z�q�Ɛ��l���A�����Ă���
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
	///  �J�b�R�I���̕����ԍ���Ԃ�
	/// </summary>
	/// <param name="baseStr">���̕�����</param>
	/// <param name="parenthesisStartIdx">���ʂ̊J�n�����ԍ�</param>
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
			var targetChar = baseStr[i];
			if (targetChar == ParenthesisEnd)
			{
				if (nest != 0)
				{
					nest--;

					continue;
				}
				//����ɏI��
				return i;
			}
			else if (targetChar == ParenthesisStart)
			{
				nest++;
			}
		}

		//�J�b�R�������ɏI������ꍇ
		throw new ParenthesisException();
	}

	/// <summary>
	/// �������A�����Ă��鎞�A�I���̕����ԍ���Ԃ�
	/// </summary>
	/// <param name="baseStr">���̕�����</param>
	/// <param name="numericalStartIdx">�����̊J�n�����ԍ�</param>
	/// <returns></returns>
	public int GetNumericalEndIndex(string baseStr,int numericalStartIdx)
	{
		if (!char.IsNumber(baseStr[numericalStartIdx]) && baseStr[numericalStartIdx] != Period)
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
	public class NumericalOperatorOrderException : Exception { }
	/// <summary>
	/// ������
	/// </summary>
	public class FormulaEmptyException : Exception { }
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

/// <summary>
/// ���Z�q�̃x�[�X
/// </summary>
public abstract class OperatorBase
{
	public abstract float Calc(float val1, float val2);
}

/// <summary>
/// ���Z
/// </summary>
[Operator('+')]
public class OperatorAddition : OperatorBase { public override float Calc(float val1, float val2) => val1 + val2; }

/// <summary>
/// ���Z
/// </summary>
[Operator('-')]
public class OperatorSubtraction : OperatorBase { public override float Calc(float val1, float val2) => val1 - val2; }

/// <summary>
/// ��Z
/// </summary>
[Operator('*')]
public class OperatorMultiplication : OperatorBase { public override float Calc(float val1, float val2) => val1 * val2; }

/// <summary>
/// ���Z
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
}

/// <summary>
/// ���l
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
/// ���Z�q
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