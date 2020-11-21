using System;
using System.Collections.Generic;

namespace UniRx
{
	public class EventPattern<TEventArgs> : EventPattern<object, TEventArgs>
	{
		public EventPattern(object sender, TEventArgs e)
			: base(sender, e)
		{
		}
	}
	public class EventPattern<TSender, TEventArgs> : IEquatable<EventPattern<TSender, TEventArgs>>, IEventPattern<TSender, TEventArgs>
	{
		public TSender Sender
		{
			get;
			private set;
		}

		public TEventArgs EventArgs
		{
			get;
			private set;
		}

		public EventPattern(TSender sender, TEventArgs e)
		{
			Sender = sender;
			EventArgs = e;
		}

		public bool Equals(EventPattern<TSender, TEventArgs> other)
		{
			if (object.ReferenceEquals(null, other))
			{
				return false;
			}
			if (object.ReferenceEquals(this, other))
			{
				return true;
			}
			return EqualityComparer<TSender>.Default.Equals(Sender, other.Sender) && EqualityComparer<TEventArgs>.Default.Equals(EventArgs, other.EventArgs);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as EventPattern<TSender, TEventArgs>);
		}

		public override int GetHashCode()
		{
			int hashCode = EqualityComparer<TSender>.Default.GetHashCode(Sender);
			int hashCode2 = EqualityComparer<TEventArgs>.Default.GetHashCode(EventArgs);
			return (hashCode << 5) + (hashCode ^ hashCode2);
		}

		public static bool operator ==(EventPattern<TSender, TEventArgs> first, EventPattern<TSender, TEventArgs> second)
		{
			return object.Equals(first, second);
		}

		public static bool operator !=(EventPattern<TSender, TEventArgs> first, EventPattern<TSender, TEventArgs> second)
		{
			return !object.Equals(first, second);
		}
	}
}
