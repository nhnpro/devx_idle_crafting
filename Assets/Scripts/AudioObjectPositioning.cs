using System;
using UnityEngine;

[Serializable]
public class AudioObjectPositioning
{
	public enum VOLUMEROLLOFF
	{
		LogarithmicRolloff,
		LinearRolloff,
		Custom
	}

	public float panStereo;

	public bool randomisePanStereo;

	public float minRandomPan;

	public float maxRandomPan;

	public float minDistance = 1f;

	public float maxDistance = 500f;

	public VOLUMEROLLOFF volumeRolloff;

	public AnimationCurve customVolumeRolloff;

	public float dopplerLevel = 1f;

	public float spatialBlend = 1f;

	public bool useSpatialBlendCurve;

	public AnimationCurve customSpatialBlendCurve;

	public float spread;

	public bool useSpreadCustomCurve;

	public AnimationCurve customSpreadCurve;

	public float reverbZoneMix;

	public bool useReverbMixCustomCurve;

	public AnimationCurve customReverbZoneMixCurve;
}
