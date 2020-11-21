using System;

namespace UniRx.Operators
{
	internal interface ISelect<TR>
	{
		UniRx.IObservable<TR> CombinePredicate(Func<TR, bool> predicate);
	}
}
