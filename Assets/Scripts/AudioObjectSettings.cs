using System;

[Serializable]
public class AudioObjectSettings
{
	public bool playOnAwake;

	public bool mute;

	public bool ignoreListenerVolume;

	public bool ignoreListenerPause;

	public bool bypassEffects;

	public bool bypassListenerEffects;

	public bool bypassReverbZones;

	public float maxTriggers = -1f;

	public float timesTriggered;

	public bool loop;

	public int numberOfLoops;

	public bool loopInfinite;

	public double initialDelayTime;

	public bool randomiseStartSeekTime;
}
