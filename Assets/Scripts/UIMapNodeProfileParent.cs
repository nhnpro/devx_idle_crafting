using UnityEngine;

public class UIMapNodeProfileParent : MonoBehaviour
{
	protected void Start()
	{
		MapNodeRunner mapNodeRunner = (MapNodeRunner)Singleton<PropertyManager>.Instance.GetContext("MapNodeRunner", base.transform);
		PredictableRandom.SetSeed((uint)(mapNodeRunner.NodeIndex + 125), (uint)(PlayerData.Instance.LifetimePrestiges.Value + 125));
		int nextRangeInt = PredictableRandom.GetNextRangeInt(0, 3);
		Singleton<PropertyManager>.Instance.InstantiateSetParent(BindingManager.Instance.BiomeList.Biomes[mapNodeRunner.GetBiomeIndex()].MapNodeProfile[nextRangeInt], Vector3.zero, Quaternion.identity, null, base.transform);
	}
}
