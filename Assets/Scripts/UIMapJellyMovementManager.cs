using UniRx;
using UnityEngine;

public class UIMapJellyMovementManager : MonoBehaviour
{
	private ReactiveProperty<bool> m_onEnabled = new ReactiveProperty<bool>();

	private MapNodeRunner m_runner;

	private bool m_transitionedTo;

	private bool m_transitionedFrom;

	protected void OnEnable()
	{
		m_onEnabled.SetValueAndForceNotify(value: true);
	}

	private void Start()
	{
		if (PersistentSingleton<GameSettings>.Instance.MapJellyOn)
		{
			m_runner = (MapNodeRunner)Singleton<PropertyManager>.Instance.GetContext("MapNodeRunner", base.transform);
			(from _ in m_onEnabled
				where PlayerData.Instance.LifetimeChunk.Value >= m_runner.NodeIndex * 10 && PlayerData.Instance.LifetimeChunk.Value < (m_runner.NodeIndex + 1) * 10
				select _).DelayFrame(1).Subscribe(delegate
			{
				BindingManager.Instance.MapJelly.Move(base.transform.position);
			}).AddTo(this);
			(from completed in m_runner.Completed.CombineLatest(m_onEnabled, (bool completed, bool enabled) => completed).DelayFrame(2)
				where completed && PlayerData.Instance.LifetimeChunk.Value == (m_runner.NodeIndex + 1) * 10
				select completed).Subscribe(delegate
			{
				if (Singleton<WorldRunner>.Instance.HaveProgressedInCurrentSession.Value && !m_transitionedFrom)
				{
					BindingManager.Instance.MapJelly.Move(base.transform.position);
					m_transitionedFrom = true;
				}
			}).AddTo(this);
			(from current in m_runner.IsCurrent.CombineLatest(m_onEnabled, (bool current, bool enabled) => current).DelayFrame(3)
				where current && PlayerData.Instance.LifetimeChunk.Value == m_runner.NodeIndex * 10
				select current).Subscribe(delegate
			{
				if (Singleton<WorldRunner>.Instance.HaveProgressedInCurrentSession.Value && !m_transitionedTo)
				{
					BindingManager.Instance.MapJelly.DecreaseJelly(base.transform.position);
					m_transitionedTo = true;
				}
			}).AddTo(this);
		}
	}
}
