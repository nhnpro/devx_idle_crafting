using UnityEngine;

public class UIPopupOnEnableManager : MonoBehaviour
{
	[SerializeField]
	private GameObject m_popupPrefab;

	private GameObject m_popup;

	private void OnEnable()
	{
		if (m_popup == null)
		{
			base.transform.DestroyChildrenImmediate();
			m_popup = Singleton<PropertyManager>.Instance.Instantiate(m_popupPrefab, Vector3.zero, Quaternion.identity, null);
			m_popup.transform.SetParent(base.transform, worldPositionStays: false);
		}
		else
		{
			m_popup.gameObject.SetActive(value: true);
		}
	}
}
