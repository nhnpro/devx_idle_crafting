public class IAPItemCollectionRunner : Singleton<IAPItemCollectionRunner>
{
	private IAPItemRunner[] m_iapItemRunners = new IAPItemRunner[6];

	public IAPItemCollectionRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
	}

	public IAPItemRunner GetOrCreateIAPItemRunner(IAPProductEnum iapItem)
	{
		if (m_iapItemRunners[(int)iapItem] == null)
		{
			m_iapItemRunners[(int)iapItem] = new IAPItemRunner(iapItem);
		}
		return m_iapItemRunners[(int)iapItem];
	}
}
