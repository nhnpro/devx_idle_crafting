using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class PropertyFormerlyAs : Attribute
{
	public readonly string FormerName;

	public PropertyFormerlyAs(string name)
	{
		FormerName = name;
	}
}
