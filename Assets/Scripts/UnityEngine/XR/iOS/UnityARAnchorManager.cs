using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.XR.iOS
{
	public class UnityARAnchorManager
	{
		private Dictionary<string, ARPlaneAnchorGameObject> planeAnchorMap;

		public UnityARAnchorManager()
		{
			planeAnchorMap = new Dictionary<string, ARPlaneAnchorGameObject>();
			UnityARSessionNativeInterface.ARAnchorAddedEvent += AddAnchor;
			UnityARSessionNativeInterface.ARAnchorUpdatedEvent += UpdateAnchor;
			UnityARSessionNativeInterface.ARAnchorRemovedEvent += RemoveAnchor;
		}

		public void AddAnchor(ARPlaneAnchor arPlaneAnchor)
		{
			GameObject gameObject = UnityARUtility.CreatePlaneInScene(arPlaneAnchor);
			gameObject.AddComponent<DontDestroyOnLoad>();
			ARPlaneAnchorGameObject aRPlaneAnchorGameObject = new ARPlaneAnchorGameObject();
			aRPlaneAnchorGameObject.planeAnchor = arPlaneAnchor;
			aRPlaneAnchorGameObject.gameObject = gameObject;
			planeAnchorMap.Add(arPlaneAnchor.identifier, aRPlaneAnchorGameObject);
		}

		public void RemoveAnchor(ARPlaneAnchor arPlaneAnchor)
		{
			if (planeAnchorMap.ContainsKey(arPlaneAnchor.identifier))
			{
				ARPlaneAnchorGameObject aRPlaneAnchorGameObject = planeAnchorMap[arPlaneAnchor.identifier];
				UnityEngine.Object.Destroy(aRPlaneAnchorGameObject.gameObject);
				planeAnchorMap.Remove(arPlaneAnchor.identifier);
			}
		}

		public void UpdateAnchor(ARPlaneAnchor arPlaneAnchor)
		{
			if (planeAnchorMap.ContainsKey(arPlaneAnchor.identifier))
			{
				ARPlaneAnchorGameObject aRPlaneAnchorGameObject = planeAnchorMap[arPlaneAnchor.identifier];
				UnityARUtility.UpdatePlaneWithAnchorTransform(aRPlaneAnchorGameObject.gameObject, arPlaneAnchor);
				aRPlaneAnchorGameObject.planeAnchor = arPlaneAnchor;
				planeAnchorMap[arPlaneAnchor.identifier] = aRPlaneAnchorGameObject;
			}
		}

		public void Destroy()
		{
			foreach (ARPlaneAnchorGameObject currentPlaneAnchor in GetCurrentPlaneAnchors())
			{
				UnityEngine.Object.Destroy(currentPlaneAnchor.gameObject);
			}
			planeAnchorMap.Clear();
		}

		public List<ARPlaneAnchorGameObject> GetCurrentPlaneAnchors()
		{
			return planeAnchorMap.Values.ToList();
		}
	}
}
