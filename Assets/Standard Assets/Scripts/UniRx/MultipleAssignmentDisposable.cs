using System;

namespace UniRx
{
	public sealed class MultipleAssignmentDisposable : IDisposable, ICancelable
	{
		private static readonly BooleanDisposable True = new BooleanDisposable(isDisposed: true);

		private object gate = new object();

		private IDisposable current;

		public bool IsDisposed
		{
			get
			{
				lock (gate)
				{
					return current == True;
				}
			}
		}

		public IDisposable Disposable
		{
			get
			{
				lock (gate)
				{
					return (current != True) ? current : UniRx.Disposable.Empty;
				}
			}
			set
			{
				bool flag = false;
				lock (gate)
				{
					flag = (current == True);
					if (!flag)
					{
						current = value;
					}
				}
				if (flag)
				{
					value?.Dispose();
				}
			}
		}

		public void Dispose()
		{
			IDisposable disposable = null;
			lock (gate)
			{
				if (current != True)
				{
					disposable = current;
					current = True;
				}
			}
			disposable?.Dispose();
		}
	}
}
