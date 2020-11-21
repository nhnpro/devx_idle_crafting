using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class TweakableFloatUI : TweakableUI
{
	[SerializeField]
	private Text m_text;

	[SerializeField]
	private Text m_value;

	[SerializeField]
	private Slider m_slider;

	public void Init(TweakableFloat tweakable)
	{
		base.TweakableObject = tweakable;
		base.TweakableName = tweakable.TweakableName;
		m_text.text = GetShortName();
		tweakable.Float.Subscribe(delegate(float f)
		{
			m_value.text = f.ToString("0.00");
			m_slider.value = (f - tweakable.Min) / (tweakable.Max - tweakable.Min);
		}).AddTo(this);
	}

	public void OnChanged()
	{
		TweakableFloat tweakableFloat = (TweakableFloat)base.TweakableObject;
		tweakableFloat.Float.Value = m_slider.value * (tweakableFloat.Max - tweakableFloat.Min) + tweakableFloat.Min;
	}
}
