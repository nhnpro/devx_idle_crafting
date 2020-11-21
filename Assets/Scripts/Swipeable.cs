using UnityEngine;

public abstract class Swipeable : MonoBehaviour
{
	protected abstract void OnSwipe();

	public void SwipeEntered()
	{
		OnSwipe();
	}
}
