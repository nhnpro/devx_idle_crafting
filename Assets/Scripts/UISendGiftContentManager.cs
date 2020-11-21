using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class UISendGiftContentManager : MonoBehaviour
{
	[SerializeField]
	private GameObject m_giftPrefab;

	private Coroutine m_populateRoutine;

	protected void Start()
	{
		Singleton<FacebookRunner>.Instance.GiftsFetched.StartWith(value: true).Subscribe(delegate
		{
			if (m_populateRoutine != null)
			{
				BindingManager.Instance.StopCoroutine(m_populateRoutine);
			}
			m_populateRoutine = BindingManager.Instance.StartCoroutine(PopulateGiftRoutine());
		}).AddTo(this);
		Singleton<FacebookRunner>.Instance.GiftSendingSuccess.Subscribe(delegate(string id)
		{
			RemoveGift(id);
		}).AddTo(this);
	}

	private IEnumerator PopulateGiftRoutine()
	{
		base.transform.DestroyChildrenImmediate();
		Dictionary<string, object> pars = new Dictionary<string, object>();
		List<KeyValuePair<string, FBPlayer>> list = PersistentSingleton<FacebookAPIService>.Instance.FBPlayers.ToList();
		foreach (KeyValuePair<string, FBPlayer> keyValue in list)
		{
			FBPlayer fbPlayer = keyValue.Value;
			if (FacebookRunner.CanGiftPlayer(fbPlayer))
			{
				pars.Clear();
				GameObject uiGift = UnityEngine.Object.Instantiate(m_giftPrefab, Vector3.zero, Quaternion.identity);
				uiGift.GetComponent<UISendFacebookGift>().Init(keyValue.Value.Id);
				uiGift.transform.SetParent(base.transform, worldPositionStays: false);
				yield return null;
			}
		}
	}

	public void RemoveGift(string friendId)
	{
		for (int num = base.transform.childCount - 1; num >= 0; num--)
		{
			Transform child = base.transform.GetChild(num);
			if (child.gameObject.GetComponent<UISendFacebookGift>().FriendId == friendId)
			{
				UnityEngine.Object.DestroyImmediate(child.gameObject);
			}
		}
	}

	public void SendAllGifts()
	{
		Singleton<FacebookRunner>.Instance.GiftAllPlayers(PersistentSingleton<LocalizationService>.Instance.Text("SendGift.Message"), PersistentSingleton<LocalizationService>.Instance.Text("SendGift.Title"));
	}
}
