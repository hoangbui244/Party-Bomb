using System.Collections;
using UnityEngine;
public class HandSeal_AI : MonoBehaviour
{
	[SerializeField]
	[Header("プレイヤ\u30fc処理")]
	private HandSeal_Player player;
	[SerializeField]
	[Header("印結び処理")]
	private HandSeal_Hand hand;
	private int playerNo;
	private HandSeal_Define.AiStrength aiStrength;
	private float pushWaitTime;
	private bool isPushWait;
	private HandSeal_Hand.InputButton inputButton;
	public int PlayerNo => playerNo;
	public void Init()
	{
		aiStrength = (HandSeal_Define.AiStrength)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		playerNo = hand.PlayerNo;
		switch (aiStrength)
		{
		case HandSeal_Define.AiStrength.WEAK:
			pushWaitTime = 0.75f;
			break;
		case HandSeal_Define.AiStrength.COMMON:
			pushWaitTime = 0.6f;
			break;
		case HandSeal_Define.AiStrength.STRONG:
			pushWaitTime = 0.45f;
			break;
		}
		pushWaitTime += UnityEngine.Random.Range(0f, 30f) * 0.01f;
	}
	public void UpdateMethod()
	{
		if (!isPushWait)
		{
			StartCoroutine(IsPushWait());
			if (!hand.IsSecretStylePlay && hand.secretGauge == 1f)
			{
				inputButton = HandSeal_Hand.InputButton.R;
			}
			else
			{
				inputButton = hand.enemys[hand.SelectEnemy].GetInputButton();
			}
			switch (inputButton)
			{
			case HandSeal_Hand.InputButton.A:
				hand.InputSeal(HandSeal_Hand.InputButton.A);
				break;
			case HandSeal_Hand.InputButton.B:
				hand.InputSeal(HandSeal_Hand.InputButton.B);
				break;
			case HandSeal_Hand.InputButton.Y:
				hand.InputSeal(HandSeal_Hand.InputButton.Y);
				break;
			case HandSeal_Hand.InputButton.X:
				hand.InputSeal(HandSeal_Hand.InputButton.X);
				break;
			case HandSeal_Hand.InputButton.R:
				hand.SecretStyleAction();
				break;
			}
		}
	}
	private IEnumerator IsPushWait()
	{
		isPushWait = true;
		yield return new WaitForSeconds(pushWaitTime);
		isPushWait = false;
	}
}
