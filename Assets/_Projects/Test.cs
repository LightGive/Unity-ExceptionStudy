using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Test : MonoBehaviour
{
	UIDocument _document = null;
	TextField _textField = null;
	Label _labelException = null;
	StringCalculatorOld _parser = null;

	void Start()
    {
		_parser = new StringCalculatorOld();
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
			var val = _parser.Calc(_textField.text);
			_textField.SetValueWithoutNotify(val.ToString());
			_labelException.text = string.Empty;
		}
		catch (StringCalculatorOld.FormulaEmptyException)
		{
			_labelException.text = "������ł�";
		}
		catch (StringCalculatorOld.ParenthesisException)
		{
			_labelException.text = "()�̈ʒu���Ⴂ�܂�";
		}
		catch(StringCalculatorOld.MorePeriodException)
		{
			_labelException.text = "�s���I�h�������ł�";
		}
		catch (StringCalculatorOld.NumericalOperatorOrderException)
		{
			_labelException.text = "���l�Ɖ��Z�q�̏��Ԃ��Ⴂ�܂�";
		}
		catch (DivideByZeroException)
		{
			_labelException.text = "�[�����Z�ł�";
		}
		catch (StringCalculatorOld.InvalidStringException)
		{
			_labelException.text = "�����ȕ�����ł�";
		}
	}
}
