using UnityEngine;

public class UIChangeLanguage : MonoBehaviour
{
	public void OnChangeLanguage()
	{
		PersistentSingleton<LocalizationService>.Instance.ChangeLanguage();
	}
}
