using UnityEngine;
public class SnowBoard_AI : MonoBehaviour
{
	[SerializeField]
	[Header("プレイヤ\u30fc処理")]
	private SnowBoard_Player player;
	[SerializeField]
	[Header("スキ\u30fc板処理")]
	private SnowBoard_SkiBoard skiBoard;
	private SnowBoard_Define.AiStrength aiStrength;
	private Transform currentCheckPointAnchor;
	private int checkPointIdx;
	private float inputHorizontal;
	private float checkAngle;
	private bool isBranch;
	private float targetCross;
	private bool targetDir;
	private float targetAngle;
	private float targetRndX;
	private float targetRndZ;
	public SnowBoard_SkiBoard SkiBoard => skiBoard;
	public int CurrentCheckPointIdx => checkPointIdx;
	public bool IsReverse
	{
		get;
		set;
	}
	public float TargetCross => targetCross;
	public bool TargetDir => targetDir;
	public float TargetAngle => targetAngle;
	public void Init()
	{
		aiStrength = (SnowBoard_Define.AiStrength)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		checkPointIdx = 0;
		currentCheckPointAnchor = SingletonCustom<SnowBoard_CourseManager>.Instance.GetCheckPointAnchor(checkPointIdx);
		switch (aiStrength)
		{
		case SnowBoard_Define.AiStrength.WEAK:
			checkAngle = 15f;
			break;
		case SnowBoard_Define.AiStrength.COMMON:
			checkAngle = 10f;
			break;
		case SnowBoard_Define.AiStrength.STRONG:
			checkAngle = 10f;
			break;
		}
		targetRndX = UnityEngine.Random.Range(-2f, 2f);
	}
	private void Update()
	{
		if (skiBoard.processType == SnowBoard_SkiBoard.SkiBoardProcessType.SLIDING)
		{
			targetCross = Vector3.Cross(currentCheckPointAnchor.position + currentCheckPointAnchor.forward * targetRndX - base.gameObject.transform.position, skiBoard.characterAnchor_Xrot.transform.forward).y;
			if (targetCross < 0f)
			{
				targetDir = true;
			}
			else
			{
				targetDir = false;
			}
			targetAngle = Vector3.Angle(skiBoard.characterAnchor_Xrot.transform.forward, currentCheckPointAnchor.position - base.gameObject.transform.position);
		}
	}
	public void UpdateMethod()
	{
		switch (aiStrength)
		{
		case SnowBoard_Define.AiStrength.WEAK:
			if (skiBoard.AccelGauge == 1f)
			{
				skiBoard.ActionInput();
			}
			break;
		case SnowBoard_Define.AiStrength.COMMON:
			if (skiBoard.AccelGauge == 1f)
			{
				skiBoard.ActionInput();
			}
			if (!skiBoard.IsTurn)
			{
				skiBoard.IsTurnChange(_set: true);
			}
			break;
		case SnowBoard_Define.AiStrength.STRONG:
			if (skiBoard.AccelGauge >= 0.5f)
			{
				skiBoard.ActionInput();
			}
			if (!skiBoard.IsTurn)
			{
				skiBoard.IsTurnChange(_set: true);
			}
			break;
		}
		if (targetAngle > checkAngle)
		{
			if (targetDir)
			{
				skiBoard.MoveCursor(SnowBoard_SkiBoard.MoveCursorDirection.RIGHT, 1f);
			}
			else
			{
				skiBoard.MoveCursor(SnowBoard_SkiBoard.MoveCursorDirection.LEFT, -1f);
			}
		}
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "CheckPoint" && other.gameObject == SingletonCustom<SnowBoard_CourseManager>.Instance.arrayCheckPointAuto[checkPointIdx] && checkPointIdx < SingletonCustom<SnowBoard_CourseManager>.Instance.CheckPointLength)
		{
			checkPointIdx++;
			currentCheckPointAnchor = SingletonCustom<SnowBoard_CourseManager>.Instance.GetCheckPointAnchor(checkPointIdx);
			targetRndX = UnityEngine.Random.Range(-2f, 2f);
		}
	}
}
