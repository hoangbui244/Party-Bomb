using System;
using System.Collections;
using UnityEngine;
public class GoldfishCharacterScript : MonoBehaviour
{
	private const float POI_RADIUS = 0.09f;
	private const float POI_DANGER_INTERVAL_DIS = 0.15f;
	[SerializeField]
	private GoldfishPoi poi;
	private bool isPlayer;
	private int playerNo;
	private int charaNo;
	private int teamNo;
	private bool isWaterInWait;
	private bool isScoopWait;
	private static readonly bool[] AI_IS_CHECK_DONUT = new bool[3]
	{
		false,
		false,
		true
	};
	private static readonly float[] AI_DONUT_HOLE_RADIUS = new float[3]
	{
		0f,
		0f,
		0.03f
	};
	private bool isAiCheckDonut;
	private float aiDonutHoleRadius;
	private Vector2 aiStickDir;
	private int aiStickDirChangeCount;
	public bool IsPlayer => isPlayer;
	public int PlayerNo => playerNo;
	public int CharaNo => charaNo;
	public int TeamNo => teamNo;
	public bool IsScoopAnim => poi.IsAnimation;
	public bool IsEnd => poi.IsBreakPaper;
	public GameObject PoiObj => poi.gameObject;
	public void Init(int _charaNo)
	{
		int teamNum = SingletonCustom<GoldfishGameManager>.Instance.TeamNum;
		charaNo = _charaNo;
		playerNo = SingletonCustom<GoldfishGameManager>.Instance.GetCharaPlayerNo(charaNo);
		teamNo = SingletonCustom<GoldfishGameManager>.Instance.GetCharaTeamNo(charaNo);
		isPlayer = (playerNo < 4);
		SetPoiMaterial(SingletonCustom<GoldfishCharacterManager>.Instance.GetPoiMaterial(charaNo));
		DataInit();
	}
	public void SecondGroupInit()
	{
		DataInit();
	}
	private void DataInit()
	{
		poi.Init();
		poi.Chara = this;
	}
	public void UpdateMethod()
	{
		poi.UpdateMethod();
		if (poi.IsWaterIn && poi.WaterInMoveLength > 0.15f)
		{
			SingletonCustom<GoldfishTargetManager>.Instance.DangerCheck(poi.GetPoiPos(), 0.135f);
			poi.WaterInMoveLength -= 0.15f;
		}
	}
	public void WaterInAnimation()
	{
		if (!poi.IsWaterIn && !poi.IsAnimation && !isWaterInWait && !isScoopWait)
		{
			if (poi.IsAnimation)
			{
				StartCoroutine(_WaterInWait());
			}
			else
			{
				WaterIn();
			}
		}
	}
	private IEnumerator _WaterInWait()
	{
		isWaterInWait = true;
		while (poi.IsAnimation)
		{
			yield return null;
		}
		WaterIn();
		isWaterInWait = false;
	}
	private void WaterIn()
	{
		poi.Move(Vector3.zero);
		poi.WaterInAnimationStart();
		SingletonCustom<GoldfishTargetManager>.Instance.DangerCheck(poi.GetPoiPos(), 0.135f);
	}
	public void ScoopAnimation()
	{
		if ((poi.IsWaterIn || poi.IsWaterInAnim) && !isWaterInWait && !isScoopWait)
		{
			if (poi.IsAnimation)
			{
				StartCoroutine(_ScoopWait());
			}
			else
			{
				Scoop();
			}
		}
	}
	private IEnumerator _ScoopWait()
	{
		isScoopWait = true;
		while (poi.IsAnimation)
		{
			yield return null;
		}
		Scoop();
		isScoopWait = false;
	}
	private void Scoop()
	{
		poi.Move(Vector3.zero);
		poi.ScoopAnimationStart(delegate
		{
			GoldfishCharacterScript goldfishCharacterScript = this;
			GoldfishTarget[] array = SingletonCustom<GoldfishTargetManager>.Instance.GetOnTriggerFishArray(poi.transform.position, 0.09f);
			float num = 0f;
			for (int i = 0; i < array.Length; i++)
			{
				num += array[i].CalcDamage(poi.transform.position, 0.09f);
			}
			poi.AddDamage(num);
			if (!poi.IsBreakPaper)
			{
				int getPoint = 0;
				int getCount = 0;
				bool isRareGet = false;
				for (int j = 0; j < array.Length; j++)
				{
					array[j].Scoop(poi.GetPoiAnchor(), charaNo);
					getPoint += array[j].Point;
					getCount++;
					if (array[j].IsKing || array[j].IsGold)
					{
						isRareGet = true;
					}
				}
				if (getPoint > 0)
				{
					if (isPlayer)
					{
						SingletonCustom<HidVibration>.Instance.SetCustomVibration(playerNo, HidVibration.VibrationType.Weak, 0.5f);
					}
					LeanTween.delayedCall(0.3f, (Action)delegate
					{
						SingletonCustom<GoldfishGameManager>.Instance.AddScore(goldfishCharacterScript.charaNo, getPoint);
						SingletonCustom<GoldfishUiManager>.Instance.ScoreUpdate(goldfishCharacterScript.charaNo);
						if (goldfishCharacterScript.isPlayer)
						{
							if (isRareGet)
							{
								SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_perfect");
							}
							else
							{
								getCount = Mathf.Clamp(getCount, 1, 3);
								for (int k = 0; k < getCount; k++)
								{
									if (k == getCount - 1)
									{
										SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_good", _loop: false, 0f, 1f, 1f, 0.08f * (float)k);
									}
									else
									{
										SingletonCustom<AudioManager>.Instance.SePlay("se_goldfish_subgood", _loop: false, 0f, 1f, 1f, 0.08f * (float)k);
									}
								}
							}
						}
						for (int l = 0; l < array.Length; l++)
						{
							SingletonCustom<GoldfishUiManager>.Instance.ViewGetPoint(goldfishCharacterScript.charaNo, array[l].Point, array[l].transform.position);
						}
					});
				}
			}
			else if (isPlayer)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_bad");
			}
		});
	}
	public void SetPoiMaterial(Material _mat)
	{
		poi.SetMaterial(_mat);
	}
	public GoldfishPoi GetPoi()
	{
		return poi;
	}
	public void CursorControl(Vector2 _stickDir)
	{
		Vector3 vec = new Vector3(_stickDir.x, 0f, _stickDir.y);
		poi.Move(vec);
	}
	public void AiInit()
	{
		int aiStrength = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		isAiCheckDonut = AI_IS_CHECK_DONUT[aiStrength];
		aiDonutHoleRadius = AI_DONUT_HOLE_RADIUS[aiStrength];
	}
	public void AiStickDirSetting()
	{
		if (aiStickDirChangeCount == 0)
		{
			float f = UnityEngine.Random.Range(0f, 360f) * ((float)Math.PI / 180f);
			aiStickDir.x = Mathf.Sin(f) / 2f;
			aiStickDir.y = Mathf.Cos(f) / 2f;
		}
		else
		{
			aiStickDir = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(-90f, 90f)) * aiStickDir;
		}
		aiStickDirChangeCount++;
		if (aiStickDirChangeCount > 2)
		{
			aiStickDirChangeCount = 0;
		}
	}
	public void AiCursorUpdate()
	{
		CursorControl(aiStickDir);
	}
	public void AiCheckScoop()
	{
		if (poi.IsWaterIn)
		{
			if (SingletonCustom<GoldfishTargetManager>.Instance.GetOnTriggerFishArray(poi.transform.position, 0.09f, isAiCheckDonut, aiDonutHoleRadius).Length != 0)
			{
				ScoopAnimation();
			}
		}
		else if (SingletonCustom<GoldfishTargetManager>.Instance.GetOnTriggerFishArray(poi.transform.position, 0.09f).Length != 0)
		{
			WaterInAnimation();
			aiStickDir = Vector2.zero;
		}
	}
}
