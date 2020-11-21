using UnityEngine;

public class UIPropertySetAsFirstChild : UIPropertyBase
{
	[SerializeField]
	private bool m_moveOnTrue = true;

	private bool started;

	private int initialIndex;

	private void Start()
	{
		initialIndex = base.transform.GetSiblingIndex();
		SetAsFirstChild();
		started = true;
	}

	private void OnEnable()
	{
		if (started)
		{
			SetAsFirstChild();
		}
	}

	private void SetAsFirstChild()
	{
		if (GetProperty<bool>() != null)
		{
			if (GetProperty<bool>().Value == m_moveOnTrue)
			{
				base.transform.SetSiblingIndex(0);
			}
			else
			{
				base.transform.SetSiblingIndex(initialIndex);
			}
		}
	}
}
