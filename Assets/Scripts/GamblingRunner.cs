using Big;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[PropertyClass]
public class GamblingRunner : Singleton<GamblingRunner>
{
	public ReactiveProperty<BigDouble[]> Rewards;

	[PropertyInt]
	public ReactiveProperty<int> FailCost;

	[PropertyBool]
	public ReactiveProperty<bool> FailPaid;

	public ReactiveProperty<int> FailAmount;

	[PropertyInt]
	public ReactiveProperty<int> ButtonOneState;

	[PropertyInt]
	public ReactiveProperty<int> ButtonTwoState;

	[PropertyInt]
	public ReactiveProperty<int> ButtonThreeState;

	[PropertyInt]
	public ReactiveProperty<int> ButtonFourState;

	[PropertyInt]
	public ReactiveProperty<int> CurrentGamblingLevel;

	public ReactiveProperty<RewardData[]> RewardCards = Observable.Never<RewardData[]>().ToReactiveProperty();

	[PropertyInt]
	public ReactiveProperty<int> NextJackpotLevel = new ReactiveProperty<int>(5);

	[PropertyInt]
	public ReactiveProperty<int> MaxGamblingLevel = new ReactiveProperty<int>(0);

	[PropertyBool]
	public ReadOnlyReactiveProperty<bool> IsJackpotLevel;

	private GamblingConfig m_gambConf;

	private ReactiveProperty<int>[] m_buttonStates = new ReactiveProperty<int>[4];

	private Animator[] m_buttonAnimators = new Animator[4];

	private Coroutine m_initialOpening;

	private bool m_claimedRewards;

	public GamblingRunner()
	{
		Singleton<PropertyManager>.Instance.AddRootContext(this);
		SceneLoader root = SceneLoader.Instance;
		GamblingBindingManager bind = GamblingBindingManager.Instance;
		LoadGamblingState();
		SetButtonStatesAndAnimatorsToArrays();
		MaxGamblingLevel.Value = Singleton<EconomyHelpers>.Instance.GetMaxGamblingLevel();
		CurrentGamblingLevel.Subscribe(delegate(int lvl)
		{
			m_gambConf = Singleton<EconomyHelpers>.Instance.GetGamblingConfig(lvl);
		}).AddTo(root);
		FailPaid.Subscribe(delegate(bool paid)
		{
			bind.DrJellyStealPopup.SetActive(!paid);
		}).AddTo(root);
		(from states in m_buttonStates[0].CombineLatest(m_buttonStates[1], m_buttonStates[2], m_buttonStates[3], (int one, int two, int three, int four) => new int[4]
			{
				one,
				two,
				three,
				four
			}).Skip(1)
			where states[0] > 0 || states[1] > 0 || states[2] > 0 || states[3] > 0
			select states).Subscribe(delegate
		{
			root.StartCoroutine(OpenRestCards());
		}).AddTo(root);
		(from curr in CurrentGamblingLevel
			where curr > NextJackpotLevel.Value
			select curr).Subscribe(delegate(int lvl)
		{
			NextJackpotLevel.SetValueAndForceNotify(Singleton<EconomyHelpers>.Instance.GetNextGamblingJackpotLevel(lvl));
		}).AddTo(root);
		CurrentGamblingLevel.Subscribe(delegate
		{
			root.StartCoroutine(CardSpawnAnimations());
		}).AddTo(root);
		IsJackpotLevel = (from isJP in CurrentGamblingLevel.CombineLatest(NextJackpotLevel, (int lvl, int jp) => lvl == jp)
			select (isJP)).TakeUntilDestroy(root).ToReadOnlyReactiveProperty();
		m_initialOpening = bind.StartCoroutine(OpenInitialCards());
	}

	private IEnumerator CardSpawnAnimations()
	{
		GamblingBindingManager bind = GamblingBindingManager.Instance;
		bind.CardDim.SetTrigger("CardDimExit");
		bind.CardButtons.GetComponent<Animator>().SetTrigger("HideCards");
		yield return new WaitForSeconds(1f);
		bind.Machine.SetTrigger("MachineOpen");
		bind.CardButtons.SetActive(value: false);
		yield return new WaitForSeconds(2f);
		bind.CardDim.SetTrigger("CardDimEnter");
		bind.CardButtons.SetActive(value: true);
	}

	private IEnumerator OpenRestCards()
	{
		if (m_initialOpening != null)
		{
			GamblingBindingManager.Instance.StopCoroutine(m_initialOpening);
			m_initialOpening = null;
		}
		yield return new WaitForSeconds(2.5f);
		for (int i = 0; i < 4; i++)
		{
			if (m_buttonStates[i].Value == -1)
			{
				m_buttonAnimators[i].SetTrigger("Capsule_OpenFast");
			}
			AudioController.Instance.QueueEvent(new AudioEvent("OpenCardFast", AUDIOEVENTACTION.Play));
		}
		yield return new WaitForSeconds(1.5f);
		if (CurrentGamblingLevel.Value < MaxGamblingLevel.Value)
		{
			GamblingBindingManager.Instance.ContinueButton.SetActive(value: true);
		}
		else
		{
			GamblingBindingManager.Instance.LastLevelReachedPopup.SetActive(value: true);
		}
	}

	private IEnumerator OpenInitialCards()
	{
		yield return new WaitForSeconds(2.5f);
		bool cont = false;
		for (int i = 0; i < 4; i++)
		{
			if (m_buttonStates[i].Value == 1 || m_buttonStates[i].Value == -2)
			{
				if (!m_buttonAnimators[i].gameObject.activeInHierarchy)
				{
					yield return new WaitForSeconds(2f);
				}
				m_buttonAnimators[i].SetTrigger("Capsule_OpenSlow");
				if (m_buttonStates[i].Value == 1)
				{
					cont = true;
				}
			}
		}
		yield return new WaitForSeconds(2.5f);
		int minuses = 0;
		for (int j = 0; j < 4; j++)
		{
			if (m_buttonStates[j].Value == -1)
			{
				m_buttonAnimators[j].SetTrigger("Capsule_OpenFast");
				minuses++;
			}
			if (m_buttonStates[j].Value == -2)
			{
				minuses++;
			}
		}
		if (minuses == 3)
		{
			cont = true;
		}
		yield return new WaitForSeconds(1.5f);
		if (cont && CurrentGamblingLevel.Value < MaxGamblingLevel.Value)
		{
			GamblingBindingManager.Instance.ContinueButton.SetActive(value: true);
		}
	}

	private void SetButtonStatesAndAnimatorsToArrays()
	{
		GamblingBindingManager instance = GamblingBindingManager.Instance;
		m_buttonStates[0] = ButtonOneState;
		m_buttonStates[1] = ButtonTwoState;
		m_buttonStates[2] = ButtonThreeState;
		m_buttonStates[3] = ButtonFourState;
		m_buttonAnimators[0] = instance.ButtonOne;
		m_buttonAnimators[1] = instance.ButtonTwo;
		m_buttonAnimators[2] = instance.ButtonThree;
		m_buttonAnimators[3] = instance.ButtonFour;
	}

	public void GambleForReward(int clickedCard)
	{
		bool flag = false;
		if (PlayerData.Instance.LifetimeGamblings.Value == 0 && CurrentGamblingLevel.Value < 10)
		{
			flag = true;
		}
		RewardData[] array = new RewardData[4];
		bool flag2 = false;
		for (int i = 0; i < array.Length; i++)
		{
			RewardData rewardData = m_gambConf.Rewards.AllotObject();
			float gamblingRewardVariation = PersistentSingleton<GameSettings>.Instance.GamblingRewardVariation;
			if (m_buttonStates[i].Value == -2)
			{
				array[i] = new RewardData(RewardEnum.Invalid, 0L);
				flag2 = true;
			}
			else if (rewardData.Amount < 2.14748365E+09f * (1f + gamblingRewardVariation))
			{
				int num = (int)((float)rewardData.Amount.ToInt() * Random.Range(1f, 1f + gamblingRewardVariation));
				array[i] = new RewardData(rewardData.Type, new BigDouble(num));
			}
			else
			{
				array[i] = rewardData;
			}
		}
		if (flag2 || m_gambConf.FailChance < Random.Range(0f, 1f) || flag)
		{
			AddReward(array[clickedCard]);
			if (m_gambConf.FailChance > 0f && !flag2)
			{
				List<int> list = new List<int>();
				for (int j = 0; j < 4; j++)
				{
					if (j != clickedCard)
					{
						list.Add(j);
					}
				}
				array[list[Random.Range(0, list.Count)]] = new RewardData(RewardEnum.Invalid, 0L);
			}
			for (int k = 0; k < 4; k++)
			{
				if (clickedCard != k && m_buttonStates[k].Value != -2)
				{
					m_buttonStates[k].Value = -1;
				}
			}
			m_buttonStates[clickedCard].Value = 1;
			m_buttonAnimators[clickedCard].SetTrigger("Capsule_OpenSlow");
		}
		else
		{
			FailPaid.Value = false;
			FailAmount.Value++;
			array[clickedCard] = new RewardData(RewardEnum.Invalid, 0L);
			m_buttonStates[clickedCard].Value = -2;
			m_buttonAnimators[clickedCard].SetTrigger("Capsule_OpenSlow");
			LockCards();
		}
		RewardCards.SetValueAndForceNotify(array);
		PersistentSingleton<MainSaver>.Instance.PleaseSave("gambling_clicked");
	}

	private void LockCards()
	{
		for (int i = 0; i < 4; i++)
		{
			if (m_buttonStates[i].Value != -2)
			{
				m_buttonStates[i].Value = -3;
			}
		}
	}

	private void OpenLockedCards()
	{
		for (int i = 0; i < 4; i++)
		{
			if (m_buttonStates[i].Value != -2)
			{
				m_buttonStates[i].Value = 0;
			}
		}
	}

	public void ContinueToNextLevel()
	{
		CurrentGamblingLevel.Value++;
		GamblingBindingManager.Instance.CardButtons.SetActive(value: false);
		for (int i = 0; i < 4; i++)
		{
			if (m_buttonStates[i].Value == 1)
			{
				m_buttonStates[i].Value = 0;
			}
		}
		for (int j = 0; j < 4; j++)
		{
			m_buttonStates[j].Value = 0;
		}
	}

	private void AddReward(RewardData reward)
	{
		BigDouble[] value = Rewards.Value;
		BigDouble[] array;
		int type;
		(array = value)[type = (int)reward.Type] = array[type] + reward.Amount;
		Rewards.SetValueAndForceNotify(value);
	}

	public void EmptyRewards()
	{
		BigDouble[] value = Rewards.Value;
		for (int i = 0; i < Rewards.Value.Length; i++)
		{
			value[i] = BigDouble.ZERO;
		}
		Rewards.SetValueAndForceNotify(value);
	}

	public void ClaimRewards()
	{
		m_claimedRewards = true;
		BigDouble[] value = Rewards.Value;
		for (int i = 0; i < value.Length; i++)
		{
			if (!(value[i] > BigDouble.ZERO))
			{
				continue;
			}
			RewardData reward = new RewardData((RewardEnum)i, value[i]);
			int numUnlockedHeroes = Singleton<EconomyHelpers>.Instance.GetNumUnlockedHeroes(PlayerData.Instance.LifetimeChunk.Value);
			switch (i)
			{
			case 29:
			{
				int[] array2 = new int[numUnlockedHeroes];
				for (int l = 0; l < value[i]; l++)
				{
					array2[Random.Range(1, numUnlockedHeroes)]++;
				}
				for (int m = 0; m < numUnlockedHeroes; m++)
				{
					if (array2[m] > 0)
					{
						reward = new RewardData(RewardEnum.AddToSpecificBerries, array2[m]);
						RewardAction rewardAction3 = RewardFactory.ToRewardAction(reward, RarityEnum.Common, numUnlockedHeroes, null, m);
						rewardAction3.GiveReward();
					}
				}
				break;
			}
			case 25:
			{
				List<RewardEnum> unlockedSkills = Singleton<EconomyHelpers>.Instance.GetUnlockedSkills();
				int[] array = new int[unlockedSkills.Count];
				for (int j = 0; j < value[i]; j++)
				{
					array[Random.Range(0, unlockedSkills.Count)]++;
				}
				for (int k = 0; k < unlockedSkills.Count; k++)
				{
					reward = new RewardData(unlockedSkills[k], array[k]);
					RewardAction rewardAction2 = RewardFactory.ToRewardAction(reward, RarityEnum.Common);
					rewardAction2.GiveReward();
				}
				break;
			}
			default:
			{
				RewardAction rewardAction = RewardFactory.ToRewardAction(reward, RarityEnum.Common);
				rewardAction.GiveReward();
				break;
			}
			}
		}
	}

	public void PayForFail()
	{
		if (PlayerData.Instance.Gems.Value < FailCost.Value)
		{
			NotEnoughGemsForBooster();
			GamblingBindingManager.Instance.NotEnoughGemsOverlay.SetActive(value: true);
			return;
		}
		Singleton<FundRunner>.Instance.RemoveGems(FailCost.Value, "gambling", "fail");
		FailCost.Value = Mathf.Min((int)(PersistentSingleton<GameSettings>.Instance.GamblingFailCostMultiplier * (float)FailCost.Value), PersistentSingleton<GameSettings>.Instance.MaximumGamblingFailCost);
		FailPaid.Value = true;
		OpenLockedCards();
		PersistentSingleton<MainSaver>.Instance.PleaseSave("gambling_fail_paid");
	}

	public void NotEnoughGemsForBooster()
	{
		int missingGems = FailCost.Value - PlayerData.Instance.Gems.Value;
		Singleton<NotEnoughGemsRunner>.Instance.NotEnoughGems(missingGems);
	}

	private void LoadGamblingState()
	{
		GamblingState value = PlayerData.Instance.Gambling.Value;
		if (value.CurrentGamblingLevel.Value < 0)
		{
			value.CurrentGamblingLevel.Value = 0;
		}
		Rewards = value.Rewards;
		FailCost = value.FailCost;
		FailPaid = value.FailPaid;
		FailAmount = value.FailAmount;
		ButtonOneState = value.ButtonOneState;
		ButtonTwoState = value.ButtonTwoState;
		ButtonThreeState = value.ButtonThreeState;
		ButtonFourState = value.ButtonFourState;
		CurrentGamblingLevel = value.CurrentGamblingLevel;
		RewardCards = value.RewardCards;
	}

	public void EmptyGamblingState()
	{
		PersistentSingleton<AnalyticsService>.Instance.FortunePodsResult.Value = new FortunePodResult(m_claimedRewards, FailAmount.Value, CurrentGamblingLevel.Value);
		PlayerData.Instance.LifetimeGamblings.Value++;
		PlayerData.Instance.Gambling.Value = new GamblingState();
	}
}
