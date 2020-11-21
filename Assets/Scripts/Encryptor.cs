public static class Encryptor
{
	public static string Encrypt(string plainText)
	{
		return RijndaelEncryptor.Encrypt(plainText);
	}

	public static string Decrypt(string cipherText)
	{
		return RijndaelEncryptor.Decrypt(cipherText);
	}
}
