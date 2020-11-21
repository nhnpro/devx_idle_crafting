using Big;
using UnityEngine;

public class RollingBigDouble
{
	private const float Duration = 0.5f;

	private BigDouble m_current = BigDouble.ZERO;

	private BigDouble m_target = BigDouble.ZERO;

	private BigDouble m_speed = BigDouble.ZERO;

	public BigDouble Current => m_current;

	public void SetTarget(BigDouble target)
	{
		m_target = target;
		m_speed = (m_target - m_current) * 2.0;
	}

	public void SetCurrent(BigDouble current)
	{
		m_target = current;
		DoneRolling();
	}

	public bool Update()
	{
		if (m_speed == BigDouble.ZERO)
		{
			return false;
		}
		BigDouble bigDouble = m_speed * Time.deltaTime;
		m_current += bigDouble;
		if (bigDouble > BigDouble.ZERO)
		{
			if (m_current > m_target)
			{
				DoneRolling();
			}
		}
		else if (m_current < m_target)
		{
			DoneRolling();
		}
		return true;
	}

	public void DoneRolling()
	{
		m_current = m_target;
		m_speed = BigDouble.ZERO;
	}
}
