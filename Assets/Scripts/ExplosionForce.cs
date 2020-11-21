using UnityEngine;

public class ExplosionForce : MonoBehaviour
{
	public float radius = 5f;

	public float power = 10f;

	private void Start()
	{
		Vector3 position = base.transform.position;
		Collider[] array = Physics.OverlapSphere(position, radius);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			Rigidbody component = collider.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.AddExplosionForce(power, position, radius, 3f);
			}
		}
	}
}
