using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace StringCalculator
{
    /// <summary>
    /// �����񂩂玮�𐶐����邽�߂̃N���X
    /// </summary>
    internal class ParserFormula
    {
        const char ParenthesisStart = '(';
        const char ParenthesisEnd = ')';
        const char Period = '.';

        /// <summary>
        /// �萔�≉�Z�q�Ɏg���Ȃ�������
        /// </summary>
        ReadOnlyCollection<char> IgnoreChars { get; } = Array.AsReadOnly(new[]
        {
            ParenthesisStart,
            ParenthesisEnd,
            Period,
        });

        Dictionary<string, ParserSymbolString> _dictionaryParseSymbol = null;
        public ParserFormula()
        {
            var symbolList = Assembly.GetAssembly(typeof(StringCalculator.StringCalculatorSymbolList))
                .GetTypes()
                .Where(x => x.IsSubclassOf(typeof(ParserSymbolString)) && !x.IsAbstract)
                .ToArray();

            //Dictionary�쐬
            _dictionaryParseSymbol = new Dictionary<string, ParserSymbolString>();
            foreach (var symbol in symbolList)
            {
                var instance = (ParserSymbolString)Activator.CreateInstance(symbol);
                foreach (var ignoreChar in IgnoreChars)
                {
                    //�萔�A���Z�q�ɖ����ȕ����񂪊܂܂�Ă��邩�`�F�b�N
                    if (instance.ComparisonStr.Contains(ignoreChar))
                    {
                        throw new StringCalculatorException.CantUsedStringSymbolChar();
                    }
                }
                _dictionaryParseSymbol.Add(instance.ComparisonStr, instance);
            }
        }

        /// <summary>
        /// �����񂩂玮���\�����郊�X�g�Ԃ�
        /// </summary>
        /// <param name="baseStr">������</param>
        /// <param name="priority">�D��x</param>
        /// <returns></returns>
        internal List<FormulaSymbol> GetFormula(string baseStr, int priority = 0)
        {
            //�X�y�[�X��������������ɂ��Ă���
            var noSpaceStr = baseStr.Replace(" ", "");

            if (string.IsNullOrEmpty(noSpaceStr))
            {
                throw new StringCalculatorException.FormulaEmptyException();
            }

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
                else if (IsCompareString(noSpaceStr, i, priority, out var symbol, ref length))
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
            throw new StringCalculatorException.ParenthesisException();
        }

        /// <summary>
        /// ���l�̔���
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        bool IsNumerical(string str, int startIndex, ref int length)
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
                        throw new StringCalculatorException.MorePeriodException(); 
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
                if (_dictionaryParseSymbol.ContainsKey(targetStr))
                {
                    compareStr = targetStr;
                    break;
                }
            }

			if (compareStr == string.Empty)
			{
                throw new StringCalculatorException.InvalidStringException();
			}

            length = compareStr.Length;
            symbol = null;

            var p = _dictionaryParseSymbol[compareStr];
            switch (p)
            {
                case ParserSymbolConstant:
                    var parserSymbolConstant = (ParserSymbolConstant)p;
                    symbol = new FormulaSymbolNumerical(priority, parserSymbolConstant.ConstValue);
                    break;
                case ParserSymbolOperator:
                    var parserSymbolOperator = (ParserSymbolOperator)p;
                    symbol = new FormulaSymbolString(priority, parserSymbolOperator);
                    break;
                case ParserSymbolMethod:
                    throw new Exception("������");
            }
            return true;
        }
    }
}