using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RoadRaceCharacterManager : SingletonCustom<RoadRaceCharacterManager>
{
	public struct CharaData
	{
		public int uniformNo;
		public int byicycleNo;
	}
	private const float MULTI_LAST_GOAL_WAIT_TIME = 10f;
	private int memberNum = 4;
	private int playerNum = 1;
	[SerializeField]
	[Header("キャラプレハブ")]
	private GameObject charaPrefab;
	private List<KeyValuePair<int, float>>[] checkCharaData;
	private RoadRaceCharacterScript[] charas;
	[SerializeField]
	[Header("自転車")]
	private RoadRaceBicycleScript bicycleBasePrefs;
	[SerializeField]
	[Header("自転車マテリアル")]
	private Material[] bicycleMats;
	private bool isStarted;
	private AudioSource raceAudioSource;
	private bool isRunSePlay;
	private int runSeNum;
	private int checkPointTotalNum;
	private CharaData[] charaDataList;
	private float multiLastGoalWaitTime;
	private int pitchUpFlag = 1;
	private bool playergoal;
	private float seTime;
	public int MemberNum => memberNum;
	public int PlayerNum
	{
		get
		{
			return playerNum;
		}
		set
		{
			playerNum = value;
		}
	}
	public RoadRaceBicycleScript BicycleBasePrefs => bicycleBasePrefs;
	public Material[] BicycleMats => bicycleMats;
	public bool IsStarted => isStarted;
	public int CheckPointTotalNum => checkPointTotalNum;
	public void Init()
	{
		PlayerNum = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
		checkPointTotalNum = Scene_RoadRace.FM.RaceCheckPoint.Length * Scene_RoadRace.GM.LapNum;
		isStarted = false;
		checkCharaData = new List<KeyValuePair<int, float>>[CheckPointTotalNum + 1];
		for (int i = 0; i < checkCharaData.Length; i++)
		{
			checkCharaData[i] = new List<KeyValuePair<int, float>>();
		}
		CreateCharaDataList(MemberNum);
		charas = new RoadRaceCharacterScript[MemberNum];
		int num = 0;
		int num2 = 0;
		if (Scene_RoadRace.GM.IsOneOnOne)
		{
			num = 1;
		}
		for (int j = 0; j < charas.Length; j++)
		{
			num2 = (j + num) % Scene_RoadRace.FM.StageData.CreateAnchor.Length;
			charas[j] = Object.Instantiate(charaPrefab, Scene_RoadRace.FM.StageData.CreateAnchor[num2].position, Quaternion.identity, Scene_RoadRace.FM.CharacterAnchor).GetComponent<RoadRaceCharacterScript>();
			charas[j].gameObject.SetActive(value: true);
			charas[j].Init(j, Scene_RoadRace.FM.RaceCheckPoint, Scene_RoadRace.FM.StageData.CreateAnchor[num2], (int)Scene_RoadRace.GM.SpeedClass);
			if (charas[j].IsCpu)
			{
				charas[j].SetAIPosition(Scene_RoadRace.FM.AiPoints[0].GetPointPosition());
				charas[j].SetAiPointNo(0);
			}
		}
		for (int k = 0; k < charas.Length; k++)
		{
			charas[k].isDontMove = true;
		}
		SetCharaMoveActive(_active: false);
	}
	private void CreateCharaDataList(int _charaNum)
	{
		charaDataList = new CharaData[_charaNum];
		for (int i = 0; i < charaDataList.Length; i++)
		{
		}
	}
	public void RaceStart()
	{
		isStarted = true;
		for (int i = 0; i < charas.Length; i++)
		{
			charas[i].isDontMove = false;
		}
		SetCharaMoveActive(_active: true);
	}
	public void UpdateMethod()
	{
		if (!Scene_RoadRace.GM.IsGameEnd && isStarted)
		{
			seTime -= Time.deltaTime;
			if (seTime < 0f)
			{
				seTime = 0f;
			}
		}
		for (int i = 0; i < charas.Length; i++)
		{
			if (isStarted)
			{
				bool flag = true;
				if (charas[i].IsGoal)
				{
					if (Scene_RoadRace.GM.IsLapRace)
					{
						charas[i].AiMove();
					}
					else
					{
						charas[i].Move(CalcManager.mVector3Zero, _isAccel: false);
					}
				}
				else
				{
					if (!charas[i].IsCpu)
					{
						PlayerOperation(i);
						flag = false;
					}
					if (flag)
					{
						charas[i].AiMove();
					}
				}
			}
			charas[i].UpdateMethod();
		}
		UpdateRank();
	}
	public void UpdateRank()
	{
		for (int i = 0; i < checkCharaData.Length; i++)
		{
			checkCharaData[i].Clear();
		}
		for (int j = 0; j < charas.Length; j++)
		{
			if (!charas[j].IsGoal)
			{
				if (charas[j].CheckPointNext < 0)
				{
					checkCharaData[0].Add(new KeyValuePair<int, float>(j, charas[j].GetDisNextCheck()));
				}
				else
				{
					checkCharaData[charas[j].CheckPointNext].Add(new KeyValuePair<int, float>(j, charas[j].GetDisNextCheck()));
				}
			}
		}
		for (int num = checkCharaData.Length - 1; num >= 0; num--)
		{
			checkCharaData[num].Sort((KeyValuePair<int, float> a, KeyValuePair<int, float> b) => (int)(a.Value * 100f) - (int)(b.Value * 100f));
		}
		int num2 = Scene_RoadRace.GM.GoalCnt;
		for (int num3 = checkCharaData.Length - 1; num3 >= 0; num3--)
		{
			for (int k = 0; k < checkCharaData[num3].Count; k++)
			{
				int key = checkCharaData[num3][k].Key;
				Scene_RoadRace.UM.ChangeRankNum(key, num2);
				num2++;
			}
		}
	}
	public void UpdateLap(int _charaNo)
	{
		Scene_RoadRace.UM.SetLap(_charaNo, charas[_charaNo].LapNum);
		int lapNum = charas[_charaNo].LapNum;
		if (lapNum == Scene_RoadRace.GM.LapNum - 1)
		{
			Scene_RoadRace.UM.PlayLastOneEffect(_charaNo);
		}
		else
		{
			Scene_RoadRace.UM.PlayRapEffect(_charaNo, lapNum);
		}
		pitchUpFlag = lapNum;
	}
	public int GetRapCount()
	{
		return pitchUpFlag;
	}
	public void Goal(int _charaNo)
	{
		UnityEngine.Debug.Log("ゴ\u30fcル : No." + _charaNo.ToString() + " : 順位 = " + charas[_charaNo].GetNowRank().ToString() + " : ゴ\u30fcル人数 = " + Scene_RoadRace.GM.GoalCnt.ToString());
		if (!charas[_charaNo].IsGoal)
		{
			charas[_charaNo].SetIsGoal(_isGoal: true);
			charas[_charaNo].IsPlaySe = false;
			if (!Scene_RoadRace.GM.IsLapRace)
			{
				charas[_charaNo].IsInertiaAfterGoal = true;
			}
			charas[_charaNo].SetGoalTime(CalcManager.ConvertDecimalSecond(Scene_RoadRace.GM.GetGameTime()));
			Scene_RoadRace.GM.SetGoal(charas[_charaNo].CharaNo, (int)charas[_charaNo].UserType);
		}
	}
	private IEnumerator _LastOneBGM()
	{
		yield return new WaitForSeconds(0.1f);
	}
	private void PlayerOperation(int _playerNo)
	{
		CharaMove(_playerNo);
		if (SingletonCustom<RoadRaceControllerManager>.Instance.IsActionBtnDown(_playerNo))
		{
			CharaAction(_playerNo);
		}
		charas[_playerNo].IsRearLook = SingletonCustom<RoadRaceControllerManager>.Instance.IsLookRearBtn(_playerNo);
		if (SingletonCustom<RoadRaceControllerManager>.Instance.IsCameraAngleLeftBtnDown(_playerNo))
		{
			UnityEngine.Debug.Log("No." + _playerNo.ToString() + " : Lボタン");
			SingletonCustom<RoadRaceCameraManager>.Instance.CngCameraPosType(_playerNo);
		}
		if (SingletonCustom<RoadRaceControllerManager>.Instance.IsCameraAngleRightBtnDown(_playerNo))
		{
			UnityEngine.Debug.Log("No." + _playerNo.ToString() + " : Rボタン");
			SingletonCustom<RoadRaceCameraManager>.Instance.CngCameraPosType(_playerNo);
		}
	}
	private void CharaMove(int _no)
	{
		Vector3 vector = SingletonCustom<RoadRaceControllerManager>.Instance.GetStickDir(_no);
		vector.z = vector.y;
		vector.y = 0f;
		charas[_no].Move(vector, SingletonCustom<RoadRaceControllerManager>.Instance.IsAccelBtn(_no));
		charas[_no].UnicycleRotZ();
		charas[_no].UnicycleRotX();
	}
	private void CharaAction(int _no)
	{
		charas[_no].Action();
	}
	public bool CheckControlChara(RoadRaceCharacterScript _chara)
	{
		if (_chara.PlayerNo == -1)
		{
			return false;
		}
		return charas[_chara.CharaNo] == _chara;
	}
	public Transform[] GetCarTranses()
	{
		Transform[] array = new Transform[charas.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = charas[i].transform;
		}
		return array;
	}
	public Vector3 GetPlayerOnePosition()
	{
		return charas[0].GetPos();
	}
	public RoadRaceCharacterScript[] GetCharas()
	{
		return charas;
	}
	public RoadRaceCharacterScript GetChara(int _charaNo)
	{
		if (_charaNo >= charas.Length)
		{
			return null;
		}
		return charas[_charaNo];
	}
	public RoadRaceCharacterScript GetChara(GameObject _obj)
	{
		for (int i = 0; i < charas.Length; i++)
		{
			if (charas[i].gameObject == _obj)
			{
				return charas[i];
			}
		}
		return null;
	}
	public RoadRaceCharacterScript GetControlChara(int _playerNo)
	{
		return charas[_playerNo];
	}
	public void SetCharaMoveActive(bool _active)
	{
		for (int i = 0; i < charas.Length; i++)
		{
			charas[i].Rigid.isKinematic = !_active;
		}
	}
	public void SetAiPoint(int _id, int _no)
	{
		int num = _no + 1;
		if (num >= Scene_RoadRace.FM.AiPoints.Length)
		{
			num = 0;
		}
		if (Scene_RoadRace.FM.AiPoints[num].isPrevFixPoint)
		{
			charas[_id].SetAIPosition(Scene_RoadRace.FM.AiPoints[num].GetPointPosition(charas[_id].CheckPointIdx));
		}
		else
		{
			int randomPointIdx = Scene_RoadRace.FM.AiPoints[num].GetRandomPointIdx();
			charas[_id].SetAIPosition(Scene_RoadRace.FM.AiPoints[num].GetPointPosition(randomPointIdx));
		}
		charas[_id].SetAiPointNo(num);
	}
	private void OnDrawGizmos()
	{
	}
	public void AllSePlayCheck(string _seName, float _volum = 1f)
	{
		if (seTime <= 0f && !Scene_RoadRace.GM.IsGameEnd)
		{
			SingletonCustom<AudioManager>.Instance.SePlay(_seName, _loop: false, 0f, _volum);
			seTime = 0.07f;
		}
	}
}
