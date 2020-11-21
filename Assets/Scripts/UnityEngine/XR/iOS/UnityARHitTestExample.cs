using System.Collections.Generic;

namespace UnityEngine.XR.iOS
{
	public class UnityARHitTestExample : MonoBehaviour
	{
		public Transform m_HitTransform;

		private bool HitTestWithResultType(ARPoint point, ARHitTestResultType resultTypes)
		{
			List<ARHitTestResult> list = UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(point, resultTypes);
			if (list.Count > 0)
			{
				using (List<ARHitTestResult>.Enumerator enumerator = list.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						ARHitTestResult current = enumerator.Current;
						UnityEngine.Debug.Log("Got hit!");
						m_HitTransform.position = UnityARMatrixOps.GetPosition(current.worldTransform);
						m_HitTransform.rotation = UnityARMatrixOps.GetRotation(current.worldTransform);
						Vector3 position = m_HitTransform.position;
						object arg = position.x;
						Vector3 position2 = m_HitTransform.position;
						object arg2 = position2.y;
						Vector3 position3 = m_HitTransform.position;
						UnityEngine.Debug.Log($"x:{arg:0.######} y:{arg2:0.######} z:{position3.z:0.######}");
						return true;
					}
				}
			}
			return false;
		}

		private void Update()
		{
			if (UnityEngine.Input.touchCount <= 0 || !(m_HitTransform != null))
			{
				return;
			}
			Touch touch = UnityEngine.Input.GetTouch(0);
			if (touch.phase != 0 && touch.phase != TouchPhase.Moved)
			{
				return;
			}
			Vector3 vector = Camera.main.ScreenToViewportPoint(touch.position);
			ARPoint aRPoint = default(ARPoint);
			aRPoint.x = vector.x;
			aRPoint.y = vector.y;
			ARPoint point = aRPoint;
			ARHitTestResultType[] array = new ARHitTestResultType[3]
			{
				ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent,
				ARHitTestResultType.ARHitTestResultTypeHorizontalPlane,
				ARHitTestResultType.ARHitTestResultTypeFeaturePoint
			};
			ARHitTestResultType[] array2 = array;
			foreach (ARHitTestResultType resultTypes in array2)
			{
				if (HitTestWithResultType(point, resultTypes))
				{
					break;
				}
			}
		}
	}
}
