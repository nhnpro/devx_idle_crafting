using Big;
using System.Collections.Generic;

public class ParameterRunnerContext : PropertyContext
{
	public override void Install(Dictionary<string, object> parameters)
	{
		ParameterRunner parameterRunner = new ParameterRunner();
		if (parameters != null && parameters.ContainsKey("IntValue"))
		{
			parameterRunner.IntValue.Value = (int)parameters["IntValue"];
		}
		if (parameters != null && parameters.ContainsKey("BigDoubleValue"))
		{
			parameterRunner.BigDoubleValue.Value = (BigDouble)parameters["BigDoubleValue"];
		}
		if (parameters != null && parameters.ContainsKey("StringValue"))
		{
			parameterRunner.StringValue.Value = (string)parameters["StringValue"];
		}
		if (parameters != null && parameters.ContainsKey("BoolValue"))
		{
			parameterRunner.BoolValue.Value = (bool)parameters["BoolValue"];
		}
		Add("ParameterRunner", parameterRunner);
	}
}
