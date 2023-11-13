using System;
using UnityEngine;
public class Golf_PredictionBall : Golf_Ball
{
	[Serializable]
	private struct Prediction
	{
		public PredictionType predictionType;
		public Vector3 rotDir;
		[HideInInspector]
		public Vector3 diffVec;
	}
	public enum PredictionType
	{
		Wind,
		Rot
	}
	[SerializeField]
	[Header("予測する時間")]
	private float predictTime;
	[SerializeField]
	[Header("fixedDeltaTime補正")]
	private float fixedDeltaTime_Diff;
	[SerializeField]
	[Header("予測情報")]
	private Prediction[] arrayPrediction;
	private PredictionType currentPredictionType;
	private bool isCheckExit;
	public override void Init(int _playerNo = -1)
	{
		base.Init(_playerNo);
		mesh.enabled = false;
		base.gameObject.SetActive(value: false);
	}
	public override void InitPlay()
	{
		Physics.autoSimulation = false;
		UnityEngine.Debug.Log("ボ\u30fcルを飛ばす ");
		UnityEngine.Debug.DrawRay(SingletonCustom<Golf_FieldManager>.Instance.GetReadyBallPos(), SingletonCustom<Golf_FieldManager>.Instance.GetHole().GetReadyBallPosToCupVec(), Color.black, 15f);
		for (int i = 0; i < arrayPrediction.Length; i++)
		{
			isCheckExit = false;
			currentPredictionType = (PredictionType)i;
			base.InitPlay();
			Golf_Player turnPlayer = SingletonCustom<Golf_PlayerManager>.Instance.GetTurnPlayer();
			float shotPower = SingletonCustom<Golf_PlayerManager>.Instance.GetBaseShotPower() * SingletonCustom<Golf_UIManager>.Instance.GetCupPowerLerp();
			SetShotPower(shotPower);
			Vector3 a = UnityEngine.Random.insideUnitCircle;
			Vector3 b = a * (SingletonCustom<Golf_PlayerManager>.Instance.GetMaxShotImpactDiff() * SingletonCustom<Golf_UIManager>.Instance.GetCupImpactLerp());
			SetShotVec((Quaternion.Euler(new Vector3(SingletonCustom<Golf_PlayerManager>.Instance.GetShotAngle(), 0f, 0f) + b) * -turnPlayer.transform.right).normalized);
			SetRotDir(arrayPrediction[i].rotDir + a * SingletonCustom<Golf_UIManager>.Instance.GetCupImpactLerp());
			Shot();
			float num = 0f;
			while (num < predictTime && !isCheckExit)
			{
				Physics.Simulate(Time.fixedUnscaledDeltaTime * fixedDeltaTime_Diff);
				num += Time.fixedUnscaledDeltaTime * fixedDeltaTime_Diff;
				if (!isWaitUpdate && num > 0.5f)
				{
					isWaitUpdate = true;
				}
				SetRotForce();
			}
			UnityEngine.Debug.Log(base.transform.position);
			Vector3 readyBallPos = SingletonCustom<Golf_FieldManager>.Instance.GetReadyBallPos();
			Vector3 from = base.transform.position - readyBallPos;
			from.y = 0f;
			Vector3 readyBallPosToCupVec = SingletonCustom<Golf_FieldManager>.Instance.GetHole().GetReadyBallPosToCupVec();
			float f = (Mathf.Atan2(readyBallPosToCupVec.z, readyBallPosToCupVec.x) * 57.29578f + Vector3.Angle(from, readyBallPosToCupVec)) * ((float)Math.PI / 180f);
			arrayPrediction[i].diffVec = new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f));
			UnityEngine.Debug.DrawRay(SingletonCustom<Golf_FieldManager>.Instance.GetReadyBallPos(), arrayPrediction[i].diffVec * SingletonCustom<Golf_FieldManager>.Instance.GetHole().GetReadyBallPosToCupVec().magnitude, (i == 0) ? Color.red : Color.cyan, 15f);
		}
		UnityEngine.Debug.Log("ボ\u30fcルを飛ばす処理終了 ");
		base.gameObject.SetActive(value: false);
		Physics.autoSimulation = true;
	}
	public Vector3 GetDiffVec(PredictionType _predictionType)
	{
		return arrayPrediction[(int)_predictionType].diffVec;
	}
	public PredictionType GetCurrentPredictionType()
	{
		return currentPredictionType;
	}
	public void SetIsCheckExit()
	{
		isCheckExit = true;
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (isWaitUpdate && collision.gameObject.layer == LayerMask.NameToLayer("BackGround"))
		{
			SetIsCheckExit();
		}
	}
}
