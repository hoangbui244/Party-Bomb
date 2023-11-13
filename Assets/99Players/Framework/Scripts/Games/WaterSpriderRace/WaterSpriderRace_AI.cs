using System.Collections;
using UnityEngine;
public class WaterSpriderRace_AI : MonoBehaviour
{
	[SerializeField]
	[Header("プレイヤ\u30fc処理")]
	private WaterSpriderRace_Player player;
	[SerializeField]
	[Header("水蜘蛛処理")]
	private WaterSpriderRace_WaterSprider waterSprider;
	private int playerNo;
	private WaterSpriderRace_Define.AiStrength aiStrength;
	private Transform currentCheckPointAnchor;
	private int checkPointIdx;
	private float checkAngle;
	private float waitTime;
	private float waitTimeOrigin;
	private bool targetDir;
	private bool isAccelInput;
	private bool isCrash;
	private bool isCrashMoveDir;
	public int PlayerNo => playerNo;
	public int CheckPointIdx => checkPointIdx;
	public void Init()
	{
		aiStrength = (WaterSpriderRace_Define.AiStrength)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		checkPointIdx = 0;
		playerNo = waterSprider.PlayerNo;
		isAccelInput = false;
		isCrash = false;
		currentCheckPointAnchor = SingletonCustom<WaterSpriderRace_CourseManager>.Instance.GetCheckPointAnchor(playerNo, checkPointIdx);
		switch (aiStrength)
		{
		case WaterSpriderRace_Define.AiStrength.WEAK:
			checkAngle = 3f;
			waitTime = 0.3f;
			break;
		case WaterSpriderRace_Define.AiStrength.COMMON:
			checkAngle = 3f;
			waitTime = 0.2f;
			break;
		case WaterSpriderRace_Define.AiStrength.STRONG:
			checkAngle = 1f;
			waitTime = 0.15f;
			break;
		}
		waitTimeOrigin = waitTime;
	}
	public void UpdateMethod()
	{
		if (!isCrash)
		{
			if (Vector3.Cross(currentCheckPointAnchor.position - base.gameObject.transform.position, waterSprider.characterAnchorY.transform.forward).y < 0f)
			{
				targetDir = true;
			}
			else
			{
				targetDir = false;
			}
			if (Vector3.Angle(waterSprider.characterAnchorY.transform.forward, currentCheckPointAnchor.position - base.gameObject.transform.position) > checkAngle)
			{
				if (targetDir)
				{
					waterSprider.MoveCursor(WaterSpriderRace_WaterSprider.MoveCursorDirection.RIGHT, 1f);
				}
				else
				{
					waterSprider.MoveCursor(WaterSpriderRace_WaterSprider.MoveCursorDirection.LEFT, -1f);
				}
			}
		}
		else if (isCrashMoveDir)
		{
			waterSprider.MoveCursor(WaterSpriderRace_WaterSprider.MoveCursorDirection.RIGHT, 1f);
		}
		else
		{
			waterSprider.MoveCursor(WaterSpriderRace_WaterSprider.MoveCursorDirection.LEFT, -1f);
		}
		if (!isAccelInput)
		{
			waterSprider.AccelInput();
			StartCoroutine(IsAccelInputWait());
		}
	}
	public void CrashMethod()
	{
		StartCoroutine(IsCrashWait());
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "CheckPoint_Player" + playerNo.ToString() && other.gameObject == SingletonCustom<WaterSpriderRace_CourseManager>.Instance.arrayCheckPointAuto[playerNo][checkPointIdx] && checkPointIdx < SingletonCustom<WaterSpriderRace_CourseManager>.Instance.currentCheckPointLength[playerNo])
		{
			checkPointIdx++;
			currentCheckPointAnchor = SingletonCustom<WaterSpriderRace_CourseManager>.Instance.GetCheckPointAnchor(playerNo, checkPointIdx);
		}
	}
	private IEnumerator IsAccelInputWait()
	{
		isAccelInput = true;
		yield return new WaitForSeconds(waitTime);
		isAccelInput = false;
	}
	private IEnumerator IsCrashWait()
	{
		if (isCrashMoveDir)
		{
			isCrashMoveDir = false;
		}
		else
		{
			isCrashMoveDir = true;
		}
		waitTime = 0.1f;
		yield return new WaitForSeconds(0.7f);
		yield return new WaitForSeconds(0.8f);
		waitTime = waitTimeOrigin;
	}
}
