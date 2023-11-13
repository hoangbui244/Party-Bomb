using System;
using System.Collections;
using UnityEngine;
public class WaterSlider_UIManager : SingletonCustom<WaterSlider_UIManager>
{
	private Camera uiCamera;
	[SerializeField]
	[Header("フェ\u30fcド")]
	private SpriteRenderer fade;
	[SerializeField]
	private GameObject multiPartitionTwo;
	[SerializeField]
	private GameObject multiPartitionFour;
	[SerializeField]
	private GameObject multiHide4P;
	[SerializeField]
	private GameObject UISetTwo;
	[SerializeField]
	private GameObject UISetThree;
	[SerializeField]
	private GameObject UISetFour;
	[SerializeField]
	private GameObject UISetRelayOne;
	[SerializeField]
	private GameObject UISetRelayTwo;
	[SerializeField]
	private SpriteRenderer[] playerSpriteTwo;
	[SerializeField]
	private SpriteRenderer[] playerSpriteRelayOne;
	[SerializeField]
	private SpriteRenderer[] playerSpriteRelayTwo_0;
	[SerializeField]
	private SpriteRenderer[] playerSpriteRelayTwo_1;
	[SerializeField]
	private Sprite[] playerSprite;
	[SerializeField]
	private SpriteRenderer numSpriteOne;
	[SerializeField]
	private SpriteRenderer[] numSpriteTwo;
	[SerializeField]
	private SpriteRenderer[] numSpriteFour;
	[SerializeField]
	private ResultPlacementAnimation resultAnimationOne;
	[SerializeField]
	private ResultPlacementAnimation[] resultAnimationTwo;
	[SerializeField]
	private ResultPlacementAnimation[] resultAnimationFour;
	[SerializeField]
	private Sprite[] numSprite;
	private string[] rankSpriteName = new string[6]
	{
		"_common_rank_s_0",
		"_common_rank_s_1",
		"_common_rank_s_2",
		"_common_rank_s_3",
		"_common_rank_s_4",
		"_common_rank_s_5"
	};
	private string[] lapSpriteName = new string[6]
	{
		"_common_number_b_1",
		"_common_number_b_1",
		"_common_number_b_2",
		"_common_number_b_3",
		"_common_number_b_4",
		"_common_number_b_5"
	};
	[SerializeField]
	private SpriteRenderer rankNumOne;
	[SerializeField]
	private SpriteRenderer[] rankNumTwo;
	[SerializeField]
	private SpriteRenderer[] rankNumFour;
	[SerializeField]
	private GameObject[] rankNumObject;
	[SerializeField]
	[Header("4P操作の表示")]
	private GameObject objFourPlayerInfo;
	[SerializeField]
	[Header("CPU設定のスプライト")]
	private SpriteRenderer renderCpuSprite;
	[SerializeField]
	[Header("CPU操作画像")]
	private Sprite spCpuSprite;
	[SerializeField]
	private GameObject rankNumOneObj;
	[SerializeField]
	private GameObject[] rankNumTwoObj;
	[SerializeField]
	private GameObject[] rankNumFourObj;
	[SerializeField]
	private GameObject[] raceTimeObject;
	[SerializeField]
	private CommonGameTimeUI_Font_Time raceTimeSetOne;
	[SerializeField]
	private CommonGameTimeUI_Font_Time[] raceTimeSetTwo;
	[SerializeField]
	private CommonGameTimeUI_Font_Time[] raceTimeSetFour;
	[SerializeField]
	private SpriteRenderer lapNumOne;
	[SerializeField]
	private SpriteRenderer[] lapNumTwo;
	[SerializeField]
	private SpriteRenderer[] lapNumFour;
	[SerializeField]
	private SpriteRenderer[] lapMaxNumTwo;
	[SerializeField]
	private SpriteRenderer reverseRunOne;
	[SerializeField]
	private SpriteRenderer[] reverseRunTwo;
	[SerializeField]
	private SpriteRenderer[] reverseRunFour;
	[SerializeField]
	private ParticleSystem lastOneEffectOne;
	[SerializeField]
	private ParticleSystem[] lastOneEffectTwo;
	[SerializeField]
	private ParticleSystem[] lastOneEffectFour;
	[SerializeField]
	private GameObject[] singleObjs;
	[SerializeField]
	private GameObject ctrlSingleUI;
	[SerializeField]
	private GameObject ctrlMultiUI;
	[SerializeField]
	private LayoutThreeLive layoutThreeLive;
	[SerializeField]
	private GameObject pauseUI;
	[SerializeField]
	private GameObject goalTextAnchor;
	[SerializeField]
	[Header("一人操作表示")]
	private GameObject objSingleInfo;
	private float goalTextLocalPosX;
	private int playerNum;
	private int[] rankPrev = new int[4];
	private bool isCanChangeRank;
	private float[] numSpriteSize = new float[4];
	private float[] numSpriteSizeStart = new float[4];
	[SerializeField]
	private Material[] lapEffectMaterails = new Material[4];
	private Vector2[] lapEffectSize = new Vector2[4]
	{
		new Vector2(685f, 269f),
		new Vector2(681f, 270f),
		new Vector2(686f, 269f),
		new Vector2(1475f, 313f)
	};
	[SerializeField]
	private TurnCutIn turnCutInOne;
	[SerializeField]
	private TurnCutIn turnCutInTwo_0;
	[SerializeField]
	private TurnCutIn turnCutInTwo_1;
	private bool isSecondGame;
	public bool IsCanChangeRank
	{
		get
		{
			return isCanChangeRank;
		}
		set
		{
			isCanChangeRank = value;
		}
	}
	public void Init()
	{
		uiCamera = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>();
		playerNum = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
		if ((playerNum == 3 || playerNum == 4) && SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
		{
			playerNum = 2;
		}
		if (playerNum == 2)
		{
			multiPartitionTwo.SetActive(value: true);
		}
		else if (playerNum > 2)
		{
			multiPartitionFour.SetActive(value: true);
		}
		for (int i = 0; i < singleObjs.Length; i++)
		{
			singleObjs[i].SetActive(playerNum == 1);
		}
		switch (playerNum)
		{
		case 1:
			rankNumObject[0].SetActive(value: true);
			raceTimeObject[0].SetActive(value: true);
			numSpriteSizeStart[0] = rankNumOne.transform.localScale.x;
			numSpriteSizeStart[1] = (numSpriteSizeStart[2] = (numSpriteSizeStart[3] = 0f));
			break;
		case 2:
			rankNumObject[1].SetActive(value: true);
			raceTimeObject[1].SetActive(value: true);
			UISetTwo.SetActive(value: true);
			for (int k = 0; k < rankNumTwo.Length; k++)
			{
				numSpriteSizeStart[k] = rankNumTwo[k].transform.localScale.x;
			}
			break;
		case 3:
			rankNumObject[2].SetActive(value: true);
			raceTimeObject[2].SetActive(value: true);
			UISetFour.SetActive(value: true);
			for (int l = 0; l < rankNumFour.Length; l++)
			{
				numSpriteSizeStart[l] = rankNumFour[l].transform.localScale.x;
			}
			objFourPlayerInfo.SetActive(value: false);
			if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE)
			{
				renderCpuSprite.sprite = spCpuSprite;
				renderCpuSprite.transform.SetLocalPositionX(90f);
			}
			break;
		case 4:
			rankNumObject[2].SetActive(value: true);
			raceTimeObject[2].SetActive(value: true);
			UISetFour.SetActive(value: true);
			for (int j = 0; j < rankNumFour.Length; j++)
			{
				numSpriteSizeStart[j] = rankNumFour[j].transform.localScale.x;
			}
			objFourPlayerInfo.SetActive(value: true);
			break;
		}
		for (int m = 0; m < numSpriteSize.Length; m++)
		{
			numSpriteSize[m] = numSpriteSizeStart[m];
		}
		for (int n = 0; n < rankPrev.Length; n++)
		{
			rankPrev[n] = n;
		}
		if ((SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3 || SingletonCustom<GameSettingManager>.Instance.PlayerNum == 4) && SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
		{
			playerSpriteTwo[0].sprite = playerSprite[SingletonCustom<WaterSlider_GameManager>.Instance.GetCoopPlayerNo(0)];
			playerSpriteTwo[1].sprite = playerSprite[SingletonCustom<WaterSlider_GameManager>.Instance.GetCoopPlayerNo(1)];
		}
		goalTextLocalPosX = goalTextAnchor.transform.localPosition.x;
		RankNumActive();
		StartRankNum();
		if (SingletonCustom<GameSettingManager>.Instance.PlayerNum > 1)
		{
			objSingleInfo.SetActive(value: false);
		}
	}
	public void NextRunInit()
	{
		playerNum = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
		playerNum = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
		{
			if (playerNum == 3)
			{
				playerNum = 1;
			}
			else
			{
				playerNum = 2;
			}
		}
		if (playerNum == 2)
		{
			multiPartitionTwo.SetActive(value: true);
		}
		else
		{
			multiPartitionTwo.SetActive(value: false);
		}
		for (int i = 0; i < singleObjs.Length; i++)
		{
			singleObjs[i].SetActive(playerNum == 1);
		}
		if (playerNum == 1)
		{
			rankNumObject[0].SetActive(value: true);
			raceTimeObject[0].SetActive(value: true);
			numSpriteSizeStart[0] = rankNumOne.transform.localScale.x;
			numSpriteSizeStart[1] = (numSpriteSizeStart[2] = (numSpriteSizeStart[3] = 0f));
			rankNumObject[1].SetActive(value: false);
			raceTimeObject[1].SetActive(value: false);
			playerSpriteTwo[0].sprite = playerSprite[SingletonCustom<WaterSlider_GameManager>.Instance.GetCoopPlayerNo(2)];
			playerSpriteTwo[0].transform.AddLocalPositionX(570f);
			playerSpriteTwo[1].gameObject.SetActive(value: false);
			SetTime(0, SingletonCustom<WaterSlider_GameManager>.Instance.GetCoopPlayerNo(2), 0f);
		}
		else
		{
			playerSpriteTwo[0].sprite = playerSprite[SingletonCustom<WaterSlider_GameManager>.Instance.GetCoopPlayerNo(2)];
			playerSpriteTwo[1].sprite = playerSprite[SingletonCustom<WaterSlider_GameManager>.Instance.GetCoopPlayerNo(3)];
			SetTime(0, SingletonCustom<WaterSlider_GameManager>.Instance.GetCoopPlayerNo(2), 0f);
			SetTime(1, SingletonCustom<WaterSlider_GameManager>.Instance.GetCoopPlayerNo(3), 0f);
		}
		for (int j = 0; j < numSpriteSize.Length; j++)
		{
			numSpriteSize[j] = numSpriteSizeStart[j];
		}
		for (int k = 0; k < rankPrev.Length; k++)
		{
			rankPrev[k] = k;
		}
		numSpriteOne.gameObject.SetActive(value: false);
		for (int l = 0; l < numSpriteTwo.Length; l++)
		{
			numSpriteTwo[l].gameObject.SetActive(value: false);
		}
		for (int m = 0; m < numSpriteFour.Length; m++)
		{
			numSpriteFour[m].gameObject.SetActive(value: false);
		}
		RankNumActive();
		isSecondGame = true;
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < numSpriteSize.Length; i++)
		{
			if (numSpriteSize[i] < numSpriteSizeStart[i])
			{
				numSpriteSize[i] += Time.deltaTime * 4f;
			}
			else
			{
				numSpriteSize[i] = numSpriteSizeStart[i];
			}
		}
		switch (playerNum)
		{
		case 1:
			rankNumOne.transform.SetLocalScale(numSpriteSize[0], numSpriteSize[0], 0f);
			break;
		case 2:
			for (int k = 0; k < rankNumTwo.Length; k++)
			{
				rankNumTwo[k].transform.SetLocalScale(numSpriteSize[k], numSpriteSize[k], 0f);
			}
			break;
		case 3:
		case 4:
			for (int j = 0; j < rankNumFour.Length; j++)
			{
				rankNumFour[j].transform.SetLocalScale(numSpriteSize[j], numSpriteSize[j], 0f);
			}
			break;
		}
	}
	public void SetThreeLiveData(int[] _rank)
	{
		if (isCanChangeRank)
		{
			layoutThreeLive.SetRankData(_rank);
		}
	}
	public void SetTime(int i, int _playerNo, float time)
	{
		switch (playerNum)
		{
		case 1:
			raceTimeSetOne.SetTime(time, _playerNo);
			break;
		case 2:
			raceTimeSetTwo[i].SetTime(time, _playerNo);
			break;
		case 3:
		case 4:
			raceTimeSetFour[i].SetTime(time, _playerNo);
			break;
		}
	}
	public void SetLapNum(int i, int num)
	{
		switch (playerNum)
		{
		case 1:
			lapNumOne.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, lapSpriteName[num]);
			break;
		case 2:
			lapNumTwo[i].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, lapSpriteName[num]);
			break;
		case 3:
		case 4:
			lapNumFour[i].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, lapSpriteName[num]);
			break;
		}
	}
	public void SetLapMaxNum(int num)
	{
		for (int i = 0; i < lapMaxNumTwo.Length; i++)
		{
			lapMaxNumTwo[i].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, lapSpriteName[num]);
		}
	}
	public void SetNumSprite(int p, int no)
	{
		if (isSecondGame)
		{
			switch (playerNum)
			{
			case 1:
			{
				numSpriteOne.sprite = numSprite[no];
				Vector3 localScale2 = numSpriteOne.transform.localScale;
				numSpriteOne.transform.localScale = Vector3.zero;
				numSpriteOne.gameObject.SetActive(value: true);
				LeanTween.scale(numSpriteOne.gameObject, localScale2, 0.25f);
				LeanTween.delayedCall(numSpriteOne.gameObject, 0.25f, (Action)delegate
				{
					resultAnimationOne.Play();
				});
				break;
			}
			case 2:
			{
				numSpriteTwo[p].sprite = numSprite[no];
				Vector3 localScale = numSpriteTwo[p].transform.localScale;
				numSpriteTwo[p].transform.localScale = Vector3.zero;
				numSpriteTwo[p].gameObject.SetActive(value: true);
				LeanTween.scale(numSpriteTwo[p].gameObject, localScale, 0.25f);
				LeanTween.delayedCall(numSpriteTwo[p].gameObject, 0.25f, (Action)delegate
				{
					resultAnimationTwo[p].Play();
				});
				break;
			}
			}
			return;
		}
		switch (playerNum)
		{
		case 1:
		{
			numSpriteOne.sprite = numSprite[no];
			Vector3 localScale5 = numSpriteOne.transform.localScale;
			numSpriteOne.transform.localScale = Vector3.zero;
			numSpriteOne.gameObject.SetActive(value: true);
			LeanTween.scale(numSpriteOne.gameObject, localScale5, 0.25f);
			LeanTween.delayedCall(numSpriteOne.gameObject, 0.25f, (Action)delegate
			{
				resultAnimationOne.Play();
			});
			break;
		}
		case 2:
		{
			numSpriteTwo[p].sprite = numSprite[no];
			Vector3 localScale4 = numSpriteTwo[p].transform.localScale;
			numSpriteTwo[p].transform.localScale = Vector3.zero;
			numSpriteTwo[p].gameObject.SetActive(value: true);
			LeanTween.scale(numSpriteTwo[p].gameObject, localScale4, 0.25f);
			LeanTween.delayedCall(numSpriteTwo[p].gameObject, 0.25f, (Action)delegate
			{
				resultAnimationTwo[p].Play();
			});
			break;
		}
		case 3:
		case 4:
		{
			numSpriteFour[p].sprite = numSprite[no];
			Vector3 localScale3 = numSpriteFour[p].transform.localScale;
			numSpriteFour[p].transform.localScale = Vector3.zero;
			numSpriteFour[p].gameObject.SetActive(value: true);
			LeanTween.scale(numSpriteFour[p].gameObject, localScale3, 0.25f);
			LeanTween.delayedCall(numSpriteFour[p].gameObject, 0.25f, (Action)delegate
			{
				resultAnimationFour[p].Play();
			});
			break;
		}
		}
	}
	public void StartLapEffect(int _no, int _rap)
	{
		switch (playerNum)
		{
		case 1:
		{
			lastOneEffectOne.GetComponent<Renderer>().material = lapEffectMaterails[_rap];
			Vector2 vector2 = lapEffectSize[_rap];
			ParticleSystem.MainModule main2 = lastOneEffectOne.main;
			main2.startSizeX = vector2.x;
			main2.startSizeY = vector2.y;
			lastOneEffectOne.Play();
			break;
		}
		case 2:
		{
			lastOneEffectTwo[_no].GetComponent<Renderer>().material = lapEffectMaterails[_rap];
			Vector2 vector3 = lapEffectSize[_rap];
			if (_rap == 3)
			{
				vector3 *= 0.6f;
			}
			ParticleSystem.MainModule main3 = lastOneEffectTwo[_no].main;
			main3.startSizeX = vector3.x;
			main3.startSizeY = vector3.y;
			lastOneEffectTwo[_no].Play();
			break;
		}
		case 3:
		case 4:
		{
			lastOneEffectFour[_no].GetComponent<Renderer>().material = lapEffectMaterails[_rap];
			Vector2 vector = lapEffectSize[_rap] * 0.8f;
			if (_rap == 3)
			{
				vector *= 0.6f;
			}
			ParticleSystem.MainModule main = lastOneEffectFour[_no].main;
			main.startSizeX = vector.x;
			main.startSizeY = vector.y;
			lastOneEffectFour[_no].Play();
			break;
		}
		}
	}
	public void StartLastOneEffect(int _no)
	{
		switch (playerNum)
		{
		case 1:
		{
			lastOneEffectOne.GetComponent<Renderer>().material = lapEffectMaterails[3];
			ParticleSystem.MainModule main3 = lastOneEffectOne.main;
			main3.startSizeX = 1473f;
			main3.startSizeY = 288.4f;
			lastOneEffectOne.Play();
			break;
		}
		case 2:
		{
			lastOneEffectTwo[_no].GetComponent<Renderer>().material = lapEffectMaterails[3];
			ParticleSystem.MainModule main2 = lastOneEffectTwo[_no].main;
			main2.startSizeX = 825f;
			main2.startSizeY = 173f;
			lastOneEffectTwo[_no].Play();
			break;
		}
		case 3:
		case 4:
		{
			lastOneEffectFour[_no].GetComponent<Renderer>().material = lapEffectMaterails[3];
			ParticleSystem.MainModule main = lastOneEffectFour[_no].main;
			main.startSizeX = 825f;
			main.startSizeY = 173f;
			lastOneEffectFour[_no].Play();
			break;
		}
		}
	}
	public void ReverseRunON(int p)
	{
		switch (playerNum)
		{
		case 1:
			reverseRunOne.gameObject.SetActive(value: true);
			break;
		case 2:
			reverseRunTwo[p].gameObject.SetActive(value: true);
			break;
		case 3:
		case 4:
			reverseRunFour[p].gameObject.SetActive(value: true);
			break;
		}
	}
	public void ReverseRunOFF(int p)
	{
		switch (playerNum)
		{
		case 1:
			reverseRunOne.gameObject.SetActive(value: false);
			break;
		case 2:
			reverseRunTwo[p].gameObject.SetActive(value: false);
			break;
		case 3:
		case 4:
			reverseRunFour[p].gameObject.SetActive(value: false);
			break;
		}
	}
	public void ReverseReset()
	{
		reverseRunOne.gameObject.SetActive(value: false);
		for (int i = 0; i < reverseRunTwo.Length; i++)
		{
			reverseRunTwo[i].gameObject.SetActive(value: false);
		}
		for (int j = 0; j < reverseRunFour.Length; j++)
		{
			reverseRunFour[j].gameObject.SetActive(value: false);
		}
	}
	public void CloseGameUI()
	{
		numSpriteOne.gameObject.SetActive(value: false);
		for (int i = 0; i < numSpriteTwo.Length; i++)
		{
			numSpriteTwo[i].gameObject.SetActive(value: false);
		}
		for (int j = 0; j < numSpriteFour.Length; j++)
		{
			numSpriteFour[j].gameObject.SetActive(value: false);
		}
		rankNumOneObj.gameObject.SetActive(value: false);
		for (int k = 0; k < rankNumTwo.Length; k++)
		{
			rankNumTwoObj[k].gameObject.SetActive(value: false);
		}
		for (int l = 0; l < rankNumFour.Length; l++)
		{
			rankNumFourObj[l].gameObject.SetActive(value: false);
		}
		layoutThreeLive.gameObject.SetActive(value: false);
		pauseUI.SetActive(value: false);
	}
	public void RankNumActive()
	{
		switch (playerNum)
		{
		case 1:
			rankNumOneObj.gameObject.SetActive(value: true);
			break;
		case 2:
			for (int j = 0; j < rankNumTwo.Length; j++)
			{
				rankNumTwoObj[j].gameObject.SetActive(value: true);
			}
			break;
		case 3:
		case 4:
			for (int i = 0; i < rankNumTwo.Length; i++)
			{
				rankNumTwoObj[i].gameObject.SetActive(value: true);
			}
			break;
		}
	}
	public void CloseRankNum(int p)
	{
		switch (playerNum)
		{
		case 1:
			rankNumOneObj.gameObject.SetActive(value: false);
			break;
		case 2:
			rankNumTwoObj[p].gameObject.SetActive(value: false);
			break;
		case 3:
		case 4:
			rankNumFourObj[p].gameObject.SetActive(value: false);
			break;
		}
	}
	public void ChangeRankNum(int p, int rank)
	{
		if (!isCanChangeRank || rankPrev[p] == rank)
		{
			return;
		}
		rankPrev[p] = rank;
		if (isSecondGame)
		{
			switch (playerNum)
			{
			case 1:
				rankNumOne.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, rankSpriteName[rank]);
				break;
			case 2:
				rankNumTwo[p].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, rankSpriteName[rank]);
				break;
			}
		}
		else
		{
			switch (playerNum)
			{
			case 1:
				rankNumOne.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, rankSpriteName[rank]);
				break;
			case 2:
				rankNumTwo[p].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, rankSpriteName[rank]);
				break;
			case 3:
			case 4:
				rankNumFour[p].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, rankSpriteName[rank]);
				break;
			}
		}
		numSpriteSize[p] = 0f;
	}
	public void StartRankNum()
	{
		for (int i = 0; i < rankPrev.Length; i++)
		{
			rankPrev[i] = -1;
		}
		if (isSecondGame)
		{
			switch (SingletonCustom<GameSettingManager>.Instance.PlayerNum)
			{
			case 1:
				rankNumOne.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, rankSpriteName[0]);
				break;
			case 2:
				for (int j = 0; j < rankNumTwo.Length; j++)
				{
					rankNumTwo[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, rankSpriteName[0]);
				}
				break;
			case 3:
				rankNumOne.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, rankSpriteName[0]);
				break;
			}
			return;
		}
		switch (SingletonCustom<GameSettingManager>.Instance.PlayerNum)
		{
		case 1:
			rankNumOne.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, rankSpriteName[0]);
			break;
		case 2:
			for (int m = 0; m < rankNumTwo.Length; m++)
			{
				rankNumTwo[m].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, rankSpriteName[0]);
			}
			break;
		case 3:
			if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
			{
				for (int n = 0; n < rankNumTwo.Length; n++)
				{
					rankNumTwo[n].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, rankSpriteName[0]);
				}
			}
			else
			{
				for (int num = 0; num < rankNumFour.Length; num++)
				{
					rankNumFour[num].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, rankSpriteName[0]);
				}
			}
			break;
		case 4:
			if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
			{
				for (int k = 0; k < rankNumTwo.Length; k++)
				{
					rankNumTwo[k].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, rankSpriteName[0]);
				}
			}
			else
			{
				for (int l = 0; l < rankNumFour.Length; l++)
				{
					rankNumFour[l].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, rankSpriteName[0]);
				}
			}
			break;
		}
	}
	public void SetRelayPlayerOrderOne(int[] _order)
	{
		for (int i = 0; i < playerSpriteRelayOne.Length; i++)
		{
			playerSpriteRelayOne[i].sprite = playerSprite[_order[i]];
			playerSpriteRelayOne[i].color = ((i == 0) ? Color.white : Color.grey);
		}
	}
	public void SetRelayPlayerOrderTwo(int[] _order0, int[] _order1)
	{
		for (int i = 0; i < playerSpriteRelayTwo_0.Length; i++)
		{
			playerSpriteRelayTwo_0[i].sprite = playerSprite[_order0[i]];
			playerSpriteRelayTwo_0[i].color = ((i == 0) ? Color.white : Color.grey);
			playerSpriteRelayTwo_1[i].sprite = playerSprite[_order1[i]];
			playerSpriteRelayTwo_1[i].color = ((i == 0) ? Color.white : Color.grey);
		}
	}
	public void SetRelayTurnOne(int _newRap, int _turnPlayerNo)
	{
		for (int i = 0; i < playerSpriteRelayOne.Length; i++)
		{
			playerSpriteRelayOne[i].color = (((i + 1) % 3 == _newRap % 3) ? Color.white : Color.grey);
		}
		turnCutInOne.ShowTurnCutIn(_turnPlayerNo, 0.5f);
	}
	public void SetRelayTurnTwo_0(int _newRap)
	{
		for (int i = 0; i < playerSpriteRelayTwo_0.Length; i++)
		{
			playerSpriteRelayTwo_0[i].color = (((i + 1) % 2 == _newRap % 2) ? Color.white : Color.grey);
		}
	}
	public void SetRelayTurnTwo_1(int _newRap)
	{
		for (int i = 0; i < playerSpriteRelayTwo_1.Length; i++)
		{
			playerSpriteRelayTwo_1[i].color = (((i + 1) % 2 == _newRap % 2) ? Color.white : Color.grey);
		}
	}
	public void EndGame()
	{
		multiPartitionTwo.SetActive(value: false);
		multiPartitionFour.SetActive(value: false);
	}
	public void ViewGoal(int p, int rank)
	{
		if (playerNum == 1)
		{
			StartCoroutine(_ViewGoal());
			return;
		}
		switch (playerNum)
		{
		case 1:
			rankNumOneObj.SetActive(value: false);
			raceTimeSetOne.gameObject.SetActive(value: false);
			lapNumOne.gameObject.SetActive(value: false);
			break;
		case 2:
			rankNumTwoObj[p].SetActive(value: false);
			raceTimeSetTwo[p].gameObject.SetActive(value: false);
			lapNumTwo[p].gameObject.SetActive(value: false);
			break;
		case 3:
		case 4:
			rankNumFourObj[p].SetActive(value: false);
			raceTimeSetFour[p].gameObject.SetActive(value: false);
			lapNumFour[p].gameObject.SetActive(value: false);
			break;
		}
		SetNumSprite(p, rank);
	}
	private IEnumerator _ViewGoal()
	{
		goalTextAnchor.SetActive(value: true);
		goalTextAnchor.transform.SetLocalPositionX(goalTextLocalPosX);
		LeanTween.moveLocalX(goalTextAnchor, 0f, 0.75f).setEaseOutBack();
		yield return new WaitForSeconds(2f);
		LeanTween.moveLocalX(goalTextAnchor, 0f - goalTextLocalPosX, 0.5f).setEaseOutQuad();
		yield return new WaitForSeconds(0.5f);
		goalTextAnchor.SetActive(value: false);
	}
	public void Fade(float _time, float _delay, Action _act)
	{
		Color color = fade.color;
		color.a = 1f;
		fade.SetAlpha(0f);
		fade.gameObject.SetActive(value: true);
		LeanTween.value(fade.gameObject, 0f, 1f, _time * 0.5f).setDelay(_delay).setEaseOutCubic()
			.setOnUpdate(delegate(float _value)
			{
				fade.SetAlpha(_value);
			})
			.setOnComplete((Action)delegate
			{
				_act();
			});
		color.a = 0f;
		LeanTween.value(fade.gameObject, 1f, 0f, _time * 0.5f).setDelay(_time * 0.5f + _delay).setEaseOutCubic()
			.setOnUpdate(delegate(float _value)
			{
				fade.SetAlpha(_value);
			})
			.setOnComplete((Action)delegate
			{
				fade.gameObject.SetActive(fade);
			});
	}
}
