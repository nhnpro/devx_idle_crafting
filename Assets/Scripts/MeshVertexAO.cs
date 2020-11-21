using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshVertexAO : MonoBehaviour
{
	private const float SphereSize = 0.5f;

	private Mesh m_mesh;

	private Collider[] m_buffer = new Collider[8];

	private float[] m_ao;

	private int m_raycastMask;

	private Vector3[] m_vertices;

	private Vector3[] m_normals;

	private Color[] m_colors;

	protected void Awake()
	{
		m_raycastMask = (1 << LayerMask.NameToLayer("Blocks")) + (1 << LayerMask.NameToLayer("Default"));
		m_mesh = GetComponent<MeshFilter>().mesh;
		m_mesh.MarkDynamic();
		m_ao = new float[m_mesh.vertices.Length];
	}

	protected void OnEnable()
	{
		UpdateAO(instant: true);
		Singleton<AORunner>.Instance.Register(this);
	}

	protected void OnDisable()
	{
		Singleton<AORunner>.Instance.Unregister(this);
	}

	public void UpdateAO(bool instant)
	{
		if (m_vertices == null)
		{
			m_vertices = m_mesh.vertices;
			m_normals = m_mesh.normals;
			m_colors = new Color[m_mesh.vertices.Length];
			m_mesh.colors = m_colors;
		}
		for (int i = 0; i < m_vertices.Length; i++)
		{
			Vector3 position = m_vertices[i];
			Vector3 normal = m_normals[i];
			float num = CalculateAO(position, normal);
			if (instant)
			{
				m_ao[i] = num;
			}
			else
			{
				m_ao[i] = LerpTowards(m_ao[i], num);
			}
			m_colors[i] = new Color(m_ao[i], m_ao[i], m_ao[i]);
		}
		m_mesh.colors = m_colors;
	}

	private float CalculateAO(Vector3 position, Vector3 normal)
	{
		Vector3 a = base.transform.TransformPoint(position);
		Vector3 a2 = base.transform.TransformDirection(normal);
		Vector3 position2 = a + a2 * 0.5f;
		int num = Physics.OverlapSphereNonAlloc(position2, 0.45f, m_buffer, m_raycastMask);
		for (int i = 0; i < num; i++)
		{
			if (m_buffer[i].gameObject != base.gameObject)
			{
				return 0f;
			}
		}
		return 1f;
	}

	private float LerpTowards(float val, float towards)
	{
		if (val < towards)
		{
			return Mathf.Min(towards, val + 0.25f);
		}
		return Mathf.Max(towards, val - 0.25f);
	}
}
