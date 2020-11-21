using Big;
using UniRx;

[PropertyClass]
public class HeroCostRunner
{
	[PropertyBigDouble]
	public ReactiveProperty<BigDouble> Cost;

	[PropertyBool]
	public ReactiveProperty<bool> UpgradeAvailable;

	public ReactiveProperty<int> LevelsToUpgrade;

	private HeroRunner m_heroRunner;

	public int HeroIndex
	{
		get;
		private set;
	}

	public HeroCostRunner(int heroIndex)
	{
		SceneLoader instance = SceneLoader.Instance;
		HeroIndex = heroIndex;
		m_heroRunner = Singleton<HeroTeamRunner>.Instance.GetOrCreateHeroRunner(heroIndex);
		UniRx.IObservable<BigDouble> costOne = m_heroRunner.Level.CombineLatest(Singleton<CumulativeBonusRunner>.Instance.BonusMult[6], (int level, BigDouble mult) => Singleton<EconomyHelpers>.Instance.GetUpgradeCost(heroIndex, level));
		UniRx.IObservable<BigDouble> costTen = m_heroRunner.Level.CombineLatest(Singleton<CumulativeBonusRunner>.Instance.BonusMult[6], (int level, BigDouble mult) => Singleton<EconomyHelpers>.Instance.GetUpgradeCostRepeat(heroIndex, level, 10 - level % 10));
		UniRx.IObservable<BigDouble> costMax = m_heroRunner.Level.CombineLatest(PlayerData.Instance.Coins, Singleton<CumulativeBonusRunner>.Instance.BonusMult[6], (int lvl, BigDouble coins, BigDouble mult) => Singleton<EconomyHelpers>.Instance.GetUpgradeMaxCost(heroIndex, lvl, coins));
		Cost = (from cost in Singleton<UpgradeRunner>.Instance.UpgradeStep.Select(delegate(UpgradeEnum step)
			{
				switch (step)
				{
				case UpgradeEnum.One:
					return costOne;
				case UpgradeEnum.Ten:
					return costTen;
				case UpgradeEnum.Max:
					return costMax;
				default:
					return costOne;
				}
			}).Switch()
			select (!(cost >= new BigDouble(1.0))) ? new BigDouble(1.0) : cost).TakeUntilDestroy(instance).ToReactiveProperty();
		LevelsToUpgrade = Singleton<UpgradeRunner>.Instance.UpgradeStep.CombineLatest(PlayerData.Instance.Coins, m_heroRunner.Level, delegate(UpgradeEnum step, BigDouble coins, int lvl)
		{
			switch (step)
			{
			case UpgradeEnum.One:
				return 1;
			case UpgradeEnum.Ten:
				return 10 - lvl % 10;
			case UpgradeEnum.Max:
				return Singleton<EconomyHelpers>.Instance.GetUpgradeMaxLevels(heroIndex, lvl, coins);
			default:
				return 1;
			}
		}).TakeUntilDestroy(instance).ToReactiveProperty();
		UpgradeAvailable = Cost.CombineLatest(PlayerData.Instance.Coins, (BigDouble cost, BigDouble coins) => cost <= coins).CombineLatest(m_heroRunner.Found, (bool cost, bool found) => cost && found).TakeUntilDestroy(instance)
			.ToReactiveProperty();
	}
}
