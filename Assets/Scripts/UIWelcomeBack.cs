using UnityEngine;

public class UIWelcomeBack : MonoBehaviour
{
	public void OnWelcomeBack()
	{
		Singleton<WelcomeBackRunner>.Instance.PerformWelcomeBack();
		BindingManager.Instance.WelcomeBackDoneParent.ShowInfo();
	}
}
