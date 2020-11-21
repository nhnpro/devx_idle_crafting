using UnityEngine;
using UnityEngine.UI;

public class TweakableTriggerUI : TweakableUI
{
	[SerializeField]
	private Text m_text;

	public void Init(TweakableTrigger tweakable)
	{
		base.TweakableObject = tweakable;
		base.TweakableName = tweakable.TweakableName;
		m_text.text = GetShortName();
	}

	public void OnClick()
	{
		((TweakableTrigger)base.TweakableObject).Trigger.SetValueAndForceNotify(value: true);
	}
}
