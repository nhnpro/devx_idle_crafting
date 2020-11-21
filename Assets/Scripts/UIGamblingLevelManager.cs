using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class UIGamblingLevelManager : MonoBehaviour
{
	[SerializeField]
	private GameObject m_levelIndicatorPrefab;

	private List<GameObject> m_indicators = new List<GameObject>();

	private void Start()
	{
		base.transform.DestroyChildrenImmediate();
		for (int i = 0; i < 4; i++)
		{
			AddLevelIndicator(Singleton<GamblingRunner>.Instance.CurrentGamblingLevel.Value + i);
		}
		Singleton<GamblingRunner>.Instance.CurrentGamblingLevel.Subscribe(delegate(int lvl)
		{
			AddLevelIndicator(lvl + 4);
		}).AddTo(this);
	}

	private void AddLevelIndicator(int lvl)
	{
		if (m_indicators.Count > 10)
		{
			UnityEngine.Object.DestroyImmediate(m_indicators[0]);
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		bool flag = lvl == Singleton<GamblingRunner>.Instance.NextJackpotLevel.Value;
		string value = " ";
		if (lvl <= Singleton<GamblingRunner>.Instance.MaxGamblingLevel.Value)
		{
			value = lvl.ToString();
		}
		dictionary.Add("BoolValue", flag);
		dictionary.Add("StringValue", value);
		GameObject gameObject = Singleton<PropertyManager>.Instance.Instantiate(m_levelIndicatorPrefab, Vector3.zero, Quaternion.identity, dictionary);
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
		m_indicators.Add(gameObject);
	}
}
