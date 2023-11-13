using UnityEngine;
public class SwordFight_TimeLimitGauge : MonoBehaviour
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
		showPosOffset[0] = 60f;
		showPosOffset[0] = 60f;
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
			gauge.size = new Vector2(timeLimitNow / timeLimit, gauge.size.y);
		}
	}
	private void UpdateGaugePos()
	{
	}
	private void SetGaugePos(Vector3 _charaPos, float _offset)
	{
		Vector3 position = SingletonCustom<SwordFight_FieldManager>.Instance.Get3dCamera().WorldToScreenPoint(_charaPos);
		Vector3 vector = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(position);
		base.transform.SetPosition(vector.x, vector.y + _offset, base.transform.position.z);
		charaPos = _charaPos;
	}
}
