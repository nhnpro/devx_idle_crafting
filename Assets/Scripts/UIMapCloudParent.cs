using UniRx;
using UnityEngine;

public class UIMapCloudParent : MonoBehaviour
{
	private ReactiveProperty<bool> m_onEnabled = new ReactiveProperty<bool>();

	protected void OnEnable()
	{
		m_onEnabled.SetValueAndForceNotify(value: true);
	}

	private void Start()
	{
		int num = (int)Singleton<PropertyManager>.Instance.GetContext("Chapter", base.transform);
		int startingLevel = num * 4 * 10;
		Animator animator = GetComponent<Animator>();
		(from open in PlayerData.Instance.LifetimeChunk.CombineLatest(m_onEnabled, Singleton<WorldRunner>.Instance.HaveProgressedInCurrentSession, (int chunk, bool enabled, bool progressed) => chunk == startingLevel && enabled && progressed)
			where open
			select open).Take(1).Subscribe(delegate
		{
			animator.SetTrigger("OnOpen");
		}).AddTo(this);
		(from opened in PlayerData.Instance.LifetimeChunk.CombineLatest(m_onEnabled, Singleton<WorldRunner>.Instance.HaveProgressedInCurrentSession, (int chunk, bool enabled, bool progressed) => chunk == startingLevel && enabled && !progressed)
			where opened
			select opened).Take(1).Subscribe(delegate
		{
			animator.SetTrigger("OnOpened");
		}).AddTo(this);
		(from opened in PlayerData.Instance.LifetimeChunk.CombineLatest(m_onEnabled, (int chunk, bool enabled) => chunk > startingLevel && enabled)
			where opened
			select opened).Take(1).Subscribe(delegate
		{
			animator.SetTrigger("OnOpened");
		}).AddTo(this);
		(from chunk in PlayerData.Instance.LifetimeChunk
			where chunk == 0
			select chunk).Take(1).Subscribe(delegate
		{
			animator.SetTrigger("OnIntro");
		}).AddTo(this);
	}
}
