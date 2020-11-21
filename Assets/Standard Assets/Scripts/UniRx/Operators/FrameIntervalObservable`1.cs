using System;
using UnityEngine;

namespace UniRx.Operators
{
	internal class FrameIntervalObservable<T> : OperatorObservableBase<FrameInterval<T>>
	{
		private class FrameInterval : OperatorObserverBase<T, FrameInterval<T>>
		{
			private int lastFrame;

			public FrameInterval(UniRx.IObserver<FrameInterval<T>> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				lastFrame = Time.frameCount;
			}

			public override void OnNext(T value)
			{
				int frameCount = Time.frameCount;
				int interval = frameCount - lastFrame;
				lastFrame = frameCount;
				observer.OnNext(new FrameInterval<T>(value, interval));
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

		public FrameIntervalObservable(UniRx.IObservable<T> source)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<FrameInterval<T>> observer, IDisposable cancel)
		{
			return source.Subscribe(new FrameInterval(observer, cancel));
		}
	}
}
