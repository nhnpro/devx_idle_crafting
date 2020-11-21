using UnityEngine;

namespace Unity.Performance
{
	public static class ValueBinUtils
	{
		public static void AddValue(this ValueBin[] bins, float value)
		{
			int num = 0;
			while (true)
			{
				if (num < bins.Length)
				{
					if (!(bins[num].v < value))
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			bins[num].f++;
		}

		public static float EstimatedPercentile(this ValueBin[] bins, float percentile)
		{
			int num = 0;
			for (int i = 0; i < bins.Length; i++)
			{
				ValueBin valueBin = bins[i];
				num += valueBin.f;
			}
			return bins.EstimatedPercentile(num, percentile);
		}

		public static float EstimatedPercentile(this ValueBin[] bins, int totalSamples, float percentile)
		{
			int num = Mathf.RoundToInt(percentile * (float)totalSamples);
			if (num >= totalSamples)
			{
				num = totalSamples - 1;
			}
			int num2 = 0;
			while (num >= 0 && num2 < bins.Length)
			{
				num -= bins[num2++].f;
			}
			num2--;
			if (num2 >= bins.Length)
			{
				return bins[bins.Length - 1].v;
			}
			return (bins[num2].v + ((num2 <= 0) ? 0f : bins[num2 - 1].v)) * 0.5f;
		}
	}
}
