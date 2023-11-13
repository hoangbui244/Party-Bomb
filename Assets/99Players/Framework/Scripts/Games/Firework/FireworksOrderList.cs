using UnityEngine;
public class FireworksOrderList : MonoBehaviour
{
	[SerializeField]
	[Header("プレイヤ\u30fc番号")]
	private SpriteRenderer spPlayerNo;
	private FireworksBall.ItemType[] arrayColorList = new FireworksBall.ItemType[3];
	private int colorCnt;
	public FireworksBall.ItemType[] ArrayColorList => arrayColorList;
	public FireworksPlayer Player
	{
		get;
		set;
	}
	public void Init(FireworksPlayer _player)
	{
		UpdateColor();
		Player = _player;
		SetPlayerIcon();
	}
	public bool CheckLift(FireworksBall.ItemType[] _haveList, FireworksBall.ItemType _slatBoxColor)
	{
		int num = 0;
		for (int i = 0; i < arrayColorList.Length; i++)
		{
			if (arrayColorList[i] == _slatBoxColor)
			{
				num++;
			}
		}
		if (num <= 0)
		{
			return false;
		}
		int num2 = 0;
		for (int j = 0; j < _haveList.Length; j++)
		{
			if (_haveList[j] == _slatBoxColor)
			{
				num2++;
			}
		}
		if (num2 < num)
		{
			return true;
		}
		return false;
	}
	private void SetPlayerIcon()
	{
		switch (Player.UserType)
		{
		case FireworksDefine.UserType.PLAYER_1:
			spPlayerNo.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, SingletonCustom<GameSettingManager>.Instance.IsSinglePlay ? "_screen_you" : "_screen_1p");
			break;
		case FireworksDefine.UserType.PLAYER_2:
			spPlayerNo.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_2p");
			break;
		case FireworksDefine.UserType.PLAYER_3:
			spPlayerNo.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_3p");
			break;
		case FireworksDefine.UserType.PLAYER_4:
			spPlayerNo.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_4p");
			break;
		case FireworksDefine.UserType.CPU_1:
			spPlayerNo.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp1");
			break;
		case FireworksDefine.UserType.CPU_2:
			spPlayerNo.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp2");
			break;
		case FireworksDefine.UserType.CPU_3:
			spPlayerNo.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp3");
			break;
		case FireworksDefine.UserType.CPU_4:
			spPlayerNo.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp4");
			break;
		case FireworksDefine.UserType.CPU_5:
			spPlayerNo.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp5");
			break;
		}
		spPlayerNo.SetAlpha(1f);
		LeanTween.value(1f, 0f, 0.25f).setOnUpdate(delegate(float _value)
		{
			spPlayerNo.SetAlpha(_value);
		}).setDelay(4f);
	}
	public void Set(FireworksBall.ItemType _colorType)
	{
		UpdateColor();
	}
	public bool CheckLift(FireworksBall.ItemType _slatBoxColor)
	{
		for (int i = 0; i < arrayColorList.Length; i++)
		{
			if (arrayColorList[i] == _slatBoxColor)
			{
				return true;
			}
		}
		return false;
	}
	private void UpdateColor()
	{
	}
	private void OnDestroy()
	{
		LeanTween.cancel(spPlayerNo.gameObject);
	}
}
