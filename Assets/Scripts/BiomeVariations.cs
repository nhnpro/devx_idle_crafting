using System;
using UnityEngine;

[Serializable]
public class BiomeVariations
{
	[SerializeField]
	public GameObject[] StartBiomes;

	[SerializeField]
	public GameObject[] InsertBiomes;

	[SerializeField]
	public GameObject[] TournamentBiomes;

	[SerializeField]
	public GameObject[] MapNodeProfile;

	[SerializeField]
	public string BiomeAudio;

	[SerializeField]
	public GameObject Intro;
}
