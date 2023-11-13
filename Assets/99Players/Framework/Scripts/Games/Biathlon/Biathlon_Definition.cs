using System.Runtime.InteropServices;
using UnityEngine;
[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct Biathlon_Definition
{
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
		[InspectorName("弱い")]
		Easy,
		[InspectorName("普通")]
		Normal,
		[InspectorName("強い")]
		Hard
	}
	public static readonly float BronzeTime = 165f;
	public static readonly float SilverTime = 145f;
	public static readonly float GoldTime = 132f;
}
