using UniRx;
using UnityEngine;

[PropertyClass]
public class ARRunner : Singleton<ARRunner>
{
	[PropertyBool]
	public ReactiveProperty<bool> LevelEditorUnlocked;

	[PropertyBool]
	public ReactiveProperty<bool> ARSupported = new ReactiveProperty<bool>(initialValue: false);

	[PropertyBool]
	public ReactiveProperty<bool> HasCameraPermission = new ReactiveProperty<bool>(initialValue: false);

	public ARRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		LevelEditorUnlocked = (from ltchunk in PlayerData.Instance.LifetimeChunk
			select ltchunk >= 75).ToReactiveProperty().AddTo(SceneLoader.Instance);
		(from unlocked in LevelEditorUnlocked.Pairwise()
			where !unlocked.Previous && unlocked.Current
			select unlocked).Subscribe(delegate
		{
			BindingManager.Instance.LevelEditorUnlockedParent.ShowInfo();
		}).AddTo(SceneLoader.Instance);
		GameObject gameObject = (!(SceneLoader.Instance != null)) ? ARBindingManager.Instance.gameObject : BindingManager.Instance.gameObject;
		(from p in Observable.EveryApplicationPause().StartWith(value: false)
			where !p
			select p).DelayFrame(1).Subscribe(delegate
		{
			HasCameraPermission.Value = ARService.HasCameraPermission();
		}).AddTo(gameObject);
		if (PersistentSingleton<GameAnalytics>.Instance != null)
		{
			PersistentSingleton<GameAnalytics>.Instance.ARSupported = ARSupported;
		}
	}
}
