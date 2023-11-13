using UnityEngine;
namespace BeachSoccer
{
	public class TouchManager : SingletonCustom<TouchManager>
	{
		public enum TOUCH_TYPE
		{
			DOWN,
			MOVE,
			UP,
			MAX
		}
		public GameObject[] mTouchMarker;
		public bool SHOW_TOUCH_MARKER;
		public bool DebugFlg;
		public float GizumoRadius = 50f;
		public static readonly int TOUCH_MAX = 4;
		private Ray mRay;
		private RaycastHit[] mHitObj;
		public Camera myCamera;
		private bool mStartCheckFlg;
		private bool mMultiTouchFlg;
		private Vector3[] mTouchPosPrev;
		private Vector3[,] mTouchPos;
		private Vector3[] mTouchDeltaPos;
		private Vector2 mNativeScale;
		private TOUCH_TYPE[] mTouchType;
		public Vector3 mCalcVector3;
		public Vector2 mCalcVector2;
		private Vector3 mCalcVector3Buf;
		private Vector2 mCalcVector2Buf;
		private float[] mTouchTime;
		private int[] mTouchFingerNo;
		private void Awake()
		{
			mStartCheckFlg = true;
			mMultiTouchFlg = false;
			mTouchPos = new Vector3[TOUCH_MAX, 3];
			mTouchDeltaPos = new Vector3[TOUCH_MAX];
			mTouchPosPrev = new Vector3[TOUCH_MAX];
			mTouchType = new TOUCH_TYPE[TOUCH_MAX];
			mTouchTime = new float[TOUCH_MAX];
			mTouchFingerNo = new int[TOUCH_MAX];
			for (int i = 0; i < TOUCH_MAX; i++)
			{
				mTouchFingerNo[i] = -1;
				mTouchType[i] = TOUCH_TYPE.MAX;
			}
			mHitObj = new RaycastHit[TOUCH_MAX];
			new Vector2(Screen.width, Screen.height);
		}
		private void Update()
		{
			for (int i = 0; i < TOUCH_MAX; i++)
			{
				mTouchType[i] = TOUCH_TYPE.MAX;
			}
			if (Application.isEditor && UnityEngine.Input.touchCount < 2)
			{
				return;
			}
			for (int j = 0; j < TOUCH_MAX; j++)
			{
				if (j >= UnityEngine.Input.touchCount)
				{
					mTouchType[j] = TOUCH_TYPE.MAX;
					continue;
				}
				switch (UnityEngine.Input.GetTouch(j).phase)
				{
				case TouchPhase.Began:
					mTouchType[j] = TOUCH_TYPE.DOWN;
					mTouchTime[j] = 0f;
					break;
				case TouchPhase.Moved:
				case TouchPhase.Stationary:
					mTouchType[j] = TOUCH_TYPE.MOVE;
					mTouchTime[j] += Time.deltaTime;
					break;
				case TouchPhase.Ended:
				case TouchPhase.Canceled:
					mTouchType[j] = TOUCH_TYPE.UP;
					break;
				}
				if (mTouchType[j] != TOUCH_TYPE.MAX)
				{
					TouchPosition(myCamera.ScreenToWorldPoint(UnityEngine.Input.GetTouch(j).position), j);
				}
			}
		}
		private void OnDrawGizmosSelected()
		{
			if (!mStartCheckFlg)
			{
				return;
			}
			Gizmos.color = (mMultiTouchFlg ? Color.green : Color.cyan);
			if (!SHOW_TOUCH_MARKER)
			{
				return;
			}
			for (int i = 0; i < TOUCH_MAX; i++)
			{
				if (CheckTouchMove(i))
				{
					Gizmos.DrawWireSphere(GetTouchPos(TOUCH_TYPE.MOVE, i), GizumoRadius);
				}
			}
		}
		public void TouchPosition(Vector3 _pos, int _fingerNo = 0)
		{
			if (mTouchType[_fingerNo] == TOUCH_TYPE.DOWN)
			{
				mTouchPosPrev[_fingerNo] = _pos;
				mTouchTime[_fingerNo] = 0f;
			}
			else
			{
				mTouchPosPrev[_fingerNo] = mTouchPos[_fingerNo, (int)mTouchType[_fingerNo]];
			}
			mTouchTime[_fingerNo] += Time.deltaTime;
			mTouchPos[_fingerNo, (int)mTouchType[_fingerNo]] = _pos;
			if (mTouchType[_fingerNo] != TOUCH_TYPE.UP)
			{
				mTouchDeltaPos[_fingerNo] = mTouchPos[_fingerNo, (int)mTouchType[_fingerNo]] - mTouchPosPrev[_fingerNo];
			}
			if (mTouchType[_fingerNo] == TOUCH_TYPE.DOWN)
			{
				mTouchPos[_fingerNo, 1] = _pos;
			}
			CheckRayHit(_fingerNo);
		}
		public void SetCamera(Camera _camera)
		{
			myCamera = _camera;
		}
		public bool CheckHitObject(GameObject _checkObj, int _fingerNo = 0)
		{
			if (mTouchType[_fingerNo] == TOUCH_TYPE.MAX)
			{
				return false;
			}
			if (mHitObj[_fingerNo].collider == null)
			{
				return false;
			}
			if (mHitObj[_fingerNo].collider.gameObject == null)
			{
				return false;
			}
			if (mHitObj[_fingerNo].collider.gameObject == _checkObj)
			{
				return true;
			}
			return false;
		}
		public RaycastHit GetHitObj(int _fingerNo = 0)
		{
			return mHitObj[_fingerNo];
		}
		private bool CheckRayHit(int _fingerNo = 0)
		{
			mRay = myCamera.ScreenPointToRay(myCamera.WorldToScreenPoint(mTouchPos[_fingerNo, (int)mTouchType[_fingerNo]]));
			if (Physics.Raycast(mRay, out mHitObj[_fingerNo], float.PositiveInfinity))
			{
				return true;
			}
			return false;
		}
		public bool CheckHitObject(Camera _checkCamera, GameObject _checkObj, int _fingerNo = 0)
		{
			mRay = _checkCamera.ScreenPointToRay(myCamera.WorldToScreenPoint(mTouchPos[_fingerNo, (int)mTouchType[_fingerNo]]));
			if (Physics.Raycast(mRay, out RaycastHit hitInfo, float.PositiveInfinity) && hitInfo.collider == _checkObj.GetComponent<Collider>())
			{
				return true;
			}
			return false;
		}
		public bool TouchFlg(int _fingerNo = 0)
		{
			return mTouchType[_fingerNo] != TOUCH_TYPE.MAX;
		}
		public bool CheckTouchFlg(TOUCH_TYPE _type, int _fingerNo = 0)
		{
			return mTouchType[_fingerNo] == _type;
		}
		public bool CheckTouchDown(int _fingerNo = 0)
		{
			return mTouchType[_fingerNo] == TOUCH_TYPE.DOWN;
		}
		public bool CheckTouchMove(int _fingerNo = 0)
		{
			return mTouchType[_fingerNo] == TOUCH_TYPE.MOVE;
		}
		public bool CheckTouchUp(int _fingerNo = 0)
		{
			return mTouchType[_fingerNo] == TOUCH_TYPE.UP;
		}
		public Vector3 GetTouchPos(TOUCH_TYPE _type = TOUCH_TYPE.MAX, int _fingerNo = 0)
		{
			if (_type == TOUCH_TYPE.MAX)
			{
				_type = mTouchType[_fingerNo];
				if (_type == TOUCH_TYPE.MAX)
				{
					mCalcVector3Buf.x = 0f;
					mCalcVector3Buf.y = 0f;
					mCalcVector3Buf.z = 0f;
					return mCalcVector3Buf;
				}
			}
			return mTouchPos[_fingerNo, (int)_type];
		}
		public Vector3 GetTouchPosPrev(TOUCH_TYPE _type = TOUCH_TYPE.MAX, int _fingerNo = 0)
		{
			if (_type == TOUCH_TYPE.MAX)
			{
				_type = mTouchType[_fingerNo];
				if (_type == TOUCH_TYPE.MAX)
				{
					mCalcVector3Buf.x = 0f;
					mCalcVector3Buf.y = 0f;
					mCalcVector3Buf.z = 0f;
					return mCalcVector3Buf;
				}
			}
			return mTouchPosPrev[_fingerNo];
		}
		public Vector3 GetTouchPosToNativeScale(TOUCH_TYPE _type = TOUCH_TYPE.MAX, int _fingerNo = 0)
		{
			if (_type == TOUCH_TYPE.MAX)
			{
				_type = mTouchType[_fingerNo];
				if (_type == TOUCH_TYPE.MAX)
				{
					mCalcVector3Buf.x = 0f;
					mCalcVector3Buf.y = 0f;
					mCalcVector3Buf.z = 0f;
					return mCalcVector3Buf;
				}
			}
			UnityEngine.Debug.Log("native_x:" + mNativeScale.x.ToString() + " y:" + mNativeScale.y.ToString());
			Vector3 result = mTouchPos[_fingerNo, (int)_type];
			result.x *= mNativeScale.x;
			result.y *= mNativeScale.y;
			return result;
		}
		public int GetTouchCount()
		{
			return Mathf.Min(TOUCH_MAX, UnityEngine.Input.touchCount);
		}
		public bool IsMultiTouch()
		{
			return UnityEngine.Input.touchCount >= 2;
		}
		public Vector3 GetDeltaPos(int _fingerNo = 0)
		{
			return mTouchDeltaPos[_fingerNo];
		}
		public Vector3 GetMouseDeltaPos()
		{
			return mTouchDeltaPos[0];
		}
		public float MoveLength(int _fingerNo = 0)
		{
			return Length(mTouchPos[_fingerNo, 0], mTouchPos[_fingerNo, 1]);
		}
		public Vector2 MoveVec2Length(int _fingerNo = 0)
		{
			mCalcVector2Buf.x = Mathf.Abs(mTouchPos[_fingerNo, 1].x - mTouchPos[_fingerNo, 0].x);
			mCalcVector2Buf.y = Mathf.Abs(mTouchPos[_fingerNo, 1].y - mTouchPos[_fingerNo, 0].y);
			return mCalcVector2Buf;
		}
		public Vector3 MoveDir(int _fingerNo = 0)
		{
			return Vector3.Normalize(mTouchPos[_fingerNo, 1] - mTouchPos[_fingerNo, 0]);
		}
		public float Length(Vector3 _obj, Vector3 _target)
		{
			return Mathf.Sqrt((_target.x - _obj.x) * (_target.x - _obj.x) + (_target.y - _obj.y) * (_target.y - _obj.y) + (_target.z - _obj.z) * (_target.z - _obj.z));
		}
		public float Length(Vector3 _vec)
		{
			return Mathf.Sqrt(_vec.x * _vec.x + _vec.y * _vec.y + _vec.z * _vec.z);
		}
		public Vector2 DirVec2(Vector3 _obj, Vector3 _target)
		{
			if (Length(_obj, _target) == 0f)
			{
				mCalcVector3Buf.x = 0f;
				mCalcVector3Buf.y = 0f;
				mCalcVector3Buf.z = 0f;
				return mCalcVector3Buf;
			}
			mCalcVector3Buf = (_target - _obj) / Length(_obj, _target);
			mCalcVector2Buf.x = mCalcVector3Buf.x;
			mCalcVector2Buf.y = mCalcVector3Buf.y;
			return mCalcVector2Buf;
		}
		public Vector3 DirVec3(Vector3 _obj, Vector3 _target)
		{
			if (Length(_obj, _target) == 0f)
			{
				mCalcVector3Buf.x = 0f;
				mCalcVector3Buf.y = 0f;
				mCalcVector3Buf.z = 0f;
				return mCalcVector3Buf;
			}
			return (_target - _obj) / Length(_obj, _target);
		}
		public float Rot(Vector3 _obj, Vector3 _target)
		{
			mCalcVector2Buf.x = Mathf.Atan2(_target.y - _obj.y, _target.x - _obj.x);
			mCalcVector2Buf.y = mCalcVector2Buf.x * 57.29578f;
			if (mCalcVector2Buf.y < 0f)
			{
				mCalcVector2Buf.y = 360f + mCalcVector2Buf.y;
			}
			return mCalcVector2Buf.y;
		}
		public bool CheckTouchInArea(Vector3 _center, Vector3 _size, int _fingerNo = 0)
		{
			if (GetTouchPos(TOUCH_TYPE.MAX, _fingerNo).x >= _center.x - _size.x * 0.5f && GetTouchPos(TOUCH_TYPE.MAX, _fingerNo).x <= _center.x + _size.x * 0.5f && GetTouchPos(TOUCH_TYPE.MAX, _fingerNo).y >= _center.y - _size.y * 0.5f && GetTouchPos(TOUCH_TYPE.MAX, _fingerNo).y <= _center.y + _size.y * 0.5f)
			{
				return true;
			}
			return false;
		}
		public float GetTouchTime(int _fingerNo = 0)
		{
			return mTouchTime[_fingerNo];
		}
	}
}
