using UniRx;
using UnityEngine;

[PropertyClass]
public class SettingsRunner : Singleton<SettingsRunner>
{
	public ReactiveProperty<float> MasterVolume = new ReactiveProperty<float>();

	public ReactiveProperty<float> MusicVolume = new ReactiveProperty<float>();

	public ReactiveProperty<float> SFXVolume = new ReactiveProperty<float>();

	public ReactiveProperty<float> SFXUIVolume = new ReactiveProperty<float>();

	public ReactiveProperty<float> SFXAmbVolume = new ReactiveProperty<float>();

	public ReactiveProperty<float> SFXEnvVolume = new ReactiveProperty<float>();

	[PropertyString]
	public ReadOnlyReactiveProperty<string> MasterVolumeString;

	[PropertyString]
	public ReadOnlyReactiveProperty<string> MusicVolumeString;

	[PropertyString]
	public ReadOnlyReactiveProperty<string> SFXVolumeString;

	[PropertyString]
	public ReadOnlyReactiveProperty<string> SFXUIVolumeString;

	[PropertyString]
	public ReadOnlyReactiveProperty<string> SFXAmbVolumeString;

	[PropertyString]
	public ReadOnlyReactiveProperty<string> SFXEnvVolumeString;

	[PropertyBool]
	public ReactiveProperty<bool> Android = new ReactiveProperty<bool>(initialValue: false);

	[PropertyString]
	public ReactiveProperty<string> Language = new ReactiveProperty<string>();

	[PropertyString]
	public ReactiveProperty<string> PlayerID = new ReactiveProperty<string>();

	[PropertyString]
	public ReactiveProperty<string> Version = new ReactiveProperty<string>();

	private ReactiveProperty<bool> PFLogged = new ReactiveProperty<bool>(initialValue: false);

	public SettingsRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		SceneLoader instance = SceneLoader.Instance;
		BindingManager bind = BindingManager.Instance;
		InitializeVolumes(bind);
		Language.Value = PersistentSingleton<LocalizationService>.Instance.Text("UI.Language." + PlayerData.Instance.Language);
		PlayerID.Value = PlayerData.Instance.PlayerId;
		Version.Value = Application.version;
		Android.Value = true;
		PersistentSingleton<PlayFabService>.Instance.LoggedOnPlayerId.Subscribe(delegate
		{
			PFLogged.Value = true;
		}).AddTo(instance);
		PlayerData.Instance.PFId.CombineLatest(PFLogged, (string id, bool pf) => new
		{
			id,
			pf
		}).Subscribe(tuple =>
		{
			if (tuple.id == string.Empty)
			{
				PlayerID.Value = "ID: " + PlayerData.Instance.PlayerId;
			}
			else if (tuple.pf)
			{
				PlayerID.Value = "ID: " + tuple.id;
			}
			else
			{
				PlayerID.Value = "ID: " + tuple.id + " (Disconnected)";
			}
		}).AddTo(instance);
		MasterVolume.DistinctUntilChanged().Subscribe(delegate(float vol)
		{
			bind.MasterVolume.SetVolume(vol);
			PlayerPrefs.SetFloat("MasterVolume", vol);
		}).AddTo(instance);
		MusicVolume.DistinctUntilChanged().Subscribe(delegate(float vol)
		{
			bind.MusicVolume.SetVolume(vol);
			PlayerPrefs.SetFloat("MusicVolume", vol);
		}).AddTo(instance);
		SFXVolume.DistinctUntilChanged().Subscribe(delegate(float vol)
		{
			bind.SFXVolume.SetVolume(vol);
			PlayerPrefs.SetFloat("SFXVolume", vol);
		}).AddTo(instance);
		SFXUIVolume.DistinctUntilChanged().Subscribe(delegate(float vol)
		{
			bind.UIVolume.SetVolume(vol);
			PlayerPrefs.SetFloat("SFXUIVolume", vol);
		}).AddTo(instance);
		SFXAmbVolume.DistinctUntilChanged().Subscribe(delegate(float vol)
		{
			bind.AmbientVolume.SetVolume(vol);
			PlayerPrefs.SetFloat("SFXAmbVolume", vol);
		}).AddTo(instance);
		SFXEnvVolume.DistinctUntilChanged().Subscribe(delegate(float vol)
		{
			bind.EnvironmentVolume.SetVolume(vol);
			PlayerPrefs.SetFloat("SFXEnvVolume", vol);
		}).AddTo(instance);
		MasterVolumeString = (from vol in MasterVolume
			select Mathf.RoundToInt(vol * 100f).ToString() + "%").TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		MusicVolumeString = (from vol in MusicVolume
			select Mathf.RoundToInt(vol * 100f).ToString() + "%").TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		SFXVolumeString = (from vol in SFXVolume
			select Mathf.RoundToInt(vol * 100f).ToString() + "%").TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		SFXUIVolumeString = (from vol in SFXUIVolume
			select Mathf.RoundToInt(vol * 100f).ToString() + "%").TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		SFXAmbVolumeString = (from vol in SFXAmbVolume
			select Mathf.RoundToInt(vol * 100f).ToString() + "%").TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		SFXEnvVolumeString = (from vol in SFXEnvVolume
			select Mathf.RoundToInt(vol * 100f).ToString() + "%").TakeUntilDestroy(instance).ToReadOnlyReactiveProperty();
		if (PersistentSingleton<GameAnalytics>.Instance != null)
		{
			PersistentSingleton<GameAnalytics>.Instance.MusicVolume = MusicVolume;
			PersistentSingleton<GameAnalytics>.Instance.SFXVolume = SFXVolume;
		}
	}

	private void InitializeVolumes(BindingManager bind)
	{
		MasterVolume.Value = PlayerPrefs.GetFloat("MasterVolume", 1f);
		bind.MasterVolume.SetVolume(MasterVolume.Value);
		MusicVolume.Value = PlayerPrefs.GetFloat("MusicVolume", 1f);
		bind.MusicVolume.SetVolume(MusicVolume.Value);
		SFXVolume.Value = PlayerPrefs.GetFloat("SFXVolume", 1f);
		bind.SFXVolume.SetVolume(SFXVolume.Value);
		SFXUIVolume.Value = PlayerPrefs.GetFloat("SFXUIVolume", 1f);
		bind.UIVolume.SetVolume(SFXUIVolume.Value);
		SFXAmbVolume.Value = PlayerPrefs.GetFloat("SFXAmbVolume", 1f);
		bind.AmbientVolume.SetVolume(SFXAmbVolume.Value);
		SFXEnvVolume.Value = PlayerPrefs.GetFloat("SFXEnvVolume", 1f);
		bind.EnvironmentVolume.SetVolume(SFXEnvVolume.Value);
	}
}
