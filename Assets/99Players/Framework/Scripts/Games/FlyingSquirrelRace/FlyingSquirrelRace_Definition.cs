using System.Runtime.InteropServices;
[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct FlyingSquirrelRace_Definition
{
	public enum Controller
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
	public enum GameState
	{
		NotInitialized,
		Initialized,
		DuringGame,
		GameEnd,
		GameResult
	}
}
