using System;
using UnityEngine;
using UnityEngine.Extension;
public class SmeltFishing_FishingSpace : MonoBehaviour
{
	[SerializeField]
	[DisplayName("イス")]
	private SmeltFishing_FishingChair chair;
	[SerializeField]
	[DisplayName("バケツ")]
	private SmeltFishing_FishingBucket bucket;
	[SerializeField]
	[Header("バケツを置くアンカ\u30fc")]
	private Transform bucketAnchor;
	[SerializeField]
	[DisplayName("魚影")]
	private SmeltFishing_SmeltShadows shadows;
	[SerializeField]
	[DisplayName("キャラクタ\u30fcの座る位置")]
	private Transform characterSitDownPoint;
	[SerializeField]
	private CapsuleCollider barrier;
	[SerializeField]
	private ParticleSystem bubble;
	private SmeltFishing_Character playingCharacter;
	private Vector3 originPos;
	public Vector3 SitDownPosition => characterSitDownPoint.position;
	public float SitDownEulerAngleY => characterSitDownPoint.eulerAngles.y;
	public void Init(SmeltFishing_Character character)
	{
		playingCharacter = character;
		chair.Init();
		bucket.Init();
		bucket.SetMaterial(SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)playingCharacter.ControlUser]);
		bucket.SetAnchor(SmeltFishing_FishingBucket.PLACE_TYPE.CharacterHand, playingCharacter.GetBucketAnchor());
		shadows.Init(playingCharacter);
		barrier.enabled = false;
		originPos = base.transform.position;
	}
	public void UpdateMethod()
	{
		shadows.UpdateMethod();
		bucket.UpdateMethod();
	}
	public void SitDown(SmeltFishing_IcePlate icePlate)
	{
		base.transform.position = icePlate.Position;
		base.transform.SetLocalPositionY(0f);
		chair.Show();
		bucket.SetAnchor(SmeltFishing_FishingBucket.PLACE_TYPE.Field, bucketAnchor);
		shadows.Activate(icePlate);
		barrier.enabled = true;
	}
	public void StandUp()
	{
		chair.Hide();
		bucket.SetAnchor(SmeltFishing_FishingBucket.PLACE_TYPE.CharacterHand, playingCharacter.GetBucketAnchor());
		shadows.Deactivate();
		barrier.enabled = false;
		base.transform.position = originPos;
	}
	public void ChangeChairLayer(int _layer)
	{
		chair.gameObject.layer = _layer;
	}
	public void AddSmelt(int count)
	{
		bucket.AddSmelt(count);
	}
	public void PlayBubble()
	{
		bubble.Play();
		LeanTween.delayedCall(base.gameObject, 1.3f, (Action)delegate
		{
			StopBubble();
		});
	}
	public void StopBubble()
	{
		bubble.Stop();
	}
}
