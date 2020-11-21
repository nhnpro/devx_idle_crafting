using UnityEngine;
using UnityEngine.UI;

public class UIAudioSettings : MonoBehaviour
{
	[SerializeField]
	private Slider musicSlider;

	[SerializeField]
	private Slider sfxSlider;

	private void Start()
	{
		musicSlider.value = Singleton<SettingsRunner>.Instance.MusicVolume.Value;
		musicSlider.onValueChanged.AddListener(delegate
		{
			MusicSliderValueChanged();
		});
		sfxSlider.value = Singleton<SettingsRunner>.Instance.SFXVolume.Value;
		sfxSlider.onValueChanged.AddListener(delegate
		{
			SFXSliderValueChanged();
		});
	}

	private void MusicSliderValueChanged()
	{
		Singleton<SettingsRunner>.Instance.MusicVolume.Value = musicSlider.value;
	}

	private void SFXSliderValueChanged()
	{
		Singleton<SettingsRunner>.Instance.SFXVolume.Value = sfxSlider.value;
	}
}
