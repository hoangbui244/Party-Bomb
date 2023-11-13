using UnityEngine;
public class MonsterKill_PlayerUI : MonoBehaviour
{
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン")]
	private SpriteRenderer playerIcon;
	[SerializeField]
	[Header("キャラクタ\u30fcアイコン")]
	private SpriteRenderer characterIcon;
	private readonly string[] CHARACTER_SPRITE_NAME = new string[8]
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
	[Header("ポイントアンカ\u30fc")]
	private GameObject pointAnchor;
	[SerializeField]
	[Header("ポイント")]
	private SpriteNumbers point;
	private readonly int POINT_FRONT_ORDER_BASE = 9;
	private readonly int POINT_TARGET_ORDER_BASE = 8;
	private readonly int POINT_BACK_ORDER_BASE = 7;
	[SerializeField]
	[Header("描画優先度を変更する対象")]
	private SpriteRenderer[] arraySortOrderPoint;
	[SerializeField]
	[Header("ポイントのマスク")]
	private SpriteMask sortOrderPointMask;
	public void Init(int _playerNo, int _userType)
	{
		SetPlayerIcon(_userType);
		SetCharacterIcon(_userType);
		point.Set(0);
		if (sortOrderPointMask != null)
		{
			sortOrderPointMask.frontSortingOrder = POINT_FRONT_ORDER_BASE + _playerNo * 2;
			sortOrderPointMask.backSortingOrder = POINT_BACK_ORDER_BASE + _playerNo * 2;
			for (int i = 0; i < arraySortOrderPoint.Length; i++)
			{
				arraySortOrderPoint[i].sortingOrder = POINT_TARGET_ORDER_BASE + _playerNo * 2;
			}
		}
		else
		{
			for (int j = 0; j < arraySortOrderPoint.Length; j++)
			{
				arraySortOrderPoint[j].sortingOrder = POINT_TARGET_ORDER_BASE;
			}
		}
	}
	private void SetPlayerIcon(int _userType)
	{
		if (_userType < 4)
		{
			if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
			{
				playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_you");
			}
			else
			{
				playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_" + (_userType + 1).ToString() + "p");
			}
		}
		else
		{
			playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp" + (_userType - 4 + 1).ToString());
		}
	}
	private void SetCharacterIcon(int _userType)
	{
		characterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, CHARACTER_SPRITE_NAME[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[_userType]]);
	}
	public void SetPoint(int _point, int _addPoint)
	{
		LeanTween.cancel(point.gameObject);
		LeanTween.value(point.gameObject, _point, _addPoint, 0.5f).setOnUpdate(delegate(float _value)
		{
			point.Set((int)_value);
		});
	}
	public void Hide(float _delay, Vector3 _hidePos)
	{
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			LeanTween.moveLocal(base.gameObject, base.transform.localPosition + _hidePos, 1.25f).setEaseInQuint().setDelay(_delay);
		}
		else
		{
			LeanTween.moveLocal(pointAnchor, pointAnchor.transform.localPosition + _hidePos, 1.25f).setEaseInQuint().setDelay(_delay);
		}
	}
}
