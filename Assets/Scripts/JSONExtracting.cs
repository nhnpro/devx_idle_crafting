using Big;
using System;

public static class JSONExtracting
{
	public static bool asBool(this JSONObject json, string name, Func<bool> orDefault)
	{
		return JSONConverter.Get(json, name, (JSONObject f) => f.b, orDefault, JSONObject.Type.BOOL);
	}

	public static float asFloat(this JSONObject json, string name, Func<float> orDefault)
	{
		return JSONConverter.Get(json, name, (JSONObject f) => f.n, orDefault, JSONObject.Type.NUMBER);
	}

	public static int asInt(this JSONObject json, string name, Func<int> orDefault)
	{
		return JSONConverter.Get(json, name, (JSONObject f) => (int)f.i, orDefault, JSONObject.Type.NUMBER);
	}

	public static long asLong(this JSONObject json, string name, Func<long> orDefault)
	{
		return JSONConverter.Get(json, name, (JSONObject f) => f.i, orDefault, JSONObject.Type.NUMBER);
	}

	public static string asString(this JSONObject json, string name, Func<string> orDefault)
	{
		return JSONConverter.Get(json, name, (JSONObject f) => f.str, orDefault, JSONObject.Type.STRING);
	}

	public static BigDouble asBigDouble(this JSONObject json, string name, Func<BigDouble> orDefault)
	{
		return JSONConverter.Get(json, name, (JSONObject f) => BigDouble.Parse(f.str), orDefault, JSONObject.Type.STRING);
	}

	public static JSONObject asJSONObject(this JSONObject json, string name, JSONObject.Type expectedType = JSONObject.Type.OBJECT)
	{
		return JSONConverter.Get(json, name, (JSONObject f) => f, () => new JSONObject(expectedType), expectedType);
	}

	public static T asCustom<T>(this JSONObject json, string name, Func<JSONObject, T> convert, Func<T> orDefault)
	{
		return JSONConverter.Get(json, name, (JSONObject f) => convert(f), orDefault);
	}

	public static Func<T> toError<T>(this JSONObject json, string message = "")
	{
		return delegate
		{
			throw new SystemException("Could not get the field. " + message);
		};
	}
}
