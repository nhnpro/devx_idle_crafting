using Big;
using System.Collections;
using UnityEngine;

public class UISlerpControllerBigDouble : UIPropertyBase
{
	[SerializeField]
	private SlerpTargetEnum m_slerpTarget;

	[SerializeField]
	private int m_min;

	[SerializeField]
	private int m_max = 10;

	[SerializeField]
	private bool m_onEnable;

	[SerializeField]
	private float m_delay;

	protected void OnEnable()
	{
		if (m_onEnable)
		{
			OnSlerp();
		}
	}

	public void OnSlerp()
	{
		StartCoroutine(Slerp());
	}

	private IEnumerator Slerp()
	{
		yield return new WaitForSeconds(m_delay);
		SlerpTarget target = BindingManager.Instance.GetSlerpTarget(m_slerpTarget);
		int num = Mathf.Max(b: BigDouble.Min(GetProperty<BigDouble>().Value, m_max).ToInt(), a: m_min);
		for (int i = 0; i < num; i++)
		{
			target.SlerpFromHud(base.transform.position);
		}
	}
}
