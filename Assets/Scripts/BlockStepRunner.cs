using UniRx;

[PropertyClass]
public class BlockStepRunner : Singleton<BlockStepRunner>
{
	[PropertyInt]
	public ReactiveProperty<int> TriggerShowBackpackResource = Observable.Never<int>().ToReactiveProperty();

	public BlockStepRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		for (int i = 0; i < 7; i++)
		{
			BindBlockStep(i);
		}
	}

	private void BindBlockStep(int i)
	{
		SceneLoader instance = SceneLoader.Instance;
		(from amount in PlayerData.Instance.BlocksInBackpack[i]
			where amount % PersistentSingleton<GameSettings>.Instance.BlocksCollectStep[i] == 0
			select amount).Subscribe(delegate
		{
			TriggerShowBackpackResource.SetValueAndForceNotify(i);
		}).AddTo(instance);
	}
}
