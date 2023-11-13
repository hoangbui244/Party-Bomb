using GamepadInput;
using UnityEngine;
public class Golf_PlayerManager : SingletonCustom<Golf_PlayerManager>
{
	[Header("デバッグ：打つパワ\u30fcの割合を固定するかどうか")]
	public bool isDebugShotPowerLerp;
	[Header("デバッグ：打つパワ\u30fcの割合 (最小 : 0  最大 : 1)")]
	public float debugShotPowerLerp;
	[Header("デバッグ：打つインパクトのズレの割合を固定するかどうか")]
	public bool isDebugShotImpactLerp;
	[Header("デバッグ：打つインパクトのズレの割合 (最小 : 0  最大 : 1)")]
	public float debugShotImpactLerp;
	[SerializeField]
	[Header("プレイヤ\u30fc")]
	private Golf_Player[] arrayPlayer;
	private Golf_Player turnPlayer;
	[SerializeField]
	[Header("クラブのマテリアル")]
	private Material[] arrayClubMaterial;
	[SerializeField]
	[Header("ボ\u30fcルを打つ前の回転速度")]
	private float SHOT_READY_ROT_SPEED;
	[SerializeField]
	[Header("ボ\u30fcルを打つ前の制限角度")]
	private float READY_ROT_LIMIT_ANGLE;
	[SerializeField]
	[Header("ボ\u30fcルを打つ基本パワ\u30fc")]
	private float BASE_SHOT_POWER;
	[SerializeField]
	[Header("ボ\u30fcルを打つ時の最大のズレ")]
	private float MAX_SHOT_IMPACT_DIFF;
	[SerializeField]
	[Header("ボ\u30fcルを打つ時の角度")]
	private float SHOT_ANGLE;
	public void Init()
	{
		isDebugShotPowerLerp = false;
		isDebugShotImpactLerp = false;
		for (int i = 0; i < arrayPlayer.Length; i++)
		{
			int num = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[i][0];
			int num2 = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[num];
			arrayPlayer[i].Init(i, num);
			arrayPlayer[i].SetClubMaterial(arrayClubMaterial[num2]);
		}
		InitPlay();
	}
	public void InitPlay()
	{
		for (int i = 0; i < arrayPlayer.Length; i++)
		{
			if (i == turnPlayer.GetPlayerNo())
			{
				arrayPlayer[i].InitPlay();
			}
			else
			{
				arrayPlayer[i].SetAudience();
			}
		}
	}
	public void UpdateMethod()
	{
		if (SingletonCustom<Golf_GameManager>.Instance.GetIsSkip() || SingletonCustom<Golf_GameManager>.Instance.GetIsWaitUpdate())
		{
			return;
		}
		Golf_GameManager.State state = SingletonCustom<Golf_GameManager>.Instance.GetState();
		if ((uint)(state - 2) <= 2u)
		{
			if (turnPlayer.GetIsCpu() && SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.X))
			{
				SingletonCustom<Golf_GameManager>.Instance.SetIsSkip(_isSkip: true);
				SingletonCustom<Golf_UIManager>.Instance.StartScreenFade(delegate
				{
					turnPlayer.Skip();
				}, delegate
				{
					SingletonCustom<Golf_GameManager>.Instance.SetIsSkip(_isSkip: false);
				});
			}
			else
			{
				turnPlayer.UpdateMethod();
			}
		}
	}
	public Golf_Player GetPlayer(int _playerNo)
	{
		return arrayPlayer[_playerNo];
	}
	public Golf_Player GetTurnPlayer()
	{
		return turnPlayer;
	}
	public void SetTurnPlayer()
	{
		turnPlayer = arrayPlayer[SingletonCustom<Golf_GameManager>.Instance.GetTurnPlayerOrderOfPlay()];
	}
	public void ResetAnimation()
	{
		turnPlayer.ResetAnimation();
	}
	public float GetShotReadyRotSpeed()
	{
		return SHOT_READY_ROT_SPEED;
	}
	public float GetReadyRotLimitAngle()
	{
		return READY_ROT_LIMIT_ANGLE;
	}
	public float GetBaseShotPower()
	{
		return BASE_SHOT_POWER;
	}
	public float GetMaxShotImpactDiff()
	{
		return MAX_SHOT_IMPACT_DIFF;
	}
	public float GetShotAngle()
	{
		return SHOT_ANGLE;
	}
	public void GroupVibration()
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			if (!arrayPlayer[i].GetIsCpu())
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)arrayPlayer[i].GetUserType());
			}
		}
	}
}
