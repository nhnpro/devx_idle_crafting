namespace UnityEngine.XR.iOS
{
	public class EditorHitTest : MonoBehaviour
	{
		public Transform m_HitTransform;

		public float maxRayDistance = 30f;

		public LayerMask collisionLayerMask;
	}
}
