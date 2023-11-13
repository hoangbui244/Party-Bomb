using System.Collections;
using UnityEngine;
public class BeachFlag_AI : MonoBehaviour
{
	[SerializeField]
	[Header("プレイヤ\u30fc処理")]
	private BeachFlag_Player player;
	[SerializeField]
	[Header("キャラクタ\u30fc処理")]
	private BeachFlag_Chara chara;
	private int playerNo;
	private IEnumerator cor;
	private BeachFlag_Define.AiStrength aiStrength;
	private Transform currentCheckPointAnchor;
	private int checkPointIdx;
	private int balance;
	private float checkAngle;
	private bool isStart;
	private float waitTime;
	private float waitTimeOrigin;
	private bool isAccelInput;
	public int PlayerNo => playerNo;
	public int CheckPointIdx => checkPointIdx;
	public void Init()
	{
		aiStrength = (BeachFlag_Define.AiStrength)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		cor = null;
		checkPointIdx = 0;
		isAccelInput = false;
		isStart = true;
		for (int i = 0; i < BeachFlag_Define.PM.Players.Length; i++)
		{
			if (BeachFlag_Define.PM.Players[i].UserType <= BeachFlag_Define.UserType.PLAYER_4)
			{
				balance++;
			}
			else
			{
				balance--;
			}
		}
		float num = UnityEngine.Random.Range(0.25f, 1.5f);
		switch (aiStrength)
		{
		case BeachFlag_Define.AiStrength.WEAK:
			checkAngle = 3f;
			waitTime = 0.3f;
			break;
		case BeachFlag_Define.AiStrength.COMMON:
			checkAngle = 3f;
			waitTime = 0.2f;
			break;
		case BeachFlag_Define.AiStrength.STRONG:
			checkAngle = 1f;
			waitTime = 0.1f;
			break;
		}
		waitTimeOrigin = waitTime;
		waitTime *= num;
	}
	public void UpdateMethod()
	{
		if (chara.Cart.m_Position >= chara.CART_MAX)
		{
			StopCoroutine(IsAccelInputWait());
			isAccelInput = true;
		}
		if (isStart)
		{
			if (!isAccelInput)
			{
				StartCoroutine(IsAccelInputWait());
			}
		}
		else if (!isAccelInput)
		{
			StartCoroutine(IsAccelInputWait());
			chara.AccelInput();
		}
	}
	private IEnumerator IsAccelInputWait()
	{
		float num = (balance != 0) ? UnityEngine.Random.Range(0.2f, 2.3f) : UnityEngine.Random.Range(0.3f, 1.7f);
		isAccelInput = true;
		yield return new WaitForSeconds(waitTime * num);
		if (isStart)
		{
			waitTime = waitTimeOrigin;
			isStart = !isStart;
		}
		isAccelInput = false;
	}
}
