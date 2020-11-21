using System.Collections.Generic;
using UniRx;

public class AudioRunner : Singleton<AudioRunner>
{
	private ReactiveProperty<bool> ShouldPlayTournamentCheer = new ReactiveProperty<bool>(initialValue: false);

	public AudioRunner()
	{
		SceneLoader instance = SceneLoader.Instance;
		BindingManager bind = BindingManager.Instance;
		UniRx.IObservable<bool> right = from chunk in Singleton<WorldRunner>.Instance.CurrentChunk
			select chunk.Index == 0;
		Singleton<WorldRunner>.Instance.CurrentBiomeConfig.CombineLatest(right, (BiomeConfig biome, bool chunk) => new
		{
			biome,
			chunk
		}).DistinctUntilChanged().Subscribe(tuple =>
		{
			if (tuple.chunk)
			{
				AudioController.Instance.QueueEvent(new AudioEvent(bind.BiomeList.BasecampAudio, AUDIOEVENTACTION.Play));
			}
			else
			{
				AudioController.Instance.QueueEvent(new AudioEvent(bind.BiomeList.Biomes[tuple.biome.BiomeIndex].BiomeAudio, AUDIOEVENTACTION.Play));
			}
		})
			.AddTo(instance);
		Singleton<WorldRunner>.Instance.MapSequence.Subscribe(delegate(bool seq)
		{
			if (seq)
			{
				bind.CharacterSoundsVolume.SetVolume(0f);
			}
			else
			{
				bind.CharacterSoundsVolume.SetVolume(1f);
			}
		}).AddTo(instance);
		Singleton<WorldRunner>.Instance.CurrentChunk.CombineLatest(Singleton<ChunkRunner>.Instance.TournamentChunk, (ChunkStruct chunk, List<int> chunks) => new
		{
			chunk,
			chunks
		}).Subscribe(tuple =>
		{
			bool value = false;
			for (int num = tuple.chunks.Count - 1; num >= 0; num--)
			{
				if (tuple.chunks[num] == tuple.chunk.Index)
				{
					value = true;
				}
				if (tuple.chunks[num] < tuple.chunk.Index)
				{
					tuple.chunks.Remove(tuple.chunks[num]);
				}
			}
			Singleton<ChunkRunner>.Instance.TournamentChunk.Value = tuple.chunks;
			ShouldPlayTournamentCheer.Value = value;
		}).AddTo(instance);
		ShouldPlayTournamentCheer.Subscribe(delegate(bool shouldPlay)
		{
			if (shouldPlay)
			{
				AudioController.Instance.QueueEvent(new AudioEvent("TournamentCheer", AUDIOEVENTACTION.Play));
			}
			else
			{
				AudioController.Instance.QueueEvent(new AudioEvent("TournamentCheer", AUDIOEVENTACTION.Stop));
			}
		}).AddTo(instance);
	}
}
