using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IEventSystemHandler
{
	[SerializeField]
	public UnityEvent OnHold;

	private static AnimationCurve Curve = AnimationCurve.Linear(0f, 0f, 2f, 20f);

	private bool m_held;

	private float m_timeStampStart;

	private float m_timeStampCurrent;

	private float m_toGive;

	private string m_audio = string.Empty;

	private float m_pitch = 1f;

	public Button Button
	{
		get;
		private set;
	}

	public void Awake()
	{
		Button = GetComponent<Button>();
		ToolkitClip component = GetComponent<ToolkitClip>();
		if (component != null)
		{
			m_audio = component.m_audioEventName;
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		InitCounters();
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		m_held = false;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		m_held = false;
		if (m_audio == "UpgradeBought")
		{
			m_pitch = 1f;
			BindingManager.Instance.UpgradeBoughtSound.SetPitch(m_pitch);
		}
	}

	protected void OnEnable()
	{
		m_held = false;
	}

	protected void OnDisable()
	{
		m_held = false;
		if (m_audio == "UpgradeBought" && BindingManager.Instance != null)
		{
			m_pitch = 1f;
			BindingManager.Instance.UpgradeBoughtSound.SetPitch(m_pitch);
		}
	}

	protected void Update()
	{
		if (!m_held || !Button.IsInteractable())
		{
			m_held = false;
			return;
		}
		float num = Curve.Evaluate(Time.realtimeSinceStartup - m_timeStampStart);
		m_toGive += num * (Time.realtimeSinceStartup - m_timeStampCurrent);
		while (m_toGive >= 1f)
		{
			if (Button.IsInteractable())
			{
				OnHold.Invoke();
				if (m_audio == "UpgradeBought")
				{
					m_pitch += 0.1f;
					BindingManager.Instance.UpgradeBoughtSound.SetPitch(m_pitch);
				}
			}
			m_toGive -= 1f;
		}
		m_timeStampCurrent = Time.realtimeSinceStartup;
	}

	private void InitCounters()
	{
		m_held = true;
		m_toGive = 1f;
		m_timeStampStart = Time.realtimeSinceStartup;
		m_timeStampCurrent = Time.realtimeSinceStartup;
	}
}
