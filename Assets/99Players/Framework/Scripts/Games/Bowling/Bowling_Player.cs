using System;
using UnityEngine;
public class Bowling_Player : MonoBehaviour
{
	public struct ThrowData
	{
		public Vector3 throwVec;
		public float touchLengthPer;
		public Bowling_Define.ThrowType throwType;
	}
	private ThrowData throwData;
	[SerializeField]
	[Header("ボ\u30fcル")]
	private Bowling_Ball ball;
	[SerializeField]
	[Header("AI")]
	private Bowling_AI ai;
	[SerializeField]
	[Header("ボ\u30fcルUI")]
	private Bowling_BallMoveUI ballUI;
	private Bowling_BallThrowUI throwUI;
	private Bowling_Define.OperationState operationState;
	private Bowling_Define.UserType userType;
	private Bowling_Define.TeamType teamType;
	private bool isPlayer;
	private Vector3 inputCameraAngle;
	private readonly float AXIS_MIN_DIR = 0.05f;
	private readonly float CAMERA_ANGLE_SPEED = 0.5f;
	private const float STICK_OFFSET = 0.3f;
	public readonly float PULL_MAX_ANGLE = 60f;
	public readonly float PULL_MAX_LENGTH = 150f;
	public readonly float PULL_MIN_LENGTH = 50f;
	private float stickHorizontal;
	private float stickVertical;
	public ThrowData gsThrowData => throwData;
	public Bowling_Define.OperationState OperationState
	{
		get
		{
			return operationState;
		}
		set
		{
			operationState = value;
		}
	}
	public Bowling_Define.UserType UserType => userType;
	public void Init(Bowling_Define.UserType _userType, Bowling_Define.TeamType _teamType, bool _isPlayer)
	{
		userType = _userType;
		teamType = _teamType;
		isPlayer = _isPlayer;
		ball.Init(userType);
		if (!isPlayer)
		{
			ai.Init();
		}
		inputCameraAngle = Vector3.zero;
		throwUI = Bowling_Define.GUIM.GetBallThrowUI();
		ballUI.Init();
	}
	public void UpdateMethod()
	{
		ball.UpdateMethod();
		if (!isPlayer)
		{
			return;
		}
		stickHorizontal = Bowling_Define.CM.GetLStickDir(userType).x;
		stickVertical = Bowling_Define.CM.GetLStickDir(userType).z;
		switch (operationState)
		{
		case Bowling_Define.OperationState.NONE:
			break;
		case Bowling_Define.OperationState.BALL_MOVE:
		{
			float x2 = ball.InitPos.x;
			ball.GetBallPos();
			float x3 = Bowling_Define.MSM.GetApproachSize.x;
			ball.ThrowAnimAnchor.AddPositionX(stickHorizontal * Time.deltaTime);
			ball.ThrowAnimAnchor.SetPositionX(Mathf.Clamp(ball.ThrowAnimAnchor.position.x, x2 - x3 * 0.47f, x2 + x3 * 0.47f));
			ballUI.SetBallArrowPos(ball.GetBallPos());
			Bowling_Define.MSM.SetStageCameraPosX(ball.GetBallPos().x);
			bool flag2 = false;
			if (Bowling_Define.CM.IsPushButton_A(userType, Bowling_ControllerManager.ButtonPushType.HOLD))
			{
				flag2 = true;
				throwData.throwType = Bowling_Define.ThrowType.LEFT_S;
			}
			else if (Bowling_Define.CM.IsPushButton_X(userType, Bowling_ControllerManager.ButtonPushType.HOLD))
			{
				flag2 = true;
				throwData.throwType = Bowling_Define.ThrowType.STRAIGHT;
			}
			else if (Bowling_Define.CM.IsPushButton_Y(userType, Bowling_ControllerManager.ButtonPushType.HOLD))
			{
				flag2 = true;
				throwData.throwType = Bowling_Define.ThrowType.RIGHT_S;
			}
			throwUI.SetThrowArrow(throwData.throwType);
			if (flag2)
			{
				operationState = Bowling_Define.OperationState.VECTOR_SELECT;
				Bowling_Define.MSM.GetCameraRoot.SetLocalEulerAngles(0f, 0f, 0f);
				inputCameraAngle = Vector3.zero;
				ballUI.FadeOutBallMoveArrow(0f);
				throwUI.SetThrowStartUI();
			}
			if (ball.ThrowAnimAnchor != null)
			{
				ball.ThrowAnimAnchor.SetLocalEulerAnglesX(0f);
			}
			break;
		}
		case Bowling_Define.OperationState.VECTOR_SELECT:
		{
			float num = 0f;
			float num2 = Vector3.Angle(Vector3.down, new Vector3(stickHorizontal, stickVertical, 0f));
			if (stickHorizontal < 0f)
			{
				num2 *= -1f;
			}
			num2 *= 0.3f;
			if (num2 < num - PULL_MAX_ANGLE)
			{
				num2 = num - PULL_MAX_ANGLE;
			}
			else if (num2 > num + PULL_MAX_ANGLE)
			{
				num2 = num + PULL_MAX_ANGLE;
			}
			Vector3 vector = new Vector3(Mathf.Sin(num2 * ((float)Math.PI / 180f)), Mathf.Cos(num2 * ((float)Math.PI / 180f)) * -1f, 0f);
			float num3 = CalcManager.Length(Vector2.zero, new Vector2(stickHorizontal, stickVertical)) * PULL_MAX_LENGTH;
			if (num3 > PULL_MAX_LENGTH)
			{
				num3 = PULL_MAX_LENGTH;
			}
			else if (num3 < PULL_MIN_LENGTH)
			{
				num3 = PULL_MIN_LENGTH;
			}
			float x = 0f - vector.x;
			float z = 0f - vector.y;
			throwData.throwVec.x = x;
			throwData.throwVec.y = 0f;
			throwData.throwVec.z = z;
			throwData.touchLengthPer = num3 / PULL_MAX_LENGTH;
			if (CalcManager.Length(Vector2.zero, new Vector2(stickHorizontal, stickVertical)) * PULL_MAX_LENGTH > PULL_MIN_LENGTH)
			{
				throwUI.SetActiveBallArrow(_value: true);
				throwUI.BallArrowSetting(num3, vector, PULL_MAX_LENGTH, PULL_MIN_LENGTH);
				if (num2 > 40f || num2 < -40f)
				{
					stickHorizontal = 0f;
					stickVertical = 0f;
					num3 = 0f;
					num2 = 0f;
					vector = new Vector3(Mathf.Sin(num2 * ((float)Math.PI / 180f)), Mathf.Cos(num2 * ((float)Math.PI / 180f)) * -1f, 0f);
					throwUI.SetActiveBallArrow(_value: false);
				}
				else
				{
					throwUI.SetActiveBallArrow(_value: true);
				}
			}
			else
			{
				throwUI.SetActiveBallArrow(_value: false);
				num3 = 0f;
				num2 = 0f;
				vector = new Vector3(Mathf.Sin(num2 * ((float)Math.PI / 180f)), Mathf.Cos(num2 * ((float)Math.PI / 180f)) * -1f, 0f);
			}
			if (ball.ThrowAnimAnchor != null)
			{
				float num4 = (num3 - PULL_MIN_LENGTH) * (60f / (PULL_MAX_LENGTH - PULL_MIN_LENGTH));
				if (num4 < 0f)
				{
					num4 = 0f;
				}
				ball.ThrowAnimAnchor.SetLocalEulerAnglesX(num4);
				ball.ThrowAnimAnchor.SetLocalEulerAnglesY(CalcManager.AngleUsingVec(vector, Vector3.zero) * -1f);
			}
			bool flag = false;
			if (!Bowling_Define.CM.IsPushButton_A(userType, Bowling_ControllerManager.ButtonPushType.HOLD) && !Bowling_Define.CM.IsPushButton_X(userType, Bowling_ControllerManager.ButtonPushType.HOLD) && !Bowling_Define.CM.IsPushButton_Y(userType, Bowling_ControllerManager.ButtonPushType.HOLD))
			{
				flag = true;
			}
			if (!flag)
			{
				break;
			}
			if (CalcManager.Length(Vector2.zero, new Vector2(stickHorizontal, stickVertical)) * PULL_MAX_LENGTH > PULL_MIN_LENGTH)
			{
				operationState = Bowling_Define.OperationState.NONE;
				throwData.throwVec = CalcThrowVec(throwData.throwVec);
				if (ball.ThrowAnimAnchor != null)
				{
					Transform throwAnimAnchor = ball.ThrowAnimAnchor;
					LeanTween.rotateLocal(throwAnimAnchor.gameObject, new Vector3(-40f, throwAnimAnchor.localEulerAngles.y, throwAnimAnchor.localEulerAngles.z), 0.1f).setOnComplete((Action)delegate
					{
						BallThrow(throwData.throwVec, throwData.touchLengthPer, throwData.throwType);
					});
				}
				else
				{
					BallThrow(throwData.throwVec, throwData.touchLengthPer, throwData.throwType);
				}
				throwUI.SetActiveThrowArrow(_active: false);
				ballUI.SetActiveThrowArrow(_value: false);
				throwUI.SetActiveBallArrow(_value: false);
			}
			else
			{
				operationState = Bowling_Define.OperationState.BALL_MOVE;
				ballUI.FadeInBallMoveArrow(0f);
			}
			break;
		}
		}
	}
	public void SetThrowBallReady()
	{
		ball.BallStandby();
		if (!isPlayer)
		{
			operationState = Bowling_Define.OperationState.NONE;
			ai.Throw();
			throwUI.SetActiveThrowArrow(_active: true);
			ballUI.SetActiveThrowArrow(_value: false);
		}
		else
		{
			operationState = Bowling_Define.OperationState.BALL_MOVE;
			throwUI.SetActiveThrowArrow(_active: true);
			ballUI.SetActiveThrowArrow(_value: true);
			ballUI.FadeInBallMoveArrow(0f);
		}
	}
	public void BallThrow(Vector3 _vec, float _speed, Bowling_Define.ThrowType _throwType)
	{
		operationState = Bowling_Define.OperationState.THROW;
		Bowling_Define.MSM.SetCameraMoveStartData();
		ball.Throw(_vec, _speed, _throwType);
	}
	public void TimeUpThrow()
	{
		operationState = Bowling_Define.OperationState.THROW;
		throwData.throwVec = Vector3.forward;
		throwData.throwVec = CalcManager.PosRotation2D(throwData.throwVec, CalcManager.mVector3Zero, UnityEngine.Random.Range(-2f, 2f), CalcManager.AXIS.Y);
		throwData.touchLengthPer = 1f;
		throwData.throwType = Bowling_Define.ThrowType.STRAIGHT;
		Bowling_Define.MPM.GetNowThrowUser().BallThrow(throwData.throwVec, throwData.touchLengthPer, throwData.throwType);
	}
	public void SkipCPU()
	{
		ai.IsSkipMode = true;
		operationState = Bowling_Define.OperationState.THROW;
	}
	public void SkipCPUThrow()
	{
		ai.SkipThrow();
	}
	public Bowling_Ball GetBall()
	{
		return ball;
	}
	private void MoveCameraAngle()
	{
		if (Bowling_Define.PLAYER_NUM != 1)
		{
			return;
		}
		if (operationState == Bowling_Define.OperationState.BALL_MOVE)
		{
			if (Bowling_Define.CM.GetRStickDir(userType).magnitude >= AXIS_MIN_DIR)
			{
				inputCameraAngle.y += Bowling_Define.CM.GetRStickDir(userType).x * CAMERA_ANGLE_SPEED;
				inputCameraAngle.x += (0f - Bowling_Define.CM.GetRStickDir(userType).y) * CAMERA_ANGLE_SPEED;
			}
			if (inputCameraAngle.y > 5f || inputCameraAngle.y < -5f)
			{
				inputCameraAngle.y = Mathf.Clamp(inputCameraAngle.y, -5f, 5f);
			}
			if (inputCameraAngle.x > 5f || inputCameraAngle.x < -5f)
			{
				inputCameraAngle.x = Mathf.Clamp(inputCameraAngle.x, -5f, 5f);
			}
			if (Bowling_Define.CM.IsPushButton_StickR(userType, Bowling_ControllerManager.ButtonPushType.DOWN))
			{
				inputCameraAngle = Vector3.zero;
			}
			LeanTween.cancel(Bowling_Define.MSM.GetCameraRoot.gameObject);
			if ((Bowling_Define.MSM.GetCameraRoot.rotation.eulerAngles - (Vector3.zero + inputCameraAngle)).sqrMagnitude > 0.01f)
			{
				LeanTween.rotateLocal(Bowling_Define.MSM.GetCameraRoot.gameObject, Vector3.zero + inputCameraAngle, 0.55f).setEaseOutCubic();
			}
		}
		else
		{
			LeanTween.cancel(Bowling_Define.MSM.GetCameraRoot.gameObject);
			Bowling_Define.MSM.GetCameraRoot.SetLocalEulerAngles(0f, 0f, 0f);
		}
	}
	private Vector3 CalcThrowVec(Vector3 _throwVec)
	{
		Vector3 vector = _throwVec;
		vector.y = vector.z;
		vector.z = 0f;
		Vector3 vector2 = ball.GetBallPos();
		if (ball.ThrowAnimAnchor != null)
		{
			vector2 = ball.ThrowAnimAnchor.position;
		}
		Vector3 vector3 = Bowling_Define.MSM.StageCamera.WorldToScreenPoint(vector2) + vector * throwUI.GetLaneLengthToScreen();
		Vector2 screenPos = default(Vector2);
		screenPos.x = vector3.x;
		screenPos.y = vector3.y;
		Ray ray = RectTransformUtility.ScreenPointToRay(Bowling_Define.MSM.StageCamera, screenPos);
		Vector3 a = vector2 + Vector3.forward;
		if (Physics.Raycast(ray, out RaycastHit hitInfo, 50f, 1 << LayerMask.NameToLayer(Bowling_Define.LAYER_NO_HIT)))
		{
			a = hitInfo.point;
		}
		a.y = vector2.y;
		return (a - vector2).normalized;
	}
	public void ShowPlayerBall()
	{
		ball.gameObject.SetActive(value: true);
	}
	public void HidePlayerBall()
	{
		ball.gameObject.SetActive(value: false);
	}
}
