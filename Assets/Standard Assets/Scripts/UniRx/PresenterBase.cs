using System;
using UnityEngine;

namespace UniRx
{
	public abstract class PresenterBase : PresenterBase<Unit>
	{
		protected sealed override void BeforeInitialize(Unit argument)
		{
			BeforeInitialize();
		}

		protected abstract void BeforeInitialize();

		protected override void Initialize(Unit argument)
		{
			Initialize();
		}

		public void ForceInitialize()
		{
			ForceInitialize(Unit.Default);
		}

		protected abstract void Initialize();
	}
	public abstract class PresenterBase<T> : MonoBehaviour, IPresenter
	{
		protected static readonly IPresenter[] EmptyChildren = new IPresenter[0];

		private int childrenCount;

		private int currentCalledCount;

		private bool isAwaken;

		private bool isInitialized;

		private bool isStartedCapturePhase;

		private Subject<Unit> initializeSubject;

		private IPresenter[] children;

		private IPresenter parent;

		private T argument = default(T);

		public IPresenter Parent => parent;

		protected abstract IPresenter[] Children
		{
			get;
		}

		public UniRx.IObservable<Unit> InitializeAsObservable()
		{
			if (isInitialized)
			{
				return Observable.Return(Unit.Default);
			}
			return initializeSubject ?? (initializeSubject = new Subject<Unit>());
		}

		public void PropagateArgument(T argument)
		{
			this.argument = argument;
		}

		protected abstract void BeforeInitialize(T argument);

		protected abstract void Initialize(T argument);

		public virtual void ForceInitialize(T argument)
		{
			Awake();
			PropagateArgument(argument);
			Start();
		}

		void IPresenter.ForceInitialize(object argument)
		{
			ForceInitialize((T)argument);
		}

		void IPresenter.Awake()
		{
			if (!isAwaken)
			{
				isAwaken = true;
				children = Children;
				childrenCount = children.Length;
				for (int i = 0; i < children.Length; i++)
				{
					IPresenter presenter = children[i];
					presenter.RegisterParent(this);
					presenter.Awake();
				}
				OnAwake();
			}
		}

		protected void Awake()
		{
			((IPresenter)this).Awake();
		}

		protected virtual void OnAwake()
		{
		}

		protected void Start()
		{
			if (isStartedCapturePhase)
			{
				return;
			}
			IPresenter presenter = parent;
			if (presenter == null)
			{
				presenter = this;
			}
			else
			{
				while (presenter.Parent != null)
				{
					presenter = presenter.Parent;
				}
			}
			presenter.StartCapturePhase();
		}

		void IPresenter.StartCapturePhase()
		{
			isStartedCapturePhase = true;
			BeforeInitialize(argument);
			for (int i = 0; i < children.Length; i++)
			{
				IPresenter presenter = children[i];
				presenter.StartCapturePhase();
			}
			if (children.Length == 0)
			{
				Initialize(argument);
				isInitialized = true;
				if (initializeSubject != null)
				{
					initializeSubject.OnNext(Unit.Default);
					initializeSubject.OnCompleted();
				}
				if (parent != null)
				{
					parent.InitializeCore();
				}
			}
		}

		void IPresenter.RegisterParent(IPresenter parent)
		{
			if (this.parent != null)
			{
				throw new InvalidOperationException("PresenterBase can't register multiple parent. Name:" + base.name);
			}
			this.parent = parent;
		}

		void IPresenter.InitializeCore()
		{
			currentCalledCount++;
			if (childrenCount == currentCalledCount)
			{
				Initialize(argument);
				isInitialized = true;
				if (initializeSubject != null)
				{
					initializeSubject.OnNext(Unit.Default);
					initializeSubject.OnCompleted();
				}
				if (parent != null)
				{
					parent.InitializeCore();
				}
			}
		}
	}
}
