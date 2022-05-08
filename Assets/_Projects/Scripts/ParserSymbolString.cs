using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StringCalculator
{
    /// <summary>
    /// ®‚Ì’†‚Ì”’lˆÈŠO‚Ì•¶š—ñ
    /// </summary>
    public abstract class ParserSymbolString
    {
        public abstract string ComparisonStr { get; }
    }

	/// <summary>
	/// ’è”
	/// </summary>
	public abstract class ParserSymbolConstant : ParserSymbolString
    {
        public abstract float ConstValue { get; }
    }

    /// <summary>
    /// ŠÖ”
    /// </summary>
    public abstract class ParserSymbolMethod : ParserSymbolString
    {
        public abstract float Calc(float val);
    }

    /// <summary>
    /// Zp‰‰Zq
    /// </summary>
    public abstract class ParserSymbolOperator : ParserSymbolString
    {
        /// <summary>
        /// ‰‰Zq“à‚Ì—Dæ‡ˆÊ
        /// </summary>
        public abstract int Priority { get; }
        /// <summary>
        /// ŒvZ
        /// </summary>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <returns></returns>
        public abstract float Calc(float val1, float val2);
    }
}