using UnityEngine;

public class UILevelSkip : MonoBehaviour
{
	public void OnBuySkip()
	{
		Singleton<LevelSkipRunner>.Instance.BuyLevelSkipWithGems();
	}

	public void SelectAdSkip()
	{
		Singleton<LevelSkipRunner>.Instance.SelectedSkip = LevelSkipEnum.Free;
		BindingManager.Instance.GoatConfirmParent.ShowInfo();
	}

	public void SelectSmallSkip()
	{
		Singleton<LevelSkipRunner>.Instance.SelectedSkip = LevelSkipEnum.Small;
		BindingManager.Instance.GoatConfirmParent.ShowInfo();
	}

	public void SelectMediumSkip()
	{
		Singleton<LevelSkipRunner>.Instance.SelectedSkip = LevelSkipEnum.Medium;
		BindingManager.Instance.GoatConfirmParent.ShowInfo();
	}

	public void SelectLargeSkip()
	{
		Singleton<LevelSkipRunner>.Instance.SelectedSkip = LevelSkipEnum.Large;
		BindingManager.Instance.GoatConfirmParent.ShowInfo();
	}

	public void OnSkipMore()
	{
		BindingManager.Instance.GoatEntryParent.ShowInfo();
	}
}
