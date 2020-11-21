using System;
using System.Collections.Generic;
using System.Security.Cryptography;

public sealed class Crc32 : HashAlgorithm
{
	public const uint DefaultPolynomial = 3988292384u;

	public const uint DefaultSeed = uint.MaxValue;

	private static uint[] defaultTable;

	private readonly uint seed;

	private readonly uint[] table;

	private uint hash;

	public override int HashSize => 32;

	public Crc32()
		: this(3988292384u, uint.MaxValue)
	{
	}

	public Crc32(uint polynomial, uint seed)
	{
		table = InitializeTable(polynomial);
		this.seed = (hash = seed);
	}

	public override void Initialize()
	{
		hash = seed;
	}

	protected override void HashCore(byte[] buffer, int start, int length)
	{
		hash = CalculateHash(table, hash, buffer, start, length);
	}

	protected override byte[] HashFinal()
	{
		return HashValue = UInt32ToBigEndianBytes(~hash);
	}

	public static uint Compute(byte[] buffer)
	{
		return Compute(uint.MaxValue, buffer);
	}

	public static uint Compute(uint seed, byte[] buffer)
	{
		return Compute(3988292384u, seed, buffer);
	}

	public static uint Compute(uint polynomial, uint seed, byte[] buffer)
	{
		return ~CalculateHash(InitializeTable(polynomial), seed, buffer, 0, buffer.Length);
	}

	private static uint[] InitializeTable(uint polynomial)
	{
		if (polynomial == 3988292384u && defaultTable != null)
		{
			return defaultTable;
		}
		uint[] array = new uint[256];
		for (int i = 0; i < 256; i++)
		{
			uint num = (uint)i;
			for (int j = 0; j < 8; j++)
			{
				num = (((num & 1) != 1) ? (num >> 1) : ((num >> 1) ^ polynomial));
			}
			array[i] = num;
		}
		if (polynomial == 3988292384u)
		{
			defaultTable = array;
		}
		return array;
	}

	private static uint CalculateHash(uint[] table, uint seed, IList<byte> buffer, int start, int size)
	{
		uint num = seed;
		for (int i = start; i < size - start; i++)
		{
			num = ((num >> 8) ^ table[buffer[i] ^ (num & 0xFF)]);
		}
		return num;
	}

	private static byte[] UInt32ToBigEndianBytes(uint uint32)
	{
		byte[] bytes = BitConverter.GetBytes(uint32);
		if (BitConverter.IsLittleEndian)
		{
			Array.Reverse(bytes);
		}
		return bytes;
	}
}
