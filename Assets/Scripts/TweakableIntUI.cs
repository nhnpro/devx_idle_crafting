using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class TweakableIntUI : TweakableUI
{
	[SerializeField]
	private Text m_text;

	[SerializeField]
	private Text m_value;

	[SerializeField]
	private Slider m_slider;

	private int m_changedFrame = -1;

	public void Init(TweakableInt tweakable)
	{
		base.TweakableObject = tweakable;
		base.TweakableName = tweakable.TweakableName;
		m_text.text = GetShortName();
		tweakable.Int.Subscribe(delegate(int i)
		{
			m_value.text = i.ToString();
			if (m_changedFrame != Time.frameCount)
			{
				m_slider.value = (float)(i - tweakable.Min) / (float)(tweakable.Max - tweakable.Min);
			}
		}).AddTo(this);
	}

	public void OnChanged()
	{
		TweakableInt tweakableInt = (TweakableInt)base.TweakableObject;
		m_changedFrame = Time.frameCount;
		tweakableInt.Int.Value = Mathf.RoundToInt(m_slider.value * (float)(tweakableInt.Max - tweakableInt.Min) + (float)tweakableInt.Min);
	}
}
