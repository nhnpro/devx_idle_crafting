public static class PassThroughtEncryptor
{
	public static string Encrypt(string plainText)
	{
		return plainText.Replace(",", ",\n");
	}

	public static string Decrypt(string cipherText)
	{
		return cipherText;
	}
}
