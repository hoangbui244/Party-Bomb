using System.Runtime.InteropServices;
using UnityEngine;
[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct Shuriken_Definition
{
	public enum ControlUser
	{
		Player1,
		Player2,
		Player3,
		Player4,
		Cpu1,
		Cpu2,
		Cpu3,
		AutoPlay
	}
	public enum AIStrength
	{
		Easy,
		Normal,
		Hard
	}
	public enum GameState
	{
		NotInitialized,
		Initialized,
		DuringGame,
		GameEnd,
		GameResult
	}
	public const float GameTime = 60f;
	public static readonly Vector2[] SinglePlayerReticleDefaultPositions = new Vector2[4]
	{
		new Vector2(960f, 486f),
		new Vector2(480f, 486f),
		new Vector2(960f, 486f),
		new Vector2(1440f, 486f)
	};
	public static readonly Vector2[] TwoPlayersReticleDefaultPositions = new Vector2[4]
	{
		new Vector2(480f, 486f),
		new Vector2(1440f, 486f),
		new Vector2(480f, 540f),
		new Vector2(1440f, 540f)
	};
	public static readonly Vector2[] ThreePlayersReticleDefaultPositions = new Vector2[4]
	{
		new Vector2(480f, 486f),
		new Vector2(960f, 486f),
		new Vector2(1440f, 486f),
		new Vector2(960f, 486f)
	};
	public static readonly Vector2[] FourPlayersReticleDefaultPositions = new Vector2[4]
	{
		new Vector2(392f, 486f),
		new Vector2(784f, 486f),
		new Vector2(1176f, 486f),
		new Vector2(1568f, 486f)
	};
	public static readonly float BronzeScore = 3000f;
	public static readonly float SilverScore = 4000f;
	public static readonly float GoldScore = 5000f;
}
