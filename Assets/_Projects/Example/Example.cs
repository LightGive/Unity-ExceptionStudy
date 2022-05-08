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
				_labelException.text = "������ł�";
			}
			catch (StringCalculatorException.ParenthesisException)
			{
				_labelException.text = "()�̈ʒu���Ⴂ�܂�";
			}
			catch (StringCalculatorException.MorePeriodException)
			{
				_labelException.text = "�s���I�h�������ł�";
			}
			catch (StringCalculatorException.NumericalOperatorOrderException)
			{
				_labelException.text = "���l�Ɖ��Z�q�̏��Ԃ��Ⴂ�܂�";
			}
			catch (DivideByZeroException)
			{
				_labelException.text = "�[�����Z�ł�";
			}
			catch (StringCalculatorException.InvalidStringException)
			{
				_labelException.text = "�����ȕ�����ł�";
			}
		}
	}
}