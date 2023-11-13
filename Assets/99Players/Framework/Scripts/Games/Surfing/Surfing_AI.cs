using System.Collections;
using UnityEngine;
public class Surfing_AI : MonoBehaviour
{
	[SerializeField]
	[Header("プレイヤ\u30fc処理")]
	private Surfing_Player player;
	[SerializeField]
	[Header("水蜘蛛処理")]
	private Surfing_Surfer waterSprider;
	private int playerNo;
	private Surfing_Define.AiStrength aiStrength;
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
		aiStrength = (Surfing_Define.AiStrength)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
	}
	public void UpdateMethod()
	{
	}
	public void CrashMethod()
	{
		StartCoroutine(IsCrashWait());
	}
	private void OnTriggerEnter(Collider other)
	{
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
