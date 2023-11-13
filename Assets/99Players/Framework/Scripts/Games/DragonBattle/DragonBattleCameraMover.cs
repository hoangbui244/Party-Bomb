using UnityEngine;
public class DragonBattleCameraMover : SingletonCustom<DragonBattleCameraMover>
{
	public enum State
	{
		StartWait,
		InGame,
		EndGame,
		BossBattle
	}
	public struct DefData
	{
		public Vector3 localPos;
		public Vector3 localPosCamera;
		public Quaternion localRotCamera;
		public float fieldOfView;
	}
	[SerializeField]
	[Header("カメラ")]
	private Camera cameraObj;
	private float MOVE_SPEED = 3.5f;
	private float END_POINT_Z = 402.7f;
	private float CHECK_SPEED_UP = 12f;
	private State currentState;
	private float currentSpeed;
	private float addSpeed;
	private int fieldStep;
	private DefData defData;
	[SerializeField]
	private Transform deathAnchor;
	[SerializeField]
	private Transform respawnAnchor;
	private float CameraWorkSpeed = 3f;
	[SerializeField]
	private Transform[] viewCenterAnchor;
	public float DistanceScale => (END_POINT_Z - 16.7f - base.transform.localPosition.z) / END_POINT_Z;
	public Transform DeathAnchor => deathAnchor;
	public Transform RespawnAnchor => respawnAnchor;
	public Transform[] ViewCenterAnchor => viewCenterAnchor;
	public void Init()
	{
		currentSpeed = 0f;
		fieldStep = 0;
		defData.localPos = base.transform.localPosition;
		defData.localRotCamera = cameraObj.transform.localRotation;
		defData.localPosCamera = cameraObj.transform.localPosition;
		defData.fieldOfView = cameraObj.fieldOfView;
	}
	public Transform GetAnchor()
	{
		return base.transform;
	}
	public Camera GetCamera()
	{
		return cameraObj;
	}
	public void OnGameStart()
	{
		SetState(State.InGame);
	}
	public void StartBossBattle()
	{
		SetState(State.BossBattle);
	}
	public void FinishBossBattle()
	{
		SetState(State.InGame);
	}
	public void Update()
	{
		State state = currentState;
		if ((uint)(state - 1) <= 2u)
		{
			if (currentSpeed < MOVE_SPEED)
			{
				currentSpeed = Mathf.Clamp(currentSpeed += Time.deltaTime * 4f, 0f, MOVE_SPEED);
			}
			if (base.transform.localPosition.z < SingletonCustom<DragonBattlePlayerManager>.Instance.GetEndLineCharacterLocalZ() - CHECK_SPEED_UP)
			{
				addSpeed = Mathf.Clamp(addSpeed + Time.deltaTime * 1.5f, 0f, MOVE_SPEED * 0.4f);
			}
			else
			{
				addSpeed = Mathf.Clamp(addSpeed - Time.deltaTime * 2.5f, 0f, MOVE_SPEED * 0.4f);
			}
		}
	}
	public void LateUpdate()
	{
		switch (currentState)
		{
		case State.BossBattle:
			if (base.transform.position.z < SingletonCustom<DragonBattleFieldManager>.Instance.NowParts.transform.position.z - 15f)
			{
				base.transform.AddLocalPositionZ((currentSpeed + addSpeed) * Time.deltaTime);
			}
			else
			{
				SingletonCustom<DragonBattleUIManager>.Instance.ShowBossHp(delegate
				{
					SingletonCustom<DragonBattlePlayerManager>.Instance.IsProhibitAttack = false;
					SingletonCustom<DragonBattleFieldManager>.Instance.NowParts.EnemyActive();
				});
			}
			if (SingletonCustom<DragonBattleUIManager>.Instance.IsShowBossHp())
			{
				cameraObj.fieldOfView = Mathf.Lerp(cameraObj.fieldOfView, defData.fieldOfView - 5f, CameraWorkSpeed * Time.deltaTime);
				cameraObj.transform.SetLocalEulerAnglesX(Mathf.Lerp(cameraObj.transform.localEulerAngles.x, defData.localRotCamera.eulerAngles.x - 10f, CameraWorkSpeed * Time.deltaTime));
				cameraObj.transform.SetLocalPositionY(Mathf.Lerp(cameraObj.transform.localPosition.y, defData.localPosCamera.y - 3f, CameraWorkSpeed * Time.deltaTime));
			}
			break;
		case State.InGame:
			if (base.transform.localPosition.z < END_POINT_Z)
			{
				if (DistanceScale <= 0.1f && !SingletonCustom<DragonBattleUIManager>.Instance.IsHideScore)
				{
					SingletonCustom<DragonBattleUIManager>.Instance.HideScore();
				}
				CheckFieldStep();
				base.transform.AddLocalPositionZ((currentSpeed + addSpeed) * Time.deltaTime);
			}
			else if (SingletonCustom<DragonBattlePlayerManager>.Instance.IsAllPlayerGoal())
			{
				currentState = State.EndGame;
			}
			cameraObj.fieldOfView = Mathf.Lerp(cameraObj.fieldOfView, defData.fieldOfView, CameraWorkSpeed * Time.deltaTime);
			cameraObj.transform.SetLocalEulerAnglesX(Mathf.Lerp(cameraObj.transform.localEulerAngles.x, defData.localRotCamera.eulerAngles.x, CameraWorkSpeed * Time.deltaTime));
			cameraObj.transform.SetLocalPositionY(Mathf.Lerp(cameraObj.transform.localPosition.y, defData.localPosCamera.y, CameraWorkSpeed * Time.deltaTime));
			break;
		case State.EndGame:
			base.transform.transform.SetLocalPositionZ(Mathf.Lerp(base.transform.transform.localPosition.z, END_POINT_Z + 13f, CameraWorkSpeed * Time.deltaTime));
			cameraObj.transform.SetLocalPositionY(Mathf.Lerp(cameraObj.transform.localPosition.y, defData.localPosCamera.y - 7.5f, CameraWorkSpeed * Time.deltaTime));
			break;
		}
	}
	private void CheckFieldStep()
	{
		switch (fieldStep)
		{
		case 0:
			if (base.transform.localPosition.z > 89f)
			{
				fieldStep++;
			}
			break;
		case 1:
			base.transform.SetLocalPositionY(Mathf.SmoothStep(base.transform.localPosition.y, defData.localPos.y + 0f, 0.05f));
			if (base.transform.localPosition.z > 184.7f)
			{
				fieldStep++;
			}
			break;
		case 2:
			base.transform.SetLocalPositionY(Mathf.SmoothStep(base.transform.localPosition.y, defData.localPos.y + 0f, 0.05f));
			if (base.transform.localPosition.z > 280.4f)
			{
				fieldStep++;
			}
			break;
		case 3:
			base.transform.SetLocalPositionY(Mathf.SmoothStep(base.transform.localPosition.y, defData.localPos.y + 0f, 0.05f));
			break;
		}
	}
	private void SetState(State _state)
	{
		currentState = _state;
	}
	public bool CheckState(State _state)
	{
		return currentState == _state;
	}
	public void SetEndPointZ(float _value)
	{
		END_POINT_Z = _value;
	}
	public void DebugGoal()
	{
		base.transform.transform.SetLocalPositionZ(END_POINT_Z);
		SetState(State.InGame);
	}
}
