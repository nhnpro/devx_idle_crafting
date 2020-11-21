using System;

public static class L
{
	public static Func<A, bool> And<A>(this Func<A, bool> left, Func<A, bool> right)
	{
		return (A a) => left(a) && right(a);
	}

	public static Func<A, bool> Or<A>(this Func<A, bool> left, Func<A, bool> right)
	{
		return (A a) => left(a) || right(a);
	}
}
