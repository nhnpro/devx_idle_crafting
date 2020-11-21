using System;
using System.Collections;
using UniRx.InternalUtil;
using UnityEngine;

namespace UniRx
{
	public sealed class MainThreadDispatcher : MonoBehaviour
	{
		public enum CullingMode
		{
			Disabled,
			Self,
			All
		}

		public static CullingMode cullingMode = CullingMode.Self;

		private ThreadSafeQueueWorker queueWorker = new ThreadSafeQueueWorker();

		private Action<Exception> unhandledExceptionCallback = delegate(Exception ex)
		{
			UnityEngine.Debug.LogException(ex);
		};

		private MicroCoroutine updateMicroCoroutine;

		private MicroCoroutine fixedUpdateMicroCoroutine;

		private MicroCoroutine endOfFrameMicroCoroutine;

		private static MainThreadDispatcher instance;

		private static bool initialized;

		private static bool isQuitting;

		[ThreadStatic]
		private static object mainThreadToken;

		private Subject<Unit> update;

		private Subject<Unit> lateUpdate;

		private Subject<bool> onApplicationFocus;

		private Subject<bool> onApplicationPause;

		private Subject<Unit> onApplicationQuit;

		public static string InstanceName
		{
			get
			{
				if (instance == null)
				{
					throw new NullReferenceException("MainThreadDispatcher is not initialized.");
				}
				return instance.name;
			}
		}

		public static bool IsInitialized => initialized && instance != null;

		private static MainThreadDispatcher Instance
		{
			get
			{
				Initialize();
				return instance;
			}
		}

		public static bool IsInMainThread => mainThreadToken != null;

		public static void Post(Action<object> action, object state)
		{
			MainThreadDispatcher mainThreadDispatcher = Instance;
			if (!isQuitting && !object.ReferenceEquals(mainThreadDispatcher, null))
			{
				mainThreadDispatcher.queueWorker.Enqueue(action, state);
			}
		}

		public static void Send(Action<object> action, object state)
		{
			if (mainThreadToken != null)
			{
				try
				{
					action(state);
				}
				catch (Exception obj)
				{
					MainThreadDispatcher mainThreadDispatcher = Instance;
					if (mainThreadDispatcher != null)
					{
						mainThreadDispatcher.unhandledExceptionCallback(obj);
					}
				}
			}
			else
			{
				Post(action, state);
			}
		}

		public static void UnsafeSend(Action action)
		{
			try
			{
				action();
			}
			catch (Exception obj)
			{
				MainThreadDispatcher mainThreadDispatcher = Instance;
				if (mainThreadDispatcher != null)
				{
					mainThreadDispatcher.unhandledExceptionCallback(obj);
				}
			}
		}

		public static void UnsafeSend<T>(Action<T> action, T state)
		{
			try
			{
				action(state);
			}
			catch (Exception obj)
			{
				MainThreadDispatcher mainThreadDispatcher = Instance;
				if (mainThreadDispatcher != null)
				{
					mainThreadDispatcher.unhandledExceptionCallback(obj);
				}
			}
		}

		public static void SendStartCoroutine(IEnumerator routine)
		{
			if (mainThreadToken != null)
			{
				StartCoroutine(routine);
				return;
			}
			MainThreadDispatcher mainThreadDispatcher = Instance;
			if (!isQuitting && !object.ReferenceEquals(mainThreadDispatcher, null))
			{
				mainThreadDispatcher.queueWorker.Enqueue(delegate
				{
					MainThreadDispatcher mainThreadDispatcher2 = Instance;
					if (mainThreadDispatcher2 != null)
					{
						((MonoBehaviour)mainThreadDispatcher2).StartCoroutine(routine);
					}
				}, null);
			}
		}

		public static void StartUpdateMicroCoroutine(IEnumerator routine)
		{
			MainThreadDispatcher mainThreadDispatcher = Instance;
			if (mainThreadDispatcher != null)
			{
				mainThreadDispatcher.updateMicroCoroutine.AddCoroutine(routine);
			}
		}

		public static void StartFixedUpdateMicroCoroutine(IEnumerator routine)
		{
			MainThreadDispatcher mainThreadDispatcher = Instance;
			if (mainThreadDispatcher != null)
			{
				mainThreadDispatcher.fixedUpdateMicroCoroutine.AddCoroutine(routine);
			}
		}

		public static void StartEndOfFrameMicroCoroutine(IEnumerator routine)
		{
			MainThreadDispatcher mainThreadDispatcher = Instance;
			if (mainThreadDispatcher != null)
			{
				mainThreadDispatcher.endOfFrameMicroCoroutine.AddCoroutine(routine);
			}
		}

		public new static Coroutine StartCoroutine(IEnumerator routine)
		{
			MainThreadDispatcher mainThreadDispatcher = Instance;
			if (mainThreadDispatcher != null)
			{
				return ((MonoBehaviour)mainThreadDispatcher).StartCoroutine(routine);
			}
			return null;
		}

		public static void RegisterUnhandledExceptionCallback(Action<Exception> exceptionCallback)
		{
			if (exceptionCallback == null)
			{
				Instance.unhandledExceptionCallback = Stubs<Exception>.Ignore;
			}
			else
			{
				Instance.unhandledExceptionCallback = exceptionCallback;
			}
		}

		public static void Initialize()
		{
			if (initialized)
			{
				return;
			}
			MainThreadDispatcher mainThreadDispatcher = null;
			try
			{
				mainThreadDispatcher = UnityEngine.Object.FindObjectOfType<MainThreadDispatcher>();
			}
			catch
			{
				Exception ex = new Exception("UniRx requires a MainThreadDispatcher component created on the main thread. Make sure it is added to the scene before calling UniRx from a worker thread.");
				UnityEngine.Debug.LogException(ex);
				throw ex;
			}
			if (!isQuitting)
			{
				if (mainThreadDispatcher == null)
				{
					new GameObject("MainThreadDispatcher").AddComponent<MainThreadDispatcher>();
				}
				else
				{
					mainThreadDispatcher.Awake();
				}
			}
		}

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
				mainThreadToken = new object();
				initialized = true;
				updateMicroCoroutine = new MicroCoroutine(delegate(Exception ex)
				{
					unhandledExceptionCallback(ex);
				});
				fixedUpdateMicroCoroutine = new MicroCoroutine(delegate(Exception ex)
				{
					unhandledExceptionCallback(ex);
				});
				endOfFrameMicroCoroutine = new MicroCoroutine(delegate(Exception ex)
				{
					unhandledExceptionCallback(ex);
				});
				StartCoroutine(RunUpdateMicroCoroutine());
				StartCoroutine(RunFixedUpdateMicroCoroutine());
				StartCoroutine(RunEndOfFrameMicroCoroutine());
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
			else if (this != instance)
			{
				if (cullingMode == CullingMode.Self)
				{
					UnityEngine.Debug.LogWarning("There is already a MainThreadDispatcher in the scene. Removing myself...");
					DestroyDispatcher(this);
				}
				else if (cullingMode == CullingMode.All)
				{
					UnityEngine.Debug.LogWarning("There is already a MainThreadDispatcher in the scene. Cleaning up all excess dispatchers...");
					CullAllExcessDispatchers();
				}
				else
				{
					UnityEngine.Debug.LogWarning("There is already a MainThreadDispatcher in the scene.");
				}
			}
		}

		private IEnumerator RunUpdateMicroCoroutine()
		{
			while (true)
			{
				yield return null;
				updateMicroCoroutine.Run();
			}
		}

		private IEnumerator RunFixedUpdateMicroCoroutine()
		{
			while (true)
			{
				yield return YieldInstructionCache.WaitForFixedUpdate;
				fixedUpdateMicroCoroutine.Run();
			}
		}

		private IEnumerator RunEndOfFrameMicroCoroutine()
		{
			while (true)
			{
				yield return YieldInstructionCache.WaitForEndOfFrame;
				endOfFrameMicroCoroutine.Run();
			}
		}

		private static void DestroyDispatcher(MainThreadDispatcher aDispatcher)
		{
			if (!(aDispatcher != instance))
			{
				return;
			}
			Component[] components = aDispatcher.gameObject.GetComponents<Component>();
			if (aDispatcher.gameObject.transform.childCount == 0 && components.Length == 2)
			{
				if (components[0] is Transform && components[1] is MainThreadDispatcher)
				{
					UnityEngine.Object.Destroy(aDispatcher.gameObject);
				}
			}
			else
			{
				UnityEngine.Object.Destroy(aDispatcher);
			}
		}

		public static void CullAllExcessDispatchers()
		{
			MainThreadDispatcher[] array = UnityEngine.Object.FindObjectsOfType<MainThreadDispatcher>();
			for (int i = 0; i < array.Length; i++)
			{
				DestroyDispatcher(array[i]);
			}
		}

		private void OnDestroy()
		{
			if (instance == this)
			{
				instance = UnityEngine.Object.FindObjectOfType<MainThreadDispatcher>();
				initialized = (instance != null);
			}
		}

		private void Update()
		{
			if (update != null)
			{
				try
				{
					update.OnNext(Unit.Default);
				}
				catch (Exception obj)
				{
					unhandledExceptionCallback(obj);
				}
			}
			queueWorker.ExecuteAll(unhandledExceptionCallback);
		}

		public static UniRx.IObservable<Unit> UpdateAsObservable()
		{
			return Instance.update ?? (Instance.update = new Subject<Unit>());
		}

		private void LateUpdate()
		{
			if (lateUpdate != null)
			{
				lateUpdate.OnNext(Unit.Default);
			}
		}

		public static UniRx.IObservable<Unit> LateUpdateAsObservable()
		{
			return Instance.lateUpdate ?? (Instance.lateUpdate = new Subject<Unit>());
		}

		private void OnApplicationFocus(bool focus)
		{
			if (onApplicationFocus != null)
			{
				onApplicationFocus.OnNext(focus);
			}
		}

		public static UniRx.IObservable<bool> OnApplicationFocusAsObservable()
		{
			return Instance.onApplicationFocus ?? (Instance.onApplicationFocus = new Subject<bool>());
		}

		private void OnApplicationPause(bool pause)
		{
			if (onApplicationPause != null)
			{
				onApplicationPause.OnNext(pause);
			}
		}

		public static UniRx.IObservable<bool> OnApplicationPauseAsObservable()
		{
			return Instance.onApplicationPause ?? (Instance.onApplicationPause = new Subject<bool>());
		}

		private void OnApplicationQuit()
		{
			isQuitting = true;
			if (onApplicationQuit != null)
			{
				onApplicationQuit.OnNext(Unit.Default);
			}
		}

		public static UniRx.IObservable<Unit> OnApplicationQuitAsObservable()
		{
			return Instance.onApplicationQuit ?? (Instance.onApplicationQuit = new Subject<Unit>());
		}
	}
}
