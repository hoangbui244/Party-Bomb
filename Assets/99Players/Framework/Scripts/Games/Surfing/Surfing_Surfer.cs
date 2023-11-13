using Cinemachine;
using System;
using System.Collections;
using UnityEngine;
public class Surfing_Surfer : MonoBehaviour
{
	public enum MoveCursorDirection
	{
		UP,
		RIGHT,
		LEFT,
		DOWN
	}
	public enum SurferProcessType
	{
		STANDBY,
		START,
		GOAL
	}
	public enum SurferAnimeType
	{
		IDOL,
		SWIM,
		RIDEWAVES,
		WIPEOUT
	}
	[SerializeField]
	[Header("対象のSurfing_Player")]
	public Surfing_Player player;
	[SerializeField]
	[Header("対象のSurfing_AI")]
	private Surfing_AI ai;
	[SerializeField]
	[Header("対象のSurfing_Character")]
	private Surfing_Character character;
	[SerializeField]
	[Header("搭乗キャラオブジェクトの位置移動用")]
	public GameObject characterObj;
	[SerializeField]
	[Header("搭乗キャラオブジェクトの親アンカ\u30fc")]
	public GameObject characterAnchor;
	[SerializeField]
	[Header("搭乗キャラ回転アクション用アンカ\u30fc")]
	public GameObject characterActionAnchor;
	[SerializeField]
	[Header("移動エフェクト")]
	private ParticleSystem psMove;
	[SerializeField]
	[Header("旋回エフェクト(右回り)")]
	private ParticleSystem psTurnRight;
	[SerializeField]
	[Header("旋回エフェクト(左回り)")]
	private ParticleSystem psTurnLeft;
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
	public float directionChangeSpeed = 2000f;
	[SerializeField]
	[Header("キャラのAnimator")]
	private Animator animator;
	[SerializeField]
	[Header("対応するSurfing_Camera")]
	private Surfing_Camera surfingCamera;
	[SerializeField]
	[Header("三人称_通常カメラ")]
	private CinemachineVirtualCamera cameraNormal;
	[SerializeField]
	[Header("見えない壁用アニメ\u30fcション")]
	private Animation transparentWallAnim;
	[SerializeField]
	[Header("対応するSurfing_WaveManager")]
	private Surfing_WaveManager waveManager;
	[SerializeField]
	[Header("プレイヤ\u30fc番号")]
	private int playerNo;
	private Rigidbody rb;
	private float maxSpeed = 10f;
	private float addSpeed;
	private Vector3 moveVector;
	private float driftAngle = 30f;
	private float accelDeg;
	private Surfing_Define.UserType userType;
	public SurferProcessType processType;
	private SurferAnimeType animeType;
	private bool isGround;
	private bool isJump;
	private IEnumerator routine;
	private bool isCrash;
	private float sePitch;
	private bool isTube;
	private bool isWaveTop;
	private bool isWaveRide;
	private bool isWaveArea;
	private bool isFacingTop;
	private bool isFacingUnder;
	private float pointGetTime = 2f;
	private float pointGetTimer;
	private int id_curve;
	private Vector3 respornPos;
	private float cameraDeg;
	private CinemachineTransposer artViewCameraTransposer;
	private int layerMask;
	private bool isSuperJumpInput;
	private bool isSuperJumpSuccess;
	private bool isPlaySuperJumpAction;
	public int PlayerNo => playerNo;
	public bool IsGround => isGround;
	public bool IsJump => isJump;
	public bool IsCrash => isCrash;
	public bool IsTube => isTube;
	public bool IsWaveTop => isWaveTop;
	public bool IsWaveRide => isWaveRide;
	public bool IsWaveArea => isWaveArea;
	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		artViewCameraTransposer = cameraNormal.GetCinemachineComponent<CinemachineTransposer>();
		respornPos = base.gameObject.transform.position;
		ProcessTypeChange(SurferProcessType.STANDBY);
		layerMask = base.gameObject.layer;
	}
	public void PlayerInit(Surfing_Player _player)
	{
		userType = _player.UserType;
		rb = GetComponent<Rigidbody>();
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		ProcessTypeChange(SurferProcessType.STANDBY);
		Surfing_Define.PM.SetPoint(userType, 0);
		isCrash = false;
	}
	public void StartMethod()
	{
		pointGetTimer = pointGetTime;
		ProcessTypeChange(SurferProcessType.START);
	}
	private void FixedUpdate()
	{
		isTube = false;
		isWaveTop = false;
		isWaveRide = false;
		isWaveArea = false;
		characterObj.transform.position = base.transform.position;
		if (processType == SurferProcessType.START)
		{
			addSpeed = Mathf.Clamp(addSpeed - Time.deltaTime * 1f, 0f, 10f);
			rb.AddForce(character.angleAcc * acceleration);
			if (!isGround && !isJump)
			{
				rb.AddForce(Vector3.down * 9.8f);
			}
			if (rb.velocity.sqrMagnitude >= maxSpeed + addSpeed)
			{
				while (rb.velocity.sqrMagnitude >= maxSpeed + addSpeed)
				{
					rb.velocity *= 0.98f;
				}
			}
			if (isCrash)
			{
				rb.velocity *= 0f;
			}
			if (!isGround && isJump)
			{
				rb.MovePosition(base.transform.position + Vector3.back * Time.deltaTime * 2f);
			}
		}
		else if (processType == SurferProcessType.GOAL)
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
		else if (processType == SurferProcessType.STANDBY)
		{
			rb.velocity *= 0f;
		}
	}
	private void Update()
	{
		character.UpdateMethod();
		if (processType != SurferProcessType.START || isCrash)
		{
			return;
		}
		if (isTube)
		{
			pointGetTimer -= Time.deltaTime;
			if (pointGetTimer <= 0f)
			{
				pointGetTimer = pointGetTime;
				GetPoint(200);
			}
		}
		else if (isWaveTop)
		{
			pointGetTimer -= Time.deltaTime;
			if (pointGetTimer <= 0f)
			{
				pointGetTimer = pointGetTime;
				GetPoint(200);
			}
		}
		else
		{
			pointGetTimer = Mathf.Clamp(pointGetTimer + Time.deltaTime, 0f, pointGetTime);
		}
		if (isWaveRide)
		{
			if (!isFacingTop && !isFacingUnder)
			{
				if (character.IsFacingTop())
				{
					isFacingTop = true;
				}
				else if (character.IsFacingUnder())
				{
					isFacingUnder = true;
				}
			}
			else if (isFacingTop)
			{
				if (character.IsFacingUnder())
				{
					isFacingTop = false;
					isFacingUnder = true;
					GetPoint(100);
				}
			}
			else if (isFacingUnder && character.IsFacingTop())
			{
				isFacingUnder = false;
				isFacingTop = true;
				GetPoint(100);
			}
		}
		else
		{
			isFacingTop = false;
			isFacingUnder = false;
		}
		if (isWaveArea)
		{
			SetAnimator(SurferAnimeType.RIDEWAVES);
		}
		else if (isGround)
		{
			SetAnimator(SurferAnimeType.SWIM);
		}
		if (animeType != SurferAnimeType.RIDEWAVES)
		{
			if (psMove.isPlaying)
			{
				psMove.Stop();
			}
		}
		else if (isGround && psMove.isStopped)
		{
			psMove.Play();
		}
		if (transparentWallAnim.gameObject.activeSelf && !transparentWallAnim.isPlaying)
		{
			transparentWallAnim.gameObject.SetActive(value: false);
		}
	}
	public void MoveCursor(MoveCursorDirection _dir, float _input)
	{
		switch (_dir)
		{
		case MoveCursorDirection.RIGHT:
			CurveMove(_input);
			if (animeType == SurferAnimeType.RIDEWAVES && isGround)
			{
				psTurnRight.Emit(1);
			}
			break;
		case MoveCursorDirection.LEFT:
			CurveMove(_input);
			if (animeType == SurferAnimeType.RIDEWAVES && isGround)
			{
				psTurnLeft.Emit(1);
			}
			break;
		}
		NormalJumpAction(_dir);
	}
	private void NormalJumpAction(MoveCursorDirection _dir)
	{
		if (isJump && !isGround && !isPlaySuperJumpAction)
		{
			isPlaySuperJumpAction = true;
			switch (_dir)
			{
			case MoveCursorDirection.UP:
				LeanTween.rotateAroundLocal(characterActionAnchor, Vector3.right, 360f, 1f).setEase(LeanTweenType.easeOutQuad);
				break;
			case MoveCursorDirection.RIGHT:
				LeanTween.rotateAroundLocal(characterActionAnchor, Vector3.up, 360f, 1f).setEase(LeanTweenType.easeOutQuad);
				break;
			case MoveCursorDirection.LEFT:
				LeanTween.rotateAroundLocal(characterActionAnchor, Vector3.up, -360f, 1f).setEase(LeanTweenType.easeOutQuad);
				break;
			case MoveCursorDirection.DOWN:
				LeanTween.rotateAroundLocal(characterActionAnchor, Vector3.right, -360f, 1f).setEase(LeanTweenType.easeOutQuad);
				break;
			}
		}
	}
	private void SuperJumpAction()
	{
	}
	public void MoveCameraDeg(float _input)
	{
		cameraDeg += _input * 2f;
		artViewCameraTransposer.m_FollowOffset.x = 4f * Mathf.Sin((float)Math.PI / 180f * cameraDeg);
		artViewCameraTransposer.m_FollowOffset.z = -4f * Mathf.Cos((float)Math.PI / 180f * cameraDeg);
	}
	public void ProcessTypeChange(SurferProcessType _type)
	{
		switch (_type)
		{
		case SurferProcessType.STANDBY:
			processType = SurferProcessType.STANDBY;
			break;
		case SurferProcessType.START:
		{
			if (processType == SurferProcessType.STANDBY)
			{
				processType = SurferProcessType.START;
			}
			Surfing_Define.UserType userType2 = player.UserType;
			break;
		}
		case SurferProcessType.GOAL:
			if (processType == SurferProcessType.START)
			{
				processType = SurferProcessType.GOAL;
			}
			break;
		}
	}
	private void CurveMove(float _input)
	{
		if (!isCrash && processType == SurferProcessType.START)
		{
			LeanTween.cancel(id_curve);
			id_curve = LeanTween.rotateAround(characterObj, characterObj.transform.up, _input * directionChangeSpeed * Time.deltaTime, 0.1f).id;
		}
	}
	public void InputSuperJumpAction()
	{
		if (!isSuperJumpInput)
		{
			StartCoroutine(InputJumpActionTime());
		}
	}
	public void GetPoint(int _point)
	{
		Surfing_Define.PM.SetPoint(userType, _point);
	}
	private void Resporn()
	{
		base.gameObject.transform.position = respornPos;
		base.gameObject.transform.localEulerAngles = Vector3.zero;
		characterObj.transform.localEulerAngles = Vector3.zero;
		LeanTween.moveLocalY(characterAnchor, 0f, 0f);
		SetAnimator(SurferAnimeType.IDOL);
		player.characterStyle.SetMainCharacterFaceDiff((int)userType, StyleTextureManager.MainCharacterFaceType.NORMAL);
		cameraDeg = 0f;
		MoveCameraDeg(0f);
		waveManager.Init();
	}
	public void SetAnimator(SurferAnimeType _set)
	{
		if (animeType != _set)
		{
			animeType = _set;
			switch (_set)
			{
			case SurferAnimeType.IDOL:
				animator.SetTrigger("Idol");
				break;
			case SurferAnimeType.SWIM:
				animator.SetTrigger("Swim");
				break;
			case SurferAnimeType.RIDEWAVES:
				animator.SetTrigger("RideWaves");
				break;
			case SurferAnimeType.WIPEOUT:
				animator.SetTrigger("Wipeout");
				break;
			}
		}
	}
	public void ObstacleAction()
	{
		if (!isCrash)
		{
			isCrash = true;
			UnityEngine.Debug.Log("波にのまれた！！");
			rb.velocity *= 0f;
			StartCoroutine(ObstacleCrashAnime());
		}
	}
	private void OnTriggerStay(Collider other)
	{
		if (processType == SurferProcessType.GOAL)
		{
			return;
		}
		if (other.tag == "Water")
		{
			isTube = true;
			UnityEngine.Debug.Log("チュ\u30fcブランディング中");
			rb.MovePosition(base.transform.position + Vector3.back * Time.deltaTime * 2f);
			if (surfingCamera.CameraTubeLeftSide.enabled)
			{
				UnityEngine.Debug.Log("左に補正移動");
				rb.MovePosition(base.transform.position + Vector3.left * Time.deltaTime * 2f);
			}
			else if (surfingCamera.CameraTubeRightSide.enabled)
			{
				UnityEngine.Debug.Log("右に補正移動");
				rb.MovePosition(base.transform.position + Vector3.right * Time.deltaTime * 2f);
			}
		}
		if (other.name.Contains("WaveTopArea"))
		{
			isWaveTop = true;
		}
		if (other.name.Contains("WaveTurnArea"))
		{
			isWaveRide = true;
		}
		if (other.name.Contains("WaveArea"))
		{
			isWaveArea = true;
		}
	}
	private void OnCollisionStay(Collision collision)
	{
		if (processType != SurferProcessType.START)
		{
			return;
		}
		if (!isGround && (collision.gameObject.layer == LayerMask.NameToLayer("Field") || collision.gameObject.layer == layerMask))
		{
			isGround = true;
			if (psMove.isStopped)
			{
				psMove.Play();
			}
			isPlaySuperJumpAction = false;
			LeanTween.moveLocalY(characterAnchor, -0.05f, 0.2f).setEase(LeanTweenType.easeOutCubic).setOnComplete((Action)delegate
			{
				LeanTween.moveLocalY(characterAnchor, 0f, 0.25f).setEase(LeanTweenType.easeOutBounce);
			});
		}
		if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
		{
			if (!transparentWallAnim.gameObject.activeSelf)
			{
				transparentWallAnim.gameObject.SetActive(value: true);
				transparentWallAnim.Play();
			}
			transparentWallAnim.transform.position = base.transform.position - collision.contacts[0].normal * 0.3f;
			transparentWallAnim.transform.AddPositionY(0.25f);
			transparentWallAnim.transform.forward = collision.contacts[0].normal;
		}
	}
	private void OnCollisionExit(Collision collision)
	{
		if (processType != SurferProcessType.START || !isGround || (collision.gameObject.layer != LayerMask.NameToLayer("Field") && collision.gameObject.layer != layerMask))
		{
			return;
		}
		isGround = false;
		if (psMove.isPlaying)
		{
			psMove.Stop();
		}
		if (isWaveTop && !isJump && IsDirFront())
		{
			StartCoroutine(JumpTime());
			if (isSuperJumpSuccess)
			{
				isPlaySuperJumpAction = true;
				UnityEngine.Debug.Log("ジャンプアクションに成功");
				GetPoint(500);
				rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
			}
			else
			{
				GetPoint(200);
				addSpeed += 10f;
				rb.AddForce(Vector3.up * 5f + character.angleAcc * 5f, ForceMode.Impulse);
			}
		}
	}
	private IEnumerator ObstacleCrashAnime()
	{
		player.characterStyle.SetMainCharacterFaceDiff((int)userType, StyleTextureManager.MainCharacterFaceType.SAD);
		SetAnimator(SurferAnimeType.WIPEOUT);
		LeanTween.moveLocalY(characterAnchor, -0.5f, 1f);
		if (player.UserType <= Surfing_Define.UserType.PLAYER_4)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_surfing_drown", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
		}
		yield return new WaitForSeconds(0.5f);
		Surfing_Define.UIM.SetFade(userType);
		yield return new WaitForSeconds(1f);
		Resporn();
		yield return new WaitForSeconds(1f);
		Surfing_Define.UIM.SetFade(userType, _set: false);
		yield return new WaitForSeconds(1f);
		isCrash = false;
	}
	private IEnumerator JumpTime()
	{
		isJump = true;
		yield return new WaitForSeconds(1f);
		isJump = false;
	}
	private IEnumerator InputJumpActionTime()
	{
		isSuperJumpInput = true;
		isSuperJumpSuccess = true;
		yield return new WaitForSeconds(0.5f);
		isSuperJumpSuccess = false;
		yield return new WaitForSeconds(0.5f);
		isSuperJumpInput = false;
	}
	public bool IsDirFront()
	{
		if (characterObj.transform.localEulerAngles.y < 90f || characterObj.transform.localEulerAngles.y > 270f)
		{
			return true;
		}
		return false;
	}
	public bool IsDirRight()
	{
		if (characterObj.transform.localEulerAngles.y < 180f)
		{
			return true;
		}
		return false;
	}
	public bool IsDirRightTubeRideing()
	{
		if (characterObj.transform.localEulerAngles.y < 225f || characterObj.transform.localEulerAngles.y > 315f)
		{
			return true;
		}
		return false;
	}
	public bool IsDirLeftTubeRideing()
	{
		if (characterObj.transform.localEulerAngles.y < 45f || characterObj.transform.localEulerAngles.y > 135f)
		{
			return true;
		}
		return false;
	}
}
