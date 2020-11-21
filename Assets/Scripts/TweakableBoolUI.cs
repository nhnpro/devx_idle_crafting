using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class TweakableBoolUI : TweakableUI
{
	[SerializeField]
	private Text m_text;

	[SerializeField]
	private Image m_toggle;

	public void Init(TweakableBool tweakable)
	{
		base.TweakableObject = tweakable;
		base.TweakableName = tweakable.TweakableName;
		m_text.text = GetShortName();
		tweakable.Bool.Subscribe(delegate(bool b)
		{
			m_toggle.gameObject.SetActive(b);
		}).AddTo(this);
	}

	public void OnToggle()
	{
		((TweakableBool)base.TweakableObject).Bool.Value = !((TweakableBool)base.TweakableObject).Bool.Value;
	}
}
