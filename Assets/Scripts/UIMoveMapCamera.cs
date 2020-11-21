using System.Collections;
using UnityEngine;

public class UIMoveMapCamera : MonoBehaviour
{
	private float m_mouseY = -1f;

	private float m_slidFor = 10f;

	[SerializeField]
	private float m_forceMultiplier = 0.01f;

	[SerializeField]
	private float m_slideMultiplier = 15f;

	[SerializeField]
	private float m_slideTime = 1f;

	[SerializeField]
	private AnimationCurve m_slidingCurve;

	[SerializeField]
	private Transform m_mapContent;

	[SerializeField]
	private float m_topBoundaryOffset = -30f;

	[SerializeField]
	private float m_bottomBoundaryOffset = -5.8f;

	[SerializeField]
	private float m_topPeakingOffset = 10f;

	[SerializeField]
	private float m_bottomPeakingOffset = 4f;

	[SerializeField]
	private float m_centerCameraOffset = -15f;

	[SerializeField]
	private AnimationCurve m_introCurve;

	[SerializeField]
	private float m_introTime = 3f;

	[SerializeField]
	private float m_introStartZ = 45f;

	[SerializeField]
	private float m_introEndZ = -45f;

	private float m_topBoundary = float.PositiveInfinity;

	private float m_bottomBoundary = float.PositiveInfinity;

	private bool intro;

	private void OnEnable()
	{
		m_topBoundary = float.PositiveInfinity;
		m_bottomBoundary = float.PositiveInfinity;
	}

	public void CenterCamera(Vector3 pos)
	{
		if (!intro)
		{
			Transform transform = base.transform;
			Vector3 position = base.transform.position;
			float x = position.x;
			Vector3 position2 = base.transform.position;
			transform.position = new Vector3(x, position2.y, pos.z - m_centerCameraOffset);
		}
	}

	public void IntroCameraDrive()
	{
		intro = true;
		StartCoroutine(Intro());
		StartCoroutine(PlaySwooshSound());
	}

	public void ExitIntro()
	{
		intro = false;
	}

	private void Update()
	{
		if (intro)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			m_slidFor = 10f;
			Vector3 mousePosition = UnityEngine.Input.mousePosition;
			m_mouseY = mousePosition.y;
			if (m_topBoundary == float.PositiveInfinity)
			{
				Vector3 position = m_mapContent.GetChild(0).position;
				m_topBoundary = position.z;
				Vector3 position2 = m_mapContent.GetChild(m_mapContent.childCount - 1).position;
				m_bottomBoundary = position2.z;
			}
		}
		if (m_topBoundary != float.PositiveInfinity)
		{
			if (Input.GetMouseButtonUp(0))
			{
				m_slidFor = 0f;
				StartCoroutine(Slide());
			}
			if (Input.GetMouseButton(0))
			{
				Vector3 position3 = base.transform.position;
				float z = position3.z;
				float mouseY = m_mouseY;
				Vector3 mousePosition2 = UnityEngine.Input.mousePosition;
				float z2 = Mathf.Clamp(z + (mouseY - mousePosition2.y) * m_forceMultiplier, m_bottomBoundary + m_bottomBoundaryOffset - m_bottomPeakingOffset, m_topBoundary + m_topBoundaryOffset + m_topPeakingOffset);
				Transform transform = base.transform;
				Vector3 position4 = base.transform.position;
				float x = position4.x;
				Vector3 position5 = base.transform.position;
				transform.position = new Vector3(x, position5.y, z2);
				Vector3 mousePosition3 = UnityEngine.Input.mousePosition;
				m_mouseY = mousePosition3.y;
			}
		}
	}

	private IEnumerator Slide()
	{
		float mouseY = m_mouseY;
		Vector3 mousePosition = UnityEngine.Input.mousePosition;
		float initialSpeed = (mouseY - mousePosition.y) * m_forceMultiplier;
		Vector3 origin = base.transform.position;
		Vector3 position = base.transform.position;
		float targetZ = Mathf.Clamp(position.z + initialSpeed * m_slideMultiplier, m_bottomBoundary + m_bottomBoundaryOffset, m_topBoundary + m_topBoundaryOffset);
		Vector3 position2 = base.transform.position;
		float x = position2.x;
		Vector3 position3 = base.transform.position;
		Vector3 target = new Vector3(x, position3.y, targetZ);
		while (m_slidFor <= m_slideTime)
		{
			m_slidFor += Time.deltaTime;
			float percent = Mathf.Clamp01(m_slidFor / m_slideTime);
			float curvePercent = m_slidingCurve.Evaluate(percent);
			base.transform.position = Vector3.Lerp(origin, target, curvePercent);
			yield return false;
		}
	}

	private IEnumerator Intro()
	{
		float introedFor = 0f;
		Vector3 position = base.transform.position;
		float x = position.x;
		Vector3 position2 = base.transform.position;
		Vector3 origin = new Vector3(x, position2.y, m_introStartZ);
		Vector3 position3 = base.transform.position;
		float x2 = position3.x;
		Vector3 position4 = base.transform.position;
		Vector3 target = new Vector3(x2, position4.y, m_introEndZ);
		base.transform.position = origin;
		while (introedFor <= m_introTime)
		{
			introedFor += Time.deltaTime;
			float percent = Mathf.Clamp01(introedFor / m_introTime);
			float curvePercent = m_introCurve.Evaluate(percent);
			base.transform.position = Vector3.Lerp(origin, target, curvePercent);
			yield return false;
		}
	}

	private IEnumerator PlaySwooshSound()
	{
		yield return new WaitForSeconds(2.5f);
		yield return new WaitForSeconds(4.5f);
		AudioController.Instance.QueueEvent(new AudioEvent("ChunkMove", AUDIOEVENTACTION.Play));
		yield return new WaitForSeconds(1.5f);
		AudioController.Instance.QueueEvent(new AudioEvent("BossblockHit", AUDIOEVENTACTION.Play));
		AudioController.Instance.QueueEvent(new AudioEvent("BossblockHit", AUDIOEVENTACTION.Play));
		yield return new WaitForSeconds(3.1f);
		AudioController.Instance.QueueEvent(new AudioEvent("CraftingItemDone", AUDIOEVENTACTION.Play));
	}
}
