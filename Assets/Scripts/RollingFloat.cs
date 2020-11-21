using UnityEngine;

public class RollingFloat
{
	private const float Duration = 0.5f;

	private float m_current;

	private float m_target;

	private float m_speed;

	public float Current => m_current;

	public void SetTarget(float target)
	{
		m_target = target;
		m_speed = (m_target - m_current) * 2f;
	}

	public void SetCurrent(float current)
	{
		m_target = current;
		DoneRolling();
	}

	public bool Update()
	{
		if (m_speed == 0f)
		{
			return false;
		}
		float num = m_speed * Time.deltaTime;
		m_current += num;
		if (num > 0f)
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
		m_speed = 0f;
	}
}
