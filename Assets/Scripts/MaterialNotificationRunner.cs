using System;
using UniRx;
using UnityEngine;

[PropertyClass]
public class MaterialNotificationRunner : Singleton<MaterialNotificationRunner>
{
	private IDisposable[] observers = new IDisposable[6];

	[PropertyBool]
	public ReactiveProperty<bool> GrassFound;

	[PropertyBool]
	public ReactiveProperty<bool> DirtFound;

	[PropertyBool]
	public ReactiveProperty<bool> WoodFound;

	[PropertyBool]
	public ReactiveProperty<bool> StoneFound;

	[PropertyBool]
	public ReactiveProperty<bool> MetalFound;

	[PropertyBool]
	public ReactiveProperty<bool> RelicFound;

	public MaterialNotificationRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		SceneLoader instance = SceneLoader.Instance;
		BindingManager bind = BindingManager.Instance;
		for (int i = 0; i < PlayerData.Instance.LifetimeBlocksDestroyed.Count; i++)
		{
			BlockType mat = (BlockType)i;
			if (mat != BlockType.Gold && PlayerData.Instance.LifetimeBlocksDestroyed[i].Value == 0)
			{
				(from amount in PlayerData.Instance.LifetimeBlocksDestroyed[i]
					where amount > 0
					select amount).Take(1).Subscribe(delegate
				{
					GameObject gameObject = GameObjectExtensions.InstantiateFromResources("UI/NewMaterialFoundProfiles/MaterialFound_" + mat);
					gameObject.transform.SetParent(bind.NewMaterialFound, worldPositionStays: false);
				}).AddTo(instance);
			}
		}
		PlayerData.Instance.LifetimePrestiges.Subscribe(delegate
		{
			CreateNearbyMaterialObservers(bind);
		}).AddTo(instance);
		(from berries in PlayerData.Instance.LifetimeBerries.Pairwise()
			where berries.Current > 0 && berries.Previous == 0
			select berries).Subscribe(delegate
		{
			bind.BerryTutorialParent.ShowInfo();
		}).AddTo(instance);
		(from keys in PlayerData.Instance.LifetimeKeys.Pairwise()
			where keys.Current > 0 && keys.Previous == 0
			select keys).Subscribe(delegate
		{
			bind.KeyTutorialParent.ShowInfo();
		}).AddTo(instance);
		GrassFound = CreateMaterialProperty(0);
		DirtFound = CreateMaterialProperty(1);
		WoodFound = CreateMaterialProperty(2);
		StoneFound = CreateMaterialProperty(3);
		MetalFound = CreateMaterialProperty(4);
		RelicFound = PlayerData.Instance.BlocksInBackpack[6].CombineLatest(PlayerData.Instance.LifetimeRelics, (long bp, long lt) => bp > 0 || lt > 0).TakeUntilDestroy(instance).ToReactiveProperty();
	}

	private ReactiveProperty<bool> CreateMaterialProperty(int blockType)
	{
		return (from material in PlayerData.Instance.LifetimeBlocksDestroyed[blockType]
			select material > 0).TakeUntilDestroy(SceneLoader.Instance).ToReactiveProperty();
	}

	public void CreateNearbyMaterialObservers(BindingManager bind)
	{
		int i;
		for (i = 0; i < 5; i++)
		{
			if (observers[i] != null)
			{
				observers[i].Dispose();
			}
			BlockType mat = (BlockType)i;
			int chunkIndex = PersistentSingleton<Economies>.Instance.ChunkGeneratings.Find((ChunkGeneratingConfig x) => x.Materials[i].Weight > 0f).Chunk;
			chunkIndex = Math.Max(1, chunkIndex);
			if (PlayerData.Instance.LifetimeBlocksDestroyed[i].Value > 0)
			{
				IDisposable disposable = (from chunk in Singleton<WorldRunner>.Instance.CurrentChunk
					where chunk.Index == chunkIndex
					select chunk).Take(1).Subscribe(delegate
				{
					GameObject gameObject = GameObjectExtensions.InstantiateFromResources("UI/NearbyMaterialsProfiles/NearbyMaterial_" + mat);
					gameObject.transform.SetParent(bind.NearbyMaterial, worldPositionStays: false);
				}).AddTo(bind);
				observers[i] = disposable;
			}
		}
	}
}
