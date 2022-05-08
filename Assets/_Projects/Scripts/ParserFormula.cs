using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace StringCalculator
{
    /// <summary>
    /// 文字列から式を生成するためのクラス
    /// </summary>
    internal class ParserFormula
    {
        const char ParenthesisStart = '(';
        const char ParenthesisEnd = ')';
        const char Period = '.';

        /// <summary>
        /// 定数や演算子に使えない文字列
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

            //Dictionary作成
            _dictionaryParseSymbol = new Dictionary<string, ParserSymbolString>();
            foreach (var symbol in symbolList)
            {
                var instance = (ParserSymbolString)Activator.CreateInstance(symbol);
                foreach (var ignoreChar in IgnoreChars)
                {
                    //定数、演算子に無効な文字列が含まれているかチェック
                    if (instance.ComparisonStr.Contains(ignoreChar))
                    {
                        throw new StringCalculatorException.CantUsedStringSymbolChar();
                    }
                }
                _dictionaryParseSymbol.Add(instance.ComparisonStr, instance);
            }
        }

        /// <summary>
        /// 文字列から式を構成するリスト返す
        /// </summary>
        /// <param name="baseStr">文字列</param>
        /// <param name="priority">優先度</param>
        /// <returns></returns>
        internal List<FormulaSymbol> GetFormula(string baseStr, int priority = 0)
        {
            //スペースを除いた文字列にしておく
            var noSpaceStr = baseStr.Replace(" ", "");

            if (string.IsNullOrEmpty(noSpaceStr))
            {
                throw new StringCalculatorException.FormulaEmptyException();
            }

            //演算子と値を交互に入れるリスト
            List<FormulaSymbol> symbolList = new List<FormulaSymbol>();

            int length = 0;
            //１文字ずつ処理
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
        /// 括弧
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
                    //正常に終了
                    length = i - startIndex + 1;
                    return true;
                }
                else if (targetChar == ParenthesisStart)
                {
                    //違う括弧
                    nest++;
                }
            }

            //カッコが閉じずに終わった場合
            throw new StringCalculatorException.ParenthesisException();
        }

        /// <summary>
        /// 数値の判定
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
                    //ピリオドが2つ以上あるかの判定
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
        /// 文字列の判定
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
                    throw new Exception("未実装");
            }
            return true;
        }
    }
}