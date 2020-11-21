using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class UINamingManager : MonoBehaviour
{
	[SerializeField]
	private InputField m_nameInputField;

	[SerializeField]
	private GameObject m_checkingNicknameOverlay;

	[SerializeField]
	private GameObject m_enabledButton;

	private string m_latestNameAttempt;

	private bool m_latestNameApproved;

	public static string FilterDisplayName(string name)
	{
		Regex regex = new Regex("[^\\w ]");
		return regex.Replace(name, string.Empty);
	}

	public void FilterNameField()
	{
		m_nameInputField.text = FilterDisplayName(m_nameInputField.text);
	}

	public void CheckNameField()
	{
		m_nameInputField.text = FilterDisplayName(m_nameInputField.text);
		SetAccountName();
	}

	private void SetAccountName()
	{
		m_enabledButton.SetActive(value: false);
		if (string.IsNullOrEmpty(m_nameInputField.text))
		{
			return;
		}
		string text = m_nameInputField.text.Trim();
		if (m_latestNameAttempt == text)
		{
			if (m_latestNameApproved)
			{
				m_enabledButton.SetActive(value: true);
			}
		}
		else
		{
			m_latestNameAttempt = text;
			m_checkingNicknameOverlay.SetActive(value: true);
			PersistentSingleton<PlayFabService>.Instance.UpdateUserDisplayName(text, delegate
			{
				m_checkingNicknameOverlay.SetActive(value: false);
				m_enabledButton.SetActive(value: true);
				m_latestNameApproved = true;
			}, delegate(string str)
			{
				if (str.Substring(0, 15) == "400 Bad Request")
				{
					BindingManager.Instance.IngameNotifications.InstantiateGenericNotification(PersistentSingleton<LocalizationService>.Instance.Text("UI.GenericNotification.AccountNameUnacceptable"));
				}
				m_nameInputField.text = PlayerData.Instance.DisplayName.Value;
				m_checkingNicknameOverlay.SetActive(value: false);
				m_latestNameApproved = false;
			});
		}
	}

	private IEnumerator EmulateCheckingDelay()
	{
		yield return new WaitForSeconds(1f);
		m_checkingNicknameOverlay.SetActive(value: false);
		m_enabledButton.SetActive(value: true);
		m_latestNameApproved = true;
	}
}
