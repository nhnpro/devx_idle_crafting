using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
	[SerializeField]
	private Text target;

	public void FloatText(string text)
	{
		target.text = text;
		base.gameObject.SetActive(value: true);
		StartCoroutine(HideText(2f));
	}

	private IEnumerator HideText(float time)
	{
		yield return new WaitForSeconds(time);
		base.gameObject.SetActive(value: false);
	}
}
