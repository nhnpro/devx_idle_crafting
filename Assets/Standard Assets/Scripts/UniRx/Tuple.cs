using System;
using System.Collections;
using System.Collections.Generic;

namespace UniRx
{
	public static class Tuple
	{
		public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>> Create<T1, T2, T3, T4, T5, T6, T7, T8>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8)
		{
			return new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8>(item8));
		}

		public static Tuple<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
		{
			return new Tuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7);
		}

		public static Tuple<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
		{
			return new Tuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6);
		}

		public static Tuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
		{
			return new Tuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
		}

		public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4)
		{
			return new Tuple<T1, T2, T3, T4>(item1, item2, item3, item4);
		}

		public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
		{
			return new Tuple<T1, T2, T3>(item1, item2, item3);
		}

		public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
		{
			return new Tuple<T1, T2>(item1, item2);
		}

		public static Tuple<T1> Create<T1>(T1 item1)
		{
			return new Tuple<T1>(item1);
		}
	}
	[Serializable]
	public class Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple, IEquatable<UniRx.Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>>
	{
		private T1 item1;

		private T2 item2;

		private T3 item3;

		private T4 item4;

		private T5 item5;

		private T6 item6;

		private T7 item7;

		private TRest rest;

		public T1 Item1 => item1;

		public T2 Item2 => item2;

		public T3 Item3 => item3;

		public T4 Item4 => item4;

		public T5 Item5 => item5;

		public T6 Item6 => item6;

		public T7 Item7 => item7;

		public TRest Rest => rest;

		public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, TRest rest)
		{
			this.item1 = item1;
			this.item2 = item2;
			this.item3 = item3;
			this.item4 = item4;
			this.item5 = item5;
			this.item6 = item6;
			this.item7 = item7;
			this.rest = rest;
			if (!(rest is ITuple))
			{
				throw new ArgumentException("rest", "The last element of an eight element tuple must be a Tuple.");
			}
		}

		int IComparable.CompareTo(object obj)
		{
			return ((IStructuralComparable)this).CompareTo(obj, (IComparer)Comparer<object>.Default);
		}

		int IStructuralComparable.CompareTo(object other, IComparer comparer)
		{
			if (other == null)
			{
				return 1;
			}
			if (!(other is Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>))
			{
				throw new ArgumentException("other");
			}
			Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> tuple = (UniRx.Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>)other;
			int num = comparer.Compare(item1, tuple.item1);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item2, tuple.item2);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item3, tuple.item3);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item4, tuple.item4);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item5, tuple.item5);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item6, tuple.item6);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item7, tuple.item7);
			if (num != 0)
			{
				return num;
			}
			return comparer.Compare(rest, tuple.rest);
		}

		public override bool Equals(object obj)
		{
			return ((IStructuralEquatable)this).Equals(obj, (IEqualityComparer)EqualityComparer<object>.Default);
		}

		bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
		{
			if (!(other is Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>))
			{
				return false;
			}
			Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> tuple = (UniRx.Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>)other;
			return comparer.Equals(item1, tuple.item1) && comparer.Equals(item2, tuple.item2) && comparer.Equals(item3, tuple.item3) && comparer.Equals(item4, tuple.item4) && comparer.Equals(item5, tuple.item5) && comparer.Equals(item6, tuple.item6) && comparer.Equals(item7, tuple.item7) && comparer.Equals(rest, tuple.rest);
		}

		public override int GetHashCode()
		{
			EqualityComparer<T1> @default = EqualityComparer<T1>.Default;
			EqualityComparer<T2> default2 = EqualityComparer<T2>.Default;
			EqualityComparer<T3> default3 = EqualityComparer<T3>.Default;
			EqualityComparer<T4> default4 = EqualityComparer<T4>.Default;
			EqualityComparer<T5> default5 = EqualityComparer<T5>.Default;
			EqualityComparer<T6> default6 = EqualityComparer<T6>.Default;
			EqualityComparer<T7> default7 = EqualityComparer<T7>.Default;
			EqualityComparer<TRest> default8 = EqualityComparer<TRest>.Default;
			int hashCode = @default.GetHashCode(item1);
			hashCode = (((hashCode << 5) + hashCode) ^ default2.GetHashCode(item2));
			int hashCode2 = default3.GetHashCode(item3);
			hashCode2 = (((hashCode2 << 5) + hashCode2) ^ default4.GetHashCode(item4));
			hashCode = (((hashCode << 5) + hashCode) ^ hashCode2);
			hashCode2 = default5.GetHashCode(item5);
			hashCode2 = (((hashCode2 << 5) + hashCode2) ^ default6.GetHashCode(item6));
			int hashCode3 = default7.GetHashCode(item7);
			hashCode3 = (((hashCode3 << 5) + hashCode3) ^ default8.GetHashCode(rest));
			hashCode2 = (((hashCode2 << 5) + hashCode2) ^ hashCode3);
			return ((hashCode << 5) + hashCode) ^ hashCode2;
		}

		int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			int hashCode = comparer.GetHashCode(item1);
			hashCode = (((hashCode << 5) + hashCode) ^ comparer.GetHashCode(item2));
			int hashCode2 = comparer.GetHashCode(item3);
			hashCode2 = (((hashCode2 << 5) + hashCode2) ^ comparer.GetHashCode(item4));
			hashCode = (((hashCode << 5) + hashCode) ^ hashCode2);
			hashCode2 = comparer.GetHashCode(item5);
			hashCode2 = (((hashCode2 << 5) + hashCode2) ^ comparer.GetHashCode(item6));
			int hashCode3 = comparer.GetHashCode(item7);
			hashCode3 = (((hashCode3 << 5) + hashCode3) ^ comparer.GetHashCode(rest));
			hashCode2 = (((hashCode2 << 5) + hashCode2) ^ hashCode3);
			return ((hashCode << 5) + hashCode) ^ hashCode2;
		}

		string ITuple.ToString()
		{
			return $"{item1}, {item2}, {item3}, {item4}, {item5}, {item6}, {item7}, {((ITuple)(object)rest).ToString()}";
		}

		public override string ToString()
		{
			return "(" + ((ITuple)this).ToString() + ")";
		}

		public bool Equals(UniRx.Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> other)
		{
			EqualityComparer<T1> @default = EqualityComparer<T1>.Default;
			EqualityComparer<T2> default2 = EqualityComparer<T2>.Default;
			EqualityComparer<T3> default3 = EqualityComparer<T3>.Default;
			EqualityComparer<T4> default4 = EqualityComparer<T4>.Default;
			EqualityComparer<T5> default5 = EqualityComparer<T5>.Default;
			EqualityComparer<T6> default6 = EqualityComparer<T6>.Default;
			EqualityComparer<T7> default7 = EqualityComparer<T7>.Default;
			EqualityComparer<TRest> default8 = EqualityComparer<TRest>.Default;
			return @default.Equals(item1, other.Item1) && default2.Equals(item2, other.Item2) && default3.Equals(item3, other.Item3) && default4.Equals(item4, other.Item4) && default5.Equals(item5, other.Item5) && default6.Equals(item6, other.Item6) && default7.Equals(item7, other.Item7) && default8.Equals(rest, other.rest);
		}
	}
	[Serializable]
	public struct Tuple<T1> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple, IEquatable<UniRx.Tuple<T1>>
	{
		private T1 item1;

		public T1 Item1 => item1;

		public Tuple(T1 item1)
		{
			this.item1 = item1;
		}

		int IComparable.CompareTo(object obj)
		{
			return ((IStructuralComparable)this).CompareTo(obj, (IComparer)Comparer<object>.Default);
		}

		int IStructuralComparable.CompareTo(object other, IComparer comparer)
		{
			if (other == null)
			{
				return 1;
			}
			if (!(other is Tuple<T1>))
			{
				throw new ArgumentException("other");
			}
			Tuple<T1> tuple = (UniRx.Tuple<T1>)other;
			return comparer.Compare(item1, tuple.item1);
		}

		public override bool Equals(object obj)
		{
			return ((IStructuralEquatable)this).Equals(obj, (IEqualityComparer)EqualityComparer<object>.Default);
		}

		bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
		{
			if (!(other is Tuple<T1>))
			{
				return false;
			}
			Tuple<T1> tuple = (UniRx.Tuple<T1>)other;
			return comparer.Equals(item1, tuple.item1);
		}

		public override int GetHashCode()
		{
			return EqualityComparer<T1>.Default.GetHashCode(item1);
		}

		int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			return comparer.GetHashCode(item1);
		}

		string ITuple.ToString()
		{
			return $"{item1}";
		}

		public override string ToString()
		{
			return "(" + ((ITuple)this).ToString() + ")";
		}

		public bool Equals(UniRx.Tuple<T1> other)
		{
			return EqualityComparer<T1>.Default.Equals(item1, other.item1);
		}
	}
	[Serializable]
	public struct Tuple<T1, T2> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple, IEquatable<UniRx.Tuple<T1, T2>>
	{
		private T1 item1;

		private T2 item2;

		public T1 Item1 => item1;

		public T2 Item2 => item2;

		public Tuple(T1 item1, T2 item2)
		{
			this.item1 = item1;
			this.item2 = item2;
		}

		int IComparable.CompareTo(object obj)
		{
			return ((IStructuralComparable)this).CompareTo(obj, (IComparer)Comparer<object>.Default);
		}

		int IStructuralComparable.CompareTo(object other, IComparer comparer)
		{
			if (other == null)
			{
				return 1;
			}
			if (!(other is Tuple<T1, T2>))
			{
				throw new ArgumentException("other");
			}
			Tuple<T1, T2> tuple = (UniRx.Tuple<T1, T2>)other;
			int num = comparer.Compare(item1, tuple.item1);
			if (num != 0)
			{
				return num;
			}
			return comparer.Compare(item2, tuple.item2);
		}

		public override bool Equals(object obj)
		{
			return ((IStructuralEquatable)this).Equals(obj, (IEqualityComparer)EqualityComparer<object>.Default);
		}

		bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
		{
			if (!(other is Tuple<T1, T2>))
			{
				return false;
			}
			Tuple<T1, T2> tuple = (UniRx.Tuple<T1, T2>)other;
			return comparer.Equals(item1, tuple.item1) && comparer.Equals(item2, tuple.item2);
		}

		public override int GetHashCode()
		{
			EqualityComparer<T1> @default = EqualityComparer<T1>.Default;
			EqualityComparer<T2> default2 = EqualityComparer<T2>.Default;
			int hashCode = @default.GetHashCode(item1);
			return ((hashCode << 5) + hashCode) ^ default2.GetHashCode(item2);
		}

		int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			int hashCode = comparer.GetHashCode(item1);
			return ((hashCode << 5) + hashCode) ^ comparer.GetHashCode(item2);
		}

		string ITuple.ToString()
		{
			return $"{item1}, {item2}";
		}

		public override string ToString()
		{
			return "(" + ((ITuple)this).ToString() + ")";
		}

		public bool Equals(UniRx.Tuple<T1, T2> other)
		{
			EqualityComparer<T1> @default = EqualityComparer<T1>.Default;
			EqualityComparer<T2> default2 = EqualityComparer<T2>.Default;
			return @default.Equals(item1, other.item1) && default2.Equals(item2, other.item2);
		}
	}
	[Serializable]
	public struct Tuple<T1, T2, T3> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple, IEquatable<UniRx.Tuple<T1, T2, T3>>
	{
		private T1 item1;

		private T2 item2;

		private T3 item3;

		public T1 Item1 => item1;

		public T2 Item2 => item2;

		public T3 Item3 => item3;

		public Tuple(T1 item1, T2 item2, T3 item3)
		{
			this.item1 = item1;
			this.item2 = item2;
			this.item3 = item3;
		}

		int IComparable.CompareTo(object obj)
		{
			return ((IStructuralComparable)this).CompareTo(obj, (IComparer)Comparer<object>.Default);
		}

		int IStructuralComparable.CompareTo(object other, IComparer comparer)
		{
			if (other == null)
			{
				return 1;
			}
			if (!(other is Tuple<T1, T2, T3>))
			{
				throw new ArgumentException("other");
			}
			Tuple<T1, T2, T3> tuple = (UniRx.Tuple<T1, T2, T3>)other;
			int num = comparer.Compare(item1, tuple.item1);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item2, tuple.item2);
			if (num != 0)
			{
				return num;
			}
			return comparer.Compare(item3, tuple.item3);
		}

		public override bool Equals(object obj)
		{
			return ((IStructuralEquatable)this).Equals(obj, (IEqualityComparer)EqualityComparer<object>.Default);
		}

		bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
		{
			if (!(other is Tuple<T1, T2, T3>))
			{
				return false;
			}
			Tuple<T1, T2, T3> tuple = (UniRx.Tuple<T1, T2, T3>)other;
			return comparer.Equals(item1, tuple.item1) && comparer.Equals(item2, tuple.item2) && comparer.Equals(item3, tuple.item3);
		}

		public override int GetHashCode()
		{
			EqualityComparer<T1> @default = EqualityComparer<T1>.Default;
			EqualityComparer<T2> default2 = EqualityComparer<T2>.Default;
			EqualityComparer<T3> default3 = EqualityComparer<T3>.Default;
			int hashCode = @default.GetHashCode(item1);
			hashCode = (((hashCode << 5) + hashCode) ^ default2.GetHashCode(item2));
			return ((hashCode << 5) + hashCode) ^ default3.GetHashCode(item3);
		}

		int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			int hashCode = comparer.GetHashCode(item1);
			hashCode = (((hashCode << 5) + hashCode) ^ comparer.GetHashCode(item2));
			return ((hashCode << 5) + hashCode) ^ comparer.GetHashCode(item3);
		}

		string ITuple.ToString()
		{
			return $"{item1}, {item2}, {item3}";
		}

		public override string ToString()
		{
			return "(" + ((ITuple)this).ToString() + ")";
		}

		public bool Equals(UniRx.Tuple<T1, T2, T3> other)
		{
			EqualityComparer<T1> @default = EqualityComparer<T1>.Default;
			EqualityComparer<T2> default2 = EqualityComparer<T2>.Default;
			EqualityComparer<T3> default3 = EqualityComparer<T3>.Default;
			return @default.Equals(item1, other.item1) && default2.Equals(item2, other.item2) && default3.Equals(item3, other.item3);
		}
	}
	[Serializable]
	public struct Tuple<T1, T2, T3, T4> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple, IEquatable<UniRx.Tuple<T1, T2, T3, T4>>
	{
		private T1 item1;

		private T2 item2;

		private T3 item3;

		private T4 item4;

		public T1 Item1 => item1;

		public T2 Item2 => item2;

		public T3 Item3 => item3;

		public T4 Item4 => item4;

		public Tuple(T1 item1, T2 item2, T3 item3, T4 item4)
		{
			this.item1 = item1;
			this.item2 = item2;
			this.item3 = item3;
			this.item4 = item4;
		}

		int IComparable.CompareTo(object obj)
		{
			return ((IStructuralComparable)this).CompareTo(obj, (IComparer)Comparer<object>.Default);
		}

		int IStructuralComparable.CompareTo(object other, IComparer comparer)
		{
			if (other == null)
			{
				return 1;
			}
			if (!(other is Tuple<T1, T2, T3, T4>))
			{
				throw new ArgumentException("other");
			}
			Tuple<T1, T2, T3, T4> tuple = (UniRx.Tuple<T1, T2, T3, T4>)other;
			int num = comparer.Compare(item1, tuple.item1);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item2, tuple.item2);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item3, tuple.item3);
			if (num != 0)
			{
				return num;
			}
			return comparer.Compare(item4, tuple.item4);
		}

		public override bool Equals(object obj)
		{
			return ((IStructuralEquatable)this).Equals(obj, (IEqualityComparer)EqualityComparer<object>.Default);
		}

		bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
		{
			if (!(other is Tuple<T1, T2, T3, T4>))
			{
				return false;
			}
			Tuple<T1, T2, T3, T4> tuple = (UniRx.Tuple<T1, T2, T3, T4>)other;
			return comparer.Equals(item1, tuple.item1) && comparer.Equals(item2, tuple.item2) && comparer.Equals(item3, tuple.item3) && comparer.Equals(item4, tuple.item4);
		}

		public override int GetHashCode()
		{
			EqualityComparer<T1> @default = EqualityComparer<T1>.Default;
			EqualityComparer<T2> default2 = EqualityComparer<T2>.Default;
			EqualityComparer<T3> default3 = EqualityComparer<T3>.Default;
			EqualityComparer<T4> default4 = EqualityComparer<T4>.Default;
			int hashCode = @default.GetHashCode(item1);
			hashCode = (((hashCode << 5) + hashCode) ^ default2.GetHashCode(item2));
			int hashCode2 = default3.GetHashCode(item3);
			hashCode2 = (((hashCode2 << 5) + hashCode2) ^ default4.GetHashCode(item4));
			return ((hashCode << 5) + hashCode) ^ hashCode2;
		}

		int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			int hashCode = comparer.GetHashCode(item1);
			hashCode = (((hashCode << 5) + hashCode) ^ comparer.GetHashCode(item2));
			int hashCode2 = comparer.GetHashCode(item3);
			hashCode2 = (((hashCode2 << 5) + hashCode2) ^ comparer.GetHashCode(item4));
			return ((hashCode << 5) + hashCode) ^ hashCode2;
		}

		string ITuple.ToString()
		{
			return $"{item1}, {item2}, {item3}, {item4}";
		}

		public override string ToString()
		{
			return "(" + ((ITuple)this).ToString() + ")";
		}

		public bool Equals(UniRx.Tuple<T1, T2, T3, T4> other)
		{
			EqualityComparer<T1> @default = EqualityComparer<T1>.Default;
			EqualityComparer<T2> default2 = EqualityComparer<T2>.Default;
			EqualityComparer<T3> default3 = EqualityComparer<T3>.Default;
			EqualityComparer<T4> default4 = EqualityComparer<T4>.Default;
			return @default.Equals(item1, other.item1) && default2.Equals(item2, other.item2) && default3.Equals(item3, other.item3) && default4.Equals(item4, other.item4);
		}
	}
	[Serializable]
	public struct Tuple<T1, T2, T3, T4, T5> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple, IEquatable<UniRx.Tuple<T1, T2, T3, T4, T5>>
	{
		private T1 item1;

		private T2 item2;

		private T3 item3;

		private T4 item4;

		private T5 item5;

		public T1 Item1 => item1;

		public T2 Item2 => item2;

		public T3 Item3 => item3;

		public T4 Item4 => item4;

		public T5 Item5 => item5;

		public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
		{
			this.item1 = item1;
			this.item2 = item2;
			this.item3 = item3;
			this.item4 = item4;
			this.item5 = item5;
		}

		int IComparable.CompareTo(object obj)
		{
			return ((IStructuralComparable)this).CompareTo(obj, (IComparer)Comparer<object>.Default);
		}

		int IStructuralComparable.CompareTo(object other, IComparer comparer)
		{
			if (other == null)
			{
				return 1;
			}
			if (!(other is Tuple<T1, T2, T3, T4, T5>))
			{
				throw new ArgumentException("other");
			}
			Tuple<T1, T2, T3, T4, T5> tuple = (UniRx.Tuple<T1, T2, T3, T4, T5>)other;
			int num = comparer.Compare(item1, tuple.item1);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item2, tuple.item2);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item3, tuple.item3);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item4, tuple.item4);
			if (num != 0)
			{
				return num;
			}
			return comparer.Compare(item5, tuple.item5);
		}

		public override bool Equals(object obj)
		{
			return ((IStructuralEquatable)this).Equals(obj, (IEqualityComparer)EqualityComparer<object>.Default);
		}

		bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
		{
			if (!(other is Tuple<T1, T2, T3, T4, T5>))
			{
				return false;
			}
			Tuple<T1, T2, T3, T4, T5> tuple = (UniRx.Tuple<T1, T2, T3, T4, T5>)other;
			return comparer.Equals(item1, tuple.item1) && comparer.Equals(item2, tuple.item2) && comparer.Equals(item3, tuple.item3) && comparer.Equals(item4, tuple.item4) && comparer.Equals(item5, tuple.item5);
		}

		public override int GetHashCode()
		{
			EqualityComparer<T1> @default = EqualityComparer<T1>.Default;
			EqualityComparer<T2> default2 = EqualityComparer<T2>.Default;
			EqualityComparer<T3> default3 = EqualityComparer<T3>.Default;
			EqualityComparer<T4> default4 = EqualityComparer<T4>.Default;
			EqualityComparer<T5> default5 = EqualityComparer<T5>.Default;
			int hashCode = @default.GetHashCode(item1);
			hashCode = (((hashCode << 5) + hashCode) ^ default2.GetHashCode(item2));
			int hashCode2 = default3.GetHashCode(item3);
			hashCode2 = (((hashCode2 << 5) + hashCode2) ^ default4.GetHashCode(item4));
			hashCode = (((hashCode << 5) + hashCode) ^ hashCode2);
			return ((hashCode << 5) + hashCode) ^ default5.GetHashCode(item5);
		}

		int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			int hashCode = comparer.GetHashCode(item1);
			hashCode = (((hashCode << 5) + hashCode) ^ comparer.GetHashCode(item2));
			int hashCode2 = comparer.GetHashCode(item3);
			hashCode2 = (((hashCode2 << 5) + hashCode2) ^ comparer.GetHashCode(item4));
			hashCode = (((hashCode << 5) + hashCode) ^ hashCode2);
			return ((hashCode << 5) + hashCode) ^ comparer.GetHashCode(item5);
		}

		string ITuple.ToString()
		{
			return $"{item1}, {item2}, {item3}, {item4}, {item5}";
		}

		public override string ToString()
		{
			return "(" + ((ITuple)this).ToString() + ")";
		}

		public bool Equals(UniRx.Tuple<T1, T2, T3, T4, T5> other)
		{
			EqualityComparer<T1> @default = EqualityComparer<T1>.Default;
			EqualityComparer<T2> default2 = EqualityComparer<T2>.Default;
			EqualityComparer<T3> default3 = EqualityComparer<T3>.Default;
			EqualityComparer<T4> default4 = EqualityComparer<T4>.Default;
			EqualityComparer<T5> default5 = EqualityComparer<T5>.Default;
			return @default.Equals(item1, other.Item1) && default2.Equals(item2, other.Item2) && default3.Equals(item3, other.Item3) && default4.Equals(item4, other.Item4) && default5.Equals(item5, other.Item5);
		}
	}
	[Serializable]
	public struct Tuple<T1, T2, T3, T4, T5, T6> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple, IEquatable<UniRx.Tuple<T1, T2, T3, T4, T5, T6>>
	{
		private T1 item1;

		private T2 item2;

		private T3 item3;

		private T4 item4;

		private T5 item5;

		private T6 item6;

		public T1 Item1 => item1;

		public T2 Item2 => item2;

		public T3 Item3 => item3;

		public T4 Item4 => item4;

		public T5 Item5 => item5;

		public T6 Item6 => item6;

		public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
		{
			this.item1 = item1;
			this.item2 = item2;
			this.item3 = item3;
			this.item4 = item4;
			this.item5 = item5;
			this.item6 = item6;
		}

		int IComparable.CompareTo(object obj)
		{
			return ((IStructuralComparable)this).CompareTo(obj, (IComparer)Comparer<object>.Default);
		}

		int IStructuralComparable.CompareTo(object other, IComparer comparer)
		{
			if (other == null)
			{
				return 1;
			}
			if (!(other is Tuple<T1, T2, T3, T4, T5, T6>))
			{
				throw new ArgumentException("other");
			}
			Tuple<T1, T2, T3, T4, T5, T6> tuple = (UniRx.Tuple<T1, T2, T3, T4, T5, T6>)other;
			int num = comparer.Compare(item1, tuple.item1);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item2, tuple.item2);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item3, tuple.item3);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item4, tuple.item4);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item5, tuple.item5);
			if (num != 0)
			{
				return num;
			}
			return comparer.Compare(item6, tuple.item6);
		}

		public override bool Equals(object obj)
		{
			return ((IStructuralEquatable)this).Equals(obj, (IEqualityComparer)EqualityComparer<object>.Default);
		}

		bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
		{
			if (!(other is Tuple<T1, T2, T3, T4, T5, T6>))
			{
				return false;
			}
			Tuple<T1, T2, T3, T4, T5, T6> tuple = (UniRx.Tuple<T1, T2, T3, T4, T5, T6>)other;
			return comparer.Equals(item1, tuple.item1) && comparer.Equals(item2, tuple.item2) && comparer.Equals(item3, tuple.item3) && comparer.Equals(item4, tuple.item4) && comparer.Equals(item5, tuple.item5) && comparer.Equals(item6, tuple.item6);
		}

		public override int GetHashCode()
		{
			EqualityComparer<T1> @default = EqualityComparer<T1>.Default;
			EqualityComparer<T2> default2 = EqualityComparer<T2>.Default;
			EqualityComparer<T3> default3 = EqualityComparer<T3>.Default;
			EqualityComparer<T4> default4 = EqualityComparer<T4>.Default;
			EqualityComparer<T5> default5 = EqualityComparer<T5>.Default;
			EqualityComparer<T6> default6 = EqualityComparer<T6>.Default;
			int hashCode = @default.GetHashCode(item1);
			hashCode = (((hashCode << 5) + hashCode) ^ default2.GetHashCode(item2));
			int hashCode2 = default3.GetHashCode(item3);
			hashCode2 = (((hashCode2 << 5) + hashCode2) ^ default4.GetHashCode(item4));
			hashCode = (((hashCode << 5) + hashCode) ^ hashCode2);
			hashCode2 = default5.GetHashCode(item5);
			hashCode2 = (((hashCode2 << 5) + hashCode2) ^ default6.GetHashCode(item6));
			return ((hashCode << 5) + hashCode) ^ hashCode2;
		}

		int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			int hashCode = comparer.GetHashCode(item1);
			hashCode = (((hashCode << 5) + hashCode) ^ comparer.GetHashCode(item2));
			int hashCode2 = comparer.GetHashCode(item3);
			hashCode2 = (((hashCode2 << 5) + hashCode2) ^ comparer.GetHashCode(item4));
			hashCode = (((hashCode << 5) + hashCode) ^ hashCode2);
			hashCode2 = comparer.GetHashCode(item5);
			hashCode2 = (((hashCode2 << 5) + hashCode2) ^ comparer.GetHashCode(item6));
			return ((hashCode << 5) + hashCode) ^ hashCode2;
		}

		string ITuple.ToString()
		{
			return $"{item1}, {item2}, {item3}, {item4}, {item5}, {item6}";
		}

		public override string ToString()
		{
			return "(" + ((ITuple)this).ToString() + ")";
		}

		public bool Equals(UniRx.Tuple<T1, T2, T3, T4, T5, T6> other)
		{
			EqualityComparer<T1> @default = EqualityComparer<T1>.Default;
			EqualityComparer<T2> default2 = EqualityComparer<T2>.Default;
			EqualityComparer<T3> default3 = EqualityComparer<T3>.Default;
			EqualityComparer<T4> default4 = EqualityComparer<T4>.Default;
			EqualityComparer<T5> default5 = EqualityComparer<T5>.Default;
			EqualityComparer<T6> default6 = EqualityComparer<T6>.Default;
			return @default.Equals(item1, other.Item1) && default2.Equals(item2, other.Item2) && default3.Equals(item3, other.Item3) && default4.Equals(item4, other.Item4) && default5.Equals(item5, other.Item5) && default6.Equals(item6, other.Item6);
		}
	}
	[Serializable]
	public struct Tuple<T1, T2, T3, T4, T5, T6, T7> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple, IEquatable<UniRx.Tuple<T1, T2, T3, T4, T5, T6, T7>>
	{
		private T1 item1;

		private T2 item2;

		private T3 item3;

		private T4 item4;

		private T5 item5;

		private T6 item6;

		private T7 item7;

		public T1 Item1 => item1;

		public T2 Item2 => item2;

		public T3 Item3 => item3;

		public T4 Item4 => item4;

		public T5 Item5 => item5;

		public T6 Item6 => item6;

		public T7 Item7 => item7;

		public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
		{
			this.item1 = item1;
			this.item2 = item2;
			this.item3 = item3;
			this.item4 = item4;
			this.item5 = item5;
			this.item6 = item6;
			this.item7 = item7;
		}

		int IComparable.CompareTo(object obj)
		{
			return ((IStructuralComparable)this).CompareTo(obj, (IComparer)Comparer<object>.Default);
		}

		int IStructuralComparable.CompareTo(object other, IComparer comparer)
		{
			if (other == null)
			{
				return 1;
			}
			if (!(other is Tuple<T1, T2, T3, T4, T5, T6, T7>))
			{
				throw new ArgumentException("other");
			}
			Tuple<T1, T2, T3, T4, T5, T6, T7> tuple = (UniRx.Tuple<T1, T2, T3, T4, T5, T6, T7>)other;
			int num = comparer.Compare(item1, tuple.item1);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item2, tuple.item2);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item3, tuple.item3);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item4, tuple.item4);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item5, tuple.item5);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item6, tuple.item6);
			if (num != 0)
			{
				return num;
			}
			return comparer.Compare(item7, tuple.item7);
		}

		public override bool Equals(object obj)
		{
			return ((IStructuralEquatable)this).Equals(obj, (IEqualityComparer)EqualityComparer<object>.Default);
		}

		bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
		{
			if (!(other is Tuple<T1, T2, T3, T4, T5, T6, T7>))
			{
				return false;
			}
			Tuple<T1, T2, T3, T4, T5, T6, T7> tuple = (UniRx.Tuple<T1, T2, T3, T4, T5, T6, T7>)other;
			return comparer.Equals(item1, tuple.item1) && comparer.Equals(item2, tuple.item2) && comparer.Equals(item3, tuple.item3) && comparer.Equals(item4, tuple.item4) && comparer.Equals(item5, tuple.item5) && comparer.Equals(item6, tuple.item6) && comparer.Equals(item7, tuple.item7);
		}

		public override int GetHashCode()
		{
			EqualityComparer<T1> @default = EqualityComparer<T1>.Default;
			EqualityComparer<T2> default2 = EqualityComparer<T2>.Default;
			EqualityComparer<T3> default3 = EqualityComparer<T3>.Default;
			EqualityComparer<T4> default4 = EqualityComparer<T4>.Default;
			EqualityComparer<T5> default5 = EqualityComparer<T5>.Default;
			EqualityComparer<T6> default6 = EqualityComparer<T6>.Default;
			EqualityComparer<T7> default7 = EqualityComparer<T7>.Default;
			int hashCode = @default.GetHashCode(item1);
			hashCode = (((hashCode << 5) + hashCode) ^ default2.GetHashCode(item2));
			int hashCode2 = default3.GetHashCode(item3);
			hashCode2 = (((hashCode2 << 5) + hashCode2) ^ default4.GetHashCode(item4));
			hashCode = (((hashCode << 5) + hashCode) ^ hashCode2);
			hashCode2 = default5.GetHashCode(item5);
			hashCode2 = (((hashCode2 << 5) + hashCode2) ^ default6.GetHashCode(item6));
			hashCode2 = (((hashCode2 << 5) + hashCode2) ^ default7.GetHashCode(item7));
			return ((hashCode << 5) + hashCode) ^ hashCode2;
		}

		int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			int hashCode = comparer.GetHashCode(item1);
			hashCode = (((hashCode << 5) + hashCode) ^ comparer.GetHashCode(item2));
			int hashCode2 = comparer.GetHashCode(item3);
			hashCode2 = (((hashCode2 << 5) + hashCode2) ^ comparer.GetHashCode(item4));
			hashCode = (((hashCode << 5) + hashCode) ^ hashCode2);
			hashCode2 = comparer.GetHashCode(item5);
			hashCode2 = (((hashCode2 << 5) + hashCode2) ^ comparer.GetHashCode(item6));
			hashCode2 = (((hashCode2 << 5) + hashCode2) ^ comparer.GetHashCode(item7));
			return ((hashCode << 5) + hashCode) ^ hashCode2;
		}

		string ITuple.ToString()
		{
			return $"{item1}, {item2}, {item3}, {item4}, {item5}, {item6}, {item7}";
		}

		public override string ToString()
		{
			return "(" + ((ITuple)this).ToString() + ")";
		}

		public bool Equals(UniRx.Tuple<T1, T2, T3, T4, T5, T6, T7> other)
		{
			EqualityComparer<T1> @default = EqualityComparer<T1>.Default;
			EqualityComparer<T2> default2 = EqualityComparer<T2>.Default;
			EqualityComparer<T3> default3 = EqualityComparer<T3>.Default;
			EqualityComparer<T4> default4 = EqualityComparer<T4>.Default;
			EqualityComparer<T5> default5 = EqualityComparer<T5>.Default;
			EqualityComparer<T6> default6 = EqualityComparer<T6>.Default;
			EqualityComparer<T7> default7 = EqualityComparer<T7>.Default;
			return @default.Equals(item1, other.Item1) && default2.Equals(item2, other.Item2) && default3.Equals(item3, other.Item3) && default4.Equals(item4, other.Item4) && default5.Equals(item5, other.Item5) && default6.Equals(item6, other.Item6) && default7.Equals(item7, other.Item7);
		}
	}
}
