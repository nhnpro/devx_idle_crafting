using UnityEngine;

public class UICloseMap : MonoBehaviour
{
	public void OnCloseMap()
	{
		BindingManager.Instance.MapCamera.ExitIntro();
		BindingManager.Instance.MapPanel.SetActive(value: false);
	}

	public void OnCloseIntro()
	{
		OnCloseMap();
		Singleton<TutorialGoalCollectionRunner>.Instance.NextTutorialGoal();
		Singleton<ChunkRunner>.Instance.StartAdventure();
	}
}
