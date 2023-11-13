using System;
using UnityEngine;
using UnityEngine.Serialization;
public class Fishing_Characters : SingletonCustom<Fishing_Characters>
{
	[Serializable]
	public struct UserData
	{
		public Fishing_Character character;
		[FormerlySerializedAs("userType")]
		public FishingDefinition.User user;
		public int fishingCount;
		public int nowPoint;
		public bool isPlayer;
	}
	[FormerlySerializedAs("characters_Group1")]
	[SerializeField]
	[Header("キャラ")]
	private Fishing_Character[] characters;
	[SerializeField]
	[Header("教師キャラ")]
	private CharacterStyle characterTeacher;
	private UserData[] userData;
	private bool isGroup1Playing = true;
	public void Init()
	{
		InitUserData();
	}
	private void InitUserData()
	{
		userData = new UserData[4];
		for (int i = 0; i < userData.Length; i++)
		{
			userData[i].character = characters[i];
			if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3 && i == userData.Length - 1)
			{
				userData[i].user = FishingDefinition.User.Cpu1;
			}
			else
			{
				userData[i].user = (FishingDefinition.User)SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[i][0];
			}
			userData[i].fishingCount = 0;
			userData[i].nowPoint = 0;
			userData[i].isPlayer = (userData[i].user <= FishingDefinition.User.Player4);
			userData[i].character.Init(userData[i].isPlayer, userData[i].user, i);
			SingletonCustom<Fishing_GameUI>.Instance.SetUserUIData(i, userData[i].user);
		}
		FishingDefinition.CUIM.InitActiveCharacterFollowUI();
		characterTeacher.SetTeacherStyle(CharacterStyle.TeacherType.HOUJYOU);
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < userData.Length; i++)
		{
			userData[i].character.UpdateMethod();
		}
	}
	public void FixedUpdateMethod()
	{
		for (int i = 0; i < userData.Length; i++)
		{
			userData[i].character.FixedUpdateMethod();
		}
	}
	private void SettingMainCharaStyle()
	{
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			if (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.resultCharacterNo != 0)
			{
				int num = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.resultCharacterNo;
				if (num == 0)
				{
					num = UnityEngine.Random.Range(0, 4);
				}
				userData[0].character.SetMainCharaStyle((FishingDefinition.User)num);
			}
		}
		else
		{
			for (int i = 0; i < userData.Length; i++)
			{
				userData[i].character.SetMainCharaStyle(userData[i].user);
			}
		}
	}
	public UserData GetUserData(int _dataNo)
	{
		return userData[_dataNo];
	}
	public int GetUserDataLength()
	{
		return userData.Length;
	}
	public int[] GetUserRecordArray()
	{
		int[] array = new int[userData.Length];
		for (int i = 0; i < userData.Length; i++)
		{
			array[i] = userData[i].nowPoint;
		}
		return array;
	}
	public int[] GetUserNoArray()
	{
		int[] array = new int[userData.Length];
		for (int i = 0; i < userData.Length; i++)
		{
			array[i] = (int)userData[i].user;
		}
		return array;
	}
	public bool CheckNowGroup1Playing()
	{
		return isGroup1Playing;
	}
	public bool CheckAllCharacterWait()
	{
		for (int i = 0; i < userData.Length; i++)
		{
			if (userData[i].character.GetAnimationType() != 0)
			{
				return false;
			}
		}
		return true;
	}
	public void SetFishResult(int userNo, FishingDefinition.FishType fishSpecies, FishingDefinition.FishSizeType fishSize)
	{
		userData[userNo].fishingCount++;
		userData[userNo].nowPoint += FishingDefinition.FDM.GetFishPoint(fishSpecies);
		FishingDefinition.GUIM.SetFishCount(userNo, userData[userNo].fishingCount);
		FishingDefinition.GUIM.SetPoint(userNo, userData[userNo].nowPoint);
		switch (fishSize)
		{
		case FishingDefinition.FishSizeType.Garbage:
			break;
		case FishingDefinition.FishSizeType.Small:
			if (UnityEngine.Random.Range(0, 2) != 0)
			{
				if (UnityEngine.Random.Range(0, 2) == 0)
				{
					SingletonCustom<AudioManager>.Instance.VoicePlay("voice_insect_catching_out_0", _loop: false, 0f, 0.8f);
				}
				else
				{
					SingletonCustom<AudioManager>.Instance.VoicePlay("voice_fishing_get");
				}
			}
			break;
		case FishingDefinition.FishSizeType.Medium:
			if (UnityEngine.Random.Range(0, 2) != 0)
			{
				if (UnityEngine.Random.Range(0, 2) == 0)
				{
					SingletonCustom<AudioManager>.Instance.VoicePlay("voice_insect_catching_out_0", _loop: false, 0f, 0.8f);
				}
				else
				{
					SingletonCustom<AudioManager>.Instance.VoicePlay("voice_fishing_get");
				}
			}
			break;
		case FishingDefinition.FishSizeType.Large:
			if (UnityEngine.Random.Range(0, 2) != 0)
			{
				if (UnityEngine.Random.Range(0, 2) == 0)
				{
					SingletonCustom<AudioManager>.Instance.VoicePlay("voice_insect_catching_out_0", _loop: false, 0f, 0.8f);
				}
				else
				{
					SingletonCustom<AudioManager>.Instance.VoicePlay("voice_fishing_get");
				}
			}
			break;
		}
	}
	public void SetDebugRecord()
	{
		for (int i = 0; i < userData.Length; i++)
		{
			userData[i].nowPoint = UnityEngine.Random.Range(500, 1000);
		}
	}
}
