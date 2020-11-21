using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class TweakableLabelUI : TweakableUI
{
	[SerializeField]
	private Text m_text;

	public void Init(TweakableLabel tweakable)
	{
		base.TweakableObject = tweakable;
		base.TweakableName = tweakable.TweakableName;
		tweakable.Text.Subscribe(delegate(string t)
		{
			m_text.text = t;
		}).AddTo(this);
	}
}
