using System.Collections.Generic;
using UnityEngine;

public class UIBossSuccessManager : MonoBehaviour
{
	[SerializeField]
	private GameObject m_creatureTamedPrefab;

	[SerializeField]
	private GameObject m_bossRewardPrefab;

	public void ShowReward(int heroIndex)
	{
		base.transform.DestroyChildrenImmediate();
		if (heroIndex != -1)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("HeroIndex", heroIndex);
			GameObject gameObject = Singleton<PropertyManager>.Instance.Instantiate(m_creatureTamedPrefab, Vector3.zero, Quaternion.identity, dictionary);
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
		}
		else
		{
			GameObject gameObject2 = Singleton<PropertyManager>.Instance.Instantiate(m_bossRewardPrefab, Vector3.zero, Quaternion.identity, null);
			gameObject2.transform.SetParent(base.transform, worldPositionStays: false);
		}
	}
}
