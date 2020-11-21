using Spine;
using Spine.Unity;
using UnityEngine;

public class AnimationStateMachine
{
	private int m_nextState;

	private AnimState[] m_states;

	private SkeletonAnimation m_animation;

	public int NextState
	{
		get
		{
			return m_nextState;
		}
		set
		{
			m_nextState = value;
			QueueState = -1;
		}
	}

	public int QueueState
	{
		get;
		set;
	}

	public int CurrentState
	{
		get;
		private set;
	}

	public int OverrideState
	{
		get;
		private set;
	}

	public TrackEntry CurrentTrack
	{
		get;
		private set;
	}

	public TrackEntry OverrideTrack
	{
		get;
		private set;
	}

	public Skeleton Skeleton => m_animation.skeleton;

	public AnimationStateMachine(int size, SkeletonAnimation animation)
	{
		m_states = new AnimState[size];
		m_animation = animation;
		m_animation.state.Complete += OnAnimComplete;
		m_animation.state.Complete += OnOverrideAnimComplete;
		m_animation.state.Event += OnAnimEvent;
		m_animation.state.Data.DefaultMix = 0f;
		QueueState = -1;
		CurrentState = -1;
		OverrideState = -1;
	}

	public void Add(AnimState state)
	{
		m_states[state.Index] = state;
	}

	public void Step()
	{
		if (CurrentState == -1 || NextState != CurrentState)
		{
			ChangeState(NextState);
		}
		if (m_states[CurrentState].Step != null)
		{
			m_states[CurrentState].Step();
		}
	}

	private void ChangeState(int index)
	{
		if (CurrentState != -1 && m_states[CurrentState].Exit != null)
		{
			m_states[CurrentState].Exit();
		}
		CurrentState = index;
		PlayAnim();
		if (m_states[CurrentState].Enter != null)
		{
			m_states[CurrentState].Enter();
		}
	}

	private void PlayAnim()
	{
		AnimState animState = m_states[CurrentState];
		string animationName = animState.AnimationNames[Random.Range(0, animState.AnimationNames.Length)];
		CurrentTrack = m_animation.state.SetAnimation(0, animationName, animState.Looping);
		CurrentTrack.TimeScale = Random.Range(animState.Speed.Min, animState.Speed.Max);
		CurrentTrack.MixDuration = animState.MixDuration;
	}

	public void PlayOverrideAnim(int index)
	{
		OverrideState = index;
		AnimState animState = m_states[OverrideState];
		string animationName = animState.AnimationNames[Random.Range(0, animState.AnimationNames.Length)];
		OverrideTrack = m_animation.state.SetAnimation(1, animationName, animState.Looping);
		OverrideTrack.TimeScale = Random.Range(animState.Speed.Min, animState.Speed.Max);
	}

	public void PlayOverrideAnimOrdered(int index, int animIndex)
	{
		OverrideState = index;
		AnimState animState = m_states[OverrideState];
		string animationName = animState.AnimationNames[animIndex];
		OverrideTrack = m_animation.state.SetAnimation(1, animationName, animState.Looping);
		OverrideTrack.TimeScale = Random.Range(animState.Speed.Min, animState.Speed.Max);
	}

	private void OnAnimComplete(TrackEntry trackEntry)
	{
		if (trackEntry.trackIndex != 0)
		{
			return;
		}
		AnimState animState = m_states[CurrentState];
		if (animState.AnimComplete != null)
		{
			for (int i = 0; i < animState.AnimationNames.Length; i++)
			{
				if (animState.AnimationNames[i] == trackEntry.animation.Name)
				{
					animState.AnimComplete();
					break;
				}
			}
		}
		if (QueueState != -1)
		{
			NextState = QueueState;
			QueueState = -1;
		}
		else if (animState.Looping)
		{
			PlayAnim();
		}
	}

	private void OnOverrideAnimComplete(TrackEntry trackEntry)
	{
		if (trackEntry.trackIndex == 0 || OverrideState < 0)
		{
			return;
		}
		AnimState animState = m_states[OverrideState];
		if (animState.AnimComplete != null)
		{
			for (int i = 0; i < animState.AnimationNames.Length; i++)
			{
				if (animState.AnimationNames[i] == trackEntry.animation.Name)
				{
					animState.AnimComplete();
					break;
				}
			}
		}
		if (!animState.Looping)
		{
			OverrideTrack = m_animation.state.SetEmptyAnimation(1, 0f);
			OverrideState = -1;
		}
	}

	private void OnAnimEvent(TrackEntry trackEntry, Spine.Event evt)
	{
		if (m_states[CurrentState].AnimEvent != null)
		{
			m_states[CurrentState].AnimEvent(evt.Data.Name);
		}
	}
}
