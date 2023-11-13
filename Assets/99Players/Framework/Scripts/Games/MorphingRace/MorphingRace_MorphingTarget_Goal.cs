using UnityEngine;
public class MorphingRace_MorphingTarget_Goal : MorphingRace_MorphingTarget
{
	[SerializeField]
	[Header("ゴ\u30fcル後に移動させるアンカ\u30fc")]
	private Transform[] arrayAfterGoalMoveAnchor;
	public Transform GetAfterGoalMoveAnchor(int _playerNo)
	{
		return arrayAfterGoalMoveAnchor[_playerNo];
	}
}
