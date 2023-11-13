using UnityEngine;
public class SwordFight_CameraMover : SingletonCustom<SwordFight_CameraMover>
{
	private readonly Vector3 DEFAULT_OFFSET = new Vector3(6.25f, 12.8f, 0f);
	[SerializeField]
	[Header("対象キャラクタ\u30fc配列")]
	private SwordFight_CharacterScript[] arrayCharacter;
	[SerializeField]
	[Header("カメラ")]
	private Camera fieldCamera;
	[SerializeField]
	private float CHECK_ADJUST_THRESHOLD = 10f;
	private Vector3 leftTop;
	private Vector3 rightBottom;
	private Vector3 targetPos;
	private const float MOVER_SPEED = 0.025f;
	private bool isInit;
	private Vector3 initPos;
	public Camera FieldCamera => fieldCamera;
	public Vector3 TargetPos => targetPos;
	private void Awake()
	{
		initPos = base.transform.position;
	}
	public void Init()
	{
		base.transform.position = initPos;
		targetPos = base.transform.position;
	}
	public void FixCameraPos()
	{
		base.transform.position = targetPos;
	}
	private void Update()
	{
		if (!SingletonCustom<CommonNotificationManager>.Instance.IsPause || !(Time.timeScale < 1f))
		{
			UpdateTargetRect();
			UpdatePosition();
		}
	}
	public void UpdateTargetRect()
	{
		leftTop = Vector3.zero;
		rightBottom = Vector3.zero;
		isInit = false;
		arrayCharacter = SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList;
		for (int i = 0; i < arrayCharacter.Length; i++)
		{
			if (!arrayCharacter[i].gameObject.activeSelf)
			{
				continue;
			}
			CalcManager.mCalcVector3 = arrayCharacter[i].transform.position;
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
			targetPos.x += DEFAULT_OFFSET.x;
			targetPos.y = DEFAULT_OFFSET.y;
			targetPos.z += DEFAULT_OFFSET.z;
			CalcManager.mCalcFloat = Vector3.Distance(leftTop, rightBottom);
			if (CalcManager.mCalcFloat > CHECK_ADJUST_THRESHOLD)
			{
				targetPos.y += (CalcManager.mCalcFloat - CHECK_ADJUST_THRESHOLD) * 0.1f;
				targetPos.x += (CalcManager.mCalcFloat - CHECK_ADJUST_THRESHOLD) * 0.5f;
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
		if (base.transform.localPosition.z > 4.15f)
		{
			base.transform.SetLocalPositionZ(4.15f);
		}
		if (base.transform.localPosition.z < -4.15f)
		{
			base.transform.SetLocalPositionZ(-4.15f);
		}
	}
}
