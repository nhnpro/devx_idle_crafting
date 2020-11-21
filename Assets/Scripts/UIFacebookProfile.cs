using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIFacebookProfile : MonoBehaviour
{
	private void Start()
	{
		Image image = GetComponent<Image>();
		Singleton<FacebookRunner>.Instance.PlayerID.Subscribe(delegate(string id)
		{
			Texture2D value = PngTexture.LoadPNGAsTexture2D(id + ".png", 120, 120);
			Material material = new Material(image.material);
			material.SetTexture("_MainTex", value);
			image.material = material;
		}).AddTo(this);
	}
}
