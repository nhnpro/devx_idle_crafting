using Big;
using System.Collections.Generic;
using UniRx;

[PropertyClass]
public class PlayerData : BaseData
{
	private static PlayerData m_instance;

	[PropertyBigDouble]
	public ReactiveProperty<BigDouble> Coins = new ReactiveProperty<BigDouble>(0L);

	public ReactiveProperty<BigDouble> WelcomebackCoins = new ReactiveProperty<BigDouble>(0L);

	[PropertyBigDouble]
	public ReactiveProperty<BigDouble> LifetimeCoins = new ReactiveProperty<BigDouble>(0L);

	[PropertyInt]
	public ReactiveProperty<int> LifetimePrestiges = new ReactiveProperty<int>(0);

	[PropertyInt]
	public ReactiveProperty<int> RetryLevelNumber = new ReactiveProperty<int>(0);

	[PropertyInt]
	public ReactiveProperty<int> Gems = new ReactiveProperty<int>(0);

	[PropertyInt]
	public ReactiveProperty<int> LifetimeGems = new ReactiveProperty<int>(0);

	[PropertyInt]
	public ReactiveProperty<int> Keys = new ReactiveProperty<int>(0);

	[PropertyInt]
	public ReactiveProperty<int> LifetimeKeys = new ReactiveProperty<int>(0);

	[PropertyInt]
	public ReactiveProperty<int> Medals = new ReactiveProperty<int>(0);

	[PropertyInt]
	public ReactiveProperty<int> LifetimeMedals = new ReactiveProperty<int>(0);

	[PropertyInt]
	public ReactiveProperty<int> Trophies = new ReactiveProperty<int>(0);

	public ReactiveProperty<int> LifetimeGamblings = new ReactiveProperty<int>(0);

	public List<ReactiveProperty<int>> BoostersBought;

	public List<ReactiveProperty<float>> BoostersEffect;

	public ReactiveProperty<long> HammerTimeElapsedTime = new ReactiveProperty<long>(85536000000000L);

	public ReactiveProperty<int> HammerTimeBonusDuration = new ReactiveProperty<int>(0);

	public ReactiveProperty<int> CurrentBundleEnum = new ReactiveProperty<int>(int.MinValue);

	public ReactiveProperty<int> CurrentBundleGearIndex = new ReactiveProperty<int>(0);

	public ReactiveProperty<long> BundleTimeStamp = new ReactiveProperty<long>(0L);

	public ReactiveProperty<int> LifetimeBundles = new ReactiveProperty<int>(0);

	[PropertyLong]
	public ReactiveProperty<long> LifetimeBlocksTaps = new ReactiveProperty<long>(0L);

	[PropertyInt]
	public ReactiveProperty<int> LifetimeCreatures = new ReactiveProperty<int>(0);

	public ReactiveProperty<int> LifetimeGears = new ReactiveProperty<int>(0);

	public ReactiveProperty<int> LifetimeGearLevels = new ReactiveProperty<int>(0);

	public List<ReactiveProperty<long>> LifetimeBlocksDestroyed;

	public List<ReactiveProperty<long>> BlocksInBackpack;

	public List<ReactiveProperty<long>> BlocksCollected;

	[PropertyLong]
	public ReactiveProperty<long> LifetimeRelics = new ReactiveProperty<long>(0L);

	[PropertyLong]
	public ReactiveProperty<long> LifetimeBerries = new ReactiveProperty<long>(0L);

	public List<ReactiveProperty<int>> GearChestsToCollect;

	[PropertyInt]
	public ReactiveProperty<int> NormalChests = new ReactiveProperty<int>(0);

	[PropertyInt]
	public ReactiveProperty<int> SilverChests = new ReactiveProperty<int>(0);

	[PropertyInt]
	public ReactiveProperty<int> GoldChests = new ReactiveProperty<int>(0);

	[PropertyInt]
	public ReactiveProperty<int> LifetimeAllOpenedChests = new ReactiveProperty<int>(0);

	public ReactiveCollection<string> PlayerGoalsSeen;

	[PropertyInt]
	public ReactiveProperty<int> TutorialStep = new ReactiveProperty<int>(0);

	[PropertyInt]
	public ReactiveProperty<int> ARTutorialStep = new ReactiveProperty<int>(0);

	[PropertyInt]
	public ReactiveProperty<int> MainChunk = new ReactiveProperty<int>(0);

	public ReactiveProperty<int> BonusChunk = new ReactiveProperty<int>(-1);

	[PropertyInt]
	public ReactiveProperty<int> LifetimeChunk = new ReactiveProperty<int>(0);

	public ReactiveProperty<bool> BiomeStarted = new ReactiveProperty<bool>(initialValue: true);

	public List<ReactiveProperty<int>> LevelSkipsBought;

	public ReactiveProperty<long> GamblingTimeStamp = new ReactiveProperty<long>(0L);

	public ReactiveProperty<GamblingState> Gambling = new ReactiveProperty<GamblingState>(new GamblingState());

	public List<SkillState> SkillStates;

	public List<HeroState> HeroStates;

	public List<GearState> GearStates;

	public List<PlayerGoalState> PlayerGoalStates;

	public ReactiveProperty<bool> BossFailedLastTime = new ReactiveProperty<bool>(initialValue: false);

	public ReactiveProperty<long> WelcomebackTimeStamp = new ReactiveProperty<long>(0L);

	public ReactiveProperty<long> DrillTimeStamp = new ReactiveProperty<long>(0L);

	[PropertyInt]
	public ReactiveProperty<int> DrillLevel = new ReactiveProperty<int>(0);

	public ReactiveProperty<int> DrJellyLevel = new ReactiveProperty<int>(-1);

	public ReactiveProperty<int> DrJellySpawningLevel = new ReactiveProperty<int>(-1);

	public ReactiveProperty<long> TournamentTimeStamp = new ReactiveProperty<long>(0L);

	public ReactiveProperty<int> TournamentIdCurrent = new ReactiveProperty<int>(-1);

	public ReactiveProperty<long> TournamentLastPointOnline = new ReactiveProperty<long>(-1L);

	public ReactiveProperty<short[]> TournamentRun = new ReactiveProperty<short[]>(new short[90]);

	public ReactiveProperty<string> TournamentEnteringDevice = new ReactiveProperty<string>(string.Empty);

	public ReactiveProperty<string> DisplayName = new ReactiveProperty<string>(string.Empty);

	public ReactiveProperty<bool> AREditorChosen = new ReactiveProperty<bool>(initialValue: false);

	public ReactiveProperty<Dictionary<string, string>> PlayFabToFBIds = Observable.Never<Dictionary<string, string>>().ToReactiveProperty();

	[PropertyBool]
	public ReactiveProperty<bool> AndroidEarlyAccess = new ReactiveProperty<bool>(initialValue: false);

	public static PlayerData Instance
	{
		get
		{
			if (m_instance == null)
			{
				m_instance = new PlayerData();
			}
			return m_instance;
		}
	}
}
