using UnityEngine;
public class Golf_BallManager : SingletonCustom<Golf_BallManager>
{
	[SerializeField]
	[Header("ボ\u30fcル")]
	private Golf_Ball[] arrayBall;
	[SerializeField]
	[Header("ボ\u30fcルのマテリアル")]
	private Material[] arrayMaterial;
	[SerializeField]
	[Header("ボ\u30fcルのTrailのマテリアル")]
	private Material[] arrayTrailMaterial;
	[SerializeField]
	[Header("ボ\u30fcルのTrailのStartColor")]
	private Color[] arrayTrailColor;
	[SerializeField]
	[Header("ボ\u30fcルのTrailのEndColor")]
	private Color trailEndColor;
	[SerializeField]
	[Header("予測ボ\u30fcルラインクラス")]
	private Golf_PredictionBallLine predictionBallLine;
	[SerializeField]
	[Header("予測ボ\u30fcルクラス")]
	private Golf_PredictionBall predictionBall;
	[SerializeField]
	[Header("地面接触時の空気抵抗")]
	private float GROUND_COLLISION_DRAG;
	[SerializeField]
	[Header("ボ\u30fcルが停止する速度")]
	private float BALL_STOP_MAGNITUDE;
	[SerializeField]
	[Header("ボ\u30fcルを回転させた時の飛んでいる時の影響力")]
	private float BALL_ROT_VELOCITY_FORCE;
	[SerializeField]
	[Header("ボ\u30fcルをトップスピンさせた時の地面に接触した時の影響力")]
	private float BALL_ROT_GROUND_FORWARD_FORCE;
	[SerializeField]
	[Header("ボ\u30fcルをバックスピンさせた時の地面に接触した時の影響力")]
	private float BALL_ROT_GROUND_BACK_FORCE;
	private float remainingDistanceToCup;
	public void Init()
	{
		for (int i = 0; i < arrayBall.Length; i++)
		{
			arrayBall[i].Init(i);
			int num = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[i][0];
			int num2 = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[num];
			arrayBall[i].SetMaterial(arrayMaterial[num2]);
			arrayBall[i].SetTrailMaterial(arrayTrailColor[num2], trailEndColor);
		}
		predictionBallLine.Init();
		predictionBall.Init(-1);
		InitPlay();
	}
	public void InitPlay()
	{
		for (int i = 0; i < arrayBall.Length; i++)
		{
			if (i == SingletonCustom<Golf_PlayerManager>.Instance.GetTurnPlayer().GetPlayerNo())
			{
				arrayBall[i].InitPlay();
			}
			else
			{
				arrayBall[i].SetAudience();
			}
		}
		if (SingletonCustom<Golf_PlayerManager>.Instance.GetTurnPlayer().GetIsCpu())
		{
			predictionBall.InitPlay();
		}
	}
	public void InitPlayPredictionBallLine()
	{
		predictionBallLine.InitPlay();
	}
	public void FixedUpdateMethod()
	{
		if (SingletonCustom<Golf_GameManager>.Instance.GetState() == Golf_GameManager.State.BALL_FLY)
		{
			GetTurnPlayerBall().FixedUpdateMethod();
		}
	}
	public void UpdateMethod()
	{
		Golf_GameManager.State state = SingletonCustom<Golf_GameManager>.Instance.GetState();
		if ((uint)(state - 2) <= 2u)
		{
			predictionBallLine.UpdateMethod();
		}
	}
	public Golf_Ball GetBall(int _playerNo)
	{
		return arrayBall[_playerNo];
	}
	public Golf_Ball GetTurnPlayerBall()
	{
		return arrayBall[SingletonCustom<Golf_PlayerManager>.Instance.GetTurnPlayer().GetPlayerNo()];
	}
	public void SetShotPower(float _shotPower)
	{
		GetTurnPlayerBall().SetShotPower(_shotPower);
	}
	public void SetShotVec(Vector3 _shotVec)
	{
		GetTurnPlayerBall().SetShotVec(_shotVec);
	}
	public void SetRotDir(Vector3 _rotDir)
	{
		GetTurnPlayerBall().SetRotDir(_rotDir);
	}
	public void Shot()
	{
		GetTurnPlayerBall().Shot();
	}
	public void HidePredictionBallLine()
	{
		predictionBallLine.Hide();
	}
	public Golf_PredictionBall GetPredictionBall()
	{
		return predictionBall;
	}
	public float GetDistanceCarry(Vector3 _ballPos)
	{
		Vector3 readyBallPos = SingletonCustom<Golf_FieldManager>.Instance.GetReadyBallPos();
		readyBallPos.y = 0f;
		Vector3 target = _ballPos;
		target.y = 0f;
		return SingletonCustom<Golf_FieldManager>.Instance.GetConversionYardDistance(CalcManager.Length(readyBallPos, target));
	}
	public void SetRemainingDistanceToCup()
	{
		Golf_Hole hole = SingletonCustom<Golf_FieldManager>.Instance.GetHole();
		Vector3 cupPos = hole.GetCupPos();
		cupPos.y = 0f;
		Vector3 position = GetTurnPlayerBall().transform.position;
		position.y = 0f;
		float conversionYardDistance = SingletonCustom<Golf_FieldManager>.Instance.GetConversionYardDistance(CalcManager.Length(cupPos, position));
		remainingDistanceToCup = Mathf.Clamp(conversionYardDistance, 0f, hole.GetReadyBallPosToCupDistance());
	}
	public float GetRemainingDistanceToCup()
	{
		return remainingDistanceToCup;
	}
	public float GetGroundCollisionDrag()
	{
		return GROUND_COLLISION_DRAG;
	}
	public float GetBallStopMagnitude()
	{
		return BALL_STOP_MAGNITUDE;
	}
	public float GetBallRotVelocityForce()
	{
		return BALL_ROT_VELOCITY_FORCE;
	}
	public float GetBallRotGroundForwardForce()
	{
		return BALL_ROT_GROUND_FORWARD_FORCE;
	}
	public float GetBallRotGroundBackForce()
	{
		return BALL_ROT_GROUND_BACK_FORCE;
	}
}
