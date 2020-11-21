public class FacebookGift
{
	public string FromId
	{
		get;
		private set;
	}

	public string GiftId
	{
		get;
		private set;
	}

	public FacebookGift(string fromId, string giftId)
	{
		FromId = fromId;
		GiftId = giftId;
	}
}
