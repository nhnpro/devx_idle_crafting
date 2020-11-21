using System;
using System.Collections.Generic;

public class PlayFabException : Exception
{
	public PlayFabError error;

	public string errorName;

	private static Dictionary<int, PlayFabError> ErrorCodeMapping = new Dictionary<int, PlayFabError>
	{
		{
			1001,
			PlayFabError.ACCOUNT_NOT_FOUND
		},
		{
			1014,
			PlayFabError.ACCOUNT_NOT_LINKED
		},
		{
			1074,
			PlayFabError.NOT_AUTHENTICATED
		},
		{
			1011,
			PlayFabError.ACCOUNT_ALREADY_LINKED
		},
		{
			1012,
			PlayFabError.LINKED_ACCOUNT_ALREADY_CLAIMED
		},
		{
			1009,
			PlayFabError.NICKNAME_NOT_AVAILABLE
		},
		{
			1234,
			PlayFabError.PROFANE_DISPLAY_NAME
		},
		{
			1059,
			PlayFabError.INSUFFICIENT_FUNDS
		}
	};

	public PlayFabException(PlayFabError error, string errorName = "", string errorMessage = "")
		: base(errorMessage)
	{
		this.error = error;
		this.errorName = errorName;
	}

	public PlayFabException(int errorCode, string errorName, string errorMessage = "")
		: base(errorMessage)
	{
		error = ((!ErrorCodeMapping.ContainsKey(errorCode)) ? PlayFabError.UNKNOWN : ErrorCodeMapping[errorCode]);
		this.errorName = errorName;
	}

	public override string ToString()
	{
		return error.ToString() + "-" + errorName + "-" + Message;
	}
}
