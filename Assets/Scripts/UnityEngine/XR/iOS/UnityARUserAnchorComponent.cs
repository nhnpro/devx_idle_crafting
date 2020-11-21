namespace UnityEngine.XR.iOS
{
	public class UnityARUserAnchorComponent : MonoBehaviour
	{
		private string m_AnchorId;

		public string AnchorId => m_AnchorId;

		private void Awake()
		{
			UnityARSessionNativeInterface.ARUserAnchorUpdatedEvent += GameObjectAnchorUpdated;
			UnityARSessionNativeInterface.ARUserAnchorRemovedEvent += AnchorRemoved;
			m_AnchorId = UnityARSessionNativeInterface.GetARSessionNativeInterface().AddUserAnchorFromGameObject(base.gameObject).identifierStr;
		}

		private void Start()
		{
		}

		public void AnchorRemoved(ARUserAnchor anchor)
		{
			if (anchor.identifier.Equals(m_AnchorId))
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private void OnDestroy()
		{
			UnityARSessionNativeInterface.ARUserAnchorUpdatedEvent -= GameObjectAnchorUpdated;
			UnityARSessionNativeInterface.ARUserAnchorRemovedEvent -= AnchorRemoved;
			UnityARSessionNativeInterface.GetARSessionNativeInterface().RemoveUserAnchor(m_AnchorId);
		}

		private void GameObjectAnchorUpdated(ARUserAnchor anchor)
		{
		}
	}
}
