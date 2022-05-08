using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace StringCalculator.Example
{
	public class Example : MonoBehaviour
	{
		UIDocument _document = null;
		TextField _textField = null;
		Label _labelException = null;

		void Start()
		{
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
				var val = Calculator.Calc(_textField.text);
				_textField.SetValueWithoutNotify(val.ToString());
				_labelException.text = string.Empty;
			}
			catch (StringCalculatorException.FormulaEmptyException)
			{
				_labelException.text = "式が空です";
			}
			catch (StringCalculatorException.ParenthesisException)
			{
				_labelException.text = "()の位置が違います";
			}
			catch (StringCalculatorException.MorePeriodException)
			{
				_labelException.text = "ピリオドが多いです";
			}
			catch (StringCalculatorException.NumericalOperatorOrderException)
			{
				_labelException.text = "数値と演算子の順番が違います";
			}
			catch (DivideByZeroException)
			{
				_labelException.text = "ゼロ除算です";
			}
			catch (StringCalculatorException.InvalidStringException)
			{
				_labelException.text = "無効な文字列です";
			}
		}
	}
}