using UnityEngine;
public class GS_SettingToggle : GS_SettingRow
{
	[SerializeField]
	[Header("項目タイプ")]
	private GS_Setting.SettingType settingType;
	[SerializeField]
	[Header("On有効時オブジェクト")]
	private GameObject objOnEnable;
	[SerializeField]
	[Header("Off有効時オブジェクト")]
	private GameObject objOffEnable;
	private const int DEF_VALUE = 1;
	private int value = 1;
	public void Start()
	{
		switch (settingType)
		{
		case GS_Setting.SettingType.Vibration:
			value = (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.isVibration ? 1 : 0);
			break;
		case GS_Setting.SettingType.Crown:
			value = (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.isCrown ? 1 : 0);
			break;
		case GS_Setting.SettingType.StartHelp:
			value = (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.isStartHelp ? 1 : 0);
			break;
		}
		UpdateToggle();
	}
	public override void InputRight()
	{
		value = 0;
		switch (settingType)
		{
		case GS_Setting.SettingType.Vibration:
			SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.isVibration = false;
			SingletonCustom<HidVibration>.Instance.IsEnable = false;
			break;
		case GS_Setting.SettingType.Crown:
			SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.isCrown = false;
			break;
		case GS_Setting.SettingType.StartHelp:
			SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.isStartHelp = false;
			break;
		}
		UpdateToggle();
	}
	public override void InputLeft()
	{
		value = 1;
		switch (settingType)
		{
		case GS_Setting.SettingType.Vibration:
			SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.isVibration = true;
			SingletonCustom<HidVibration>.Instance.IsEnable = true;
			break;
		case GS_Setting.SettingType.Crown:
			SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.isCrown = true;
			break;
		case GS_Setting.SettingType.StartHelp:
			SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.isStartHelp = true;
			break;
		}
		UpdateToggle();
	}
	private void UpdateToggle()
	{
		objOnEnable.SetActive(value == 1);
		objOffEnable.SetActive(value == 0);
	}
	public override void Reset()
	{
		base.Reset();
		switch (settingType)
		{
		case GS_Setting.SettingType.Vibration:
			SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.isVibration = true;
			SingletonCustom<HidVibration>.Instance.IsEnable = true;
			value = 1;
			break;
		case GS_Setting.SettingType.Crown:
			SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.isCrown = true;
			value = 1;
			break;
		case GS_Setting.SettingType.StartHelp:
			SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.isStartHelp = false;
			value = 0;
			break;
		}
		UpdateToggle();
	}
}
