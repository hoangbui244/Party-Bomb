using System;
using TMPro;
using UnityEngine;
public class Golf_ViewCupYardUI : MonoBehaviour
{
	[SerializeField]
	[Header("ル\u30fcト")]
	private GameObject root;
	private Vector3 originRootScale;
	[SerializeField]
	[Header("ル\u30fcトのボ\u30fcルとの補正座標")]
	private Vector3 diffRootPos;
	[SerializeField]
	[Header("残りヤ\u30fcドの中央ル\u30fcト")]
	private Transform remainingYardCenterRoot;
	[SerializeField]
	[Header("残りヤ\u30fcド")]
	private TextMeshPro remainingYard;
	[SerializeField]
	[Header("１文字の間隔")]
	private float YARD_SPACE;
	private int MAX_REMAINING_YARD_LENGTH;
	[SerializeField]
	[Header("ル\u30fcトを拡大する時間（ラインの表示するまでの待機時間と合わせる）")]
	private float ROOT_SCALE_TIME;
	[SerializeField]
	[Header("残りヤ\u30fcドを更新する時間（ラインの表示する時間と合わせる）")]
	private float YARD_UPDATE_TIME;
	public void Init()
	{
		originRootScale = root.transform.localScale;
		MAX_REMAINING_YARD_LENGTH = SingletonCustom<Golf_FieldManager>.Instance.GetHole().GetReadyBallPosToCupDistance().ToString()
			.Length;
			root.SetActive(value: false);
		}
		public void InitPlay()
		{
			remainingYardCenterRoot.transform.localPosition = Vector3.zero;
			remainingYard.text = "0.00";
			root.transform.localScale = Vector3.zero;
		}
		public void Show()
		{
			Vector3 position = SingletonCustom<Golf_BallManager>.Instance.GetTurnPlayerBall().transform.position;
			float remainingDistanceToCup = SingletonCustom<Golf_BallManager>.Instance.GetRemainingDistanceToCup();
			UnityEngine.Debug.Log("remainingDistance " + remainingDistanceToCup.ToString());
			int num = remainingDistanceToCup.ToString().Length - MAX_REMAINING_YARD_LENGTH;
			remainingYardCenterRoot.transform.SetLocalPositionX((float)num * YARD_SPACE);
			CalcManager.mCalcVector3 = SingletonCustom<Golf_CameraManager>.Instance.GetCamera().GetCameraObj().WorldToScreenPoint(position);
			CalcManager.mCalcVector3 = SingletonCustom<Golf_UIManager>.Instance.GetGlobalCamera().ScreenToWorldPoint(CalcManager.mCalcVector3);
			root.transform.SetPosition(CalcManager.mCalcVector3.x, CalcManager.mCalcVector3.y + diffRootPos.y, root.transform.position.z);
			root.SetActive(value: true);
			LeanTween.scale(root, originRootScale, ROOT_SCALE_TIME).setEaseOutBack();
			LeanTween.delayedCall(base.gameObject, ROOT_SCALE_TIME, (Action)delegate
			{
				float time = 1f;
				LeanTween.value(base.gameObject, 0f, remainingDistanceToCup, YARD_UPDATE_TIME).setOnUpdate(delegate(float _value)
				{
					time += Time.deltaTime;
					if (time > 0.1f)
					{
						time = 0f;
						SingletonCustom<AudioManager>.Instance.SePlay("se_cursor_move", _loop: false, 0f, 0.5f);
					}
					_value *= 100f;
					_value = Mathf.Floor(_value) / 100f;
					remainingYard.text = _value.ToString("0.00");
				}).setOnComplete((Action)delegate
				{
					remainingYard.text = remainingDistanceToCup.ToString();
				});
			});
		}
		public void Hide()
		{
			root.SetActive(value: false);
		}
	}
