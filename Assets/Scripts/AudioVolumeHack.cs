using UnityEngine;

public class AudioVolumeHack : MonoBehaviour
{
	[SerializeField]
	private AudioGroupComponent m_music;

	[SerializeField]
	private AudioGroupComponent m_sfx;

	private void Start()
	{
		m_music.SetVolume(0f);
		m_sfx.SetVolume(0f);
	}
}
