using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFriendNodeManager : MonoBehaviour
{
	protected void OnEnable()
	{
		StartCoroutine(Enable());
	}

	private IEnumerator Enable()
	{
		yield return false;
		MapNodeRunner mapnode = (MapNodeRunner)Singleton<PropertyManager>.Instance.GetContext("MapNodeRunner", base.transform);
		int node = mapnode.NodeIndex;
		List<FBPlayer> playersInTheNode = new List<FBPlayer>();
		foreach (KeyValuePair<string, FBPlayer> fBPlayer in PersistentSingleton<FacebookAPIService>.Instance.FBPlayers)
		{
			if (fBPlayer.Value.Highscore > 0 && fBPlayer.Value.Highscore >= node * 10 && fBPlayer.Value.Highscore < (node + 1) * 10)
			{
				playersInTheNode.Add(fBPlayer.Value);
			}
		}
		base.transform.DestroyChildrenImmediate();
		for (int i = 0; i < 3 && i < playersInTheNode.Count; i++)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("StringValue", playersInTheNode[i].Id + ".png");
			if (playersInTheNode[i].Id == PlayerData.Instance.FBId.Value)
			{
				GameObject gameObject = Singleton<PropertyManager>.Instance.InstantiateFromResources("Map/PlayerBubble", Vector3.zero, Quaternion.identity, dictionary);
				gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			}
			else
			{
				GameObject gameObject2 = Singleton<PropertyManager>.Instance.InstantiateFromResources("Map/FriendBubble", Vector3.zero, Quaternion.identity, dictionary);
				gameObject2.transform.SetParent(base.transform, worldPositionStays: false);
			}
		}
		if (playersInTheNode.Count > 3)
		{
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2.Add("StringValue", "+" + (playersInTheNode.Count - 3));
			GameObject gameObject3 = Singleton<PropertyManager>.Instance.InstantiateFromResources("Map/FriendsNumber", Vector3.zero, Quaternion.identity, dictionary2);
			gameObject3.transform.SetParent(base.transform, worldPositionStays: false);
		}
	}
}
