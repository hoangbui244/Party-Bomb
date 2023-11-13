using UnityEngine;
public class ArenaBattleUIManager : SingletonCustom<ArenaBattleUIManager>
{
	[SerializeField]
	[Header("プレイヤ\u30fc番号表示")]
	private SpriteRenderer[] spPlayerNo;
	[SerializeField]
	[Header("体力ゲ\u30fcジル\u30fcト")]
	private GameObject[] arrayHpGaugeRoot;
	[SerializeField]
	[Header("体力ゲ\u30fcジ")]
	private SpriteRenderer[] arrayHpGauge;
	[SerializeField]
	[Header("必殺技ゲ\u30fcジ")]
	private SpriteMask[] arraySpGauge;
	[SerializeField]
	[Header("必殺技ゲ\u30fcジ画像")]
	private SpriteRenderer[] arraySpGaugeRenderer;
	[SerializeField]
	[Header("必殺技ゲ\u30fcジ白画像")]
	private SpriteRenderer[] arraySpGaugeWhite;
	[SerializeField]
	[Header("必殺技アイコン画像")]
	private SpriteRenderer[] arraySpIconRenderer;
	[SerializeField]
	[Header("操作説明（シングル）")]
	private GameObject objOperationInfoSingle;
	[SerializeField]
	[Header("操作説明（マルチ）")]
	private GameObject objOperationInfoMulti;
	[SerializeField]
	[Header("必殺技アイコンカラ\u30fc")]
	private Color[] arraySpIconColor;
	private float OFFSET_PLAYER_NO_Y = 125f;
	private float OFFSET_CAMERA_ADJUST = 20f;
	private float OFFSET_HP_GAUGE = -55f;
	private Vector2 tempSize;
	public void Init()
	{
		if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
		{
			spPlayerNo[0].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_you");
			objOperationInfoSingle.SetActive(value: true);
		}
		else
		{
			objOperationInfoMulti.SetActive(value: true);
		}
		for (int i = 0; i < arraySpIconRenderer.Length; i++)
		{
			ArenaBattlePlayer playerAtIdx = SingletonCustom<ArenaBattlePlayerManager>.Instance.GetPlayerAtIdx(i);
			arraySpIconRenderer[i].color = arraySpIconColor[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[playerAtIdx.IsCpu ? (4 + (playerAtIdx.PlayerIdx - SingletonCustom<GameSettingManager>.Instance.PlayerNum)) : playerAtIdx.PlayerIdx]];
		}
		LeanTween.value(base.gameObject, 0f, 1f, 0.5f).setOnUpdate(delegate(float _value)
		{
			for (int k = 0; k < arraySpGaugeWhite.Length; k++)
			{
				arraySpGaugeWhite[k].SetAlpha(_value);
			}
		}).setLoopPingPong();
		LeanTween.value(base.gameObject, 1f, 0f, 0.25f).setOnUpdate(delegate(float _value)
		{
			for (int j = 0; j < spPlayerNo.Length; j++)
			{
				spPlayerNo[j].SetAlpha(_value);
			}
		}).setDelay(7f);
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < spPlayerNo.Length; i++)
		{
			if (SingletonCustom<ArenaBattlePlayerManager>.Instance.GetPlayerAtIdx(i).Hp > 0f && SingletonCustom<ArenaBattleGameManager>.Instance.CurrentState != ArenaBattleGameManager.State.EndGame && SingletonCustom<ArenaBattleGameManager>.Instance.CurrentState != ArenaBattleGameManager.State.Result)
			{
				CalcManager.mCalcVector3 = SingletonCustom<ArenaBattleCameraMover>.Instance.GetCamera().WorldToScreenPoint(SingletonCustom<ArenaBattlePlayerManager>.Instance.GetPlayerAtIdx(i).transform.position);
				CalcManager.mCalcVector3 = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(CalcManager.mCalcVector3);
				if (i < SingletonCustom<GameSettingManager>.Instance.PlayerNum)
				{
					spPlayerNo[i].transform.SetPosition(CalcManager.mCalcVector3.x, CalcManager.mCalcVector3.y + OFFSET_PLAYER_NO_Y + OFFSET_CAMERA_ADJUST * SingletonCustom<ArenaBattleCameraMover>.Instance.Adjust, spPlayerNo[i].transform.position.z);
				}
				arrayHpGaugeRoot[i].transform.SetPosition(CalcManager.mCalcVector3.x, CalcManager.mCalcVector3.y + OFFSET_HP_GAUGE, arrayHpGaugeRoot[i].transform.position.z);
				tempSize = arrayHpGauge[i].size;
				tempSize.x = SingletonCustom<ArenaBattlePlayerManager>.Instance.GetPlayerAtIdx(i).Hp * 131f;
				arrayHpGauge[i].size = tempSize;
				arraySpGauge[i].transform.SetLocalScaleX(SingletonCustom<ArenaBattlePlayerManager>.Instance.GetPlayerAtIdx(i).Special * 1.63f);
				if (SingletonCustom<ArenaBattlePlayerManager>.Instance.GetPlayerAtIdx(i).Special >= 1f && !arraySpGaugeWhite[i].gameObject.activeSelf)
				{
					arraySpGaugeWhite[i].gameObject.SetActive(value: true);
				}
				if (SingletonCustom<ArenaBattlePlayerManager>.Instance.GetPlayerAtIdx(i).Special < 1f && arraySpGaugeWhite[i].gameObject.activeSelf)
				{
					arraySpGaugeWhite[i].gameObject.SetActive(value: false);
				}
			}
			else
			{
				spPlayerNo[i].transform.SetLocalPositionX(-2000f);
				arrayHpGaugeRoot[i].transform.SetLocalPositionX(-2000f);
			}
		}
	}
	private new void OnDestroy()
	{
		base.OnDestroy();
		LeanTween.cancel(base.gameObject);
	}
}
