public static class GearStateFactory
{
	public static GearState GetOrCreateGearState(int gear)
	{
		PlayerData.Instance.GearStates.EnsureSize(gear, (int _) => new GearState());
		return PlayerData.Instance.GearStates[gear];
	}
}
