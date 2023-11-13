using GamepadInput;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GS_Omikuji : MonoBehaviour
{
	public enum State
	{
		STANDBY,
		SHOW,
		SHAKE,
		OPEN_WAIT,
		FRONT_WAIT,
		REVERSE,
		BACK_WAIT,
		CLOSE_WAIT
	}
	[SerializeField]
	[Header("フェ\u30fcド")]
	private SpriteRenderer spFade;
	[SerializeField]
	[Header("フラッシュフェ\u30fcド")]
	private SpriteRenderer spFlashFade;
	[SerializeField]
	[Header("箱オブジェクト")]
	private GameObject box;
	[SerializeField]
	[Header("棒オブジェクト")]
	private GameObject stick;
	[SerializeField]
	[Header("マスクオブジェクト")]
	private GameObject mask;
	[SerializeField]
	[Header("結果オブジェクト")]
	private GameObject result;
	[SerializeField]
	[Header("結果表オブジェクト")]
	private GameObject resultFront;
	[SerializeField]
	[Header("結果裏オブジェクト")]
	private GameObject resultBack;
	[SerializeField]
	[Header("コントロ\u30fcラ\u30fc")]
	private GameObject objController;
	[SerializeField]
	[Header("フラッシュ画像")]
	private SpriteRenderer spFlash;
	[SerializeField]
	[Header("結果：種別")]
	private SpriteRenderer spResultType;
	[SerializeField]
	[Header("結果：種別【素材】")]
	private Sprite[] arraySpriteResultType;
	[SerializeField]
	[Header("結果：種別裏")]
	private SpriteRenderer spResultTypeBack;
	[SerializeField]
	[Header("結果：種別裏【素材】")]
	private Sprite[] arraySpriteResultTypeBack;
	[SerializeField]
	[Header("結果：健康運")]
	private SpriteRenderer[] arraySpResultHealth;
	[SerializeField]
	[Header("結果：勝負運")]
	private SpriteRenderer[] arraySpResultGame;
	[SerializeField]
	[Header("結果：恋愛運")]
	private SpriteRenderer[] arraySpResultLove;
	[SerializeField]
	[Header("結果：金運")]
	private SpriteRenderer[] arraySpResultMoney;
	[SerializeField]
	[Header("結果：仕事運")]
	private SpriteRenderer[] arraySpResultJob;
	[SerializeField]
	[Header("結果：運★【素材】")]
	private Sprite[] arraySpResultStar;
	[SerializeField]
	[Header("結果：ラッキ\u30fcカラ\u30fc")]
	private SpriteRenderer spResultLuckyColor;
	[SerializeField]
	[Header("結果：ラッキ\u30fcカラ\u30fc【素材】")]
	private Sprite[] arraySpriteResultLuckyColor;
	[SerializeField]
	[Header("結果：ラッキ\u30fcナンバ\u30fc")]
	private SpriteRenderer spResultLuckyNumber;
	[SerializeField]
	[Header("結果：ラッキ\u30fcナンバ\u30fc【素材】")]
	private Sprite[] arraySpriteResultLuckyNumber;
	[SerializeField]
	[Header("結果：アイコン")]
	private SpriteRenderer spResultIcon;
	[SerializeField]
	[Header("結果：アイコン裏")]
	private SpriteRenderer spResultIconBack;
	[SerializeField]
	[Header("結果：アイコン【素材】")]
	private Sprite[] arraySpriteIcon;
	[SerializeField]
	[Header("結果：文章")]
	private SpriteRenderer spResultText;
	[SerializeField]
	[Header("結果：文章【素材】")]
	private Sprite[] arraySpriteResultText;
	private readonly float POS_Y_BOX_STANDBY = 1200f;
	private readonly float POS_Y_BOX_APPEAR;
	private readonly float POS_Y_STICK_STANDBY = 66f;
	private readonly float POS_Y_STICK_DRAW = 705f;
	private readonly float POS_Y_STICK_OPEN;
	private readonly float POS_OUT_CONTROLLER = 1200f;
	private readonly float POS_DEFAULT_CONTROLLER = 618f;
	private readonly int[] GOOD_TYPE_TABLE = new int[10]
	{
		13,
		17,
		17,
		17,
		17,
		19,
		0,
		0,
		0,
		0
	};
	private readonly int[] BAD_TYPE_TABLE = new int[10]
	{
		0,
		0,
		0,
		0,
		0,
		0,
		40,
		30,
		20,
		10
	};
	private readonly int RESULT_TEXT_TYPE_0_CNT = 11;
	private readonly int RESULT_TEXT_TYPE_1_CNT = 8;
	private readonly int RESULT_TEXT_TYPE_2_CNT = 16;
	private readonly int RESULT_TEXT_TYPE_3_CNT = 7;
	private readonly int RESULT_TEXT_TYPE_4_CNT = 10;
	private readonly int RESULT_TEXT_TYPE_5_CNT = 10;
	private readonly int RESULT_TEXT_TYPE_6_CNT = 5;
	private readonly int RESULT_TEXT_TYPE_7_CNT = 5;
	private readonly int RESULT_TEXT_TYPE_8_CNT = 5;
	private readonly int RESULT_TEXT_TYPE_9_CNT = 5;
	private float shakeTime;
	private State currentState;
	private Action callback;
	public bool IsOpen
	{
		get;
		set;
	}
	public void Show(Action _callback = null)
	{
		callback = _callback;
		IsOpen = true;
		LeanTween.cancel(spFade.gameObject);
		LeanTween.cancel(box);
		LeanTween.cancel(stick);
		LeanTween.cancel(result);
		currentState = State.SHOW;
		spFade.gameObject.SetActive(value: true);
		box.SetActive(value: true);
		stick.SetActive(value: true);
		result.SetActive(value: false);
		resultFront.SetActive(value: true);
		resultBack.SetActive(value: false);
		spFlash.gameObject.SetActive(value: false);
		spFlashFade.gameObject.SetActive(value: false);
		objController.transform.SetLocalPositionX(POS_OUT_CONTROLLER);
		box.transform.SetLocalPositionX(0f);
		box.transform.SetLocalPositionY(POS_Y_BOX_STANDBY);
		stick.transform.SetLocalPositionY(POS_Y_STICK_STANDBY);
		stick.transform.SetLocalScale(0.5f, 0.5f, 1f);
		mask.SetActive(value: true);
		spFade.SetAlpha(0f);
		LeanTween.value(0f, 0.75f, 0.5f).setOnUpdate(delegate(float _value)
		{
			spFade.SetAlpha(_value);
		});
		LeanTween.moveLocalY(box, POS_Y_BOX_APPEAR, 1f).setDelay(0.5f).setEaseOutBounce()
			.setOnComplete((Action)delegate
			{
				currentState = State.SHAKE;
				StartCoroutine(_Shake());
			});
	}
	private IEnumerator _Shake()
	{
		yield return new WaitForSeconds(1.2f);
		int num;
		for (int i = 0; i < 13; i = num)
		{
			shakeTime = UnityEngine.Random.Range(0.1f, 0.125f);
			box.transform.SetLocalPosition(0f, POS_Y_BOX_APPEAR, -26f);
			LeanTween.moveLocal(box, new Vector3(0f + UnityEngine.Random.Range(-30f, 30f), POS_Y_BOX_APPEAR + UnityEngine.Random.Range(-30f, 30f), -26f), shakeTime);
			SingletonCustom<HidVibration>.Instance.SetCommonVibration(0);
			yield return new WaitForSeconds(shakeTime);
			num = i + 1;
		}
		box.transform.SetLocalPosition(0f, POS_Y_BOX_APPEAR, -26f);
		SingletonCustom<HidVibration>.Instance.SetCommonVibration(0);
		yield return new WaitForSeconds(0.25f);
		currentState = State.OPEN_WAIT;
		yield return new WaitForSeconds(0.5f);
		LeanTween.moveLocalY(stick, POS_Y_STICK_DRAW, 0.5f).setEaseOutQuart().setOnComplete((Action)delegate
		{
			mask.SetActive(value: false);
			LeanTween.moveLocalY(stick, POS_Y_STICK_OPEN, 0.75f).setDelay(0.25f).setEaseOutQuart();
			LeanTween.scale(stick, new Vector3(1f, 1f, 1f), 0.75f).setDelay(0.25f).setEaseOutQuart();
		});
		spFlash.gameObject.SetActive(value: true);
		spFlash.SetAlpha(0f);
		LeanTween.value(0f, 1f, 0.5f).setDelay(0.5f).setOnUpdate(delegate(float _value)
		{
			spFlash.SetAlpha(_value);
		})
			.setOnComplete((Action)delegate
			{
				LeanTween.value(1f, 0f, 0.5f).setOnUpdate(delegate(float _value)
				{
					spFlash.SetAlpha(_value);
				});
			});
		LeanTween.cancel(spFlashFade.gameObject);
		spFlashFade.gameObject.SetActive(value: true);
		spFlashFade.SetAlpha(0f);
		LeanTween.value(0f, 1f, 0.5f).setDelay(1.75f).setOnUpdate(delegate(float _value)
		{
			spFlashFade.SetAlpha(_value);
		})
			.setOnComplete((Action)delegate
			{
				LeanTween.value(1f, 0f, 0.25f).setOnUpdate(delegate(float _value)
				{
					spFlashFade.SetAlpha(_value);
				});
			});
		yield return new WaitForSeconds(2.25f);
		SetResult();
		result.SetActive(value: true);
		result.transform.SetLocalScale(0f, 0f, 1f);
		SingletonCustom<AudioManager>.Instance.SePlay("se_result_lastresult");
		LeanTween.scale(result, new Vector3(0.89f, 0.89f, 1f), 0.35f).setEaseOutBack().setOnComplete((Action)delegate
		{
			currentState = State.FRONT_WAIT;
		});
		objController.transform.SetLocalPositionX(POS_OUT_CONTROLLER);
		LeanTween.cancel(objController);
		LeanTween.moveLocalX(objController, POS_DEFAULT_CONTROLLER, 0.55f).setEaseOutQuint();
	}
	private void SetResult()
	{
		int randomIndex = CalcManager.GetRandomIndex((UnityEngine.Random.Range(0, 100) <= 80) ? GOOD_TYPE_TABLE : BAD_TYPE_TABLE);
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		List<int> list = new List<int>();
		switch (randomIndex)
		{
		case 0:
			num = 5;
			num2 = 5;
			num3 = 5;
			num4 = 5;
			num5 = 5;
			num6 = UnityEngine.Random.Range(0, RESULT_TEXT_TYPE_0_CNT);
			break;
		case 1:
			list.Add(5);
			list.Add(5);
			list.Add(5);
			list.Add(4);
			list.Add(4);
			list.Shuffle();
			num = list[0];
			num2 = list[1];
			num3 = list[2];
			num4 = list[3];
			num5 = list[4];
			num6 = 11 + UnityEngine.Random.Range(0, RESULT_TEXT_TYPE_1_CNT);
			break;
		case 2:
			list.Add(5);
			list.Add(4);
			list.Add(4);
			list.Add(4);
			list.Add(3);
			list.Shuffle();
			num = list[0];
			num2 = list[1];
			num3 = list[2];
			num4 = list[3];
			num5 = list[4];
			num6 = 19 + UnityEngine.Random.Range(0, RESULT_TEXT_TYPE_2_CNT);
			break;
		case 3:
			list.Add(4);
			list.Add(4);
			list.Add(4);
			list.Add(3);
			list.Add(3);
			list.Shuffle();
			num = list[0];
			num2 = list[1];
			num3 = list[2];
			num4 = list[3];
			num5 = list[4];
			num6 = 35 + UnityEngine.Random.Range(0, RESULT_TEXT_TYPE_3_CNT);
			break;
		case 4:
			list.Add(4);
			list.Add(4);
			list.Add(3);
			list.Add(3);
			list.Add(2);
			list.Shuffle();
			num = list[0];
			num2 = list[1];
			num3 = list[2];
			num4 = list[3];
			num5 = list[4];
			num6 = 42 + UnityEngine.Random.Range(0, RESULT_TEXT_TYPE_4_CNT);
			break;
		case 5:
			list.Add(4);
			list.Add(3);
			list.Add(3);
			list.Add(3);
			list.Add(2);
			list.Shuffle();
			num = list[0];
			num2 = list[1];
			num3 = list[2];
			num4 = list[3];
			num5 = list[4];
			num6 = 52 + UnityEngine.Random.Range(0, RESULT_TEXT_TYPE_5_CNT);
			break;
		case 6:
			list.Add(3);
			list.Add(3);
			list.Add(2);
			list.Add(2);
			list.Add(1);
			list.Shuffle();
			num = list[0];
			num2 = list[1];
			num3 = list[2];
			num4 = list[3];
			num5 = list[4];
			num6 = 62 + UnityEngine.Random.Range(0, RESULT_TEXT_TYPE_6_CNT);
			break;
		case 7:
			list.Add(3);
			list.Add(2);
			list.Add(2);
			list.Add(1);
			list.Add(1);
			list.Shuffle();
			num = list[0];
			num2 = list[1];
			num3 = list[2];
			num4 = list[3];
			num5 = list[4];
			num6 = 67 + UnityEngine.Random.Range(0, RESULT_TEXT_TYPE_7_CNT);
			break;
		case 8:
			list.Add(2);
			list.Add(2);
			list.Add(1);
			list.Add(1);
			list.Add(1);
			list.Shuffle();
			num = list[0];
			num2 = list[1];
			num3 = list[2];
			num4 = list[3];
			num5 = list[4];
			num6 = 72 + UnityEngine.Random.Range(0, RESULT_TEXT_TYPE_8_CNT);
			break;
		case 9:
			num = 1;
			num2 = 1;
			num3 = 1;
			num4 = 1;
			num5 = 1;
			num6 = 77 + UnityEngine.Random.Range(0, RESULT_TEXT_TYPE_9_CNT);
			break;
		}
		spResultType.sprite = arraySpriteResultType[randomIndex];
		spResultTypeBack.sprite = arraySpriteResultTypeBack[randomIndex];
		for (int i = 0; i < arraySpResultHealth.Length; i++)
		{
			arraySpResultHealth[i].sprite = arraySpResultStar[(i < num) ? 1 : 0];
		}
		for (int j = 0; j < arraySpResultGame.Length; j++)
		{
			arraySpResultGame[j].sprite = arraySpResultStar[(j < num2) ? 1 : 0];
		}
		for (int k = 0; k < arraySpResultLove.Length; k++)
		{
			arraySpResultLove[k].sprite = arraySpResultStar[(k < num3) ? 1 : 0];
		}
		for (int l = 0; l < arraySpResultMoney.Length; l++)
		{
			arraySpResultMoney[l].sprite = arraySpResultStar[(l < num4) ? 1 : 0];
		}
		for (int m = 0; m < arraySpResultJob.Length; m++)
		{
			arraySpResultJob[m].sprite = arraySpResultStar[(m < num5) ? 1 : 0];
		}
		spResultLuckyColor.sprite = arraySpriteResultLuckyColor[UnityEngine.Random.Range(0, arraySpriteResultLuckyColor.Length)];
		spResultLuckyNumber.sprite = arraySpriteResultLuckyNumber[UnityEngine.Random.Range(0, arraySpriteResultLuckyNumber.Length)];
		spResultIcon.sprite = arraySpriteIcon[randomIndex];
		spResultIconBack.sprite = arraySpriteIcon[randomIndex];
		spResultText.sprite = arraySpriteResultText[num6];
	}
	public void Close()
	{
		IsOpen = false;
		LeanTween.cancel(box);
		LeanTween.cancel(stick);
		LeanTween.cancel(result);
		box.SetActive(value: false);
		result.SetActive(value: false);
		resultFront.SetActive(value: true);
		resultBack.SetActive(value: false);
		objController.transform.SetLocalPositionX(POS_OUT_CONTROLLER);
		currentState = State.STANDBY;
		if (callback != null)
		{
			callback();
		}
	}
	private void Update()
	{
		switch (currentState)
		{
		case State.SHOW:
		case State.SHAKE:
		case State.OPEN_WAIT:
		case State.REVERSE:
		case State.CLOSE_WAIT:
			break;
		case State.FRONT_WAIT:
			if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.A))
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
				LeanTween.moveLocalX(objController, POS_OUT_CONTROLLER, 0.55f).setEaseOutQuint();
				LeanTween.value(0.75f, 0f, 0.5f).setOnUpdate(delegate(float _value)
				{
					spFade.SetAlpha(_value);
				});
				LeanTween.moveLocalY(box, POS_Y_BOX_STANDBY, 0.25f);
				stick.SetActive(value: false);
				LeanTween.scale(result, new Vector3(0f, 0f, 1f), 0.35f).setEaseInQuart().setOnComplete((Action)delegate
				{
					if (!SingletonCustom<AudioManager>.Instance.IsBgmPlaying("bgm_menu"))
					{
						SingletonCustom<AudioManager>.Instance.BgmStop();
						SingletonCustom<AudioManager>.Instance.BgmPlay("bgm_menu", _loop: true);
					}
					Close();
				});
				currentState = State.CLOSE_WAIT;
			}
			else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.X))
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
				currentState = State.REVERSE;
				LeanTween.scale(result, new Vector3(0f, 0.89f, 1f), 0.25f).setOnComplete((Action)delegate
				{
					resultFront.SetActive(value: false);
					resultBack.SetActive(value: true);
					LeanTween.scale(result, new Vector3(0.89f, 0.89f, 1f), 0.25f).setOnComplete((Action)delegate
					{
						currentState = State.BACK_WAIT;
					});
				});
				LeanTween.moveLocalY(result, 15f, 0.25f).setOnComplete((Action)delegate
				{
					LeanTween.moveLocalY(result, POS_Y_BOX_APPEAR, 0.25f);
				});
			}
			break;
		case State.BACK_WAIT:
			if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.A))
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
				LeanTween.moveLocalX(objController, POS_OUT_CONTROLLER, 0.55f).setEaseOutQuint();
				LeanTween.value(0.75f, 0f, 0.5f).setOnUpdate(delegate(float _value)
				{
					spFade.SetAlpha(_value);
				});
				LeanTween.moveLocalY(box, POS_Y_BOX_STANDBY, 0.25f);
				stick.SetActive(value: false);
				LeanTween.scale(result, new Vector3(0f, 0f, 1f), 0.35f).setEaseInQuart().setOnComplete((Action)delegate
				{
					if (!SingletonCustom<AudioManager>.Instance.IsBgmPlaying("bgm_menu"))
					{
						SingletonCustom<AudioManager>.Instance.BgmStop();
						SingletonCustom<AudioManager>.Instance.BgmPlay("bgm_menu", _loop: true);
					}
					Close();
				});
				currentState = State.CLOSE_WAIT;
			}
			else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.X))
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
				LeanTween.scale(result, new Vector3(0f, 0.89f, 1f), 0.25f).setOnComplete((Action)delegate
				{
					resultFront.SetActive(value: true);
					resultBack.SetActive(value: false);
					LeanTween.scale(result, new Vector3(0.89f, 0.89f, 1f), 0.25f).setOnComplete((Action)delegate
					{
						currentState = State.FRONT_WAIT;
					});
				});
				LeanTween.moveLocalY(result, 15f, 0.25f).setOnComplete((Action)delegate
				{
					LeanTween.moveLocalY(result, POS_Y_BOX_APPEAR, 0.25f);
				});
				currentState = State.REVERSE;
			}
			break;
		}
	}
	private void OnDestroy()
	{
		LeanTween.cancel(box);
		LeanTween.cancel(stick);
		LeanTween.cancel(result);
		LeanTween.cancel(objController);
	}
}
