using System.Collections;
using TMPro;
using UnityEngine;
namespace BeachSoccer
{
	public class GameUiManager : SingletonCustom<GameUiManager>
	{
		private string[] schoolName = new string[2];
		[SerializeField]
		[Header("学校名テキスト")]
		private TextMeshPro[] schoolNameText = new TextMeshPro[2];
		[SerializeField]
		[Header("プレイヤ\u30fcスコア")]
		private TextMeshPro[] playerScore = new TextMeshPro[2];
		[SerializeField]
		[Header("対戦時プレイヤ\u30fcアイコン")]
		private SpriteRenderer[] multiPlayerIcon = new SpriteRenderer[4];
		[SerializeField]
		[Header("対戦時プレイヤ\u30fcアイコンアンカ\u30fc")]
		private Transform[] multiPlayerIconAnchor = new Transform[2];
		[SerializeField]
		[Header("スコア背景")]
		private SpriteRenderer[] scoreBack = new SpriteRenderer[2];
		[SerializeField]
		[Header("表裏画像")]
		private SpriteRenderer gameType;
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
		[Header("フェ\u30fcド")]
		private SpriteRenderer fade;
		[SerializeField]
		[Header("加点時の学校名テキスト")]
		private TextMeshPro[] addScoreSchoolNameText = new TextMeshPro[2];
		[SerializeField]
		[Header("加点時のスコアグル\u30fcプ")]
		private GameObject[] addScoreGroup = new GameObject[2];
		[SerializeField]
		[Header("加点時のプレイヤ\u30fc0スコア")]
		private SpriteRenderer[] addScorePlayer0 = new SpriteRenderer[2];
		[SerializeField]
		[Header("加点時のプレイヤ\u30fc1スコア")]
		private SpriteRenderer[] addScorePlayer1 = new SpriteRenderer[2];
		[SerializeField]
		[Header("加点前のスコアグル\u30fcプ")]
		private GameObject[] addBeforeScoreGroup = new GameObject[2];
		[SerializeField]
		[Header("加点前のプレイヤ\u30fc0スコア")]
		private SpriteRenderer[] addBeforeScorePlayer0 = new SpriteRenderer[2];
		[SerializeField]
		[Header("加点前のプレイヤ\u30fc1スコア")]
		private SpriteRenderer[] addBeforeScorePlayer1 = new SpriteRenderer[2];
		[SerializeField]
		[Header("加点時のスコアオブジェクト")]
		private GameObject addScoreObj;
		[SerializeField]
		[Header("ハイフン")]
		private SpriteRenderer hyphen;
		[SerializeField]
		[Header("加点時プレイヤ\u30fcアイコン")]
		private SpriteRenderer[] addScorePlayerIcon = new SpriteRenderer[4];
		[SerializeField]
		[Header("加点時プレイヤ\u30fcアイコンアンカ\u30fc")]
		private Transform[] addScorePlayerIconAnchor = new Transform[2];
		[SerializeField]
		[Header("ハ\u30fcフタイム画像")]
		private SpriteRenderer halfTimeTex;
		[SerializeField]
		[Header("ハ\u30fcフタイムの学校名テキスト")]
		private TextMeshPro[] halfTimeSchoolNameText = new TextMeshPro[2];
		[SerializeField]
		[Header("ハ\u30fcフタイムのプレイヤ\u30fc0スコア")]
		private SpriteRenderer[] halfTimeScorePlayer0 = new SpriteRenderer[2];
		[SerializeField]
		[Header("ハ\u30fcフタイムのプレイヤ\u30fc1スコア")]
		private SpriteRenderer[] halfTimeScorePlayer1 = new SpriteRenderer[2];
		[SerializeField]
		[Header("スコアオブジェクト")]
		private GameObject halfTimeScoreObj;
		[SerializeField]
		[Header("ハ\u30fcフタイムキャラオブジェクト")]
		private CharaUniform[] halfTimeCharaObj;
		[SerializeField]
		[Header("ハイフン")]
		private SpriteRenderer halfTimeHyphen;
		[SerializeField]
		[Header("ハ\u30fcフタイムプレイヤ\u30fcアイコン")]
		private SpriteRenderer[] halfTimePlayerIcon = new SpriteRenderer[4];
		[SerializeField]
		[Header("ハ\u30fcフタイムプレイヤ\u30fcアイコンアンカ\u30fc")]
		private Transform[] halfTimePlayerIconAnchor = new Transform[2];
		[SerializeField]
		[Header("制限時間")]
		private TimeLimitGauge timeLimitGauge;
		[SerializeField]
		[Header("操作情報")]
		private SpriteRenderer operationSprite;
		private int[] score = new int[2];
		private int[] realScore = new int[2];
		private bool isChangeScoreColor;
		private int roundMyHitNum;
		private int roundMyHomerunNum;
		private int roundMyStruckOutNum;
		private int roundEnemyHitNum;
		private int roundEnemyHomerunNum;
		private int roundEnemyStruckOutNum;
		private const float PLAYER_2_ICON_POS_0 = -36.6f;
		private const float PLAYER_2_ICON_POS_1 = 24.9f;
		private const float PLAYER_3_ICON_POS_0 = -66.7f;
		private const float PLAYER_3_ICON_POS_1 = -5.7f;
		private const float PLAYER_3_ICON_POS_2 = 55.4f;
		private const float PLAYER_4_ICON_POS_0 = -99.1f;
		private const float PLAYER_4_ICON_POS_1 = -38.6f;
		private const float PLAYER_4_ICON_POS_2 = 22.2f;
		private const float PLAYER_4_ICON_POS_3 = 83.8f;
		private int[] notChangeFaceUniformNoList = new int[8]
		{
			29,
			35,
			36,
			37,
			38,
			60,
			61,
			62
		};
		private int[] changeFaceGirlUniformNoList = new int[2]
		{
			47,
			48
		};
		private bool IsMulti => GameSaveData.CheckSelectMainGameMode(GameSaveData.MainGameMode.MULTI);
		public bool IsNotJapanese => false;
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
		public void Init()
		{
			InitScore();
			SetSchoolName();
			addScoreObj.SetActive(value: false);
			gameType.gameObject.SetActive(value: false);
		}
		public void TimeLimitStart(TimeLimitGauge.TIME_LIMIT_TYPE _type, int _teamNo)
		{
			timeLimitGauge.TimeStart(_type, _teamNo, 4f);
		}
		public void TimeLimitFinish()
		{
			timeLimitGauge.Finish();
		}
		public bool IsTimeLimitFinish(int _teamNo)
		{
			return timeLimitGauge.IsFinish(_teamNo);
		}
		public void InitScore()
		{
			for (int i = 0; i < score.Length; i++)
			{
				score[i] = 0;
				realScore[i] = 0;
			}
			UpdateScoreDisplay();
		}
		public void AddScore(int _teamNo)
		{
			score[_teamNo]++;
			realScore[_teamNo]++;
			if (score[_teamNo] > 99)
			{
				score[_teamNo] = 99;
			}
			UpdateScoreDisplay();
			SingletonCustom<AudioManager>.Instance.SePlay("se_get_score");
		}
		public void UpdateScoreDisplay()
		{
			int num = 0;
			int num2 = 1;
			if (!SingletonCustom<MainGameManager>.Instance.IsFirstHalf())
			{
				num = 1;
				num2 = 0;
			}
			playerScore[num].text = GetTotalScore(0).ToString();
			playerScore[num2].text = GetTotalScore(1).ToString();
		}
		public void ChangeGameTye(bool _additionalTime = false)
		{
			if (_additionalTime)
			{
				gameType.gameObject.SetActive(value: true);
				return;
			}
			gameType.gameObject.SetActive(value: false);
			for (int i = 0; i < addScoreGroup.Length; i++)
			{
				addScoreGroup[i].transform.SetLocalPositionX(addScoreGroup[i].transform.localPosition.x * -1f);
				addBeforeScoreGroup[i].transform.SetLocalPositionX(addBeforeScoreGroup[i].transform.localPosition.x * -1f);
			}
			UpdateScoreDisplay();
			SetSchoolName(_isLatterHalf: true);
		}
		public void ChangeTimeBackType(bool _additionalTime = false)
		{
		}
		public int GetTotalScore(int _teamNo, bool _real = false)
		{
			if (_real)
			{
				return realScore[_teamNo];
			}
			return score[_teamNo];
		}
		public int GetScore(int _teamNo, int _inningNum, bool _real = false)
		{
			if (_real)
			{
				return realScore[_teamNo];
			}
			return score[_teamNo];
		}
		private void SetSchoolName(bool _isLatterHalf = false)
		{
			int num = 0;
			int num2 = 1;
			if (_isLatterHalf)
			{
				num = 1;
				num2 = 0;
				scoreBack[0].transform.SetLocalEulerAnglesZ(180f);
				scoreBack[1].transform.SetLocalEulerAnglesZ(180f);
			}
			schoolName[num] = SchoolData.GetCommonSchoolName(0, _short: false);
			schoolName[num2] = SchoolData.GetCommonSchoolName(1, _short: false);
			schoolNameText[0].text = schoolName[0].Remove(schoolName[0].Length - 2, 2);
			schoolNameText[1].text = schoolName[1].Remove(schoolName[1].Length - 2, 2);
			if (GameSaveData.CheckSelectMainGameMode(GameSaveData.MainGameMode.MULTI))
			{
				scoreBack[0].gameObject.SetActive(value: false);
				scoreBack[1].gameObject.SetActive(value: true);
				int num3 = 0;
				int num4 = 0;
				for (int i = 0; i < multiPlayerIcon.Length; i++)
				{
					multiPlayerIcon[i].gameObject.SetActive(value: false);
					if (i >= GameSaveData.GetSelectMultiPlayerNum())
					{
						continue;
					}
					multiPlayerIcon[i].gameObject.SetActive(value: true);
					int count = GameSaveData.GetSelectMultiTeamPlayer(num).Count;
					if (i < count)
					{
						int num5 = GameSaveData.GetSelectMultiTeamPlayer(num)[i];
						multiPlayerIcon[num5].transform.parent = multiPlayerIconAnchor[0];
						multiPlayerIcon[num5].transform.SetLocalPositionX(0f);
						switch (count)
						{
						case 2:
							if (num3 == 0)
							{
								multiPlayerIcon[num5].transform.SetLocalPositionX(-36.6f);
							}
							if (num3 == 1)
							{
								multiPlayerIcon[num5].transform.SetLocalPositionX(24.9f);
							}
							break;
						case 3:
							if (num3 == 0)
							{
								multiPlayerIcon[num5].transform.SetLocalPositionX(-66.7f);
							}
							if (num3 == 1)
							{
								multiPlayerIcon[num5].transform.SetLocalPositionX(-5.7f);
							}
							if (num3 == 2)
							{
								multiPlayerIcon[num5].transform.SetLocalPositionX(55.4f);
							}
							break;
						case 4:
							if (num3 == 0)
							{
								multiPlayerIcon[num5].transform.SetLocalPositionX(-99.1f);
							}
							if (num3 == 1)
							{
								multiPlayerIcon[num5].transform.SetLocalPositionX(-38.6f);
							}
							if (num3 == 2)
							{
								multiPlayerIcon[num5].transform.SetLocalPositionX(22.2f);
							}
							if (num3 == 3)
							{
								multiPlayerIcon[num5].transform.SetLocalPositionX(83.8f);
							}
							break;
						}
						num3++;
					}
					count = GameSaveData.GetSelectMultiTeamPlayer(num2).Count;
					if (i >= count)
					{
						continue;
					}
					int num6 = GameSaveData.GetSelectMultiTeamPlayer(num2)[i];
					multiPlayerIcon[num6].transform.parent = multiPlayerIconAnchor[1];
					multiPlayerIcon[num6].transform.SetLocalPositionX(0f);
					switch (count)
					{
					case 2:
						if (num4 == 0)
						{
							multiPlayerIcon[num6].transform.SetLocalPositionX(-36.6f);
						}
						if (num4 == 1)
						{
							multiPlayerIcon[num6].transform.SetLocalPositionX(24.9f);
						}
						break;
					case 3:
						if (num4 == 0)
						{
							multiPlayerIcon[num6].transform.SetLocalPositionX(-66.7f);
						}
						if (num4 == 1)
						{
							multiPlayerIcon[num6].transform.SetLocalPositionX(-5.7f);
						}
						if (num4 == 2)
						{
							multiPlayerIcon[num6].transform.SetLocalPositionX(55.4f);
						}
						break;
					case 4:
						if (num4 == 0)
						{
							multiPlayerIcon[num6].transform.SetLocalPositionX(-99.1f);
						}
						if (num4 == 1)
						{
							multiPlayerIcon[num6].transform.SetLocalPositionX(-38.6f);
						}
						if (num4 == 2)
						{
							multiPlayerIcon[num6].transform.SetLocalPositionX(22.2f);
						}
						if (num4 == 3)
						{
							multiPlayerIcon[num6].transform.SetLocalPositionX(83.8f);
						}
						break;
					}
					num4++;
				}
			}
			else
			{
				scoreBack[0].gameObject.SetActive(value: true);
				scoreBack[1].gameObject.SetActive(value: false);
				for (int j = 0; j < multiPlayerIcon.Length; j++)
				{
					multiPlayerIcon[j].gameObject.SetActive(value: false);
				}
			}
		}
		public void ShowAddScoreObj(int _teamNo)
		{
			addScoreSchoolNameText[0].text = schoolName[0];
			addScoreSchoolNameText[1].text = schoolName[1];
			bool flag = GetTotalScore(0) >= 10 || GetTotalScore(1) >= 10;
			if (flag)
			{
				hyphen.transform.SetLocalScale(0.6f, 0.6f, 1f);
			}
			else
			{
				hyphen.transform.SetLocalScale(1f, 1f, 1f);
			}
			for (int i = 0; i < addScorePlayer0.Length; i++)
			{
				addScorePlayer0[i].gameObject.SetActive((float)GetTotalScore(0) >= Mathf.Pow(10f, i) - (float)((i == 0) ? 1 : 0));
				bool activeSelf = addScorePlayer0[i].gameObject.activeSelf;
				if (GetTotalScore(0) >= 10)
				{
					addScorePlayer0[i].transform.SetLocalPositionX((i == 0) ? 78.3f : (-78.3f));
				}
				else
				{
					addScorePlayer0[i].transform.SetLocalPositionX(0f);
				}
				if (flag)
				{
					addScorePlayer0[i].transform.SetLocalScale(0.85f, 0.85f, 1f);
				}
				else
				{
					addScorePlayer0[i].transform.SetLocalScale(1f, 1f, 1f);
				}
				int num = GetTotalScore(0) - 1;
				addBeforeScorePlayer0[i].gameObject.SetActive((float)num >= Mathf.Pow(10f, i) - (float)((i == 0) ? 1 : 0));
				if (addBeforeScorePlayer0[i].gameObject.activeSelf)
				{
					UnityEngine.Debug.Log("数字 = " + ((float)num % Mathf.Pow(10f, i + 1) / Mathf.Pow(10f, i)).ToString());
				}
				if (num >= 10)
				{
					addBeforeScorePlayer0[i].transform.SetLocalPositionX((i == 0) ? 78.3f : (-78.3f));
				}
				else
				{
					addBeforeScorePlayer0[i].transform.SetLocalPositionX(0f);
				}
				if (flag)
				{
					addBeforeScorePlayer0[i].transform.SetLocalScale(0.85f, 0.85f, 1f);
				}
				else
				{
					addBeforeScorePlayer0[i].transform.SetLocalScale(1f, 1f, 1f);
				}
			}
			for (int j = 0; j < addScorePlayer1.Length; j++)
			{
				addScorePlayer1[j].gameObject.SetActive((float)GetTotalScore(1) >= Mathf.Pow(10f, j) - (float)((j == 0) ? 1 : 0));
				bool activeSelf2 = addScorePlayer1[j].gameObject.activeSelf;
				if (GetTotalScore(1) >= 10)
				{
					addScorePlayer1[j].transform.SetLocalPositionX((j == 0) ? 78.3f : (-78.3f));
				}
				else
				{
					addScorePlayer1[j].transform.SetLocalPositionX(0f);
				}
				if (flag)
				{
					addScorePlayer1[j].transform.SetLocalScale(0.85f, 0.85f, 1f);
				}
				else
				{
					addScorePlayer1[j].transform.SetLocalScale(1f, 1f, 1f);
				}
				int num2 = GetTotalScore(1) - 1;
				if (num2 < 0)
				{
					num2 = 0;
				}
				addBeforeScorePlayer1[j].gameObject.SetActive((float)num2 >= Mathf.Pow(10f, j) - (float)((j == 0) ? 1 : 0));
				bool activeSelf3 = addBeforeScorePlayer1[j].gameObject.activeSelf;
				if (num2 >= 10)
				{
					addBeforeScorePlayer1[j].transform.SetLocalPositionX((j == 0) ? 78.3f : (-78.3f));
				}
				else
				{
					addBeforeScorePlayer1[j].transform.SetLocalPositionX(0f);
				}
				if (flag)
				{
					addBeforeScorePlayer1[j].transform.SetLocalScale(0.85f, 0.85f, 1f);
				}
				else
				{
					addBeforeScorePlayer1[j].transform.SetLocalScale(1f, 1f, 1f);
				}
			}
			if (!GameSaveData.CheckSelectMainGameMode(GameSaveData.MainGameMode.MULTI))
			{
				addScoreObj.transform.SetLocalEulerAnglesZ(0f);
			}
			addScoreObj.SetActive(value: true);
			RotAddScore(_teamNo);
			SetAddScoreCharaUniform(_teamNo);
		}
		private void SetAddScoreCharaUniform(int _addScoreTeamNo = 0)
		{
			int teamNo = 0;
			int teamNo2 = 1;
			if (!SingletonCustom<MainGameManager>.Instance.IsFirstHalf())
			{
				teamNo = 1;
				teamNo2 = 0;
			}
			string[] array = new string[2]
			{
				"face_man_joy",
				"face_man_joy"
			};
			string[] array2 = new string[2]
			{
				"face_man_sorrow",
				"face_man_sorrow"
			};
			if (GameSaveData.CheckSelectMainGameMode(GameSaveData.MainGameMode.MULTI))
			{
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < addScorePlayerIcon.Length; i++)
				{
					addScorePlayerIcon[i].gameObject.SetActive(value: false);
					if (i >= GameSaveData.GetSelectMultiPlayerNum())
					{
						continue;
					}
					addScorePlayerIcon[i].gameObject.SetActive(value: true);
					int count = GameSaveData.GetSelectMultiTeamPlayer(teamNo).Count;
					if (i < count)
					{
						int num3 = GameSaveData.GetSelectMultiTeamPlayer(teamNo)[i];
						addScorePlayerIcon[num3].transform.parent = addScorePlayerIconAnchor[0];
						addScorePlayerIcon[num3].transform.SetLocalPositionX(0f);
						switch (count)
						{
						case 2:
							if (num == 0)
							{
								addScorePlayerIcon[num3].transform.SetLocalPositionX(-45f);
							}
							if (num == 1)
							{
								addScorePlayerIcon[num3].transform.SetLocalPositionX(45f);
							}
							break;
						case 3:
							if (num == 0)
							{
								addScorePlayerIcon[num3].transform.SetLocalPositionX(-90f);
							}
							if (num == 1)
							{
								addScorePlayerIcon[num3].transform.SetLocalPositionX(0f);
							}
							if (num == 2)
							{
								addScorePlayerIcon[num3].transform.SetLocalPositionX(90f);
							}
							break;
						case 4:
							if (num == 0)
							{
								addScorePlayerIcon[num3].transform.SetLocalPositionX(-135f);
							}
							if (num == 1)
							{
								addScorePlayerIcon[num3].transform.SetLocalPositionX(-45f);
							}
							if (num == 2)
							{
								addScorePlayerIcon[num3].transform.SetLocalPositionX(45f);
							}
							if (num == 3)
							{
								addScorePlayerIcon[num3].transform.SetLocalPositionX(135f);
							}
							break;
						}
						num++;
					}
					count = GameSaveData.GetSelectMultiTeamPlayer(teamNo2).Count;
					if (i >= count)
					{
						continue;
					}
					int num4 = GameSaveData.GetSelectMultiTeamPlayer(teamNo2)[i];
					addScorePlayerIcon[num4].transform.parent = addScorePlayerIconAnchor[1];
					addScorePlayerIcon[num4].transform.SetLocalPositionX(0f);
					switch (count)
					{
					case 2:
						if (num2 == 0)
						{
							addScorePlayerIcon[num4].transform.SetLocalPositionX(-45f);
						}
						if (num2 == 1)
						{
							addScorePlayerIcon[num4].transform.SetLocalPositionX(45f);
						}
						break;
					case 3:
						if (num2 == 0)
						{
							addScorePlayerIcon[num4].transform.SetLocalPositionX(-90f);
						}
						if (num2 == 1)
						{
							addScorePlayerIcon[num4].transform.SetLocalPositionX(0f);
						}
						if (num2 == 2)
						{
							addScorePlayerIcon[num4].transform.SetLocalPositionX(90f);
						}
						break;
					case 4:
						if (num2 == 0)
						{
							addScorePlayerIcon[num4].transform.SetLocalPositionX(-135f);
						}
						if (num2 == 1)
						{
							addScorePlayerIcon[num4].transform.SetLocalPositionX(-45f);
						}
						if (num2 == 2)
						{
							addScorePlayerIcon[num4].transform.SetLocalPositionX(45f);
						}
						if (num2 == 3)
						{
							addScorePlayerIcon[num4].transform.SetLocalPositionX(135f);
						}
						break;
					}
					num2++;
				}
			}
			else
			{
				scoreBack[0].gameObject.SetActive(value: true);
				scoreBack[1].gameObject.SetActive(value: false);
				for (int j = 0; j < addScorePlayerIcon.Length; j++)
				{
					addScorePlayerIcon[j].gameObject.SetActive(value: false);
				}
			}
		}
		private void RotAddScore(int _addTeamNo)
		{
			for (int i = 0; i < 2; i++)
			{
				addBeforeScoreGroup[i].transform.SetLocalEulerAnglesY(0f);
				addScoreGroup[i].transform.SetLocalEulerAnglesY(90f);
			}
			if (_addTeamNo == 0)
			{
				addBeforeScoreGroup[0].SetActive(value: true);
				addBeforeScoreGroup[1].SetActive(value: false);
				addScoreGroup[1].transform.SetLocalEulerAnglesY(0f);
			}
			else
			{
				addBeforeScoreGroup[0].SetActive(value: false);
				addBeforeScoreGroup[1].SetActive(value: true);
				addScoreGroup[0].transform.SetLocalEulerAnglesY(0f);
			}
			isChangeScoreColor = true;
			StartCoroutine(_HideAddScoreObj());
		}
		public IEnumerator _HideAddScoreObj()
		{
			yield return new WaitForSeconds(3f);
			addScoreObj.SetActive(value: false);
		}
		public void ShowHalfTime()
		{
			halfTimeSchoolNameText[0].text = schoolName[0];
			halfTimeSchoolNameText[1].text = schoolName[1];
			bool flag = GetTotalScore(0) >= 10 || GetTotalScore(1) >= 10;
			if (flag)
			{
				halfTimeHyphen.transform.SetLocalScale(0.6f, 0.6f, 1f);
			}
			else
			{
				halfTimeHyphen.transform.SetLocalScale(1f, 1f, 1f);
			}
			if (GetTotalScore(0) != GetTotalScore(1))
			{
				GetTotalScore(0);
				GetTotalScore(1);
			}
			for (int i = 0; i < halfTimeScorePlayer0.Length; i++)
			{
				halfTimeScorePlayer0[i].gameObject.SetActive((float)GetTotalScore(0) >= Mathf.Pow(10f, i) - (float)((i == 0) ? 1 : 0));
				bool activeSelf = halfTimeScorePlayer0[i].gameObject.activeSelf;
				if (GetTotalScore(0) >= 10)
				{
					halfTimeScorePlayer0[i].transform.SetLocalPositionX((i == 0) ? 78.3f : (-78.3f));
				}
				else
				{
					halfTimeScorePlayer0[i].transform.SetLocalPositionX(0f);
				}
				if (flag)
				{
					halfTimeScorePlayer0[i].transform.SetLocalScale(0.85f, 0.85f, 1f);
				}
				else
				{
					halfTimeScorePlayer0[i].transform.SetLocalScale(1f, 1f, 1f);
				}
			}
			for (int j = 0; j < halfTimeScorePlayer1.Length; j++)
			{
				halfTimeScorePlayer1[j].gameObject.SetActive((float)GetTotalScore(1) >= Mathf.Pow(10f, j) - (float)((j == 0) ? 1 : 0));
				bool activeSelf2 = halfTimeScorePlayer1[j].gameObject.activeSelf;
				if (GetTotalScore(1) >= 10)
				{
					halfTimeScorePlayer1[j].transform.SetLocalPositionX((j == 0) ? 78.3f : (-78.3f));
				}
				else
				{
					halfTimeScorePlayer1[j].transform.SetLocalPositionX(0f);
				}
				if (flag)
				{
					halfTimeScorePlayer1[j].transform.SetLocalScale(0.85f, 0.85f, 1f);
				}
				else
				{
					halfTimeScorePlayer1[j].transform.SetLocalScale(1f, 1f, 1f);
				}
			}
			if (GameSaveData.CheckSelectMainGameMode(GameSaveData.MainGameMode.MULTI))
			{
				halfTimeScoreObj.transform.SetLocalEulerAnglesZ(180f);
				halfTimeScoreObj.transform.SetLocalEulerAnglesZ(0f);
			}
			else
			{
				halfTimeScoreObj.transform.SetLocalEulerAnglesZ(0f);
			}
			halfTimeScoreObj.SetActive(value: true);
			StartCoroutine(_HideHalfTimeScoreObj());
			SetHalfTimeCharaUniform();
			if (GameSaveData.CheckSelectMainGameMode(GameSaveData.MainGameMode.MULTI))
			{
				int num = 0;
				int num2 = 0;
				for (int k = 0; k < halfTimePlayerIcon.Length; k++)
				{
					halfTimePlayerIcon[k].gameObject.SetActive(value: false);
					if (k >= GameSaveData.GetSelectMultiPlayerNum())
					{
						continue;
					}
					halfTimePlayerIcon[k].gameObject.SetActive(value: true);
					int count = GameSaveData.GetSelectMultiTeamPlayer(0).Count;
					if (k < count)
					{
						int num3 = GameSaveData.GetSelectMultiTeamPlayer(0)[k];
						halfTimePlayerIcon[num3].transform.parent = halfTimePlayerIconAnchor[0];
						halfTimePlayerIcon[num3].transform.SetLocalPositionX(0f);
						switch (count)
						{
						case 2:
							if (num == 0)
							{
								halfTimePlayerIcon[num3].transform.SetLocalPositionX(-45f);
							}
							if (num == 1)
							{
								halfTimePlayerIcon[num3].transform.SetLocalPositionX(45f);
							}
							break;
						case 3:
							if (num == 0)
							{
								halfTimePlayerIcon[num3].transform.SetLocalPositionX(-90f);
							}
							if (num == 1)
							{
								halfTimePlayerIcon[num3].transform.SetLocalPositionX(0f);
							}
							if (num == 2)
							{
								halfTimePlayerIcon[num3].transform.SetLocalPositionX(90f);
							}
							break;
						case 4:
							if (num == 0)
							{
								halfTimePlayerIcon[num3].transform.SetLocalPositionX(-135f);
							}
							if (num == 1)
							{
								halfTimePlayerIcon[num3].transform.SetLocalPositionX(-45f);
							}
							if (num == 2)
							{
								halfTimePlayerIcon[num3].transform.SetLocalPositionX(45f);
							}
							if (num == 3)
							{
								halfTimePlayerIcon[num3].transform.SetLocalPositionX(135f);
							}
							break;
						}
						num++;
					}
					count = GameSaveData.GetSelectMultiTeamPlayer(1).Count;
					if (k >= count)
					{
						continue;
					}
					int num4 = GameSaveData.GetSelectMultiTeamPlayer(1)[k];
					halfTimePlayerIcon[num4].transform.parent = halfTimePlayerIconAnchor[1];
					halfTimePlayerIcon[num4].transform.SetLocalPositionX(0f);
					switch (count)
					{
					case 2:
						if (num2 == 0)
						{
							halfTimePlayerIcon[num4].transform.SetLocalPositionX(-45f);
						}
						if (num2 == 1)
						{
							halfTimePlayerIcon[num4].transform.SetLocalPositionX(45f);
						}
						break;
					case 3:
						if (num2 == 0)
						{
							halfTimePlayerIcon[num4].transform.SetLocalPositionX(-90f);
						}
						if (num2 == 1)
						{
							halfTimePlayerIcon[num4].transform.SetLocalPositionX(0f);
						}
						if (num2 == 2)
						{
							halfTimePlayerIcon[num4].transform.SetLocalPositionX(90f);
						}
						break;
					case 4:
						if (num2 == 0)
						{
							halfTimePlayerIcon[num4].transform.SetLocalPositionX(-135f);
						}
						if (num2 == 1)
						{
							halfTimePlayerIcon[num4].transform.SetLocalPositionX(-45f);
						}
						if (num2 == 2)
						{
							halfTimePlayerIcon[num4].transform.SetLocalPositionX(45f);
						}
						if (num2 == 3)
						{
							halfTimePlayerIcon[num4].transform.SetLocalPositionX(135f);
						}
						break;
					}
					num2++;
				}
			}
			else
			{
				scoreBack[0].gameObject.SetActive(value: true);
				scoreBack[1].gameObject.SetActive(value: false);
				for (int l = 0; l < halfTimePlayerIcon.Length; l++)
				{
					halfTimePlayerIcon[l].gameObject.SetActive(value: false);
				}
			}
		}
		public IEnumerator _HideHalfTimeScoreObj()
		{
			yield return new WaitForSeconds(3f);
			halfTimeScoreObj.SetActive(value: false);
		}
		public bool IsShowHideHalfTimeScoreObj()
		{
			return halfTimeScoreObj.activeSelf;
		}
		private void SetHalfTimeCharaUniform()
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
	}
}
