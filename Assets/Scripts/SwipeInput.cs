using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeInput : MonoBehaviour
{
	private const float SwipeDistance = 1f;

	private static Vector3 m_lastPos;

	private static bool m_debugSwipeLeft;

	private static int sm_raycastMask;

	private static float sm_lastTap;

	public static bool IsSwiping
	{
		get;
		private set;
	}

	protected void Start()
	{
		sm_raycastMask = (1 << LayerMask.NameToLayer("Blocks")) + (1 << LayerMask.NameToLayer("Swipeable"));
		sm_lastTap = Time.realtimeSinceStartup;
	}

	private void UpdateHoldTap()
	{
		if (Input.GetMouseButton(0))
		{
			float holdTapInterval = PersistentSingleton<GameSettings>.Instance.HoldTapInterval;
			while (sm_lastTap < Time.realtimeSinceStartup - holdTapInterval)
			{
				Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
				PerformTap(ray);
				sm_lastTap += holdTapInterval;
			}
		}
	}

	protected void Update()
	{
		IsSwiping = false;
		if (UnityEngine.Input.touchCount > 0)
		{
			for (int i = 0; i < UnityEngine.Input.touchCount; i++)
			{
				if (EventSystem.current.IsPointerOverGameObject(UnityEngine.Input.GetTouch(i).fingerId) && UnityEngine.Input.GetTouch(i).phase != TouchPhase.Ended)
				{
					return;
				}
			}
			if (Input.GetMouseButtonDown(0))
			{
				IsSwiping = true;
				Tap();
			}
			else if (Input.GetMouseButton(0))
			{
				IsSwiping = true;
				Swipe();
			}
		}
		UpdateHoldTap();
	}

	private void Tap()
	{
		sm_lastTap = Time.realtimeSinceStartup;
		Ray ray = new Ray(Vector3.zero, Vector3.up);
		ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
		m_lastPos = MathUtils.ProjectToFloor(ray, 0f);
		PerformTap(ray);
	}

	private void Swipe()
	{
		if (!(Camera.main == null))
		{
			Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
			Vector3 newPos = MathUtils.ProjectToFloor(ray, 0f);
			SwipeTo(newPos);
		}
	}

	public static void DebugRandomSwipe()
	{
		Vector3 position = new Vector3((!m_debugSwipeLeft) ? ((float)Screen.width) : 0f, UnityEngine.Random.Range((float)Screen.height * 0.2f, (float)Screen.height * 0.8f), 0f);
		m_debugSwipeLeft = !m_debugSwipeLeft;
		Ray ray = Camera.main.ScreenPointToRay(position);
		Vector3 newPos = MathUtils.ProjectToFloor(ray, 0f);
		SwipeTo(newPos);
	}

	private static void SwipeTo(Vector3 newPos)
	{
		while (true)
		{
			Vector3 vector = newPos - m_lastPos;
			float sqrMagnitude = vector.sqrMagnitude;
			if (sqrMagnitude < 1f)
			{
				break;
			}
			sm_lastTap = Time.realtimeSinceStartup;
			m_lastPos += vector.normalized * 1f;
			Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(m_lastPos));
			PerformTap(ray);
		}
	}

	private static void PerformTap(Ray ray)
	{
		if (Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue, sm_raycastMask))
		{
			Swipeable component = hitInfo.transform.gameObject.GetComponent<Swipeable>();
			if (component != null)
			{
				component.SwipeEntered();
			}
		}
	}
}
