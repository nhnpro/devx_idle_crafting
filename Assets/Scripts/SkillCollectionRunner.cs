using UniRx;

public class SkillCollectionRunner : Singleton<SkillCollectionRunner>
{
	public UniRx.IObservable<SkillsEnum> SkillUnlocked;

	private SkillRunner[] m_skillRunners = new SkillRunner[5];

	public SkillCollectionRunner()
	{
		SkillUnlocked = CreateUnlockedObservable();
	}

	public void ResetCooldowns()
	{
		for (int i = 0; i < m_skillRunners.Length; i++)
		{
			m_skillRunners[i].ResetCooldown();
		}
	}

	public UniRx.IObservable<SkillsEnum> CreateUnlockedObservable()
	{
		UniRx.IObservable<SkillsEnum> observable = Observable.Never<SkillsEnum>();
		for (int i = 0; i < 5; i++)
		{
			observable = observable.Merge(GetOrCreateSkillRunner((SkillsEnum)i).UnlockTriggered);
		}
		return observable;
	}

	public SkillRunner GetOrCreateSkillRunner(SkillsEnum skill)
	{
		if (m_skillRunners[(int)skill] == null)
		{
			m_skillRunners[(int)skill] = new SkillRunner(skill);
		}
		return m_skillRunners[(int)skill];
	}
}
