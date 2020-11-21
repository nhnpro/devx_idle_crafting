using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextTyper : MonoBehaviour
{
	public float letterPause = 0.2f;

	private string message;

	private Text textComp;

	private void Start()
	{
		textComp = GetComponent<Text>();
		message = textComp.text;
		textComp.text = string.Empty;
		StartCoroutine(TypeText());
	}

	private IEnumerator TypeText()
	{
		char[] array = message.ToCharArray();
		foreach (char letter in array)
		{
			textComp.text += letter;
			AudioController.Instance.QueueEvent(new AudioEvent("TypeSound", AUDIOEVENTACTION.Play));
			yield return new WaitForSeconds(letterPause);
		}
	}
}
