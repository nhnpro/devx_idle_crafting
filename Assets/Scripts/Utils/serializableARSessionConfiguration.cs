using System;
using UnityEngine.XR.iOS;

namespace Utils
{
	[Serializable]
	public class serializableARSessionConfiguration
	{
		public UnityARAlignment alignment;

		public UnityARPlaneDetection planeDetection;

		public bool getPointCloudData;

		public bool enableLightEstimation;

		public serializableARSessionConfiguration(UnityARAlignment align, UnityARPlaneDetection planeDet, bool getPtCloud, bool enableLightEst)
		{
			alignment = align;
			planeDetection = planeDet;
			getPointCloudData = getPtCloud;
			enableLightEstimation = enableLightEst;
		}

		public static implicit operator serializableARSessionConfiguration(ARKitWorldTrackingSessionConfiguration awtsc)
		{
			return new serializableARSessionConfiguration(awtsc.alignment, awtsc.planeDetection, awtsc.getPointCloudData, awtsc.enableLightEstimation);
		}

		public static implicit operator ARKitWorldTrackingSessionConfiguration(serializableARSessionConfiguration sasc)
		{
			return new ARKitWorldTrackingSessionConfiguration(sasc.alignment, sasc.planeDetection, sasc.getPointCloudData, sasc.enableLightEstimation);
		}
	}
}
