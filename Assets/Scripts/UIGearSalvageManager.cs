using System.Collections.Generic;
using UnityEngine;

public class UIGearSalvageManager : MonoBehaviour
{
	[SerializeField]
	private GameObject m_gearSalvagePrefab;

	public void ShowConfirm(int gearIndex)
	{
		base.transform.DestroyChildrenImmediate();
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("GearIndex", gearIndex);
		GameObject gameObject = Singleton<PropertyManager>.Instance.Instantiate(m_gearSalvagePrefab, Vector3.zero, Quaternion.identity, dictionary);
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
	}
}
