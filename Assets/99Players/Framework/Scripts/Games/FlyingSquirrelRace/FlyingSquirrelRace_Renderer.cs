using System.Collections;
using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_Renderer : DecoratedMonoBehaviour
{
	private const int CapeBlendShapeIndex = 0;
	[SerializeField]
	[Header("最小値 : blendShape")]
	private float MinBlendShape;
	[SerializeField]
	[Header("最大値 : blendShape")]
	private float MaxBlendShape = 50f;
	[SerializeField]
	[DisplayName("風呂敷レンダラ\u30fc")]
	private SkinnedMeshRenderer renderer;
	[SerializeField]
	[DisplayName("風呂敷レンダラ\u30fc（アニメ\u30fcション用）")]
	private SkinnedMeshRenderer renderer_anim;
	[SerializeField]
	[DisplayName("キャラクタ\u30fcモデル")]
	private CharacterStyle characterStyle;
	[SerializeField]
	[DisplayName("膨らむ時間")]
	private float inflateTime = 0.5f;
	[SerializeField]
	[DisplayName("収縮時間 : blendShape")]
	private float shrinkageTime = 2f;
	[SerializeField]
	[DisplayName("点滅間隔")]
	private float flashInterval = 0.05f;
	[SerializeField]
	[DisplayName("コイン取得エフェクト")]
	private ParticleSystem coinCollectEffect;
	[SerializeField]
	[DisplayName("速度アップのエフェクト")]
	private ParticleSystem speedUpEffect;
	[SerializeField]
	[DisplayName("障害物（杭）に当たった時のエフェクト")]
	private ParticleSystem obstaclePileHitEffect;
	[SerializeField]
	[DisplayName("障害物に当たった時のエフェクト")]
	private ParticleSystem obstacleBadEffect;
	private FlyingSquirrelRace_Player owner;
	private Coroutine flash;
	private Renderer[] renderers;
	private float blendShape;
	private float blendShapeRepeat;
	private const int CapeBlendShapeIndex_2 = 1;
	[SerializeField]
	[Header("最小値 : blendShape_2")]
	private float MinBlendShape_2;
	[SerializeField]
	[Header("最大値 : blendShape_2")]
	private float MaxBlendShape_2 = 50f;
	[SerializeField]
	[Header("収縮時間 : blendShape_2")]
	private float shrinkageTime_2 = 0.15f;
	private float blendShape_2;
	private float blendShapeRepeat_2;
	public void Initialize(FlyingSquirrelRace_Player player)
	{
		owner = player;
		characterStyle.SetGameStyle(GS_Define.GameType.ARCHER_BATTLE, (int)owner.Controller);
		Material furoshiki = SingletonMonoBehaviour<FlyingSquirrelRace_Players>.Instance.GetFuroshiki(SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)owner.Controller]);
		renderer.material = furoshiki;
		renderer_anim.material = furoshiki;
		blendShape = GetCapeSize(0);
		blendShape_2 = GetCapeSize(1);
		renderers = GetComponentsInChildren<Renderer>(includeInactive: false);
	}
	public void UpdateMethod()
	{
		bool flag = SingletonCustom<FlyingSquirrelRace_Input>.Instance.IsDownButtonA(owner.Controller);
		if (flag)
		{
			owner.Rise();
			flag = SingletonCustom<FlyingSquirrelRace_Input>.Instance.IsHoldButtonA(owner.Controller);
		}
		if (flag)
		{
			blendShape += Time.fixedDeltaTime * MaxBlendShape / inflateTime;
			blendShapeRepeat = blendShape;
		}
		else
		{
			blendShapeRepeat += Time.fixedDeltaTime * MaxBlendShape / shrinkageTime;
			blendShape = Mathf.PingPong(blendShapeRepeat, MaxBlendShape);
		}
		blendShape = Mathf.Clamp(blendShape, MinBlendShape, MaxBlendShape);
		SetCapeSize(0, blendShape);
		blendShapeRepeat_2 += Time.fixedDeltaTime * MaxBlendShape_2 / shrinkageTime_2;
		blendShape_2 = Mathf.PingPong(blendShapeRepeat_2, MaxBlendShape_2);
		blendShape_2 = Mathf.Clamp(blendShape_2, MinBlendShape_2, MaxBlendShape_2);
		SetCapeSize(1, blendShape_2);
	}
	public void UpdateForAIMethod(FlyingSquirrelRace_AI ai)
	{
		if (ai.IsPressButtonA)
		{
			blendShape += Time.fixedDeltaTime * MaxBlendShape / inflateTime;
			blendShapeRepeat = blendShape;
		}
		else
		{
			blendShapeRepeat += Time.fixedDeltaTime * MaxBlendShape / shrinkageTime;
			blendShape = Mathf.PingPong(blendShapeRepeat, MaxBlendShape);
		}
		blendShape = Mathf.Clamp(blendShape, MinBlendShape, MaxBlendShape);
		SetCapeSize(0, blendShape);
		blendShapeRepeat_2 += Time.fixedDeltaTime * MaxBlendShape_2 / shrinkageTime_2;
		blendShape_2 = Mathf.PingPong(blendShapeRepeat_2, MaxBlendShape_2);
		blendShape_2 = Mathf.Clamp(blendShape_2, MinBlendShape_2, MaxBlendShape_2);
		SetCapeSize(1, blendShape_2);
	}
	public float GetCapeSize(int _capeBlendShapeIndex)
	{
		return renderer.GetBlendShapeWeight(_capeBlendShapeIndex);
	}
	public void SetCapeSize(int _capeBlendShapeIndex, float blendShapeSize)
	{
		renderer.SetBlendShapeWeight(_capeBlendShapeIndex, blendShapeSize);
	}
	public void PlayCoinCollectEffect()
	{
		coinCollectEffect.Play();
	}
	public void PlaySpeedUpEffect()
	{
		speedUpEffect.Play();
	}
	public void PlayObstaclePileHitEffect()
	{
		obstaclePileHitEffect.Play();
	}
	public void PlayobstacleBadEffect()
	{
		obstacleBadEffect.Play();
	}
	public void PlayFlash(float duration)
	{
		if (flash == null)
		{
			flash = StartCoroutine(Flash(duration));
		}
	}
	private IEnumerator Flash(float duration)
	{
		float elapsed2 = 0f;
		bool enable = false;
		Renderer[] array;
		while (elapsed2 < duration)
		{
			elapsed2 += Time.deltaTime;
			array = renderers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = enable;
			}
			enable = !enable;
			yield return new WaitForSeconds(flashInterval);
			elapsed2 += flashInterval;
		}
		array = renderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
		flash = null;
	}
}
