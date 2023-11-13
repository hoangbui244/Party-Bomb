using UnityEngine;
public class GS_SettingGauge : GS_SettingRow
{
	[SerializeField]
	[Header("項目タイプ")]
	private GS_Setting.SettingType settingType;
	[SerializeField]
	[Header("ゲ\u30fcジ画像")]
	private SpriteRenderer[] arrayGauge;
	[SerializeField]
	[Header("段階カラ\u30fc")]
	private Color[] arrayColor;
	public const int DEF_VALUE = 5;
	private const int VALUE_MIN = 0;
	private const int VALUE_MAX = 10;
	private int value = 5;
	public void Start()
	{
		switch (settingType)
		{
		case GS_Setting.SettingType.BGM:
			value = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeBgm;
			break;
		case GS_Setting.SettingType.SE:
			value = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeSe;
			break;
		case GS_Setting.SettingType.VOICE:
			value = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeVoice;
			break;
		}
		UpdateGauge();
	}
	public override void InputRight()
	{
		if (value >= 10)
		{
			return;
		}
		value++;
		switch (settingType)
		{
		case GS_Setting.SettingType.BGM:
			SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeBgm = value;
			SingletonCustom<AudioManager>.Instance.SetBgmVolume();
			if (!SingletonCustom<AudioManager>.Instance.IsBgmFlg())
			{
				SingletonCustom<AudioManager>.Instance.SetBgmFlg(_flg: true);
				UnityEngine.Debug.Log("LAST:" + SingletonCustom<AudioManager>.Instance.LastBgmIndex);
				UnityEngine.Debug.Log("check:" + (!SingletonCustom<AudioManager>.Instance.IsBgmPlaying(SingletonCustom<AudioManager>.Instance.LastBgmIndex)).ToString());
				if (!SingletonCustom<AudioManager>.Instance.IsBgmPlaying(SingletonCustom<AudioManager>.Instance.LastBgmIndex))
				{
					SingletonCustom<AudioManager>.Instance.BgmStop();
					SingletonCustom<AudioManager>.Instance.BgmPlayPitch(SingletonCustom<AudioManager>.Instance.LastBgmIndex, _loop: true);
				}
			}
			break;
		case GS_Setting.SettingType.SE:
			SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeSe = value;
			SingletonCustom<AudioManager>.Instance.SetSeVolume();
			break;
		case GS_Setting.SettingType.VOICE:
			SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeVoice = value;
			SingletonCustom<AudioManager>.Instance.SetVoiceVolume();
			if (!SingletonCustom<AudioManager>.Instance.IsVoiceFlg())
			{
				SingletonCustom<AudioManager>.Instance.SetVoiceFlg(_flg: true);
			}
			break;
		}
		UpdateGauge();
	}
	public override void InputLeft()
	{
		if (value <= 0)
		{
			return;
		}
		value--;
		switch (settingType)
		{
		case GS_Setting.SettingType.BGM:
			SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeBgm = value;
			SingletonCustom<AudioManager>.Instance.SetBgmVolume();
			if (value <= 0)
			{
				SingletonCustom<AudioManager>.Instance.SetBgmFlg(_flg: false);
			}
			break;
		case GS_Setting.SettingType.SE:
			SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeSe = value;
			SingletonCustom<AudioManager>.Instance.SetSeVolume();
			break;
		case GS_Setting.SettingType.VOICE:
			SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeVoice = value;
			SingletonCustom<AudioManager>.Instance.SetVoiceVolume();
			if (value <= 0)
			{
				SingletonCustom<AudioManager>.Instance.SetVoiceFlg(_flg: false);
			}
			break;
		}
		UpdateGauge();
	}
	private void UpdateGauge()
	{
		for (int i = 0; i < arrayGauge.Length; i++)
		{
			arrayGauge[i].color = ((i < value) ? arrayColor[i] : new Color(0.337f, 0.337f, 0.337f));
		}
	}
	public override void Reset()
	{
		base.Reset();
		switch (settingType)
		{
		case GS_Setting.SettingType.BGM:
			value = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeBgm;
			SingletonCustom<AudioManager>.Instance.SetBgmVolume();
			if (!SingletonCustom<AudioManager>.Instance.IsBgmFlg())
			{
				SingletonCustom<AudioManager>.Instance.SetBgmFlg(_flg: true);
				if (!SingletonCustom<AudioManager>.Instance.IsBgmPlaying(SingletonCustom<AudioManager>.Instance.LastBgmIndex))
				{
					SingletonCustom<AudioManager>.Instance.BgmStop();
					SingletonCustom<AudioManager>.Instance.BgmPlay(SingletonCustom<AudioManager>.Instance.LastBgmIndex, _loop: true);
				}
			}
			break;
		case GS_Setting.SettingType.SE:
			value = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeSe;
			SingletonCustom<AudioManager>.Instance.SetSeVolume();
			break;
		case GS_Setting.SettingType.VOICE:
			value = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeVoice;
			SingletonCustom<AudioManager>.Instance.SetVoiceVolume();
			if (!SingletonCustom<AudioManager>.Instance.IsVoiceFlg())
			{
				SingletonCustom<AudioManager>.Instance.SetVoiceFlg(_flg: true);
			}
			break;
		}
		UpdateGauge();
	}
}
