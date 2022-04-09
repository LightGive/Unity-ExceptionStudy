using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace StringCalculator
{
    /// <summary>
    /// �����񂩂玮�𐶐����邽�߂̃N���X
    /// </summary>
    public class ParserFormula
    {
        const char ParenthesisStart = '(';
        const char ParenthesisEnd = ')';
        const char Period = '.';

        public static ReadOnlyCollection<char> IgnoreChars { get; } = Array.AsReadOnly(new[]
        {
            ParenthesisStart,
            ParenthesisEnd,
            Period,
        });

        public Dictionary<string, ParserSymbolString> DictionaryParseSymbol { get; private set; } = null;
        List<ParserSymbolString> _parserList = null;
        public ParserFormula()
        {
            _parserList = new List<ParserSymbolString>()
            { 
                //���Z
                new ParserSymbolOperator("+", (x, y) => x + y, 0),
                new ParserSymbolOperator("-", (x, y) => x - y, 0),
                new ParserSymbolOperator("*", (x, y) => x * y, 1),
                new ParserSymbolOperator("/", (x, y) =>
                {
                    if (y == 0) { throw new DivideByZeroException(); }
                    return x / y;
                }, 1),
                new ParserSymbolOperator("^", (x, y) => Mathf.Pow(x, y), 2),

                //�萔
                new ParserSymbolConstant("pi", Mathf.PI),
                new ParserSymbolConstant("radtodeg", Mathf.Rad2Deg),
                new ParserSymbolConstant("degtorad", Mathf.Deg2Rad),
            };

            //Dictionary�쐬
            DictionaryParseSymbol = new Dictionary<string, ParserSymbolString>();
            foreach (var c in _parserList)
            {
                foreach(var ignoreChar in IgnoreChars)
				{
                    //TODO:Operator�ƒ萔��֐����̕����񂪔���Ă��Ȃ������`�F�b�N����
					if (c.ComparisonStr.Contains(ignoreChar))
					{
                        throw new FormulaException.CantUsedStringSymbolChar();
					}
				}
                DictionaryParseSymbol.Add(c.ComparisonStr, c);
            }
        }

        /// <summary>
        /// �����񂩂玮���\�����郊�X�g�Ԃ�
        /// </summary>
        /// <param name="baseStr">���̕�����</param>
        /// <returns></returns>
        public List<FormulaSymbol> GetFormula(string baseStr,int priority =0)
        {
            //�X�y�[�X��������������ɂ��Ă���
            var noSpaceStr = baseStr.Replace(" ", "");

            //���Z�q�ƒl�����݂ɓ���郊�X�g
            List<FormulaSymbol> symbolList = new List<FormulaSymbol>();

            int length = 0;
            //�P����������
            for (var i = 0; i < noSpaceStr.Length; i++)
            {
                length = 0;
                if (IsParenthesis(noSpaceStr, i, ref length))
                {
                    var symbol = noSpaceStr.AsSpan(i + 1, length - 2).ToString();
                    symbolList.AddRange(GetFormula(symbol, priority + 1));
                }
                else if (IsNumerical(noSpaceStr, i, ref length))
                {
                    var symbol = noSpaceStr.AsSpan(i, length);
                    symbolList.Add(new FormulaSymbolNumerical(priority, float.Parse(symbol)));
                }
                else if (IsCompareString(noSpaceStr, i, priority, out var symbol,ref length))
                {
                    symbolList.Add(symbol);
                }
                i += length - 1;
            }
            return symbolList;
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="baseStr"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        bool IsParenthesis(string baseStr,int startIndex, ref int length)
        {
            if (baseStr[startIndex] != ParenthesisStart) { return false; }
            var nest = 0;
            for (var i = startIndex + 1; i < baseStr.Length; i++)
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
                    length = i - startIndex + 1;
                    return true;
                }
                else if (targetChar == ParenthesisStart)
                {
                    //�Ⴄ����
                    nest++;
                }
            }

            //�J�b�R�������ɏI������ꍇ
            throw new FormulaException.ParenthesisException();
        }

        /// <summary>
        /// ���l�̔���
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public bool IsNumerical(string str, int startIndex, ref int length)
        {
            if (!char.IsNumber(str[startIndex]) && str[startIndex] != Period)
            {
                return false;
            }
            var isPeriod = false;
            for (var i = startIndex + 1; i < str.Length; i++)
            {
                if (char.IsNumber(str[i]))
                {
                    continue;
                }
                else if (str[i] == Period)
                {
                    //�s���I�h��2�ȏ゠�邩�̔���
                    if (isPeriod) 
                    {
                        throw new FormulaException.MorePeriodException(); 
                    }
                    isPeriod = true;
                    continue;
                }
                else
                {
                    length = i - startIndex;
                    return true;
                }
            }

            length = str.Length - startIndex;
            return true;
        }

        /// <summary>
        /// ������̔���
        /// </summary>
        /// <returns></returns>
        bool IsCompareString(string str, int startIndex, int priority, out FormulaSymbol symbol, ref int length)
        {
            var compareStr = string.Empty;
            for (var i = startIndex; i < str.Length; i++)
            {
                string targetStr = str.Substring(startIndex, i - startIndex + 1);
                if (DictionaryParseSymbol.ContainsKey(targetStr))
                {
                    compareStr = targetStr;
                    break;
                }
            }

			if (compareStr == string.Empty)
			{
                throw new FormulaException.InvalidStringException();
			}
            length = compareStr.Length;
            symbol = null;

            var p = DictionaryParseSymbol[compareStr];
			switch (p.GetType().Name)
			{
                case nameof(ParserSymbolConstant):
                    var parserSymbolConstant = (ParserSymbolConstant)p;
                    symbol = new FormulaSymbolNumerical(priority, parserSymbolConstant.Calc());
                    break;
                case nameof(ParserSymbolOperator):;
                    var parserSymbolOperator = (ParserSymbolOperator)p;
                    symbol = new FormulaSymbolString(priority, parserSymbolOperator);
                    break;
                case nameof(ParserSymbolMethod):
                    throw new Exception("������");
			}
            return true;
        }
    }
}