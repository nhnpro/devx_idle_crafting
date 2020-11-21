using UniRx;
using UnityEngine;

public class UIPropertyCompareSetActive : UIAlwaysStartPropertyBase
{
	[SerializeField]
	private UIPropertyCompareEnum m_compareOp;

	[SerializeField]
	private int m_cmpValue;

	public override void AlwaysStart()
	{
		IReadOnlyReactiveProperty<int> property = GetProperty<int>();
		(from val in property.TakeWhile((int _) => this != null)
			select ShouldBeActive(val)).SubscribeToActiveUntilNull(base.gameObject).AddTo(this);
	}

	public void SetCmpValueAndUpdate(int val)
	{
		m_cmpValue = val;
		base.gameObject.SetActive(ShouldBeActive(GetProperty<int>().Value));
	}

	private bool ShouldBeActive(int val)
	{
		switch (m_compareOp)
		{
		case UIPropertyCompareEnum.Equal:
			return val == m_cmpValue;
		case UIPropertyCompareEnum.NotEqual:
			return val != m_cmpValue;
		case UIPropertyCompareEnum.LessThan:
			return val < m_cmpValue;
		case UIPropertyCompareEnum.LessThanOrEqual:
			return val <= m_cmpValue;
		case UIPropertyCompareEnum.GreaterThan:
			return val > m_cmpValue;
		case UIPropertyCompareEnum.GreaterThanOrEqual:
			return val >= m_cmpValue;
		default:
			return false;
		}
	}
}
