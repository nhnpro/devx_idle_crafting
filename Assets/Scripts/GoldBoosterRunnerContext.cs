using System.Collections.Generic;
using UnityEngine;

public class GoldBoosterRunnerContext : PropertyContext
{
	[SerializeField]
	private int m_goldBoosterIndex;

	protected void OnDisable()
	{
		OnDeselectBooster();
	}

	public override void Install(Dictionary<string, object> parameters)
	{
		if (parameters != null && parameters.ContainsKey("GoldBoosterIndex"))
		{
			m_goldBoosterIndex = (int)parameters["GoldBoosterIndex"];
		}
		Add("GoldBoosterIndex", m_goldBoosterIndex);
		if (m_goldBoosterIndex != -1)
		{
			Add("GoldBoosterRunner", Singleton<GoldBoosterCollectionRunner>.Instance.GetOrCreateGoldBoosterRunner(m_goldBoosterIndex));
		}
	}

	public void OnBuyBoosterClicked()
	{
		Singleton<GoldBoosterCollectionRunner>.Instance.GetOrCreateGoldBoosterRunner(m_goldBoosterIndex).BuyGoldBooster();
		OnDeselectBooster();
	}

	public void OnNotEnoughGemsToBuyBoosterClicked()
	{
		Singleton<GoldBoosterCollectionRunner>.Instance.GetOrCreateGoldBoosterRunner(m_goldBoosterIndex).NotEnoughGemsForBooster();
		BindingManager.Instance.NotEnoughGemsOverlay.SetActive(value: true);
		BindingManager.Instance.NotEnoughGemsOverlay.transform.SetAsLastSibling();
	}

	public void OnSelectBooster()
	{
		Singleton<GoldBoosterCollectionRunner>.Instance.SetActiveBooster(m_goldBoosterIndex);
	}

	public void OnDeselectBooster()
	{
		Singleton<GoldBoosterCollectionRunner>.Instance.ActiveBooster.SetValueAndForceNotify(-1);
	}
}
