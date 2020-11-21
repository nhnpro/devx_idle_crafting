using System;
using System.Collections.Generic;
using UnityEngine.Purchasing;

public class IAPTransactionState
{
	public IAPConfig Config
	{
		get;
		private set;
	}

	public Product Product
	{
		get;
		private set;
	}

	public string ReceiptRawData
	{
		get;
		private set;
	}

	public IAPTransactionState(IAPConfig cfg, Product prod, string receipt)
	{
		Config = cfg;
		Product = prod;
		ReceiptRawData = receipt;
	}

	public Dictionary<string, string> asDictionary()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("ProductID", Config.ProductID);
		try
		{
			dictionary.Add("State", "N/A");
			dictionary.Add("TransactionID", Product.transactionID);
			dictionary.Add("Price", Product.metadata.localizedPrice.ToString());
			dictionary.Add("CurrencyCode", Product.metadata.isoCurrencyCode);
			return dictionary;
		}
		catch (Exception ex)
		{
			dictionary.Add("Error", ex.Message);
			return dictionary;
		}
	}
}
