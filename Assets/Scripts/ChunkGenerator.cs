using System;
using System.Collections.Generic;
using UnityEngine;

public static class ChunkGenerator
{
	public static void GenerateFromPlaceholders(GameObject template, Transform container)
	{
		BlockPlaceholder[] componentsInChildren = template.GetComponentsInChildren<BlockPlaceholder>();
		BlockPlaceholder[] array = componentsInChildren;
		foreach (BlockPlaceholder blockPlaceholder in array)
		{
			GameObject orCreateGameObject = Singleton<EntityPoolManager>.Instance.GetOrCreateGameObject(GetPrefabPath(blockPlaceholder));
			BlockController component = orCreateGameObject.GetComponent<BlockController>();
			if (component != null)
			{
				component.Init(blockPlaceholder.Type);
			}
			BossBlockController component2 = orCreateGameObject.GetComponent<BossBlockController>();
			if (component2 != null)
			{
				component2.Init();
			}
			orCreateGameObject.transform.position = blockPlaceholder.transform.position;
			orCreateGameObject.transform.rotation = Quaternion.identity;
			if (orCreateGameObject.transform.parent == null)
			{
				orCreateGameObject.transform.SetParent(container, worldPositionStays: true);
			}
		}
	}

	public static void GenerateFromCustomLevel(List<JSONObject> blocks, Vector3 pos, Transform container)
	{
		int num = 0;
		for (int i = 0; i < blocks.Count; i++)
		{
			string text = blocks[i].asString("Type", () => string.Empty);
			int num2 = blocks[i].asInt("Size", () => 0);
			if (text == string.Empty || num2 == 0)
			{
				continue;
			}
			if (i == blocks.Count - 1 && num == 0)
			{
				text = "Gold";
			}
			GameObject orCreateGameObject = Singleton<EntityPoolManager>.Instance.GetOrCreateGameObject("Blocks/" + text + "Cube_" + num2 + "x" + num2);
			BlockController component = orCreateGameObject.GetComponent<BlockController>();
			BlockType blockType = (BlockType)Enum.Parse(typeof(BlockType), text);
			if (component != null)
			{
				component.Init(blockType);
				if (blockType == BlockType.Gold)
				{
					switch (num2)
					{
					case 1:
						num++;
						break;
					case 2:
						num += 9;
						break;
					case 4:
						num += 73;
						break;
					}
				}
			}
			orCreateGameObject.transform.position = pos + new Vector3(blocks[i].asFloat("X", () => 0f), blocks[i].asFloat("Y", () => 0f), blocks[i].asFloat("Z", () => 0f)) * 10f;
			orCreateGameObject.transform.rotation = Quaternion.identity;
			if (orCreateGameObject.transform.parent == null)
			{
				orCreateGameObject.transform.SetParent(container, worldPositionStays: true);
			}
		}
		Singleton<ChunkRunner>.Instance.BlocksInChunk.SetValueAndForceNotify(blocks.Count);
		Singleton<ChunkRunner>.Instance.GoldBlocks.SetValueAndForceNotify(num);
	}

	public static void GenerateFromConfig(Vector3 pos, int chunk, int prestige, int retryNumber, Transform container, bool bossFight)
	{
		int[] array = new int[13]
		{
			50,
			70,
			50,
			30,
			20,
			10,
			5,
			3,
			0,
			0,
			0,
			0,
			0
		};
		ChunkGeneratingConfig chunkGeneratingConfig = Singleton<EconomyHelpers>.Instance.GetChunkGeneratingConfig(chunk, bossFight);
		int num = chunkGeneratingConfig.MaxBlocks;
		if (!bossFight)
		{
			Singleton<ChunkRunner>.Instance.BlocksInChunk.SetValueAndForceNotify(num);
		}
		else
		{
			Singleton<ChunkRunner>.Instance.BlocksInChunk.SetValueAndForceNotify(num - 64);
		}
		int num2 = (int)((float)chunkGeneratingConfig.GoldBlockAverage * (PredictableRandom.GetNextRangeFloat(0f, 0.5f) + 0.75f));
		Singleton<ChunkRunner>.Instance.GoldBlocks.SetValueAndForceNotify(num2);
		PredictableRandom.SetSeed((uint)chunk, (uint)(prestige + retryNumber));
		List<BlockSpawnPoint> list = new List<BlockSpawnPoint>();
		int[] array2 = new int[5];
		List<WeightedObject<int[]>> list2 = new List<WeightedObject<int[]>>();
		if (bossFight)
		{
			list2.AddRange(PersistentSingleton<Economies>.Instance.ChunkMaps[0].ChunkMapClone());
			list2 = RemoveSpawnLocationsForBossFight(list2);
		}
		else
		{
			list2.AddRange(PersistentSingleton<Economies>.Instance.ChunkMaps[chunk % 5].ChunkMapClone());
		}
		int nextRangeInt = PredictableRandom.GetNextRangeInt(chunkGeneratingConfig.FourBlockMin, chunkGeneratingConfig.FourBlockMax + 1);
		for (int num3 = nextRangeInt; num3 > 0; num3--)
		{
			List<WeightedObject<int[]>> list3 = list2.FindAll((WeightedObject<int[]> x) => x.Value[3] >= 4);
			if (list3.Count <= 0)
			{
				break;
			}
			int nextRangeInt2 = PredictableRandom.GetNextRangeInt(0, list3.Count);
			int[] spawnSpot = list3[nextRangeInt2].Value;
			BlockType blockType = chunkGeneratingConfig.Materials.PredictableAllotObject();
			string prefabPath = GetPrefabPath(blockType, 4);
			if (list.Count <= 0 && bossFight)
			{
				spawnSpot = new int[5]
				{
					0,
					0,
					1,
					4,
					1
				};
				blockType = BlockType.Invalid;
				prefabPath = ((!Singleton<DrJellyRunner>.Instance.DrJellyBattle.Value) ? "Blocks/BigBossCube_4x4" : "Blocks/DrJelly");
			}
			Vector3 coordinates = new Vector3(spawnSpot[0], spawnSpot[1] + 2, spawnSpot[2]) + pos;
			list.Add(new BlockSpawnPoint(blockType, prefabPath, coordinates));
			int i;
			for (i = -4; i <= 3; i++)
			{
				int j;
				for (j = -4; j <= 3; j++)
				{
					int num4 = list2.FindIndex((WeightedObject<int[]> x) => x.Value[0] == spawnSpot[0] + i && x.Value[2] == spawnSpot[2] + j);
					if (num4 == -1)
					{
						continue;
					}
					WeightedObject<int[]> weightedObject = list2[num4];
					if (-2 <= i && i <= 1 && -2 <= j && j <= 1)
					{
						weightedObject.Value[1] += 4;
						weightedObject.Weight = array[weightedObject.Value[1]];
					}
					if (i == 0 && j == 0)
					{
						if (weightedObject.Value[1] > 4)
						{
							weightedObject.Value[3] = 2;
						}
					}
					else if (-1 <= i && i <= 1 && -1 <= j && j <= 1)
					{
						weightedObject.Value[3] = 2;
					}
					else if (-2 <= i && i <= 2 && -2 <= j && j <= 2)
					{
						weightedObject.Value[3] = 1;
					}
					else
					{
						if (weightedObject.Value[3] == 4)
						{
							weightedObject.Value[3] = 2;
						}
						if (-3 <= i && i <= 2 && -3 <= j && j <= 2)
						{
							weightedObject.Weight = array[1];
						}
						else
						{
							weightedObject.Weight = array[0];
						}
					}
					list2[num4] = weightedObject;
				}
			}
			num -= 64;
		}
		int nextRangeInt3 = PredictableRandom.GetNextRangeInt(chunkGeneratingConfig.TwoBlockMin, chunkGeneratingConfig.TwoBlockMax + 1);
		float num5 = (float)(nextRangeInt3 * 8) / (float)num;
		int num6 = Mathf.RoundToInt((float)num2 * num5 / 9f);
		for (int num7 = 0; num7 < nextRangeInt3; num7++)
		{
			array2[(int)chunkGeneratingConfig.Materials.PredictableAllotObject()]++;
		}
		int num8 = 0;
		if (PredictableRandom.GetNextRangeFloat(0f, 1f) < chunkGeneratingConfig.TNTChance)
		{
			num8 = PredictableRandom.GetNextRangeInt(chunkGeneratingConfig.TNTMin, chunkGeneratingConfig.TNTMax + 1);
		}
		nextRangeInt3 += num8;
		for (int num9 = nextRangeInt3; num9 > 0; num9--)
		{
			List<WeightedObject<int[]>> list4 = list2.FindAll((WeightedObject<int[]> x) => x.Value[3] >= 2);
			if (list4.Count <= 0)
			{
				break;
			}
			int[] spawnSpot2 = list4.PredictableAllotObject();
			BlockType blockType2 = BlockType.Gold;
			for (int num10 = array2.Length - 1; num10 >= 0; num10--)
			{
				if (num8 > 0)
				{
					blockType2 = BlockType.TNT;
					num8--;
					break;
				}
				if (num6 > 0)
				{
					blockType2 = BlockType.Gold;
					num6--;
					num2 -= 9;
					break;
				}
				if (array2[num10] > 0)
				{
					blockType2 = (BlockType)num10;
					array2[num10]--;
					break;
				}
			}
			string prefabPath2 = GetPrefabPath(blockType2, 2);
			Vector3 coordinates2 = new Vector3(spawnSpot2[0], spawnSpot2[1] + 1, spawnSpot2[2]) + pos;
			list.Add(new BlockSpawnPoint(blockType2, prefabPath2, coordinates2));
			int k;
			for (k = -2; k <= 1; k++)
			{
				int l;
				for (l = -2; l <= 1; l++)
				{
					int num11 = list2.FindIndex((WeightedObject<int[]> x) => x.Value[0] == spawnSpot2[0] + k && x.Value[2] == spawnSpot2[2] + l);
					if (num11 == -1)
					{
						continue;
					}
					WeightedObject<int[]> weightedObject2 = list2[num11];
					if (-1 <= k && k <= 0 && -1 <= l && l <= 0)
					{
						weightedObject2.Value[1] += 2;
					}
					if (-1 <= k && k <= 1 && -1 <= l && l <= 1)
					{
						if (weightedObject2.Value[1] >= 10)
						{
							weightedObject2.Value[3] = 0;
						}
						else
						{
							weightedObject2.Value[3] = 1;
							if (k == 0 && l == 0)
							{
								weightedObject2.Value[3] = 2;
							}
						}
					}
					weightedObject2.Weight = array[weightedObject2.Value[1]];
					list2[num11] = weightedObject2;
				}
			}
			num -= 8;
		}
		int num12 = 0;
		if (PredictableRandom.GetNextRangeFloat(0f, 1f) < chunkGeneratingConfig.DiamondChance)
		{
			num12 = PredictableRandom.GetNextRangeInt(chunkGeneratingConfig.DiamondMin, chunkGeneratingConfig.DiamondMax + 1);
		}
		for (int num13 = 0; num13 < num - num12 - num2; num13++)
		{
			array2[(int)chunkGeneratingConfig.Materials.PredictableAllotObject()]++;
		}
		List<WeightedObject<int[]>> list5 = list2.FindAll((WeightedObject<int[]> x) => x.Value[3] >= 1);
		for (int num14 = num; num14 > 0; num14--)
		{
			if (list5.Count > 0)
			{
				int[] spawnSpot3 = list5.PredictableAllotObject();
				BlockType type = BlockType.Diamond;
				for (int num15 = array2.Length - 1; num15 >= 0; num15--)
				{
					if (num12 > 0)
					{
						type = BlockType.Diamond;
						num12--;
						break;
					}
					if (num2 > 0)
					{
						type = BlockType.Gold;
						num2--;
						break;
					}
					if (array2[num15] > 0)
					{
						type = (BlockType)num15;
						array2[num15]--;
						break;
					}
				}
				Vector3 coordinates3 = new Vector3((float)spawnSpot3[0] + 0.5f, (float)spawnSpot3[1] + 0.5f, (float)spawnSpot3[2] + 0.5f) + pos;
				list.Add(new BlockSpawnPoint(type, string.Empty, coordinates3));
				int m;
				for (m = -1; m <= 1; m++)
				{
					int n;
					for (n = -1; n <= 1; n++)
					{
						WeightedObject<int[]> weightedObject3 = list5.Find((WeightedObject<int[]> x) => x.Value[0] == spawnSpot3[0] + m && x.Value[2] == spawnSpot3[2] + n);
						if (weightedObject3 != null)
						{
							if (m == 0 && n == 0)
							{
								weightedObject3.Value[1]++;
							}
							if (weightedObject3.Value[1] >= 10)
							{
								list5.Remove(weightedObject3);
							}
							else
							{
								weightedObject3.Weight = array[weightedObject3.Value[1]];
							}
						}
					}
				}
			}
		}
		List<BlockSpawnPoint> blocks = list.FindAll((BlockSpawnPoint x) => x.PrefabPath == string.Empty);
		for (int num16 = -5; num16 <= 4; num16++)
		{
			for (int num17 = -7; num17 <= 8; num17++)
			{
				List<BlockSpawnPoint> blocksNearCoordinates = GetBlocksNearCoordinates(blocks, (float)num16 + 0.5f + pos.x, (float)num17 + 0.5f + pos.z);
				BlockType blockType3 = BlockType.Invalid;
				if (blocksNearCoordinates.Count <= 0)
				{
					continue;
				}
				for (int num18 = blocksNearCoordinates.Count - 1; num18 >= 0; num18--)
				{
					string empty = string.Empty;
					BlockType num19 = blockType3;
					BlockSpawnPoint blockSpawnPoint = blocksNearCoordinates[num18];
					if (num19 != blockSpawnPoint.Type)
					{
						BlockSpawnPoint blockSpawnPoint2 = blocksNearCoordinates[num18];
						empty = GetPrefabPath(blockSpawnPoint2.Type, 1, BlockAlignment.Top);
					}
					else if (num18 - 1 >= 0)
					{
						BlockSpawnPoint blockSpawnPoint3 = blocksNearCoordinates[num18];
						BlockType type2 = blockSpawnPoint3.Type;
						BlockSpawnPoint blockSpawnPoint4 = blocksNearCoordinates[num18 - 1];
						if (type2 == blockSpawnPoint4.Type)
						{
							BlockSpawnPoint blockSpawnPoint5 = blocksNearCoordinates[num18];
							empty = GetPrefabPath(blockSpawnPoint5.Type, 1, BlockAlignment.Middle);
						}
						else
						{
							BlockSpawnPoint blockSpawnPoint6 = blocksNearCoordinates[num18];
							empty = GetPrefabPath(blockSpawnPoint6.Type, 1, BlockAlignment.Bottom);
						}
					}
					else
					{
						BlockSpawnPoint blockSpawnPoint7 = blocksNearCoordinates[num18];
						empty = GetPrefabPath(blockSpawnPoint7.Type, 1, BlockAlignment.Bottom);
					}
					BlockSpawnPoint blockSpawnPoint8 = blocksNearCoordinates[num18];
					blockType3 = blockSpawnPoint8.Type;
					List<BlockSpawnPoint> list6 = list;
					int index = list.IndexOf(blocksNearCoordinates[num18]);
					BlockSpawnPoint blockSpawnPoint9 = blocksNearCoordinates[num18];
					BlockType type3 = blockSpawnPoint9.Type;
					string prefabPath3 = empty;
					BlockSpawnPoint blockSpawnPoint10 = blocksNearCoordinates[num18];
					list6[index] = new BlockSpawnPoint(type3, prefabPath3, blockSpawnPoint10.Coordinates);
				}
			}
		}
		for (int num20 = 0; num20 < list.Count; num20++)
		{
			EntityPoolManager instance = Singleton<EntityPoolManager>.Instance;
			BlockSpawnPoint blockSpawnPoint11 = list[num20];
			GameObject orCreateGameObject = instance.GetOrCreateGameObject(blockSpawnPoint11.PrefabPath);
			BlockController component = orCreateGameObject.GetComponent<BlockController>();
			if (component != null)
			{
				BlockController blockController = component;
				BlockSpawnPoint blockSpawnPoint12 = list[num20];
				blockController.Init(blockSpawnPoint12.Type);
			}
			else
			{
				BossBlockController component2 = orCreateGameObject.GetComponent<BossBlockController>();
				if (component2 != null)
				{
					component2.Init();
				}
			}
			Transform transform = orCreateGameObject.transform;
			BlockSpawnPoint blockSpawnPoint13 = list[num20];
			transform.position = blockSpawnPoint13.Coordinates;
			orCreateGameObject.transform.rotation = Quaternion.identity;
			if (orCreateGameObject.transform.parent == null)
			{
				orCreateGameObject.transform.SetParent(container, worldPositionStays: true);
			}
		}
	}

	private static List<WeightedObject<int[]>> RemoveSpawnLocationsForBossFight(List<WeightedObject<int[]>> spawnlocations)
	{
		int i;
		for (i = -5; i <= 1; i++)
		{
			int j;
			for (j = 3; j <= 8; j++)
			{
				spawnlocations.Remove(spawnlocations.Find((WeightedObject<int[]> x) => x.Value[0] == i && x.Value[2] == j));
			}
		}
		spawnlocations.Remove(spawnlocations.Find((WeightedObject<int[]> x) => x.Value[0] == -5 && x.Value[2] == 2));
		spawnlocations.Remove(spawnlocations.Find((WeightedObject<int[]> x) => x.Value[0] == -4 && x.Value[2] == 2));
		spawnlocations.Remove(spawnlocations.Find((WeightedObject<int[]> x) => x.Value[0] == -3 && x.Value[2] == 2));
		spawnlocations.Remove(spawnlocations.Find((WeightedObject<int[]> x) => x.Value[0] == -3 && x.Value[2] == 1));
		spawnlocations.Remove(spawnlocations.Find((WeightedObject<int[]> x) => x.Value[0] == -3 && x.Value[2] == 0));
		return spawnlocations;
	}

	private static List<BlockSpawnPoint> GetBlocksNearCoordinates(List<BlockSpawnPoint> blocks, float x, float z)
	{
		return blocks.FindAll((BlockSpawnPoint block) => block.Coordinates.x <= x + float.Epsilon && block.Coordinates.x >= x - float.Epsilon && block.Coordinates.z <= z + float.Epsilon && block.Coordinates.z >= z - float.Epsilon);
	}

	public static string GetPrefabPath(BlockType block, int blockSize)
	{
		return "Blocks/" + block.ToString() + "Cube_" + blockSize + "x" + blockSize;
	}

	public static string GetPrefabPath(BlockType block, int blockSize, BlockAlignment blockAlignment)
	{
		string text = string.Empty;
		switch (block)
		{
		case BlockType.Grass:
		case BlockType.Dirt:
		case BlockType.Stone:
		case BlockType.Gold:
			text = "_" + blockAlignment;
			break;
		}
		return "Blocks/" + block.ToString() + "Cube_" + blockSize + "x" + blockSize + text;
	}

	public static string GetPrefabPath(BlockPlaceholder block)
	{
		string text = string.Empty;
		BlockAlignment alignment = block.Alignment;
		if (alignment == BlockAlignment.Top || alignment == BlockAlignment.Middle || alignment == BlockAlignment.Bottom)
		{
			text = "_" + block.Alignment;
		}
		return "Blocks/" + block.Type.ToString() + "Cube_" + block.Width + "x" + block.Height + text;
	}

	public static void GenerateVegetation(Vector3 pos, Transform container)
	{
		for (int i = 0; i < 32; i++)
		{
			Vector3 vector = pos + new Vector3(UnityEngine.Random.Range(-8, 8), 0f, UnityEngine.Random.Range(-12, 12));
			Ray ray = new Ray(vector + new Vector3(-0.1f, 10.1f, 0.1f), Vector3.down);
			if (!Physics.Raycast(ray, 10f))
			{
				GameObject orCreateGameObject = Singleton<EntityPoolManager>.Instance.GetOrCreateGameObject("Environment/Clover" + UnityEngine.Random.Range(0, 4));
				orCreateGameObject.transform.position = vector;
				orCreateGameObject.transform.rotation = Quaternion.identity;
				if (orCreateGameObject.transform.parent == null)
				{
					orCreateGameObject.transform.SetParent(container, worldPositionStays: true);
				}
			}
		}
	}
}
