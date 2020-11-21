using System.Collections;
using UnityEngine;

public class FakeFallForBlock : MonoBehaviour
{
	private int size;

	private bool falling;

	private void Start()
	{
		if (ARInstaller.Instance.RestrictedMode)
		{
			size = int.Parse(base.gameObject.name.Substring(base.gameObject.name.Length - 8, 1));
			CheckThenFall();
		}
	}

	public void CheckThenFall()
	{
		if (!falling)
		{
			StartCoroutine(FallRoutine());
		}
	}

	private IEnumerator FallRoutine()
	{
		falling = true;
		float halfScale = ARInstaller.Instance.Scale / 2f;
		GameObject floor = ARBindingManager.Instance.World;
		bool informed = false;
		yield return false;
		float closest = float.MaxValue;
		while (closest > 0f)
		{
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					Vector3 vector = base.transform.position + Vector3.down * halfScale * (size - 1) + floor.transform.right * halfScale * (i * 2 - (size - 1)) + floor.transform.forward * halfScale * (j * 2 - (size - 1));
					Ray ray = new Ray(vector, Vector3.down);
					if (Physics.Raycast(ray, out RaycastHit hitInfo, halfScale * 20f))
					{
						float num = Vector3.Distance(vector, hitInfo.point) - halfScale;
						if (num < closest)
						{
							closest = num;
						}
					}
				}
			}
			if (closest <= halfScale / 500f)
			{
				break;
			}
			float moveDist = Mathf.Min(halfScale / 2f, closest);
			base.transform.position = base.transform.position + Vector3.down * moveDist;
			if (!informed)
			{
				ShootFallBeams();
				informed = true;
			}
			yield return false;
		}
		falling = false;
	}

	public void ShootFallBeams()
	{
		if (!ARInstaller.Instance.RestrictedMode)
		{
			return;
		}
		float num = ARInstaller.Instance.Scale / 2f;
		GameObject world = ARBindingManager.Instance.World;
		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				Vector3 origin = base.transform.position + Vector3.up * num * (size - 1) + world.transform.right * num * (i * 2 - (size - 1)) + world.transform.forward * num * (j * 2 - (size - 1));
				Ray ray = new Ray(origin, Vector3.up);
				if (Physics.Raycast(ray, out RaycastHit hitInfo, num * 20f))
				{
					Transform transform = hitInfo.transform;
					if (transform.parent != ARBindingManager.Instance.BlockContainer)
					{
						transform = transform.parent.parent;
					}
					if (transform.parent != null && transform.parent == ARBindingManager.Instance.BlockContainer)
					{
						transform.GetComponent<FakeFallForBlock>().CheckThenFall();
					}
				}
			}
		}
	}
}
