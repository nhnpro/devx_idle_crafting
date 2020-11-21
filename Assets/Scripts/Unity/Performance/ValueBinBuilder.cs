using System.Collections.Generic;

namespace Unity.Performance
{
	internal class ValueBinBuilder
	{
		private readonly List<ValueBin> _bins;

		public float CurrentHighestValue
		{
			get
			{
				float result;
				if (_bins.Count > 0)
				{
					ValueBin valueBin = _bins[_bins.Count - 1];
					result = valueBin.v;
				}
				else
				{
					result = 0f;
				}
				return result;
			}
		}

		public ValueBin[] Result => _bins.ToArray();

		public ValueBinBuilder()
		{
			_bins = new List<ValueBin>();
		}

		public void AddBin(float size)
		{
			_bins.Add(new ValueBin
			{
				v = CurrentHighestValue + size,
				f = 0
			});
		}

		public void AddBinsUpTo(float size, float limit)
		{
			while (CurrentHighestValue < limit)
			{
				AddBin(size);
			}
		}
	}
}
