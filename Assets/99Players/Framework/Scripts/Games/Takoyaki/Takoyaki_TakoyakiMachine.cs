using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Takoyaki_TakoyakiMachine : MonoBehaviour
{
	public enum MoveCursorDirection
	{
		UP,
		RIGHT,
		LEFT,
		DOWN
	}
	public enum TakoyakiProcessType
	{
		BUTTER_BURRIED,
		INGREDIENTS_PUT_IN,
		BALL_SPIN
	}
	[Serializable]
	public struct TakoBallPointData
	{
		[Header("たこ焼きが入るポイント")]
		public Transform takoBallPoint;
		[Header("たこ焼きの疑似的な生地の画像")]
		public SpriteRenderer takoBatterSp;
		[Header("たこ焼きの疑似的な溢れた生地の画像")]
		public SpriteRenderer takoBatterOverSp;
		[Header("たこ焼きが焼けた合図のキラキラエフェクト")]
		public ParticleSystem takoBallBakeShineFX;
		[Header("穴の番号")]
		public int holeNo;
		[Header("上側の穴の番号")]
		public int upHoleNo;
		[Header("下側の穴の番号")]
		public int downHoleNo;
		[Header("左側の穴の番号")]
		public int leftHoleNo;
		[Header("右側の穴の番号")]
		public int rightHoleNo;
		[Header("たこ焼きが焼ける時間")]
		public float takoBallBakeTime;
	}
	[Serializable]
	public struct TakoBallData
	{
		public Takoyaki_TakoyakiBall takoBall;
		public TakoyakiProcessType processType;
		public bool[] takoBallBake;
		public bool[] takoBallOverBakeAlert;
		public bool[] takoBallOverBake;
		public bool isButterSpFadeOut;
		public void Init()
		{
			takoBall = null;
			processType = TakoyakiProcessType.BUTTER_BURRIED;
			takoBallBake = new bool[2];
			takoBallOverBakeAlert = new bool[2];
			takoBallOverBake = new bool[2];
			isButterSpFadeOut = false;
		}
		public void DataReset()
		{
			takoBall = null;
			processType = TakoyakiProcessType.BUTTER_BURRIED;
			for (int i = 0; i < 2; i++)
			{
				takoBallBake[i] = false;
				takoBallOverBakeAlert[i] = false;
				takoBallOverBake[i] = false;
			}
			isButterSpFadeOut = false;
		}
	}
	[Serializable]
	public struct CursorMoveData
	{
		public int startNo;
		public CursorMoveRootData[] cursorMoveRootDatas;
		public void Init(int _holeNum)
		{
			cursorMoveRootDatas = new CursorMoveRootData[_holeNum];
		}
	}
	[Serializable]
	public struct CursorMoveRootData
	{
		public int targetHoleNo;
		public string moveRootStr;
	}
	[Serializable]
	public struct GridMapData
	{
		[NonReorderable]
		public int[] rootArray_W;
	}
	[SerializeField]
	[Header("たこ焼き機の穴のデ\u30fcタ")]
	private TakoBallPointData[] takoBallPointDatas;
	[SerializeField]
	[Header("たこ焼きのプレハブ")]
	private Takoyaki_TakoyakiBall takoBallPrefab;
	[SerializeField]
	[Header("チャッキリ処理")]
	private Takoyaki_Chakkiri chakkiri;
	[SerializeField]
	[Header("ピック処理")]
	private Takoyaki_Pick pick;
	[SerializeField]
	[Header("油引き処理")]
	private Takoyaki_OilBrush oilBrush;
	[SerializeField]
	[Header("ソ\u30fcスブラシ処理")]
	private Takoyaki_SauceBrush sauceBrush;
	[SerializeField]
	[Header("選択カ\u30fcソル親アンカ\u30fc")]
	private Transform selectCursorRootAnchor;
	[SerializeField]
	[Header("選択カ\u30fcソル画像")]
	private MeshRenderer selectCursor;
	[SerializeField]
	[Header("カ\u30fcソルテクスチャ")]
	private Texture[] cursorTex;
	[SerializeField]
	[Header("具材を入れる際のエフェクト")]
	private ParticleSystem ingredientsPutInFX;
	[SerializeField]
	[Header("たこ焼きが焼けた際の煙エフェクト")]
	private ParticleSystem bakeSmokeFX;
	[SerializeField]
	[Header("たこ焼きが焦げる前の警告用の煙エフェクト")]
	private ParticleSystem overBakeAlertSmokeFX;
	[SerializeField]
	[Header("たこ焼きが焦げた際の煙エフェクト")]
	private ParticleSystem overBakeSmokeFX;
	[SerializeField]
	[Header("GOOD文字のエフェクト")]
	private ParticleSystem goodTextFX;
	[SerializeField]
	[Header("BAD文字のエフェクト")]
	private ParticleSystem badTextFX;
	[SerializeField]
	[Header("たこ焼きを入れる箱のプレハブ")]
	private Takoyaki_TakoBox takoBoxPrefab;
	[SerializeField]
	[Header("たこ焼きを入れる箱の画面外配置アンカ\u30fc")]
	private Transform takoBoxHideAnchor;
	[SerializeField]
	[Header("たこ焼きを入れる箱の画面内配置アンカ\u30fc")]
	private Transform takoBoxShowAnchor;
	[SerializeField]
	[Header("たこ焼きを入れる箱のトッピング時の画面外配置アンカ\u30fc")]
	private Transform takoBox_ToppingHideAnchor;
	[SerializeField]
	[Header("たこ焼きを入れる箱のトッピング時の画面内配置アンカ\u30fc")]
	private Transform takoBox_ToppingShowAnchor;
	private Takoyaki_Player player;
	private TakoBallData[] takoBallDatas;
	private int selectHoleNo;
	private readonly float TAKOBALL_DUMMY_BUTTER_DEF_YPOS = -0.02f;
	private readonly float TAKOBALL_DUMMY_BUTTER_BURRIED_YPOS = 0.0015f;
	private readonly float DUMMY_BUTTER_BURRIED_TIME = 0.25f;
	private readonly float DUMMY_BUTTER_OVER_TIME = 0.01f;
	private readonly float DUMMY_BUTTER_OVER_SCALE = 0.0002f;
	private readonly float DUMMY_BUTTER_FADE_OUT_TIME = 1f;
	private readonly float INGREDIENDTS_PUT_IN_TIME = 0.15f;
	private readonly int DEF_HOLE_NO = 11;
	private bool duringInteractMachine;
	private Takoyaki_TakoBox useTakoBox;
	private TakoyakiProcessType currentProcessType;
	private int currentProcessHoleNo = -1;
	private float[] takoBatterOverPattern = new float[3]
	{
		0f,
		135f,
		270f
	};
	private List<ParticleSystem> smokeFXList = new List<ParticleSystem>();
	[SerializeField]
	[Header("たこ焼き機の穴の縦数")]
	private int TAKOYAKI_MACHINE_HEIGHT_NUM = 6;
	[SerializeField]
	[Header("たこ焼き機の穴の横数")]
	private int TAKOYAKI_MACHINE_WIDTH_NUM = 3;
	private CursorMoveData[] cursorMoveDatas;
	[NonReorderable]
	private GridMapData[] rootArray_H;
	private string rootStr = "";
	public void Init(Takoyaki_Player _player)
	{
		player = _player;
		for (int i = 0; i < takoBallPointDatas.Length; i++)
		{
			takoBallPointDatas[i].takoBatterSp.transform.SetLocalPositionY(TAKOBALL_DUMMY_BUTTER_DEF_YPOS);
			takoBallPointDatas[i].takoBatterSp.SetAlpha(1f);
			takoBallPointDatas[i].takoBatterSp.gameObject.SetActive(value: false);
			takoBallPointDatas[i].takoBatterOverSp.transform.localScale = Vector3.zero;
			takoBallPointDatas[i].takoBatterOverSp.SetAlpha(1f);
			takoBallPointDatas[i].takoBatterOverSp.gameObject.SetActive(value: false);
			takoBallPointDatas[selectHoleNo].takoBatterOverSp.transform.SetLocalEulerAnglesY(takoBatterOverPattern[UnityEngine.Random.Range(0, takoBatterOverPattern.Length)]);
		}
		if (takoBallDatas != null)
		{
			for (int j = 0; j < takoBallDatas.Length; j++)
			{
				if (takoBallDatas[j].takoBall != null)
				{
					UnityEngine.Object.Destroy(takoBallDatas[j].takoBall.gameObject);
				}
			}
		}
		takoBallDatas = new TakoBallData[takoBallPointDatas.Length];
		for (int k = 0; k < takoBallDatas.Length; k++)
		{
			takoBallDatas[k].Init();
		}
		duringInteractMachine = false;
		chakkiri.Init();
		pick.Init();
		oilBrush.Init();
		sauceBrush.Init();
		selectHoleNo = DEF_HOLE_NO;
		selectCursorRootAnchor.position = takoBallPointDatas[selectHoleNo].takoBallPoint.position;
		useTakoBox = UnityEngine.Object.Instantiate(takoBoxPrefab, Vector3.zero, Quaternion.identity, base.transform.parent);
		useTakoBox.Init(player.UserType, takoBoxHideAnchor, takoBoxShowAnchor, takoBox_ToppingHideAnchor, takoBox_ToppingShowAnchor, goodTextFX, badTextFX);
		switch (player.UserType)
		{
		case Takoyaki_Define.UserType.PLAYER_1:
		case Takoyaki_Define.UserType.PLAYER_2:
		case Takoyaki_Define.UserType.PLAYER_3:
		case Takoyaki_Define.UserType.PLAYER_4:
			selectCursor.material.SetTexture("_MainTex", cursorTex[(int)player.UserType]);
			break;
		case Takoyaki_Define.UserType.CPU_1:
		case Takoyaki_Define.UserType.CPU_2:
		case Takoyaki_Define.UserType.CPU_3:
		case Takoyaki_Define.UserType.CPU_4:
		case Takoyaki_Define.UserType.CPU_5:
			selectCursor.gameObject.SetActive(value: false);
			break;
		}
		for (int l = 0; l < takoBallPointDatas.Length; l++)
		{
			takoBallPointDatas[l].takoBallBakeShineFX.gameObject.SetActive(value: false);
			takoBallPointDatas[l].takoBallBakeShineFX.gameObject.SetActive(value: true);
		}
		for (int m = 0; m < smokeFXList.Count; m++)
		{
			if (smokeFXList[m] != null)
			{
				UnityEngine.Object.Destroy(smokeFXList[m].gameObject);
			}
		}
		CreateCursorMoveData();
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < takoBallDatas.Length; i++)
		{
			if (takoBallDatas[i].takoBall != null && takoBallDatas[i].processType == TakoyakiProcessType.BUTTER_BURRIED)
			{
				UnityEngine.Debug.LogError("本来は通ることはない処理：" + player.UserType.ToString());
				takoBallDatas[i].Init();
			}
			else if (takoBallDatas[i].takoBall == null && takoBallDatas[i].processType == TakoyakiProcessType.INGREDIENTS_PUT_IN)
			{
				UnityEngine.Debug.LogError("本来は通ることはない処理：" + player.UserType.ToString());
				takoBallDatas[i].Init();
			}
			else
			{
				if (!(takoBallDatas[i].takoBall != null))
				{
					continue;
				}
				takoBallDatas[i].takoBall.BakeTakoBall();
				for (int j = 0; j < takoBallDatas[i].takoBall.TakoBallBakeStatus.Length; j++)
				{
					if (!takoBallDatas[i].takoBallBake[j] && takoBallDatas[i].takoBall.TakoBallBakeStatus[j] == Takoyaki_Define.TakoBallBakeStatus.Bake)
					{
						takoBallDatas[i].takoBallBake[j] = true;
						ParticleSystem particleSystem = UnityEngine.Object.Instantiate(bakeSmokeFX, Vector3.zero, Quaternion.identity, takoBallPointDatas[i].takoBallPoint);
						particleSystem.transform.localPosition = Vector3.zero;
						particleSystem.transform.localEulerAngles = Vector3.zero;
						particleSystem.transform.localScale = Vector3.one;
						if (takoBallDatas[i].takoBall.GetTakoBallBakeStatus() == Takoyaki_Define.TakoBallBakeStatus.Bake)
						{
							takoBallPointDatas[i].takoBallBakeShineFX.Play();
						}
						smokeFXList.Add(particleSystem);
					}
					if (!takoBallDatas[i].takoBallOverBakeAlert[j] && takoBallDatas[i].takoBall.IsOverBakeAlert)
					{
						takoBallDatas[i].takoBallOverBakeAlert[j] = true;
						ParticleSystem particleSystem2 = UnityEngine.Object.Instantiate(overBakeAlertSmokeFX, Vector3.zero, Quaternion.identity, takoBallPointDatas[i].takoBallPoint);
						particleSystem2.transform.localPosition = Vector3.zero;
						particleSystem2.transform.localEulerAngles = Vector3.zero;
						particleSystem2.transform.localScale = Vector3.one;
						smokeFXList.Add(particleSystem2);
					}
					if (!takoBallDatas[i].takoBallOverBake[j] && takoBallDatas[i].takoBall.TakoBallBakeStatus[j] == Takoyaki_Define.TakoBallBakeStatus.OverBake)
					{
						takoBallDatas[i].takoBallOverBake[j] = true;
						ParticleSystem particleSystem3 = UnityEngine.Object.Instantiate(overBakeSmokeFX, Vector3.zero, Quaternion.identity, takoBallPointDatas[i].takoBallPoint);
						particleSystem3.transform.localPosition = Vector3.zero;
						particleSystem3.transform.localEulerAngles = Vector3.zero;
						particleSystem3.transform.localScale = Vector3.one;
						takoBallPointDatas[i].takoBallBakeShineFX.Stop();
						smokeFXList.Add(particleSystem3);
					}
				}
				if (takoBallDatas[i].takoBall.IsTakoBallHalfForm && !takoBallDatas[i].isButterSpFadeOut)
				{
					takoBallDatas[i].isButterSpFadeOut = true;
					StartCoroutine(SetAlphaColor(takoBallPointDatas[i].takoBatterSp, DUMMY_BUTTER_FADE_OUT_TIME, 0f, _isFadeOut: true));
					StartCoroutine(SetAlphaColor(takoBallPointDatas[i].takoBatterOverSp, DUMMY_BUTTER_FADE_OUT_TIME, 0f, _isFadeOut: true));
				}
			}
		}
		for (int k = 0; k < smokeFXList.Count; k++)
		{
			if (smokeFXList[k] == null)
			{
				smokeFXList.RemoveAt(k);
			}
		}
	}
	private void LateUpdate()
	{
		if (SingletonCustom<CommonNotificationManager>.Instance.IsPause)
		{
			if (SingletonCustom<AudioManager>.Instance.IsSePlaying("se_takoyaki_baking"))
			{
				SingletonCustom<AudioManager>.Instance.SePause("se_takoyaki_baking", _isPause: true);
			}
		}
		else if (!SingletonCustom<AudioManager>.Instance.IsSePlaying("se_takoyaki_baking"))
		{
			SingletonCustom<AudioManager>.Instance.SePause("se_takoyaki_baking", _isPause: false);
		}
	}
	public void MoveCursor(MoveCursorDirection _dir)
	{
		if (!duringInteractMachine && !chakkiri.DuringChakkiriAnimation)
		{
			switch (_dir)
			{
			case MoveCursorDirection.UP:
				selectHoleNo = takoBallPointDatas[selectHoleNo].upHoleNo;
				break;
			case MoveCursorDirection.RIGHT:
				selectHoleNo = takoBallPointDatas[selectHoleNo].rightHoleNo;
				break;
			case MoveCursorDirection.LEFT:
				selectHoleNo = takoBallPointDatas[selectHoleNo].leftHoleNo;
				break;
			case MoveCursorDirection.DOWN:
				selectHoleNo = takoBallPointDatas[selectHoleNo].downHoleNo;
				break;
			}
			selectCursorRootAnchor.position = takoBallPointDatas[selectHoleNo].takoBallPoint.position;
		}
	}
	public void TakoyakiProcessAdvance(bool _isHoldMode)
	{
		if (!duringInteractMachine && (!_isHoldMode || currentProcessHoleNo != selectHoleNo))
		{
			if (!_isHoldMode)
			{
				currentProcessType = takoBallDatas[selectHoleNo].processType;
			}
			currentProcessHoleNo = selectHoleNo;
			switch (currentProcessType)
			{
			case TakoyakiProcessType.BUTTER_BURRIED:
				ButterBurriedTakoBallPoint();
				break;
			case TakoyakiProcessType.INGREDIENTS_PUT_IN:
				IngredientsPutIn();
				break;
			case TakoyakiProcessType.BALL_SPIN:
				TakoBallSpin();
				break;
			}
		}
	}
	public void TakoBallBoxed()
	{
		if (takoBallDatas[selectHoleNo].processType == TakoyakiProcessType.BALL_SPIN && !takoBallDatas[selectHoleNo].takoBall.IsBakingTakoBallHalf && !(useTakoBox == null) && !useTakoBox.IsMaxPutOn && !takoBallDatas[selectHoleNo].takoBall.DuringSpinAnimation)
		{
			useTakoBox.TakoBallPutOnBox(takoBallDatas[selectHoleNo].takoBall);
			takoBallPointDatas[selectHoleNo].takoBatterSp.transform.SetLocalPositionY(TAKOBALL_DUMMY_BUTTER_DEF_YPOS);
			takoBallPointDatas[selectHoleNo].takoBatterSp.SetAlpha(1f);
			takoBallPointDatas[selectHoleNo].takoBatterSp.gameObject.SetActive(value: false);
			takoBallPointDatas[selectHoleNo].takoBatterOverSp.transform.localScale = Vector3.zero;
			takoBallPointDatas[selectHoleNo].takoBatterOverSp.SetAlpha(1f);
			takoBallPointDatas[selectHoleNo].takoBatterOverSp.gameObject.SetActive(value: false);
			takoBallPointDatas[selectHoleNo].takoBatterOverSp.transform.SetLocalEulerAnglesY(takoBatterOverPattern[UnityEngine.Random.Range(0, takoBatterOverPattern.Length)]);
			takoBallPointDatas[selectHoleNo].takoBallBakeShineFX.Stop();
			takoBallDatas[selectHoleNo].DataReset();
			CheckTakoyakiBakingSE();
			oilBrush.PlayOilBrushAnimation(takoBallPointDatas[selectHoleNo].takoBallPoint.position);
			if (useTakoBox.IsMaxPutOn)
			{
				LeanTween.delayedCall(0.25f, (Action)delegate
				{
					useTakoBox.ToppingTakoBox(sauceBrush);
					useTakoBox = UnityEngine.Object.Instantiate(takoBoxPrefab, Vector3.zero, Quaternion.identity, base.transform.parent);
					useTakoBox.Init(player.UserType, takoBoxHideAnchor, takoBoxShowAnchor, takoBox_ToppingHideAnchor, takoBox_ToppingShowAnchor, goodTextFX, badTextFX);
				});
			}
		}
	}
	private void ButterBurriedTakoBallPoint()
	{
		if (takoBallDatas[selectHoleNo].processType == TakoyakiProcessType.BUTTER_BURRIED && !(takoBallDatas[selectHoleNo].takoBall != null) && !chakkiri.DuringChakkiriAnimation)
		{
			duringInteractMachine = true;
			if (player.UserType <= Takoyaki_Define.UserType.PLAYER_4 && !SingletonCustom<AudioManager>.Instance.IsSePlaying("se_takoyaki_baking"))
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_takoyaki_baking", _loop: true);
			}
			chakkiri.PlayChakkiriAnimation(takoBallPointDatas[selectHoleNo].takoBallPoint.position);
			takoBallPointDatas[selectHoleNo].takoBatterSp.gameObject.SetActive(value: true);
			LeanTween.moveLocalY(takoBallPointDatas[selectHoleNo].takoBatterSp.gameObject, TAKOBALL_DUMMY_BUTTER_BURRIED_YPOS, DUMMY_BUTTER_BURRIED_TIME);
			LeanTween.delayedCall(DUMMY_BUTTER_BURRIED_TIME + 0.1f, (Action)delegate
			{
				chakkiri.StopChakkiriAnimation();
				takoBallDatas[selectHoleNo].takoBall = UnityEngine.Object.Instantiate(takoBallPrefab, Vector3.zero, Quaternion.identity, takoBallPointDatas[selectHoleNo].takoBallPoint);
				takoBallDatas[selectHoleNo].takoBall.Init(player.UserType, takoBallPointDatas[selectHoleNo].takoBallBakeTime);
				takoBallPointDatas[selectHoleNo].takoBatterOverSp.gameObject.SetActive(value: true);
				LeanTween.scale(takoBallPointDatas[selectHoleNo].takoBatterOverSp.gameObject, Vector3.one * DUMMY_BUTTER_OVER_SCALE, DUMMY_BUTTER_OVER_TIME).setEaseOutQuart();
				takoBallDatas[selectHoleNo].processType = TakoyakiProcessType.INGREDIENTS_PUT_IN;
				EndInteract();
			});
		}
	}
	private void IngredientsPutIn()
	{
		if (takoBallDatas[selectHoleNo].processType == TakoyakiProcessType.INGREDIENTS_PUT_IN && !(takoBallDatas[selectHoleNo].takoBall == null) && !takoBallDatas[selectHoleNo].takoBall.IsIngredientsPutIn)
		{
			duringInteractMachine = true;
			takoBallDatas[selectHoleNo].processType = TakoyakiProcessType.BALL_SPIN;
			ParticleSystem particleSystem = UnityEngine.Object.Instantiate(ingredientsPutInFX, Vector3.zero, Quaternion.identity, takoBallPointDatas[selectHoleNo].takoBallPoint);
			particleSystem.transform.localPosition = Vector3.zero;
			particleSystem.transform.localEulerAngles = Vector3.zero;
			particleSystem.transform.localScale = Vector3.one;
			takoBallDatas[selectHoleNo].takoBall.ShowIngredients();
			LeanTween.delayedCall(INGREDIENDTS_PUT_IN_TIME, EndInteract);
		}
	}
	private void TakoBallSpin()
	{
		if (takoBallDatas[selectHoleNo].processType == TakoyakiProcessType.BALL_SPIN && takoBallDatas[selectHoleNo].isButterSpFadeOut && takoBallDatas[selectHoleNo].takoBall.IsIngredientsFadeIn && !takoBallDatas[selectHoleNo].takoBall.DuringSpinAnimation)
		{
			takoBallDatas[selectHoleNo].takoBall.TakoBallSpin();
			pick.PlayPickAnimation(takoBallPointDatas[selectHoleNo].takoBallPoint.position);
			if (player.UserType <= Takoyaki_Define.UserType.PLAYER_4)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_takoyaki_spin");
			}
			LeanTween.delayedCall(takoBallDatas[selectHoleNo].takoBall.TAKOBALL_SPIN_TIME, EndInteract);
		}
	}
	private void EndInteract()
	{
		duringInteractMachine = false;
	}
	private void CheckTakoyakiBakingSE()
	{
		if (player.UserType >= Takoyaki_Define.UserType.CPU_1)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < takoBallDatas.Length; i++)
		{
			if (takoBallDatas[i].takoBall != null)
			{
				flag = true;
			}
		}
		if (!flag && SingletonCustom<AudioManager>.Instance.IsSePlaying("se_takoyaki_baking"))
		{
			SingletonCustom<AudioManager>.Instance.SeStop("se_takoyaki_baking");
		}
	}
	private IEnumerator SetAlphaColor(SpriteRenderer _spriteRenderer, float _fadeTime, float _delayTime = 0f, bool _isFadeOut = false)
	{
		float time = 0f;
		float startAlpha = 0f;
		float endAlpha = 1f;
		if (_isFadeOut)
		{
			startAlpha = 1f;
			endAlpha = 0f;
		}
		yield return new WaitForSeconds(_delayTime);
		while (time < _fadeTime)
		{
			_spriteRenderer.SetAlpha(Mathf.Lerp(startAlpha, endAlpha, time / _fadeTime));
			time += Time.deltaTime;
			yield return null;
		}
		_spriteRenderer.SetAlpha(endAlpha);
		if (_isFadeOut)
		{
			_spriteRenderer.gameObject.SetActive(value: false);
		}
	}
	public TakoyakiProcessType GetSelectTakoBallProcess()
	{
		return takoBallDatas[selectHoleNo].processType;
	}
	public Takoyaki_TakoyakiBall GetSelectTakoBall()
	{
		return takoBallDatas[selectHoleNo].takoBall;
	}
	public void StartMoveCursor_AI(int _targetHoleNo, float _cursorDelaySpeed, Action _callBack)
	{
		StartCoroutine(AI_MoveCursorProcess(_targetHoleNo, _cursorDelaySpeed, _callBack));
	}
	private IEnumerator AI_MoveCursorProcess(int _targetHoleNo, float _cursorDelaySpeed, Action _callBack)
	{
		if (selectHoleNo != _targetHoleNo)
		{
			string[] moveHoleNoArray = cursorMoveDatas[selectHoleNo].cursorMoveRootDatas[_targetHoleNo].moveRootStr.Split('_');
			for (int i = 0; i < moveHoleNoArray.Length; i++)
			{
				selectHoleNo = int.Parse(moveHoleNoArray[i]);
				selectCursorRootAnchor.position = takoBallPointDatas[selectHoleNo].takoBallPoint.position;
				yield return new WaitForSeconds(_cursorDelaySpeed);
			}
		}
		_callBack();
	}
	public void TakoyakiProcessAdvance_AI(Action _callBack)
	{
		currentProcessType = takoBallDatas[selectHoleNo].processType;
		switch (currentProcessType)
		{
		case TakoyakiProcessType.BUTTER_BURRIED:
			StartCoroutine(ButterBurriedTakoBallPoint_AI(_callBack));
			break;
		case TakoyakiProcessType.INGREDIENTS_PUT_IN:
			IngredientsPutIn_AI(_callBack);
			break;
		case TakoyakiProcessType.BALL_SPIN:
			if (takoBallDatas[selectHoleNo].takoBall.IsBakingPlaneBaked() && takoBallDatas[selectHoleNo].takoBall.GetTakoBallBakeStatus() == Takoyaki_Define.TakoBallBakeStatus.HalfBake)
			{
				StartCoroutine(TakoBallSpin_AI(_callBack));
			}
			else
			{
				_callBack();
			}
			break;
		}
	}
	private IEnumerator ButterBurriedTakoBallPoint_AI(Action _callBack)
	{
		yield return new WaitUntil(() => !chakkiri.DuringChakkiriAnimation);
		chakkiri.PlayChakkiriAnimation(takoBallPointDatas[selectHoleNo].takoBallPoint.position);
		takoBallPointDatas[selectHoleNo].takoBatterSp.gameObject.SetActive(value: true);
		LeanTween.moveLocalY(takoBallPointDatas[selectHoleNo].takoBatterSp.gameObject, TAKOBALL_DUMMY_BUTTER_BURRIED_YPOS, DUMMY_BUTTER_BURRIED_TIME);
		LeanTween.delayedCall(DUMMY_BUTTER_BURRIED_TIME + 0.1f, (Action)delegate
		{
			chakkiri.StopChakkiriAnimation();
			takoBallDatas[selectHoleNo].takoBall = UnityEngine.Object.Instantiate(takoBallPrefab, Vector3.zero, Quaternion.identity, takoBallPointDatas[selectHoleNo].takoBallPoint);
			takoBallDatas[selectHoleNo].takoBall.Init(player.UserType, takoBallPointDatas[selectHoleNo].takoBallBakeTime);
			takoBallPointDatas[selectHoleNo].takoBatterOverSp.gameObject.SetActive(value: true);
			LeanTween.scale(takoBallPointDatas[selectHoleNo].takoBatterOverSp.gameObject, Vector3.one * DUMMY_BUTTER_OVER_SCALE, DUMMY_BUTTER_OVER_TIME);
			takoBallDatas[selectHoleNo].processType = TakoyakiProcessType.INGREDIENTS_PUT_IN;
			_callBack();
		});
	}
	private void IngredientsPutIn_AI(Action _callBack)
	{
		takoBallDatas[selectHoleNo].processType = TakoyakiProcessType.BALL_SPIN;
		ParticleSystem particleSystem = UnityEngine.Object.Instantiate(ingredientsPutInFX, Vector3.zero, Quaternion.identity, takoBallPointDatas[selectHoleNo].takoBallPoint);
		particleSystem.transform.localPosition = Vector3.zero;
		particleSystem.transform.localEulerAngles = Vector3.zero;
		particleSystem.transform.localScale = Vector3.one;
		takoBallDatas[selectHoleNo].takoBall.ShowIngredients();
		LeanTween.delayedCall(INGREDIENDTS_PUT_IN_TIME, _callBack);
	}
	private IEnumerator TakoBallSpin_AI(Action _callBack)
	{
		yield return new WaitUntil(() => takoBallDatas[selectHoleNo].isButterSpFadeOut);
		yield return new WaitUntil(() => !takoBallDatas[selectHoleNo].takoBall.DuringSpinAnimation);
		yield return new WaitUntil(() => takoBallDatas[selectHoleNo].takoBall.IsIngredientsFadeIn);
		takoBallDatas[selectHoleNo].takoBall.TakoBallSpin();
		pick.PlayPickAnimation(takoBallPointDatas[selectHoleNo].takoBallPoint.position);
		LeanTween.delayedCall(takoBallDatas[selectHoleNo].takoBall.TAKOBALL_SPIN_TIME, _callBack);
	}
	public void TakoBallBoxed_AI(Action _callBack)
	{
		useTakoBox.TakoBallPutOnBox(takoBallDatas[selectHoleNo].takoBall);
		takoBallPointDatas[selectHoleNo].takoBatterSp.transform.SetLocalPositionY(TAKOBALL_DUMMY_BUTTER_DEF_YPOS);
		takoBallPointDatas[selectHoleNo].takoBatterSp.SetAlpha(1f);
		takoBallPointDatas[selectHoleNo].takoBatterSp.gameObject.SetActive(value: false);
		takoBallPointDatas[selectHoleNo].takoBatterOverSp.transform.localScale = Vector3.zero;
		takoBallPointDatas[selectHoleNo].takoBatterOverSp.SetAlpha(1f);
		takoBallPointDatas[selectHoleNo].takoBatterOverSp.gameObject.SetActive(value: false);
		takoBallPointDatas[selectHoleNo].takoBallBakeShineFX.Stop();
		takoBallDatas[selectHoleNo].DataReset();
		CheckTakoyakiBakingSE();
		oilBrush.PlayOilBrushAnimation(takoBallPointDatas[selectHoleNo].takoBallPoint.position);
		if (useTakoBox.IsMaxPutOn)
		{
			LeanTween.delayedCall(0.25f, (Action)delegate
			{
				useTakoBox.ToppingTakoBox(sauceBrush);
				useTakoBox = UnityEngine.Object.Instantiate(takoBoxPrefab, Vector3.zero, Quaternion.identity, base.transform.parent);
				useTakoBox.Init(player.UserType, takoBoxHideAnchor, takoBoxShowAnchor, takoBox_ToppingHideAnchor, takoBox_ToppingShowAnchor, goodTextFX, badTextFX);
				_callBack();
			});
		}
		else
		{
			_callBack();
		}
	}
	private void CreateCursorMoveData()
	{
		cursorMoveDatas = new CursorMoveData[takoBallPointDatas.Length];
		rootArray_H = new GridMapData[TAKOYAKI_MACHINE_HEIGHT_NUM];
		for (int i = 0; i < rootArray_H.Length; i++)
		{
			rootArray_H[i].rootArray_W = new int[TAKOYAKI_MACHINE_WIDTH_NUM];
		}
		for (int j = 0; j < cursorMoveDatas.Length; j++)
		{
			cursorMoveDatas[j].startNo = takoBallPointDatas[j].holeNo;
			cursorMoveDatas[j].Init(takoBallPointDatas.Length);
			for (int k = 0; k < cursorMoveDatas[j].cursorMoveRootDatas.Length; k++)
			{
				if (cursorMoveDatas[j].startNo != k)
				{
					cursorMoveDatas[j].cursorMoveRootDatas[k].targetHoleNo = k;
					CreateRootData(k / TAKOYAKI_MACHINE_WIDTH_NUM, k % TAKOYAKI_MACHINE_WIDTH_NUM);
					cursorMoveDatas[j].cursorMoveRootDatas[k].moveRootStr = GetCalcCursorMoveRoot(cursorMoveDatas[j].startNo / TAKOYAKI_MACHINE_WIDTH_NUM, cursorMoveDatas[j].startNo % TAKOYAKI_MACHINE_WIDTH_NUM);
				}
				else
				{
					cursorMoveDatas[j].cursorMoveRootDatas[k].targetHoleNo = -1;
				}
			}
		}
	}
	private void CreateRootData(int targetY, int targetX)
	{
		for (int i = 0; i < rootArray_H.Length; i++)
		{
			for (int j = 0; j < rootArray_H[i].rootArray_W.Length; j++)
			{
				rootArray_H[i].rootArray_W[j] = 99;
			}
		}
		rootArray_H[targetY].rootArray_W[targetX] = 0;
		CalcRoot(targetY, targetX);
	}
	private void CalcRoot(int y, int x)
	{
		for (int i = 0; i < 4; i++)
		{
			switch (i)
			{
			case 0:
				if (y - 1 >= 0 && rootArray_H[y - 1].rootArray_W[x] > rootArray_H[y].rootArray_W[x] + 1)
				{
					rootArray_H[y - 1].rootArray_W[x] = rootArray_H[y].rootArray_W[x] + 1;
					CalcRoot(y - 1, x);
				}
				break;
			case 1:
				if (x - 1 >= 0 && rootArray_H[y].rootArray_W[x - 1] > rootArray_H[y].rootArray_W[x] + 1)
				{
					rootArray_H[y].rootArray_W[x - 1] = rootArray_H[y].rootArray_W[x] + 1;
					CalcRoot(y, x - 1);
				}
				break;
			case 2:
				if (x + 1 < rootArray_H[y].rootArray_W.Length && rootArray_H[y].rootArray_W[x + 1] > rootArray_H[y].rootArray_W[x] + 1)
				{
					rootArray_H[y].rootArray_W[x + 1] = rootArray_H[y].rootArray_W[x] + 1;
					CalcRoot(y, x + 1);
				}
				break;
			case 3:
				if (y + 1 < rootArray_H.Length && rootArray_H[y + 1].rootArray_W[x] > rootArray_H[y].rootArray_W[x] + 1)
				{
					rootArray_H[y + 1].rootArray_W[x] = rootArray_H[y].rootArray_W[x] + 1;
					CalcRoot(y + 1, x);
				}
				break;
			}
		}
	}
	private string GetCalcCursorMoveRoot(int startY, int startX)
	{
		rootStr = "";
		SearchRoot(startY, startX);
		rootStr = ConvertOneDimensionalArray(rootStr, TAKOYAKI_MACHINE_WIDTH_NUM);
		return rootStr;
	}
	private void SearchRoot(int y, int x)
	{
		if (y - 1 >= 0 && rootArray_H[y - 1].rootArray_W[x] < rootArray_H[y].rootArray_W[x])
		{
			if (rootStr == "")
			{
				rootStr = rootStr + (y - 1).ToString() + "," + x.ToString();
			}
			else
			{
				rootStr = rootStr + "_" + (y - 1).ToString() + "," + x.ToString();
			}
			SearchRoot(y - 1, x);
		}
		else if (x - 1 >= 0 && rootArray_H[y].rootArray_W[x - 1] < rootArray_H[y].rootArray_W[x])
		{
			if (rootStr == "")
			{
				rootStr = rootStr + y.ToString() + "," + (x - 1).ToString();
			}
			else
			{
				rootStr = rootStr + "_" + y.ToString() + "," + (x - 1).ToString();
			}
			SearchRoot(y, x - 1);
		}
		else if (x + 1 < rootArray_H[y].rootArray_W.Length && rootArray_H[y].rootArray_W[x + 1] < rootArray_H[y].rootArray_W[x])
		{
			if (rootStr == "")
			{
				rootStr = rootStr + y.ToString() + "," + (x + 1).ToString();
			}
			else
			{
				rootStr = rootStr + "_" + y.ToString() + "," + (x + 1).ToString();
			}
			SearchRoot(y, x + 1);
		}
		else if (y + 1 < rootArray_H.Length && rootArray_H[y + 1].rootArray_W[x] < rootArray_H[y].rootArray_W[x])
		{
			if (rootStr == "")
			{
				rootStr = rootStr + (y + 1).ToString() + "," + x.ToString();
			}
			else
			{
				rootStr = rootStr + "_" + (y + 1).ToString() + "," + x.ToString();
			}
			SearchRoot(y + 1, x);
		}
	}
	private string ConvertOneDimensionalArray(string _rootStr, int _TwoDimensionalArraySize_X)
	{
		string[] array = _rootStr.Split('_');
		string text = "";
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(',');
			text += (int.Parse(array2[0]) * _TwoDimensionalArraySize_X + int.Parse(array2[1])).ToString();
			if (array.Length - 1 != i)
			{
				text += "_";
			}
		}
		return text;
	}
}
