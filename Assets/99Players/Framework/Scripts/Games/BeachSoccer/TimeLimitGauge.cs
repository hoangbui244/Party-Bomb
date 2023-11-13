using UnityEngine;
namespace BeachSoccer
{
	public class TimeLimitGauge : MonoBehaviour
	{
		public enum TIME_LIMIT_TYPE
		{
			KICK_OFF,
			PUNT_KICK,
			GOAL_KICK,
			THROW_IN,
			CORNER_KICK
		}
		private TIME_LIMIT_TYPE timeLimitType;
		[SerializeField]
		[Header("ゲ\u30fcジ")]
		private SpriteRenderer gauge;
		private bool isEffective;
		private bool isFinish;
		private float timeLimit;
		private float timeLimitNow;
		[SerializeField]
		[Header("オブジェクト")]
		private GameObject obj;
		private Vector3 charaPos;
		private int teamNo;
		private float[] showPosOffset = new float[2];
		public bool IsFinish(int _teamNo)
		{
			if (isFinish && isEffective)
			{
				return teamNo == _teamNo;
			}
			return false;
		}
		private void Start()
		{
			if (GameSaveData.CheckSelectMainGameMode(GameSaveData.MainGameMode.MULTI))
			{
				for (int i = 0; i < showPosOffset.Length; i++)
				{
					showPosOffset[i] = 80f;
				}
			}
			else
			{
				showPosOffset[0] = 122f;
				showPosOffset[1] = 80f;
			}
		}
		public void TimeStart(TIME_LIMIT_TYPE _timeLimitType, int _teamNo, float _time)
		{
			timeLimitType = _timeLimitType;
			teamNo = _teamNo;
			obj.SetActive(value: true);
			timeLimit = (timeLimitNow = _time);
			isFinish = false;
			isEffective = true;
		}
		public void Finish()
		{
			isEffective = false;
			obj.SetActive(value: false);
		}
		private void Update()
		{
			if (isEffective)
			{
				UpdateGaugePos();
				timeLimitNow -= Time.deltaTime;
				if (timeLimitNow < 0f)
				{
					timeLimitNow = 0f;
					isFinish = true;
				}
				gauge.transform.SetLocalScaleX(timeLimitNow / timeLimit);
			}
		}
		private void UpdateGaugePos()
		{
			if ((!GameSaveData.CheckSelectMainGameMode(GameSaveData.MainGameMode.MULTI) && SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara() != null && SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara().TeamNo == 1) || MainCharacterManager.IsGameWatchingMode())
			{
				obj.SetActive(value: false);
			}
			if (SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara() == null)
			{
				obj.SetActive(value: false);
				return;
			}
			bool flag = GameSaveData.CheckSelectCameraMode(GameSaveData.CameraMode.VERTICAL);
			switch (timeLimitType)
			{
			case TIME_LIMIT_TYPE.KICK_OFF:
				SetGaugePos(SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara().GetPos(), (SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara().TeamNo != 0) ? (0f - showPosOffset[1]) : (flag ? showPosOffset[0] : (0f - showPosOffset[1])));
				break;
			case TIME_LIMIT_TYPE.THROW_IN:
				SetGaugePos(SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara().GetPos(), (SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara().TeamNo != 0) ? (0f - showPosOffset[1]) : (flag ? (showPosOffset[0] * 1.2f) : (0f - showPosOffset[1])));
				break;
			case TIME_LIMIT_TYPE.GOAL_KICK:
				SetGaugePos(SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara().GetPos(), (SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara().TeamNo != 0) ? ((0f - showPosOffset[1]) * 0.8f) : (flag ? (showPosOffset[0] * 0.8f) : ((0f - showPosOffset[1]) * 0.8f)));
				break;
			case TIME_LIMIT_TYPE.CORNER_KICK:
				SetGaugePos(SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara().GetPos(), (SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara().TeamNo != 0) ? ((0f - showPosOffset[1]) * 0.8f) : (flag ? (showPosOffset[0] * 0.8f) : ((0f - showPosOffset[1]) * 0.8f)));
				break;
			case TIME_LIMIT_TYPE.PUNT_KICK:
				SetGaugePos(SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara().GetPos(), (SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara().TeamNo != 0) ? (0f - showPosOffset[1]) : (flag ? (showPosOffset[0] * 1.1f) : (0f - showPosOffset[1])));
				break;
			}
		}
		private void SetGaugePos(Vector3 _charaPos, float _offset)
		{
			Vector3 position = SingletonCustom<FieldManager>.Instance.Get3dCamera().WorldToScreenPoint(_charaPos);
			Vector3 vector = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(position);
			base.transform.SetPosition(vector.x, vector.y + _offset, base.transform.position.z);
			charaPos = _charaPos;
		}
	}
}
