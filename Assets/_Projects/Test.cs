using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using StringCalculator;

public class Test : MonoBehaviour
{
	UIDocument _document = null;
	TextField _textField = null;
	Label _labelException = null;
	ParserFormula _parser = null;

	void Start()
    {
		_parser = new ParserFormula();

        _document = GetComponent<UIDocument>();
		_textField = _document.rootVisualElement.Q<TextField>();
		_labelException = _document.rootVisualElement.Q<Label>("ExceptionLabel");
		var button = _document.rootVisualElement.Q<Button>();
        button.clicked += OnButtonClicked;
    }

	private void OnButtonClicked()
	{
		try
		{
			var val = Calculator.Calc(_textField.text, _parser);
			_textField.SetValueWithoutNotify(val.ToString());
			_labelException.text = string.Empty;
		}
		catch (StringCalculatorOld.FormulaEmptyException)
		{
			_labelException.text = "式が空です";
		}
		catch (StringCalculatorOld.ParenthesisException)
		{
			_labelException.text = "()の位置が違います";
		}
		catch(StringCalculatorOld.MorePeriodException)
		{
			_labelException.text = "ピリオドが多いです";
		}
		catch (StringCalculatorOld.NumericalOperatorOrderException)
		{
			_labelException.text = "数値と演算子の順番が違います";
		}
		catch (DivideByZeroException)
		{
			_labelException.text = "ゼロ除算です";
		}
		catch (StringCalculatorOld.InvalidStringException)
		{
			_labelException.text = "無効な文字列です";
		}
	}
}
