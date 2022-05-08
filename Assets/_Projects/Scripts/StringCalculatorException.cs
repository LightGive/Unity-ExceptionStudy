using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StringCalculator
{
	public static class StringCalculatorException
	{
		/// <summary>
		/// �s���I�h������
		/// </summary>
		public class MorePeriodException : Exception { }
		/// <summary>
		/// �J�b�R���Ⴄ
		/// </summary>
		public class ParenthesisException : Exception { }
		/// <summary>
		/// �����ȕ�����
		/// </summary>
		public class InvalidStringException : Exception { }
		/// <summary>
		/// ������
		/// </summary>
		public class FormulaEmptyException : Exception { }
		/// <summary>
		/// ���l�Ɖ��Z�q�̏��Ԃ��Ⴄ
		/// </summary>
		public class NumericalOperatorOrderException : Exception { }
		/// <summary>
		/// �萔�A�֐��A���Z�q�Ɏg�p�ł��Ȃ������񂪊܂܂�Ă���
		/// </summary>
		public class CantUsedStringSymbolChar : Exception { }
		/// <summary>
		/// ����������Ă��Ȃ�
		/// </summary>
		public class ParserNotInitialized : Exception { }
	}
}