using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UIPropertySlider : UIPropertyBase
{
	private Slider m_slider;

	protected void Start()
	{
		m_slider = GetComponent<Slider>();
		IReadOnlyReactiveProperty<float> property = GetProperty<float>();
		property.TakeUntilDestroy(this).Subscribe(delegate(float val)
		{
			m_slider.value = val;
		}).AddTo(this);
	}
}
