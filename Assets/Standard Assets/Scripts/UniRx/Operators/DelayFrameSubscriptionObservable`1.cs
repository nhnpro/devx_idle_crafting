using System;

namespace UniRx.Operators
{
	internal class DelayFrameSubscriptionObservable<T> : OperatorObservableBase<T>
	{
		private readonly UniRx.IObservable<T> source;

		private readonly int frameCount;

		private readonly FrameCountType frameCountType;

		public DelayFrameSubscriptionObservable(UniRx.IObservable<T> source, int frameCount, FrameCountType frameCountType)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.frameCount = frameCount;
			this.frameCountType = frameCountType;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			MultipleAssignmentDisposable multipleAssignmentDisposable = new MultipleAssignmentDisposable();
			multipleAssignmentDisposable.Disposable = Observable.TimerFrame(frameCount, frameCountType).SubscribeWithState3(observer, multipleAssignmentDisposable, source, delegate(long _, IObserver<T> o, MultipleAssignmentDisposable disp, UniRx.IObservable<T> s)
			{
				disp.Disposable = s.Subscribe(o);
			});
			return multipleAssignmentDisposable;
		}
	}
}
