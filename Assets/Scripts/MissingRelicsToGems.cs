using Big;
using System.Collections.Generic;

public static class MissingRelicsToGems
{
	public static PiecewiseLinearFunction m_linearFunc = new PiecewiseLinearFunction
	{
		DataPoints = new Dictionary<BigDouble, int>
		{
			{
				0L,
				25
			},
			{
				5L,
				50
			},
			{
				10L,
				70
			},
			{
				15L,
				80
			},
			{
				25L,
				100
			},
			{
				50L,
				100
			},
			{
				360L,
				500
			},
			{
				86400L,
				600
			},
			{
				604800L,
				700
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
