using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class UIGearSetUnlockManager : MonoBehaviour
{
	[SerializeField]
	private GameObject m_gearUnlockPrefab;

	protected void OnEnable()
	{
		Singleton<GearSetCollectionRunner>.Instance.SetUnlocked.Subscribe(delegate(List<int> unlocks)
		{
			ShowUnlockedGears(unlocks);
		}).AddTo(this);
	}

	public void ShowUnlockedGears(List<int> gearIndexes)
	{
		base.transform.DestroyChildrenImmediate();
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (int gearIndex in gearIndexes)
		{
			dictionary.Clear();
			dictionary.Add("GearIndex", gearIndex);
			GameObject gameObject = Singleton<PropertyManager>.Instance.Instantiate(m_gearUnlockPrefab, Vector3.zero, Quaternion.identity, dictionary);
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
		}
	}
}
