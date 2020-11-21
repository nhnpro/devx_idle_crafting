public class AnimState
{
	public delegate void ActionDelegate();

	public delegate void EventDelegate(string str);

	public ActionDelegate Enter;

	public ActionDelegate Exit;

	public ActionDelegate Step;

	public ActionDelegate AnimComplete;

	public EventDelegate AnimEvent;

	public string[] AnimationNames;

	public bool Looping;

	public AnimSpeed Speed = new AnimSpeed(1f, 1f);

	public float MixDuration;

	public int Index
	{
		get;
		private set;
	}

	public AnimState(int index)
	{
		Index = index;
	}
}
