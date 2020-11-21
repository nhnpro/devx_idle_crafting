using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UniRx;
using UnityEngine;

[PropertyClass]
public class WorldRunner : Singleton<WorldRunner>
{
	public const int ProgressBarsPerBiome = 1;

	public const int ChunksPerProgressBar = 10;

	public ReadOnlyReactiveProperty<BiomeConfig> MainBiomeConfig;

	public ReadOnlyReactiveProperty<BiomeConfig> CurrentBiomeConfig;

	public ReadOnlyReactiveProperty<ChunkStruct> CurrentChunk;

	public ReactiveProperty<bool> MapSequence = new ReactiveProperty<bool>(initialValue: false);

	[PropertyInt]
	public ReactiveProperty<int> MainChunk;

	[PropertyInt]
	public ReactiveProperty<int> RelativeChunk;

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> InBaseCamp;

	[PropertyString]
	public ReadOnlyReactiveProperty<string> BiomeName;

	[PropertyString]
	public ReadOnlyReactiveProperty<string> ProgressString;

	[PropertyFloat]
	public ReadOnlyReactiveProperty<float> CurrentProgress;

	[PropertyInt]
	public ReadOnlyReactiveProperty<int> CurrentProgressInt;

	public ReactiveProperty<bool> HaveProgressedInCurrentSession = new ReactiveProperty<bool>(initialValue: false);

	private bool m_closeSequenceOn;

	[CompilerGenerated]
	private static Func<float, int> _003C_003Ef__mg_0024cache0;

	public WorldRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		SceneLoader instance = SceneLoader.Instance;
		BindingManager bind = BindingManager.Instance;
		CurrentChunk = PlayerData.Instance.MainChunk.CombineLatest(PlayerData.Instance.BonusChunk, (int main, int bonus) => (bonus <= -1) ? new ChunkStruct(main, bonus: false) : new ChunkStruct(bonus, bonus: true)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		MainBiomeConfig = (from chunk in PlayerData.Instance.MainChunk
			select Singleton<EconomyHelpers>.Instance.GetBiomeConfig(chunk)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		CurrentBiomeConfig = (from cs in CurrentChunk
			select Singleton<EconomyHelpers>.Instance.GetBiomeConfig(cs.Index)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		MainChunk = PlayerData.Instance.MainChunk;
		RelativeChunk = PlayerData.Instance.MainChunk;
		InBaseCamp = (from chunk in CurrentChunk
			select chunk.Index == 0).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		BiomeName = (from cnfg in CurrentBiomeConfig
			select PersistentSingleton<LocalizationService>.Instance.Text("Biome.Name." + cnfg.BiomeIndex)).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		ProgressString = (from chunk in CurrentChunk
			where chunk.Index >= 0
			select Mathf.FloorToInt(chunk.Index % 10 / 10) + 1 into curr
			select curr.ToString() + "/" + 1.ToString()).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		CurrentProgress = (from chunk in CurrentChunk
			select (float)((chunk.Index - CurrentBiomeConfig.Value.Chunk) % 10) + 1f).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		CurrentProgressInt = CurrentProgress.Select(Mathf.RoundToInt).TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		(from seq in MapSequence.Pairwise()
			where !seq.Current && seq.Previous
			select seq).Delay(TimeSpan.FromSeconds(1.5)).Subscribe(delegate
		{
			bind.ChunkProgressNotification.SetActive(value: true);
		}).AddTo(instance);
		(from chunk in CurrentChunk.Pairwise()
			select chunk.Previous.Index < chunk.Current.Index into chunkChanged
			where chunkChanged
			select chunkChanged).Delay(TimeSpan.FromSeconds(3.0)).Subscribe(delegate
		{
			bind.ChunkProgressNotification.SetActive(value: true);
		}).AddTo(instance);
		(from chunk in CurrentChunk.Pairwise()
			select chunk.Previous.Index < chunk.Current.Index into progressed
			where progressed
			select progressed).Subscribe(delegate(bool progressed)
		{
			HaveProgressedInCurrentSession.SetValueAndForceNotify(progressed);
		}).AddTo(instance);
	}

	public void PostInit()
	{
		SceneLoader instance = SceneLoader.Instance;
		(from chunk in PlayerData.Instance.MainChunk
			where chunk > PlayerData.Instance.LifetimeChunk.Value
			select chunk).Subscribe(delegate(int chunk)
		{
			PlayerData.Instance.LifetimeChunk.Value = chunk;
		}).AddTo(instance);
		(from order in Singleton<PrestigeRunner>.Instance.PrestigeTriggered
			where order == PrestigeOrder.PrestigeInit
			select order).Subscribe(delegate
		{
			ResetWorld();
		}).AddTo(instance);
	}

	public void StartTransition()
	{
		BindingManager.Instance.LocationTransition.SetActive(value: true);
	}

	public void GoToMap()
	{
		MapSequence.SetValueAndForceNotify(value: true);
		BindingManager.Instance.MapEntryTransition.SetActive(value: true);
		BindingManager.Instance.MapPanel.SetActive(value: true);
	}

	public void CloseMap()
	{
		if (!m_closeSequenceOn)
		{
			BindingManager.Instance.StartCoroutine(CloseMapSequence());
		}
	}

	public IEnumerator CloseMapSequence()
	{
		m_closeSequenceOn = true;
		StartTransition();
		BindingManager.Instance.MapExitTransition.SetActive(value: true);
		if (!CurrentChunk.Value.Bonus && ChunkRunner.IsLastChunkForBiome(CurrentChunk.Value.Index - 1))
		{
			GameObject intro = BindingManager.Instance.BiomeList.Biomes[CurrentBiomeConfig.Value.BiomeIndex].Intro;
			GameObject gameObject = UnityEngine.Object.Instantiate(intro);
			gameObject.transform.SetParent(BindingManager.Instance.LocationIntroParent.transform, worldPositionStays: false);
		}
		yield return new WaitForSeconds(0.5f);
		BindingManager.Instance.MapPanel.SetActive(value: false);
		BindingManager.Instance.MapExitTransition.SetActive(value: false);
		BindingManager.Instance.MapEntryTransition.SetActive(value: false);
		MapSequence.SetValueAndForceNotify(value: false);
		Singleton<EnableObjectsRunner>.Instance.MapCloseButton.Value = true;
		m_closeSequenceOn = false;
	}

	public void LevelSkip()
	{
		SceneLoader.Instance.StartCoroutine(LevelSkipSequence());
	}

	public IEnumerator LevelSkipSequence()
	{
		MapSequence.SetValueAndForceNotify(value: true);
		BindingManager.Instance.GoatLoadingParent.ShowInfo();
		yield return new WaitForSeconds(1.5f);
		BindingManager.Instance.GoatEntryParent.HideInfo();
		Singleton<ChunkRunner>.Instance.ResetChunks();
		yield return new WaitForSeconds(0.5f);
		Singleton<ChunkRunner>.Instance.SpawnGameLoaded();
		yield return new WaitForSeconds(2.5f);
		BindingManager.Instance.GoatSuccessParent.ShowInfo();
		MapSequence.SetValueAndForceNotify(value: false);
	}

	public void IncreaseChunk()
	{
		PlayerData.Instance.MainChunk.Value++;
	}

	private void ResetWorld()
	{
		PlayerData.Instance.MainChunk.Value = 0;
	}
}
