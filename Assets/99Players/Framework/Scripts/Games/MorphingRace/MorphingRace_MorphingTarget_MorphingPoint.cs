using UnityEngine;
public class MorphingRace_MorphingTarget_MorphingPoint : MonoBehaviour
{
	[SerializeField]
	[Header("変身する動物の種類")]
	private MorphingRace_FieldManager.TargetPrefType morphingTargetType;
	[SerializeField]
	[Header("変身ポイントを非表示にするかどうかのフラグ")]
	private bool isHidePoint;
	[SerializeField]
	[Header("変身ポイント用の画像やエフェクトを非表示にするかどうかのフラグ")]
	private bool isHidePointEffect;
	[SerializeField]
	[Header("変身ポイント用の画像")]
	private SpriteRenderer point;
	[SerializeField]
	[Header("変身ポイント用のエフェクト")]
	private ParticleSystem pointEffect;
	[SerializeField]
	[Header("変身ポイント用のコライダ\u30fc")]
	private MorphingRace_MorphingTarget_Collider collider;
	public void Init(MorphingRace_MorphingTarget _morphingRace_MorphingTarget)
	{
		collider.Init(_morphingRace_MorphingTarget, this);
		if (isHidePointEffect)
		{
			point.gameObject.SetActive(value: false);
			pointEffect.gameObject.SetActive(value: false);
		}
		else
		{
			pointEffect.Play();
		}
	}
	public void PlayBlinkPoint()
	{
		Color color = point.color;
		LeanTween.value(point.gameObject, 0.25f, 1f, 1.5f).setOnUpdate(delegate(float _value)
		{
			color.a = _value;
			point.color = color;
		}).setLoopPingPong();
	}
	public void Hide()
	{
		LeanTween.cancel(point.gameObject);
		base.gameObject.SetActive(value: false);
	}
	public MorphingRace_FieldManager.TargetPrefType GetMorphingTargetType()
	{
		return morphingTargetType;
	}
	public bool GetIsHidePoint()
	{
		return isHidePoint;
	}
}
