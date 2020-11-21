using UnityEngine;

public class GamblingBindingManager : MonoBehaviour
{
	[SerializeField]
	public GameObject CardButtons;

	[SerializeField]
	public Animator ButtonOne;

	[SerializeField]
	public Animator ButtonTwo;

	[SerializeField]
	public Animator ButtonThree;

	[SerializeField]
	public Animator ButtonFour;

	[SerializeField]
	public GameObject ContinueButton;

	[SerializeField]
	public GameObject DrJellyStealPopup;

	[SerializeField]
	public GameObject NotEnoughGemsOverlay;

	[SerializeField]
	public GameObject SceneTransition;

	[SerializeField]
	public Animator CardDim;

	[SerializeField]
	public Animator Machine;

	[SerializeField]
	public GameObject LastLevelReachedPopup;

	public static GamblingBindingManager Instance
	{
		get;
		private set;
	}

	public static void Construct()
	{
		Instance = GameObject.Find("BindingManager").GetComponent<GamblingBindingManager>();
	}

	public static void Release()
	{
		Instance = null;
	}
}
