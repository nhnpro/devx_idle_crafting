using Big;
using System.Collections.Generic;
using System.Linq;

public class PiecewiseLinearFunction
{
	public Dictionary<BigDouble, int> DataPoints;

	public int Evaluate(BigDouble x)
	{
		KeyValuePair<BigDouble, int> keyValuePair = DataPoints.Last((KeyValuePair<BigDouble, int> dp) => dp.Key <= x);
		KeyValuePair<BigDouble, int> keyValuePair2 = DataPoints.FirstOr((KeyValuePair<BigDouble, int> dp) => dp.Key > x, () => new KeyValuePair<BigDouble, int>(DataPoints.Last().Key * 2L, DataPoints.Last().Value));
		float num = ((x - keyValuePair.Key) / (keyValuePair2.Key - keyValuePair.Key)).ToFloat();
		float num2 = keyValuePair2.Value - keyValuePair.Value;
		return keyValuePair.Value + (int)(num * num2);
	}
}
