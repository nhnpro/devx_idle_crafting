using UnityEngine;
using UnityEngine.UI;

public class UIToggleAudio : MonoBehaviour
{
	[SerializeField]
	public Toggle BottomToggle;

	[SerializeField]
	private string BottomToggleOnEvent;

	[SerializeField]
	private string BottomToggleOffEvent;

	public void SetSound()
	{
		if (BottomToggle.isOn)
		{
			AudioController.Instance.QueueEvent(new AudioEvent(BottomToggleOnEvent, AUDIOEVENTACTION.Play));
		}
		else
		{
			AudioController.Instance.QueueEvent(new AudioEvent(BottomToggleOffEvent, AUDIOEVENTACTION.Play));
		}
	}
}
