using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct EditStep
{
	public GameObject Block
	{
		get;
		private set;
	}

	public bool Activate
	{
		get;
		private set;
	}

	public EditStep(GameObject block, bool activate)
	{
		Block = block;
		Activate = activate;
	}
}
