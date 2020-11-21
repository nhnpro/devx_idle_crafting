using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using UniRx.InternalUtil;
using UnityEngine;

namespace UniRx
{
	public static class Scheduler
	{
		private class CurrentThreadScheduler : IScheduler
		{
			private static class Trampoline
			{
				public static void Run(SchedulerQueue queue)
				{
					while (queue.Count > 0)
					{
						ScheduledItem scheduledItem = queue.Dequeue();
						if (!scheduledItem.IsCanceled)
						{
							TimeSpan timeout = scheduledItem.DueTime - Time;
							if (timeout.Ticks > 0)
							{
								Thread.Sleep(timeout);
							}
							if (!scheduledItem.IsCanceled)
							{
								scheduledItem.Invoke();
							}
						}
					}
				}
			}

			[ThreadStatic]
			private static SchedulerQueue s_threadLocalQueue;

			[ThreadStatic]
			private static Stopwatch s_clock;

			private static TimeSpan Time
			{
				get
				{
					if (s_clock == null)
					{
						s_clock = Stopwatch.StartNew();
					}
					return s_clock.Elapsed;
				}
			}

			[EditorBrowsable(EditorBrowsableState.Advanced)]
			public static bool IsScheduleRequired => GetQueue() == null;

			public DateTimeOffset Now => Scheduler.Now;

			private static SchedulerQueue GetQueue()
			{
				return s_threadLocalQueue;
			}

			private static void SetQueue(SchedulerQueue newQueue)
			{
				s_threadLocalQueue = newQueue;
			}

			public IDisposable Schedule(Action action)
			{
				return Schedule(TimeSpan.Zero, action);
			}

			public IDisposable Schedule(TimeSpan dueTime, Action action)
			{
				if (action == null)
				{
					throw new ArgumentNullException("action");
				}
				TimeSpan dueTime2 = Time + Normalize(dueTime);
				ScheduledItem scheduledItem = new ScheduledItem(action, dueTime2);
				SchedulerQueue queue = GetQueue();
				if (queue == null)
				{
					queue = new SchedulerQueue(4);
					queue.Enqueue(scheduledItem);
					SetQueue(queue);
					try
					{
						Trampoline.Run(queue);
					}
					finally
					{
						SetQueue(null);
					}
				}
				else
				{
					queue.Enqueue(scheduledItem);
				}
				return scheduledItem.Cancellation;
			}
		}

		private class ImmediateScheduler : IScheduler
		{
			public DateTimeOffset Now => Scheduler.Now;

			public IDisposable Schedule(Action action)
			{
				action();
				return Disposable.Empty;
			}

			public IDisposable Schedule(TimeSpan dueTime, Action action)
			{
				TimeSpan timeout = Normalize(dueTime);
				if (timeout.Ticks > 0)
				{
					Thread.Sleep(timeout);
				}
				action();
				return Disposable.Empty;
			}
		}

		public static class DefaultSchedulers
		{
			private static IScheduler constantTime;

			private static IScheduler tailRecursion;

			private static IScheduler iteration;

			private static IScheduler timeBasedOperations;

			private static IScheduler asyncConversions;

			public static IScheduler ConstantTimeOperations
			{
				get
				{
					return constantTime ?? (constantTime = Immediate);
				}
				set
				{
					constantTime = value;
				}
			}

			public static IScheduler TailRecursion
			{
				get
				{
					return tailRecursion ?? (tailRecursion = Immediate);
				}
				set
				{
					tailRecursion = value;
				}
			}

			public static IScheduler Iteration
			{
				get
				{
					return iteration ?? (iteration = CurrentThread);
				}
				set
				{
					iteration = value;
				}
			}

			public static IScheduler TimeBasedOperations
			{
				get
				{
					return timeBasedOperations ?? (timeBasedOperations = MainThread);
				}
				set
				{
					timeBasedOperations = value;
				}
			}

			public static IScheduler AsyncConversions
			{
				get
				{
					return asyncConversions ?? (asyncConversions = ThreadPool);
				}
				set
				{
					asyncConversions = value;
				}
			}

			public static void SetDotNetCompatible()
			{
				ConstantTimeOperations = Immediate;
				TailRecursion = Immediate;
				Iteration = CurrentThread;
				TimeBasedOperations = ThreadPool;
				AsyncConversions = ThreadPool;
			}
		}

		private class ThreadPoolScheduler : IScheduler, ISchedulerPeriodic, ISchedulerQueueing
		{
			private sealed class Timer : IDisposable
			{
				private static readonly HashSet<System.Threading.Timer> s_timers = new HashSet<System.Threading.Timer>();

				private readonly SingleAssignmentDisposable _disposable;

				private Action _action;

				private System.Threading.Timer _timer;

				private bool _hasAdded;

				private bool _hasRemoved;

				public Timer(TimeSpan dueTime, Action action)
				{
					_disposable = new SingleAssignmentDisposable();
					_disposable.Disposable = Disposable.Create(Unroot);
					_action = action;
					_timer = new System.Threading.Timer(Tick, null, dueTime, TimeSpan.FromMilliseconds(-1.0));
					lock (s_timers)
					{
						if (!_hasRemoved)
						{
							s_timers.Add(_timer);
							_hasAdded = true;
						}
					}
				}

				private void Tick(object state)
				{
					try
					{
						if (!_disposable.IsDisposed)
						{
							_action();
						}
					}
					finally
					{
						Unroot();
					}
				}

				private void Unroot()
				{
					_action = Stubs.Nop;
					System.Threading.Timer timer = null;
					lock (s_timers)
					{
						if (!_hasRemoved)
						{
							timer = _timer;
							_timer = null;
							if (_hasAdded && timer != null)
							{
								s_timers.Remove(timer);
							}
							_hasRemoved = true;
						}
					}
					timer?.Dispose();
				}

				public void Dispose()
				{
					_disposable.Dispose();
				}
			}

			private sealed class PeriodicTimer : IDisposable
			{
				private static readonly HashSet<System.Threading.Timer> s_timers = new HashSet<System.Threading.Timer>();

				private Action _action;

				private System.Threading.Timer _timer;

				private readonly AsyncLock _gate;

				public PeriodicTimer(TimeSpan period, Action action)
				{
					_action = action;
					_timer = new System.Threading.Timer(Tick, null, period, period);
					_gate = new AsyncLock();
					lock (s_timers)
					{
						s_timers.Add(_timer);
					}
				}

				private void Tick(object state)
				{
					_gate.Wait(delegate
					{
						_action();
					});
				}

				public void Dispose()
				{
					System.Threading.Timer timer = null;
					lock (s_timers)
					{
						timer = _timer;
						_timer = null;
						if (timer != null)
						{
							s_timers.Remove(timer);
						}
					}
					if (timer != null)
					{
						timer.Dispose();
						_action = Stubs.Nop;
					}
				}
			}

			public DateTimeOffset Now => Scheduler.Now;

			public IDisposable Schedule(Action action)
			{
				BooleanDisposable d = new BooleanDisposable();
				System.Threading.ThreadPool.QueueUserWorkItem(delegate
				{
					if (!d.IsDisposed)
					{
						action();
					}
				});
				return d;
			}

			public IDisposable Schedule(DateTimeOffset dueTime, Action action)
			{
				return Schedule(dueTime - Now, action);
			}

			public IDisposable Schedule(TimeSpan dueTime, Action action)
			{
				return new Timer(dueTime, action);
			}

			public IDisposable SchedulePeriodic(TimeSpan period, Action action)
			{
				return new PeriodicTimer(period, action);
			}

			public void ScheduleQueueing<T>(ICancelable cancel, T state, Action<T> action)
			{
				System.Threading.ThreadPool.QueueUserWorkItem(delegate(object callBackState)
				{
					if (!cancel.IsDisposed)
					{
						action((T)callBackState);
					}
				}, state);
			}
		}

		private class MainThreadScheduler : IScheduler, ISchedulerPeriodic, ISchedulerQueueing
		{
			private static class QueuedAction<T>
			{
				public static readonly Action<object> Instance = Invoke;

				public static void Invoke(object state)
				{
					Tuple<ICancelable, T, Action<T>> tuple = (UniRx.Tuple<ICancelable, T, Action<T>>)state;
					if (!tuple.Item1.IsDisposed)
					{
						tuple.Item3(tuple.Item2);
					}
				}
			}

			private readonly Action<object> scheduleAction;

			public DateTimeOffset Now => Scheduler.Now;

			public MainThreadScheduler()
			{
				MainThreadDispatcher.Initialize();
				scheduleAction = Schedule;
			}

			private IEnumerator DelayAction(TimeSpan dueTime, Action action, ICancelable cancellation)
			{
				if (dueTime == TimeSpan.Zero)
				{
					yield return null;
				}
				else
				{
					yield return new WaitForSeconds((float)dueTime.TotalSeconds);
				}
				if (!cancellation.IsDisposed)
				{
					MainThreadDispatcher.UnsafeSend(action);
				}
			}

			private IEnumerator PeriodicAction(TimeSpan period, Action action, ICancelable cancellation)
			{
				if (period == TimeSpan.Zero)
				{
					while (true)
					{
						yield return null;
						if (cancellation.IsDisposed)
						{
							break;
						}
						MainThreadDispatcher.UnsafeSend(action);
					}
					yield break;
				}
				float seconds = (float)(period.TotalMilliseconds / 1000.0);
				WaitForSeconds yieldInstruction = new WaitForSeconds(seconds);
				while (true)
				{
					yield return yieldInstruction;
					if (cancellation.IsDisposed)
					{
						break;
					}
					MainThreadDispatcher.UnsafeSend(action);
				}
			}

			private void Schedule(object state)
			{
				Tuple<BooleanDisposable, Action> tuple = (UniRx.Tuple<BooleanDisposable, Action>)state;
				if (!tuple.Item1.IsDisposed)
				{
					tuple.Item2();
				}
			}

			public IDisposable Schedule(Action action)
			{
				BooleanDisposable booleanDisposable = new BooleanDisposable();
				MainThreadDispatcher.Post(scheduleAction, Tuple.Create(booleanDisposable, action));
				return booleanDisposable;
			}

			public IDisposable Schedule(DateTimeOffset dueTime, Action action)
			{
				return Schedule(dueTime - Now, action);
			}

			public IDisposable Schedule(TimeSpan dueTime, Action action)
			{
				BooleanDisposable booleanDisposable = new BooleanDisposable();
				TimeSpan dueTime2 = Normalize(dueTime);
				MainThreadDispatcher.SendStartCoroutine(DelayAction(dueTime2, action, booleanDisposable));
				return booleanDisposable;
			}

			public IDisposable SchedulePeriodic(TimeSpan period, Action action)
			{
				BooleanDisposable booleanDisposable = new BooleanDisposable();
				TimeSpan period2 = Normalize(period);
				MainThreadDispatcher.SendStartCoroutine(PeriodicAction(period2, action, booleanDisposable));
				return booleanDisposable;
			}

			private void ScheduleQueueing<T>(object state)
			{
				Tuple<ICancelable, T, Action<T>> tuple = (UniRx.Tuple<ICancelable, T, Action<T>>)state;
				if (!tuple.Item1.IsDisposed)
				{
					tuple.Item3(tuple.Item2);
				}
			}

			public void ScheduleQueueing<T>(ICancelable cancel, T state, Action<T> action)
			{
				MainThreadDispatcher.Post(QueuedAction<T>.Instance, Tuple.Create(cancel, state, action));
			}
		}

		private class IgnoreTimeScaleMainThreadScheduler : IScheduler, ISchedulerPeriodic, ISchedulerQueueing
		{
			private static class QueuedAction<T>
			{
				public static readonly Action<object> Instance = Invoke;

				public static void Invoke(object state)
				{
					Tuple<ICancelable, T, Action<T>> tuple = (UniRx.Tuple<ICancelable, T, Action<T>>)state;
					if (!tuple.Item1.IsDisposed)
					{
						tuple.Item3(tuple.Item2);
					}
				}
			}

			private readonly Action<object> scheduleAction;

			public DateTimeOffset Now => Scheduler.Now;

			public IgnoreTimeScaleMainThreadScheduler()
			{
				MainThreadDispatcher.Initialize();
				scheduleAction = Schedule;
			}

			private IEnumerator DelayAction(TimeSpan dueTime, Action action, ICancelable cancellation)
			{
				if (dueTime == TimeSpan.Zero)
				{
					yield return null;
					if (!cancellation.IsDisposed)
					{
						MainThreadDispatcher.UnsafeSend(action);
					}
					yield break;
				}
				float elapsed = 0f;
				float dt = (float)dueTime.TotalSeconds;
				do
				{
					yield return null;
					if (cancellation.IsDisposed)
					{
						yield break;
					}
					elapsed += Time.unscaledDeltaTime;
				}
				while (!(elapsed >= dt));
				MainThreadDispatcher.UnsafeSend(action);
			}

			private IEnumerator PeriodicAction(TimeSpan period, Action action, ICancelable cancellation)
			{
				if (period == TimeSpan.Zero)
				{
					while (true)
					{
						yield return null;
						if (cancellation.IsDisposed)
						{
							break;
						}
						MainThreadDispatcher.UnsafeSend(action);
					}
					yield break;
				}
				float elapsed = 0f;
				float dt = (float)period.TotalSeconds;
				while (true)
				{
					yield return null;
					if (cancellation.IsDisposed)
					{
						break;
					}
					elapsed += Time.unscaledDeltaTime;
					if (elapsed >= dt)
					{
						MainThreadDispatcher.UnsafeSend(action);
						elapsed = 0f;
					}
				}
			}

			private void Schedule(object state)
			{
				Tuple<BooleanDisposable, Action> tuple = (UniRx.Tuple<BooleanDisposable, Action>)state;
				if (!tuple.Item1.IsDisposed)
				{
					tuple.Item2();
				}
			}

			public IDisposable Schedule(Action action)
			{
				BooleanDisposable booleanDisposable = new BooleanDisposable();
				MainThreadDispatcher.Post(scheduleAction, Tuple.Create(booleanDisposable, action));
				return booleanDisposable;
			}

			public IDisposable Schedule(DateTimeOffset dueTime, Action action)
			{
				return Schedule(dueTime - Now, action);
			}

			public IDisposable Schedule(TimeSpan dueTime, Action action)
			{
				BooleanDisposable booleanDisposable = new BooleanDisposable();
				TimeSpan dueTime2 = Normalize(dueTime);
				MainThreadDispatcher.SendStartCoroutine(DelayAction(dueTime2, action, booleanDisposable));
				return booleanDisposable;
			}

			public IDisposable SchedulePeriodic(TimeSpan period, Action action)
			{
				BooleanDisposable booleanDisposable = new BooleanDisposable();
				TimeSpan period2 = Normalize(period);
				MainThreadDispatcher.SendStartCoroutine(PeriodicAction(period2, action, booleanDisposable));
				return booleanDisposable;
			}

			public void ScheduleQueueing<T>(ICancelable cancel, T state, Action<T> action)
			{
				MainThreadDispatcher.Post(QueuedAction<T>.Instance, Tuple.Create(cancel, state, action));
			}
		}

		private class FixedUpdateMainThreadScheduler : IScheduler, ISchedulerPeriodic, ISchedulerQueueing
		{
			public DateTimeOffset Now => Scheduler.Now;

			public FixedUpdateMainThreadScheduler()
			{
				MainThreadDispatcher.Initialize();
			}

			private IEnumerator ImmediateAction<T>(T state, Action<T> action, ICancelable cancellation)
			{
				yield return null;
				if (!cancellation.IsDisposed)
				{
					MainThreadDispatcher.UnsafeSend(action, state);
				}
			}

			private IEnumerator DelayAction(TimeSpan dueTime, Action action, ICancelable cancellation)
			{
				if (dueTime == TimeSpan.Zero)
				{
					yield return null;
					if (!cancellation.IsDisposed)
					{
						MainThreadDispatcher.UnsafeSend(action);
					}
					yield break;
				}
				float startTime = Time.fixedTime;
				float dt = (float)dueTime.TotalSeconds;
				float elapsed;
				do
				{
					yield return null;
					if (cancellation.IsDisposed)
					{
						yield break;
					}
					elapsed = Time.fixedTime - startTime;
				}
				while (!(elapsed >= dt));
				MainThreadDispatcher.UnsafeSend(action);
			}

			private IEnumerator PeriodicAction(TimeSpan period, Action action, ICancelable cancellation)
			{
				if (period == TimeSpan.Zero)
				{
					while (true)
					{
						yield return null;
						if (cancellation.IsDisposed)
						{
							break;
						}
						MainThreadDispatcher.UnsafeSend(action);
					}
					yield break;
				}
				float startTime = Time.fixedTime;
				float dt = (float)period.TotalSeconds;
				while (true)
				{
					yield return null;
					if (cancellation.IsDisposed)
					{
						break;
					}
					float ft = Time.fixedTime;
					float elapsed = ft - startTime;
					if (elapsed >= dt)
					{
						MainThreadDispatcher.UnsafeSend(action);
						startTime = ft;
					}
				}
			}

			public IDisposable Schedule(Action action)
			{
				return Schedule(TimeSpan.Zero, action);
			}

			public IDisposable Schedule(DateTimeOffset dueTime, Action action)
			{
				return Schedule(dueTime - Now, action);
			}

			public IDisposable Schedule(TimeSpan dueTime, Action action)
			{
				BooleanDisposable booleanDisposable = new BooleanDisposable();
				TimeSpan dueTime2 = Normalize(dueTime);
				MainThreadDispatcher.StartFixedUpdateMicroCoroutine(DelayAction(dueTime2, action, booleanDisposable));
				return booleanDisposable;
			}

			public IDisposable SchedulePeriodic(TimeSpan period, Action action)
			{
				BooleanDisposable booleanDisposable = new BooleanDisposable();
				TimeSpan period2 = Normalize(period);
				MainThreadDispatcher.StartFixedUpdateMicroCoroutine(PeriodicAction(period2, action, booleanDisposable));
				return booleanDisposable;
			}

			public void ScheduleQueueing<T>(ICancelable cancel, T state, Action<T> action)
			{
				MainThreadDispatcher.StartFixedUpdateMicroCoroutine(ImmediateAction(state, action, cancel));
			}
		}

		private class EndOfFrameMainThreadScheduler : IScheduler, ISchedulerPeriodic, ISchedulerQueueing
		{
			public DateTimeOffset Now => Scheduler.Now;

			public EndOfFrameMainThreadScheduler()
			{
				MainThreadDispatcher.Initialize();
			}

			private IEnumerator ImmediateAction<T>(T state, Action<T> action, ICancelable cancellation)
			{
				yield return null;
				if (!cancellation.IsDisposed)
				{
					MainThreadDispatcher.UnsafeSend(action, state);
				}
			}

			private IEnumerator DelayAction(TimeSpan dueTime, Action action, ICancelable cancellation)
			{
				if (dueTime == TimeSpan.Zero)
				{
					yield return null;
					if (!cancellation.IsDisposed)
					{
						MainThreadDispatcher.UnsafeSend(action);
					}
					yield break;
				}
				float elapsed = 0f;
				float dt = (float)dueTime.TotalSeconds;
				do
				{
					yield return null;
					if (cancellation.IsDisposed)
					{
						yield break;
					}
					elapsed += Time.deltaTime;
				}
				while (!(elapsed >= dt));
				MainThreadDispatcher.UnsafeSend(action);
			}

			private IEnumerator PeriodicAction(TimeSpan period, Action action, ICancelable cancellation)
			{
				if (period == TimeSpan.Zero)
				{
					while (true)
					{
						yield return null;
						if (cancellation.IsDisposed)
						{
							break;
						}
						MainThreadDispatcher.UnsafeSend(action);
					}
					yield break;
				}
				float elapsed = 0f;
				float dt = (float)period.TotalSeconds;
				while (true)
				{
					yield return null;
					if (cancellation.IsDisposed)
					{
						break;
					}
					elapsed += Time.deltaTime;
					if (elapsed >= dt)
					{
						MainThreadDispatcher.UnsafeSend(action);
						elapsed = 0f;
					}
				}
			}

			public IDisposable Schedule(Action action)
			{
				return Schedule(TimeSpan.Zero, action);
			}

			public IDisposable Schedule(DateTimeOffset dueTime, Action action)
			{
				return Schedule(dueTime - Now, action);
			}

			public IDisposable Schedule(TimeSpan dueTime, Action action)
			{
				BooleanDisposable booleanDisposable = new BooleanDisposable();
				TimeSpan dueTime2 = Normalize(dueTime);
				MainThreadDispatcher.StartEndOfFrameMicroCoroutine(DelayAction(dueTime2, action, booleanDisposable));
				return booleanDisposable;
			}

			public IDisposable SchedulePeriodic(TimeSpan period, Action action)
			{
				BooleanDisposable booleanDisposable = new BooleanDisposable();
				TimeSpan period2 = Normalize(period);
				MainThreadDispatcher.StartEndOfFrameMicroCoroutine(PeriodicAction(period2, action, booleanDisposable));
				return booleanDisposable;
			}

			public void ScheduleQueueing<T>(ICancelable cancel, T state, Action<T> action)
			{
				MainThreadDispatcher.StartEndOfFrameMicroCoroutine(ImmediateAction(state, action, cancel));
			}
		}

		public static readonly IScheduler CurrentThread = new CurrentThreadScheduler();

		public static readonly IScheduler Immediate = new ImmediateScheduler();

		public static readonly IScheduler ThreadPool = new ThreadPoolScheduler();

		private static IScheduler mainThread;

		private static IScheduler mainThreadIgnoreTimeScale;

		private static IScheduler mainThreadFixedUpdate;

		private static IScheduler mainThreadEndOfFrame;

		public static bool IsCurrentThreadSchedulerScheduleRequired => CurrentThreadScheduler.IsScheduleRequired;

		public static DateTimeOffset Now => DateTimeOffset.UtcNow;

		public static IScheduler MainThread => mainThread ?? (mainThread = new MainThreadScheduler());

		public static IScheduler MainThreadIgnoreTimeScale => mainThreadIgnoreTimeScale ?? (mainThreadIgnoreTimeScale = new IgnoreTimeScaleMainThreadScheduler());

		public static IScheduler MainThreadFixedUpdate => mainThreadFixedUpdate ?? (mainThreadFixedUpdate = new FixedUpdateMainThreadScheduler());

		public static IScheduler MainThreadEndOfFrame => mainThreadEndOfFrame ?? (mainThreadEndOfFrame = new EndOfFrameMainThreadScheduler());

		public static TimeSpan Normalize(TimeSpan timeSpan)
		{
			return (!(timeSpan >= TimeSpan.Zero)) ? TimeSpan.Zero : timeSpan;
		}

		public static IDisposable Schedule(this IScheduler scheduler, DateTimeOffset dueTime, Action action)
		{
			return scheduler.Schedule(dueTime - scheduler.Now, action);
		}

		public static IDisposable Schedule(this IScheduler scheduler, Action<Action> action)
		{
			CompositeDisposable group = new CompositeDisposable(1);
			object gate = new object();
			Action recursiveAction = null;
			recursiveAction = delegate
			{
				action(delegate
				{
					bool isAdded = false;
					bool isDone = false;
					IDisposable d = null;
					d = scheduler.Schedule(delegate
					{
						lock (gate)
						{
							if (isAdded)
							{
								group.Remove(d);
							}
							else
							{
								isDone = true;
							}
						}
						recursiveAction();
					});
					lock (gate)
					{
						if (!isDone)
						{
							group.Add(d);
							isAdded = true;
						}
					}
				});
			};
			group.Add(scheduler.Schedule(recursiveAction));
			return group;
		}

		public static IDisposable Schedule(this IScheduler scheduler, TimeSpan dueTime, Action<Action<TimeSpan>> action)
		{
			CompositeDisposable group = new CompositeDisposable(1);
			object gate = new object();
			Action recursiveAction = null;
			recursiveAction = delegate
			{
				action(delegate(TimeSpan dt)
				{
					bool isAdded = false;
					bool isDone = false;
					IDisposable d = null;
					d = scheduler.Schedule(dt, delegate
					{
						lock (gate)
						{
							if (isAdded)
							{
								group.Remove(d);
							}
							else
							{
								isDone = true;
							}
						}
						recursiveAction();
					});
					lock (gate)
					{
						if (!isDone)
						{
							group.Add(d);
							isAdded = true;
						}
					}
				});
			};
			group.Add(scheduler.Schedule(dueTime, recursiveAction));
			return group;
		}

		public static IDisposable Schedule(this IScheduler scheduler, DateTimeOffset dueTime, Action<Action<DateTimeOffset>> action)
		{
			CompositeDisposable group = new CompositeDisposable(1);
			object gate = new object();
			Action recursiveAction = null;
			recursiveAction = delegate
			{
				action(delegate(DateTimeOffset dt)
				{
					bool isAdded = false;
					bool isDone = false;
					IDisposable d = null;
					d = scheduler.Schedule(dt, (Action)delegate
					{
						lock (gate)
						{
							if (isAdded)
							{
								group.Remove(d);
							}
							else
							{
								isDone = true;
							}
						}
						recursiveAction();
					});
					lock (gate)
					{
						if (!isDone)
						{
							group.Add(d);
							isAdded = true;
						}
					}
				});
			};
			group.Add(scheduler.Schedule(dueTime, recursiveAction));
			return group;
		}

		public static void SetDefaultForUnity()
		{
			DefaultSchedulers.ConstantTimeOperations = Immediate;
			DefaultSchedulers.TailRecursion = Immediate;
			DefaultSchedulers.Iteration = CurrentThread;
			DefaultSchedulers.TimeBasedOperations = MainThread;
			DefaultSchedulers.AsyncConversions = ThreadPool;
		}
	}
}
