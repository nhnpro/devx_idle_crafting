public class CircularBuffer<T>
{
	private int m_mask;

	private int m_pos;

	public T[] Buffer
	{
		get;
		private set;
	}

	public int Size
	{
		get;
		private set;
	}

	public CircularBuffer(int sizeInNpot)
	{
		Size = sizeInNpot;
		m_mask = Size - 1;
		Buffer = new T[Size];
	}

	public void Clear()
	{
		for (int i = 0; i < Size; i++)
		{
			Buffer[i] = default(T);
		}
	}

	public void Add(T obj)
	{
		m_pos++;
		m_pos &= m_mask;
		Buffer[m_pos] = obj;
	}

	public bool Contains(T obj)
	{
		for (int i = 0; i < Size; i++)
		{
			if (obj.Equals(Buffer[i]))
			{
				return true;
			}
		}
		return false;
	}
}
