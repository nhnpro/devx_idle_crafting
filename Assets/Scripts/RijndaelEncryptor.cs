using System;
using System.Security.Cryptography;
using System.Text;

public static class RijndaelEncryptor
{
	private static readonly byte[] keyArray = Encoding.UTF8.GetBytes("0yrABk$csrPDyq(kYPSfAzwnBENJ)i1l");

	public static string Encrypt(string toEncrypt)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(toEncrypt);
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.Key = keyArray;
		rijndaelManaged.Mode = CipherMode.ECB;
		rijndaelManaged.Padding = PaddingMode.PKCS7;
		ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor();
		byte[] array = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
		return Convert.ToBase64String(array, 0, array.Length);
	}

	public static string Decrypt(string toDecrypt)
	{
		byte[] array = Convert.FromBase64String(toDecrypt);
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.Key = keyArray;
		rijndaelManaged.Mode = CipherMode.ECB;
		rijndaelManaged.Padding = PaddingMode.PKCS7;
		ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor();
		byte[] bytes = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
		return Encoding.UTF8.GetString(bytes);
	}
}
