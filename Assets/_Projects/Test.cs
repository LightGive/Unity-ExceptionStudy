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
	StringCalculator _parser = null;

	void Start()
    {
		_parser = new StringCalculator();
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
			var str = _textField.text;
			var val = _parser.Calc(str);
			_textField.SetValueWithoutNotify(val.ToString());
			_labelException.text = string.Empty;
		}
		catch (StringCalculator.FormulaEmptyException)
		{
			_labelException.text = "������ł�";
		}
		catch (StringCalculator.ParenthesisException)
		{
			_labelException.text = "()�̈ʒu���Ⴂ�܂�";
		}
		catch(StringCalculator.MorePeriodException)
		{
			_labelException.text = "�s���I�h�������ł�";
		}
		catch (StringCalculator.NumericalOperatorOrderException)
		{
			_labelException.text = "���l�Ɖ��Z�q�̏��Ԃ��Ⴂ�܂�";
		}
		catch (DivideByZeroException)
		{
			_labelException.text = "�[�����Z�ł�";
		}
	}
}
