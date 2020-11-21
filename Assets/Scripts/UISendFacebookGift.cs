using UnityEngine;
using UnityEngine.UI;

public class UISendFacebookGift : MonoBehaviour
{
	[SerializeField]
	private Image m_friendImage;

	[SerializeField]
	private Text m_friendName;

	public string FriendId
	{
		get;
		private set;
	}

	public void Init(string id)
	{
		FriendId = id;
		string fileName = FriendId + ".png";
		Texture2D texture2D = PngTexture.LoadPNGAsTexture2D(fileName, 120, 120);
		if (texture2D != null)
		{
			Material material = new Material(m_friendImage.material);
			material.SetTexture("_MainTex", texture2D);
			m_friendImage.material = material;
		}
		if (PersistentSingleton<FacebookAPIService>.Instance.FBPlayers.ContainsKey(FriendId))
		{
			m_friendName.text = PersistentSingleton<FacebookAPIService>.Instance.FBPlayers[FriendId].Name;
		}
	}

	public void OnSendGift()
	{
		Singleton<FacebookRunner>.Instance.GiftPlayer(FriendId, PersistentSingleton<LocalizationService>.Instance.Text("SendGift.Message"), PersistentSingleton<LocalizationService>.Instance.Text("SendGift.Title"));
	}
}
