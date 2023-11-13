using System;
using UnityEngine;
public class BigMerchantCustomerCharacter : MonoBehaviour
{
	public enum State
	{
		Wait,
		Move_Counter,
		Move_Wait1,
		Move_Wait2,
		Move_Side,
		Move_Return
	}
	[SerializeField]
	[Header("所属クラス")]
	private BigMerchantCustomer root;
	[SerializeField]
	[Header("移動位置")]
	private Transform[] arrayWaitPos;
	[SerializeField]
	[Header("マテリアル変更対象")]
	private MeshRenderer[] arrayChangeTarget;
	[SerializeField]
	[Header("マテリアル")]
	private Material[] arrayMat;
	[SerializeField]
	[Header("マテリアル1表示オブジェクト")]
	private GameObject[] arrayMat1Obj;
	[SerializeField]
	[Header("マテリアル2表示オブジェクト")]
	private GameObject[] arrayMat2Obj;
	[SerializeField]
	[Header("マテリアル3表示オブジェクト")]
	private GameObject[] arrayMat3Obj;
	[SerializeField]
	[Header("マテリアル4表示オブジェクト")]
	private GameObject[] arrayMat4Obj;
	[SerializeField]
	[Header("マテリアル5表示オブジェクト")]
	private GameObject[] arrayMat5Obj;
	[SerializeField]
	[Header("マテリアル6表示オブジェクト")]
	private GameObject[] arrayMat6Obj;
	[SerializeField]
	[Header("マテリアル7表示オブジェクト")]
	private GameObject[] arrayMat7Obj;
	[SerializeField]
	[Header("マテリアル8表示オブジェクト")]
	private GameObject[] arrayMat8Obj;
	[SerializeField]
	[Header("顔モデル")]
	private MeshRenderer headMesh;
	[SerializeField]
	[Header("顔マテリアル：男")]
	private Material[] arrayMatHeadBoy;
	[SerializeField]
	[Header("顔マテリアル：女")]
	private Material[] arrayMatHeadGirl;
	[SerializeField]
	[Header("アニメ\u30fcション")]
	private Animator anim;
	private float stayingTime;
	private readonly float STAYING_TIME_MAX = 15f;
	private readonly float STAYING_TIME_FLUC = 3f;
	private Vector3 targetPos;
	private State currentState;
	public State CurrentState => currentState;
	public void Init()
	{
		base.transform.position = arrayWaitPos[0].position;
		currentState = State.Wait;
	}
	public void EnterShop(State _state)
	{
		if (currentState == State.Wait)
		{
			SetCharacter();
		}
		ResetStayingTime();
		switch (_state)
		{
		case State.Move_Counter:
			targetPos = arrayWaitPos[1].position;
			break;
		case State.Move_Wait1:
			targetPos = arrayWaitPos[2].position;
			break;
		case State.Move_Wait2:
			targetPos = arrayWaitPos[3].position;
			break;
		}
		LeanTween.cancel(base.gameObject);
		LeanTween.move(base.gameObject, targetPos, Vector3.Distance(base.transform.position, targetPos) * 0.75f).setDelay((currentState == State.Wait) ? UnityEngine.Random.Range(0f, 1f) : 0f).setOnStart(delegate
		{
			SetTrigger("Move");
		})
			.setOnComplete((Action)delegate
			{
				switch (_state)
				{
				case State.Move_Counter:
					root.CreateOrder();
					break;
				}
				SetTrigger("Standby");
			});
		base.transform.SetLocalEulerAnglesY(180f);
		currentState = _state;
	}
	public void ClosedShop(float _delay)
	{
		targetPos = arrayWaitPos[4].position;
		LeanTween.cancel(base.gameObject);
		LeanTween.move(base.gameObject, targetPos, Vector3.Distance(base.transform.position, targetPos) * 0.75f).setDelay(_delay).setOnStart(delegate
		{
			SetTrigger("Move");
		})
			.setOnComplete((Action)delegate
			{
				currentState = State.Move_Return;
				targetPos = arrayWaitPos[5].position;
				LeanTween.move(base.gameObject, targetPos, Vector3.Distance(base.transform.position, targetPos) * 0.75f).setOnComplete((Action)delegate
				{
					Init();
				});
				LeanTween.rotateLocal(base.gameObject, new Vector3(0f, 0f, 0f), 0.15f);
				root.NextMove();
			});
		LeanTween.rotateLocal(base.gameObject, new Vector3(0f, (targetPos.x > base.transform.position.x) ? 90f : (-90f), 0f), 0.15f).setDelay(_delay);
		currentState = State.Move_Side;
	}
	public void ResetStayingTime()
	{
		stayingTime = STAYING_TIME_MAX + UnityEngine.Random.Range(0f, STAYING_TIME_FLUC);
	}
	public void PlayBuyAnim()
	{
		SetTrigger("Buy");
	}
	public void UpdateMethod()
	{
		if (currentState == State.Move_Counter && stayingTime > 0f)
		{
			stayingTime -= Time.deltaTime;
			if (stayingTime <= 0f)
			{
				root.OnClosedShop();
				ClosedShop(0f);
			}
		}
	}
	private void SetTrigger(string _name)
	{
		if (!anim.GetCurrentAnimatorStateInfo(0).IsName(_name))
		{
			anim.SetTrigger(_name);
		}
	}
	private void SetCharacter()
	{
		int characterMatIdx = SingletonCustom<BigMerchantCustomerManager>.Instance.GetCharacterMatIdx();
		for (int i = 0; i < arrayChangeTarget.Length; i++)
		{
			arrayChangeTarget[i].material = arrayMat[characterMatIdx];
		}
		for (int j = 0; j < arrayMat1Obj.Length; j++)
		{
			arrayMat1Obj[j].SetActive(characterMatIdx % 8 == 0);
		}
		for (int k = 0; k < arrayMat2Obj.Length; k++)
		{
			arrayMat2Obj[k].SetActive(1 == characterMatIdx % 8);
		}
		for (int l = 0; l < arrayMat3Obj.Length; l++)
		{
			arrayMat3Obj[l].SetActive(2 == characterMatIdx % 8);
		}
		for (int m = 0; m < arrayMat4Obj.Length; m++)
		{
			arrayMat4Obj[m].SetActive(3 == characterMatIdx % 8);
		}
		for (int n = 0; n < arrayMat5Obj.Length; n++)
		{
			arrayMat5Obj[n].SetActive(4 == characterMatIdx % 8);
		}
		for (int num = 0; num < arrayMat6Obj.Length; num++)
		{
			arrayMat6Obj[num].SetActive(5 == characterMatIdx % 8);
		}
		for (int num2 = 0; num2 < arrayMat7Obj.Length; num2++)
		{
			arrayMat7Obj[num2].SetActive(6 == characterMatIdx % 8);
		}
		for (int num3 = 0; num3 < arrayMat8Obj.Length; num3++)
		{
			arrayMat8Obj[num3].SetActive(7 == characterMatIdx % 8);
		}
		if (characterMatIdx % 8 < 6)
		{
			headMesh.material = arrayMatHeadBoy[UnityEngine.Random.Range(0, arrayMatHeadBoy.Length)];
		}
		else
		{
			headMesh.material = arrayMatHeadGirl[UnityEngine.Random.Range(0, arrayMatHeadGirl.Length)];
		}
	}
	public void OnResult()
	{
		LeanTween.cancel(base.gameObject);
	}
	private void OnDestroy()
	{
		LeanTween.cancel(base.gameObject);
	}
}
