using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class BindingManager : MonoBehaviour
{
	[Header("Cameras")]
	[SerializeField]
	public CameraController CameraCtrl;

	[SerializeField]
	public Animator CameraAnimator;

	[SerializeField]
	public GameObject PerspCamera;

	[SerializeField]
	public BlurOptimized Blur;

	[SerializeField]
	public Camera UICamera;

	[SerializeField]
	public Canvas UI;

	[SerializeField]
	public GameObject EntryClouds;

	[Header("UI")]
	[SerializeField]
	public UIPopupManager HeroInfoManager;

	[SerializeField]
	public UICompanionInfoManager CompanionInfoManager;

	[SerializeField]
	public UISkillInfoManager SkillInfoManager;

	[SerializeField]
	public UIPerkInfoManager PerkInfoManager;

	[SerializeField]
	public UICompanionEvolveManager CompanionEvolveManager;

	[SerializeField]
	public UIBossSuccessManager BossSuccessManager;

	[SerializeField]
	public UIPopupManager WelcomeBackParent;

	[SerializeField]
	public UIPopupManager WelcomeBackSuccessParent;

	[SerializeField]
	public UIPopupManager WelcomeBackDoneParent;

	[SerializeField]
	public GameObject PrestigeBagOpeningParent;

	[SerializeField]
	public UIPrestigeBagContentManager PrestigeBagContent;

	[SerializeField]
	public UIPopupManager PrestigeLoadingParent;

	[SerializeField]
	public UIGearInfoManager GearInfoManager;

	[SerializeField]
	public GameObject ChestOpeningParent;

	[SerializeField]
	public GameObject KeyChestAnimObj;

	[SerializeField]
	public GameObject BronzeChestAnimObj;

	[SerializeField]
	public GameObject SilverChestAnimObj;

	[SerializeField]
	public GameObject GoldChestAnimObj;

	[SerializeField]
	public UIPopupManager KeyChestOpeningParent;

	[SerializeField]
	public UIPopupManager GiftRewardParent;

	[SerializeField]
	public UIPopupManager GoldenHammerEntryParent;

	[SerializeField]
	public UIPopupManager GoldenHammerSuccessParent;

	[SerializeField]
	public UIIngameNotifications IngameNotifications;

	[SerializeField]
	public GameObject NotEnoughGemsOverlay;

	[SerializeField]
	public UIPopupManager HeroLevelParent;

	[SerializeField]
	public GameObject MapPanel;

	[SerializeField]
	public GameObject SceneTransition;

	[SerializeField]
	public GameObject LocationTransition;

	[SerializeField]
	public GameObject LocationIntroParent;

	[SerializeField]
	public GameObject MapEntryTransition;

	[SerializeField]
	public GameObject MapExitTransition;

	[SerializeField]
	public Transform NewMaterialFound;

	[SerializeField]
	public Transform NearbyMaterial;

	[SerializeField]
	public GameObject ChunkProgressNotification;

	[SerializeField]
	public Animator ChunkProgressBar;

	[SerializeField]
	public UIPopupManager FacebookConnectionSuccessParent;

	[SerializeField]
	public UIPopupManager FacebookInviteSuccessParent;

	[SerializeField]
	public UIPopupManager FacebookRelinkParent;

	[SerializeField]
	public GameObject CloudSyncPopup;

	[SerializeField]
	public GameObject SystemPopup;

	[SerializeField]
	public UIPopupManager DrillSuccessParent;

	[SerializeField]
	public UIPopupManager DrillEntryParent;

	[SerializeField]
	public UIPopupManager BerryTutorialParent;

	[SerializeField]
	public UIPopupManager KeyTutorialParent;

	[SerializeField]
	public GameObject CustomLevelNode;

	[SerializeField]
	public GameObject CustomLevelFinishedNode;

	[SerializeField]
	public GameObject NewUpdatePopup;

	[SerializeField]
	public UIPopupManager GoatEntryParent;

	[SerializeField]
	public UIPopupManager GoatConfirmParent;

	[SerializeField]
	public UIPopupManager GoatSuccessParent;

	[SerializeField]
	public UIPopupManager GoatLoadingParent;

	[SerializeField]
	public UIPopupManager LanguageConfirmParent;

	[SerializeField]
	public UIMoveMapCamera MapCamera;

	[SerializeField]
	public UIMapMoveJelly MapJelly;

	[SerializeField]
	public UIPopupManager TournamentUnlockedParent;

	[SerializeField]
	public UIPopupManager LevelEditorUnlockedParent;

	[SerializeField]
	public UIPopupManager GoatUnlockedParent;

	[SerializeField]
	public UIPopupManager TournamentComingUpParent;

	[SerializeField]
	public UIPopupManager TournamentAvailableParent;

	[SerializeField]
	public UIPopupManager TournamentRewardsParent;

	[SerializeField]
	public UIPopupManager TournamentRewardsClaimingParent;

	[SerializeField]
	public UIPopupManager TournamentTrophyUnlockedParent;

	[SerializeField]
	public Toggle ShopButton;

	[Header("Skills")]
	[SerializeField]
	public GameObject PrefabTntBlock;

	[SerializeField]
	public GameObject PrefabTntExplosion;

	[SerializeField]
	public GameObject PrefabTntCubeBlock;

	[SerializeField]
	public GameObject PrefabDynamite;

	[SerializeField]
	public GameObject Tornado;

	[SerializeField]
	public Transform TornadoDamageNode;

	[Header("Pools")]
	[SerializeField]
	public SimplePoolManager PlusTextsPool;

	[SerializeField]
	public SimplePoolManager CritTextsPool;

	[SerializeField]
	public SimplePoolManager UITextsPool;

	[SerializeField]
	public SlerpTarget CoinsTarget;

	[SerializeField]
	public SlerpTarget KeysTargetFromSwipe;

	[SerializeField]
	public SlerpTarget UICoinsTarget;

	[SerializeField]
	public SlerpTarget UICardsTarget;

	[SerializeField]
	public SlerpTarget UIRelicsTarget;

	[SerializeField]
	public SlerpTarget UIGemsTarget;

	[SerializeField]
	public SlerpTarget UIKeysTarget;

	[SerializeField]
	public SlerpTarget UIBerryTarget;

	[SerializeField]
	public SlerpTarget UIChestsTarget;

	[SerializeField]
	public SlerpTarget GrassTarget;

	[SerializeField]
	public SlerpTarget DirtTarget;

	[SerializeField]
	public SlerpTarget WoodTarget;

	[SerializeField]
	public SlerpTarget StoneTarget;

	[SerializeField]
	public SlerpTarget MetalTarget;

	[SerializeField]
	public SlerpTarget GoldTarget;

	[Header("Helpers")]
	[SerializeField]
	public Transform BlockContainer;

	[SerializeField]
	public Transform BiomeContainer;

	[SerializeField]
	public BiomeListManager BiomeList;

	[SerializeField]
	public ChapterListManager ChapterList;

	[HideInInspector]
	public AudioGroupComponent MasterVolume;

	[HideInInspector]
	public AudioGroupComponent MusicVolume;

	[HideInInspector]
	public AudioGroupComponent SFXVolume;

	[HideInInspector]
	public AudioGroupComponent UIVolume;

	[HideInInspector]
	public AudioGroupComponent AmbientVolume;

	[HideInInspector]
	public AudioGroupComponent EnvironmentVolume;

	[HideInInspector]
	public AudioGroupComponent CharacterSoundsVolume;

	[HideInInspector]
	public AudioComponent UpgradeBoughtSound;

	public static BindingManager Instance
	{
		get;
		private set;
	}

	public static void Construct()
	{
		Instance = GameObject.Find("BindingManager").GetComponent<BindingManager>();
		Instance.PopulateVolumes();
	}

	public static void Release()
	{
		Instance = null;
	}

	public void PopulateVolumes()
	{
		MasterVolume = AudioBindingManager.Instance.MasterVolume;
		MusicVolume = AudioBindingManager.Instance.MusicVolume;
		SFXVolume = AudioBindingManager.Instance.SFXVolume;
		UIVolume = AudioBindingManager.Instance.UIVolume;
		AmbientVolume = AudioBindingManager.Instance.AmbientVolume;
		EnvironmentVolume = AudioBindingManager.Instance.EnvironmentVolume;
		CharacterSoundsVolume = AudioBindingManager.Instance.CharacterSoundsVolume;
		UpgradeBoughtSound = AudioBindingManager.Instance.UpgradeBoughtSound;
	}

	public SlerpTarget GetSlerpTarget(SlerpTargetEnum target)
	{
		switch (target)
		{
		case SlerpTargetEnum.Coins:
			return CoinsTarget;
		case SlerpTargetEnum.UICoins:
			return UICoinsTarget;
		case SlerpTargetEnum.UICards:
			return UICardsTarget;
		case SlerpTargetEnum.UIRelics:
			return UIRelicsTarget;
		case SlerpTargetEnum.UIGems:
			return UIGemsTarget;
		case SlerpTargetEnum.UIKeys:
			return UIKeysTarget;
		case SlerpTargetEnum.UIBerries:
			return UIBerryTarget;
		case SlerpTargetEnum.UIChests:
			return UIChestsTarget;
		case SlerpTargetEnum.Grass:
			return GrassTarget;
		case SlerpTargetEnum.Dirt:
			return DirtTarget;
		case SlerpTargetEnum.Wood:
			return WoodTarget;
		case SlerpTargetEnum.Stone:
			return StoneTarget;
		case SlerpTargetEnum.Metal:
			return MetalTarget;
		case SlerpTargetEnum.Gold:
			return GoldTarget;
		default:
			return null;
		}
	}
}
