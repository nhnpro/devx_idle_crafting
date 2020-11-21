public class GpuPerfData
{
	public string Gpu
	{
		get;
		private set;
	}

	public GpuPerfEnum Perf
	{
		get;
		private set;
	}

	public GpuPerfData(string gpu, GpuPerfEnum perf)
	{
		Gpu = gpu;
		Perf = perf;
	}
}
