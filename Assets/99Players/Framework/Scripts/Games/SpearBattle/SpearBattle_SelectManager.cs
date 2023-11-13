using GamepadInput;
using UnityEngine;
public class SpearBattle_SelectManager : SingletonCustom<SpearBattle_SelectManager>
{
	[SerializeField]
	private Camera[] cameras;
	[SerializeField]
	private CursorManager leftCursorManager;
	[SerializeField]
	private CursorManager rightCursorManager;
	private SpearBattle_Define.SkillType[] leftSkillArray = new SpearBattle_Define.SkillType[5];
	private SpearBattle_Define.SkillType[] rightSkillArray = new SpearBattle_Define.SkillType[5];
	private int leftPlayerNo;
	private int rightPlayerNo;
	private bool isLeftPlayer;
	private bool isRightPlayer;
	private bool isStartWait;
	private bool isSelectEnd;
	private float leftAiWaitTimer;
	private float rightAiWaitTimer;
	public void Init()
	{
		SpearBattle_GameManager.BattleData nowBattleData = SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData();
		leftPlayerNo = nowBattleData.leftCharaData.playerNo;
		rightPlayerNo = nowBattleData.rightCharaData.playerNo;
		isLeftPlayer = nowBattleData.leftCharaData.isPlayer;
		isRightPlayer = nowBattleData.rightCharaData.isPlayer;
		leftCursorManager.SetCursorPos(0, 0);
		rightCursorManager.SetCursorPos(0, 0);
		leftCursorManager.IsStop = false;
		leftCursorManager.TargetPadId = leftPlayerNo;
		rightCursorManager.IsStop = false;
		rightCursorManager.TargetPadId = rightPlayerNo;
		leftAiWaitTimer = Random.Range(0f, 0.5f);
		rightAiWaitTimer = Random.Range(0f, 0.5f);
		isStartWait = true;
		isSelectEnd = false;
		InitSkillArray();
	}
	public void InitSkillArray()
	{
		for (int i = 0; i < leftSkillArray.Length; i++)
		{
			leftSkillArray[i] = SpearBattle_Define.SkillType.Empty;
		}
		for (int j = 0; j < rightSkillArray.Length; j++)
		{
			rightSkillArray[j] = SpearBattle_Define.SkillType.Empty;
		}
	}
	public void SelectStart()
	{
		isStartWait = false;
		SpearBattle_GameManager.BattleData nowBattleData = SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData();
		if (nowBattleData.leftCharaData.isPlayer)
		{
			SingletonCustom<HidVibration>.Instance.SetCommonVibration(nowBattleData.leftCharaData.playerNo);
		}
		if (nowBattleData.rightCharaData.isPlayer)
		{
			SingletonCustom<HidVibration>.Instance.SetCommonVibration(nowBattleData.rightCharaData.playerNo);
		}
	}
	public void UpdateMethod()
	{
		if (!isStartWait && !isSelectEnd)
		{
			Control(leftSkillArray, leftPlayerNo, isLeftPlayer, leftCursorManager.GetSelectNo(), _isLeft: true);
			Control(rightSkillArray, rightPlayerNo, isRightPlayer, rightCursorManager.GetSelectNo(), _isLeft: false);
			SingletonCustom<SpearBattle_SelectUIManager>.Instance.SetDecideActive(_isLeft: true, leftSkillArray);
			SingletonCustom<SpearBattle_SelectUIManager>.Instance.SetDecideActive(_isLeft: false, rightSkillArray);
			if (CheckSelectEnd())
			{
				DoSelectEnd();
			}
			else if (SingletonCustom<SpearBattle_UIManager>.Instance.IsSkipShow && SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.X))
			{
				Skip();
			}
		}
	}
	private void Control(SpearBattle_Define.SkillType[] _skillArray, int _playerNo, bool _isPlayer, int _selectNo, bool _isLeft)
	{
		if (_isPlayer)
		{
			bool flag = false;
			if (_skillArray[_selectNo] == SpearBattle_Define.SkillType.Empty)
			{
				bool flag2 = false;
				if (SpearBattle_Controller.GetRockButtonDown(_playerNo))
				{
					_skillArray[_selectNo] = ((!flag) ? SpearBattle_Define.SkillType.Rock : SpearBattle_Define.SkillType.Special_Rock);
					flag2 = true;
				}
				else if (SpearBattle_Controller.GetScissorsButtonDown(_playerNo))
				{
					_skillArray[_selectNo] = (flag ? SpearBattle_Define.SkillType.Special_Scissors : SpearBattle_Define.SkillType.Scissors);
					flag2 = true;
				}
				else if (SpearBattle_Controller.GetPaperButtonDown(_playerNo))
				{
					_skillArray[_selectNo] = (flag ? SpearBattle_Define.SkillType.Special_Paper : SpearBattle_Define.SkillType.Paper);
					flag2 = true;
				}
				if (flag2)
				{
					if (_selectNo < 4)
					{
						if (_isLeft)
						{
							leftCursorManager.SetCursorPos(0, _selectNo + 1);
						}
						else
						{
							rightCursorManager.SetCursorPos(0, _selectNo + 1);
						}
					}
					SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
				}
			}
			else
			{
				if (SpearBattle_Controller.GetCancelButtonDown(_playerNo))
				{
					_skillArray[_selectNo] = SpearBattle_Define.SkillType.Empty;
					SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
				}
				flag = (_skillArray[_selectNo] == SpearBattle_Define.SkillType.Special_Rock || _skillArray[_selectNo] == SpearBattle_Define.SkillType.Special_Scissors || _skillArray[_selectNo] == SpearBattle_Define.SkillType.Special_Paper);
			}
			if (flag)
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration(_playerNo);
			}
			return;
		}
		bool flag3 = true;
		for (int i = 0; i < leftSkillArray.Length; i++)
		{
			if (leftSkillArray[i] == SpearBattle_Define.SkillType.Empty)
			{
				flag3 = false;
				break;
			}
		}
		bool flag4 = true;
		for (int j = 0; j < rightSkillArray.Length; j++)
		{
			if (rightSkillArray[j] == SpearBattle_Define.SkillType.Empty)
			{
				flag4 = false;
				break;
			}
		}
		if (_isLeft)
		{
			if (flag3)
			{
				return;
			}
			leftAiWaitTimer += Time.deltaTime;
			if (leftAiWaitTimer < 1f)
			{
				return;
			}
			if (flag4)
			{
				leftAiWaitTimer = 0.5f;
			}
			else
			{
				leftAiWaitTimer = 0f;
			}
		}
		else
		{
			if (flag4)
			{
				return;
			}
			rightAiWaitTimer += Time.deltaTime;
			if (rightAiWaitTimer < 1f)
			{
				return;
			}
			if (flag3)
			{
				rightAiWaitTimer = 0.5f;
			}
			else
			{
				rightAiWaitTimer = 0f;
			}
		}
		if (_skillArray[_selectNo] == SpearBattle_Define.SkillType.Empty)
		{
			bool flag5 = false;
			switch (Random.Range(0, 3))
			{
			case 0:
				_skillArray[_selectNo] = ((!flag5) ? SpearBattle_Define.SkillType.Rock : SpearBattle_Define.SkillType.Special_Rock);
				break;
			case 1:
				_skillArray[_selectNo] = (flag5 ? SpearBattle_Define.SkillType.Special_Scissors : SpearBattle_Define.SkillType.Scissors);
				break;
			case 2:
				_skillArray[_selectNo] = (flag5 ? SpearBattle_Define.SkillType.Special_Paper : SpearBattle_Define.SkillType.Paper);
				break;
			}
			if (_selectNo != 4)
			{
				if (_isLeft)
				{
					leftCursorManager.SetCursorPos(0, _selectNo + 1);
				}
				else
				{
					rightCursorManager.SetCursorPos(0, _selectNo + 1);
				}
			}
		}
		else if (_selectNo != 4)
		{
			if (_isLeft)
			{
				leftCursorManager.SetCursorPos(0, _selectNo + 1);
			}
			else
			{
				rightCursorManager.SetCursorPos(0, _selectNo + 1);
			}
		}
	}
	private int SearchSpecialCount(SpearBattle_Define.SkillType[] _skillArray)
	{
		int num = 0;
		for (int i = 0; i < _skillArray.Length; i++)
		{
			if (_skillArray[i] == SpearBattle_Define.SkillType.Special_Rock || _skillArray[i] == SpearBattle_Define.SkillType.Special_Scissors || _skillArray[i] == SpearBattle_Define.SkillType.Special_Paper)
			{
				num++;
			}
		}
		return num;
	}
	private bool CheckSelectEnd()
	{
		for (int i = 0; i < leftSkillArray.Length; i++)
		{
			if (leftSkillArray[i] == SpearBattle_Define.SkillType.Empty)
			{
				return false;
			}
		}
		for (int j = 0; j < rightSkillArray.Length; j++)
		{
			if (rightSkillArray[j] == SpearBattle_Define.SkillType.Empty)
			{
				return false;
			}
		}
		return true;
	}
	private void DoSelectEnd()
	{
		switch (SingletonCustom<SpearBattle_GameManager>.Instance.AiStrength)
		{
		case 0:
			CpuWeaken();
			break;
		case 1:
			CpuLittleWeaken();
			break;
		}
		isSelectEnd = true;
		SingletonCustom<SpearBattle_GameManager>.Instance.SetSkillArray(_isLeft: true, leftSkillArray);
		SingletonCustom<SpearBattle_GameManager>.Instance.SetSkillArray(_isLeft: false, rightSkillArray);
		SingletonCustom<SpearBattle_UIManager>.Instance.Fade(0.5f, 0f, delegate
		{
			SingletonCustom<SpearBattle_GameManager>.Instance.ChangeBattle();
		}, delegate
		{
			if (SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData().IsCpuBattle)
			{
				SingletonCustom<SpearBattle_UIManager>.Instance.ShowSkip();
			}
			else
			{
				SingletonCustom<SpearBattle_UIManager>.Instance.HideSkip();
			}
			SingletonCustom<SpearBattle_BattleManager>.Instance.BattleStart();
		});
	}
	private void CpuWeaken()
	{
		int num = Random.Range(0, 5);
		SpearBattle_GameManager.BattleData nowBattleData = SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData();
		if (nowBattleData.leftCharaData.isPlayer && !nowBattleData.rightCharaData.isPlayer)
		{
			SpearBattle_Define.PhaseResultType phaseResultType = SpearBattle_Define.CalcPhaseResultType(leftSkillArray[num], rightSkillArray[num]);
			if (phaseResultType == SpearBattle_Define.PhaseResultType.Draw || phaseResultType == SpearBattle_Define.PhaseResultType.Lose)
			{
				bool flag = rightSkillArray[num] == SpearBattle_Define.SkillType.Special_Rock || rightSkillArray[num] == SpearBattle_Define.SkillType.Special_Scissors || rightSkillArray[num] == SpearBattle_Define.SkillType.Special_Paper;
				switch (leftSkillArray[num])
				{
				case SpearBattle_Define.SkillType.Rock:
				case SpearBattle_Define.SkillType.Special_Rock:
					rightSkillArray[num] = (flag ? SpearBattle_Define.SkillType.Special_Scissors : SpearBattle_Define.SkillType.Scissors);
					break;
				case SpearBattle_Define.SkillType.Scissors:
				case SpearBattle_Define.SkillType.Special_Scissors:
					rightSkillArray[num] = (flag ? SpearBattle_Define.SkillType.Special_Paper : SpearBattle_Define.SkillType.Paper);
					break;
				case SpearBattle_Define.SkillType.Paper:
				case SpearBattle_Define.SkillType.Special_Paper:
					rightSkillArray[num] = ((!flag) ? SpearBattle_Define.SkillType.Rock : SpearBattle_Define.SkillType.Special_Rock);
					break;
				}
			}
		}
		else
		{
			if (nowBattleData.leftCharaData.isPlayer || !nowBattleData.rightCharaData.isPlayer)
			{
				return;
			}
			SpearBattle_Define.PhaseResultType phaseResultType2 = SpearBattle_Define.CalcPhaseResultType(rightSkillArray[num], leftSkillArray[num]);
			if (phaseResultType2 == SpearBattle_Define.PhaseResultType.Draw || phaseResultType2 == SpearBattle_Define.PhaseResultType.Lose)
			{
				bool flag2 = leftSkillArray[num] == SpearBattle_Define.SkillType.Special_Rock || leftSkillArray[num] == SpearBattle_Define.SkillType.Special_Scissors || leftSkillArray[num] == SpearBattle_Define.SkillType.Special_Paper;
				switch (rightSkillArray[num])
				{
				case SpearBattle_Define.SkillType.Rock:
				case SpearBattle_Define.SkillType.Special_Rock:
					leftSkillArray[num] = (flag2 ? SpearBattle_Define.SkillType.Special_Scissors : SpearBattle_Define.SkillType.Scissors);
					break;
				case SpearBattle_Define.SkillType.Scissors:
				case SpearBattle_Define.SkillType.Special_Scissors:
					leftSkillArray[num] = (flag2 ? SpearBattle_Define.SkillType.Special_Paper : SpearBattle_Define.SkillType.Paper);
					break;
				case SpearBattle_Define.SkillType.Paper:
				case SpearBattle_Define.SkillType.Special_Paper:
					leftSkillArray[num] = ((!flag2) ? SpearBattle_Define.SkillType.Rock : SpearBattle_Define.SkillType.Special_Rock);
					break;
				}
			}
		}
	}
	private void CpuLittleWeaken()
	{
		int num = Random.Range(0, 5);
		SpearBattle_GameManager.BattleData nowBattleData = SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData();
		if (nowBattleData.leftCharaData.isPlayer && !nowBattleData.rightCharaData.isPlayer)
		{
			while (rightSkillArray[num] == SpearBattle_Define.SkillType.Special_Rock || rightSkillArray[num] == SpearBattle_Define.SkillType.Special_Scissors || rightSkillArray[num] == SpearBattle_Define.SkillType.Special_Paper)
			{
				num = Random.Range(0, 5);
			}
			bool flag = rightSkillArray[num] == SpearBattle_Define.SkillType.Special_Rock || rightSkillArray[num] == SpearBattle_Define.SkillType.Special_Scissors || rightSkillArray[num] == SpearBattle_Define.SkillType.Special_Paper;
			switch (SpearBattle_Define.CalcPhaseResultNo(leftSkillArray[num], rightSkillArray[num]))
			{
			case 2:
				switch (leftSkillArray[num])
				{
				case SpearBattle_Define.SkillType.Rock:
				case SpearBattle_Define.SkillType.Special_Rock:
					rightSkillArray[num] = (flag ? SpearBattle_Define.SkillType.Special_Scissors : SpearBattle_Define.SkillType.Scissors);
					break;
				case SpearBattle_Define.SkillType.Scissors:
				case SpearBattle_Define.SkillType.Special_Scissors:
					rightSkillArray[num] = (flag ? SpearBattle_Define.SkillType.Special_Paper : SpearBattle_Define.SkillType.Paper);
					break;
				case SpearBattle_Define.SkillType.Paper:
				case SpearBattle_Define.SkillType.Special_Paper:
					rightSkillArray[num] = ((!flag) ? SpearBattle_Define.SkillType.Rock : SpearBattle_Define.SkillType.Special_Rock);
					break;
				}
				break;
			case 1:
				switch (leftSkillArray[num])
				{
				case SpearBattle_Define.SkillType.Rock:
				case SpearBattle_Define.SkillType.Special_Rock:
					rightSkillArray[num] = ((!flag) ? SpearBattle_Define.SkillType.Rock : SpearBattle_Define.SkillType.Special_Rock);
					break;
				case SpearBattle_Define.SkillType.Scissors:
				case SpearBattle_Define.SkillType.Special_Scissors:
					rightSkillArray[num] = (flag ? SpearBattle_Define.SkillType.Special_Scissors : SpearBattle_Define.SkillType.Scissors);
					break;
				case SpearBattle_Define.SkillType.Paper:
				case SpearBattle_Define.SkillType.Special_Paper:
					rightSkillArray[num] = (flag ? SpearBattle_Define.SkillType.Special_Paper : SpearBattle_Define.SkillType.Paper);
					break;
				}
				break;
			}
		}
		else
		{
			if (nowBattleData.leftCharaData.isPlayer || !nowBattleData.rightCharaData.isPlayer)
			{
				return;
			}
			while (leftSkillArray[num] == SpearBattle_Define.SkillType.Special_Rock || leftSkillArray[num] == SpearBattle_Define.SkillType.Special_Scissors || leftSkillArray[num] == SpearBattle_Define.SkillType.Special_Paper)
			{
				num = Random.Range(0, 5);
			}
			bool flag2 = leftSkillArray[num] == SpearBattle_Define.SkillType.Special_Rock || leftSkillArray[num] == SpearBattle_Define.SkillType.Special_Scissors || leftSkillArray[num] == SpearBattle_Define.SkillType.Special_Paper;
			switch (SpearBattle_Define.CalcPhaseResultNo(rightSkillArray[num], leftSkillArray[num]))
			{
			case 2:
				switch (rightSkillArray[num])
				{
				case SpearBattle_Define.SkillType.Rock:
				case SpearBattle_Define.SkillType.Special_Rock:
					leftSkillArray[num] = (flag2 ? SpearBattle_Define.SkillType.Special_Scissors : SpearBattle_Define.SkillType.Scissors);
					break;
				case SpearBattle_Define.SkillType.Scissors:
				case SpearBattle_Define.SkillType.Special_Scissors:
					leftSkillArray[num] = (flag2 ? SpearBattle_Define.SkillType.Special_Paper : SpearBattle_Define.SkillType.Paper);
					break;
				case SpearBattle_Define.SkillType.Paper:
				case SpearBattle_Define.SkillType.Special_Paper:
					leftSkillArray[num] = ((!flag2) ? SpearBattle_Define.SkillType.Rock : SpearBattle_Define.SkillType.Special_Rock);
					break;
				}
				break;
			case 1:
				switch (rightSkillArray[num])
				{
				case SpearBattle_Define.SkillType.Rock:
				case SpearBattle_Define.SkillType.Special_Rock:
					leftSkillArray[num] = ((!flag2) ? SpearBattle_Define.SkillType.Rock : SpearBattle_Define.SkillType.Special_Rock);
					break;
				case SpearBattle_Define.SkillType.Scissors:
				case SpearBattle_Define.SkillType.Special_Scissors:
					leftSkillArray[num] = (flag2 ? SpearBattle_Define.SkillType.Special_Scissors : SpearBattle_Define.SkillType.Scissors);
					break;
				case SpearBattle_Define.SkillType.Paper:
				case SpearBattle_Define.SkillType.Special_Paper:
					leftSkillArray[num] = (flag2 ? SpearBattle_Define.SkillType.Special_Paper : SpearBattle_Define.SkillType.Paper);
					break;
				}
				break;
			}
		}
	}
	private void Skip()
	{
		for (int i = 0; i < 5; i++)
		{
			bool flag = false;
			if (leftSkillArray[i] == SpearBattle_Define.SkillType.Empty)
			{
				switch (Random.Range(0, 3))
				{
				case 0:
					leftSkillArray[i] = ((!flag) ? SpearBattle_Define.SkillType.Rock : SpearBattle_Define.SkillType.Special_Rock);
					break;
				case 1:
					leftSkillArray[i] = (flag ? SpearBattle_Define.SkillType.Special_Scissors : SpearBattle_Define.SkillType.Scissors);
					break;
				case 2:
					leftSkillArray[i] = (flag ? SpearBattle_Define.SkillType.Special_Paper : SpearBattle_Define.SkillType.Paper);
					break;
				}
			}
			if (rightSkillArray[i] == SpearBattle_Define.SkillType.Empty)
			{
				switch (Random.Range(0, 3))
				{
				case 0:
					rightSkillArray[i] = ((!flag) ? SpearBattle_Define.SkillType.Rock : SpearBattle_Define.SkillType.Special_Rock);
					break;
				case 1:
					rightSkillArray[i] = (flag ? SpearBattle_Define.SkillType.Special_Scissors : SpearBattle_Define.SkillType.Scissors);
					break;
				case 2:
					rightSkillArray[i] = (flag ? SpearBattle_Define.SkillType.Special_Paper : SpearBattle_Define.SkillType.Paper);
					break;
				}
			}
		}
		SingletonCustom<SpearBattle_SelectUIManager>.Instance.SetDecideActive(_isLeft: true, leftSkillArray);
		SingletonCustom<SpearBattle_SelectUIManager>.Instance.SetDecideActive(_isLeft: false, rightSkillArray);
		DoSelectEnd();
	}
}
