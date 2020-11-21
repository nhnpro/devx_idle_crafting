using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OneAtATimeButton : MonoBehaviour
{
	private static bool pressed;

	public Button.ButtonClickedEvent OnClick = new Button.ButtonClickedEvent();

	public void Awake()
	{
		ReadOnlyReactiveProperty<bool> source = (from _ in GetComponent<Button>().OnClickAsObservable()
			select pressed).TakeUntilDestroy(this).ToReadOnlyReactiveProperty();
		(from p in source
			where !p
			select p).Subscribe(delegate
		{
			invoke();
		});
		(from p in source
			where p
			select p).Subscribe(delegate
		{
		});
	}

	private void invoke()
	{
		pressed = true;
		Observable.NextFrame().Subscribe(delegate
		{
			pressed = false;
		});
		try
		{
			OnClick.Invoke();
		}
		catch (Exception)
		{
		}
	}
}
