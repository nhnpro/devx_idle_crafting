using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class UIMiniMapManager : MonoBehaviour
{
	protected void Start()
	{
		PopulateMinimap();
		(from node in (from chunk in PlayerData.Instance.MainChunk
				select Mathf.CeilToInt((float)(chunk + 1) / 10f)).Pairwise()
			where node.Previous != node.Current
			select node).Subscribe(delegate
		{
			PopulateMinimap();
		}).AddTo(this);
	}

	private void PopulateMinimap()
	{
		base.transform.DestroyChildrenImmediate();
		int num = Mathf.CeilToInt((float)(PlayerData.Instance.MainChunk.Value + 1) / 40f) - 1;
		int num2 = Mathf.FloorToInt((float)PlayerData.Instance.MainChunk.Value / 10f) % 4;
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("Chapter", num);
		dictionary.Add("NodeOffset", num2);
		Singleton<PropertyManager>.Instance.InstantiateFromResourcesSetParent("Map/MapProgressListObject_Current", Vector3.zero, Quaternion.identity, dictionary, base.transform);
		for (int i = 1; i < 9; i++)
		{
			dictionary.Clear();
			int num3 = (num2 + i) % 4;
			if (num3 == 0)
			{
				num++;
			}
			dictionary.Add("Chapter", num);
			dictionary.Add("NodeOffset", num3);
			Singleton<PropertyManager>.Instance.InstantiateFromResourcesSetParent("Map/MapProgressListObject_Upcoming", Vector3.zero, Quaternion.identity, dictionary, base.transform);
		}
	}
}
