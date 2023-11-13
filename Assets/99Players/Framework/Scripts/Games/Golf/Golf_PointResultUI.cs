using System;
using UnityEngine;
public class Golf_PointResultUI : MonoBehaviour
{
	private readonly float FRAME_SCALE_TIME = 0.5f;
	private readonly float RESULT_SCALE_TIME = 0.25f;
	private readonly float POINT_ADD_TIME = 0.5f;
	[SerializeField]
	[Header("ル\u30fcト")]
	private GameObject root;
	[SerializeField]
	[Header("フレ\u30fcム")]
	private GameObject frame;
	[SerializeField]
	[Header("カップまで残りテキスト")]
	private SpriteRenderer remainingToCupText;
	[SerializeField]
	[Header("残りヤ\u30fcドの中央ル\u30fcト")]
	private GameObject remainingYardCenterRoot;
	[SerializeField]
	[Header("残りヤ\u30fcド")]
	private GameObject yardNumRoot;
	[SerializeField]
	[Header("残りヤ\u30fcドの数字")]
	private SpriteRenderer[] arrayRemainingYard;
	[SerializeField]
	[Header("残りヤ\u30fcドの数字の間隔")]
	private float REMAINING_YARD_NUM_SPACE;
	[SerializeField]
	[Header("残りヤ\u30fcドのドットの間隔")]
	private float REMAINING_YARD_DOT_SPACE;
	[SerializeField]
	[Header("ホ\u30fcルインワン")]
	private SpriteRenderer holeInOne;
	[SerializeField]
	[Header("記録なし")]
	private SpriteRenderer nonRecord;
	[SerializeField]
	[Header("ポイントの中央ル\u30fcト")]
	private Transform pointCenterRoot;
	private float pointCenterNumSpace;
	[SerializeField]
	[Header("ポイントの数字")]
	private SpriteNumbers point;
	private Vector3 originRootScale;
	private Vector3 originRemainingYardRootScale;
	private Vector3 originYardPos;
	private Vector3 originHoleInOneScale;
	private Vector3 originNonRecordScale;
	public void Init()
	{
		float num = point.NumSpace * 0.5f;
		float num2 = num * point.transform.localScale.x;
		point.transform.SetLocalPositionX(num + num2);
		pointCenterNumSpace = num2;
		originRootScale = root.transform.localScale;
		originRemainingYardRootScale = remainingYardCenterRoot.transform.localScale;
		originYardPos = yardNumRoot.transform.localPosition;
		originHoleInOneScale = holeInOne.transform.localScale;
		originNonRecordScale = nonRecord.transform.localScale;
	}
	public void InitPlay()
	{
		root.transform.localScale = Vector3.zero;
		remainingYardCenterRoot.transform.localScale = Vector3.zero;
		holeInOne.transform.localScale = Vector3.zero;
		nonRecord.transform.localScale = Vector3.zero;
		root.SetActive(value: false);
		remainingToCupText.gameObject.SetActive(value: false);
		remainingYardCenterRoot.SetActive(value: false);
		yardNumRoot.transform.localPosition = originYardPos;
		holeInOne.gameObject.SetActive(value: false);
		nonRecord.gameObject.SetActive(value: false);
		point.Set(0);
		pointCenterRoot.transform.SetLocalPositionX(0f - point.transform.localPosition.x);
	}
	public void Show(float __remainingDistanceToCup, int _addPoint)
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_result_view_score");
		Golf_Ball ball = SingletonCustom<Golf_BallManager>.Instance.GetTurnPlayerBall();
		if (ball.GetIsCupIn())
		{
			remainingToCupText.gameObject.SetActive(value: false);
			remainingYardCenterRoot.SetActive(value: false);
			holeInOne.gameObject.SetActive(value: true);
			nonRecord.gameObject.SetActive(value: false);
		}
		else if (ball.GetIsOB())
		{
			remainingToCupText.gameObject.SetActive(value: false);
			remainingYardCenterRoot.SetActive(value: false);
			holeInOne.gameObject.SetActive(value: false);
			nonRecord.gameObject.SetActive(value: true);
		}
		else
		{
			remainingToCupText.gameObject.SetActive(value: true);
			remainingYardCenterRoot.SetActive(value: true);
			ConvertRemainingYard(__remainingDistanceToCup);
			holeInOne.gameObject.SetActive(value: false);
			nonRecord.gameObject.SetActive(value: false);
		}
		root.SetActive(value: true);
		LeanTween.scale(root, originRootScale, FRAME_SCALE_TIME).setEaseOutBack();
		LeanTween.delayedCall(base.gameObject, FRAME_SCALE_TIME + 0.5f, (Action)delegate
		{
			if (ball.GetIsCupIn())
			{
				LeanTween.scale(holeInOne.gameObject, originHoleInOneScale, RESULT_SCALE_TIME).setEaseOutBack();
				SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
			}
			else if (ball.GetIsOB())
			{
				LeanTween.scale(nonRecord.gameObject, originNonRecordScale, RESULT_SCALE_TIME).setEaseOutBack();
				SingletonCustom<AudioManager>.Instance.SePlay("se_applause_0");
			}
			else
			{
				LeanTween.scale(remainingYardCenterRoot, originRemainingYardRootScale, RESULT_SCALE_TIME).setEaseOutBack();
				if (__remainingDistanceToCup < 3f)
				{
					SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
				}
				else if (__remainingDistanceToCup < 5f)
				{
					SingletonCustom<AudioManager>.Instance.SePlay("se_cheer");
				}
				else if (__remainingDistanceToCup < 10f)
				{
					SingletonCustom<AudioManager>.Instance.SePlay("se_applause_1");
				}
				else
				{
					SingletonCustom<AudioManager>.Instance.SePlay("se_applause_0");
				}
			}
			if (!ball.GetIsOB())
			{
				float time = 1f;
				LeanTween.value(base.gameObject, 0f, _addPoint, POINT_ADD_TIME).setOnUpdate(delegate(float _value)
				{
					time += Time.deltaTime;
					if (time > 0.1f)
					{
						time = 0f;
						SingletonCustom<AudioManager>.Instance.SePlay("se_cursor_move");
					}
					point.Set((int)_value);
					pointCenterRoot.transform.SetLocalPositionX(pointCenterNumSpace * (float)(((int)_value).ToString().Length - 1) - point.transform.localPosition.x);
				});
			}
		});
	}
	private void ConvertRemainingYard(float _remainingYard)
	{
		UnityEngine.Debug.Log("_remainingYard " + _remainingYard.ToString());
		float num = 0f;
		UnityEngine.Debug.Log("--------整数の計算--------");
		string text = ((int)_remainingYard).ToString();
		switch (text.Length)
		{
		case 1:
			arrayRemainingYard[0].gameObject.SetActive(value: false);
			arrayRemainingYard[1].gameObject.SetActive(value: false);
			arrayRemainingYard[2].gameObject.SetActive(value: true);
			arrayRemainingYard[2].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_number_b_" + int.Parse(text[0].ToString()).ToString());
			num -= REMAINING_YARD_NUM_SPACE * 2f * yardNumRoot.transform.localScale.x;
			break;
		case 2:
			arrayRemainingYard[0].gameObject.SetActive(value: false);
			arrayRemainingYard[1].gameObject.SetActive(value: true);
			arrayRemainingYard[1].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_number_b_" + int.Parse(text[0].ToString()).ToString());
			arrayRemainingYard[2].gameObject.SetActive(value: true);
			arrayRemainingYard[2].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_number_b_" + int.Parse(text[1].ToString()).ToString());
			num -= REMAINING_YARD_NUM_SPACE * yardNumRoot.transform.localScale.x;
			break;
		case 3:
			arrayRemainingYard[0].gameObject.SetActive(value: true);
			arrayRemainingYard[0].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_number_b_" + int.Parse(text[0].ToString()).ToString());
			arrayRemainingYard[1].gameObject.SetActive(value: true);
			arrayRemainingYard[1].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_number_b_" + int.Parse(text[1].ToString()).ToString());
			arrayRemainingYard[2].gameObject.SetActive(value: true);
			arrayRemainingYard[2].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_number_b_" + int.Parse(text[2].ToString()).ToString());
			break;
		}
		UnityEngine.Debug.Log("--------小数点の計算--------");
		text = _remainingYard.ToString();
		int num2 = text.IndexOf(".");
		UnityEngine.Debug.Log("remainingYardStr : " + text + " length : " + text.Length.ToString() + " dotIdx : " + num2.ToString());
		if (num2 == -1)
		{
			UnityEngine.Debug.Log("小数点がない場合");
			arrayRemainingYard[3].gameObject.SetActive(value: false);
			arrayRemainingYard[4].gameObject.SetActive(value: false);
			arrayRemainingYard[5].gameObject.SetActive(value: false);
			yardNumRoot.transform.AddLocalPositionX((REMAINING_YARD_NUM_SPACE * 2f + REMAINING_YARD_DOT_SPACE) * yardNumRoot.transform.localScale.x);
			num -= (REMAINING_YARD_NUM_SPACE * 2f + REMAINING_YARD_DOT_SPACE) * yardNumRoot.transform.localScale.x;
		}
		else
		{
			UnityEngine.Debug.Log("小数点がある場合");
			arrayRemainingYard[3].gameObject.SetActive(value: true);
			switch (num2)
			{
			case 1:
				switch (text.Length)
				{
				case 3:
					arrayRemainingYard[4].gameObject.SetActive(value: true);
					arrayRemainingYard[4].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_number_b_" + int.Parse(text[2].ToString()).ToString());
					arrayRemainingYard[5].gameObject.SetActive(value: false);
					yardNumRoot.transform.AddLocalPositionX(REMAINING_YARD_NUM_SPACE * yardNumRoot.transform.localScale.x);
					num -= REMAINING_YARD_NUM_SPACE * yardNumRoot.transform.localScale.x;
					break;
				case 4:
					arrayRemainingYard[4].gameObject.SetActive(value: true);
					arrayRemainingYard[4].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_number_b_" + int.Parse(text[2].ToString()).ToString());
					arrayRemainingYard[5].gameObject.SetActive(value: true);
					arrayRemainingYard[5].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_number_b_" + int.Parse(text[3].ToString()).ToString());
					break;
				}
				break;
			case 2:
				switch (text.Length)
				{
				case 4:
					arrayRemainingYard[4].gameObject.SetActive(value: true);
					arrayRemainingYard[4].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_number_b_" + int.Parse(text[3].ToString()).ToString());
					arrayRemainingYard[5].gameObject.SetActive(value: false);
					yardNumRoot.transform.AddLocalPositionX(REMAINING_YARD_NUM_SPACE * yardNumRoot.transform.localScale.x);
					num -= REMAINING_YARD_NUM_SPACE * yardNumRoot.transform.localScale.x;
					break;
				case 5:
					arrayRemainingYard[4].gameObject.SetActive(value: true);
					arrayRemainingYard[4].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_number_b_" + int.Parse(text[3].ToString()).ToString());
					arrayRemainingYard[5].gameObject.SetActive(value: true);
					arrayRemainingYard[5].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_number_b_" + int.Parse(text[4].ToString()).ToString());
					break;
				}
				break;
			case 3:
				switch (text.Length)
				{
				case 5:
					arrayRemainingYard[4].gameObject.SetActive(value: true);
					arrayRemainingYard[4].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_number_b_" + int.Parse(text[4].ToString()).ToString());
					arrayRemainingYard[5].gameObject.SetActive(value: false);
					yardNumRoot.transform.AddLocalPositionX(REMAINING_YARD_NUM_SPACE * yardNumRoot.transform.localScale.x);
					num -= REMAINING_YARD_NUM_SPACE * yardNumRoot.transform.localScale.x;
					break;
				case 6:
					arrayRemainingYard[4].gameObject.SetActive(value: true);
					arrayRemainingYard[4].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_number_b_" + int.Parse(text[4].ToString()).ToString());
					arrayRemainingYard[5].gameObject.SetActive(value: true);
					arrayRemainingYard[5].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_number_b_" + int.Parse(text[5].ToString()).ToString());
					break;
				}
				break;
			}
		}
		num *= 0.5f;
		remainingYardCenterRoot.transform.SetLocalPositionX(num);
	}
	public void Hide()
	{
		root.SetActive(value: false);
	}
	public float GetResultViewTime()
	{
		return FRAME_SCALE_TIME + RESULT_SCALE_TIME + POINT_ADD_TIME;
	}
}
