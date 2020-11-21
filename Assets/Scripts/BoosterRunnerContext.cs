using System.Collections.Generic;
using UnityEngine;

public class BoosterRunnerContext : PropertyContext
{
	[SerializeField]
	private int m_boosterIndex;

	protected void OnDisable()
	{
		OnDeselectBooster();
	}

	public override void Install(Dictionary<string, object> parameters)
	{
		if (parameters != null && parameters.ContainsKey("BoosterIndex"))
		{
			m_boosterIndex = (int)parameters["BoosterIndex"];
		}
		Add("BoosterIndex", m_boosterIndex);
		if (m_boosterIndex != -1)
		{
			Add("BoosterRunner", Singleton<BoosterCollectionRunner>.Instance.GetOrCreateBoosterRunner(m_boosterIndex));
		}
	}

	public void OnBuyBoosterClicked()
	{
		Singleton<BoosterCollectionRunner>.Instance.GetOrCreateBoosterRunner(m_boosterIndex).BuyBooster();
		OnDeselectBooster();
	}

	public void OnNotEnoughGemsToBuyBoosterClicked()
	{
		Singleton<BoosterCollectionRunner>.Instance.GetOrCreateBoosterRunner(m_boosterIndex).NotEnoughGemsForBooster();
		BindingManager.Instance.NotEnoughGemsOverlay.SetActive(value: true);
		BindingManager.Instance.NotEnoughGemsOverlay.transform.SetAsLastSibling();
	}

	public void OnOpenShopPanel()
	{
		BindingManager.Instance.ShopButton.isOn = true;
	}

	public void OnSelectBooster()
	{
		Singleton<BoosterCollectionRunner>.Instance.SetActiveBooster(m_boosterIndex);
	}

	public void OnDeselectBooster()
	{
		Singleton<BoosterCollectionRunner>.Instance.ActiveBooster.SetValueAndForceNotify(-1);
	}
}
