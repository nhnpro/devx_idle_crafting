using UnityEngine;
using UnityEngine.XR.iOS;

public class UnityARCameraManager : MonoBehaviour
{
	public Camera m_camera;

	private UnityARSessionNativeInterface m_session;

	private Material savedClearMaterial;

	[Header("AR Config Options")]
	public UnityARAlignment startAlignment;

	public UnityARPlaneDetection planeDetection = UnityARPlaneDetection.Horizontal;

	public bool getPointCloud = true;

	public bool enableLightEstimation = true;

	private void Start()
	{
		m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();
		ARKitWorldTrackingSessionConfiguration config = default(ARKitWorldTrackingSessionConfiguration);
		config.planeDetection = planeDetection;
		config.alignment = startAlignment;
		config.getPointCloudData = getPointCloud;
		config.enableLightEstimation = enableLightEstimation;
		m_session.RunWithConfig(config);
		if (m_camera == null)
		{
			m_camera = Camera.main;
		}
	}

	public void SetCamera(Camera newCamera)
	{
		if (m_camera != null)
		{
			UnityARVideo component = m_camera.gameObject.GetComponent<UnityARVideo>();
			if (component != null)
			{
				savedClearMaterial = component.m_ClearMaterial;
				UnityEngine.Object.Destroy(component);
			}
		}
		SetupNewCamera(newCamera);
	}

	private void SetupNewCamera(Camera newCamera)
	{
		m_camera = newCamera;
		if (m_camera != null)
		{
			UnityARVideo component = m_camera.gameObject.GetComponent<UnityARVideo>();
			if (component != null)
			{
				savedClearMaterial = component.m_ClearMaterial;
				UnityEngine.Object.Destroy(component);
			}
			component = m_camera.gameObject.AddComponent<UnityARVideo>();
			component.m_ClearMaterial = savedClearMaterial;
		}
	}

	private void Update()
	{
		if (m_camera != null)
		{
			Matrix4x4 cameraPose = m_session.GetCameraPose();
			m_camera.transform.localPosition = UnityARMatrixOps.GetPosition(cameraPose);
			m_camera.transform.localRotation = UnityARMatrixOps.GetRotation(cameraPose);
			m_camera.projectionMatrix = m_session.GetCameraProjection();
		}
	}
}
