using System.Collections;
using UnityEngine;
public class SnowBoard_Camera : MonoBehaviour
{
	[SerializeField]
	[Header("追尾タ\u30fcゲット(SnowBoard)")]
	private GameObject objTarget;
	[SerializeField]
	[Header("対象のSnowBoard_SkiBoard")]
	private SnowBoard_SkiBoard skiBoard;
	[SerializeField]
	[Header("RadialBlurEffect")]
	private RadialBlurEffect radialBlurEffect;
	[SerializeField]
	[Header("AlpineSkiing_RadioControl")]
	public AlpineSkiing_RadioControl radioControl;
	[SerializeField]
	[Header("ゴ\u30fcル時カメラアンカ\u30fc")]
	private Transform goalCameraAnchor;
	private Camera _camera;
	private float cameraFoV;
	private Vector3 targetPos;
	private Rigidbody rb;
	private float moveCompSpeed = 0.275f;
	private float rotCompSpeed = 0.3f;
	private Vector3 relativePos;
	private Quaternion rotation;
	private bool isAddSpeed;
	private bool isAccel;
	private float addSpeed;
	private Vector3 tempPos;
	private void Start()
	{
		if (radialBlurEffect != null)
		{
			radialBlurEffect.blurDegree = 0f;
		}
		_camera = GetComponent<Camera>();
		rb = objTarget.GetComponent<Rigidbody>();
	}
	public void RadialBlur(bool active, float _set = 0.01f, bool autoOFF = false)
	{
		if (active)
		{
			LeanTween.value(base.gameObject, 0f, _set, 0.1f).setOnUpdate(RadialBlurSet);
			radioControl.SpeedLineStart();
			if (autoOFF)
			{
				LeanTween.value(base.gameObject, _set, radialBlurEffect.blurDegree, 0.5f).setOnUpdate(RadialBlurSet).setDelay(0.5f);
				StartCoroutine(SpeedLineEndWait());
			}
		}
		else
		{
			LeanTween.value(base.gameObject, radialBlurEffect.blurDegree, 0f, 0.1f).setOnUpdate(RadialBlurSet);
			radioControl.SpeedLineEnd(0.1f);
		}
	}
	private void RadialBlurSet(float _set)
	{
		radialBlurEffect.blurDegree = _set;
	}
	private void FovSet(float _set)
	{
		_camera.fieldOfView = _set;
	}
	public void Init()
	{
		targetPos = objTarget.transform.position + objTarget.transform.forward * -1.7f;
		targetPos.y += 0.8f;
		base.transform.position = targetPos;
		relativePos = objTarget.transform.position + Vector3.up * 0.35f - base.transform.position;
		base.transform.rotation = Quaternion.LookRotation(relativePos);
		base.transform.SetLocalEulerAnglesX(25f);
	}
	private void FixedUpdate()
	{
		if (skiBoard.processType == SnowBoard_SkiBoard.SkiBoardProcessType.GOAL)
		{
			moveCompSpeed = 0.065f;
			targetPos = goalCameraAnchor.position;
		}
		else if (skiBoard.processType == SnowBoard_SkiBoard.SkiBoardProcessType.SLIDING)
		{
			switch (skiBoard.CameraPos)
			{
			case SnowBoard_SkiBoard.CameraPosType.NEAR:
				tempPos = objTarget.transform.position + rb.velocity.normalized * -1f;
				tempPos.y += 0.3f;
				break;
			case SnowBoard_SkiBoard.CameraPosType.NORMAL:
				tempPos = objTarget.transform.position + rb.velocity.normalized * -1.7f;
				tempPos.y += 0.8f;
				break;
			case SnowBoard_SkiBoard.CameraPosType.DISTANT:
				tempPos = objTarget.transform.position + rb.velocity.normalized * -2.5f;
				tempPos.y += 1f;
				break;
			}
			targetPos = Vector3.Slerp(targetPos, tempPos, 0.175f);
		}
		base.transform.position = Vector3.Slerp(base.transform.position, targetPos, moveCompSpeed);
		relativePos = objTarget.transform.position + Vector3.up * 0.35f - base.transform.position;
		rotation = Quaternion.LookRotation(relativePos);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, rotation, rotCompSpeed);
		if (base.transform.localEulerAngles.x <= 15f)
		{
			base.transform.SetLocalEulerAnglesX(15f);
		}
	}
	private IEnumerator SpeedLineEndWait()
	{
		yield return new WaitForSeconds(0.5f);
		radioControl.SpeedLineEnd();
	}
}
