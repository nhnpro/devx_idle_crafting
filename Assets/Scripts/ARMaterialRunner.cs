using UniRx;

[PropertyClass]
public class ARMaterialRunner : Singleton<ARMaterialRunner>
{
	[PropertyBool]
	public ReactiveProperty<bool> GrassAvailable;

	[PropertyBool]
	public ReactiveProperty<bool> DirtAvailable;

	[PropertyBool]
	public ReactiveProperty<bool> WoodAvailable;

	[PropertyBool]
	public ReactiveProperty<bool> StoneAvailable;

	[PropertyBool]
	public ReactiveProperty<bool> MetalAvailable;

	public ARMaterialRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		GrassAvailable = CreateMaterialProperty(0);
		DirtAvailable = CreateMaterialProperty(1);
		WoodAvailable = CreateMaterialProperty(2);
		StoneAvailable = CreateMaterialProperty(3);
		MetalAvailable = CreateMaterialProperty(4);
	}

	private ReactiveProperty<bool> CreateMaterialProperty(int blockType)
	{
		return (from material in PlayerData.Instance.LifetimeBlocksDestroyed[blockType]
			select material > 0).TakeUntilDestroy(ARBindingManager.Instance).ToReactiveProperty();
	}
}
