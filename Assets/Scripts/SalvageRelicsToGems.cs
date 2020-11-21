using Big;
using System.Collections.Generic;

public static class SalvageRelicsToGems
{
	public static PiecewiseLinearFunction m_linearFunc = new PiecewiseLinearFunction
	{
		DataPoints = new Dictionary<BigDouble, int>
		{
			{
				0L,
				50
			},
			{
				60L,
				100
			},
			{
				360L,
				200
			},
			{
				86400L,
				400
			},
			{
				604800L,
				600
			},
			{
				2628000L,
				800
			},
			{
				31540000L,
				1000
			}
		}
	};

	public static int Evaluate(BigDouble relics)
	{
		return m_linearFunc.Evaluate(relics);
	}
}
