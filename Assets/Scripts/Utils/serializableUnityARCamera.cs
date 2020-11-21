using System;
using UnityEngine.XR.iOS;

namespace Utils
{
	[Serializable]
	public class serializableUnityARCamera
	{
		public serializableUnityARMatrix4x4 worldTransform;

		public serializableUnityARMatrix4x4 projectionMatrix;

		public ARTrackingState trackingState;

		public ARTrackingStateReason trackingReason;

		public UnityVideoParams videoParams;

		public serializableUnityARLightData lightData;

		public serializablePointCloud pointCloud;

		public serializableUnityARMatrix4x4 displayTransform;

		public serializableUnityARCamera(serializableUnityARMatrix4x4 wt, serializableUnityARMatrix4x4 pm, ARTrackingState ats, ARTrackingStateReason atsr, UnityVideoParams uvp, UnityARLightData lightDat, serializableUnityARMatrix4x4 dt, serializablePointCloud spc)
		{
			worldTransform = wt;
			projectionMatrix = pm;
			trackingState = ats;
			trackingReason = atsr;
			videoParams = uvp;
			lightData = lightDat;
			displayTransform = dt;
			pointCloud = spc;
		}

		public static implicit operator serializableUnityARCamera(UnityARCamera rValue)
		{
			return new serializableUnityARCamera(rValue.worldTransform, rValue.projectionMatrix, rValue.trackingState, rValue.trackingReason, rValue.videoParams, rValue.lightData, rValue.displayTransform, rValue.pointCloudData);
		}

		public static implicit operator UnityARCamera(serializableUnityARCamera rValue)
		{
			return new UnityARCamera(rValue.worldTransform, rValue.projectionMatrix, rValue.trackingState, rValue.trackingReason, rValue.videoParams, rValue.lightData, rValue.displayTransform, rValue.pointCloud);
		}
	}
}
