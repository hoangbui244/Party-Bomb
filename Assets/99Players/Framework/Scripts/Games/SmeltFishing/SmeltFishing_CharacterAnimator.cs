using System;
using UnityEngine;
using UnityEngine.Extension;
public class SmeltFishing_CharacterAnimator : MonoBehaviour
{
	public enum Animation
	{
		Wait,
		RodSetUp,
		ShitAndFishing,
		RodCast,
		FishFight,
		Fishing,
		RodCancel,
		None
	}
	[Serializable]
	public class BodyParts
	{
		[SerializeField]
		[DisplayName("頭")]
		private Transform head;
		[SerializeField]
		[DisplayName("胴体")]
		private Transform torso;
		[SerializeField]
		[DisplayName("腰")]
		private Transform waist;
		[SerializeField]
		[DisplayName("左腕")]
		private Transform leftArm;
		[SerializeField]
		[DisplayName("右腕")]
		private Transform rightArm;
		[SerializeField]
		[DisplayName("左足")]
		private Transform leftLeg;
		[SerializeField]
		[DisplayName("右足")]
		private Transform rightLeg;
		public Transform Head => head;
		public Transform Torso => torso;
		public Transform Waist => waist;
		public Transform LeftArm => leftArm;
		public Transform RightArm => rightArm;
		public Transform LeftLeg => leftLeg;
		public Transform RightLeg => rightLeg;
	}
	[SerializeField]
	[DisplayName("モ\u30fcションデ\u30fcタ")]
	private CharaLeanTweenMotion motion;
	[SerializeField]
	private SmeltFishing_CharacterSfx audio;
	[SerializeField]
	[DisplayName("ボディパ\u30fcツ")]
	private BodyParts bodyParts;
	[SerializeField]
	[DisplayName("共通設定")]
	private SmeltFishing_CharacterAnimatorConfig config;
	private bool initialized;
	private Animation currentMotion = Animation.None;
	private bool useMotion;
	private bool isDuringMotionAnimation;
	private float walkAnimationTime;
	public void Init()
	{
		if (!initialized)
		{
			motion.Init();
			initialized = true;
		}
	}
	public void SetAnimation(Animation anim)
	{
		if (anim != currentMotion)
		{
			currentMotion = anim;
			useMotion = true;
			switch (currentMotion)
			{
			case Animation.Wait:
				WaitAnimation();
				break;
			case Animation.RodCast:
				RodCastAnimation();
				break;
			case Animation.ShitAndFishing:
				ShitAndFishingAnimation();
				break;
			case Animation.RodSetUp:
				RodSetUpAnimation();
				break;
			case Animation.FishFight:
				FishFightAnimation();
				break;
			case Animation.Fishing:
				FishingAnimation();
				break;
			case Animation.RodCancel:
				RodCancelAnimation();
				break;
			}
		}
	}
	public void SetAnimationWithMotion(Animation anim)
	{
		if (anim != currentMotion)
		{
			currentMotion = anim;
			useMotion = true;
			switch (currentMotion)
			{
			case Animation.Wait:
				WaitAnimation();
				break;
			case Animation.RodCast:
				RodCastAnimation();
				break;
			case Animation.ShitAndFishing:
				ShitAndFishingAnimation();
				break;
			case Animation.RodSetUp:
				RodSetUpAnimation();
				break;
			case Animation.FishFight:
				FishFightAnimation();
				break;
			case Animation.Fishing:
				FishingAnimation();
				break;
			case Animation.RodCancel:
				RodCancelAnimation();
				break;
			}
		}
	}
	public void UpdateWalkAnimation(float magnitude)
	{
		bodyParts.LeftArm.SetLocalEulerAnglesX(Mathf.Sin(walkAnimationTime * (float)Math.PI * 2f) * 5f);
		bodyParts.LeftLeg.SetLocalEulerAnglesX(Mathf.Sin(walkAnimationTime * (float)Math.PI * 2f) * 30f);
		bodyParts.RightLeg.SetLocalEulerAnglesX(Mathf.Sin(walkAnimationTime * (float)Math.PI * 2f) * -30f);
		if (!Mathf.Approximately(magnitude, 0f))
		{
			WalkAnimation(magnitude);
		}
		else
		{
			TransitionToIdle();
		}
	}
	private void ShitAndFishingAnimation()
	{
		if (currentMotion == Animation.ShitAndFishing)
		{
			isDuringMotionAnimation = true;
			CharaLeanTweenMotionData.MotionData shitAndFishingMotionData = CharaLeanTweenMotionData.GetShitAndFishingMotionData();
			if (!useMotion)
			{
				shitAndFishingMotionData.motionTime = 0f;
			}
			motion.StartMotion(shitAndFishingMotionData, delegate
			{
				isDuringMotionAnimation = false;
			});
		}
	}
	private void RodCastAnimation()
	{
		if (currentMotion == Animation.RodCast)
		{
			isDuringMotionAnimation = true;
			CharaLeanTweenMotionData.MotionData rodCastMotionData = CharaLeanTweenMotionData.GetRodCastMotionData();
			if (!useMotion)
			{
				rodCastMotionData.motionTime = 0f;
			}
			motion.StartMotion(rodCastMotionData, delegate
			{
				isDuringMotionAnimation = false;
			});
		}
	}
	private void RodSetUpAnimation()
	{
		if (currentMotion == Animation.RodSetUp)
		{
			CharaLeanTweenMotionData.MotionData rodSetUpMotionData = CharaLeanTweenMotionData.GetRodSetUpMotionData();
			if (!useMotion)
			{
				rodSetUpMotionData.motionTime = 0f;
			}
			motion.StartMotion(rodSetUpMotionData);
		}
	}
	private void FishFightAnimation()
	{
		if (currentMotion == Animation.FishFight)
		{
			isDuringMotionAnimation = true;
			CharaLeanTweenMotionData.MotionData fishingFightMotionData = CharaLeanTweenMotionData.GetFishingFightMotionData();
			if (!useMotion)
			{
				fishingFightMotionData.motionTime = 0f;
			}
			motion.StartMotion(fishingFightMotionData, delegate
			{
				isDuringMotionAnimation = false;
			});
		}
	}
	private void FishingAnimation()
	{
		if (currentMotion == Animation.Fishing)
		{
			isDuringMotionAnimation = true;
			CharaLeanTweenMotionData.MotionData fishingMotionData = CharaLeanTweenMotionData.GetFishingMotionData();
			if (!useMotion)
			{
				fishingMotionData.motionTime = 0f;
			}
			motion.StartMotion(fishingMotionData, delegate
			{
				isDuringMotionAnimation = false;
			});
		}
	}
	private void RodCancelAnimation()
	{
		if (currentMotion == Animation.RodCancel)
		{
			isDuringMotionAnimation = true;
			CharaLeanTweenMotionData.MotionData rodCancelMotionData = CharaLeanTweenMotionData.GetRodCancelMotionData();
			if (!useMotion)
			{
				rodCancelMotionData.motionTime = 0f;
			}
			motion.StartMotion(rodCancelMotionData, delegate
			{
				SetAnimationWithMotion(Animation.Wait);
				isDuringMotionAnimation = false;
			});
		}
	}
	private void WaitAnimation()
	{
		CharaLeanTweenMotionData.MotionData fishingWaitMotionData = CharaLeanTweenMotionData.GetFishingWaitMotionData();
		isDuringMotionAnimation = true;
		if (!useMotion)
		{
			fishingWaitMotionData.motionTime = 0f;
		}
		motion.StartMotion(fishingWaitMotionData, delegate
		{
			isDuringMotionAnimation = false;
		});
	}
	private void WalkAnimation(float magnitude)
	{
		float num = walkAnimationTime;
		walkAnimationTime += magnitude * config.WalkAnimationSpeed * Time.deltaTime;
		if (num == 0f)
		{
			audio.PlayWalkSfx();
		}
		audio.PlayWalkSfxWithInterval(walkAnimationTime);
		if (walkAnimationTime > 1f)
		{
			walkAnimationTime = Mathf.Repeat(walkAnimationTime, 1f);
		}
	}
	private void TransitionToIdle()
	{
		if (Mathf.Approximately(walkAnimationTime, 0f) || Mathf.Approximately(walkAnimationTime, 0.5f) || Mathf.Approximately(walkAnimationTime, 1f))
		{
			return;
		}
		float num = walkAnimationTime;
		if (num < 0.25f)
		{
			walkAnimationTime -= Time.deltaTime;
			if (walkAnimationTime < 0f)
			{
				walkAnimationTime = 0f;
			}
		}
		else if (num < 0.5f)
		{
			walkAnimationTime += Time.deltaTime;
			if (walkAnimationTime > 0.5f)
			{
				walkAnimationTime = 0.5f;
			}
		}
		else if (num < 0.75f)
		{
			walkAnimationTime -= Time.deltaTime;
			if (walkAnimationTime < 0.5f)
			{
				walkAnimationTime = 0.5f;
			}
		}
		else
		{
			walkAnimationTime += Time.deltaTime;
			if (walkAnimationTime > 1f)
			{
				walkAnimationTime = 1f;
			}
		}
	}
}
