using System;
using UnityEngine;
public class RingToss_Target : MonoBehaviour
{
	[SerializeField]
	[Header("移動アンカ\u30fc(nullだと開始時自動設定)")]
	private Transform moveAnchor;
	[SerializeField]
	[Header("消しゴムくん or イチゴちゃんかどうかのフラグ")]
	private bool isSpecialObject;
	[SerializeField]
	[Header("ゴレンジャ\u30fc系かどうかのフラグ")]
	private bool isHeroToys;
	private int targetNo;
	[SerializeField]
	[Header("ポイント")]
	private int point;
	private float mass;
	private int lastHitCtrlNo;
	private int createGroupNo;
	private int createAnchorNo;
	[SerializeField]
	private Rigidbody rigid;
	[SerializeField]
	private MeshRenderer mesh;
	[SerializeField]
	private Collider[] colliders;
	[SerializeField]
	[Header("ヒットエフェクト")]
	protected ParticleSystem hitEffect;
	private bool isScoreNotification;
	[SerializeField]
	[Header("AIの狙う位置アンカ\u30fc")]
	private Transform aiTargetAnchor;
	private bool isGet;
	private bool isShow;
	private ParticleSystem appearEffect;
	private float appearTime;
	private Vector3 createPos;
	public bool IsGet => isGet;
	public bool IsShow => isShow;
	public bool IsWaitShow
	{
		get
		{
			if (!isGet)
			{
				return !isShow;
			}
			return false;
		}
	}
	public bool IsCanAiTarget
	{
		get
		{
			if (!isGet)
			{
				return isShow;
			}
			return false;
		}
	}
	public bool IsSpecialObject => isSpecialObject;
	public bool IsHeroToys => isHeroToys;
	public int TargetNo
	{
		get
		{
			return targetNo;
		}
		set
		{
			targetNo = value;
		}
	}
	public int LastHitCtrlNo
	{
		get
		{
			return lastHitCtrlNo;
		}
		set
		{
			lastHitCtrlNo = value;
		}
	}
	public int CreateGroupNo
	{
		get
		{
			return createGroupNo;
		}
		set
		{
			createGroupNo = value;
		}
	}
	public int CreateAnchorNo
	{
		get
		{
			return createAnchorNo;
		}
		set
		{
			createAnchorNo = value;
		}
	}
	public int Point => point;
	public void Init(int _targetNo)
	{
		targetNo = _targetNo;
		if (moveAnchor == null)
		{
			moveAnchor = base.transform;
		}
		if (isSpecialObject)
		{
			createPos = base.transform.position;
		}
	}
	public void SecondGroupInit()
	{
		isGet = false;
		Hide();
		rigid.velocity = Vector3.zero;
		rigid.angularVelocity = Vector3.zero;
	}
	public void UpdateMethod()
	{
		if (!isGet)
		{
			return;
		}
		appearTime += Time.deltaTime;
		if (isSpecialObject)
		{
			if (appearTime > 5f)
			{
				appearTime = 0f;
				Show(_isInit: false);
				isGet = false;
			}
		}
		else if (appearTime > 1f)
		{
			appearTime = 0f;
			SingletonCustom<RingToss_TargetManager>.Instance.ShowRandomTarget();
			isGet = false;
		}
	}
	public void TargetGet(int _ctrlNo)
	{
		if (!isGet)
		{
			isGet = true;
			SingletonCustom<RingToss_ScoreManager>.Instance.AddScore(_ctrlNo, point, GetPos());
			Hide();
		}
	}
	public void Show(bool _isInit)
	{
		base.gameObject.SetActive(value: true);
		if (_isInit)
		{
			SetPos(createPos);
			isGet = false;
			SetColliderActive(_active: true);
		}
		else
		{
			if (appearEffect == null)
			{
				ParticleSystem appearEffectPrefab = SingletonCustom<RingToss_TargetManager>.Instance.GetAppearEffectPrefab();
				appearEffect = UnityEngine.Object.Instantiate(appearEffectPrefab, SingletonCustom<RingToss_TargetManager>.Instance.GetAppearEffectAnchor());
				appearEffect.transform.SetPositionY(SingletonCustom<RingToss_RingManager>.Instance.GetRingTargetBottomPosY());
				if (isSpecialObject)
				{
					appearEffect.transform.position = base.transform.position;
					appearEffect.transform.parent = base.transform.parent;
				}
			}
			if (!isSpecialObject)
			{
				appearEffect.transform.position = createPos;
			}
			appearEffect.Play();
			SetPos(createPos);
			base.transform.AddPositionY(0.5f);
			LeanTween.moveY(base.gameObject, createPos.y, 0.2f).setOnComplete((Action)delegate
			{
				isGet = false;
				SetColliderActive(_active: true);
			});
		}
		isShow = true;
	}
	public void Hide()
	{
		base.gameObject.SetActive(value: false);
		SetColliderActive(_active: false);
		isShow = false;
		if (isHeroToys)
		{
			SingletonCustom<RingToss_TargetManager>.Instance.HideHeroToy();
		}
		if (!isSpecialObject)
		{
			SingletonCustom<RingToss_TargetManager>.Instance.SetCreateGroupUseFlag(createGroupNo, createAnchorNo, _flag: false);
		}
	}
	public Transform GetAiTargetAnchor()
	{
		return aiTargetAnchor;
	}
	public Vector3 GetAiTargetPos()
	{
		return aiTargetAnchor.position;
	}
	public Vector3 GetPos()
	{
		return moveAnchor.position;
	}
	public void SetPos(Vector3 _pos)
	{
		if (!isSpecialObject)
		{
			moveAnchor.position = _pos;
		}
	}
	public void SetColliderActive(bool _active)
	{
		for (int i = 0; i < colliders.Length; i++)
		{
			colliders[i].enabled = _active;
		}
	}
	public void SetCreatePos(Vector3 _pos)
	{
		createPos = _pos;
	}
}
