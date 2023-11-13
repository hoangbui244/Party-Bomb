using System.Collections.Generic;
using UnityEngine;
public class Fishing_RodUki : MonoBehaviour
{
	[SerializeField]
	[Header("放物線移動をするウキのアンカ\u30fc")]
	private OrbitCalculation ukiAnchor;
	[SerializeField]
	[Header("タ\u30fcゲットカ\u30fcソルのアンカ\u30fc")]
	private Transform targetCursorAnchor;
	[SerializeField]
	[Header("ウキのオブジェクト")]
	private GameObject ukiObject;
	[SerializeField]
	[Header("ウキのRigidbody")]
	private Rigidbody rigid;
	[SerializeField]
	[Header("波紋エフェクト")]
	private ParticleSystem rippleEffect;
	[SerializeField]
	[Header("魚が暴れる時の波紋エフェクト")]
	private ParticleSystem fishFightRippleEffect;
	[SerializeField]
	[Header("釣り上げる時のエフェクト")]
	private ParticleSystem fishingEffect;
	[SerializeField]
	[Header("重心")]
	private Transform centerOfMass;
	[SerializeField]
	[Header("タ\u30fcゲットカ\u30fcソル")]
	private GameObject targetCursorSprite;
	[SerializeField]
	[Header("ウキのメッシュレンダラ\u30fc")]
	private MeshRenderer ukiMeshRenderer;
	[SerializeField]
	[Header("ウキの左側のトレイル")]
	private TrailRenderer trailRenderer_Left;
	[SerializeField]
	[Header("ウキの右側のトレイル")]
	private TrailRenderer trailRenderer_Right;
	private bool isFishingMode;
	private const float FISHING_MODE_START_TIME = 0.5f;
	private const float FISHING_MODE_STOP_TIME = 0.5f;
	private const float UKI_HEIGHT = 0.03f;
	private const float UKI_UP_SPEED = 0.5f;
	private const float UKI_SINK_SPEED = 0.1f;
	private const float UKI_MOVE_AROUND_SPEED = 0.3f;
	private const float FLOW_POWER = 0.1f;
	private const float FLOW_LIMIT_DISTANCE = 0.5f;
	private bool isUkiSink;
	private bool isUkiMoveAround;
	private float ukiMoveAroundTime;
	private float ukiMoveAround_X;
	private float ukiMoveAround_Z;
	private Vector2 ukiMoveAroundRange = new Vector2(0.1f, 0.1f);
	private Vector3 originFloatPoint = Vector3.zero;
	private float defTargetCursorScale = -1f;
	private const float DEF_CHECK_COLLIDER_RADIUS = 0.2f;
	private Collider[] overlapHitColliders;
	private Vector3 overlapCapsulePoint0 = new Vector3(0f, 0f, 0f);
	private Vector3 overlapCapsulePoint1 = new Vector3(0f, -0.6f, 0f);
	private float overlapCapsuleRadius = 0.2f;
	private List<Fishing_FishShadow> fishShadowAreaInList = new List<Fishing_FishShadow>();
	public void Init(FishingDefinition.User user)
	{
		rigid.centerOfMass = centerOfMass.localPosition;
		fishShadowAreaInList.Clear();
		overlapCapsuleRadius = 0f;
		isFishingMode = false;
		trailRenderer_Left.enabled = false;
		trailRenderer_Right.enabled = false;
		defTargetCursorScale = targetCursorSprite.transform.localScale.x;
	}
	public void UpdateMethod()
	{
		if (!isFishingMode)
		{
			return;
		}
		CheckFishShadowInArea();
		if (!isUkiSink)
		{
			ukiObject.transform.SetLocalPositionY(Mathf.Lerp(ukiObject.transform.localPosition.y, 0.03f, 0.5f));
			FlowUki();
		}
		else
		{
			rigid.velocity = Vector3.zero;
		}
		if (isUkiMoveAround)
		{
			rigid.velocity = Vector3.zero;
			if (ukiMoveAroundTime < 0f)
			{
				ukiMoveAroundTime = UnityEngine.Random.Range(0.1f, 0.2f);
				ukiMoveAround_X = UnityEngine.Random.Range(0f - ukiMoveAroundRange.x, ukiMoveAroundRange.x);
				ukiMoveAround_Z = UnityEngine.Random.Range(0f - ukiMoveAroundRange.y, ukiMoveAroundRange.y);
			}
			else
			{
				ukiMoveAroundTime -= Time.deltaTime;
				ukiObject.transform.localPosition = Vector3.Lerp(ukiObject.transform.localPosition, new Vector3(ukiMoveAround_X, 0.03f, ukiMoveAround_Z), 0.3f);
			}
		}
		if (Vector3.Distance(originFloatPoint, ukiAnchor.transform.position) > 0.5f)
		{
			rigid.velocity = Vector3.zero;
		}
	}
	public void LandingUki()
	{
		originFloatPoint = ukiAnchor.transform.position;
		rigid.isKinematic = false;
		isFishingMode = true;
		trailRenderer_Left.transform.localPosition = -base.transform.right * 0.05f;
		trailRenderer_Right.transform.localPosition = base.transform.right * 0.05f;
		trailRenderer_Left.enabled = true;
		trailRenderer_Right.enabled = true;
	}
	private void FlowUki()
	{
		rigid.velocity = Vector3.back * 0.1f;
	}
	public void FloatUki()
	{
		isUkiSink = false;
	}
	public void ShakeUki()
	{
		ParticleSystem particleSystem = UnityEngine.Object.Instantiate(rippleEffect, Vector3.zero, rippleEffect.transform.rotation, ukiObject.transform);
		particleSystem.gameObject.SetActive(value: true);
		particleSystem.transform.SetLocalPosition(0f, 0f, 0f);
		ukiObject.transform.AddLocalPositionY(-0.05f);
	}
	public void SinkUki()
	{
		isUkiSink = true;
		LeanTween.moveLocalY(ukiObject, ukiObject.transform.localPosition.y - 0.2f, 0.1f);
	}
	public void AroundMoveUki()
	{
		isUkiSink = false;
		isUkiMoveAround = true;
		fishFightRippleEffect.Play();
	}
	public void PlayFishingEffect()
	{
		ParticleSystem particleSystem = UnityEngine.Object.Instantiate(fishingEffect, Vector3.zero, rippleEffect.transform.rotation, base.transform);
		particleSystem.transform.position = ukiAnchor.transform.position;
		particleSystem.gameObject.SetActive(value: true);
	}
	public void ResetUki()
	{
		isUkiSink = false;
		isUkiMoveAround = false;
		ukiObject.transform.SetLocalPosition(0f, 0.03f, 0f);
		fishFightRippleEffect.Stop();
		rigid.isKinematic = true;
		fishShadowAreaInList.Clear();
		isFishingMode = false;
		overlapCapsuleRadius = 0f;
		trailRenderer_Left.enabled = false;
		trailRenderer_Right.enabled = false;
	}
	public void SetTargetCursorActive(bool _isActive)
	{
		targetCursorSprite.SetActive(_isActive);
	}
	public void SetTargetCursorAnimationActive(bool _active)
	{
		if (_active)
		{
			LeanTween.scale(targetCursorSprite, Vector3.one * defTargetCursorScale, 0.5f);
			LeanTween.value(base.gameObject, 0.2f, 0f, 0.5f).setOnUpdate(delegate(float val)
			{
				overlapCapsuleRadius = val;
			});
		}
		else
		{
			LeanTween.scale(targetCursorSprite, Vector3.zero, 0.5f);
			LeanTween.value(base.gameObject, 0f, 0.2f, 0.5f).setOnUpdate(delegate(float val)
			{
				overlapCapsuleRadius = val;
			});
		}
	}
	public void SetUkiMaterial(Material _mat)
	{
		ukiMeshRenderer.material = _mat;
	}
	public Transform GetUkiAnchor()
	{
		return ukiAnchor.transform;
	}
	public OrbitCalculation GetUkiOrbitCalculation()
	{
		return ukiAnchor;
	}
	public Transform GetTargetCursorAnchor()
	{
		return targetCursorAnchor;
	}
	public int GetFishingAreaInShadows()
	{
		return fishShadowAreaInList.Count;
	}
	public List<Fishing_FishShadow> GetAllFishShadows()
	{
		return fishShadowAreaInList;
	}
	public Fishing_FishShadow GetRandomSelectFishShadow()
	{
		return fishShadowAreaInList[Random.Range(0, fishShadowAreaInList.Count)];
	}
	public FishingDefinition.BiteDifficulty GetBiteDifficulty()
	{
		return FishingDefinition.BiteDifficulty.Easy;
	}
	public bool IsTargetCursorActive()
	{
		return targetCursorSprite.activeSelf;
	}
	public bool IsFishingMode()
	{
		return isFishingMode;
	}
	private void CheckFishShadowInArea()
	{
		overlapHitColliders = Physics.OverlapCapsule(overlapCapsulePoint0 + ukiAnchor.transform.position, overlapCapsulePoint1 + ukiAnchor.transform.position, overlapCapsuleRadius);
		for (int i = 0; i < overlapHitColliders.Length; i++)
		{
			if (!(overlapHitColliders[i].GetComponent<Fishing_FishShadow>() != null))
			{
				continue;
			}
			if (fishShadowAreaInList.Count == 0)
			{
				fishShadowAreaInList.Add(overlapHitColliders[i].GetComponent<Fishing_FishShadow>());
				continue;
			}
			for (int j = 0; j < fishShadowAreaInList.Count; j++)
			{
				if (!fishShadowAreaInList.Contains(overlapHitColliders[i].GetComponent<Fishing_FishShadow>()))
				{
					fishShadowAreaInList.Add(overlapHitColliders[i].GetComponent<Fishing_FishShadow>());
				}
			}
		}
	}
	private void OnDrawGizmos()
	{
		Gizmos.DrawLine(ukiAnchor.transform.position + overlapCapsulePoint0, ukiAnchor.transform.position + overlapCapsulePoint1);
		Gizmos.DrawWireSphere(ukiAnchor.transform.position + overlapCapsulePoint0, overlapCapsuleRadius);
		Gizmos.DrawWireSphere(ukiAnchor.transform.position + overlapCapsulePoint1, overlapCapsuleRadius);
	}
}
