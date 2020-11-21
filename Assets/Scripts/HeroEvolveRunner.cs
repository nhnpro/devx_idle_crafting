using UniRx;
using UnityEngine;

[PropertyClass]
public class HeroEvolveRunner
{
	private HeroRunner m_heroRunner;

	[PropertyString]
	public ReactiveProperty<string> LocalizedName = new ReactiveProperty<string>();

	[PropertyString]
	public ReactiveProperty<string> LocalizedNameDesc = new ReactiveProperty<string>();

	[PropertyInt]
	public ReactiveProperty<int> Tier = new ReactiveProperty<int>();

	[PropertyInt]
	public ReactiveProperty<int> Berries = new ReactiveProperty<int>();

	[PropertyInt]
	public ReactiveProperty<int> UnusedBerries = new ReactiveProperty<int>();

	[PropertyBool]
	public ReactiveProperty<bool> TierUpAvailable = new ReactiveProperty<bool>();

	[PropertyInt]
	public ReactiveProperty<int> Requirement = new ReactiveProperty<int>();

	[PropertyFloat]
	public ReactiveProperty<float> Progress = new ReactiveProperty<float>();

	public HeroEvolveRunner(HeroRunner hero)
	{
		m_heroRunner = hero;
		LocalizedName.Value = m_heroRunner.LocalizedName.Value;
		LocalizedNameDesc.Value = m_heroRunner.LocalizedNameDesc.Value;
		Tier.Value = m_heroRunner.Tier.Value;
		Berries.Value = m_heroRunner.Berries.Value;
		UnusedBerries.Value = m_heroRunner.UnusedBerries.Value;
		TierUpAvailable.Value = m_heroRunner.TierUpAvailable.Value;
		Requirement.Value = Singleton<EconomyHelpers>.Instance.GetTierBerryDeltaReq(Tier.Value);
		Progress.Value = (float)Berries.Value / (float)Requirement.Value;
	}

	public void Animate()
	{
		Berries.Value += UnusedBerries.Value;
		UnusedBerries.Value = 0;
		Progress.Value = Mathf.Min(1f, (float)Berries.Value / (float)Requirement.Value);
	}
}
