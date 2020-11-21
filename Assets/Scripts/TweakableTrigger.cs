using UniRx;
using UnityEngine;

public class TweakableTrigger : Tweakable
{
	public KeyCode m_keyShortCut;

	public ReactiveProperty<bool> Trigger = Observable.Never<bool>().ToReactiveProperty();

	public TweakableTrigger(string name, KeyCode key = KeyCode.Escape)
	{
		base.TweakableName = name;
		TweakableManagerUI.RegisterTweakable(this);
		m_keyShortCut = key;
	}

	public void UpdateKeyShortCut()
	{
		if (m_keyShortCut != KeyCode.Escape && UnityEngine.Input.GetKeyUp(m_keyShortCut))
		{
			Trigger.SetValueAndForceNotify(value: true);
		}
	}
}
