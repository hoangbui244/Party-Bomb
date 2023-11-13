using System.Collections;
using UnityEngine;
public class WaterSpriderRace_WaterSprider : MonoBehaviour
{
	public enum MoveCursorDirection
	{
		UP,
		RIGHT,
		LEFT,
		DOWN
	}
	public enum SkiBoardProcessType
	{
		STANDBY,
		START,
		GOAL
	}
	public enum CameraPosType
	{
		NEAR,
		NORMAL,
		DISTANT
	}
	[SerializeField]
	[Header("対象のWaterSpriderRace_Player")]
	public WaterSpriderRace_Player player;
	[SerializeField]
	[Header("対象のWaterSpriderRace_AI")]
	private WaterSpriderRace_AI ai;
	[SerializeField]
	[Header("搭乗キャラオブジェクトの位置移動用")]
	public GameObject characterObj;
	[SerializeField]
	[Header("Characterの親アンカ\u30fc(Y軸回転)")]
	public GameObject characterAnchorY;
	[SerializeField]
	[Header("Characterの親アンカ\u30fc(X軸回転)")]
	public GameObject characterAnchorX;
	[SerializeField]
	[Header("移動時エフェクト(右足)")]
	private ParticleSystem psMoveRippleRight;
	[SerializeField]
	[Header("移動時エフェクト(左足)")]
	private ParticleSystem psMoveRippleLeft;
	[SerializeField]
	[Header("転倒エフェクト")]
	private ParticleSystem psCrash;
	[SerializeField]
	[Header("水しぶきエフェクト")]
	private ParticleSystem psBreak;
	[SerializeField]
	[Header("加速度")]
	public float acceleration = 2f;
	[SerializeField]
	[Header("方向の変えやすさ")]
	public float directionChangeSpeed = 3f;
	[SerializeField]
	[Header("WaterSpriderRace_Animation")]
	private WaterSpriderRace_Animation anime;
	[SerializeField]
	[Header("プレイヤ\u30fc番号")]
	private int playerNo;
	private Rigidbody rb;
	private Vector3 moveVector;
	private float driftAngle = 30f;
	private float accelDeg;
	private WaterSpriderRace_Define.UserType userType;
	public SkiBoardProcessType processType;
	private bool isInvincible;
	private CameraPosType cameraPos;
	private bool isRightStep;
	private IEnumerator routine;
	private bool isCrash;
	private float sePitch;
	public int PlayerNo => playerNo;
	public CameraPosType CameraPos => cameraPos;
	public void PlayerInit(WaterSpriderRace_Player _player)
	{
		userType = _player.UserType;
		rb = GetComponent<Rigidbody>();
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		ProcessTypeChange(SkiBoardProcessType.STANDBY);
		cameraPos = CameraPosType.NORMAL;
		isCrash = false;
	}
	public void StartMethod()
	{
		ProcessTypeChange(SkiBoardProcessType.START);
		routine = StopMoveRippleEffect();
	}
	public void UpdateMethod()
	{
	}
	private void FixedUpdate()
	{
		characterObj.transform.position = base.transform.position;
		if (processType == SkiBoardProcessType.START)
		{
			if (rb.velocity.sqrMagnitude >= 10f)
			{
				while (rb.velocity.sqrMagnitude >= 10f)
				{
					rb.velocity *= 0.98f;
				}
			}
			moveVector = rb.velocity.normalized;
			if (isCrash)
			{
				rb.velocity *= 0f;
			}
			rb.velocity *= 0.992f;
		}
		else if (processType == SkiBoardProcessType.GOAL)
		{
			if (rb.velocity.sqrMagnitude > 0.01f)
			{
				rb.velocity *= 0.98f;
			}
			else
			{
				rb.velocity *= 0f;
			}
		}
		else if (processType == SkiBoardProcessType.STANDBY)
		{
			rb.velocity *= 0f;
		}
	}
	public void AccelInput()
	{
		if (!isCrash)
		{
			rb.AddForce(characterAnchorY.transform.forward * acceleration);
			if (Vector3.Angle(characterAnchorY.transform.forward, moveVector) >= driftAngle)
			{
				rb.AddForce(characterAnchorY.transform.forward * acceleration);
			}
			StopCoroutine(routine);
			routine = null;
			if (isRightStep)
			{
				anime.SetAnim(WaterSpriderRace_Animation.AnimType.ACCEL_LEFTLEG_FRONT);
				isRightStep = false;
				psMoveRippleLeft.Play();
				psMoveRippleRight.Stop();
			}
			else
			{
				anime.SetAnim(WaterSpriderRace_Animation.AnimType.ACCEL_RIGHTLEG_FRONT);
				isRightStep = true;
				psMoveRippleRight.Play();
				psMoveRippleLeft.Stop();
			}
			routine = StopMoveRippleEffect();
			StartCoroutine(routine);
			if (WaterSpriderRace_Define.GM.IsDuringGame() && player.UserType <= WaterSpriderRace_Define.UserType.PLAYER_4)
			{
				sePitch = UnityEngine.Random.Range(0.8f, 1f);
				SingletonCustom<AudioManager>.Instance.SePlay("se_fishing_caught_empty", _loop: false, 0f, 1f, sePitch, 0f, _overlap: true);
			}
		}
	}
	public void BreakInput()
	{
		rb.velocity *= 0.9f;
	}
	public void MoveCursor(MoveCursorDirection _dir, float _input)
	{
		switch (_dir)
		{
		case MoveCursorDirection.UP:
		case MoveCursorDirection.DOWN:
			break;
		case MoveCursorDirection.RIGHT:
			CurveMove(_input);
			break;
		case MoveCursorDirection.LEFT:
			CurveMove(_input);
			break;
		}
	}
	public void ProcessTypeChange(SkiBoardProcessType _type)
	{
		switch (_type)
		{
		case SkiBoardProcessType.STANDBY:
			processType = SkiBoardProcessType.STANDBY;
			break;
		case SkiBoardProcessType.START:
		{
			if (processType == SkiBoardProcessType.STANDBY)
			{
				processType = SkiBoardProcessType.START;
			}
			WaterSpriderRace_Define.UserType userType2 = player.UserType;
			break;
		}
		case SkiBoardProcessType.GOAL:
			if (processType == SkiBoardProcessType.START)
			{
				processType = SkiBoardProcessType.GOAL;
			}
			break;
		}
	}
	public void CameraPosTypeChange(bool _set)
	{
		switch (cameraPos)
		{
		case CameraPosType.NORMAL:
			cameraPos = CameraPosType.DISTANT;
			break;
		case CameraPosType.DISTANT:
			cameraPos = CameraPosType.NORMAL;
			break;
		}
	}
	private void CurveMove(float _input)
	{
		if (!isCrash && processType == SkiBoardProcessType.START)
		{
			characterAnchorY.transform.forward = Vector3.Lerp(characterAnchorY.transform.forward, Quaternion.Euler(0f, _input * directionChangeSpeed * Time.deltaTime, 0f) * characterAnchorY.transform.forward, 0.1f);
		}
	}
	private void OnTriggerEnter(Collider other)
	{
		if (processType == SkiBoardProcessType.GOAL)
		{
			return;
		}
		if (other.name.Contains("GoalChecker") && processType != SkiBoardProcessType.GOAL)
		{
			ProcessTypeChange(SkiBoardProcessType.GOAL);
			StopCoroutine(routine);
			routine = null;
			psMoveRippleLeft.Stop();
			psMoveRippleRight.Stop();
			int num = 0;
			for (int i = 0; i < WaterSpriderRace_Define.PM.UserData_Group1.Length; i++)
			{
				if (WaterSpriderRace_Define.PM.UserData_Group1[playerNo].goalTime > WaterSpriderRace_Define.PM.UserData_Group1[i].goalTime)
				{
					num++;
				}
			}
			switch (num)
			{
			case 0:
				player.characterStyle.SetMainCharacterFaceDiff((int)userType, StyleTextureManager.MainCharacterFaceType.HAPPY);
				anime.SetAnim(WaterSpriderRace_Animation.AnimType.WINNER);
				break;
			case 1:
				player.characterStyle.SetMainCharacterFaceDiff((int)userType, StyleTextureManager.MainCharacterFaceType.SMILE);
				anime.SetAnim(WaterSpriderRace_Animation.AnimType.JOY);
				break;
			case 2:
				player.characterStyle.SetMainCharacterFaceDiff((int)userType, StyleTextureManager.MainCharacterFaceType.NORMAL);
				anime.SetAnim(WaterSpriderRace_Animation.AnimType.FIGHT);
				break;
			case 3:
				player.characterStyle.SetMainCharacterFaceDiff((int)userType, StyleTextureManager.MainCharacterFaceType.SAD);
				anime.SetAnim(WaterSpriderRace_Animation.AnimType.SAD);
				break;
			}
			if (!WaterSpriderRace_Define.GM.IsGameEnd())
			{
				if (player.UserType <= WaterSpriderRace_Define.UserType.PLAYER_4)
				{
					SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
					WaterSpriderRace_Define.GM.GoalSEPlay();
				}
				WaterSpriderRace_Define.PM.SetGoalTime(userType, WaterSpriderRace_Define.GM.GetGameTime());
				if (WaterSpriderRace_Define.PLAYER_NUM != 1 && WaterSpriderRace_Define.PM.PlayerOnlyOneCheck() && WaterSpriderRace_Define.PM.StartPlayerNumCheck() == 1)
				{
					WaterSpriderRace_Define.GM.StartForcedGoal();
				}
				if (WaterSpriderRace_Define.PM.PlayerGoalCheck())
				{
					WaterSpriderRace_Define.GM.GameEnd();
				}
			}
		}
		if (other.name.Contains("ObstacleTrigger"))
		{
			if (isInvincible || isCrash)
			{
				return;
			}
			isCrash = true;
			rb.velocity *= 0f;
			StartCoroutine(ObstacleCrashAnime());
		}
		if (other.name.Contains("ObstacleBreakTrigger") && !isInvincible && !isCrash)
		{
			isCrash = true;
			rb.velocity *= 0.2f;
			StartCoroutine(ObstacleBreakAnime());
		}
	}
	private void OnCollisionStay(Collision collision)
	{
		if (processType == SkiBoardProcessType.GOAL)
		{
			return;
		}
		if (collision.gameObject.layer == LayerMask.NameToLayer("Ball"))
		{
			if (isInvincible || isCrash)
			{
				return;
			}
			StartCoroutine(OnCollisionCrashWait());
		}
		if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && !isInvincible && !isCrash)
		{
			StartCoroutine(OnCollisionBreakWait());
		}
	}
	private void OnCollisionEnter(Collision collision)
	{
	}
	private IEnumerator ObstacleCrashAnime()
	{
		StopCoroutine(routine);
		routine = null;
		routine = StopMoveRippleEffect();
		psMoveRippleLeft.Stop();
		psMoveRippleRight.Stop();
		anime.SetAnim(WaterSpriderRace_Animation.AnimType.STANDBY);
		player.characterStyle.SetMainCharacterFaceDiff((int)userType, StyleTextureManager.MainCharacterFaceType.SAD);
		LeanTween.rotateX(characterAnchorX, -15f, 1f).setEase(LeanTweenType.easeOutCubic);
		yield return new WaitForSeconds(0.25f);
		anime.SetAnim(WaterSpriderRace_Animation.AnimType.CRASH_RIGHTLEG_FRONT);
		if (WaterSpriderRace_Define.GM.IsDuringGame() && player.UserType <= WaterSpriderRace_Define.UserType.PLAYER_4)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_goldfish_scoop", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
			SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)userType, HidVibration.VibrationType.Normal, 0.2f);
		}
		yield return new WaitForSeconds(0.25f);
		anime.SetAnim(WaterSpriderRace_Animation.AnimType.CRASH_LEFTLEG_FRONT);
		if (WaterSpriderRace_Define.GM.IsDuringGame() && player.UserType <= WaterSpriderRace_Define.UserType.PLAYER_4)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_goldfish_scoop", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
			SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)userType, HidVibration.VibrationType.Normal, 0.2f);
		}
		yield return new WaitForSeconds(0.25f);
		anime.SetAnim(WaterSpriderRace_Animation.AnimType.CRASH_RIGHTLEG_FRONT);
		if (WaterSpriderRace_Define.GM.IsDuringGame() && player.UserType <= WaterSpriderRace_Define.UserType.PLAYER_4)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_goldfish_scoop", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
			SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)userType, HidVibration.VibrationType.Normal, 0.2f);
		}
		yield return new WaitForSeconds(0.25f);
		anime.SetAnim(WaterSpriderRace_Animation.AnimType.CRASH_LEFTLEG_FRONT);
		if (WaterSpriderRace_Define.GM.IsDuringGame() && player.UserType <= WaterSpriderRace_Define.UserType.PLAYER_4)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_goldfish_scoop", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
			SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)userType, HidVibration.VibrationType.Normal, 0.2f);
		}
		LeanTween.rotateX(characterAnchorX, -90f, 0.5f).setEase(LeanTweenType.easeInQuad);
		yield return new WaitForSeconds(0.5f);
		psCrash.Play();
		if (WaterSpriderRace_Define.GM.IsDuringGame() && player.UserType <= WaterSpriderRace_Define.UserType.PLAYER_4)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_fishing_caught_many", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
			SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)userType, HidVibration.VibrationType.Strong, 0.8f);
		}
		yield return new WaitForSeconds(0.5f);
		anime.Reset();
		LeanTween.rotateX(characterAnchorX, 0f, 0.5f);
		yield return new WaitForSeconds(0.5f);
		player.characterStyle.SetMainCharacterFaceDiff((int)userType, StyleTextureManager.MainCharacterFaceType.NORMAL);
		anime.SetAnim(WaterSpriderRace_Animation.AnimType.STANDBY);
		isCrash = false;
		StartCoroutine(InvincibleTime());
		ai.CrashMethod();
	}
	private IEnumerator ObstacleBreakAnime()
	{
		StopCoroutine(routine);
		routine = null;
		routine = StopMoveRippleEffect();
		psMoveRippleLeft.Stop();
		psMoveRippleRight.Stop();
		anime.SetAnim(WaterSpriderRace_Animation.AnimType.STANDBY);
		player.characterStyle.SetMainCharacterFaceDiff((int)userType, StyleTextureManager.MainCharacterFaceType.SAD);
		yield return new WaitForSeconds(0.25f);
		anime.SetAnim(WaterSpriderRace_Animation.AnimType.CRASH_RIGHTLEG_FRONT);
		if (WaterSpriderRace_Define.GM.IsDuringGame() && player.UserType <= WaterSpriderRace_Define.UserType.PLAYER_4)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_goldfish_scoop", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
			SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)userType, HidVibration.VibrationType.Normal, 0.2f);
		}
		yield return new WaitForSeconds(0.25f);
		anime.SetAnim(WaterSpriderRace_Animation.AnimType.CRASH_LEFTLEG_FRONT);
		if (WaterSpriderRace_Define.GM.IsDuringGame() && player.UserType <= WaterSpriderRace_Define.UserType.PLAYER_4)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_goldfish_scoop", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
			SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)userType, HidVibration.VibrationType.Normal, 0.2f);
		}
		yield return new WaitForSeconds(0.25f);
		anime.SetAnim(WaterSpriderRace_Animation.AnimType.CRASH_RIGHTLEG_FRONT);
		if (WaterSpriderRace_Define.GM.IsDuringGame() && player.UserType <= WaterSpriderRace_Define.UserType.PLAYER_4)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_goldfish_scoop", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
			SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)userType, HidVibration.VibrationType.Normal, 0.2f);
		}
		yield return new WaitForSeconds(0.25f);
		anime.SetAnim(WaterSpriderRace_Animation.AnimType.CRASH_LEFTLEG_FRONT);
		if (WaterSpriderRace_Define.GM.IsDuringGame() && player.UserType <= WaterSpriderRace_Define.UserType.PLAYER_4)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_goldfish_scoop", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
			SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)userType, HidVibration.VibrationType.Normal, 0.2f);
		}
		yield return new WaitForSeconds(0.5f);
		anime.Reset();
		yield return new WaitForSeconds(0.5f);
		player.characterStyle.SetMainCharacterFaceDiff((int)userType, StyleTextureManager.MainCharacterFaceType.NORMAL);
		isCrash = false;
		StartCoroutine(InvincibleTime());
		ai.CrashMethod();
	}
	private IEnumerator StopMoveRippleEffect()
	{
		yield return new WaitForSeconds(2f);
		psMoveRippleLeft.Stop();
		psMoveRippleRight.Stop();
		anime.SetAnim(WaterSpriderRace_Animation.AnimType.STANDBY);
	}
	private IEnumerator OnCollisionCrashWait()
	{
		yield return new WaitForSeconds(0.1f);
		if (!isCrash)
		{
			isCrash = true;
			rb.velocity *= 0f;
			StartCoroutine(ObstacleCrashAnime());
		}
	}
	private IEnumerator OnCollisionBreakWait()
	{
		yield return new WaitForSeconds(0.1f);
		if (!isCrash)
		{
			isCrash = true;
			rb.velocity *= 0.2f;
			StartCoroutine(ObstacleBreakAnime());
		}
	}
	private IEnumerator InvincibleTime()
	{
		isInvincible = true;
		if (player.UserType <= WaterSpriderRace_Define.UserType.PLAYER_4)
		{
			yield return new WaitForSeconds(1.5f);
		}
		else
		{
			yield return new WaitForSeconds(7f);
		}
		isInvincible = false;
	}
}
