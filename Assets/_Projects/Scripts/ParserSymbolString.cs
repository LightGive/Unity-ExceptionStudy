using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StringCalculator
{
    /// <summary>
    /// ���̒��̐��l�ȊO�̕�����
    /// </summary>
    public abstract class ParserSymbolString
    {
        public abstract string ComparisonStr { get; }
    }

	/// <summary>
	/// �萔
	/// </summary>
	public abstract class ParserSymbolConstant : ParserSymbolString
    {
        public abstract float ConstValue { get; }
    }

    /// <summary>
    /// �֐�
    /// </summary>
    public abstract class ParserSymbolMethod : ParserSymbolString
    {
        public abstract float Calc(float val);
    }

    /// <summary>
    /// �Z�p���Z�q
    /// </summary>
    public abstract class ParserSymbolOperator : ParserSymbolString
    {
        /// <summary>
        /// ���Z�q���̗D�揇��
        /// </summary>
        public abstract int Priority { get; }
        /// <summary>
        /// �v�Z
        /// </summary>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <returns></returns>
        public abstract float Calc(float val1, float val2);
    }
}