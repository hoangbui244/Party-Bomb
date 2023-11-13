using GamepadInput;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class SpearBattle_BattleManager : SingletonCustom<SpearBattle_BattleManager>
{
	private const bool IS_MANUAL = false;
	private const string HORSE_SPEED_ANIM = "Speed_f";
	[SerializeField]
	private Camera duelCamera;
	[SerializeField]
	private Transform[] duelCameraAnchors;
	[SerializeField]
	private Camera leftCamera;
	[SerializeField]
	private Camera rightCamera;
	[SerializeField]
	private ParticleSystem battleEffect;
	private SpearBattle_Define.SkillType[] leftSkillArray = new SpearBattle_Define.SkillType[5];
	private SpearBattle_Define.SkillType[] rightSkillArray = new SpearBattle_Define.SkillType[5];
	private SpearBattle_Define.SkillType[] leftCpuBattleSkillArray;
	private SpearBattle_Define.SkillType[] rightCpuBattleSkillArray;
	private int leftPlayerNo;
	private int rightPlayerNo;
	private bool isLeftPlayer;
	private bool isRightPlayer;
	private int leftScore;
	private int rightScore;
	private int phaseNo;
	private int duelCameraAnchorNo;
	private bool isStartWait;
	private bool isBattleEnd;
	private bool isDirectionWait;
	private Coroutine directionCoroutine;
	private bool isStartInit;
	private Vector3 leftStartPos;
	private Vector3 rightStartPos;
	private Vector3 leftStartEuler;
	private Vector3 rightStartEuler;
	private bool isCtrlWait;
	private bool isHorseStay;
	private float selectTimer;
	private float leftAiWaitTimer;
	private float rightAiWaitTimer;
	private bool isCpuWeakEnd;
	public bool IsBattleEnd => isBattleEnd;
	public void Init()
	{
		SpearBattle_GameManager.BattleData nowBattleData = SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData();
		leftPlayerNo = nowBattleData.leftCharaData.playerNo;
		rightPlayerNo = nowBattleData.rightCharaData.playerNo;
		isLeftPlayer = nowBattleData.leftCharaData.isPlayer;
		isRightPlayer = nowBattleData.rightCharaData.isPlayer;
		for (int i = 0; i < leftSkillArray.Length; i++)
		{
			leftSkillArray[i] = SpearBattle_Define.SkillType.Empty;
			rightSkillArray[i] = SpearBattle_Define.SkillType.Empty;
		}
		leftScore = 0;
		rightScore = 0;
		phaseNo = 0;
		isStartWait = true;
		isBattleEnd = false;
		isDirectionWait = false;
		directionCoroutine = null;
		selectTimer = 5f;
		isCpuWeakEnd = false;
		CpuBattleSkillSetting();
		CameraChange(_isDuel: false);
		SelectStart();
		HorseStay();
		SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: true).PlayAnimation(_isLeft: true, _isSpeedStart: false);
		SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: false).PlayAnimation(_isLeft: false, _isSpeedStart: false);
		SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetHideActive(_isLeft: true, _active: false);
		SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetHideActive(_isLeft: false, _active: false);
		SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetSkillActive(_isLeft: true, _active: false);
		SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetSkillActive(_isLeft: false, _active: false);
		for (int j = 0; j < 5; j++)
		{
			SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetResultActive(_isLeft: true, j, _active: false, 0);
			SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetResultActive(_isLeft: false, j, _active: false, 0);
		}
	}
	public void BattleStart()
	{
		isStartWait = false;
	}
	public void UpdateMethod()
	{
		Transform cameraAnchor = SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: true).GetCameraAnchor();
		leftCamera.transform.position = cameraAnchor.position;
		leftCamera.transform.rotation = cameraAnchor.rotation;
		cameraAnchor = SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: false).GetCameraAnchor();
		rightCamera.transform.position = cameraAnchor.position;
		rightCamera.transform.rotation = cameraAnchor.rotation;
		if (isStartWait || isBattleEnd)
		{
			return;
		}
		if (SingletonCustom<SpearBattle_UIManager>.Instance.IsSkipShow && SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.X))
		{
			if (directionCoroutine != null)
			{
				StopCoroutine(directionCoroutine);
			}
			Skip();
		}
		else
		{
			if (!isCtrlWait)
			{
				return;
			}
			selectTimer -= Time.deltaTime;
			SingletonCustom<SpearBattle_UIManager>.Instance.SetTimeValue(Mathf.Max(0f, selectTimer));
			Control(leftSkillArray, leftPlayerNo, isLeftPlayer, phaseNo, _isLeft: true);
			Control(rightSkillArray, rightPlayerNo, isRightPlayer, phaseNo, _isLeft: false);
			if (selectTimer <= 0f)
			{
				if (leftSkillArray[phaseNo] == SpearBattle_Define.SkillType.Empty)
				{
					List<SpearBattle_Define.SkillType> list = new List<SpearBattle_Define.SkillType>();
					if (CheckIsCanSelect(SpearBattle_Define.SkillType.Rock, leftSkillArray))
					{
						list.Add(SpearBattle_Define.SkillType.Rock);
					}
					if (CheckIsCanSelect(SpearBattle_Define.SkillType.Scissors, leftSkillArray))
					{
						list.Add(SpearBattle_Define.SkillType.Scissors);
					}
					if (CheckIsCanSelect(SpearBattle_Define.SkillType.Paper, leftSkillArray))
					{
						list.Add(SpearBattle_Define.SkillType.Paper);
					}
					leftSkillArray[phaseNo] = list[UnityEngine.Random.Range(0, list.Count)];
					SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
					SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetSkillActive(_isLeft: true, phaseNo, leftSkillArray[phaseNo]);
					SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetHideActive(_isLeft: true, phaseNo, _active: true);
				}
				if (rightSkillArray[phaseNo] == SpearBattle_Define.SkillType.Empty)
				{
					List<SpearBattle_Define.SkillType> list2 = new List<SpearBattle_Define.SkillType>();
					if (CheckIsCanSelect(SpearBattle_Define.SkillType.Rock, rightSkillArray))
					{
						list2.Add(SpearBattle_Define.SkillType.Rock);
					}
					if (CheckIsCanSelect(SpearBattle_Define.SkillType.Scissors, rightSkillArray))
					{
						list2.Add(SpearBattle_Define.SkillType.Scissors);
					}
					if (CheckIsCanSelect(SpearBattle_Define.SkillType.Paper, rightSkillArray))
					{
						list2.Add(SpearBattle_Define.SkillType.Paper);
					}
					rightSkillArray[phaseNo] = list2[UnityEngine.Random.Range(0, list2.Count)];
					SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
					SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetSkillActive(_isLeft: false, phaseNo, rightSkillArray[phaseNo]);
					SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetHideActive(_isLeft: false, phaseNo, _active: true);
				}
			}
			if (leftSkillArray[phaseNo] != 0 && rightSkillArray[phaseNo] != 0 && isHorseStay)
			{
				isCtrlWait = false;
				isHorseStay = false;
				SingletonCustom<SpearBattle_UIManager>.Instance.SetSelectUIActive(_active: false);
				selectTimer = 5f;
				SingletonCustom<SpearBattle_UIManager>.Instance.SetTimeActive(_active: false);
				SingletonCustom<SpearBattle_UIManager>.Instance.SetTimeValue(selectTimer);
				CpuWeakCheck();
				LeanTween.delayedCall(base.gameObject, 0.5f, (Action)delegate
				{
					SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: true).SelectEndDirection();
					SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: false).SelectEndDirection();
				});
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
					if (CheckIsCanSelect(SpearBattle_Define.SkillType.Rock, _skillArray))
					{
						_skillArray[_selectNo] = ((!flag) ? SpearBattle_Define.SkillType.Rock : SpearBattle_Define.SkillType.Special_Rock);
						flag2 = true;
					}
					else
					{
						SingletonCustom<HidVibration>.Instance.SetCommonVibration(_playerNo);
					}
				}
				else if (SpearBattle_Controller.GetScissorsButtonDown(_playerNo))
				{
					if (CheckIsCanSelect(SpearBattle_Define.SkillType.Scissors, _skillArray))
					{
						_skillArray[_selectNo] = (flag ? SpearBattle_Define.SkillType.Special_Scissors : SpearBattle_Define.SkillType.Scissors);
						flag2 = true;
					}
					else
					{
						SingletonCustom<HidVibration>.Instance.SetCommonVibration(_playerNo);
					}
				}
				else if (SpearBattle_Controller.GetPaperButtonDown(_playerNo))
				{
					if (CheckIsCanSelect(SpearBattle_Define.SkillType.Paper, _skillArray))
					{
						_skillArray[_selectNo] = (flag ? SpearBattle_Define.SkillType.Special_Paper : SpearBattle_Define.SkillType.Paper);
						flag2 = true;
					}
					else
					{
						SingletonCustom<HidVibration>.Instance.SetCommonVibration(_playerNo);
					}
				}
				if (flag2)
				{
					SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
					SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetSkillActive(_isLeft, _selectNo, _skillArray[_selectNo]);
					SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetHideActive(_isLeft, _selectNo, _active: true);
				}
			}
			if (flag)
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration(_playerNo);
			}
			return;
		}
		bool flag3 = true;
		if (leftSkillArray[_selectNo] == SpearBattle_Define.SkillType.Empty)
		{
			flag3 = false;
		}
		bool flag4 = true;
		if (rightSkillArray[_selectNo] == SpearBattle_Define.SkillType.Empty)
		{
			flag4 = false;
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
		}
		if (_skillArray[_selectNo] == SpearBattle_Define.SkillType.Empty)
		{
			if (SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData().IsCpuBattle)
			{
				_skillArray[_selectNo] = (_isLeft ? leftCpuBattleSkillArray[_selectNo] : rightCpuBattleSkillArray[_selectNo]);
			}
			else
			{
				SpearBattle_Define.SkillType[] canSelectSkillArray = GetCanSelectSkillArray(_skillArray);
				_skillArray[_selectNo] = canSelectSkillArray[UnityEngine.Random.Range(0, canSelectSkillArray.Length)];
			}
			SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetSkillActive(_isLeft, _selectNo, _skillArray[_selectNo]);
			SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetHideActive(_isLeft, _selectNo, _active: true);
		}
	}
	private void CpuWeakCheck()
	{
		if (!isCpuWeakEnd && phaseNo <= 1 && (phaseNo != 0 || UnityEngine.Random.Range(0, 2) != 1))
		{
			isCpuWeakEnd = true;
			switch (SingletonCustom<SpearBattle_GameManager>.Instance.AiStrength)
			{
			case 2:
				break;
			case 0:
				CpuWeaken(phaseNo);
				break;
			case 1:
				CpuLittleWeaken(phaseNo);
				break;
			}
		}
	}
	private void CpuWeaken(int _phaseNo)
	{
		SpearBattle_GameManager.BattleData nowBattleData = SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData();
		if (nowBattleData.leftCharaData.isPlayer && !nowBattleData.rightCharaData.isPlayer)
		{
			SpearBattle_Define.PhaseResultType phaseResultType = SpearBattle_Define.CalcPhaseResultType(leftSkillArray[_phaseNo], rightSkillArray[_phaseNo]);
			if (phaseResultType == SpearBattle_Define.PhaseResultType.Draw || phaseResultType == SpearBattle_Define.PhaseResultType.Lose)
			{
				bool flag = rightSkillArray[_phaseNo] == SpearBattle_Define.SkillType.Special_Rock || rightSkillArray[_phaseNo] == SpearBattle_Define.SkillType.Special_Scissors || rightSkillArray[_phaseNo] == SpearBattle_Define.SkillType.Special_Paper;
				switch (leftSkillArray[_phaseNo])
				{
				case SpearBattle_Define.SkillType.Rock:
				case SpearBattle_Define.SkillType.Special_Rock:
					rightSkillArray[_phaseNo] = (flag ? SpearBattle_Define.SkillType.Special_Scissors : SpearBattle_Define.SkillType.Scissors);
					break;
				case SpearBattle_Define.SkillType.Scissors:
				case SpearBattle_Define.SkillType.Special_Scissors:
					rightSkillArray[_phaseNo] = (flag ? SpearBattle_Define.SkillType.Special_Paper : SpearBattle_Define.SkillType.Paper);
					break;
				case SpearBattle_Define.SkillType.Paper:
				case SpearBattle_Define.SkillType.Special_Paper:
					rightSkillArray[_phaseNo] = ((!flag) ? SpearBattle_Define.SkillType.Rock : SpearBattle_Define.SkillType.Special_Rock);
					break;
				}
				SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetSkillActive(_isLeft: false, _phaseNo, rightSkillArray[_phaseNo]);
			}
		}
		else
		{
			if (nowBattleData.leftCharaData.isPlayer || !nowBattleData.rightCharaData.isPlayer)
			{
				return;
			}
			SpearBattle_Define.PhaseResultType phaseResultType2 = SpearBattle_Define.CalcPhaseResultType(rightSkillArray[_phaseNo], leftSkillArray[_phaseNo]);
			if (phaseResultType2 == SpearBattle_Define.PhaseResultType.Draw || phaseResultType2 == SpearBattle_Define.PhaseResultType.Lose)
			{
				bool flag2 = leftSkillArray[_phaseNo] == SpearBattle_Define.SkillType.Special_Rock || leftSkillArray[_phaseNo] == SpearBattle_Define.SkillType.Special_Scissors || leftSkillArray[_phaseNo] == SpearBattle_Define.SkillType.Special_Paper;
				switch (rightSkillArray[_phaseNo])
				{
				case SpearBattle_Define.SkillType.Rock:
				case SpearBattle_Define.SkillType.Special_Rock:
					leftSkillArray[_phaseNo] = (flag2 ? SpearBattle_Define.SkillType.Special_Scissors : SpearBattle_Define.SkillType.Scissors);
					break;
				case SpearBattle_Define.SkillType.Scissors:
				case SpearBattle_Define.SkillType.Special_Scissors:
					leftSkillArray[_phaseNo] = (flag2 ? SpearBattle_Define.SkillType.Special_Paper : SpearBattle_Define.SkillType.Paper);
					break;
				case SpearBattle_Define.SkillType.Paper:
				case SpearBattle_Define.SkillType.Special_Paper:
					leftSkillArray[_phaseNo] = ((!flag2) ? SpearBattle_Define.SkillType.Rock : SpearBattle_Define.SkillType.Special_Rock);
					break;
				}
				SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetSkillActive(_isLeft: true, _phaseNo, leftSkillArray[_phaseNo]);
			}
		}
	}
	private void CpuLittleWeaken(int _phaseNo)
	{
		SpearBattle_GameManager.BattleData nowBattleData = SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData();
		if (nowBattleData.leftCharaData.isPlayer && !nowBattleData.rightCharaData.isPlayer)
		{
			bool flag = rightSkillArray[_phaseNo] == SpearBattle_Define.SkillType.Special_Rock || rightSkillArray[_phaseNo] == SpearBattle_Define.SkillType.Special_Scissors || rightSkillArray[_phaseNo] == SpearBattle_Define.SkillType.Special_Paper;
			switch (SpearBattle_Define.CalcPhaseResultNo(leftSkillArray[_phaseNo], rightSkillArray[_phaseNo]))
			{
			case 2:
				switch (leftSkillArray[_phaseNo])
				{
				case SpearBattle_Define.SkillType.Rock:
				case SpearBattle_Define.SkillType.Special_Rock:
					rightSkillArray[_phaseNo] = (flag ? SpearBattle_Define.SkillType.Special_Scissors : SpearBattle_Define.SkillType.Scissors);
					break;
				case SpearBattle_Define.SkillType.Scissors:
				case SpearBattle_Define.SkillType.Special_Scissors:
					rightSkillArray[_phaseNo] = (flag ? SpearBattle_Define.SkillType.Special_Paper : SpearBattle_Define.SkillType.Paper);
					break;
				case SpearBattle_Define.SkillType.Paper:
				case SpearBattle_Define.SkillType.Special_Paper:
					rightSkillArray[_phaseNo] = ((!flag) ? SpearBattle_Define.SkillType.Rock : SpearBattle_Define.SkillType.Special_Rock);
					break;
				}
				SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetSkillActive(_isLeft: false, _phaseNo, rightSkillArray[_phaseNo]);
				break;
			case 1:
				switch (leftSkillArray[_phaseNo])
				{
				case SpearBattle_Define.SkillType.Rock:
				case SpearBattle_Define.SkillType.Special_Rock:
					rightSkillArray[_phaseNo] = ((!flag) ? SpearBattle_Define.SkillType.Rock : SpearBattle_Define.SkillType.Special_Rock);
					break;
				case SpearBattle_Define.SkillType.Scissors:
				case SpearBattle_Define.SkillType.Special_Scissors:
					rightSkillArray[_phaseNo] = (flag ? SpearBattle_Define.SkillType.Special_Scissors : SpearBattle_Define.SkillType.Scissors);
					break;
				case SpearBattle_Define.SkillType.Paper:
				case SpearBattle_Define.SkillType.Special_Paper:
					rightSkillArray[_phaseNo] = (flag ? SpearBattle_Define.SkillType.Special_Paper : SpearBattle_Define.SkillType.Paper);
					break;
				}
				SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetSkillActive(_isLeft: false, _phaseNo, rightSkillArray[_phaseNo]);
				break;
			}
		}
		else
		{
			if (nowBattleData.leftCharaData.isPlayer || !nowBattleData.rightCharaData.isPlayer)
			{
				return;
			}
			bool flag2 = leftSkillArray[_phaseNo] == SpearBattle_Define.SkillType.Special_Rock || leftSkillArray[_phaseNo] == SpearBattle_Define.SkillType.Special_Scissors || leftSkillArray[_phaseNo] == SpearBattle_Define.SkillType.Special_Paper;
			switch (SpearBattle_Define.CalcPhaseResultNo(rightSkillArray[_phaseNo], leftSkillArray[_phaseNo]))
			{
			case 2:
				switch (rightSkillArray[_phaseNo])
				{
				case SpearBattle_Define.SkillType.Rock:
				case SpearBattle_Define.SkillType.Special_Rock:
					leftSkillArray[_phaseNo] = (flag2 ? SpearBattle_Define.SkillType.Special_Scissors : SpearBattle_Define.SkillType.Scissors);
					break;
				case SpearBattle_Define.SkillType.Scissors:
				case SpearBattle_Define.SkillType.Special_Scissors:
					leftSkillArray[_phaseNo] = (flag2 ? SpearBattle_Define.SkillType.Special_Paper : SpearBattle_Define.SkillType.Paper);
					break;
				case SpearBattle_Define.SkillType.Paper:
				case SpearBattle_Define.SkillType.Special_Paper:
					leftSkillArray[_phaseNo] = ((!flag2) ? SpearBattle_Define.SkillType.Rock : SpearBattle_Define.SkillType.Special_Rock);
					break;
				}
				SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetSkillActive(_isLeft: true, _phaseNo, leftSkillArray[_phaseNo]);
				break;
			case 1:
				switch (rightSkillArray[_phaseNo])
				{
				case SpearBattle_Define.SkillType.Rock:
				case SpearBattle_Define.SkillType.Special_Rock:
					leftSkillArray[_phaseNo] = ((!flag2) ? SpearBattle_Define.SkillType.Rock : SpearBattle_Define.SkillType.Special_Rock);
					break;
				case SpearBattle_Define.SkillType.Scissors:
				case SpearBattle_Define.SkillType.Special_Scissors:
					leftSkillArray[_phaseNo] = (flag2 ? SpearBattle_Define.SkillType.Special_Scissors : SpearBattle_Define.SkillType.Scissors);
					break;
				case SpearBattle_Define.SkillType.Paper:
				case SpearBattle_Define.SkillType.Special_Paper:
					leftSkillArray[_phaseNo] = (flag2 ? SpearBattle_Define.SkillType.Special_Paper : SpearBattle_Define.SkillType.Paper);
					break;
				}
				SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetSkillActive(_isLeft: true, _phaseNo, leftSkillArray[_phaseNo]);
				break;
			}
		}
	}
	private SpearBattle_Define.SkillType[] GetCanSelectSkillArray(SpearBattle_Define.SkillType[] _skillArray)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < _skillArray.Length; i++)
		{
			switch (_skillArray[i])
			{
			case SpearBattle_Define.SkillType.Rock:
				num++;
				break;
			case SpearBattle_Define.SkillType.Scissors:
				num2++;
				break;
			case SpearBattle_Define.SkillType.Paper:
				num3++;
				break;
			}
		}
		List<SpearBattle_Define.SkillType> list = new List<SpearBattle_Define.SkillType>();
		if (num < 2)
		{
			list.Add(SpearBattle_Define.SkillType.Rock);
		}
		if (num2 < 2)
		{
			list.Add(SpearBattle_Define.SkillType.Scissors);
		}
		if (num3 < 2)
		{
			list.Add(SpearBattle_Define.SkillType.Paper);
		}
		return list.ToArray();
	}
	private int GetSelectSkillCount(SpearBattle_Define.SkillType _targetSkill, SpearBattle_Define.SkillType[] _skillArray)
	{
		int num = 0;
		for (int i = 0; i < _skillArray.Length; i++)
		{
			if (_skillArray[i] == _targetSkill)
			{
				num++;
			}
		}
		return num;
	}
	private bool CheckIsCanSelect(SpearBattle_Define.SkillType _targetSkill, SpearBattle_Define.SkillType[] _skillArray)
	{
		return GetSelectSkillCount(_targetSkill, _skillArray) < 2;
	}
	private void CpuBattleSkillSetting()
	{
		List<SpearBattle_Define.SkillType> source = new List<SpearBattle_Define.SkillType>
		{
			SpearBattle_Define.SkillType.Rock,
			SpearBattle_Define.SkillType.Rock,
			SpearBattle_Define.SkillType.Scissors,
			SpearBattle_Define.SkillType.Scissors,
			SpearBattle_Define.SkillType.Paper,
			SpearBattle_Define.SkillType.Paper
		};
		source = (from a in source
			orderby Guid.NewGuid()
			select a).ToList();
		leftCpuBattleSkillArray = source.ToArray();
		source = (from a in source
			orderby Guid.NewGuid()
			select a).ToList();
		rightCpuBattleSkillArray = source.ToArray();
		int num = 0;
		SpearBattle_Define.PhaseResultType[] array = new SpearBattle_Define.PhaseResultType[5];
		for (int i = 0; i < 5; i++)
		{
			array[i] = SpearBattle_Define.CalcPhaseResultType(leftCpuBattleSkillArray[i], rightCpuBattleSkillArray[i]);
			if (array[i] == SpearBattle_Define.PhaseResultType.Win)
			{
				num++;
			}
			else if (array[i] == SpearBattle_Define.PhaseResultType.Lose)
			{
				num--;
			}
		}
		if (num != 0)
		{
			return;
		}
		bool flag = UnityEngine.Random.Range(0, 2) == 1;
		int num2 = 0;
		while (true)
		{
			if (num2 >= 5)
			{
				return;
			}
			if (flag)
			{
				if ((array[num2] == SpearBattle_Define.PhaseResultType.Lose || array[num2] == SpearBattle_Define.PhaseResultType.Draw) && SpearBattle_Define.CalcPhaseResultType(leftCpuBattleSkillArray[5], rightCpuBattleSkillArray[num2]) == SpearBattle_Define.PhaseResultType.Win)
				{
					leftCpuBattleSkillArray[num2] = leftCpuBattleSkillArray[5];
					return;
				}
			}
			else if ((array[num2] == SpearBattle_Define.PhaseResultType.Win || array[num2] == SpearBattle_Define.PhaseResultType.Draw) && SpearBattle_Define.CalcPhaseResultType(rightCpuBattleSkillArray[5], leftCpuBattleSkillArray[num2]) == SpearBattle_Define.PhaseResultType.Win)
			{
				break;
			}
			num2++;
		}
		rightCpuBattleSkillArray[num2] = rightCpuBattleSkillArray[5];
	}
	private IEnumerator _BattleDirection()
	{
		yield return new WaitForSeconds(2f);
		SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: true).PlayAnimation(_isLeft: true, _isSpeedStart: true);
		SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: false).PlayAnimation(_isLeft: false, _isSpeedStart: true);
	}
	public void CameraChange(bool _isDuel)
	{
		if (!GetIsScoreEnd())
		{
			duelCamera.gameObject.SetActive(_isDuel);
			leftCamera.gameObject.SetActive(!_isDuel);
			rightCamera.gameObject.SetActive(!_isDuel);
			SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetPartitionActive(!_isDuel);
			if (_isDuel)
			{
				DuelCameraWorkChange(phaseNo);
			}
		}
	}
	private void DuelCameraWorkChange(int _anchorNo)
	{
		if (!isBattleEnd)
		{
			duelCamera.transform.position = duelCameraAnchors[_anchorNo].position;
			duelCamera.transform.rotation = duelCameraAnchors[_anchorNo].rotation;
		}
	}
	public void CharaStayDirection()
	{
		SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: true).StayDirection();
		SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: false).StayDirection();
	}
	public void CharaAnimDirection()
	{
		if (!isBattleEnd)
		{
			switch (SpearBattle_Define.CalcPhaseResultNo(leftSkillArray[phaseNo], rightSkillArray[phaseNo]))
			{
			case 0:
				SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: true).AttackAnimDirection();
				SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: false).PoseAnimDirection();
				break;
			case 1:
				SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: true).PoseAnimDirection();
				SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: false).AttackAnimDirection();
				break;
			case 2:
				SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: true).AttackAnimDirection();
				SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: false).AttackAnimDirection();
				break;
			}
		}
	}
	public void PhaseBattle()
	{
		if (isBattleEnd)
		{
			return;
		}
		SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetHideActive(_isLeft: true, phaseNo, _active: false);
		SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetHideActive(_isLeft: false, phaseNo, _active: false);
		SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetResultActive(_isLeft: true, phaseNo, _active: true, SpearBattle_Define.CalcPhaseResultNo(leftSkillArray[phaseNo], rightSkillArray[phaseNo]));
		SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetResultActive(_isLeft: false, phaseNo, _active: true, SpearBattle_Define.CalcPhaseResultNo(rightSkillArray[phaseNo], leftSkillArray[phaseNo]));
		int num = SpearBattle_Define.CalcDamage(rightSkillArray[phaseNo], leftSkillArray[phaseNo]);
		int num2 = SpearBattle_Define.CalcDamage(leftSkillArray[phaseNo], rightSkillArray[phaseNo]);
		SpearBattle_Define.PhaseResultType phaseResultType = SpearBattle_Define.CalcPhaseResultType(leftSkillArray[phaseNo], rightSkillArray[phaseNo]);
		switch (phaseResultType)
		{
		case SpearBattle_Define.PhaseResultType.SpecialWin:
			leftScore += 2;
			break;
		case SpearBattle_Define.PhaseResultType.Win:
			leftScore++;
			break;
		}
		SpearBattle_Define.PhaseResultType phaseResultType2 = SpearBattle_Define.CalcPhaseResultType(rightSkillArray[phaseNo], leftSkillArray[phaseNo]);
		switch (phaseResultType2)
		{
		case SpearBattle_Define.PhaseResultType.SpecialWin:
			rightScore += 2;
			break;
		case SpearBattle_Define.PhaseResultType.Win:
			rightScore++;
			break;
		}
		SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetScore(_isLeft: true, leftScore);
		SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetScore(_isLeft: false, rightScore);
		phaseNo++;
		if (phaseResultType == SpearBattle_Define.PhaseResultType.SpecialWin || phaseResultType == SpearBattle_Define.PhaseResultType.Win)
		{
			if (SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData().IsCpuBattle || SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData().leftCharaData.isPlayer)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
			}
		}
		else if (phaseResultType2 == SpearBattle_Define.PhaseResultType.SpecialWin || phaseResultType2 == SpearBattle_Define.PhaseResultType.Win)
		{
			if (SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData().IsCpuBattle || SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData().rightCharaData.isPlayer)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
			}
		}
		else if (phaseResultType == SpearBattle_Define.PhaseResultType.Draw && GetIsScoreEnd() && (SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData().IsCpuBattle || (leftScore > rightScore && SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData().leftCharaData.isPlayer) || (leftScore < rightScore && SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData().rightCharaData.isPlayer)))
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
		}
		battleEffect.Play();
		if (num > 0 && num2 > 0)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_attack_hit_1");
			return;
		}
		if (num > 0)
		{
			SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: true).PlayBreakEffect();
		}
		if (num2 > 0)
		{
			SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: false).PlayBreakEffect();
		}
		SingletonCustom<AudioManager>.Instance.SePlay("se_sword_fight_hit");
	}
	public void SelectStart()
	{
		if (!isBattleEnd)
		{
			isCtrlWait = true;
			selectTimer = 5f;
			SingletonCustom<SpearBattle_UIManager>.Instance.SetTimeActive(_active: true);
			SingletonCustom<SpearBattle_UIManager>.Instance.SetTimeValue(selectTimer);
			leftAiWaitTimer = 0f;
			rightAiWaitTimer = 0f;
			SingletonCustom<SpearBattle_UIManager>.Instance.SetSelectUIActive(_active: true);
			SingletonCustom<SpearBattle_SelectUIManager>.Instance.SetRemainText(_isLeft: true, 2 - GetSelectSkillCount(SpearBattle_Define.SkillType.Rock, leftSkillArray), SpearBattle_Define.SkillType.Rock);
			SingletonCustom<SpearBattle_SelectUIManager>.Instance.SetRemainText(_isLeft: true, 2 - GetSelectSkillCount(SpearBattle_Define.SkillType.Scissors, leftSkillArray), SpearBattle_Define.SkillType.Scissors);
			SingletonCustom<SpearBattle_SelectUIManager>.Instance.SetRemainText(_isLeft: true, 2 - GetSelectSkillCount(SpearBattle_Define.SkillType.Paper, leftSkillArray), SpearBattle_Define.SkillType.Paper);
			SingletonCustom<SpearBattle_SelectUIManager>.Instance.SetRemainText(_isLeft: false, 2 - GetSelectSkillCount(SpearBattle_Define.SkillType.Rock, rightSkillArray), SpearBattle_Define.SkillType.Rock);
			SingletonCustom<SpearBattle_SelectUIManager>.Instance.SetRemainText(_isLeft: false, 2 - GetSelectSkillCount(SpearBattle_Define.SkillType.Scissors, rightSkillArray), SpearBattle_Define.SkillType.Scissors);
			SingletonCustom<SpearBattle_SelectUIManager>.Instance.SetRemainText(_isLeft: false, 2 - GetSelectSkillCount(SpearBattle_Define.SkillType.Paper, rightSkillArray), SpearBattle_Define.SkillType.Paper);
		}
	}
	public void HorseStay()
	{
		isHorseStay = true;
		SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: true).StopAnimation(_isHorseLerpStop: false);
		SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: false).StopAnimation(_isHorseLerpStop: false);
	}
	public void HpEndCheck()
	{
		if (GetIsHpEnd())
		{
			BattleEnd(_isSkip: false);
		}
	}
	public void ScoreEndCheck()
	{
		if (GetIsScoreEnd())
		{
			BattleEnd(_isSkip: false);
		}
	}
	public void BattleEnd(bool _isSkip)
	{
		if (isBattleEnd)
		{
			return;
		}
		if (leftScore < rightScore)
		{
			if (!_isSkip)
			{
				SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: true).FallHorse();
			}
			SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: true).SetCharaFace(StyleTextureManager.MainCharacterFaceType.SAD);
			LeanTween.delayedCall(base.gameObject, 0.5f, (Action)delegate
			{
				SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: true).StopAnimation(_isHorseLerpStop: false);
			});
		}
		else
		{
			SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: true).SetCharaFace(StyleTextureManager.MainCharacterFaceType.SMILE);
			LeanTween.delayedCall(base.gameObject, 0.5f, (Action)delegate
			{
				SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: true).StopAnimation(_isHorseLerpStop: false);
			});
		}
		if (leftScore > rightScore)
		{
			if (!_isSkip)
			{
				SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: false).FallHorse();
			}
			SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: false).SetCharaFace(StyleTextureManager.MainCharacterFaceType.SAD);
			LeanTween.delayedCall(base.gameObject, 0.5f, (Action)delegate
			{
				SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: false).StopAnimation(_isHorseLerpStop: false);
			});
		}
		else
		{
			SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: false).SetCharaFace(StyleTextureManager.MainCharacterFaceType.SMILE);
			LeanTween.delayedCall(base.gameObject, 0.5f, (Action)delegate
			{
				SingletonCustom<SpearBattle_GameManager>.Instance.GetChara(_isLeft: false).StopAnimation(_isHorseLerpStop: false);
			});
		}
		isBattleEnd = true;
		if (leftScore == rightScore)
		{
			SingletonCustom<SpearBattle_GameManager>.Instance.BattleDraw();
		}
		else
		{
			SingletonCustom<SpearBattle_GameManager>.Instance.BattleEnd(leftScore > rightScore);
		}
	}
	private void Skip()
	{
		LeanTween.cancel(base.gameObject);
		while (!GetIsScoreEnd() && phaseNo != 5)
		{
			if (!isLeftPlayer && leftSkillArray[phaseNo] == SpearBattle_Define.SkillType.Empty)
			{
				if (SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData().IsCpuBattle)
				{
					leftSkillArray[phaseNo] = leftCpuBattleSkillArray[phaseNo];
				}
				else
				{
					SpearBattle_Define.SkillType[] canSelectSkillArray = GetCanSelectSkillArray(leftSkillArray);
					leftSkillArray[phaseNo] = canSelectSkillArray[UnityEngine.Random.Range(0, canSelectSkillArray.Length)];
				}
				SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetSkillActive(_isLeft: true, phaseNo, leftSkillArray[phaseNo]);
			}
			if (!isRightPlayer && rightSkillArray[phaseNo] == SpearBattle_Define.SkillType.Empty)
			{
				if (SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData().IsCpuBattle)
				{
					rightSkillArray[phaseNo] = rightCpuBattleSkillArray[phaseNo];
				}
				else
				{
					SpearBattle_Define.SkillType[] canSelectSkillArray2 = GetCanSelectSkillArray(rightSkillArray);
					rightSkillArray[phaseNo] = canSelectSkillArray2[UnityEngine.Random.Range(0, canSelectSkillArray2.Length)];
				}
				SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetSkillActive(_isLeft: false, phaseNo, rightSkillArray[phaseNo]);
			}
			SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetHideActive(_isLeft: true, phaseNo, _active: false);
			SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetHideActive(_isLeft: false, phaseNo, _active: false);
			SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetResultActive(_isLeft: true, phaseNo, _active: true, SpearBattle_Define.CalcPhaseResultNo(leftSkillArray[phaseNo], rightSkillArray[phaseNo]));
			SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetResultActive(_isLeft: false, phaseNo, _active: true, SpearBattle_Define.CalcPhaseResultNo(rightSkillArray[phaseNo], leftSkillArray[phaseNo]));
			SpearBattle_Define.CalcDamage(rightSkillArray[phaseNo], leftSkillArray[phaseNo]);
			SpearBattle_Define.CalcDamage(leftSkillArray[phaseNo], rightSkillArray[phaseNo]);
			switch (SpearBattle_Define.CalcPhaseResultType(leftSkillArray[phaseNo], rightSkillArray[phaseNo]))
			{
			case SpearBattle_Define.PhaseResultType.SpecialWin:
				leftScore += 2;
				break;
			case SpearBattle_Define.PhaseResultType.Win:
				leftScore++;
				break;
			}
			switch (SpearBattle_Define.CalcPhaseResultType(rightSkillArray[phaseNo], leftSkillArray[phaseNo]))
			{
			case SpearBattle_Define.PhaseResultType.SpecialWin:
				rightScore += 2;
				break;
			case SpearBattle_Define.PhaseResultType.Win:
				rightScore++;
				break;
			}
			SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetScore(_isLeft: true, leftScore);
			SingletonCustom<SpearBattle_BattleUIManager>.Instance.SetScore(_isLeft: false, rightScore);
			phaseNo++;
		}
		BattleEnd(_isSkip: true);
	}
	public bool GetIsHpEnd()
	{
		return false;
	}
	public bool GetIsScoreEnd()
	{
		int num = 5 - phaseNo;
		if (leftScore <= rightScore + num)
		{
			return rightScore > leftScore + num;
		}
		return true;
	}
}
