using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StringCalculator
{
    public abstract class ParserSymbolBase { }

    /// <summary>
    /// ‰‰Zq
    /// </summary>
    public abstract class ParserSymbolString : ParserSymbolBase
    {
        public ParserSymbolString(string str)
        {
            ComparisonStr = str;
        }

        public string ComparisonStr { get; private set; }
    }


	/// <summary>
	/// ’è”
	/// </summary>
	public class ParserSymbolConstant : ParserSymbolString
    {
        float _val;
        public ParserSymbolConstant(string str, float val) : base(str)
        {
            _val = val;
        }
        public float Calc() => _val;
    }

    /// <summary>
    /// ŠÖ”
    /// </summary>
    public class ParserSymbolMethod : ParserSymbolString
    {
        public delegate float CalcDelegate(float val);
        CalcDelegate _calcAction;
        public ParserSymbolMethod(string str, CalcDelegate calcDelegate) : base(str)
        {
            _calcAction = calcDelegate;
        }
        public float Calc(float val) => _calcAction(val);
    }

    /// <summary>
    /// Zp‰‰Zq
    /// </summary>
    public class ParserSymbolOperator : ParserSymbolString
    {
        public delegate float CalcDelegate(float val1, float val2);
        CalcDelegate _calcAction;
        /// <summary>
        /// ‰‰Zq“à‚Ì—Dæ‡ˆÊ
        /// </summary>
        public int Priority { get; private set; }
        public ParserSymbolOperator(string str, CalcDelegate calcDelegate,int priority) : base(str)
        {
            _calcAction = calcDelegate;
            Priority = priority;
        }
        public float Calc(float val1, float val2) => _calcAction(val1, val2);
    }
}