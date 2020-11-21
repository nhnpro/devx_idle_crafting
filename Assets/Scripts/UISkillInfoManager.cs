using System.Collections.Generic;
using UnityEngine;

public class UISkillInfoManager : MonoBehaviour
{
	[SerializeField]
	private GameObject m_skillInfoPrefab;

	private SkillsEnum m_prevSkill = SkillsEnum.NumSkills;

	private GameObject m_prevInfo;

	public void ShowInfo(SkillsEnum skill)
	{
		if (m_prevSkill != skill)
		{
			base.transform.DestroyChildrenImmediate();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("SkillsEnum", skill);
			m_prevInfo = Singleton<PropertyManager>.Instance.Instantiate(m_skillInfoPrefab, Vector3.zero, Quaternion.identity, dictionary);
			m_prevInfo.transform.SetParent(base.transform, worldPositionStays: false);
		}
		else
		{
			m_prevInfo.gameObject.SetActive(value: true);
		}
		m_prevSkill = skill;
	}
}
