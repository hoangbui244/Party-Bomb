using UnityEngine;
public class ArenaBattleCameraMover : SingletonCustom<ArenaBattleCameraMover>
{
	[SerializeField]
	[Header("カメラ")]
	private Camera camera;
	[SerializeField]
	[Header("対象キャラクタ\u30fc配列")]
	private ArenaBattlePlayer[] arrayCharacter;
	[SerializeField]
	private float CHECK_ADJUST_THRESHOLD = 5f;
	[SerializeField]
	private float CHECK_MIN_THRESHOLD = 3f;
	private readonly Vector3 DEFAULT_OFFSET = new Vector3(0f, 10f, -10.25f);
	private Vector3 leftTop;
	private Vector3 rightBottom;
	private Vector3 targetPos;
	private const float MOVER_SPEED = 0.025f;
	private bool isInit;
	private Vector3 initPos;
	private float adjust;
	private int targetNum;
	public float Adjust => adjust;
	public void Init()
	{
		base.transform.position = initPos;
		targetPos = base.transform.position;
	}
	public Camera GetCamera()
	{
		return camera;
	}
	private void Awake()
	{
		initPos = base.transform.position;
	}
	private void Update()
	{
		if (!SingletonCustom<CommonNotificationManager>.Instance.IsPause || !(Time.timeScale < 1f))
		{
			UpdateTargetRect();
			UpdatePosition();
		}
	}
	private void UpdateTargetRect()
	{
		leftTop = Vector3.zero;
		rightBottom = Vector3.zero;
		isInit = false;
		targetNum = 0;
		for (int i = 0; i < arrayCharacter.Length; i++)
		{
			if (!arrayCharacter[i].gameObject.activeSelf || !arrayCharacter[i].IsCameraIn())
			{
				continue;
			}
			targetNum++;
			if (arrayCharacter[i].Hp > 0f)
			{
				CalcManager.mCalcVector3 = arrayCharacter[i].transform.position;
			}
			else
			{
				CalcManager.mCalcVector3 = arrayCharacter[i].DeadPos;
			}
			if (!isInit)
			{
				leftTop.x = (rightBottom.x = CalcManager.mCalcVector3.x);
				leftTop.z = (rightBottom.z = CalcManager.mCalcVector3.z);
				isInit = true;
				continue;
			}
			if (leftTop.x > CalcManager.mCalcVector3.x)
			{
				leftTop.x = CalcManager.mCalcVector3.x;
			}
			if (leftTop.z < CalcManager.mCalcVector3.z)
			{
				leftTop.z = CalcManager.mCalcVector3.z;
			}
			if (rightBottom.x < CalcManager.mCalcVector3.x)
			{
				rightBottom.x = CalcManager.mCalcVector3.x;
			}
			if (rightBottom.z > CalcManager.mCalcVector3.z)
			{
				rightBottom.z = CalcManager.mCalcVector3.z;
			}
		}
		if (isInit)
		{
			targetPos = Vector3.zero;
			targetPos.x = (leftTop.x + rightBottom.x) / 2f;
			targetPos.z = (leftTop.z + rightBottom.z) / 2f;
			targetPos.y = DEFAULT_OFFSET.y;
			targetPos.z += DEFAULT_OFFSET.z;
			CalcManager.mCalcFloat = Vector3.Distance(leftTop, rightBottom);
			if (targetNum == 1)
			{
				adjust = 7f;
				targetPos.y -= adjust;
				targetPos.z += adjust * 1.05f;
			}
		}
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Vector3 zero = Vector3.zero;
		zero.x = (leftTop.x + rightBottom.x) / 2f;
		zero.z = (leftTop.z + rightBottom.z) / 2f;
		Gizmos.DrawSphere(zero, 0.1f);
	}
	private void UpdatePosition()
	{
		base.transform.position = Vector3.Slerp(base.transform.position, targetPos, 0.025f);
	}
}
