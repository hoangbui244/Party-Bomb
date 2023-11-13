using UnityEngine;
public class SpearBattle_AnimEvent : MonoBehaviour
{
	public void Phase_1()
	{
		SingletonCustom<SpearBattle_BattleManager>.Instance.PhaseBattle();
		SingletonCustom<SpearBattle_BattleManager>.Instance.ScoreEndCheck();
	}
	public void Phase_2()
	{
		SingletonCustom<SpearBattle_BattleManager>.Instance.PhaseBattle();
		SingletonCustom<SpearBattle_BattleManager>.Instance.ScoreEndCheck();
	}
	public void Phase_3()
	{
		SingletonCustom<SpearBattle_BattleManager>.Instance.PhaseBattle();
		SingletonCustom<SpearBattle_BattleManager>.Instance.ScoreEndCheck();
	}
	public void Phase_4()
	{
		SingletonCustom<SpearBattle_BattleManager>.Instance.PhaseBattle();
		SingletonCustom<SpearBattle_BattleManager>.Instance.ScoreEndCheck();
	}
	public void Phase_5()
	{
		SingletonCustom<SpearBattle_BattleManager>.Instance.PhaseBattle();
		SingletonCustom<SpearBattle_BattleManager>.Instance.ScoreEndCheck();
	}
	public void Phase_End()
	{
		SingletonCustom<SpearBattle_BattleManager>.Instance.BattleEnd(_isSkip: false);
	}
	public void CameraChangeDuel()
	{
		SingletonCustom<SpearBattle_BattleManager>.Instance.CameraChange(_isDuel: true);
	}
	public void CameraChangeChara()
	{
		if (!SingletonCustom<SpearBattle_BattleManager>.Instance.IsBattleEnd)
		{
			SingletonCustom<SpearBattle_BattleManager>.Instance.CameraChange(_isDuel: false);
			SingletonCustom<SpearBattle_BattleManager>.Instance.CharaStayDirection();
		}
	}
	public void CharaAnimationStart()
	{
		SingletonCustom<SpearBattle_BattleManager>.Instance.CharaAnimDirection();
	}
	public void SelectStart()
	{
		SingletonCustom<SpearBattle_BattleManager>.Instance.SelectStart();
	}
	public void HorseStay()
	{
		SingletonCustom<SpearBattle_BattleManager>.Instance.HorseStay();
	}
}
