using GamepadInput;
using System;
using UnityEngine;
public class Shooting_Controller : MonoBehaviour
{
	private int gunNo;
	private int playerNo;
	[SerializeField]
	[Header("回転軸のアンカ\u30fc")]
	private Transform anchor;
	[SerializeField]
	[Header("発射ポイント")]
	private Transform shotPoint;
	[SerializeField]
	[Header("発射する弾")]
	private Shooting_Bullet bullet;
	[SerializeField]
	[Header("装填する弾")]
	private GameObject loadingBullet;
	[SerializeField]
	[Header("色を変更するモデル")]
	private MeshRenderer[] arrayMeshRenderer;
	private bool isShot;
	private int comboCount;
	private int remainingBulletNum;
	private bool useSingleCamera;
	private Shooting_Target aiTarget;
	private Vector3 aiTargetUiPos;
	private float aiAimOkTime;
	private float aiRayHittingTime;
	private float aiCursorMoveUpdateInterval;
	private float aiCursorMoveUpdateTime;
	private Vector3 aiCursorMoveDir;
	private float aiFutureMoveTime;
	private float aiTargtGiveUpTime;
	private float aiGiveUpTime;
	private float AIRayHittingTime;
	public Transform Anchor => anchor;
	public bool IsShot => isShot;
	public bool IsBulletEnd => remainingBulletNum == 0;
	public bool IsPlayer => playerNo < 4;
	public int RemainingBulletNum => remainingBulletNum;
	public int ComboCount
	{
		get
		{
			return comboCount;
		}
		set
		{
			comboCount = value;
		}
	}
	public int GunNo => gunNo;
	public int PlayerNo => playerNo;
	public int RemainingBulletNim => remainingBulletNum;
	private void Awake()
	{
		bullet.gameObject.SetActive(value: false);
	}
	public void Init(int _gunNo)
	{
		SetGunLookCursor();
		gunNo = _gunNo;
		playerNo = SingletonCustom<Shooting_GameManager>.Instance.GetPlayerNo(gunNo);
		for (int i = 0; i < arrayMeshRenderer.Length; i++)
		{
			arrayMeshRenderer[i].material = SingletonCustom<Shooting_ControllerManager>.Instance.ArrayBulletColor[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[playerNo]];
		}
		remainingBulletNum = SingletonCustom<Shooting_ControllerManager>.Instance.RemainingBulletNum;
		useSingleCamera = (gunNo == 0 && SingletonCustom<Shooting_GameManager>.Instance.IsSingle);
		if (useSingleCamera)
		{
			SinglePositionUpdate();
			arrayMeshRenderer[0].gameObject.transform.localScale = new Vector3(0.1f, 0.25f, 0.1f);
		}
		AiInit();
	}
	public void SecondGroupInit()
	{
		playerNo = SingletonCustom<Shooting_GameManager>.Instance.GetPlayerNo(gunNo);
		remainingBulletNum = SingletonCustom<Shooting_ControllerManager>.Instance.RemainingBulletNum;
		AiInit();
	}
	public void UpdateLookCursor()
	{
		SetGunLookCursor();
	}
	public void UpdateMethod()
	{
		if (useSingleCamera)
		{
			SinglePositionUpdate();
		}
	}
	private void SetGunLookCursor()
	{
		if (remainingBulletNum != 0 && !isShot)
		{
			Vector3 worldPosition = Vector3.zero;
			if (CursorRaycast(out RaycastHit _hit, 200f, 134217728))
			{
				worldPosition = _hit.point;
			}
			anchor.LookAt(worldPosition);
		}
	}
	private bool GetShotFlag()
	{
		if (isShot)
		{
			return false;
		}
		if (IsPlayer)
		{
			return GetShotButtonDown();
		}
		return CheckCanAiShot();
	}
	private void Shot()
	{
		if (IsBulletEnd)
		{
			return;
		}
		UnityEngine.Debug.Log("発射");
		if (!isShot)
		{
			isShot = true;
			Shooting_Bullet shooting_Bullet = UnityEngine.Object.Instantiate(bullet, shotPoint.position, shotPoint.rotation, base.transform);
			shooting_Bullet.gameObject.layer = gunNo + 8;
			shooting_Bullet.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = SingletonCustom<Shooting_ControllerManager>.Instance.ArrayBulletColor[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[playerNo]];
			shooting_Bullet.gameObject.transform.GetChild(1).GetComponent<TrailRenderer>().startColor = SingletonCustom<Shooting_ControllerManager>.Instance.ArrayTrailColor[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[playerNo]];
			if (playerNo >= 4)
			{
				shooting_Bullet.IsPleryer = false;
			}
			else
			{
				shooting_Bullet.IsPleryer = true;
			}
			UnityEngine.Debug.Log("bulletInstance 生成");
			if (SingletonCustom<Shooting_BulletManager>.Instance.CheckMaxCount())
			{
				SingletonCustom<Shooting_BulletManager>.Instance.Remove(gunNo);
			}
			SingletonCustom<Shooting_BulletManager>.Instance.Add(gunNo, shooting_Bullet);
			float volume = 1f;
			if (SingletonCustom<Shooting_GameManager>.Instance.IsSingle && !IsPlayer)
			{
				volume = 0.5f;
			}
			switch (UnityEngine.Random.Range(0, 3))
			{
			case 0:
				SingletonCustom<AudioManager>.Instance.SePlay("se_blowgun_shot_00", _loop: false, 0f, volume);
				break;
			case 1:
				SingletonCustom<AudioManager>.Instance.SePlay("se_blowgun_shot_01", _loop: false, 0f, volume);
				break;
			case 2:
				SingletonCustom<AudioManager>.Instance.SePlay("se_blowgun_shot_02", _loop: false, 0f, volume);
				break;
			}
			if (IsPlayer)
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration(playerNo);
			}
			shooting_Bullet.Shot(gunNo, anchor.forward, playerNo);
			LeanTween.delayedCall(1.2f, (Action)delegate
			{
				isShot = false;
				loadingBullet.SetActive(value: true);
			});
			remainingBulletNum--;
			SingletonCustom<Shooting_UIManager>.Instance.UpdateRemainingBulletNumUI(gunNo, remainingBulletNum);
			SingletonCustom<Shooting_UIManager>.Instance.StopCursor(gunNo);
			if (IsBulletEnd)
			{
				SingletonCustom<Shooting_UIManager>.Instance.ViewBulletEnd(gunNo);
			}
			AiShot();
		}
	}
	public void PlayerControl()
	{
		Vector2 stickDir = GetStickDir();
		GetSpeedUpButton();
		SingletonCustom<Shooting_UIManager>.Instance.SetCursorMoveDir(gunNo, stickDir * 2f, anchor.gameObject, isShot);
		if (GetShotFlag())
		{
			Shot();
		}
	}
	public bool CursorRaycast(out RaycastHit _hit, float _maxDistance, int _layerMask)
	{
		Vector3 pos = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().WorldToScreenPoint(SingletonCustom<Shooting_UIManager>.Instance.GetCursorPosition(gunNo));
		return Physics.Raycast((useSingleCamera ? SingletonCustom<Shooting_GameManager>.Instance.WorldSingleCamera : SingletonCustom<Shooting_GameManager>.Instance.WorldCamera).ScreenPointToRay(pos), out _hit, _maxDistance, _layerMask);
	}
	public void SinglePositionUpdate()
	{
		float x = SingletonCustom<Shooting_ControllerManager>.Instance.GetSinglePosXLerp() * -0.6f + 0.3f;
		anchor.SetLocalPositionX(x);
		float y = SingletonCustom<Shooting_ControllerManager>.Instance.GetSinglePosYLerp() * -0.15f + -0.45f;
		anchor.SetLocalPositionY(y);
	}
	private bool GetShotButtonDown()
	{
		int playerIdx;
		if (SingletonCustom<JoyConManager>.Instance.IsSingleMode())
		{
			if (!IsPlayer)
			{
				return false;
			}
			playerIdx = 0;
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.A);
		}
		playerIdx = SingletonCustom<GameSettingManager>.Instance.GetAllocNpadId(playerNo);
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.A);
	}
	private Vector2 GetStickDir()
	{
		if (isShot)
		{
			return Vector2.zero;
		}
		float num = 0f;
		float num2 = 0f;
		Vector2 vector = CalcManager.mVector3Zero;
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? playerNo : 0;
		JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerIdx);
		num = axisInput.Stick_L.x;
		num2 = axisInput.Stick_L.y;
		if (true && Mathf.Abs(num) < 0.2f && Mathf.Abs(num2) < 0.2f)
		{
			num = 0f;
			num2 = 0f;
			if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Dpad_Right))
			{
				num = 1f;
			}
			else if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Dpad_Left))
			{
				num = -1f;
			}
			if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Dpad_Up))
			{
				num2 = 1f;
			}
			else if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Dpad_Down))
			{
				num2 = -1f;
			}
		}
		vector = new Vector2(num, num2);
		if (vector.sqrMagnitude < 0.0400000028f)
		{
			return Vector2.zero;
		}
		return vector.normalized;
	}
	private bool GetSpeedUpButton()
	{
		return false;
	}
	public void AiInit()
	{
		aiTarget = null;
		int aiStrength = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		aiAimOkTime = Shooting_Define.AI_AIM_OK_TIMES[aiStrength];
		aiCursorMoveUpdateInterval = Shooting_Define.AI_MOVE_UPDATE_INTERVALS[aiStrength];
		aiFutureMoveTime = Shooting_Define.AI_FUTURE_MOVE_TIMES[aiStrength];
	}
	public void AiUpdate()
	{
		AiTargetCheck();
		AiCursorMove();
		AiShotCheckUpdate();
		if (GetShotFlag())
		{
			Shot();
		}
	}
	private void AiCursorMove()
	{
		int aiStrength = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		if (IsShot || IsBulletEnd)
		{
			return;
		}
		aiCursorMoveUpdateTime += Time.deltaTime;
		if (aiCursorMoveUpdateTime > aiCursorMoveUpdateInterval)
		{
			aiCursorMoveUpdateTime = 0f;
			Vector3 cursorPosition = SingletonCustom<Shooting_UIManager>.Instance.GetCursorPosition(gunNo);
			Vector3 cursorMoveVec = SingletonCustom<Shooting_UIManager>.Instance.GetCursorMoveVec(gunNo);
			cursorPosition += cursorMoveVec * SingletonCustom<Shooting_UIManager>.Instance.CursorMoveValue * aiFutureMoveTime;
			aiCursorMoveDir = aiTargetUiPos - cursorPosition;
			aiCursorMoveDir.z = 0f;
			float num = 2.25f;
			aiCursorMoveDir = aiCursorMoveDir.normalized * num;
			float num2 = 25f * num;
			if (aiStrength == 2)
			{
				num2 = 10f * num;
			}
			aiCursorMoveDir = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f - num2, num2)) * aiCursorMoveDir;
		}
		SingletonCustom<Shooting_UIManager>.Instance.SetCursorMoveDir(gunNo, aiCursorMoveDir, null, isShot);
	}
	private void AiTargetCheck()
	{
		if (aiTargtGiveUpTime >= aiGiveUpTime)
		{
			Shooting_Target shooting_Target = aiTarget = SingletonCustom<Shooting_TargetManager>.Instance.SearchRandomTarget();
			aiTargtGiveUpTime = 0f;
			aiGiveUpTime = UnityEngine.Random.Range(2.5f, 3.5f);
		}
		else
		{
			aiTargtGiveUpTime += Time.deltaTime;
		}
		if (aiTarget != null)
		{
			if (useSingleCamera)
			{
				aiTargetUiPos = SingletonCustom<Shooting_GameManager>.Instance.WorldSingleCamera.WorldToScreenPoint(aiTarget.GetAiTargetAnchor().position);
			}
			else
			{
				aiTargetUiPos = SingletonCustom<Shooting_GameManager>.Instance.WorldCamera.WorldToScreenPoint(aiTarget.GetAiTargetAnchor().position);
			}
			aiTargetUiPos = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(aiTargetUiPos);
			return;
		}
		Shooting_Target shooting_Target2 = SingletonCustom<Shooting_TargetManager>.Instance.SearchRandomTarget();
		if (!(shooting_Target2 == null))
		{
			aiTarget = shooting_Target2;
			if (useSingleCamera)
			{
				aiTargetUiPos = SingletonCustom<Shooting_GameManager>.Instance.WorldSingleCamera.WorldToScreenPoint(shooting_Target2.GetAiTargetAnchor().position);
			}
			else
			{
				aiTargetUiPos = SingletonCustom<Shooting_GameManager>.Instance.WorldCamera.WorldToScreenPoint(shooting_Target2.GetAiTargetAnchor().position);
			}
			aiTargetUiPos = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(aiTargetUiPos);
		}
	}
	private void AiShotCheckUpdate()
	{
		if (!IsShot)
		{
			if (CursorRaycast(out RaycastHit _, 200f, 536870912))
			{
				aiRayHittingTime += Time.deltaTime;
			}
			else if (SingletonCustom<Shooting_ScoreManager>.Instance.ArrayScore[gunNo] >= Shooting_Define.AI_MAXSCORE_SHOT[SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength])
			{
				aiRayHittingTime += Time.deltaTime;
			}
			else
			{
				aiRayHittingTime = 0f;
			}
		}
	}
	private bool CheckCanAiShot()
	{
		bool result = false;
		AIRayHittingTime = aiAimOkTime;
		if (SingletonCustom<Shooting_ScoreManager>.Instance.ArrayScore[gunNo] >= Shooting_Define.AI_MAXSCORE_SHOT[SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength])
		{
			AIRayHittingTime += 20f;
		}
		if (AIRayHittingTime < aiRayHittingTime)
		{
			result = true;
		}
		return result;
	}
	private void AiShot()
	{
		aiRayHittingTime = 0f;
		aiTarget = null;
	}
}
