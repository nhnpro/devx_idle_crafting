using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WeightedObjecExt
{
	public static A AllotObject<A>(this IEnumerable<WeightedObject<A>> weights)
	{
		float max = (from w in weights
			select w.Weight).Aggregate(0f, (float a, float b) => a + b);
		float position = Random.Range(0f, max);
		return weights.First(delegate(WeightedObject<A> w)
		{
			position -= w.Weight;
			return position <= 0f;
		}).Value;
	}

	public static A PredictableAllotObject<A>(this List<WeightedObject<A>> weights)
	{
		float num = 0f;
		for (int i = 0; i < weights.Count; i++)
		{
			num += weights[i].Weight;
		}
		float num2 = PredictableRandom.GetNextRangeFloat(0f, num);
		for (int j = 0; j < weights.Count; j++)
		{
			WeightedObject<A> weightedObject = weights[j];
			num2 -= weightedObject.Weight;
			if (num2 <= 0f)
			{
				return weightedObject.Value;
			}
		}
		return weights[weights.Count - 1].Value;
	}

	public static List<A> AllotQueue<A>(this IEnumerable<WeightedObject<A>> weights)
	{
		return AllotQueuePriv(new List<A>(), from w in weights
			where w.Weight > 0f
			select w).ToList();
	}

	private static IEnumerable<A> AllotQueuePriv<A>(List<A> alloted, IEnumerable<WeightedObject<A>> left)
	{
		if (left.Count() == 0)
		{
			return alloted;
		}
		A head = (A)left.AllotObject();
		alloted.Add((A)head);
		return AllotQueuePriv(alloted, from w in left
			where !w.Value.Equals((A)head)
			select w);
	}
}
