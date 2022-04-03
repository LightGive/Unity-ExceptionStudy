using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace StringCalculator
{
    /// <summary>
    /// 文字列から式を生成するためのクラス
    /// </summary>
    public class FormulaParser
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

        Dictionary<string, ParserSymbolString> _dictionaryParseSymbol = null;
        List<ParserSymbolString> _parserList = null;
        public FormulaParser()
        {
            _parserList = new List<ParserSymbolString>();

            //演算子
            //加算
            _parserList.Add(new ParserSymbolOperator("+", (x, y) => x + y, 0));
            //減算
            _parserList.Add(new ParserSymbolOperator("-", (x, y) => x - y, 0));
            //乗算
            _parserList.Add(new ParserSymbolOperator("*", (x, y) => x * y, 1));
            //除算
            _parserList.Add(new ParserSymbolOperator("/", (x, y) =>
            {
                if (y == 0) { throw new DivideByZeroException(); }
                return x / y;
            }, 1));
            //べき乗
            _parserList.Add(new ParserSymbolOperator("^", (x, y) => Mathf.Pow(x, y), 2));

            //定数
            _parserList.Add(new ParserSymbolConstant("pi", Mathf.PI));
            _parserList.Add(new ParserSymbolConstant("radtodeg", Mathf.Rad2Deg));
            _parserList.Add(new ParserSymbolConstant("degtorad", Mathf.Deg2Rad));

            //Dictionary作成
            _dictionaryParseSymbol = new Dictionary<string, ParserSymbolString>();
            foreach (var c in _parserList)
            {
                _dictionaryParseSymbol.Add(c.ComparisonStr, c);
            }
        }

        /// <summary>
        /// 文字列から式を構成するリスト返す
        /// </summary>
        /// <param name="baseStr">元の文字列</param>
        /// <returns></returns>
        public List<FormulaSymbol> GetFormula(string baseStr,int priority =0)
        {
            //スペースを除いた文字列にしておく
            var noSpaceStr = baseStr.Replace(" ", "");

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
                else if (IsCompareString(noSpaceStr, i, out var symbolKey))
                {
                    symbolList.Add(new FormulaSymbolString(priority, symbolKey));
                }
                i += length;
            }
            return null;
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
            throw new FormulaException.ParenthesisException();
        }

        /// <summary>
        /// 数値の判定
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
                    //ピリオドが2つ以上あるかの判定
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
        /// 文字列の判定
        /// </summary>
        /// <returns></returns>
        bool IsCompareString(string str, int startIndex, out string symbolKey)
        {
            var compareStr = "";
            for (var i = startIndex + 1; i < str.Length; i++)
            {
                var targetChar = str[i];
                if (IgnoreChars.Contains(targetChar))
                {
                    compareStr = str.AsSpan(startIndex, i - startIndex).ToString();
                    break;
                }
            }
            symbolKey = "";
            if (!_dictionaryParseSymbol.ContainsKey(compareStr)) 
            {
                throw new FormulaException.InvalidStringException();
            }
            symbolKey = compareStr;
            return true;
        }
    }
}