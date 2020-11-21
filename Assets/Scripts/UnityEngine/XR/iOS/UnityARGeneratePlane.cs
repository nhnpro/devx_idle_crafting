using System.Collections.Generic;

namespace UnityEngine.XR.iOS
{
	public class UnityARGeneratePlane : MonoBehaviour
	{
		public GameObject planePrefab;

		private UnityARAnchorManager unityARAnchorManager;

		private void Start()
		{
			unityARAnchorManager = new UnityARAnchorManager();
			UnityARUtility.InitializePlanePrefab(planePrefab);
		}

		private void OnDestroy()
		{
			unityARAnchorManager.Destroy();
		}

		private void OnGUI()
		{
			List<ARPlaneAnchorGameObject> currentPlaneAnchors = unityARAnchorManager.GetCurrentPlaneAnchors();
			if (currentPlaneAnchors.Count < 1)
			{
			}
		}
	}
}
