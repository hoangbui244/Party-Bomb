using System.Runtime.InteropServices;
using UnityEngine;
[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct SmeltFishing_Definition
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct LayerDefinition
	{
		public static readonly int DefaultLayer = LayerMask.NameToLayer("Character");
		private static readonly int P1Layer = LayerMask.NameToLayer("Collision_Obj_1");
		private static readonly int P2Layer = LayerMask.NameToLayer("Collision_Obj_2");
		private static readonly int P3Layer = LayerMask.NameToLayer("Collision_Obj_3");
		private static readonly int P4Layer = LayerMask.NameToLayer("Collision_Obj_4");
		public static readonly int[] Layers = new int[4]
		{
			P1Layer,
			P2Layer,
			P3Layer,
			P4Layer
		};
	}
	public enum ControlUser
	{
		Player1,
		Player2,
		Player3,
		Player4,
		Cpu1,
		Cpu2,
		Cpu3
	}
	public enum AIStrength
	{
		Easy,
		Normal,
		Hard
	}
	public static float[] ARROW_PROPER_DIFF_RANGE = new float[3]
	{
		115f,
		85f,
		45f
	};
	public static float[] NEXT_SPOT_SMELT_VALUE = new float[3]
	{
		0.9f,
		0.7f,
		0.5f
	};
	public static float[] NEXT_SPOT_PROBABILITY = new float[3]
	{
		0.5f,
		0.75f,
		0.9f
	};
	public static float[] SAME_SPOT_DIFF_NEXT_SPOT_VALUE = new float[3]
	{
		0.6f,
		0.4f,
		0.2f
	};
	public static float[] ARROW_PROPER_BRING_DIFF_VALUE = new float[3]
	{
		0.9f,
		0.8f,
		0.7f
	};
	public static float[] RANDOM_SELECT_ICE_PLATE_PROBABILITY = new float[3]
	{
		0.1f,
		0.5f,
		0.9f
	};
	public static float[] ROLL_UP_SMELT_VALUE = new float[3]
	{
		0.7f,
		0.6f,
		0.5f
	};
	public static int[] CAUGHT_SMELT_COUNT = new int[3]
	{
		2,
		3,
		4
	};
	public static float[] ROLL_UP_NOT_HIT_TIME = new float[3]
	{
		9f,
		7f,
		5f
	};
}
