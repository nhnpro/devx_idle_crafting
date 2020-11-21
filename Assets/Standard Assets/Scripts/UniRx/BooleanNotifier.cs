using System;

namespace UniRx
{
	public class BooleanNotifier : UniRx.IObservable<bool>
	{
		private readonly Subject<bool> boolTrigger = new Subject<bool>();

		private bool boolValue;

		public bool Value
		{
			get
			{
				return boolValue;
			}
			set
			{
				boolValue = value;
				boolTrigger.OnNext(value);
			}
		}

		public BooleanNotifier(bool initialValue = false)
		{
			Value = initialValue;
		}

		public void TurnOn()
		{
			if (!Value)
			{
				Value = true;
			}
		}

		public void TurnOff()
		{
			if (Value)
			{
				Value = false;
			}
		}

		public void SwitchValue()
		{
			Value = !Value;
		}

		public IDisposable Subscribe(UniRx.IObserver<bool> observer)
		{
			return boolTrigger.Subscribe(observer);
		}
	}
}
