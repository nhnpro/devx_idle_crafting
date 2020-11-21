using Big;
using UnityEngine;

public interface IBlock
{
	void CauseDamage(BigDouble dmg);

	Vector3 Position();

	bool IsActive();

	int ApproximateSize();

	float TimeSinceLastSwipe();
}
