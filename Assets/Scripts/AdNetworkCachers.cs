using UniRx;

public class AdNetworkCachers : DisposableSingleton<AdNetworkCachers>
{
	private CompositeDisposable _disposable = new CompositeDisposable();

	public AdNetworkCachers()
	{
		add(new FacebookCacher());
		add(new AdMobCacher());
		add(new AdColonyCacher());
	}

	private void add(SingleNetworkCacher d)
	{
		d.Init();
		_disposable.Add(d);
	}

	public override void Dispose()
	{
		_disposable.Dispose();
	}
}
