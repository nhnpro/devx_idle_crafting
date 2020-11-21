using UnityEngine;

public class PlaceARWorld : MonoBehaviour
{
	public float newScale
	{
		get;
		set;
	}

	public float newRotation
	{
		get;
		set;
	}

	public bool Begin
	{
		get;
		private set;
	}

	public static PlaceARWorld Instance
	{
		get;
		private set;
	}

	protected void Awake()
	{
		Instance = this;
		Begin = false;
	}

	protected void Start()
	{
		ARBindingManager.Instance.World.SetActive(value: false);
		ARBindingManager.Instance.WorldEdit.SetActive(value: false);
		ARBindingManager.Instance.WorldSelection.SetActive(value: true);
		ARBindingManager.Instance.EditorUI.SetActive(value: false);
		ARBindingManager.Instance.IntroUI.SetActive(value: false);
		ARBindingManager.Instance.PreIntroUI.SetActive(value: true);
		newScale = 5f;
		newRotation = 0f;
	}

	public void Update()
	{
		if (!Begin && Time.realtimeSinceStartup > 3f)
		{
			MoveFloor(Vector3.zero);
		}
	}

	public static float DistanceToLine(Ray ray, Vector3 point)
	{
		return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
	}

	public void MoveFloor(Vector3 pos)
	{
		ARBindingManager.Instance.World.SetActive(value: true);
		ARBindingManager.Instance.IntroUI.SetActive(value: true);
		ARBindingManager.Instance.PreIntroUI.SetActive(value: false);
		ARBindingManager.Instance.World.transform.position = pos;
	}

	public void OnBegin()
	{
		Begin = true;
		ARBindingManager.Instance.IntroUI.SetActive(value: false);
		ARBindingManager.Instance.EditorUI.SetActive(value: true);
		ARBindingManager.Instance.WorldEdit.SetActive(value: true);
		ARBindingManager.Instance.WorldSelection.SetActive(value: false);
	}

	public void OnScale()
	{
		float value = ARBindingManager.Instance.ScaleSlider.value;
		value = ((!(newScale < 5f)) ? (value * (newScale - 4f)) : (value * (newScale / 5f)));
		ARBindingManager.Instance.World.transform.localScale = new Vector3(value, value, value);
	}

	public void OnRotate()
	{
		ARBindingManager.Instance.World.transform.rotation = Quaternion.identity;
		ARBindingManager.Instance.World.transform.Rotate(new Vector3(0f, (ARBindingManager.Instance.RotationSlider.value + newRotation) * 360f, 0f));
	}

	public void OnReadjustWorldPlacement()
	{
		Begin = false;
		ARBindingManager.Instance.IntroUI.SetActive(value: true);
		ARBindingManager.Instance.EditorUI.SetActive(value: false);
	}
}
