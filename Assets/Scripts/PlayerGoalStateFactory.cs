public static class PlayerGoalStateFactory
{
	public static PlayerGoalState GetOrCreatePlayerGoalState(string id)
	{
		PlayerGoalState playerGoalState = PlayerData.Instance.PlayerGoalStates.Find((PlayerGoalState pgs) => pgs.ID == id);
		if (playerGoalState == null)
		{
			PlayerGoalState playerGoalState2 = new PlayerGoalState();
			playerGoalState2.ID = id;
			playerGoalState = playerGoalState2;
			PlayerData.Instance.PlayerGoalStates.Add(playerGoalState);
		}
		return playerGoalState;
	}
}
