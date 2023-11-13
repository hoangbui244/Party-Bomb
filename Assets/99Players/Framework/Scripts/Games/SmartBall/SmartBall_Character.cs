using System;
using System.Collections.Generic;
using UnityEngine;
public class SmartBall_Character : MonoBehaviour
{
	public enum ActionState
	{
		Shot,
		PointUpdate,
		BallStandby
	}
	[SerializeField]
	[Header("AI")]
	private SmartBall_Al charaAI;
	[SerializeField]
	[Header("カメラアンカ\u30fc")]
	private Transform cameraAnchor;
	[SerializeField]
	[Header("操作する台が映るカメラ")]
	private Camera controleCamera;
	private SB.UserType userType;
	private int dataNo;
	private bool isPlayer;
	private ActionState currentActionState = ActionState.BallStandby;
	private int point;
	private int defaultUserPoint;
	private float gameOverTime;
	private readonly float GAME_INTERVAL = 5f;
	private float shotPower = 1f;
	private bool isShot;
	private bool nowShot;
	public readonly float BASE_SHOT_POWER = 9.2f;
	private readonly float MAX_SHOT_POWER = 1.7f;
	private readonly float MIN_SHOT_POWER = 1f;
	private readonly float MAX_ADD_POWER = 0.02f;
	private readonly float JUDG_SHOT_POWER = 3f;
	private readonly float POWER_CHARGE_SPEED = 0.5f;
	[SerializeField]
	[Header("ストック用ボ\u30fcルオブジェクト")]
	private GameObject ballObj;
	[SerializeField]
	[Header("ストック用ボ\u30fcルアンカ\u30fc")]
	private Transform ballObjAnchor;
	[SerializeField]
	[Header("ストック用ボ\u30fcルのマテリアル")]
	private Material[] stockBallMats;
	private List<SmartBall_BallObject> createBallList = new List<SmartBall_BallObject>();
	private List<SmartBall_BallObject> holeInBallList = new List<SmartBall_BallObject>();
	private List<GameObject> stockBallList = new List<GameObject>();
	private SmartBall_BallObject nowShotBall;
	private readonly float CREATE_INTERVAL = 0.3f;
	private float createTime;
	private readonly int USE_BALL_NUM = 20;
	private int hasBallNum = 20;
	[SerializeField]
	[Header("台生成位置アンカ\u30fc")]
	private Transform standGenerateAnchor;
	[SerializeField]
	[Header("無人の台のアンカ\u30fc")]
	private Transform[] noPlayerStandAnchor;
	private SmartBall_StandObject createPlayStand;
	private Transform ballShotStartAnchor;
	private Transform ballStockAnchor;
	private readonly float STAND_INCLINATION = 1f;
	private float aiChageTime;
	public ActionState CurrentActionState => currentActionState;
	public bool IsShot => isShot;
	public int HasBallNum => hasBallNum;
	public void Init(int _dataNo, SB.UserType _userType, bool _isPlayer)
	{
		dataNo = _dataNo;
		userType = _userType;
		isPlayer = _isPlayer;
		hasBallNum = USE_BALL_NUM;
		CreateStand();
		CreateNoPlayerStand();
		CreateStockBall();
		if (_userType > SB.UserType.PLAYER_4)
		{
			charaAI.Init();
		}
		BallStandbyProcess();
	}
	public void UpdateMethod()
	{
		if (createPlayStand.Gimmicks.Length >= 0)
		{
			createPlayStand.UpdateMethod();
		}
		if (createBallList.Count > 0)
		{
			AddPoint();
		}
		if (hasBallNum <= 0)
		{
			gameOverTime += Time.deltaTime;
			if (createBallList.Count <= 0 || gameOverTime > GAME_INTERVAL)
			{
				SB.MCM.SetTeamFailed(dataNo);
			}
			return;
		}
		switch (currentActionState)
		{
		case ActionState.Shot:
			ShotProcess();
			break;
		case ActionState.BallStandby:
			BallStandbyProcess();
			break;
		}
		if (!isPlayer)
		{
			charaAI.UpdateMethod();
		}
	}
	private void ShotProcess()
	{
		if (isShot)
		{
			createTime += Time.deltaTime;
			if (createTime >= CREATE_INTERVAL)
			{
				hasBallNum--;
				currentActionState = ActionState.BallStandby;
				createTime = 0f;
			}
			return;
		}
		if (isPlayer && createPlayStand.CheckSetBall())
		{
			if (SB.CM.IsButton_A((int)userType, SmartBall_ControllerManager.ButtonStateType.HOLD) && !nowShot)
			{
				createPlayStand.PullShotStick();
				ChargeShotPower();
			}
			if (SB.CM.IsButton_A((int)userType, SmartBall_ControllerManager.ButtonStateType.UP))
			{
				nowShot = true;
				StartCoroutine(createPlayStand._StopperMove());
				createPlayStand.BallShotStick();
				LeanTween.delayedCall(createPlayStand.GetStickShotTime(), (Action)delegate
				{
					BallShot();
					nowShot = false;
				});
			}
		}
		if (createBallList[createBallList.Count - 1].Shot && !isShot)
		{
			isShot = true;
		}
	}
	public void BallStandbyProcess()
	{
		PreparationBall();
		isShot = false;
		currentActionState = ActionState.Shot;
	}
	public void ChargeShotPower()
	{
		shotPower += POWER_CHARGE_SPEED * Time.deltaTime;
		shotPower = Mathf.Clamp(shotPower, MIN_SHOT_POWER, MAX_SHOT_POWER);
	}
	public void BallShot()
	{
		if (createPlayStand.CheckSetBall())
		{
			SmartBall_BallObject shotBall = createPlayStand.GetShotBall();
			shotBall.transform.parent = ballObjAnchor;
			float num = UnityEngine.Random.Range(0f, MAX_ADD_POWER);
			shotBall.BallAddFourse(BASE_SHOT_POWER * shotPower + num);
			shotPower = 1f;
		}
	}
	private void CreateStand()
	{
		createPlayStand = UnityEngine.Object.Instantiate(SB.MCM.GetCreateStand(), Vector3.zero, Quaternion.identity, standGenerateAnchor);
		createPlayStand.Init(isPlayer);
		createPlayStand.gameObject.SetActive(value: true);
		createPlayStand.transform.localPosition = Vector3.zero;
		createPlayStand.transform.localEulerAngles = Vector3.zero;
		createPlayStand.transform.SetLocalEulerAnglesX(STAND_INCLINATION);
		ballStockAnchor = createPlayStand.BallGenerateAnchor;
		ballShotStartAnchor = createPlayStand.BallShotAnchor;
	}
	private void CreateNoPlayerStand()
	{
		for (int i = 0; i < noPlayerStandAnchor.Length; i++)
		{
			SmartBall_StandObject smartBall_StandObject = UnityEngine.Object.Instantiate(SB.MCM.GetCreateStand(), Vector3.zero, Quaternion.identity, noPlayerStandAnchor[i]);
			smartBall_StandObject.transform.localPosition = Vector3.zero;
			smartBall_StandObject.transform.SetLocalEulerAnglesY(180f);
			smartBall_StandObject.transform.SetLocalEulerAnglesX(STAND_INCLINATION);
		}
	}
	private void CreateStockBall()
	{
		for (int i = 0; i < hasBallNum; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(ballObj, Vector3.zero, Quaternion.identity, ballStockAnchor);
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0.35f * (float)i);
			gameObject.GetComponent<MeshRenderer>().material = stockBallMats[SB.MCM.CreateStandTypeNo];
			stockBallList.Add(gameObject);
		}
	}
	public void PreparationBall()
	{
		GameObject gameObject = stockBallList[stockBallList.Count - 1];
		stockBallList.RemoveAt(stockBallList.Count - 1);
		gameObject.SetActive(value: false);
		nowShotBall = UnityEngine.Object.Instantiate(SB.MCM.GetCreateBall(), Vector3.zero, Quaternion.identity, ballShotStartAnchor);
		nowShotBall.gameObject.SetActive(value: true);
		nowShotBall.transform.parent = createPlayStand.GetStickObj().transform;
		nowShotBall.Init(dataNo, SB.MCM.CreateStandTypeNo, isPlayer);
		createBallList.Add(nowShotBall);
	}
	private void AddPoint()
	{
		for (int i = 0; i < createBallList.Count; i++)
		{
			if (!createBallList[i].AddScore && createBallList[i].HoleIn)
			{
				point += createBallList[i].GetBallHasPoint();
				if (point > 0 && isPlayer)
				{
					SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_good");
				}
				SB.MCM.SetPoint(dataNo, point);
				SmartBall_BallObject smartBall_BallObject = createBallList[i];
				createBallList.Remove(createBallList[i]);
				smartBall_BallObject.BallHoleIn();
			}
		}
	}
	public bool CheckSetBall()
	{
		return createPlayStand.CheckSetBall();
	}
	public SmartBall_StandObject GetCharaStand()
	{
		return createPlayStand;
	}
	public List<SmartBall_BallObject> GetHoleInBallList()
	{
		return holeInBallList;
	}
	public int GetCharaPoint()
	{
		return point;
	}
	public Camera GetControleCamera()
	{
		return controleCamera;
	}
}
