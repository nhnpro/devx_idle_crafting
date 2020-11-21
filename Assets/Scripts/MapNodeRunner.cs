using UniRx;

[PropertyClass]
public class MapNodeRunner
{
	[PropertyBool]
	public ReactiveProperty<bool> Locked = new ReactiveProperty<bool>(initialValue: false);

	public ReactiveProperty<bool> Completed = new ReactiveProperty<bool>(initialValue: false);

	public ReactiveProperty<bool> IsCurrent = new ReactiveProperty<bool>(initialValue: false);

	public ReactiveProperty<bool> TriggerCompleted = Observable.Never<bool>().ToReactiveProperty();

	public ReactiveProperty<bool> TriggerTravel = Observable.Never<bool>().ToReactiveProperty();

	public int NodeIndex
	{
		get;
		private set;
	}

	public MapNodeRunner(int node)
	{
		SceneLoader instance = SceneLoader.Instance;
		NodeIndex = node;
		Locked = (from chunk in PlayerData.Instance.MainChunk
			select NodeIndex > chunk / 10).TakeUntilDestroy(instance).ToReactiveProperty();
		Completed = (from chunk in PlayerData.Instance.MainChunk
			select NodeIndex < chunk / 10).TakeUntilDestroy(instance).ToReactiveProperty();
		IsCurrent = (from chunk in Singleton<WorldRunner>.Instance.MainChunk
			select NodeIndex == chunk / 10).CombineLatest(PlayerData.Instance.BiomeStarted, (bool n, bool started) => n && started).TakeUntilDestroy(instance).ToReactiveProperty();
	}

	public int GetBiomeIndex()
	{
		int chunk = NodeIndex * 10;
		return Singleton<EconomyHelpers>.Instance.GetBiomeConfig(chunk).BiomeIndex;
	}

	public void AnimateCompleted()
	{
		TriggerCompleted.SetValueAndForceNotify(value: true);
	}

	public void AnimateTravel()
	{
		TriggerTravel.SetValueAndForceNotify(value: true);
	}

	public void DoneAnimating()
	{
		TriggerCompleted.Value = false;
		TriggerTravel.Value = false;
	}
}
