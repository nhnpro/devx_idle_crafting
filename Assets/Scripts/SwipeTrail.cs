using UnityEngine;

public class SwipeTrail : MonoBehaviour
{
	protected void Update()
	{
		Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
		base.transform.position = MathUtils.ProjectToFloor(ray, 10f);
	}
}
