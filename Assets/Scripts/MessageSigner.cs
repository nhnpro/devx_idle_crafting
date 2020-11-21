using System;
using System.Security.Cryptography;
using System.Text;

public static class MessageSigner
{
	private const string ID = "BABECAGE";

	private static readonly byte[] keyArray = Encoding.UTF8.GetBytes("2oid82H2)€Hcaksh32€)ahgd229iASJS.Asefi");

	public static string Content(string input)
	{
		return (!input.StartsWith("BABECAGE")) ? input : input.Substring(input.IndexOf("\n") + 1);
	}

	public static string Resign(string input)
	{
		return Sign(Content(input));
	}

	public static string Sign(string input)
	{
		return "BABECAGE" + Hash(input) + "\n" + input;
	}

	public static string Hash(string input)
	{
		using (HMACSHA256 hMACSHA = new HMACSHA256(keyArray))
		{
			hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(input));
			return Convert.ToBase64String(hMACSHA.Hash);
		}
	}

	public static bool Verify(string signed)
	{
		using (new HMACSHA256(keyArray))
		{
			if (!signed.StartsWith("BABECAGE"))
			{
				return false;
			}
			int num = signed.IndexOf("\n");
			string a = signed.Substring("BABECAGE".Length, num - "BABECAGE".Length);
			string b = Hash(signed.Substring(num + 1));
			return a == b;
		}
	}
}
