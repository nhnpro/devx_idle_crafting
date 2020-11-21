using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIOpenFacebookGift : MonoBehaviour
{
	[SerializeField]
	private Image m_friendImage;

	[SerializeField]
	private Text m_friendName;

	[SerializeField]
	private GameObject m_button;

	public FacebookGift Gift
	{
		get;
		private set;
	}

	public void Init(FacebookGift gift)
	{
		Gift = gift;
		string fileName = Gift.FromId + ".png";
		Texture2D texture2D = PngTexture.LoadPNGAsTexture2D(fileName, 120, 120);
		if (texture2D != null)
		{
			Material material = new Material(m_friendImage.material);
			material.SetTexture("_MainTex", texture2D);
			m_friendImage.material = material;
		}
		if (PersistentSingleton<FacebookAPIService>.Instance.FBPlayers.ContainsKey(Gift.FromId))
		{
			m_friendName.text = PersistentSingleton<FacebookAPIService>.Instance.FBPlayers[Gift.FromId].Name;
		}
		(from g in Singleton<FacebookRunner>.Instance.GiftConsuming
			where g.FromId == Gift.FromId
			select g).Subscribe(delegate
		{
			m_button.SetActive(value: false);
		}).AddTo(this);
		(from g in Singleton<FacebookRunner>.Instance.GiftConsumeFailed
			where g.FromId == Gift.FromId
			select g).Subscribe(delegate
		{
			m_button.SetActive(value: true);
		}).AddTo(this);
	}

	public void OnOpenGift()
	{
		Singleton<FacebookRunner>.Instance.OpenOneGift(Gift);
	}
}
