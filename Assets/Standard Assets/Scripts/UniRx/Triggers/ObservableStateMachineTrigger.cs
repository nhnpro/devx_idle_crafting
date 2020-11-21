using UnityEngine;

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableStateMachineTrigger : StateMachineBehaviour
	{
		public class OnStateInfo
		{
			public Animator Animator
			{
				get;
				private set;
			}

			public AnimatorStateInfo StateInfo
			{
				get;
				private set;
			}

			public int LayerIndex
			{
				get;
				private set;
			}

			public OnStateInfo(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
			{
				Animator = animator;
				StateInfo = stateInfo;
				LayerIndex = layerIndex;
			}
		}

		public class OnStateMachineInfo
		{
			public Animator Animator
			{
				get;
				private set;
			}

			public int StateMachinePathHash
			{
				get;
				private set;
			}

			public OnStateMachineInfo(Animator animator, int stateMachinePathHash)
			{
				Animator = animator;
				StateMachinePathHash = stateMachinePathHash;
			}
		}

		private Subject<OnStateInfo> onStateExit;

		private Subject<OnStateInfo> onStateEnter;

		private Subject<OnStateInfo> onStateIK;

		private Subject<OnStateInfo> onStateUpdate;

		private Subject<OnStateMachineInfo> onStateMachineEnter;

		private Subject<OnStateMachineInfo> onStateMachineExit;

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (onStateExit != null)
			{
				onStateExit.OnNext(new OnStateInfo(animator, stateInfo, layerIndex));
			}
		}

		public UniRx.IObservable<OnStateInfo> OnStateExitAsObservable()
		{
			return onStateExit ?? (onStateExit = new Subject<OnStateInfo>());
		}

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (onStateEnter != null)
			{
				onStateEnter.OnNext(new OnStateInfo(animator, stateInfo, layerIndex));
			}
		}

		public UniRx.IObservable<OnStateInfo> OnStateEnterAsObservable()
		{
			return onStateEnter ?? (onStateEnter = new Subject<OnStateInfo>());
		}

		public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (onStateIK != null)
			{
				onStateIK.OnNext(new OnStateInfo(animator, stateInfo, layerIndex));
			}
		}

		public UniRx.IObservable<OnStateInfo> OnStateIKAsObservable()
		{
			return onStateIK ?? (onStateIK = new Subject<OnStateInfo>());
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (onStateUpdate != null)
			{
				onStateUpdate.OnNext(new OnStateInfo(animator, stateInfo, layerIndex));
			}
		}

		public UniRx.IObservable<OnStateInfo> OnStateUpdateAsObservable()
		{
			return onStateUpdate ?? (onStateUpdate = new Subject<OnStateInfo>());
		}

		public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
		{
			if (onStateMachineEnter != null)
			{
				onStateMachineEnter.OnNext(new OnStateMachineInfo(animator, stateMachinePathHash));
			}
		}

		public UniRx.IObservable<OnStateMachineInfo> OnStateMachineEnterAsObservable()
		{
			return onStateMachineEnter ?? (onStateMachineEnter = new Subject<OnStateMachineInfo>());
		}

		public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
		{
			if (onStateMachineExit != null)
			{
				onStateMachineExit.OnNext(new OnStateMachineInfo(animator, stateMachinePathHash));
			}
		}

		public UniRx.IObservable<OnStateMachineInfo> OnStateMachineExitAsObservable()
		{
			return onStateMachineExit ?? (onStateMachineExit = new Subject<OnStateMachineInfo>());
		}
	}
}
