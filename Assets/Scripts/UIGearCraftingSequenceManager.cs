using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class UIGearCraftingSequenceManager : MonoBehaviour
{
	[SerializeField]
	private GameObject m_gearCraftingSequencePrefab;

	protected void Start()
	{
		Singleton<UpgradeRunner>.Instance.GearUnlockTriggered.Subscribe(delegate(int gearIndex)
		{
			SetupPanel(gearIndex);
		}).AddTo(this);
	}

	public void SetupPanel(int gearIndex)
	{
		GearRunner orCreateGearRunner = Singleton<GearCollectionRunner>.Instance.GetOrCreateGearRunner(gearIndex);
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("GearIndex", gearIndex);
		dictionary.Add("GearSetIndex", orCreateGearRunner.SetIndex);
		base.transform.DestroyChildrenImmediate();
		GameObject gameObject = Singleton<PropertyManager>.Instance.Instantiate(m_gearCraftingSequencePrefab, Vector3.zero, Quaternion.identity, dictionary);
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
	}
}
