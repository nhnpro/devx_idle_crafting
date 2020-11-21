using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIPropertyTexture : UIPropertyBase
{
	protected void Start()
	{
		Image image = GetComponent<Image>();
		IReadOnlyReactiveProperty<string> property = GetProperty<string>();
		(from path in property.TakeUntilDestroy(this)
			select PngTexture.LoadPNGAsTexture2D(path, 120, 120)).Subscribe(delegate(Texture2D texture)
		{
			if (texture != null)
			{
				Material material = new Material(image.material);
				material.SetTexture("_MainTex", texture);
				image.material = material;
			}
		}).AddTo(this);
	}
}
