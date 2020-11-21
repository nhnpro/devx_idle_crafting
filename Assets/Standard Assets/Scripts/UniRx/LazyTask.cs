using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniRx
{
	public abstract class LazyTask
	{
		public enum TaskStatus
		{
			WaitingToRun,
			Running,
			Completed,
			Canceled,
			Faulted
		}

		protected readonly BooleanDisposable cancellation = new BooleanDisposable();

		public TaskStatus Status
		{
			get;
			protected set;
		}

		public abstract Coroutine Start();

		public void Cancel()
		{
			if (Status == TaskStatus.WaitingToRun || Status == TaskStatus.Running)
			{
				Status = TaskStatus.Canceled;
				cancellation.Dispose();
			}
		}

		public static LazyTask<T> FromResult<T>(T value)
		{
			return LazyTask<T>.FromResult(value);
		}

		public static Coroutine WhenAll(params LazyTask[] tasks)
		{
			return WhenAll(tasks.AsEnumerable());
		}

		public static Coroutine WhenAll(IEnumerable<LazyTask> tasks)
		{
			Coroutine[] coroutines = (from x in tasks
				select x.Start()).ToArray();
			return MainThreadDispatcher.StartCoroutine(WhenAllCore(coroutines));
		}

		private static IEnumerator WhenAllCore(Coroutine[] coroutines)
		{
			for (int i = 0; i < coroutines.Length; i++)
			{
				yield return coroutines[i];
			}
		}
	}
	public class LazyTask<T> : LazyTask
	{
		private readonly UniRx.IObservable<T> source;

		private T result;

		public T Result
		{
			get
			{
				if (base.Status != TaskStatus.Completed)
				{
					throw new InvalidOperationException("Task is not completed");
				}
				return result;
			}
		}

		public Exception Exception
		{
			get;
			private set;
		}

		public LazyTask(UniRx.IObservable<T> source)
		{
			this.source = source;
			base.Status = TaskStatus.WaitingToRun;
		}

		public override Coroutine Start()
		{
			if (base.Status != 0)
			{
				throw new InvalidOperationException("Task already started");
			}
			base.Status = TaskStatus.Running;
			return source.StartAsCoroutine(delegate(T x)
			{
				result = x;
				base.Status = TaskStatus.Completed;
			}, delegate(Exception ex)
			{
				Exception = ex;
				base.Status = TaskStatus.Faulted;
			}, new CancellationToken(cancellation));
		}

		public override string ToString()
		{
			switch (base.Status)
			{
			case TaskStatus.WaitingToRun:
				return "Status:WaitingToRun";
			case TaskStatus.Running:
				return "Status:Running";
			case TaskStatus.Completed:
				return "Status:Completed, Result:" + Result.ToString();
			case TaskStatus.Canceled:
				return "Status:Canceled";
			case TaskStatus.Faulted:
				return "Status:Faulted, Result:" + Result.ToString();
			default:
				return string.Empty;
			}
		}

		public static LazyTask<T> FromResult(T value)
		{
			LazyTask<T> lazyTask = new LazyTask<T>(null);
			lazyTask.result = value;
			lazyTask.Status = TaskStatus.Completed;
			return lazyTask;
		}
	}
}
