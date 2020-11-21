public class StartAdventure : Swipeable
{
	protected override void OnSwipe()
	{
		Singleton<ChunkRunner>.Instance.StartAdventure();
	}
}
