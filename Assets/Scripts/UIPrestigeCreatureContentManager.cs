using System.Collections.Generic;
using UnityEngine;

public class UIPrestigeCreatureContentManager : MonoBehaviour
{
	[SerializeField]
	private GameObject m_creatureEvolvedPrefab;

	private bool m_evolveAccepted;

	public void Start()
	{
		base.transform.DestroyChildrenImmediate();
		Transform transform = BindingManager.Instance.PrestigeBagContent.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			HeroEvolveRunnerContext component = transform.GetChild(i).GetComponent<HeroEvolveRunnerContext>();
			if (!(component == null) && component.EvolveRunner.TierUpAvailable.Value)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("HeroIndex", component.HeroIndex);
				GameObject gameObject = Singleton<PropertyManager>.Instance.Instantiate(m_creatureEvolvedPrefab, Vector3.zero, Quaternion.identity, dictionary);
				gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			}
		}
	}
}
