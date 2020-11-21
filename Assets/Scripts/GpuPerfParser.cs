using System.Collections.Generic;
using System.Linq;

public static class GpuPerfParser
{
	public static List<GpuPerfData> ParseGpuPerf(string text)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text).Skip(1);
		return source.Select(delegate(string[] line)
		{
			string gpu = line.asString(0, line.toError<string>());
			GpuPerfEnum perf = line.asEnum(1, line.toError<GpuPerfEnum>());
			return new GpuPerfData(gpu, perf);
		}).ToList();
	}
}
