using UnityEngine;

namespace UnityStandardAssets.Cameras
{
	public class FreeLookCam : PivotBasedCameraRig
	{
		public enum ButtonCtrlType
		{
			Y_and_A,
			L_and_R
		}

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
		[Header("左右ボタンでカメラを回転するかどうか")]
		private bool isCanButtonCtrl = true;

		[SerializeField]
		[Header("左右ボタンのタイプ")]
		private ButtonCtrlType buttonCtrlType;

		[SerializeField]
		[Header("スティック感度")]
		private Vector2 stickSensitivity;

		[SerializeField]
		[Header("ジャイロ感度")]
		private Vector2 sixAxisSensitivity;

		[SerializeField]
		private int playerNo;

		private int npadId;

		private float m_LookAngle;

		private float m_AddLookAngle;

		private float m_TiltAngle = 15f;

		private const float k_LookDistance = 100f;

		private Vector3 m_PivotEulers;

		private Quaternion m_PivotTargetRot;

		private Quaternion m_TransformTargetRot;

		private JoyConManager.AXIS_INPUT defaultInput;

		private JoyConManager.AXIS_INPUT lookInput;

		private float x;

		private float y;

		public bool IsFix
		{
			get;
			set;
		}

		public float X => x;

		public float Y => y;

		protected override void Awake()
		{
			base.Awake();
			Cursor.lockState = (m_LockCursor ? CursorLockMode.Locked : CursorLockMode.None);
			Cursor.visible = !m_LockCursor;
			m_PivotEulers = m_Pivot.rotation.eulerAngles;
			m_PivotTargetRot = m_Pivot.transform.localRotation;
			m_TransformTargetRot = base.transform.localRotation;
			if (SingletonCustom<JoyConManager>.Instance.IsSingleMode())
			{
				npadId = 0;
			}
			else
			{
				npadId = playerNo;
			}
		}

		protected void Update()
		{
			HandleRotationMovement();
			if (m_LockCursor && Input.GetMouseButtonUp(0))
			{
				Cursor.lockState = (m_LockCursor ? CursorLockMode.Locked : CursorLockMode.None);
				Cursor.visible = !m_LockCursor;
			}
		}

		private void OnDisable()
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}

		public void Init(float _lookAngle)
		{
			base.transform.position = m_Target.position;
			m_LookAngle = _lookAngle;
		}

		public float GetLookAngle()
		{
			return m_LookAngle;
		}

		public void SetLookAngle(float _angle)
		{
			m_LookAngle = _angle;
		}

		public void SetAddLookAngle(float _addLookAngle)
		{
			m_AddLookAngle = _addLookAngle;
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
			defaultInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerNo);
			m_TiltAngle = 0f;
		}

		public void UpdateCarRot()
		{
		}

		private void HandleRotationMovement()
		{
			x = (y = 0f);
			if (!(Time.timeScale < float.Epsilon) && !IsFix)
			{
				m_LookAngle += x;
				m_TransformTargetRot = Quaternion.Euler(0f, m_LookAngle + m_AddLookAngle, 0f);
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

		public float GetTiltLerp()
		{
			return Mathf.InverseLerp(0f - m_TiltMin, m_TiltMax, m_TiltAngle);
		}

		public void SetPlayerNo(int _playerNo)
		{
			playerNo = _playerNo;
			if (SingletonCustom<JoyConManager>.Instance.IsSingleMode())
			{
				npadId = 0;
			}
			else
			{
				npadId = playerNo;
			}
		}

		public void SetIsCanButtonCtrl(bool _value)
		{
			isCanButtonCtrl = _value;
		}
	}
}
