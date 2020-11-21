using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UIPropertySliderWithMovement : UIPropertyBase
{
	private Slider m_slider;

	private float m_delta;

	private Coroutine coro;

	protected void Start()
	{
		m_slider = GetComponent<Slider>();
		IReadOnlyReactiveProperty<float> property = GetProperty<float>();
		m_slider.value = property.Value;
		property.Delay(TimeSpan.FromSeconds(0.5)).Subscribe(delegate(float target)
		{
			m_delta = target - m_slider.value;
			if (coro != null)
			{
				SceneLoader.Instance.StopCoroutine(coro);
			}
			coro = SceneLoader.Instance.StartCoroutine(StepRoutine(target));
		}).AddTo(this);
	}

	protected IEnumerator StepRoutine(float target)
	{
		if (m_delta <= 0f)
		{
			m_slider.value = target;
			yield break;
		}
		while (m_slider.value < target)
		{
			m_slider.value = Mathf.Min(target, m_slider.value + m_delta * Time.deltaTime);
			yield return null;
		}
	}
}
