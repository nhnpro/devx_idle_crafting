public interface IBlockFinder
{
	IBlock Get();

	IBlock GetOrFind();

	void Clear();
}
