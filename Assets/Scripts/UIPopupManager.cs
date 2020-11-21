using UnityEngine;

public class UIPopupManager : MonoBehaviour
{
	[SerializeField]
	private GameObject m_popupPrefab;

	private GameObject m_popup;

	public void ShowInfo()
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

	public void HideInfo()
	{
		if (m_popup != null)
		{
			m_popup.gameObject.SetActive(value: false);
		}
	}
}
