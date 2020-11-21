using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class TweakableInputFieldUI : TweakableUI
{
	[SerializeField]
	private Text m_text;

	[SerializeField]
	private InputField m_inputField;

	private TweakableInputField m_input;

	public void Init(TweakableInputField tweakable)
	{
		base.TweakableObject = tweakable;
		m_input = tweakable;
		base.TweakableName = tweakable.TweakableName;
		m_text.text = GetShortName();
		tweakable.Input.Subscribe(delegate(string t)
		{
			m_inputField.text = t;
		}).AddTo(this);
	}

	public void OnEndEdit(string text)
	{
		m_input.Input.Value = text;
	}
}
