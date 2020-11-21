using System.Collections;
using UniRx;
using UnityEngine;

[PropertyClass]
public class PrestigeRunner : Singleton<PrestigeRunner>
{
	private const float TimeBeforePrestigeOverlayActivation = 2f;

	private const float TimeBeforeBaseCampInstantiation = 1f;

	private const float TimeBeforePrestigePopupActivation = 3f;

	public float PrestigeBasecampCameraDelay;

	[PropertyBool]
	public ReactiveProperty<bool> CanPrestige;

	[PropertyInt]
	public ReactiveProperty<int> PrestigeRequirement;

	[PropertyString]
	public ReadOnlyReactiveProperty<string> PrestigeRequirementText;

	public ReactiveProperty<PrestigeOrder> PrestigeTriggered = Observable.Never<PrestigeOrder>().ToReactiveProperty();

	[PropertyBool]
	public ReactiveProperty<bool> SequenceDone = new ReactiveProperty<bool>(initialValue: true);

	public PrestigeRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		SceneLoader instance = SceneLoader.Instance;
		if (PersistentSingleton<GameAnalytics>.Instance != null)
		{
			PersistentSingleton<GameAnalytics>.Instance.PrestigeTriggered = PrestigeTriggered;
		}
		PrestigeRequirement = (from gears in PlayerData.Instance.LifetimeGears
			select (gears >= PersistentSingleton<GameSettings>.Instance.PrestigeRequirements.Length) ? PersistentSingleton<GameSettings>.Instance.PrestigeRequirements[PersistentSingleton<GameSettings>.Instance.PrestigeRequirements.Length - 1] : PersistentSingleton<GameSettings>.Instance.PrestigeRequirements[gears]).TakeUntilDestroy(instance).ToReactiveProperty();
		PrestigeRequirementText = (from req in PrestigeRequirement
			select PersistentSingleton<LocalizationService>.Instance.Text("Prestige.ChunkLevelRequirement", req)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		CanPrestige = PlayerData.Instance.MainChunk.CombineLatest(PrestigeRequirement, (int lvl, int limit) => lvl >= limit).TakeUntilDestroy(instance).ToReactiveProperty();
	}

	public void StartPrestigeSequence()
	{
		if (CanPrestige.Value)
		{
			SceneLoader.Instance.StartCoroutine(StartPrestige());
		}
	}

	private IEnumerator StartPrestige()
	{
		SceneLoader root = SceneLoader.Instance;
		BindingManager bind = BindingManager.Instance;
		SequenceDone.Value = false;
		PrestigeTriggered.SetValueAndForceNotify(PrestigeOrder.PrestigeStart);
		PrestigeBasecampCameraDelay = 1f;
		int chunk = PlayerData.Instance.MainChunk.Value;
		BiomeConfig biome = Singleton<WorldRunner>.Instance.MainBiomeConfig.Value;
		Vector3 pos = bind.CameraCtrl.transform.position.x0z();
		root.StartCoroutine(Singleton<ChunkRunner>.Instance.GeneratePrestigeChunks(pos, chunk, biome));
		yield return new WaitForSeconds(2f);
		bind.PrestigeLoadingParent.ShowInfo();
		yield return new WaitForSeconds(1f);
		bind.PrestigeBagContent.InitializeCards();
		Observable.NextFrame().Subscribe(delegate
		{
			PrestigeTriggered.SetValueAndForceNotify(PrestigeOrder.PrestigeInit);
			PrestigeTriggered.SetValueAndForceNotify(PrestigeOrder.PrestigePost);
			PlayerData.Instance.LifetimePrestiges.Value++;
			PersistentSingleton<MainSaver>.Instance.PleaseSave("prestige_" + PlayerData.Instance.LifetimePrestiges.Value);
		}).AddTo(root);
		yield return new WaitForSeconds(3f);
		bind.PrestigeBagOpeningParent.SetActive(value: true);
		PrestigeBasecampCameraDelay = 0f;
	}

	public void CompletePrestigeSequence()
	{
		SequenceDone.Value = true;
	}
}
