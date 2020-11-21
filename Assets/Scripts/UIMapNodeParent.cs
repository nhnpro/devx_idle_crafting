using UniRx;
using UnityEngine;

public class UIMapNodeParent : MonoBehaviour
{
	private ReactiveProperty<bool> m_onEnabled = new ReactiveProperty<bool>();

	private MapNodeRunner m_runner;

	protected void OnEnable()
	{
		m_onEnabled.SetValueAndForceNotify(value: true);
	}

	protected void OnDisable()
	{
		m_runner.DoneAnimating();
	}

	protected void Start()
	{
		Singleton<PropertyManager>.Instance.InstantiateFromResourcesSetParent("Map/MapNode", Vector3.zero, Quaternion.identity, null, base.transform);
		m_runner = (MapNodeRunner)Singleton<PropertyManager>.Instance.GetContext("MapNodeRunner", base.transform);
		Animator animator = GetComponentInChildren<Animator>();
		(from locked in m_runner.Locked.CombineLatest(m_onEnabled, (bool locked, bool enabled) => locked)
			where locked
			select locked).Subscribe(delegate
		{
			animator.SetTrigger("Locked");
		}).AddTo(this);
		(from completed in m_runner.Completed.CombineLatest(m_onEnabled, (bool completed, bool enabled) => completed)
			where completed
			select completed).Subscribe(delegate
		{
			animator.SetTrigger("Completed");
		}).AddTo(this);
		(from current in m_runner.IsCurrent.CombineLatest(m_onEnabled, (bool current, bool enabled) => current)
			where current
			select current).Subscribe(delegate
		{
			animator.SetTrigger("Current");
		}).AddTo(this);
		(from current in m_runner.IsCurrent.CombineLatest(m_onEnabled, (bool current, bool enabled) => current)
			where current
			select current).DelayFrame(2).Subscribe(delegate
		{
			BindingManager.Instance.MapCamera.CenterCamera(base.transform.position);
		}).AddTo(this);
		(from travel in m_runner.TriggerTravel.CombineLatest(m_onEnabled, (bool travel, bool enabled) => travel)
			where travel
			select travel).Subscribe(delegate
		{
			animator.SetTrigger("OnTravel");
		}).AddTo(this);
		(from completed in m_runner.TriggerCompleted.CombineLatest(m_onEnabled, (bool completed, bool enabled) => completed)
			where completed
			select completed).Subscribe(delegate
		{
			animator.SetTrigger("OnCompleted");
		}).AddTo(this);
	}
}
