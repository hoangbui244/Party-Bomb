using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Extension;
public class SmeltFishing_CharacterFishingRod : MonoBehaviour
{
	[SerializeField]
	private SmeltFishing_CharacterFishingRodConfig config;
	[SerializeField]
	[DisplayName("釣竿")]
	private SkinnedMeshRenderer rod;
	[SerializeField]
	[DisplayName("釣竿パ\u30fcツ")]
	private MeshRenderer[] arryaRodParts;
	[SerializeField]
	[DisplayName("釣り糸")]
	private LineRenderer fishingLine;
	[SerializeField]
	[DisplayName("アニメ\u30fcション")]
	private Animation animation;
	[SerializeField]
	[DisplayName("釣り竿の先端")]
	private Transform lineTop;
	[SerializeField]
	[DisplayName("釣り竿の先端")]
	private Transform lineBottom;
	[SerializeField]
	[DisplayName("釣り竿の先端追従のル\u30fcト")]
	private Transform lineRoot;
	[SerializeField]
	[DisplayName("糸を垂らす時のル\u30fcト")]
	private Transform lineUpDownRoot;
	[SerializeField]
	[DisplayName("針に掛かったわかさぎ")]
	private SmeltFishing_CaughtSmelt[] caughtSmelts;
	[SerializeField]
	[DisplayName("釣りの時だけアクティブ")]
	private GameObject[] activeChangeObjects;
	[SerializeField]
	private Material[] rodMaterials;
	[SerializeField]
	private Material[] rodPartsMaterials;
	private SmeltFishing_Character playingCharacter;
	private SmeltFishing_CharacterSfx sfx;
	[SerializeField]
	[Header("ハンドル")]
	private GameObject handle;
	[SerializeField]
	[Header("ハンドルを回転させる速度")]
	private float HANDLE_ROT_SPEED;
	private Coroutine castLineAnimationCoroutine;
	private bool IsCast
	{
		get;
		set;
	}
	public bool IsPlayingAnimation
	{
		get;
		private set;
	}
	public void Init(SmeltFishing_Character character)
	{
		playingCharacter = character;
		sfx = playingCharacter.GetComponent<SmeltFishing_CharacterSfx>();
		rod.sharedMaterial = rodMaterials[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)playingCharacter.ControlUser]];
		for (int i = 0; i < arryaRodParts.Length; i++)
		{
			arryaRodParts[i].sharedMaterial = rodPartsMaterials[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)playingCharacter.ControlUser]];
		}
		HideSmelts();
		Deactivate();
	}
	public void UpdateMethod()
	{
		lineRoot.position = lineTop.position;
		fishingLine.SetPosition(0, lineRoot.position);
		fishingLine.SetPosition(1, lineBottom.position);
		SmeltFishing_CaughtSmelt[] array = caughtSmelts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateMethod();
		}
	}
	public void HandleUpdateMethod(bool _isRollUp)
	{
		handle.transform.AddLocalEulerAnglesX((_isRollUp ? (0f - HANDLE_ROT_SPEED) : HANDLE_ROT_SPEED) * Time.deltaTime);
	}
	public void Activate()
	{
		GameObject[] array = activeChangeObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: true);
		}
	}
	public void Deactivate()
	{
		GameObject[] array = activeChangeObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
	}
	public void ShowSmelts(int smeltCount)
	{
		int[] array = (from num in Enumerable.Range(0, 5)
			orderby Guid.NewGuid()
			select num).ToArray();
		for (int i = 0; i < smeltCount; i++)
		{
			int num2 = array[i];
			caughtSmelts[num2].Activate();
		}
	}
	public void HideSmelts()
	{
		SmeltFishing_CaughtSmelt[] array = caughtSmelts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Deactivate();
		}
	}
	public void CastLine()
	{
		castLineAnimationCoroutine = StartCoroutine(CastLineAnimation());
	}
	public void RollUp()
	{
		if (IsPlayingAnimation)
		{
			StopCoroutine(castLineAnimationCoroutine);
		}
		StartCoroutine(RollUpAnimation());
	}
	public void PlayFishFightAnimation()
	{
		if (animation.isPlaying)
		{
			animation.CrossFade("FishingLod_Catch");
		}
		else
		{
			animation.Play("FishingLod_Catch");
		}
	}
	public void PlayIdleAnimation()
	{
		if (animation.isPlaying)
		{
			animation.CrossFade("FishingLod_Idle");
		}
		else
		{
			animation.Play("FishingLod_Idle");
		}
	}
	public void StopAnimation()
	{
		animation.Play("FishingLod_Default");
	}
	private IEnumerator CastLineAnimation()
	{
		IsPlayingAnimation = true;
		float elapsed = 0f;
		sfx.PlayCastLineSfx();
		while (elapsed < config.CastDuration)
		{
			elapsed += Time.deltaTime;
			float t = Mathf.Clamp01(elapsed / config.CastDuration);
			lineUpDownRoot.transform.localPosition = Vector3.Lerp(config.OriginalLinePosition, config.CastLinePosition, t);
			yield return null;
		}
		IsPlayingAnimation = false;
		IsCast = true;
	}
	private IEnumerator RollUpAnimation()
	{
		float num = CalcManager.Length(config.OriginalLinePosition, config.CastLinePosition);
		float num2 = CalcManager.Length(config.OriginalLinePosition, lineUpDownRoot.transform.localPosition);
		float lerp = num2 / num;
		IsPlayingAnimation = true;
		float elapsed = 0f;
		while (elapsed < config.RollUpDuration * lerp)
		{
			elapsed += Time.deltaTime;
			float t = Mathf.Clamp01(elapsed / (config.RollUpDuration * lerp));
			lineUpDownRoot.transform.localPosition = Vector3.Lerp(config.CastLinePosition, config.OriginalLinePosition, t);
			yield return null;
		}
		IsPlayingAnimation = false;
		sfx.StopRollupSfx();
		IsCast = false;
	}
}
