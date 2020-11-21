namespace UnityEngine.XR.iOS
{
	public class UnityARUtility
	{
		private MeshCollider meshCollider;

		private MeshFilter meshFilter;

		private static GameObject planePrefab;

		public static void InitializePlanePrefab(GameObject go)
		{
			planePrefab = go;
		}

		public static GameObject CreatePlaneInScene(ARPlaneAnchor arPlaneAnchor)
		{
			GameObject gameObject = (!(planePrefab != null)) ? new GameObject() : Object.Instantiate(planePrefab);
			gameObject.name = arPlaneAnchor.identifier;
			return UpdatePlaneWithAnchorTransform(gameObject, arPlaneAnchor);
		}

		public static GameObject UpdatePlaneWithAnchorTransform(GameObject plane, ARPlaneAnchor arPlaneAnchor)
		{
			plane.transform.position = UnityARMatrixOps.GetPosition(arPlaneAnchor.transform);
			plane.transform.rotation = UnityARMatrixOps.GetRotation(arPlaneAnchor.transform);
			MeshFilter componentInChildren = plane.GetComponentInChildren<MeshFilter>();
			if (componentInChildren != null)
			{
				componentInChildren.gameObject.transform.localScale = new Vector3(arPlaneAnchor.extent.x * 0.1f, arPlaneAnchor.extent.y * 0.1f, arPlaneAnchor.extent.z * 0.1f);
				componentInChildren.gameObject.transform.localPosition = new Vector3(arPlaneAnchor.center.x, arPlaneAnchor.center.y, 0f - arPlaneAnchor.center.z);
			}
			return plane;
		}
	}
}
