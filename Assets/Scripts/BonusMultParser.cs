public static class BonusMultParser
{
	public static BonusMultConfig ParseBonusMultConfig(string str)
	{
		string[] row = str.Split('X');
		BonusMultConfig bonusMultConfig = new BonusMultConfig();
		bonusMultConfig.BonusType = row.asEnum(0, row.toError<BonusTypeEnum>());
		bonusMultConfig.Amount = row.asFloat(1, row.toError<float>());
		return bonusMultConfig;
	}
}
