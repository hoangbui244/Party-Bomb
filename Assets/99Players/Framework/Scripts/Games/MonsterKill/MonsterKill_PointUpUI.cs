using System;
using UnityEngine;
public class MonsterKill_PointUpUI : MonoBehaviour
{
	private int playerNo;
	private GameObject target;
	[SerializeField]
	[Header("ル\u30fcト")]
	private Transform root;
	[SerializeField]
	[Header("ポイント")]
	private SpriteRenderer point;
	public void SetPlayerNo(int _playerNo)
	{
		playerNo = _playerNo;
	}
	public void Init(GameObject _target, MonsterKill_EnemyManager.DeadPointTpe _deadPoint)
	{
		target = _target;
		root.SetLocalPositionY(0f);
		MonsterKill_Define.UserType userType = SingletonCustom<MonsterKill_PlayerManager>.Instance.GetPlayer(playerNo).GetUserType();
		int charaIdx = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)userType];
		Sprite pointSprite = SingletonCustom<MonsterKill_UIManager>.Instance.GetPointSprite(charaIdx, _deadPoint);
		point.sprite = pointSprite;
		point.SetAlpha(1f);
	}
	public void SetOrderInLayer(int _orderInLayer)
	{
		point.sortingOrder = _orderInLayer;
	}
	public void UpdateMethod()
	{
		if (!(target == null))
		{
			Vector3 position = SingletonCustom<MonsterKill_CameraManager>.Instance.GetCamera(playerNo).WorldToScreenPoint(target.transform.position);
			position = SingletonCustom<MonsterKill_UIManager>.Instance.GetGlobalCamera().ScreenToWorldPoint(position);
			base.transform.SetPosition(position.x, position.y, base.transform.position.z);
		}
	}
	public void MoveUp()
	{
		float num = 0.5f;
		float delayTime = num * 0.8f;
		float alphaTime = num * 0.2f;
		LeanTween.moveLocalY(root.gameObject, 100f, num);
		LeanTween.delayedCall(root.gameObject, delayTime, (Action)delegate
		{
			LeanTween.value(root.gameObject, 1f, 0f, alphaTime).setOnUpdate(delegate(float _value)
			{
				point.SetAlpha(_value);
			}).setOnComplete((Action)delegate
			{
				base.gameObject.SetActive(value: false);
			});
		});
	}
}
