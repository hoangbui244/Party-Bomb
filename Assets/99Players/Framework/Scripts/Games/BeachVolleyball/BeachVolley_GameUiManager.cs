using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class BeachVolley_GameUiManager : SingletonCustom<BeachVolley_GameUiManager>
{
	[SerializeField]
	[Header("操作説明")]
	private SpriteRenderer controllerHelp;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコンチ\u30fcム1")]
	private SpriteRenderer[] playerIconTeam1;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコンチ\u30fcム2")]
	private SpriteRenderer[] playerIconTeam2;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン親")]
	private Transform[] playerIconsParent;
	private int[] length = new int[2];
	private Vector3[] startPosPlayerIconsParent = new Vector3[2];
	[SerializeField]
	[Header("操作方法画像")]
	private SpriteRenderer buttonSprite;
	private string[] schoolName = new string[2];
	[SerializeField]
	[Header("学校名テキスト")]
	private TextMeshPro[] schoolNameText = new TextMeshPro[2];
	private Vector3[] schoolNameStartPos = new Vector3[2];
	[SerializeField]
	[Header("プレイヤ\u30fcスコア")]
	private TextMeshPro[] playerScore = new TextMeshPro[2];
	private Vector3[] playerScoreStartPos = new Vector3[2];
	[SerializeField]
	[Header("セット番号")]
	private SpriteRenderer quarterNo;
	[SerializeField]
	[Header("時間背景画像")]
	private SpriteRenderer timeBack;
	[SerializeField]
	[Header("UIオブジェクト")]
	private Transform uiObject;
	[SerializeField]
	[Header("対戦用UIアンカ\u30fc")]
	private Transform multiUiAnchor;
	[SerializeField]
	[Header("プレイヤ\u30fcマ\u30fcク")]
	private GameObject[] playerMark;
	[SerializeField]
	[Header("チ\u30fcムA所属プレイヤ\u30fc")]
	private SpriteRenderer[] arrayTeamPlayerA;
	[SerializeField]
	[Header("チ\u30fcムB所属プレイヤ\u30fc")]
	private SpriteRenderer[] arrayTeamPlayerB;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン")]
	private SpriteRenderer[] arrayPlayerIcon;
	[SerializeField]
	[Header("所属プレイヤ\u30fc画像")]
	private Sprite[] arrayTeamPlayerSprite;
	[SerializeField]
	[Header("チ\u30fcムAフレ\u30fcム")]
	private SpriteRenderer teamColorFrameA;
	[SerializeField]
	[Header("チ\u30fcムBフレ\u30fcム")]
	private SpriteRenderer teamColorFrameB;
	[SerializeField]
	[Header("チ\u30fcムフレ\u30fcム画像")]
	private Sprite[] arrayTeamColorFrame;
	[SerializeField]
	[Header("一人用表示画像")]
	private Sprite[] arraySinglePlayerName;
	private static readonly string[] CHARA_DEFAULT_SPRITE_NAMES = new string[8]
	{
		"character_yuto_02",
		"character_hina_02",
		"character_ituki_02",
		"character_souta_02",
		"character_takumi_02",
		"character_rin_02",
		"character_akira_02",
		"character_rui_02"
	};
	[SerializeField]
	[Header("フェ\u30fcド")]
	private SpriteRenderer fade;
	[SerializeField]
	[Header("タッチ制限回数アンカ\u30fc")]
	private GameObject limitPassCntAnchor;
	[SerializeField]
	[Header("タッチ制限回数")]
	private SpriteRenderer limitPassCntTex;
	[SerializeField]
	[Header("タッチ制限スプライト")]
	private Sprite[] limitPassCntSprite;
	private bool isChangeScoreColor;
	private int roundMyHitNum;
	private int roundMyHomerunNum;
	private int roundMyStruckOutNum;
	private int roundEnemyHitNum;
	private int roundEnemyHomerunNum;
	private int roundEnemyStruckOutNum;
	[SerializeField]
	[Header("選手マ\u30fcク矢印マネ\u30fcジャ\u30fc")]
	private BeachVolley_CharaMarkArrowManager charaMarkArrowManager;
	[SerializeField]
	[Header("セット獲得数")]
	private GameObject[] sets;
	private Vector3[] setsStartPos = new Vector3[2];
	[SerializeField]
	[Header("プレイヤ\u30fc1セット獲得数")]
	private GameObject[] set1;
	[SerializeField]
	[Header("プレイヤ\u30fc2セット獲得数")]
	private GameObject[] set2;
	[SerializeField]
	[Header("得点時のＵＩ")]
	private GameObject[] pointGetUI;
	private Vector3[] pointGetUIStartPos = new Vector3[2];
	[SerializeField]
	[Header("得点加算演出")]
	private BeachVolley_AddScoreProduction addScoreProduction;
	private bool IsMulti => !SingletonCustom<GameSettingManager>.Instance.IsSinglePlay;
	public bool IsNotJapanese => Application.systemLanguage != SystemLanguage.Japanese;
	public string LanguageConvertValue
	{
		get
		{
			if (!IsNotJapanese)
			{
				return "";
			}
			return "_english";
		}
	}
	public void ChangeSchoolNamePos()
	{
		if (Vector3.Distance(schoolNameText[0].transform.position, schoolNameStartPos[0]) < 0.1f)
		{
			schoolNameText[0].transform.SetPosition(schoolNameStartPos[1].x, schoolNameStartPos[1].y, schoolNameStartPos[1].z);
			schoolNameText[1].transform.SetPosition(schoolNameStartPos[0].x, schoolNameStartPos[0].y, schoolNameStartPos[0].z);
			playerScore[0].transform.SetPosition(playerScoreStartPos[1].x, playerScoreStartPos[1].y, playerScoreStartPos[1].z);
			playerScore[1].transform.SetPosition(playerScoreStartPos[0].x, playerScoreStartPos[0].y, playerScoreStartPos[0].z);
			sets[0].transform.SetPosition(setsStartPos[1].x, setsStartPos[1].y, setsStartPos[1].z);
			sets[1].transform.SetPosition(setsStartPos[0].x, setsStartPos[0].y, setsStartPos[0].z);
			pointGetUI[0].transform.SetPosition(pointGetUIStartPos[1].x, pointGetUIStartPos[1].y, pointGetUIStartPos[1].z);
			pointGetUI[1].transform.SetPosition(pointGetUIStartPos[0].x, pointGetUIStartPos[0].y, pointGetUIStartPos[0].z);
			playerIconsParent[0].SetLocalPositionX(startPosPlayerIconsParent[1].x - (float)(33 * length[0]));
			playerIconsParent[1].SetLocalPositionX(startPosPlayerIconsParent[0].x - (float)(33 * length[1]));
		}
		else
		{
			schoolNameText[0].transform.SetPosition(schoolNameStartPos[0].x, schoolNameStartPos[0].y, schoolNameStartPos[0].z);
			schoolNameText[1].transform.SetPosition(schoolNameStartPos[1].x, schoolNameStartPos[1].y, schoolNameStartPos[1].z);
			playerScore[0].transform.SetPosition(playerScoreStartPos[0].x, playerScoreStartPos[0].y, playerScoreStartPos[0].z);
			playerScore[1].transform.SetPosition(playerScoreStartPos[1].x, playerScoreStartPos[1].y, playerScoreStartPos[1].z);
			sets[0].transform.SetPosition(setsStartPos[0].x, setsStartPos[0].y, setsStartPos[0].z);
			sets[1].transform.SetPosition(setsStartPos[1].x, setsStartPos[1].y, setsStartPos[1].z);
			pointGetUI[0].transform.SetPosition(pointGetUIStartPos[0].x, pointGetUIStartPos[0].y, pointGetUIStartPos[0].z);
			pointGetUI[1].transform.SetPosition(pointGetUIStartPos[1].x, pointGetUIStartPos[1].y, pointGetUIStartPos[1].z);
			playerIconsParent[0].SetLocalPositionX(startPosPlayerIconsParent[0].x - (float)(33 * length[0]));
			playerIconsParent[1].SetLocalPositionX(startPosPlayerIconsParent[1].x - (float)(33 * length[1]));
		}
	}
	public void SetSetCounter(int _teamNo, int _set)
	{
		switch (_teamNo)
		{
		case 0:
			switch (_set)
			{
			case 2:
				set1[1].SetActive(value: true);
				set1[0].SetActive(value: false);
				break;
			case 1:
				set1[0].SetActive(value: true);
				break;
			}
			break;
		case 1:
			switch (_set)
			{
			case 2:
				set2[1].SetActive(value: true);
				set2[0].SetActive(value: false);
				break;
			case 1:
				set2[0].SetActive(value: true);
				break;
			}
			break;
		}
	}
	public void Init()
	{
		SetSchoolName();
		startPosPlayerIconsParent[0] = playerIconsParent[0].localPosition;
		startPosPlayerIconsParent[1] = playerIconsParent[1].localPosition;
		if (BeachVolley_MainCharacterManager.IsGameWatchingMode())
		{
			controllerHelp.gameObject.SetActive(value: false);
		}
		schoolNameStartPos[0] = schoolNameText[0].transform.position;
		schoolNameStartPos[1] = schoolNameText[1].transform.position;
		playerScoreStartPos[0] = playerScore[0].transform.position;
		playerScoreStartPos[1] = playerScore[1].transform.position;
		setsStartPos[0] = sets[0].transform.position;
		setsStartPos[1] = sets[1].transform.position;
		pointGetUIStartPos[0] = pointGetUI[0].transform.position;
		pointGetUIStartPos[1] = pointGetUI[1].transform.position;
		HideLimitPassCnt(0);
		HideLimitPassCnt(1);
		charaMarkArrowManager.Init();
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				if (i == 0)
				{
					list.Add((int)BeachVolley_Define.MCM.TeamList[i].charas[j].UserType);
				}
				else
				{
					list2.Add((int)BeachVolley_Define.MCM.TeamList[i].charas[j].UserType);
				}
			}
		}
		list.Sort();
		list2.Sort();
		int num = 0;
		int num2 = 0;
		for (int k = 0; k < arrayTeamPlayerA.Length; k++)
		{
			arrayTeamPlayerA[k].gameObject.SetActive(value: false);
		}
		for (int l = 0; l < arrayTeamPlayerB.Length; l++)
		{
			arrayTeamPlayerB[l].gameObject.SetActive(value: false);
		}
		for (int m = 0; m < list.Count; m++)
		{
			num2 = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[list[m]];
			if (list[m] <= SingletonCustom<GameSettingManager>.Instance.PlayerNum)
			{
				arrayTeamPlayerA[m].gameObject.SetActive(value: true);
				arrayTeamPlayerA[m].sprite = arrayTeamPlayerSprite[list[m]];
				if (m == 0 && SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
				{
					arrayTeamPlayerA[m].sprite = arraySinglePlayerName[(Localize_Define.Language != 0) ? 1 : 0];
				}
			}
			arrayPlayerIcon[num].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, CHARA_DEFAULT_SPRITE_NAMES[num2]);
			num++;
		}
		for (int n = 0; n < list2.Count; n++)
		{
			num2 = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[list2[n]];
			if (list2[n] <= SingletonCustom<GameSettingManager>.Instance.PlayerNum)
			{
				arrayTeamPlayerB[n].gameObject.SetActive(value: true);
				arrayTeamPlayerB[n].sprite = arrayTeamPlayerSprite[list2[n]];
			}
			arrayPlayerIcon[num].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, CHARA_DEFAULT_SPRITE_NAMES[num2]);
			num++;
		}
		int[] array = new int[2]
		{
			SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[list[0]],
			SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[list2[0]]
		};
		if ((array[0] == 3 && array[1] == 7) || (array[0] == 7 && array[1] == 3))
		{
			for (int num3 = 0; num3 < list2.Count; num3++)
			{
				if (list2[num3] >= 0 && list2[num3] < GS_Define.CHARACTER_MAX && SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[list2[num3]] != 3 && SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[list2[num3]] != 7)
				{
					array[1] = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[list2[num3]];
					break;
				}
			}
		}
		if ((array[0] == 2 && array[1] == 5) || (array[0] == 5 && array[1] == 2))
		{
			for (int num4 = 0; num4 < list2.Count; num4++)
			{
				if (list2[num4] >= 0 && list2[num4] < GS_Define.CHARACTER_MAX && SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[list2[num4]] != 2 && SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[list2[num4]] != 5)
				{
					array[1] = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[list2[num4]];
					break;
				}
			}
		}
		teamColorFrameA.sprite = arrayTeamColorFrame[array[0]];
		teamColorFrameB.sprite = arrayTeamColorFrame[array[1]];
	}
	public void UpdateMethod()
	{
		charaMarkArrowManager.UpdateMethod();
	}
	public void HideLimitPassCnt(int _teamNo)
	{
		limitPassCntAnchor.SetActive(value: false);
		if (!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			SingletonCustom<BeachVolley_CharacterNameManager>.Instance.HideCnt(_teamNo);
		}
	}
	public void SetLimitPassCnt(int _teamNo, int _num, int _playerNo = -1)
	{
		limitPassCntAnchor.SetActive(value: true);
		limitPassCntTex.sprite = limitPassCntSprite[_num];
	}
	public void StartTimeLimit(int _teamNo, float _time)
	{
		SingletonCustom<BeachVolley_CharacterNameManager>.Instance.GetCharacterName(_teamNo).StartTimeLimit(_time);
	}
	public void FinishTimeLimit(int _teamNo)
	{
		SingletonCustom<BeachVolley_CharacterNameManager>.Instance.GetCharacterName(_teamNo).FinishTimeLimit();
	}
	public bool IsTimeLimitFinish(int _teamNo)
	{
		return SingletonCustom<BeachVolley_CharacterNameManager>.Instance.GetCharacterName(_teamNo).IsFinishTimeLimit();
	}
	public void UpdateScoreDisplay(int _teamNo, int _score)
	{
		playerScore[_teamNo].text = _score.ToString();
	}
	public float PlayAddScoreProduction(int _teamNo, int[] _score)
	{
		addScoreProduction.ShowAddScoreObj(_teamNo, schoolName, _score);
		return addScoreProduction.SHOW_TIME;
	}
	public void SetSetNo(int _setNo)
	{
	}
	public void ChangeTimeBackType(bool _additionalTime = false)
	{
	}
	public int GetTotalScore(int _teamNo, bool _real = false)
	{
		return 0;
	}
	public int GetWinSetCount(int _teamNo, bool _real = false)
	{
		return 0;
	}
	public bool CheckSetPerfect(int _setNo, int _teamNo, bool _real = false)
	{
		return false;
	}
	public bool CheckResultPerfect(int _teamNo, bool _real = false)
	{
		return false;
	}
	private void SetSchoolName()
	{
	}
	public void Fade(float _time, float _delay)
	{
	}
	public Transform GetUiObject()
	{
		return uiObject;
	}
	public void AddHitNum(bool _myFlg)
	{
		if (_myFlg)
		{
			roundMyHitNum++;
		}
		else
		{
			roundEnemyHitNum++;
		}
	}
	public void AddHomerunNum(bool _myFlg)
	{
		if (_myFlg)
		{
			roundMyHomerunNum++;
		}
		else
		{
			roundEnemyHomerunNum++;
		}
	}
	public void AddStruckOutNum(bool _myFlg)
	{
		if (_myFlg)
		{
			roundMyStruckOutNum++;
		}
		else
		{
			roundEnemyStruckOutNum++;
		}
	}
	public int GetHitNum(bool _myFlg)
	{
		if (_myFlg)
		{
			return roundMyHitNum;
		}
		return roundEnemyHitNum;
	}
	public int GetHomerunNum(bool _myFlg)
	{
		if (_myFlg)
		{
			return roundMyHomerunNum;
		}
		return roundEnemyHomerunNum;
	}
	public int GetStruckOutNum(bool _myFlg)
	{
		if (_myFlg)
		{
			return roundMyStruckOutNum;
		}
		return roundEnemyStruckOutNum;
	}
	public void TutorialInit()
	{
		uiObject.gameObject.SetActive(value: false);
		charaMarkArrowManager.gameObject.SetActive(value: false);
	}
	public void TutorialUpdate()
	{
		charaMarkArrowManager.UpdateMethod();
	}
}
