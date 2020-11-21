using UnityEngine;
using UnityEngine.UI;

public class UIMapWorldNumberManager : MonoBehaviour
{
	private void Start()
	{
		MapNodeRunner mapNodeRunner = (MapNodeRunner)Singleton<PropertyManager>.Instance.GetContext("MapNodeRunner", base.transform);
		int nodeIndex = mapNodeRunner.NodeIndex;
		string text = PersistentSingleton<LocalizationService>.Instance.Text("Prestige.ChunkLevel", nodeIndex * 10);
		base.gameObject.GetComponent<Text>().text = text;
	}
}
