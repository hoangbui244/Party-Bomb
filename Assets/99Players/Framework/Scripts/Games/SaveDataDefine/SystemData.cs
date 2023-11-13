using System;
using UnityEngine;
namespace SaveDataDefine
{
	[Serializable]
	public class SystemData
	{
		public enum AiStrength
		{
			WEAK,
			NORAML,
			STRONG
		}
		public int volumeBgm;
		public int volumeSe;
		public int volumeVoice;
		public int aiStrength;
		public bool isVibration;
		public bool isStartHelp;
		public bool isCrown;
		public int map;
		public int characterNumSetting;
		public int resultCharacterNo;
		public int style;
		public int gameSelectNumRed;
		public int gameSelectNumBlue = 3;
		public int gameSelectNumYellow = 4;
		public SystemData()
		{
			volumeBgm = 5;
			volumeSe = 5;
			volumeVoice = 5;
			aiStrength = 1;
			isVibration = true;
			map = 0;
			isStartHelp = false;
			isCrown = true;
			resultCharacterNo = 1;
			style = 1;
			gameSelectNumRed = 0;
			gameSelectNumBlue = 3;
			gameSelectNumYellow = 4;
		}
		public void SetDefaultData()
		{
		}
		public void InitSetting(bool _isMainCall, bool _isPartySelect)
		{
			volumeBgm = 5;
			volumeSe = 5;
			volumeVoice = 5;
			isVibration = true;
			map = 0;
			isStartHelp = false;
			resultCharacterNo = 1;
			isCrown = true;
			if (_isMainCall)
			{
				aiStrength = 1;
				style = 0;
				if (!_isPartySelect)
				{
					gameSelectNumRed = 0;
					gameSelectNumBlue = 3;
					gameSelectNumYellow = 4;
				}
			}
		}
		public AiStrength GetAiStrengthSetting()
		{
			return (AiStrength)aiStrength;
		}
		public StyleTextureManager.TexType GetStyleSetting()
		{
			return (StyleTextureManager.TexType)style;
		}
		public void ResetGameSelectNum(int _maxGameCnt)
		{
			int num = 0;
			for (int i = 0; i < GS_Setting.GameSelectNum.Length; i++)
			{
				if (GS_Setting.GameSelectNum[i] < _maxGameCnt)
				{
					num = i;
				}
			}
			gameSelectNumRed = num - 2;
			gameSelectNumBlue = num - 1;
			gameSelectNumYellow = num;
		}
		public void CheckDLCGameSelectNum()
		{
			SystemData systemData = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData;
			int num = 19;
			if (SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_1_IDX))
			{
				num += 10;
			}
			if (SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_2_IDX))
			{
				num += 6;
			}
			if (SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_3_IDX))
			{
				num += 6;
			}
			UnityEngine.Debug.Log("CheckDLCGameSelectNum:" + num.ToString());
			if (GS_Setting.GameSelectNum[systemData.gameSelectNumRed] > num || GS_Setting.GameSelectNum[systemData.gameSelectNumBlue] > num || GS_Setting.GameSelectNum[systemData.gameSelectNumYellow] > num)
			{
				SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.ResetGameSelectNum(num);
			}
		}
	}
}
