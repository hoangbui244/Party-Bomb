using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_PlayerConfig : DecoratedScriptableObject
{
	[SerializeField]
	[DisplayName("無敵時間")]
	private float godTimeWhenAfterMiss = 2f;
	[SerializeField]
	[DisplayName("ゴ\u30fcル時の加算スコア")]
	private int[] goalScores = new int[4]
	{
		1500,
		1000,
		500,
		250
	};
	public float GodTimeWhenAfterMiss => godTimeWhenAfterMiss;
	public int[] GoalScore => goalScores;
}
