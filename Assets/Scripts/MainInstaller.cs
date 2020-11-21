using DG.Tweening;
using UnityEngine;

public static class MainInstaller
{
	public static void DoAwake()
	{
		ReleaseAll();
		AudioBindingManager.Construct();
		BindingManager.Construct();
		Singleton<PropertyManager>.Construct();
		Singleton<PropertyManager>.Instance.AddRootContext(PlayerData.Instance);
		Singleton<NewUpdateRunner>.Construct();
		Singleton<AORunner>.Construct();
		Singleton<CloudSyncRunner>.Construct();
		Singleton<FacebookRunner>.Construct();
		Singleton<XPromoRunner>.Construct();
		Singleton<ReviewAppRunner>.Construct();
		Singleton<SettingsRunner>.Construct();
		Singleton<CumulativeBonusRunner>.Construct();
		Singleton<AfterPrestigeFundCollector>.Construct();
		Singleton<IAPRunner>.Construct();
		Singleton<AdRunner>.Construct();
		Singleton<AdNetworkCachers>.Construct();
		Singleton<IAPItemCollectionRunner>.Construct();
		Singleton<IAPBundleRunner>.Construct();
		Singleton<EconomyHelpers>.Construct();
		Singleton<EntityPoolManager>.Construct();
		Singleton<QualitySettingsRunner>.Construct();
		Singleton<ResetRunner>.Construct();
		Singleton<FundRunner>.Construct();
		Singleton<HeroTeamRunner>.Construct();
		Singleton<BoosterCollectionRunner>.Construct();
		Singleton<GoldBoosterCollectionRunner>.Construct();
		Singleton<NotEnoughGemsRunner>.Construct();
		Singleton<HammerTimeRunner>.Construct();
		Singleton<ARRunner>.Construct();
		Singleton<BlockStepRunner>.Construct();
		Singleton<DrJellyRunner>.Construct();
		Singleton<TournamentRunner>.Construct();
		Singleton<ChunkRunner>.Construct();
		Singleton<UpgradeRunner>.Construct();
		Singleton<MapRunner>.Construct();
		Singleton<WorldRunner>.Construct();
		Singleton<CreatureCollectionRunner>.Construct();
		Singleton<HeroUnlockRunner>.Construct();
		Singleton<WelcomeBackRunner>.Construct();
		Singleton<DrillRunner>.Construct();
		Singleton<HeroVisibleCountRunner>.Construct();
		Singleton<BossIndexRunner>.Construct();
		Singleton<BossSuccessRunner>.Construct();
		Singleton<BlurRunner>.Construct();
		Singleton<EnableObjectsRunner>.Construct();
		Singleton<GearCollectionRunner>.Construct();
		Singleton<GearBoostRunner>.Construct();
		Singleton<ChestRunner>.Construct();
		Singleton<GearMilestoneRunner>.Construct();
		Singleton<FakeFundRunner>.Construct();
		Singleton<BlockSwipeRunner>.Construct();
		Singleton<MaterialNotificationRunner>.Construct();
		Singleton<LeaderboardRunner>.Construct();
		Singleton<GiftRewardRunner>.Construct();
		Singleton<LevelSkipRunner>.Construct();
		Singleton<PrestigeRunner>.Construct();
		Singleton<GearSetCollectionRunner>.Construct();
		Singleton<PerkMilestoneRunner>.Construct();
		Singleton<DamageRunner>.Construct();
		Singleton<SkillCollectionRunner>.Construct();
		Singleton<PlayerGoalCollectionRunner>.Construct();
		Singleton<TutorialGoalCollectionRunner>.Construct();
		Singleton<PlayerGoalClaimRunner>.Construct();
		Singleton<BossBattleRunner>.Construct();
		Singleton<CameraMoveRunner>.Construct();
		Singleton<HeroNotificaitonRunner>.Construct();
		Singleton<AutoMineRunner>.Construct();
		Singleton<TeamBoostRunner>.Construct();
		Singleton<TapBoostRunner>.Construct();
		Singleton<GoldFingerRunner>.Construct();
		Singleton<TntRunner>.Construct();
		Singleton<CollectableRewardRunner>.Construct();
		Singleton<GamblingAvailableRunner>.Construct();
		Singleton<AudioRunner>.Construct();
		Singleton<TweakableManager>.Construct();
	}

	public static void DoStart()
	{
		DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
		Singleton<PropertyManager>.Instance.InstallScene();
		Singleton<TournamentRunner>.Instance.PostInit();
		Singleton<DrJellyRunner>.Instance.PostInit();
		Singleton<ReviewAppRunner>.Instance.PostInit();
		Singleton<CumulativeBonusRunner>.Instance.PostInit();
		Singleton<IAPRunner>.Instance.PostInit();
		Singleton<IAPBundleRunner>.Instance.PostInit();
		Singleton<QualitySettingsRunner>.Instance.PostInit();
		Singleton<ResetRunner>.Instance.PostInit();
		Singleton<FundRunner>.Instance.PostInit();
		Singleton<ChunkRunner>.Instance.PostInit();
		Singleton<UpgradeRunner>.Instance.PostInit();
		Singleton<MapRunner>.Instance.PostInit();
		Singleton<WorldRunner>.Instance.PostInit();
		Singleton<CreatureCollectionRunner>.Instance.PostInit();
		Singleton<HeroUnlockRunner>.Instance.PostInit();
		Singleton<WelcomeBackRunner>.Instance.PostInit();
		Singleton<DrillRunner>.Instance.PostInit();
		Singleton<ChestRunner>.Instance.PostInit();
		Singleton<HammerTimeRunner>.Instance.PostInit();
		Singleton<LevelSkipRunner>.Instance.PostInit();
		Singleton<BossSuccessRunner>.Instance.PostInit();
		Singleton<PlayerGoalCollectionRunner>.Instance.PostInit();
		Singleton<TutorialGoalCollectionRunner>.Instance.PostInit();
		Singleton<GearMilestoneRunner>.Instance.PostInit();
		Singleton<FacebookRunner>.Instance.PostInit();
		Singleton<AutoMineRunner>.Instance.PostInit();
		Singleton<TntRunner>.Instance.PostInit();
		Singleton<TweakableManager>.Instance.PostInit();
	}

	public static void DoSceneStarted()
	{
		TickerService.StartTickers(SceneLoader.Instance);
		PersistentSingleton<AchievementsService>.Instance.LateInitialize();
		PersistentSingleton<PlayFabService>.Instance.LateInitialize();
		PersistentSingleton<AdColonyProvider>.Instance.LateInitialize();
		AndroidJNIHelper.debug = false;
	}

	public static void ReleaseAll()
	{
		SingletonManager.ReleaseAll();
		BindingManager.Release();
		AudioBindingManager.Release();
	}
}
