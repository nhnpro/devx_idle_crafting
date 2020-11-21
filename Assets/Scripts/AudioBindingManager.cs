using UnityEngine;

public class AudioBindingManager : MonoBehaviour
{
	[SerializeField]
	public AudioGroupComponent MasterVolume;

	[SerializeField]
	public AudioGroupComponent MusicVolume;

	[SerializeField]
	public AudioGroupComponent SFXVolume;

	[SerializeField]
	public AudioGroupComponent UIVolume;

	[SerializeField]
	public AudioGroupComponent AmbientVolume;

	[SerializeField]
	public AudioGroupComponent EnvironmentVolume;

	[SerializeField]
	public AudioGroupComponent CharacterSoundsVolume;

	[SerializeField]
	public AudioComponent UpgradeBoughtSound;

	public static AudioBindingManager Instance
	{
		get;
		private set;
	}

	public static void Construct()
	{
		Instance = GameObject.Find("Audio").GetComponent<AudioBindingManager>();
	}

	public static void Release()
	{
		Instance = null;
	}
}
