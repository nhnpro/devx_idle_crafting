using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class UIOpenGiftContentManager : MonoBehaviour
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
				SceneLoader.Instance.StopCoroutine(m_populateRoutine);
			}
			m_populateRoutine = SceneLoader.Instance.StartCoroutine(PopulateGiftContents());
		}).AddTo(this);
		Singleton<FacebookRunner>.Instance.GiftConsumeSuccess.Subscribe(delegate(FacebookGift gift)
		{
			RemoveGift(gift);
		}).AddTo(this);
	}

	private IEnumerator PopulateGiftContents()
	{
		base.transform.DestroyChildrenImmediate();
		Dictionary<string, object> pars = new Dictionary<string, object>();
		List<FacebookGift> gifts = Singleton<FacebookRunner>.Instance.UniqueGifts();
		foreach (FacebookGift gift in gifts)
		{
			pars.Clear();
			GameObject uiGift = UnityEngine.Object.Instantiate(m_giftPrefab, Vector3.zero, Quaternion.identity);
			uiGift.GetComponent<UIOpenFacebookGift>().Init(gift);
			uiGift.transform.SetParent(base.transform, worldPositionStays: false);
			yield return null;
		}
	}

	public void RemoveGift(FacebookGift gift)
	{
		for (int num = base.transform.childCount - 1; num >= 0; num--)
		{
			Transform child = base.transform.GetChild(num);
			if (child.gameObject.GetComponent<UIOpenFacebookGift>().Gift.GiftId == gift.GiftId)
			{
				UnityEngine.Object.DestroyImmediate(child.gameObject);
			}
		}
	}

	public void OpenAllGifts()
	{
		Singleton<FacebookRunner>.Instance.OpenAllGifts();
	}
}
