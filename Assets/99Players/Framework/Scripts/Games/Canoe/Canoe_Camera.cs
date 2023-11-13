using UnityEngine;
public class Canoe_Camera : MonoBehaviour
{
	private enum CameraType
	{
		Type_0,
		Type_1,
		Type_2
	}
	private Canoe_Player player;
	private Vector3 diffPos;
	[SerializeField]
	[Header("カメラ")]
	private Camera camera;
	[SerializeField]
	[Header("加速用のエフェクト")]
	private ParticleSystem addSpeedEffect;
	[SerializeField]
	[Header("強制加速用のエフェクト")]
	private ParticleSystem addUseStaminaDashEffect;
	[SerializeField]
	[Header("切り替え用アンカ\u30fc")]
	private Transform[] arrayChangeAnchor;
	private CameraType cameraType;
	[SerializeField]
	[Header("ゴ\u30fcル用アンカ\u30fc")]
	private Transform goalAnchor;
	private bool isCameraChange;
	public void Init(Canoe_Player _player)
	{
		player = _player;
		base.transform.SetPositionX(player.transform.position.x);
		diffPos = base.transform.position - player.transform.position;
		cameraType = CameraType.Type_1;
		camera.transform.localPosition = arrayChangeAnchor[(int)cameraType].localPosition;
		camera.transform.localEulerAngles = arrayChangeAnchor[(int)cameraType].localEulerAngles;
		camera.cullingMask |= 1 << LayerMask.NameToLayer("Collision_Obj_" + (player.GetPlayerNo() + 1).ToString());
		addSpeedEffect.gameObject.layer = LayerMask.NameToLayer("Collision_Obj_" + (player.GetPlayerNo() + 1).ToString());
		addUseStaminaDashEffect.gameObject.layer = LayerMask.NameToLayer("Collision_Obj_" + (player.GetPlayerNo() + 1).ToString());
	}
	public void SetRect(Rect _rect)
	{
		camera.rect = _rect;
	}
	public void FixedUpdateMethod()
	{
		if (player.GetIsGoal())
		{
			goalAnchor.transform.position = player.transform.position;
			goalAnchor.SetLocalPositionY(SingletonCustom<Canoe_CameraManager>.Instance.GetCameraGoalAnchorPosY());
		}
		float addSpeed = player.GetAddSpeed();
		float addSlipStreamSpeed = player.GetAddSlipStreamSpeed();
		base.transform.position = Vector3.Lerp(base.transform.position, player.transform.position + diffPos, (SingletonCustom<Canoe_CameraManager>.Instance.GetCameraMoveSpeed() + addSpeed + addSlipStreamSpeed) * Time.deltaTime);
		Quaternion b = Quaternion.Euler(0f, player.transform.eulerAngles.y, 0f);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, (SingletonCustom<Canoe_CameraManager>.Instance.GetCameraRotSpeed() + addSpeed + addSlipStreamSpeed) * Time.deltaTime);
	}
	public void ChangeCameraType(int _addCameraType)
	{
		LeanTween.cancel(camera.gameObject);
		cameraType += _addCameraType;
		if (cameraType > CameraType.Type_2)
		{
			cameraType = CameraType.Type_0;
		}
		else if (cameraType < CameraType.Type_0)
		{
			cameraType = CameraType.Type_2;
		}
		float cameraChangeTime = SingletonCustom<Canoe_CameraManager>.Instance.GetCameraChangeTime();
		LeanTween.moveLocal(camera.gameObject, arrayChangeAnchor[(int)cameraType].localPosition, cameraChangeTime).setEaseOutQuad();
		LeanTween.rotateLocal(camera.gameObject, arrayChangeAnchor[(int)cameraType].localEulerAngles, cameraChangeTime).setEaseOutQuad();
	}
	public void SetGoalAnchor()
	{
		LeanTween.cancel(camera.gameObject);
		goalAnchor.transform.position = player.transform.position;
		goalAnchor.SetLocalPositionY(SingletonCustom<Canoe_CameraManager>.Instance.GetCameraGoalAnchorPosY());
		camera.transform.parent = goalAnchor;
		float cameraGoalTime = SingletonCustom<Canoe_CameraManager>.Instance.GetCameraGoalTime();
		LeanTween.value(base.gameObject, 0f, 1f, cameraGoalTime).setEaseOutQuad().setOnUpdate(delegate(float _value)
		{
			goalAnchor.SetLocalEulerAnglesY(_value * 180f);
		});
		LeanTween.moveLocal(camera.gameObject, SingletonCustom<Canoe_CameraManager>.Instance.GetCameraGoalCameraPos(), cameraGoalTime).setEaseOutQuad();
		LeanTween.rotateLocal(camera.gameObject, SingletonCustom<Canoe_CameraManager>.Instance.GetCameraGoalCameraRot(), cameraGoalTime).setEaseOutQuad();
	}
	public bool GetIsCameraChange()
	{
		return isCameraChange;
	}
	public void PlayAddSpeedEffect()
	{
		if (!addSpeedEffect.isPlaying)
		{
			addSpeedEffect.Play();
		}
	}
	public void StopAddSpeedEffect()
	{
		if (addSpeedEffect.isPlaying)
		{
			addSpeedEffect.Stop();
		}
	}
	public void PlayAddUseStaminaDashEffect()
	{
		if (!addUseStaminaDashEffect.isPlaying)
		{
			addUseStaminaDashEffect.Play();
		}
	}
	public void StopAddUseStaminaDashEffect()
	{
		if (addUseStaminaDashEffect.isPlaying)
		{
			addUseStaminaDashEffect.Stop();
		}
	}
}
