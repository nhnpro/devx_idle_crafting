using System;

internal class JSONConverter
{
	public static T Get<T>(JSONObject json, string name, Func<JSONObject, T> convert, Func<T> orDefault, JSONObject.Type expectedType = JSONObject.Type.NULL)
	{
		try
		{
			if (!json.HasField(name))
			{
				return orDefault();
			}
			JSONObject field = json.GetField(name);
			if (expectedType != 0 && expectedType != field.type)
			{
				UnityEngine.Debug.LogError("Tried to get field '" + name + "' as " + expectedType + ", but it is incompatible (" + field.type + "). Returning default value.");
				return orDefault();
			}
			return convert(json.GetField(name));
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogError("Exception during converting '" + name + "' e:  " + ex.Message + ". Returning default value.");
			return orDefault();
		}
	}
}
