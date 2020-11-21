using System;
using UnityEngine;
using UnityEngine.UI;

namespace UniRx
{
	public static class UnityUIComponentExtensions
	{
		public static IDisposable SubscribeToText(this UniRx.IObservable<string> source, Text text)
		{
			return source.SubscribeWithState(text, delegate(string x, Text t)
			{
				t.text = x;
			});
		}

		public static IDisposable SubscribeToText<T>(this UniRx.IObservable<T> source, Text text)
		{
			return source.SubscribeWithState(text, delegate(T x, Text t)
			{
				t.text = x.ToString();
			});
		}

		public static IDisposable SubscribeToText<T>(this UniRx.IObservable<T> source, Text text, Func<T, string> selector)
		{
			return source.SubscribeWithState2(text, selector, delegate(T x, Text t, Func<T, string> s)
			{
				t.text = s(x);
			});
		}

		public static IDisposable SubscribeToInteractable(this UniRx.IObservable<bool> source, Selectable selectable)
		{
			return source.SubscribeWithState(selectable, delegate(bool x, Selectable s)
			{
				s.interactable = x;
			});
		}

		public static UniRx.IObservable<Unit> OnClickAsObservable(this Button button)
		{
			return button.onClick.AsObservable();
		}

		public static UniRx.IObservable<bool> OnValueChangedAsObservable(this Toggle toggle)
		{
			return Observable.CreateWithState(toggle, delegate(Toggle t, IObserver<bool> observer)
			{
				observer.OnNext(t.isOn);
				return t.onValueChanged.AsObservable().Subscribe(observer);
			});
		}

		public static UniRx.IObservable<float> OnValueChangedAsObservable(this Scrollbar scrollbar)
		{
			return Observable.CreateWithState(scrollbar, delegate(Scrollbar s, IObserver<float> observer)
			{
				observer.OnNext(s.value);
				return s.onValueChanged.AsObservable().Subscribe(observer);
			});
		}

		public static UniRx.IObservable<Vector2> OnValueChangedAsObservable(this ScrollRect scrollRect)
		{
			return Observable.CreateWithState(scrollRect, delegate(ScrollRect s, IObserver<Vector2> observer)
			{
				observer.OnNext(s.normalizedPosition);
				return s.onValueChanged.AsObservable().Subscribe(observer);
			});
		}

		public static UniRx.IObservable<float> OnValueChangedAsObservable(this Slider slider)
		{
			return Observable.CreateWithState(slider, delegate(Slider s, IObserver<float> observer)
			{
				observer.OnNext(s.value);
				return s.onValueChanged.AsObservable().Subscribe(observer);
			});
		}

		public static UniRx.IObservable<string> OnEndEditAsObservable(this InputField inputField)
		{
			return inputField.onEndEdit.AsObservable();
		}

		[Obsolete("onValueChange has been renamed to onValueChanged")]
		public static UniRx.IObservable<string> OnValueChangeAsObservable(this InputField inputField)
		{
			return Observable.CreateWithState(inputField, delegate(InputField i, IObserver<string> observer)
			{
				observer.OnNext(i.text);
				return i.onValueChanged.AsObservable().Subscribe(observer);
			});
		}

		public static UniRx.IObservable<string> OnValueChangedAsObservable(this InputField inputField)
		{
			return Observable.CreateWithState(inputField, delegate(InputField i, IObserver<string> observer)
			{
				observer.OnNext(i.text);
				return i.onValueChanged.AsObservable().Subscribe(observer);
			});
		}
	}
}
