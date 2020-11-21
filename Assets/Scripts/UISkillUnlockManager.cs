using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class UISkillUnlockManager : MonoBehaviour
{
	[SerializeField]
	private GameObject m_skillUnlockPrefab;

	protected void Start()
	{
		Singleton<SkillCollectionRunner>.Instance.SkillUnlocked.Subscribe(delegate(SkillsEnum skill)
		{
			ShowUnlock(skill);
		}).AddTo(this);
	}

	public void ShowUnlock(SkillsEnum skill)
	{
		base.transform.DestroyChildrenImmediate();
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("SkillsEnum", skill);
		GameObject gameObject = Singleton<PropertyManager>.Instance.Instantiate(m_skillUnlockPrefab, Vector3.zero, Quaternion.identity, dictionary);
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
	}
}
