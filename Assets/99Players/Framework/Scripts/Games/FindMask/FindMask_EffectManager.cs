using UnityEngine;
public class FindMask_EffectManager : SingletonCustom<FindMask_EffectManager>
{
	public enum EFFECT_TYPE
	{
		FIND_PAIR_0,
		FIND_PAIR_1,
		FIND_RARE_PAIR_0,
		FIND_RARE_PAIR_1,
		NOT_FIND_PAIR_0,
		NOT_FIND_PAIR_1,
		RARE_MASK_0,
		RARE_MASK_1,
		MAX
	}
	[SerializeField]
	[Header("お面探しのエフェクト配列")]
	private ParticleSystem[] maskParticles;
	public void PlayMaskEffect(FindMask_MaskData _mask, EFFECT_TYPE _particleType)
	{
		CalcManager.mCalcVector3.x = SingletonCustom<FindMask_CharacterManager>.Instance.GetFieldCamera().WorldToScreenPoint(_mask.transform.position).x;
		CalcManager.mCalcVector3.y = SingletonCustom<FindMask_CharacterManager>.Instance.GetFieldCamera().WorldToScreenPoint(_mask.transform.position).y;
		CalcManager.mCalcVector3 = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(CalcManager.mCalcVector3);
		ParticleSystem obj = maskParticles[(int)_particleType];
		obj.gameObject.SetActive(value: true);
		obj.transform.SetPosition(CalcManager.mCalcVector3.x, CalcManager.mCalcVector3.y, 0f);
		obj.transform.SetLocalPositionZ(0f);
		obj.Play();
	}
}
