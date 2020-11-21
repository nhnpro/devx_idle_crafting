using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace UniRx
{
	public class WWWErrorException : Exception
	{
		public string RawErrorMessage
		{
			get;
			private set;
		}

		public bool HasResponse
		{
			get;
			private set;
		}

		public string Text
		{
			get;
			private set;
		}

		public HttpStatusCode StatusCode
		{
			get;
			private set;
		}

		public Dictionary<string, string> ResponseHeaders
		{
			get;
			private set;
		}

		public WWW WWW
		{
			get;
			private set;
		}

		public WWWErrorException(WWW www, string text)
		{
			WWW = www;
			RawErrorMessage = www.error;
			ResponseHeaders = www.responseHeaders;
			HasResponse = false;
			Text = text;
			string[] array = RawErrorMessage.Split(' ', ':');
			if (array.Length != 0 && int.TryParse(array[0], out int result))
			{
				HasResponse = true;
				StatusCode = (HttpStatusCode)result;
			}
		}

		public override string ToString()
		{
			string text = Text;
			if (string.IsNullOrEmpty(text))
			{
				return RawErrorMessage;
			}
			return RawErrorMessage + " " + text;
		}
	}
}
