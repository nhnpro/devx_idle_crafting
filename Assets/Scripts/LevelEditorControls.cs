using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[PropertyClass]
public class LevelEditorControls : MonoBehaviour
{
	private BlockType m_prefabType;

	private int m_prefabSize = 1;

	[SerializeField]
	private LayerMask layers;

	private List<EditStep> Steps = new List<EditStep>();

	private List<JSONObject> RestrSteps = new List<JSONObject>();

	public ReactiveProperty<int> BlocksPlaced = new ReactiveProperty<int>(0);

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> MaxBlocksReached;

	[SerializeField]
	private string BlockAudioClip;

	[SerializeField]
	private string DestroyAudioClip;

	private Dictionary<int, Touch> m_touchesByFingerId = new Dictionary<int, Touch>();

	private bool m_zooming;

	private bool m_rotating;

	private float m_rotateDistanceX;

	private float m_rotateDistanceY;

	private bool m_deletingBlocks;

	public static LevelEditorControls Instance
	{
		get;
		private set;
	}

	protected void Awake()
	{
		Instance = this;
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		MaxBlocksReached = (from blocks in BlocksPlaced
			select blocks == 500).TakeUntilDestroy(this).ToReadOnlyReactiveProperty();
	}

	protected void Start()
	{
		BlocksPlaced.Subscribe(delegate(int amount)
		{
			if (amount < 30)
			{
				ARBindingManager.Instance.StateMinBlocks.SetActive(value: true);
				ARBindingManager.Instance.StateMaxBlocks.SetActive(value: false);
				ARBindingManager.Instance.BlockMinText.text = (30 - amount).ToString();
			}
			else if (amount <= 500)
			{
				ARBindingManager.Instance.StateMinBlocks.SetActive(value: false);
				ARBindingManager.Instance.StateMaxBlocks.SetActive(value: true);
				ARBindingManager.Instance.BlockMaxText.text = (500 - amount).ToString();
			}
		}).AddTo(this);
		BlocksPlaced.SetValueAndForceNotify(ARBindingManager.Instance.BlockContainer.childCount);
	}

	protected void Update()
	{
		if (m_deletingBlocks || EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}
		if (UnityEngine.Input.touchCount > 0)
		{
			for (int i = 0; i < UnityEngine.Input.touchCount; i++)
			{
				if (EventSystem.current.IsPointerOverGameObject(UnityEngine.Input.GetTouch(i).fingerId) && UnityEngine.Input.GetTouch(i).phase != TouchPhase.Ended)
				{
					return;
				}
			}
		}
		SyncTouchesAndZoom();
	}

	private void SyncTouchesAndZoom()
	{
		if (UnityEngine.Input.touchCount == 0 && (m_zooming || m_rotating))
		{
			ResetZoomAndRotating();
			return;
		}
		for (int i = 0; i < UnityEngine.Input.touchCount; i++)
		{
			Touch touch = UnityEngine.Input.GetTouch(i);
			switch (touch.phase)
			{
			case TouchPhase.Began:
				HandleTouchBegan(touch);
				break;
			case TouchPhase.Moved:
				if (m_touchesByFingerId.ContainsKey(touch.fingerId))
				{
					m_touchesByFingerId[touch.fingerId] = touch;
				}
				break;
			case TouchPhase.Ended:
			case TouchPhase.Canceled:
				if (m_touchesByFingerId.Count == 1 && !m_zooming && !m_rotating)
				{
					Tap();
				}
				m_touchesByFingerId.Remove(touch.fingerId);
				break;
			}
		}
		if (m_touchesByFingerId.Count == 1)
		{
			DoSwipeRotate(m_touchesByFingerId.Values.ToList());
		}
		if (m_touchesByFingerId.Count == 2)
		{
			DoPinchZoom(m_touchesByFingerId.Values.ToList());
		}
	}

	private void HandleTouchBegan(Touch t)
	{
		if (m_touchesByFingerId.Count < 2)
		{
			m_touchesByFingerId[t.fingerId] = t;
		}
	}

	private void DoSwipeRotate(List<Touch> touches)
	{
		if (touches.Count != 1)
		{
			return;
		}
		Touch touch = touches[0];
		Vector2 position = touch.position;
		float x = position.x;
		Vector2 vector = touch.position - touch.deltaPosition;
		float num = (x - vector.x) / 10f;
		if (!m_rotating)
		{
			m_rotateDistanceX += num;
		}
		if (m_rotateDistanceX > 10f || m_rotateDistanceX < -10f)
		{
			m_rotating = true;
			ARBindingManager.Instance.World.transform.Rotate(new Vector3(0f, -2f * num, 0f));
		}
		if (!(SceneManager.GetActiveScene().name == "LevelEditor"))
		{
			return;
		}
		Vector2 position2 = touch.position;
		float y = position2.y;
		Vector2 vector2 = touch.position - touch.deltaPosition;
		float num2 = (y - vector2.y) / 10f;
		if (!m_rotating)
		{
			m_rotateDistanceY += num2;
		}
		if (m_rotateDistanceY > 5f || m_rotateDistanceY < -5f)
		{
			m_rotating = true;
			Vector3 eulerAngles = ARBindingManager.Instance.Camera.rotation.eulerAngles;
			float x2 = eulerAngles.x;
			if (x2 - 2f * num2 > 0f && x2 - 2f * num2 < 75f)
			{
				ARBindingManager.Instance.Camera.Rotate(new Vector3(-2f * num2, 0f, 0f));
			}
		}
	}

	private void DoPinchZoom(List<Touch> touches)
	{
		if (touches.Count >= 2)
		{
			Touch touch = touches[0];
			Touch touch2 = touches[1];
			m_zooming = true;
			Vector3 localScale = ARBindingManager.Instance.World.transform.localScale;
			Vector2 a = touch.position - touch.deltaPosition;
			Vector2 b = touch2.position - touch2.deltaPosition;
			float magnitude = (a - b).magnitude;
			float magnitude2 = (touch.position - touch2.position).magnitude;
			float num = magnitude2 - magnitude;
			float num2 = Mathf.Clamp(localScale.x + localScale.x * num * 0.0035f, 0.03f, 16f);
			if (SceneManager.GetActiveScene().name == "LevelEditor")
			{
				num2 = Mathf.Clamp(localScale.x + localScale.x * num * 0.0035f, 1.4f, 5f);
			}
			localScale.x = num2;
			localScale.y = num2;
			localScale.z = num2;
			ARBindingManager.Instance.World.transform.localScale = localScale;
		}
	}

	private void ResetZoomAndRotating()
	{
		m_zooming = false;
		m_rotating = false;
		m_rotateDistanceX = 0f;
		m_rotateDistanceY = 0f;
		m_touchesByFingerId.Clear();
	}

	private void Tap()
	{
		if ((PlaceARWorld.Instance != null && !PlaceARWorld.Instance.Begin) || !Input.GetMouseButtonUp(0))
		{
			return;
		}
		Ray ray = new Ray(Vector3.zero, Vector3.up);
		ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
		if (!Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue, layers))
		{
			return;
		}
		if (UnityEngine.Input.touchCount > 0)
		{
			for (int i = 0; i < UnityEngine.Input.touchCount; i++)
			{
				if (EventSystem.current.IsPointerOverGameObject(UnityEngine.Input.GetTouch(i).fingerId) && UnityEngine.Input.GetTouch(i).phase != TouchPhase.Ended)
				{
					return;
				}
			}
		}
		if (EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}
		if (m_prefabSize == 0)
		{
			if (hitInfo.transform.parent != null && hitInfo.transform.parent == ARBindingManager.Instance.BlockContainer)
			{
				Transform transform = hitInfo.transform;
				BlocksPlaced.Value--;
				AudioController.Instance.QueueEvent(new AudioEvent(DestroyAudioClip, AUDIOEVENTACTION.Play));
				transform.GetComponent<FakeFallForBlock>().ShootFallBeams();
				if (!ARInstaller.Instance.RestrictedMode)
				{
					Steps.Add(new EditStep(transform.gameObject, activate: false));
					transform.gameObject.SetActive(value: false);
				}
				else
				{
					JSONObject item = ARInstaller.Instance.SaveARLevel();
					RestrSteps.Add(item);
					UnityEngine.Object.Destroy(transform.gameObject);
				}
			}
		}
		else
		{
			if (BlocksPlaced.Value >= 500)
			{
				return;
			}
			Vector3 vector = new Vector3(0f, 0f, 0f);
			float num = ARInstaller.Instance.Scale / 2f;
			if (hitInfo.transform.parent != null && hitInfo.transform.parent == ARBindingManager.Instance.World.transform)
			{
				Transform transform2 = ARBindingManager.Instance.World.transform;
				Vector3 b = Vector3.zero;
				if (m_prefabSize == 1)
				{
					b = (transform2.forward + transform2.right) * m_prefabSize * num;
				}
				vector = transform2.InverseTransformPoint(hitInfo.point - b);
				vector = new Vector3(Mathf.Round(vector.x * 10f) * 1f / 10f, 0f, Mathf.Round(vector.z * 10f) * 1f / 10f);
				vector = transform2.TransformPoint(vector);
				float x = vector.x;
				Vector3 position = transform2.position;
				vector = new Vector3(x, position.y + num * (float)m_prefabSize, vector.z);
				vector += (transform2.forward + transform2.right) * m_prefabSize * num;
			}
			else
			{
				vector = hitInfo.transform.position + hitInfo.normal * (m_prefabSize + 1) * num;
				if (m_prefabSize != 1)
				{
					Transform transform3 = ARBindingManager.Instance.World.transform;
					if (hitInfo.normal == transform3.forward)
					{
						vector += (-transform3.right + transform3.up) * (m_prefabSize - 1) * num;
					}
					else if (hitInfo.normal == -transform3.forward)
					{
						vector += (transform3.right + transform3.up) * (m_prefabSize - 1) * num;
					}
					else if (hitInfo.normal == transform3.right)
					{
						vector += (transform3.forward + transform3.up) * (m_prefabSize - 1) * num;
					}
					else if (hitInfo.normal == -transform3.right)
					{
						vector += (-transform3.forward + transform3.up) * (m_prefabSize - 1) * num;
					}
					else if (hitInfo.normal == transform3.up || hitInfo.normal == -transform3.up)
					{
						vector += (transform3.forward + transform3.right) * (m_prefabSize - 1) * num;
					}
				}
			}
			CheckAndSpawn(vector, num);
		}
	}

	private void CheckAndSpawn(Vector3 pos, float halfScale)
	{
		Quaternion rotation = ARBindingManager.Instance.World.transform.rotation;
		if (!Physics.CheckSphere(pos, (float)m_prefabSize * halfScale * 0.95f))
		{
			if (ARInstaller.Instance.RestrictedMode)
			{
				JSONObject item = ARInstaller.Instance.SaveARLevel();
				RestrSteps.Add(item);
			}
			string path = "Blocks/AR/" + m_prefabType.ToString() + "Cube_" + m_prefabSize + "x" + m_prefabSize;
			GameObject gameObject = GameObjectExtensions.InstantiateFromResources(path, pos, rotation);
			gameObject.transform.localScale = Vector3.one * halfScale * 20f;
			gameObject.transform.SetParent(ARBindingManager.Instance.BlockContainer);
			BlocksPlaced.Value++;
			AudioController.Instance.QueueEvent(new AudioEvent(BlockAudioClip, AUDIOEVENTACTION.Play));
			if (!ARInstaller.Instance.RestrictedMode)
			{
				Steps.Add(new EditStep(gameObject, activate: true));
			}
		}
	}

	public void ChangeBlockType(BlockType prefabType)
	{
		m_prefabType = prefabType;
	}

	public void ChangeBlockSize(int prefabSize)
	{
		m_prefabSize = prefabSize;
	}

	public void OnUndo()
	{
		if (m_deletingBlocks)
		{
			return;
		}
		if (ARInstaller.Instance.RestrictedMode)
		{
			if (RestrSteps.Count > 1)
			{
				JSONObject jSONObject = RestrSteps[RestrSteps.Count - 1];
				ARInstaller.Instance.LoadFromJSON(jSONObject);
				ARInstaller.Instance.Save(jSONObject);
				RestrSteps.Remove(jSONObject);
				BlocksPlaced.SetValueAndForceNotify(ARBindingManager.Instance.BlockContainer.childCount);
			}
		}
		else if (Steps.Count > 0)
		{
			EditStep item = Steps[Steps.Count - 1];
			if (item.Activate)
			{
				UnityEngine.Object.Destroy(item.Block);
			}
			else
			{
				item.Block.SetActive(value: true);
			}
			Steps.Remove(item);
		}
	}

	public void OnClearBlocks()
	{
		RestrSteps.Add(ARInstaller.Instance.BlocksToJSON(ARBindingManager.Instance.BlockContainer));
		StartCoroutine(DeleteBlocks());
		BlocksPlaced.SetValueAndForceNotify(0);
	}

	private IEnumerator DeleteBlocks()
	{
		m_deletingBlocks = true;
		int amount = ARBindingManager.Instance.BlockContainer.childCount - 1;
		for (int i = amount; i >= 0; i--)
		{
			UnityEngine.Object.Destroy(ARBindingManager.Instance.BlockContainer.GetChild(i).gameObject);
			yield return null;
		}
		m_deletingBlocks = false;
	}

	public void SetFirstRestrStep()
	{
		if (ARInstaller.Instance.RestrictedMode && RestrSteps.Count < 1)
		{
			RestrSteps.Add(ARInstaller.Instance.BlocksToJSON(ARBindingManager.Instance.BlockContainer));
		}
	}
}
