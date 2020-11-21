using System;
using UnityEngine;

namespace UniRx.Operators
{
	internal class FrameTimeIntervalObservable<T> : OperatorObservableBase<TimeInterval<T>>
	{
		private class FrameTimeInterval : OperatorObserverBase<T, TimeInterval<T>>
		{
			private readonly FrameTimeIntervalObservable<T> parent;

			private float lastTime;

			public FrameTimeInterval(FrameTimeIntervalObservable<T> parent, IObserver<TimeInterval<T>> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
				lastTime = ((!parent.ignoreTimeScale) ? Time.time : Time.unscaledTime);
			}

			public override void OnNext(T value)
			{
				float num = (!parent.ignoreTimeScale) ? Time.time : Time.unscaledTime;
				float num2 = num - lastTime;
				lastTime = num;
				observer.OnNext(new TimeInterval<T>(value, TimeSpan.FromSeconds(num2)));
			}

			public override void OnError(Exception error)
			{
				try
				{
					observer.OnError(error);
				}
				finally
				{
					Dispose();
				}
			}

			public override void OnCompleted()
			{
				try
				{
					observer.OnCompleted();
				}
				finally
				{
					Dispose();
				}
			}
		}

		private readonly UniRx.IObservable<T> source;

		private readonly bool ignoreTimeScale;

		public FrameTimeIntervalObservable(UniRx.IObservable<T> source, bool ignoreTimeScale)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.ignoreTimeScale = ignoreTimeScale;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<TimeInterval<T>> observer, IDisposable cancel)
		{
			return source.Subscribe(new FrameTimeInterval(this, observer, cancel));
		}
	}
}
