using System.Collections.Generic;
using System.Linq;

public static class SkillParser
{
	public static List<SkillConfig> ParseSkills(string text)
	{
		IEnumerable<string[]> source = TSVParser.Parse(text).Skip(1);
		return (from line in source
			select new SkillConfig
			{
				Name = line.asString(0, line.toError<string>()),
				DurationSeconds = line.asInt(1, line.toError<int>()),
				CoolDownSeconds = line.asInt(2, line.toError<int>()),
				LevelReq = line.asInt(3, line.toError<int>())
			}).ToList();
	}
}
