using System;
using UnityEngine;

[Serializable]
public class AudioObject
{
	public enum AUDIOSTATE
	{
		Stopped,
		Paused,
		Playing,
		FadeIn,
		FadeOut,
		Waiting
	}

	[HideInInspector]
	public string name;

	[HideInInspector]
	public GameObject gameObject;

	public GameObject parentGameObject;

	public AudioClip audioClip;

	public AudioSource audioSource;

	public float targetVolume = 1f;

	public float setVolume = 1f;

	public float realVolume = 1f;

	public float curveFadeModifier = 1f;

	public float targetPitch = 1f;

	public float setPitch = 1f;

	public float realPitch = 1f;

	public float currentTimeFading;

	public double initTime;

	public bool windowFocusPause;

	public float listenerDistance;

	public AUDIOSTATE audioState = AUDIOSTATE.Waiting;
}
