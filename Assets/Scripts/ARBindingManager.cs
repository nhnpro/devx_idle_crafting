using UnityEngine;
using UnityEngine.UI;

public class ARBindingManager : MonoBehaviour
{
	[SerializeField]
	public Transform BlockContainer;

	[SerializeField]
	public Transform Camera;

	[SerializeField]
	public GameObject World;

	[SerializeField]
	public GameObject PreIntroUI;

	[SerializeField]
	public GameObject IntroUI;

	[SerializeField]
	public GameObject EditorUI;

	[SerializeField]
	public Slider ScaleSlider;

	[SerializeField]
	public Slider RotationSlider;

	[SerializeField]
	public Text BlockMinText;

	[SerializeField]
	public GameObject StateMinBlocks;

	[SerializeField]
	public Text BlockMaxText;

	[SerializeField]
	public GameObject StateMaxBlocks;

	[SerializeField]
	public GameObject WorldSelection;

	[SerializeField]
	public GameObject WorldEdit;

	[SerializeField]
	public GameObject SceneTransition;

	public static ARBindingManager Instance
	{
		get;
		private set;
	}

	public static void Construct()
	{
		Instance = GameObject.Find("BindingManager").GetComponent<ARBindingManager>();
	}

	public static void Release()
	{
		Instance = null;
	}
}
