using System;
using UnityEngine;
public class GoldfishPoi : MonoBehaviour
{
	private const float MAX_HP = 100f;
	private const float WATER_DAMAGE_SPEED = 1f;
	private const float WATER_INVALID_HP = 30f;
	private static readonly Color PAPER_DEFAULT_COLOR = new Color(0.7686275f, 0.7686275f, 0.7686275f, 1f);
	private static readonly Color PAPER_WET_COLOR = new Color(0.5f, 0.5f, 0.5f, 0.5f);
	private const float WATER_IN_ANIMATION_TIME = 0.3f;
	private const float SCOOP_ANIMATION_TIME = 0.3f;
	private static readonly Vector3 POI_DEFAULT_LOCAL_POS = new Vector3(0.1f, 0.05f, 0f);
	private static readonly Vector3 POI_WATER_LOCAL_POS = new Vector3(0f, -0.15f, 0f);
	private static readonly Vector3 POI_SCOOP_LOCAL_POS = new Vector3(0f, 0f, 0f);
	private const float POI_DEFAULT_ANGLE = 60f;
	private const float POI_WATER_ANGLE = 0f;
	private const float POI_SCOOP_ANGLE = 0f;
	private const float RIPPLE_EFFECT_LOCAL_POS_Y = -0.05f;
	[SerializeField]
	private Transform poiAnchor;
	[SerializeField]
	private MeshRenderer poiRenderer;
	[SerializeField]
	private MeshRenderer paperRenderer;
	[SerializeField]
	private MeshFilter paperMeshFilter;
	[SerializeField]
	private Mesh[] paperMeshes;
	[SerializeField]
	private ParticleSystem rippleEffect;
	[SerializeField]
	private Transform playerUiAnchor;
	private Rigidbody rigid;
	private GoldfishCharacterScript chara;
	private float hp;
	private bool isBreakPaper;
	private bool isNearBreak;
	private bool isWaterInAnim;
	private bool isScoopAnim;
	private bool isWaterIn;
	private Vector3 prevWaterInPos;
	private float waterInMoveLength;
	private bool isRotReverse;
	private Action callback;
	public GoldfishCharacterScript Chara
	{
		get
		{
			return chara;
		}
		set
		{
			chara = value;
		}
	}
	public float Hp
	{
		get
		{
			return hp;
		}
		set
		{
			hp = value;
		}
	}
	public float HpLerp => hp / 100f;
	public bool IsPlayer => chara.IsPlayer;
	public bool IsBreakPaper => isBreakPaper;
	public bool IsWaterInAnim => isWaterInAnim;
	public bool IsScoopAnim => isScoopAnim;
	public bool IsAnimation
	{
		get
		{
			if (!isWaterInAnim)
			{
				return isScoopAnim;
			}
			return true;
		}
	}
	public bool IsWaterIn => isWaterIn;
	public float WaterInMoveLength
	{
		get
		{
			return waterInMoveLength;
		}
		set
		{
			waterInMoveLength = value;
		}
	}
	public void Init()
	{
		rigid = GetComponent<Rigidbody>();
		hp = 100f;
		isWaterInAnim = false;
		isScoopAnim = false;
		LeanTween.cancel(base.gameObject);
		paperRenderer.transform.SetLocalEulerAnglesY(UnityEngine.Random.Range(0f, 360f));
		isRotReverse = (Mathf.Abs(180f - base.transform.localEulerAngles.y) < 30f);
		RotUpdate();
	}
	public void UpdateMethod()
	{
		if (isWaterIn)
		{
			if (hp > 30f)
			{
				hp -= 1f * Time.deltaTime;
				PaperColorSetting();
			}
			Vector3 vector = poiAnchor.position - prevWaterInPos;
			vector.y = 0f;
			waterInMoveLength += vector.magnitude;
			prevWaterInPos = poiAnchor.position;
		}
		RotUpdate();
	}
	public void Move(Vector3 _vec)
	{
		rigid.velocity = _vec;
	}
	private void RotUpdate()
	{
		Vector3 tubLeftBottomPos = SingletonCustom<GoldfishGameManager>.Instance.GetTubLeftBottomPos();
		float num = Mathf.InverseLerp(b: SingletonCustom<GoldfishGameManager>.Instance.GetTubRightTopPos().x, a: tubLeftBottomPos.x, value: poiAnchor.position.x);
		if (isRotReverse)
		{
			poiAnchor.SetLocalEulerAnglesY(15f - 30f * num);
		}
		else
		{
			poiAnchor.SetLocalEulerAnglesY(-15f + 30f * num);
		}
	}
	public void PaperColorSetting()
	{
		paperRenderer.material.SetColor("_Color", Color.Lerp(PAPER_WET_COLOR, PAPER_DEFAULT_COLOR, hp / 100f));
	}
	public void AddDamage(float _value)
	{
		hp -= _value;
		if (hp < 30f)
		{
			NearBreak();
		}
		if (hp < 0f)
		{
			BreakPaper();
		}
	}
	public void BreakPaper()
	{
		if (!isBreakPaper)
		{
			isBreakPaper = true;
			paperMeshFilter.sharedMesh = paperMeshes[2];
			paperRenderer.transform.localScale = Vector3.one;
			SingletonCustom<GoldfishUiManager>.Instance.ViewOut(chara);
			LeanTween.delayedCall(2f, (Action)delegate
			{
				base.gameObject.SetActive(value: false);
			});
		}
	}
	public void NearBreak()
	{
		if (!isNearBreak)
		{
			isNearBreak = true;
			paperMeshFilter.sharedMesh = paperMeshes[1];
		}
	}
	public void WaterInAnimationStart()
	{
		isWaterInAnim = true;
		rigid.isKinematic = true;
		rippleEffect.transform.localPosition = POI_DEFAULT_LOCAL_POS;
		rippleEffect.transform.SetLocalPositionY(-0.05f);
		rippleEffect.Play();
		if (IsPlayer)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_goldfish_water_in");
		}
		LeanTween.value(base.gameObject, 0f, 1f, 0.3f).setOnUpdate(delegate(float _value)
		{
			poiAnchor.SetLocalEulerAnglesZ(Mathf.Lerp(60f, 0f, _value));
			poiAnchor.localPosition = Vector3.Lerp(POI_DEFAULT_LOCAL_POS, POI_WATER_LOCAL_POS, _value);
		}).setOnComplete((Action)delegate
		{
			poiAnchor.SetLocalEulerAnglesZ(0f);
			poiAnchor.localPosition = POI_WATER_LOCAL_POS;
			prevWaterInPos = poiAnchor.position;
			waterInMoveLength = 0f;
			isWaterInAnim = false;
			isWaterIn = true;
			rigid.isKinematic = false;
		});
	}
	public void ScoopAnimationStart(Action _callback)
	{
		callback = _callback;
		callback();
		isScoopAnim = true;
		isWaterIn = false;
		rigid.isKinematic = true;
		rippleEffect.transform.localPosition = POI_SCOOP_LOCAL_POS;
		rippleEffect.transform.SetLocalPositionY(-0.05f);
		rippleEffect.Play();
		if (IsPlayer)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_goldfish_scoop");
		}
		LeanTween.value(base.gameObject, 0f, 1f, 0.3f).setOnUpdate(delegate(float _value)
		{
			poiAnchor.SetLocalEulerAnglesZ(Mathf.Lerp(0f, 0f, _value));
			poiAnchor.localPosition = Vector3.Lerp(POI_WATER_LOCAL_POS, POI_SCOOP_LOCAL_POS, _value);
		}).setOnComplete((Action)delegate
		{
			poiAnchor.SetLocalEulerAnglesZ(0f);
			poiAnchor.localPosition = POI_SCOOP_LOCAL_POS;
			isScoopAnim = false;
			poiAnchor.SetLocalEulerAnglesZ(0f);
			poiAnchor.localPosition = Vector3.zero;
			rigid.isKinematic = false;
		});
	}
	public Transform GetPoiAnchor()
	{
		return poiAnchor;
	}
	public Vector3 GetPoiPos()
	{
		return poiAnchor.position;
	}
	public Transform GetPlayerUiAnchor()
	{
		return playerUiAnchor;
	}
	public Vector3 GetPlayerUiPos()
	{
		return playerUiAnchor.position;
	}
	public void SetMaterial(Material _mat)
	{
		poiRenderer.sharedMaterial = _mat;
	}
}
