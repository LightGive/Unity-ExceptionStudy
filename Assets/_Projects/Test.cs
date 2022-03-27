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
	FormulaParser _parser = null;

	void Start()
    {
		_parser = new FormulaParser();
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
			_labelException.text = val.ToString();
		}
		catch (FormulaParser.ParenthesisException)
		{
			_labelException.text = "()�̈ʒu���Ⴂ�܂�";
		}
		catch(FormulaParser.MorePeriodException)
		{
			_labelException.text = "�s���I�h������";
		}
		catch (FormulaParser.NumericalOperatorOrder)
		{
			_labelException.text = "���l�Ɖ��Z�q�̏��Ԃ��Ⴄ";
		}
		catch (DivideByZeroException)
		{
			_labelException.text = "�[�����Z";
		}
	}
}
