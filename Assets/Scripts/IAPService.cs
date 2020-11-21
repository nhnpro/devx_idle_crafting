using System;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

public class IAPService : PersistentSingleton<IAPService>, IStoreListener
{
	public ReactiveProperty<bool> DataFetched = new ReactiveProperty<bool>(initialValue: false);

	public ReactiveProperty<IAPNotCompleted> IAPNotCompleted = Observable.Never<IAPNotCompleted>().ToReactiveProperty();

	public ReactiveProperty<IAPTransactionState> IAPCompleted = Observable.Never<IAPTransactionState>().ToReactiveProperty();

	public ReactiveProperty<IAPTransactionState> IAPValidated = Observable.Never<IAPTransactionState>().ToReactiveProperty();

	public ReactiveProperty<bool> IAPInvalidated = Observable.Never<bool>().ToReactiveProperty();

	public ReactiveProperty<bool> PurchaseInProgress = Observable.Never<bool>().ToReactiveProperty();

	private IAPConfig m_currentlyActiveIAP;

	private IStoreController m_storeController;

	private IExtensionProvider m_extensionProvider;

	public void InitializePurchasing()
	{
		if (!base.Inited)
		{
			StandardPurchasingModule module = StandardPurchasingModule.Instance();
			(from avail in ConnectivityService.InternetConnectionAvailable
				where avail
				select avail).Take(1).Subscribe(delegate
			{
				InitIAPs(module);
			});
			base.Inited = true;
		}
	}

	private void InitIAPs(StandardPurchasingModule module)
	{
		ConfigurationBuilder configurationBuilder = ConfigurationBuilder.Instance(module);
		foreach (IAPConfig iAP in PersistentSingleton<Economies>.Instance.IAPs)
		{
			configurationBuilder.AddProduct(iAP.ProductID, (iAP.Type == IAPType.BundleDurable) ? ProductType.NonConsumable : ProductType.Consumable);
		}
		// configurationBuilder.Configure<IGooglePlayConfiguration>().SetPublicKey("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAjAYZVERoP4E4HwFAtzZt3JbsfNsiJ+MihJ9bSMjStq0zFo6gXUMc0Ubl4TCgw69UYMt7pR+2ghbTdnR0UvJ/CKhLYpqHhZMp9nQUmGZS+L9Yyl6gAn7Z4xN3kdoVh0fIg8nu0rn3/7hQoUO+7yjZ8usAWubnxRbg4cLL1r5UFwPR+lE7r2rFPdbP3YdFodUfxGdRQVZNFZtZHb4dAW7bZP6sK/UncD49hYkfonkER9cYwJkFpEkXMIl/mxN8I8Vrdro11PSTfhNO9PxqabjZZwPICxSO1pYYPpFehONESezhIf8XPvJ/qMn3PlF/J14IsGOAFOzN3ul3xf67euRMHQIDAQAB");
		UnityPurchasing.Initialize(this, configurationBuilder);
		(from iap in IAPCompleted
			where iap.Config.Type == IAPType.BundleDurable
			select iap.Config.ProductID).Subscribe(delegate(string id)
		{
			PlayerData.Instance.PurchasedIAPBundleIDs.Add(id);
			PersistentSingleton<MainSaver>.Instance.PleaseSave("iap_bundle");
		});
	}

	public void InitiatePurchase(IAPConfig cfg)
	{
		PersistentSingleton<MainSaver>.Instance.PleaseSave("iap_started_" + cfg.ProductEnum);
		if (m_storeController == null)
		{
			IAPNotCompleted.Value = new IAPNotCompleted(cfg, "Failed", "UnityIAPs not initialized.");
			return;
		}
		m_currentlyActiveIAP = cfg;
		PurchaseInProgress.SetValueAndForceNotify(value: true);
		m_storeController.InitiatePurchase(cfg.ProductID);
	}

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		m_storeController = controller;
		m_extensionProvider = extensions;
		m_extensionProvider.GetExtension<IAppleExtensions>().RegisterPurchaseDeferredListener(OnDeferred);
		Product[] all = controller.products.all;
		foreach (Product product in all)
		{
			IAPConfig iAPConfig = PersistentSingleton<Economies>.Instance.IAPs.Find((IAPConfig iap) => iap.ProductID == product.definition.storeSpecificId);
			if (iAPConfig != null && product.availableToPurchase)
			{
				iAPConfig.StoreProduct = product.metadata;
			}
			else
			{
				UnityEngine.Debug.LogWarning("Product ID : " + product.definition.storeSpecificId + " does not exist in our configuration.");
			}
		}
		DataFetched.Value = true;
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		UnityEngine.Debug.LogWarning("IAPService.OnInitializeFailed Billing failed to initialize!");
		switch (error)
		{
		case InitializationFailureReason.AppNotKnown:
			UnityEngine.Debug.LogWarning("Is your App correctly uploaded on the relevant publisher console?");
			break;
		case InitializationFailureReason.PurchasingUnavailable:
			UnityEngine.Debug.LogWarning("Billing disabled!");
			break;
		case InitializationFailureReason.NoProductsAvailable:
			UnityEngine.Debug.LogWarning("No products available for purchase!");
			break;
		}
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs evt)
	{
		try
		{
			bool flag = false;
			// CrossPlatformValidator crossPlatformValidator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
			CrossPlatformValidator crossPlatformValidator = new CrossPlatformValidator(null, null, Application.identifier);
			IPurchaseReceipt[] array = crossPlatformValidator.Validate(evt.purchasedProduct.receipt);
			for (int i = 0; i < array.Count(); i++)
			{
				IPurchaseReceipt productReceipt = array[i];
				if (productReceipt.productID == evt.purchasedProduct.definition.storeSpecificId && PersistentSingleton<Economies>.Instance.IAPs.Find((IAPConfig iap) => iap.ProductID == productReceipt.productID) != null)
				{
					flag = true;
				}
			}
			IAPConfig iAPConfig = PersistentSingleton<Economies>.Instance.IAPs.Find((IAPConfig iap) => iap.ProductID == evt.purchasedProduct.definition.storeSpecificId);
			if (flag && iAPConfig != null)
			{
				IAPValidated.Value = new IAPTransactionState(iAPConfig, evt.purchasedProduct, evt.purchasedProduct.receipt);
			}
			if (!flag)
			{
				IAPInvalidated.SetValueAndForceNotify(value: true);
			}
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogWarning("Invalid receipt - " + ex.Message);
		}
		PurchaseInProgress.SetValueAndForceNotify(value: false);
		GiveIAPToUser(evt.purchasedProduct);
		return PurchaseProcessingResult.Complete;
	}

	private void GiveIAPToUser(Product prod)
	{
		try
		{
			IAPConfig iAPConfig = PersistentSingleton<Economies>.Instance.IAPs.Find((IAPConfig iap) => iap.ProductID == prod.definition.storeSpecificId);
			if (iAPConfig != null && (iAPConfig.Type != IAPType.BundleDurable || !PlayerData.Instance.PurchasedIAPBundleIDs.Contains(iAPConfig.ProductID)))
			{
				IAPCompleted.Value = new IAPTransactionState(iAPConfig, prod, null);
				PersistentSingleton<MainSaver>.Instance.PleaseSave("iap_completed_" + iAPConfig.ProductEnum);
			}
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogError("IAPService Exception: " + ex.Message);
		}
	}

	public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
	{
		PurchaseInProgress.SetValueAndForceNotify(value: false);
		switch (p)
		{
		case PurchaseFailureReason.DuplicateTransaction:
			GiveIAPToUser(i);
			break;
		case PurchaseFailureReason.UserCancelled:
			IAPNotCompleted.Value = new IAPNotCompleted(m_currentlyActiveIAP, "Cancelled", p.ToString());
			break;
		default:
			IAPNotCompleted.Value = new IAPNotCompleted(m_currentlyActiveIAP, "Failed", p.ToString());
			break;
		}
	}

	private void OnDeferred(Product item)
	{
	}

	private void OnTransactionsRestored(bool success)
	{
		PurchaseInProgress.SetValueAndForceNotify(value: false);
	}

	public void RestorePurchases()
	{
		PurchaseInProgress.SetValueAndForceNotify(value: true);
		m_extensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions(OnTransactionsRestored);
	}

	public void UnpublishPurchases()
	{
		IAPNotCompleted.UnpublishValue();
		IAPCompleted.UnpublishValue();
		IAPValidated.UnpublishValue();
	}
}
