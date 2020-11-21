using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class MiniGameRunner : MonoBehaviour
{
	private int attackInterval = 5;

	private int lastAttackSince;

	private List<int> frozenCompanions = new List<int>();

	[SerializeField]
	private DrJellyController djc;

	private void OnEnable()
	{
		frozenCompanions.Clear();
		lastAttackSince = 2;
		(from drActive in TickerService.MasterTicksSlow.TakeUntilDisable(this).CombineLatest(Singleton<BossBattleRunner>.Instance.BossBattleActive, (long tick, bool active) => active).CombineLatest(Singleton<DrJellyRunner>.Instance.DrJellyBattle, (bool active, bool dr) => active && dr)
			where drActive
			select drActive).Subscribe(delegate
		{
			lastAttackSince++;
			if (lastAttackSince >= attackInterval)
			{
				lastAttackSince = 0;
				if (Singleton<HeroUnlockRunner>.Instance.NumUnlockedHeroes.Value - 1 > frozenCompanions.Count)
				{
					StartCoroutine(Shoot());
				}
			}
		}).AddTo(this);
	}

	private IEnumerator Shoot()
	{
		djc.Shoot();
		yield return new WaitForSeconds(2.2f);
		int targets = Singleton<HeroUnlockRunner>.Instance.NumUnlockedHeroes.Value;
		int freezeAmount = UnityEngine.Random.Range(Math.Max(targets / 4, 1), Math.Max(targets / 2, 1) + 1);
		FreezeCompanions(freezeAmount);
	}

	private void FreezeCompanions(int amount)
	{
		List<int> list = new List<int>();
		for (int i = 1; i < Singleton<HeroUnlockRunner>.Instance.NumUnlockedHeroes.Value; i++)
		{
			if (!frozenCompanions.Contains(i))
			{
				list.Add(i);
			}
		}
		int count = list.Count;
		for (int j = 0; j < amount && j < count; j++)
		{
			int freezeIndex = list[UnityEngine.Random.Range(0, list.Count)];
			AbstractCreatureController orCreateCreature = Singleton<CreatureCollectionRunner>.Instance.GetOrCreateCreature(freezeIndex);
			orCreateCreature.Freeze();
			frozenCompanions.Add(freezeIndex);
			list.Remove(freezeIndex);
			(from index in Singleton<DrJellyRunner>.Instance.UnfrozenCompanionIndex.Skip(1)
				where index == freezeIndex
				select index).Take(1).Subscribe(delegate(int index)
			{
				frozenCompanions.Remove(index);
			}).AddTo(this);
		}
	}
}
