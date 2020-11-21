using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class UnityPointCloudExample : MonoBehaviour
{
	public uint numPointsToShow = 100u;

	public GameObject PointCloudPrefab;

	private List<GameObject> pointCloudObjects;

	private Vector3[] m_PointCloudData;

	public void Start()
	{
		UnityARSessionNativeInterface.ARFrameUpdatedEvent += ARFrameUpdated;
		if (PointCloudPrefab != null)
		{
			pointCloudObjects = new List<GameObject>();
			for (int i = 0; i < numPointsToShow; i++)
			{
				pointCloudObjects.Add(UnityEngine.Object.Instantiate(PointCloudPrefab));
			}
		}
	}

	public void ARFrameUpdated(UnityARCamera camera)
	{
		m_PointCloudData = camera.pointCloudData;
	}

	public void Update()
	{
		if (PointCloudPrefab != null && m_PointCloudData != null)
		{
			for (int i = 0; i < Math.Min(m_PointCloudData.Length, numPointsToShow); i++)
			{
				Vector4 vector = m_PointCloudData[i];
				GameObject gameObject = pointCloudObjects[i];
				gameObject.transform.position = new Vector3(vector.x, vector.y, vector.z);
			}
		}
	}
}
