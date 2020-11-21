using UniRx;
using UnityEngine;

public class UICreaturePanelManager : MonoBehaviour
{
	[SerializeField]
	private GameObject m_creatureBattlePrefab;

	protected void Start()
	{
		Singleton<BossIndexRunner>.Instance.CurrentBossIndex.Subscribe(delegate(int heroIndex)
		{
			SetupPanel(heroIndex);
		}).AddTo(this);
	}

	public void SetupPanel(int heroIndex)
	{
		base.transform.DestroyChildrenImmediate();
		GameObject gameObject = Singleton<PropertyManager>.Instance.Instantiate(m_creatureBattlePrefab, Vector3.zero, Quaternion.identity, null);
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
	}
}
