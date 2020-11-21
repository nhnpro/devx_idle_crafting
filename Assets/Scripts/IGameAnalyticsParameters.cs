using System.Collections.Generic;

public interface IGameAnalyticsParameters
{
	void AddFirebaseParameters(Dictionary<string, string> p);

	void AddFacebookParameters(Dictionary<string, object> p);
}
