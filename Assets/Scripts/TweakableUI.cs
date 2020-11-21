using UnityEngine;

public class TweakableUI : MonoBehaviour
{
	public Tweakable TweakableObject
	{
		get;
		protected set;
	}

	public string TweakableName
	{
		get;
		protected set;
	}

	public string GetCategory()
	{
		string[] array = TweakableName.Split('.');
		if (array.Length < 1)
		{
			return "Default";
		}
		return array[0];
	}

	public string GetShortName()
	{
		string[] array = TweakableName.Split('.');
		if (array.Length > 1)
		{
			return array[1];
		}
		return array[0];
	}
}
