using UniRx;

public class ARTutorialRunner : Singleton<ARTutorialRunner>
{
	public ARTutorialRunner()
	{
		if (PlayerData.Instance.ARTutorialStep.Value <= 1)
		{
			(from blocks in LevelEditorControls.Instance.BlocksPlaced.Pairwise()
				where blocks.Current >= 30 && blocks.Previous < 30
				select blocks).Subscribe(delegate
			{
				PlayerData.Instance.ARTutorialStep.Value = 1;
			});
			(from blocks in LevelEditorControls.Instance.BlocksPlaced.Pairwise()
				where blocks.Current < 30 && blocks.Previous >= 30
				select blocks).Subscribe(delegate
			{
				PlayerData.Instance.ARTutorialStep.Value = 0;
			});
		}
	}

	public void CompleteARTutorial()
	{
		PlayerData.Instance.ARTutorialStep.Value = 2;
	}
}
