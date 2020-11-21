using UnityEngine;
using UnityEngine.XR.iOS;

public class PointCloudParticleExample : MonoBehaviour
{
	public ParticleSystem pointCloudParticlePrefab;

	public int maxPointsToShow;

	public float particleSize = 1f;

	private Vector3[] m_PointCloudData;

	private bool frameUpdated;

	private ParticleSystem currentPS;

	private ParticleSystem.Particle[] particles;

	private void Start()
	{
		UnityARSessionNativeInterface.ARFrameUpdatedEvent += ARFrameUpdated;
		currentPS = UnityEngine.Object.Instantiate(pointCloudParticlePrefab);
		frameUpdated = false;
	}

	public void ARFrameUpdated(UnityARCamera camera)
	{
		m_PointCloudData = camera.pointCloudData;
		frameUpdated = true;
	}

	private void Update()
	{
		if (!frameUpdated)
		{
			return;
		}
		if (m_PointCloudData != null && m_PointCloudData.Length > 0)
		{
			int num = Mathf.Min(m_PointCloudData.Length, maxPointsToShow);
			ParticleSystem.Particle[] array = new ParticleSystem.Particle[num];
			int num2 = 0;
			Vector3[] pointCloudData = m_PointCloudData;
			foreach (Vector3 position in pointCloudData)
			{
				array[num2].position = position;
				array[num2].startColor = new Color(1f, 1f, 1f);
				array[num2].startSize = particleSize;
				num2++;
			}
			currentPS.SetParticles(array, num);
		}
		else
		{
			ParticleSystem.Particle[] array2 = new ParticleSystem.Particle[1];
			array2[0].startSize = 0f;
			currentPS.SetParticles(array2, 1);
		}
		frameUpdated = false;
	}
}
