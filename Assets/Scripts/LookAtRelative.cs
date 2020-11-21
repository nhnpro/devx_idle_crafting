using System;
using UnityEngine;

[Obsolete("Use class TreeSwinger")]
public class LookAtRelative : MonoBehaviour
{
	[SerializeField]
	private SpringConstraint m_spring;

	private Quaternion m_rot;

	protected void Start()
	{
		m_rot = base.transform.rotation;
	}

	protected void Update()
	{
		Vector3 v = m_spring.Origin - base.transform.position;
		Vector3 v2 = m_spring.Pos - base.transform.position;
		AxisAngle axisAngle = MathExt.AngleBetween(v, v2);
		base.transform.rotation = Quaternion.AngleAxis(57.29578f * axisAngle.Angle, axisAngle.Axis) * m_rot;
	}
}
