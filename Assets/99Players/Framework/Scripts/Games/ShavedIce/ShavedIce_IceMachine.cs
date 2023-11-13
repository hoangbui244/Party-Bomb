using UnityEngine;
public class ShavedIce_IceMachine : MonoBehaviour
{
	[SerializeField]
	[Header("かき氷機のハンドル")]
	private GameObject machineHandle;
	[SerializeField]
	[Header("かき氷機の粉砕部分")]
	private GameObject machineCrashParts;
	[SerializeField]
	[Header("かき氷機の粉砕部分のポ\u30fcル")]
	private GameObject machineCrashPole;
	[SerializeField]
	[Header("かき氷機の氷塊")]
	private GameObject machineIceBlock;
	[SerializeField]
	[Header("かき氷機から出る氷の親エフェクト")]
	private ParticleSystem crashIceFXRoot;
	[SerializeField]
	[Header("かき氷機から出るすべての氷のエフェクト")]
	private ParticleSystem[] crashIceFXs;
	[SerializeField]
	[Header("氷のエフェクトの親アンカ\u30fc")]
	private Transform crashIceFXRootAnchor;
	private ParticleSystem.ShapeModule[] crashIceFxScaleModules;
	private const float MACHINE_HANDLE_ANIM_TIME = 3f;
	private float nowTargetMovePosX;
	private const float MOVE_ICE_FX_SPEED = 1.25f;
	private bool isIceFXMoveEnd;
	private const float MACHINE_POLE_MAX_SCALE_Y = 1.7f;
	public bool IsIceFXMoveEnd => isIceFXMoveEnd;
	public void Init()
	{
		crashIceFXRootAnchor.SetLocalPositionX(0f);
		crashIceFxScaleModules = new ParticleSystem.ShapeModule[crashIceFXs.Length];
		for (int i = 0; i < crashIceFXs.Length; i++)
		{
			crashIceFxScaleModules[i] = crashIceFXs[i].shape;
		}
	}
	public void UpdateMethod()
	{
		if (isIceFXMoveEnd)
		{
			return;
		}
		if (crashIceFXRootAnchor.localPosition.x <= nowTargetMovePosX)
		{
			crashIceFXRootAnchor.SetLocalPositionX(crashIceFXRootAnchor.localPosition.x + Time.deltaTime * 1.25f);
			if (crashIceFXRootAnchor.localPosition.x > nowTargetMovePosX)
			{
				isIceFXMoveEnd = true;
				crashIceFXRootAnchor.SetLocalPositionX(nowTargetMovePosX);
			}
		}
		else
		{
			crashIceFXRootAnchor.SetLocalPositionX(crashIceFXRootAnchor.localPosition.x - Time.deltaTime * 1.25f);
			if (crashIceFXRootAnchor.localPosition.x < nowTargetMovePosX)
			{
				isIceFXMoveEnd = true;
				crashIceFXRootAnchor.SetLocalPositionX(nowTargetMovePosX);
			}
		}
	}
	private void LateUpdate()
	{
		if (SingletonCustom<CommonNotificationManager>.Instance.IsPause)
		{
			if (SingletonCustom<AudioManager>.Instance.IsSePlaying("se_ice_machine"))
			{
				SingletonCustom<AudioManager>.Instance.SePause("se_ice_machine", _isPause: true);
			}
		}
		else if (!SingletonCustom<AudioManager>.Instance.IsSePlaying("se_ice_machine"))
		{
			SingletonCustom<AudioManager>.Instance.SePause("se_ice_machine", _isPause: false);
		}
	}
	public void PlayIceMachineAnimation()
	{
		crashIceFXRoot.Play();
		LeanTween.rotateAroundLocal(machineHandle, Vector3.right, 360f, 3f).setLoopClamp();
		LeanTween.rotateAroundLocal(machineCrashParts, Vector3.up, 360f, 3f).setLoopClamp();
		LeanTween.moveLocalY(machineCrashParts, 0f, ShavedIce_Define.GAME_TIME);
		LeanTween.scaleY(machineCrashPole, 1.7f, ShavedIce_Define.GAME_TIME);
		LeanTween.scaleY(machineIceBlock, 0f, ShavedIce_Define.GAME_TIME);
		LeanTween.rotateAroundLocal(machineIceBlock, Vector3.up, 360f, 3f).setLoopClamp();
		SingletonCustom<AudioManager>.Instance.SePlay("se_ice_machine", _loop: true, 0f, 0.4f);
	}
	public void StopIceMachineAnimation()
	{
		LeanTween.cancel(machineHandle);
		LeanTween.cancel(machineCrashParts);
		LeanTween.cancel(machineIceBlock);
		crashIceFXRoot.Stop();
		LeanTween.cancel(crashIceFXRootAnchor.gameObject);
		SingletonCustom<AudioManager>.Instance.SeStop("se_ice_machine");
	}
	public void StopIceFX()
	{
		crashIceFXRoot.Stop();
		LeanTween.cancel(crashIceFXRootAnchor.gameObject);
	}
	public void SetMoveIceFXPosX(float _moveTargetPos, float _effectXScale)
	{
		nowTargetMovePosX = _moveTargetPos;
		for (int i = 0; i < crashIceFxScaleModules.Length; i++)
		{
			crashIceFxScaleModules[i].scale = new Vector3(_effectXScale, crashIceFxScaleModules[i].scale.y, crashIceFxScaleModules[i].scale.z);
		}
		isIceFXMoveEnd = false;
	}
	public void SetCrashIceFXRootAnchorPosY(float _posY)
	{
		crashIceFXRootAnchor.SetLocalPositionY(_posY);
	}
	public Transform GetCrashIceFXRootAnchor()
	{
		return crashIceFXRootAnchor;
	}
}
