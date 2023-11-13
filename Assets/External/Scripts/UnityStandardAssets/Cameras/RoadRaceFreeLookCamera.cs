using System.Collections;
using UnityEngine;

namespace UnityStandardAssets.Cameras
{
	public class RoadRaceFreeLookCamera : PivotBasedCameraRig
	{
		[SerializeField]
		private float m_MoveSpeed = 1f;

		[Range(0f, 10f)]
		[SerializeField]
		private float m_TurnSpeed = 1.5f;

		[SerializeField]
		private float m_TurnSmoothing;

		[SerializeField]
		private float m_TiltMax = 75f;

		[SerializeField]
		private float m_TiltMin = 45f;

		[SerializeField]
		private bool m_LockCursor;

		[SerializeField]
		private bool m_VerticalAutoReturn;

		[SerializeField]
		[Header("")]
		private Vector2 stickSensitivity;

		[SerializeField]
		[Header("")]
		private Vector2 sixAxisSensitivity;

		[SerializeField]
		private int playerNo;

		private float m_LookAngle = 90f;

		private float m_TiltAngle = 15f;

		private const float k_LookDistance = 100f;

		private Vector3 m_PivotEulers;

		private Quaternion m_PivotTargetRot;

		private Quaternion m_TransformTargetRot;

		private float x;

		private float y;

		private bool lockCamera;

		private bool isRotate;

		private Vector3 cameraRot;

		private float angleY;

		private Camera camera;

		private bool animeFlag;

		private const float moveY = 0.5f;

		public void GoolCamera()
		{
			lockCamera = true;
			cameraRot = base.transform.localEulerAngles;
			cameraRot.y += 180f;
			StartCoroutine(_Sample());
		}

		private IEnumerator _Sample()
		{
			LeanTween.rotateLocal(base.gameObject, new Vector3(0f, base.transform.localEulerAngles.y + 180f, 0f), 4f).setEaseOutBack();
			yield return new WaitForSeconds(4f);
			isRotate = true;
		}

		protected override void Awake()
		{
			base.Awake();
			SingletonCustom<GlobalCameraManager>.Instance.SetCameraGlobalRect(camera);
			m_PivotEulers = m_Pivot.rotation.eulerAngles;
			m_PivotTargetRot = m_Pivot.transform.localRotation;
			m_TransformTargetRot = base.transform.localRotation;
		}

		protected void Update()
		{
			if (!lockCamera)
			{
				HandleRotationMovement();
			}
			else if (isRotate)
			{
				GoalMoveCamera();
			}
		}

		private void GoalMoveCamera()
		{
			cameraRot.y -= 0.5f;
			base.transform.localEulerAngles = cameraRot;
		}

		public void Init(float _lookAngle)
		{
			base.transform.position = m_Target.position;
			m_LookAngle = _lookAngle;
		}

		protected override void FollowTarget(float deltaTime)
		{
			if (!(m_Target == null))
			{
				base.transform.position = Vector3.Lerp(base.transform.position, m_Target.position, deltaTime * m_MoveSpeed);
			}
		}

		public void SetDefaultInput()
		{
			m_TiltAngle = 0f;
		}

		public void UpdateCarRot()
		{
		}

		private void HandleRotationMovement()
		{
			x = (y = 0f);
			if (!(Time.timeScale < float.Epsilon))
			{
				m_LookAngle += x * 1.18f;
				m_TransformTargetRot = Quaternion.Euler(0f, m_LookAngle, 0f);
				if (m_VerticalAutoReturn)
				{
					m_TiltAngle = ((y > 0f) ? Mathf.Lerp(0f, 0f - m_TiltMin, y) : Mathf.Lerp(0f, m_TiltMax, 0f - y));
				}
				else
				{
					m_TiltAngle -= y;
					m_TiltAngle = Mathf.Clamp(m_TiltAngle, 0f - m_TiltMin, m_TiltMax);
				}
				m_PivotTargetRot = Quaternion.Euler(m_TiltAngle, m_PivotEulers.y, m_PivotEulers.z);
				if (m_TurnSmoothing > 0f)
				{
					m_Pivot.localRotation = Quaternion.Slerp(m_Pivot.localRotation, m_PivotTargetRot, m_TurnSmoothing * Time.deltaTime);
					base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, m_TransformTargetRot, m_TurnSmoothing * Time.deltaTime);
				}
				else
				{
					m_Pivot.localRotation = m_PivotTargetRot;
					base.transform.localRotation = m_TransformTargetRot;
				}
			}
		}
	}
}
