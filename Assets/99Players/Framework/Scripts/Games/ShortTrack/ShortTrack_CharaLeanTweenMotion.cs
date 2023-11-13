using System;
using System.Collections;
using UnityEngine;
public class ShortTrack_CharaLeanTweenMotion : MonoBehaviour
{
	public enum BodyPartsList
	{
		HEAD,
		BODY,
		HIP,
		SHOULDER_L,
		SHOULDER_R,
		ARM_L,
		ARM_R,
		LEG_L,
		LEG_R
	}
	[Serializable]
	public struct BodyParts
	{
		[SerializeField]
		[Header("体アンカ\u30fc")]
		public GameObject bodyAnchor;
		public MeshRenderer[] rendererList;
		public Transform Parts(BodyPartsList _parts)
		{
			return rendererList[(int)_parts].transform;
		}
		public Transform Parts(int _parts)
		{
			return rendererList[_parts].transform;
		}
	}
	[Serializable]
	public class MotionLoadData
	{
		[Header("角度モ\u30fcションの前情報")]
		public Vector3 prevEulerAngles = Vector3.zero;
		[Header("角度モ\u30fcションのブレンド角度")]
		public Vector3 blendEulerAngles = Vector3.zero;
		[Header("モ\u30fcションデ\u30fcタ：角度")]
		public Vector3[] eulerAnglesData;
		[Header("モ\u30fcション読み込み番号リスト")]
		public int[] motionLoadNoList = new int[3];
		[Header("モ\u30fcション読み込み前回番号")]
		public int motionLoadPrevNo;
		[Header("モ\u30fcション読み込み完了フラグ")]
		public bool[] isCompleteLoad = new bool[3];
		[Header("角度モ\u30fcションの各角度の回転停止許可フラグ")]
		public bool[] isStopRotateAngle = new bool[3];
		[Header("モ\u30fcション読み込み反転フラグ")]
		public bool isLoadReverse;
		[Header("角度モ\u30fcションブレンド完了フラグ")]
		public bool isBlendAngles;
	}
	[SerializeField]
	[Header("体のパ\u30fcツ")]
	private BodyParts bodyParts;
	private const int BODY_PARTS_NUM = 9;
	private bool[] showLog = new bool[9];
	[NonReorderable]
	private MotionLoadData[] motionLoadDatas = new MotionLoadData[9];
	public bool[] ShowLog
	{
		get
		{
			return showLog;
		}
		set
		{
			showLog = value;
		}
	}
	public void Init()
	{
		for (int i = 0; i < 9; i++)
		{
			motionLoadDatas[i] = new MotionLoadData();
		}
	}
	public void StartMotion(ShortTrack_CharaLeanTweenMotionData.MotionData _motionData, Action callBack = null)
	{
		for (int i = 0; i < motionLoadDatas.Length; i++)
		{
			for (int j = 0; j < motionLoadDatas[i].motionLoadNoList.Length; j++)
			{
				motionLoadDatas[i].motionLoadNoList[j] = 0;
			}
			for (int k = 0; k < motionLoadDatas[i].isCompleteLoad.Length; k++)
			{
				motionLoadDatas[i].isCompleteLoad[k] = false;
			}
			for (int l = 0; l < motionLoadDatas[i].isStopRotateAngle.Length; l++)
			{
				motionLoadDatas[i].isStopRotateAngle[l] = false;
			}
			motionLoadDatas[i].isLoadReverse = false;
			motionLoadDatas[i].isBlendAngles = false;
			motionLoadDatas[i].motionLoadPrevNo = 0;
		}
		StartCoroutine(MotionAnimation(_motionData, callBack));
	}
	private IEnumerator MotionAnimation(ShortTrack_CharaLeanTweenMotionData.MotionData _motionData, Action callBack = null)
	{
		if (_motionData.motionTime == 0f)
		{
			LeanTweenMotion_None(BodyPartsList.HEAD, _motionData.pose_HEAD.pose_Pos, _motionData.pose_HEAD.pose_Angle, _motionData.pose_HEAD.pose_Scale);
			LeanTweenMotion_None(BodyPartsList.BODY, _motionData.pose_BODY.pose_Pos, _motionData.pose_BODY.pose_Angle, _motionData.pose_BODY.pose_Scale);
			LeanTweenMotion_None(BodyPartsList.HIP, _motionData.pose_HIP.pose_Pos, _motionData.pose_HIP.pose_Angle, _motionData.pose_HIP.pose_Scale);
			LeanTweenMotion_None(BodyPartsList.SHOULDER_L, _motionData.pose_SHOULDER_L.pose_Pos, _motionData.pose_SHOULDER_L.pose_Angle, _motionData.pose_SHOULDER_L.pose_Scale);
			LeanTweenMotion_None(BodyPartsList.SHOULDER_R, _motionData.pose_SHOULDER_R.pose_Pos, _motionData.pose_SHOULDER_R.pose_Angle, _motionData.pose_SHOULDER_R.pose_Scale);
			LeanTweenMotion_None(BodyPartsList.ARM_L, _motionData.pose_ARM_L.pose_Pos, _motionData.pose_ARM_L.pose_Angle, _motionData.pose_ARM_L.pose_Scale);
			LeanTweenMotion_None(BodyPartsList.ARM_R, _motionData.pose_ARM_R.pose_Pos, _motionData.pose_ARM_R.pose_Angle, _motionData.pose_ARM_R.pose_Scale);
			LeanTweenMotion_None(BodyPartsList.LEG_L, _motionData.pose_LEG_L.pose_Pos, _motionData.pose_LEG_L.pose_Angle, _motionData.pose_LEG_L.pose_Scale);
			LeanTweenMotion_None(BodyPartsList.LEG_R, _motionData.pose_LEG_R.pose_Pos, _motionData.pose_LEG_R.pose_Angle, _motionData.pose_LEG_R.pose_Scale);
		}
		else
		{
			LeanTweenMotion(BodyPartsList.HEAD, _motionData.pose_HEAD.Loop, _motionData.motionTime, _motionData.pose_HEAD.pose_Pos, _motionData.pose_HEAD.pose_Angle, _motionData.pose_HEAD.pose_Scale);
			LeanTweenMotion(BodyPartsList.BODY, _motionData.pose_BODY.Loop, _motionData.motionTime, _motionData.pose_BODY.pose_Pos, _motionData.pose_BODY.pose_Angle, _motionData.pose_BODY.pose_Scale);
			LeanTweenMotion(BodyPartsList.HIP, _motionData.pose_HIP.Loop, _motionData.motionTime, _motionData.pose_HIP.pose_Pos, _motionData.pose_HIP.pose_Angle, _motionData.pose_HIP.pose_Scale);
			LeanTweenMotion(BodyPartsList.SHOULDER_L, _motionData.pose_SHOULDER_L.Loop, _motionData.motionTime, _motionData.pose_SHOULDER_L.pose_Pos, _motionData.pose_SHOULDER_L.pose_Angle, _motionData.pose_SHOULDER_L.pose_Scale);
			LeanTweenMotion(BodyPartsList.SHOULDER_R, _motionData.pose_SHOULDER_R.Loop, _motionData.motionTime, _motionData.pose_SHOULDER_R.pose_Pos, _motionData.pose_SHOULDER_R.pose_Angle, _motionData.pose_SHOULDER_R.pose_Scale);
			LeanTweenMotion(BodyPartsList.ARM_L, _motionData.pose_ARM_L.Loop, _motionData.motionTime, _motionData.pose_ARM_L.pose_Pos, _motionData.pose_ARM_L.pose_Angle, _motionData.pose_ARM_L.pose_Scale);
			LeanTweenMotion(BodyPartsList.ARM_R, _motionData.pose_ARM_R.Loop, _motionData.motionTime, _motionData.pose_ARM_R.pose_Pos, _motionData.pose_ARM_R.pose_Angle, _motionData.pose_ARM_R.pose_Scale);
			LeanTweenMotion(BodyPartsList.LEG_L, _motionData.pose_LEG_L.Loop, _motionData.motionTime, _motionData.pose_LEG_L.pose_Pos, _motionData.pose_LEG_L.pose_Angle, _motionData.pose_LEG_L.pose_Scale);
			LeanTweenMotion(BodyPartsList.LEG_R, _motionData.pose_LEG_R.Loop, _motionData.motionTime, _motionData.pose_LEG_R.pose_Pos, _motionData.pose_LEG_R.pose_Angle, _motionData.pose_LEG_R.pose_Scale);
		}
		yield return new WaitUntil(() => IsAllMotionLoadComplete());
		callBack?.Invoke();
	}
	private void LeanTweenMotion_None(BodyPartsList _partsType, Vector3[] _posePos, Vector3[] _poseAngle, Vector3[] _poseScale)
	{
		LeanTween.cancel(bodyParts.Parts(_partsType).gameObject);
		bodyParts.Parts(_partsType).transform.localPosition = _posePos[_posePos.Length - 1];
		bodyParts.Parts(_partsType).transform.localEulerAngles = _poseAngle[_poseAngle.Length - 1];
		bodyParts.Parts(_partsType).transform.localScale = _poseScale[_poseScale.Length - 1];
		for (int i = 0; i < motionLoadDatas[(int)_partsType].isCompleteLoad.Length; i++)
		{
			motionLoadDatas[(int)_partsType].isCompleteLoad[i] = true;
		}
		motionLoadDatas[(int)_partsType].isBlendAngles = true;
	}
	private void LeanTweenMotion(BodyPartsList _partsType, bool _isPingPong, float _motionTime, Vector3[] _posePos, Vector3[] _poseAngle, Vector3[] _poseScale)
	{
		LeanTween.cancel(bodyParts.Parts(_partsType).gameObject);
		if (bodyParts.Parts(_partsType).localPosition != _posePos[motionLoadDatas[(int)_partsType].motionLoadNoList[0]])
		{
			if (_isPingPong)
			{
				LeanTweenMotion_PingPong_Pos(_partsType, _motionTime, _posePos);
			}
			else
			{
				LeanTweenMotion_Once_Pos(_partsType, _motionTime, _posePos);
			}
		}
		else
		{
			motionLoadDatas[(int)_partsType].isCompleteLoad[0] = true;
		}
		if (bodyParts.Parts(_partsType).localEulerAngles != _poseAngle[motionLoadDatas[(int)_partsType].motionLoadNoList[1]])
		{
			motionLoadDatas[(int)_partsType].eulerAnglesData = _poseAngle;
			motionLoadDatas[(int)_partsType].prevEulerAngles = bodyParts.Parts(_partsType).localEulerAngles;
			motionLoadDatas[(int)_partsType].isStopRotateAngle[0] = true;
			motionLoadDatas[(int)_partsType].isStopRotateAngle[1] = true;
			motionLoadDatas[(int)_partsType].isStopRotateAngle[2] = true;
			for (int i = 0; i < motionLoadDatas[(int)_partsType].eulerAnglesData.Length; i++)
			{
				if (motionLoadDatas[(int)_partsType].eulerAnglesData[i].x != 0f || motionLoadDatas[(int)_partsType].prevEulerAngles.x != 0f)
				{
					motionLoadDatas[(int)_partsType].isStopRotateAngle[0] = false;
				}
				if (motionLoadDatas[(int)_partsType].eulerAnglesData[i].y != 0f || motionLoadDatas[(int)_partsType].prevEulerAngles.y != 0f)
				{
					motionLoadDatas[(int)_partsType].isStopRotateAngle[1] = false;
				}
				if (motionLoadDatas[(int)_partsType].eulerAnglesData[i].z != 0f || motionLoadDatas[(int)_partsType].prevEulerAngles.z != 0f)
				{
					motionLoadDatas[(int)_partsType].isStopRotateAngle[2] = false;
				}
			}
			if (_isPingPong)
			{
				LeanTweenMotion_PingPong_Angle(_partsType, _motionTime);
			}
			else
			{
				LeanTweenMotion_Once_Angle(_partsType, _motionTime);
			}
		}
		else
		{
			motionLoadDatas[(int)_partsType].isCompleteLoad[1] = true;
		}
		if (bodyParts.Parts(_partsType).localScale != _poseScale[motionLoadDatas[(int)_partsType].motionLoadNoList[2]])
		{
			if (_isPingPong)
			{
				LeanTweenMotion_PingPong_Scale(_partsType, _motionTime, _poseScale);
			}
			else
			{
				LeanTweenMotion_Once_Scale(_partsType, _motionTime, _poseScale);
			}
		}
		else
		{
			motionLoadDatas[(int)_partsType].isCompleteLoad[2] = true;
		}
	}
	private void LeanTweenMotion_PingPong_Pos(BodyPartsList _partsType, float _motionTime, Vector3[] _posePos)
	{
		LeanTween.moveLocal(bodyParts.Parts(_partsType).gameObject, _posePos[motionLoadDatas[(int)_partsType].motionLoadNoList[0]], _motionTime).setOnComplete((Action)delegate
		{
			if (_posePos.Length > 1)
			{
				if (!motionLoadDatas[(int)_partsType].isLoadReverse)
				{
					if (motionLoadDatas[(int)_partsType].motionLoadNoList[0] + 1 == _posePos.Length)
					{
						motionLoadDatas[(int)_partsType].motionLoadNoList[0]--;
						motionLoadDatas[(int)_partsType].isLoadReverse = true;
					}
					else
					{
						motionLoadDatas[(int)_partsType].motionLoadNoList[0]++;
					}
				}
				else if (motionLoadDatas[(int)_partsType].motionLoadNoList[0] - 1 < 0)
				{
					motionLoadDatas[(int)_partsType].motionLoadNoList[0]++;
					motionLoadDatas[(int)_partsType].isLoadReverse = false;
				}
				else
				{
					motionLoadDatas[(int)_partsType].motionLoadNoList[0]--;
				}
				LeanTweenMotion_PingPong_Pos(_partsType, _motionTime, _posePos);
			}
		});
	}
	private void LeanTweenMotion_PingPong_Angle(BodyPartsList _partsType, float _motionTime)
	{
		motionLoadDatas[(int)_partsType].blendEulerAngles = motionLoadDatas[(int)_partsType].eulerAnglesData[motionLoadDatas[(int)_partsType].motionLoadNoList[1]];
		SetRotateBlend_X(_partsType);
		SetRotateBlend_Y(_partsType);
		SetRotateBlend_Z(_partsType);
		RotateBlend(_partsType, _motionTime, delegate
		{
			if (motionLoadDatas[(int)_partsType].eulerAnglesData.Length > 1)
			{
				motionLoadDatas[(int)_partsType].motionLoadPrevNo = motionLoadDatas[(int)_partsType].motionLoadNoList[1];
				if (!motionLoadDatas[(int)_partsType].isLoadReverse)
				{
					if (motionLoadDatas[(int)_partsType].motionLoadNoList[1] + 1 == motionLoadDatas[(int)_partsType].eulerAnglesData.Length)
					{
						motionLoadDatas[(int)_partsType].motionLoadNoList[1]--;
						motionLoadDatas[(int)_partsType].isLoadReverse = true;
					}
					else
					{
						motionLoadDatas[(int)_partsType].motionLoadNoList[1]++;
					}
				}
				else if (motionLoadDatas[(int)_partsType].motionLoadNoList[1] - 1 < 0)
				{
					motionLoadDatas[(int)_partsType].motionLoadNoList[1]++;
					motionLoadDatas[(int)_partsType].isLoadReverse = false;
				}
				else
				{
					motionLoadDatas[(int)_partsType].motionLoadNoList[1]--;
				}
			}
			motionLoadDatas[(int)_partsType].prevEulerAngles = bodyParts.Parts(_partsType).localEulerAngles;
			LeanTweenMotion_PingPong_Angle(_partsType, _motionTime);
		});
	}
	private void LeanTweenMotion_PingPong_Scale(BodyPartsList _partsType, float _motionTime, Vector3[] _poseScale)
	{
		LeanTween.scale(bodyParts.Parts(_partsType).gameObject, _poseScale[motionLoadDatas[(int)_partsType].motionLoadNoList[2]], _motionTime).setOnComplete((Action)delegate
		{
			if (_poseScale.Length > 1)
			{
				if (!motionLoadDatas[(int)_partsType].isLoadReverse)
				{
					if (motionLoadDatas[(int)_partsType].motionLoadNoList[2] + 1 == _poseScale.Length)
					{
						motionLoadDatas[(int)_partsType].motionLoadNoList[2]--;
						motionLoadDatas[(int)_partsType].isLoadReverse = true;
					}
					else
					{
						motionLoadDatas[(int)_partsType].motionLoadNoList[2]++;
					}
				}
				else if (motionLoadDatas[(int)_partsType].motionLoadNoList[2] - 1 < 0)
				{
					motionLoadDatas[(int)_partsType].motionLoadNoList[2]++;
					motionLoadDatas[(int)_partsType].isLoadReverse = false;
				}
				else
				{
					motionLoadDatas[(int)_partsType].motionLoadNoList[2]--;
				}
				LeanTweenMotion_PingPong_Scale(_partsType, _motionTime, _poseScale);
			}
		});
	}
	private void LeanTweenMotion_Once_Pos(BodyPartsList _partsType, float _motionTime, Vector3[] _posePos)
	{
		LeanTween.moveLocal(bodyParts.Parts(_partsType).gameObject, _posePos[motionLoadDatas[(int)_partsType].motionLoadNoList[0]], _motionTime).setOnComplete((Action)delegate
		{
			if (motionLoadDatas[(int)_partsType].motionLoadNoList[0] + 1 == _posePos.Length)
			{
				motionLoadDatas[(int)_partsType].isCompleteLoad[0] = true;
			}
			else
			{
				motionLoadDatas[(int)_partsType].motionLoadNoList[0]++;
				LeanTweenMotion_Once_Pos(_partsType, _motionTime, _posePos);
			}
		});
	}
	private void LeanTweenMotion_Once_Angle(BodyPartsList _partsType, float _motionTime)
	{
		motionLoadDatas[(int)_partsType].blendEulerAngles = motionLoadDatas[(int)_partsType].eulerAnglesData[motionLoadDatas[(int)_partsType].motionLoadNoList[1]];
		SetRotateBlend_X(_partsType);
		SetRotateBlend_Y(_partsType);
		SetRotateBlend_Z(_partsType);
		RotateBlend(_partsType, _motionTime, delegate
		{
			motionLoadDatas[(int)_partsType].motionLoadPrevNo = motionLoadDatas[(int)_partsType].motionLoadNoList[1];
			motionLoadDatas[(int)_partsType].prevEulerAngles = bodyParts.Parts(_partsType).localEulerAngles;
			if (motionLoadDatas[(int)_partsType].motionLoadNoList[1] + 1 == motionLoadDatas[(int)_partsType].eulerAnglesData.Length)
			{
				bodyParts.Parts(_partsType).localEulerAngles = motionLoadDatas[(int)_partsType].eulerAnglesData[motionLoadDatas[(int)_partsType].motionLoadNoList[1]];
				motionLoadDatas[(int)_partsType].isCompleteLoad[1] = true;
			}
			else
			{
				motionLoadDatas[(int)_partsType].motionLoadNoList[1]++;
				LeanTweenMotion_Once_Angle(_partsType, _motionTime);
			}
		});
	}
	private void LeanTweenMotion_Once_Scale(BodyPartsList _partsType, float _motionTime, Vector3[] _poseScale)
	{
		LeanTween.scale(bodyParts.Parts(_partsType).gameObject, _poseScale[motionLoadDatas[(int)_partsType].motionLoadNoList[2]], _motionTime).setOnComplete((Action)delegate
		{
			if (motionLoadDatas[(int)_partsType].motionLoadNoList[2] + 1 == _poseScale.Length)
			{
				motionLoadDatas[(int)_partsType].isCompleteLoad[2] = true;
			}
			else
			{
				motionLoadDatas[(int)_partsType].motionLoadNoList[2]++;
				LeanTweenMotion_Once_Scale(_partsType, _motionTime, _poseScale);
			}
		});
	}
	private void RotateBlend(BodyPartsList _partsType, float _motionTime, Action _nextMethod)
	{
		if (showLog[(int)_partsType])
		{
			string str = _partsType.ToString();
			Vector3 prevEulerAngles = motionLoadDatas[(int)_partsType].prevEulerAngles;
			UnityEngine.Debug.Log("[" + str + "] 【ブレンド処理】元座標 -> " + prevEulerAngles.ToString());
			string str2 = _partsType.ToString();
			prevEulerAngles = motionLoadDatas[(int)_partsType].blendEulerAngles;
			UnityEngine.Debug.Log("[" + str2 + "] 【ブレンド処理】指定座標 -> " + prevEulerAngles.ToString());
			UnityEngine.Debug.Log("---------------------------------------------------------------------------------------------------");
		}
		LeanTween.value(bodyParts.Parts(_partsType).gameObject, motionLoadDatas[(int)_partsType].prevEulerAngles, motionLoadDatas[(int)_partsType].blendEulerAngles, _motionTime).setOnUpdate(delegate(Vector3 val)
		{
			RotateQuaternion(_partsType, val);
		}).setOnComplete((Action)delegate
		{
			_nextMethod();
		});
	}
	private void SetRotateBlend_X(BodyPartsList _partsType)
	{
		Quaternion quaternion = Quaternion.Euler(motionLoadDatas[(int)_partsType].prevEulerAngles);
		Quaternion quaternion2 = Quaternion.Euler(motionLoadDatas[(int)_partsType].eulerAnglesData[motionLoadDatas[(int)_partsType].motionLoadNoList[1]]);
		if (showLog[(int)_partsType])
		{
			UnityEngine.Debug.Log("[" + _partsType.ToString() + "]元のX角度[オイラ\u30fc]：" + motionLoadDatas[(int)_partsType].prevEulerAngles.x.ToString());
			UnityEngine.Debug.Log("[" + _partsType.ToString() + "]指定X角度[オイラ\u30fc]：" + motionLoadDatas[(int)_partsType].eulerAnglesData[motionLoadDatas[(int)_partsType].motionLoadNoList[1]].x.ToString());
			UnityEngine.Debug.Log("[" + _partsType.ToString() + "]元のX角度[クォ\u30fcタニオン]：" + quaternion.eulerAngles.ToString());
			UnityEngine.Debug.Log("[" + _partsType.ToString() + "]指定X角度[クォ\u30fcタニオン]：" + quaternion2.eulerAngles.ToString());
		}
		if (motionLoadDatas[(int)_partsType].isStopRotateAngle[0])
		{
			motionLoadDatas[(int)_partsType].blendEulerAngles.x = motionLoadDatas[(int)_partsType].prevEulerAngles.x;
			if (showLog[(int)_partsType])
			{
				UnityEngine.Debug.Log("[" + _partsType.ToString() + "]のX軸は回転が許可されていません");
			}
			return;
		}
		bool flag = quaternion2.eulerAngles.x > 180f;
		bool flag2 = false;
		float num = 0f;
		if (quaternion.eulerAngles.x == quaternion2.eulerAngles.x)
		{
			if (quaternion2.eulerAngles.y != 180f || quaternion2.eulerAngles.z != 180f)
			{
				if (showLog[(int)_partsType])
				{
					UnityEngine.Debug.Log("X軸が同じ角度でジンバルロック判定をしない角度です");
				}
				motionLoadDatas[(int)_partsType].blendEulerAngles.x = motionLoadDatas[(int)_partsType].eulerAnglesData[motionLoadDatas[(int)_partsType].motionLoadPrevNo].x;
				return;
			}
			if (showLog[(int)_partsType])
			{
				UnityEngine.Debug.Log("指定した角度はジンバルロック判定した角度です");
			}
			flag2 = true;
		}
		num = ((!(quaternion.eulerAngles.x > 180f)) ? (quaternion.eulerAngles.x + 180f) : (quaternion.eulerAngles.x - 180f));
		if (flag)
		{
			switch (_partsType)
			{
			case BodyPartsList.HEAD:
			case BodyPartsList.BODY:
			case BodyPartsList.HIP:
				if (quaternion2.eulerAngles.x > num)
				{
					if (quaternion.eulerAngles.x < 180f)
					{
						motionLoadDatas[(int)_partsType].blendEulerAngles.x = quaternion2.eulerAngles.x - 360f;
					}
					else
					{
						motionLoadDatas[(int)_partsType].blendEulerAngles.x = quaternion2.eulerAngles.x;
					}
					if (showLog[(int)_partsType])
					{
						UnityEngine.Debug.Log("[" + _partsType.ToString() + "]：X軸：マイナス方向に回転 -> " + motionLoadDatas[(int)_partsType].blendEulerAngles.x.ToString());
					}
				}
				else
				{
					motionLoadDatas[(int)_partsType].blendEulerAngles.x = quaternion2.eulerAngles.x;
					if (showLog[(int)_partsType])
					{
						UnityEngine.Debug.Log("[" + _partsType.ToString() + "]：X軸：プラス方向に回転 -> " + motionLoadDatas[(int)_partsType].blendEulerAngles.x.ToString());
					}
				}
				break;
			case BodyPartsList.SHOULDER_L:
			case BodyPartsList.ARM_L:
			case BodyPartsList.LEG_L:
				if (quaternion2.eulerAngles.x > num)
				{
					if (quaternion.eulerAngles.x < 180f)
					{
						motionLoadDatas[(int)_partsType].blendEulerAngles.x = quaternion2.eulerAngles.x - 360f;
					}
					else if (flag2)
					{
						motionLoadDatas[(int)_partsType].blendEulerAngles.x = motionLoadDatas[(int)_partsType].eulerAnglesData[motionLoadDatas[(int)_partsType].motionLoadNoList[1]].x;
					}
					else
					{
						motionLoadDatas[(int)_partsType].blendEulerAngles.x = quaternion2.eulerAngles.x;
					}
					if (showLog[(int)_partsType])
					{
						UnityEngine.Debug.Log("[" + _partsType.ToString() + "]：X軸：マイナス方向に回転 -> " + motionLoadDatas[(int)_partsType].blendEulerAngles.x.ToString());
					}
				}
				else
				{
					if (quaternion.eulerAngles.x < 180f)
					{
						motionLoadDatas[(int)_partsType].blendEulerAngles.x = quaternion.eulerAngles.x + quaternion2.eulerAngles.x;
					}
					else
					{
						motionLoadDatas[(int)_partsType].blendEulerAngles.x = quaternion2.eulerAngles.x;
					}
					if (showLog[(int)_partsType])
					{
						UnityEngine.Debug.Log("[" + _partsType.ToString() + "]：X軸：プラス方向に回転 -> " + motionLoadDatas[(int)_partsType].blendEulerAngles.x.ToString());
					}
				}
				break;
			case BodyPartsList.SHOULDER_R:
			case BodyPartsList.ARM_R:
			case BodyPartsList.LEG_R:
				if (quaternion2.eulerAngles.x > num)
				{
					if (quaternion.eulerAngles.x < 180f)
					{
						motionLoadDatas[(int)_partsType].blendEulerAngles.x = quaternion2.eulerAngles.x - 360f;
					}
					else if (flag2)
					{
						motionLoadDatas[(int)_partsType].blendEulerAngles.x = motionLoadDatas[(int)_partsType].eulerAnglesData[motionLoadDatas[(int)_partsType].motionLoadNoList[1]].x;
					}
					else
					{
						motionLoadDatas[(int)_partsType].blendEulerAngles.x = quaternion2.eulerAngles.x;
					}
					if (showLog[(int)_partsType])
					{
						UnityEngine.Debug.Log("[" + _partsType.ToString() + "]：X軸：マイナス方向に回転 -> " + motionLoadDatas[(int)_partsType].blendEulerAngles.x.ToString());
					}
				}
				else
				{
					if (quaternion.eulerAngles.x < 180f)
					{
						motionLoadDatas[(int)_partsType].blendEulerAngles.x = quaternion.eulerAngles.x + quaternion2.eulerAngles.x;
					}
					else
					{
						motionLoadDatas[(int)_partsType].blendEulerAngles.x = quaternion2.eulerAngles.x;
					}
					if (showLog[(int)_partsType])
					{
						UnityEngine.Debug.Log("[" + _partsType.ToString() + "]：X軸：プラス方向に回転 -> " + motionLoadDatas[(int)_partsType].blendEulerAngles.x.ToString());
					}
				}
				break;
			}
			return;
		}
		switch (_partsType)
		{
		case BodyPartsList.HEAD:
		case BodyPartsList.BODY:
		case BodyPartsList.HIP:
			if (quaternion2.eulerAngles.x > num)
			{
				if (quaternion.eulerAngles.x < 180f)
				{
					motionLoadDatas[(int)_partsType].blendEulerAngles.x = quaternion2.eulerAngles.x - 360f;
				}
				else
				{
					motionLoadDatas[(int)_partsType].blendEulerAngles.x = quaternion2.eulerAngles.x;
				}
				if (showLog[(int)_partsType])
				{
					UnityEngine.Debug.Log("[" + _partsType.ToString() + "]：X軸：マイナス方向に回転 -> " + motionLoadDatas[(int)_partsType].blendEulerAngles.x.ToString());
				}
			}
			else
			{
				if (quaternion.eulerAngles.x < 180f)
				{
					motionLoadDatas[(int)_partsType].blendEulerAngles.x = quaternion2.eulerAngles.x;
				}
				else
				{
					motionLoadDatas[(int)_partsType].blendEulerAngles.x = quaternion2.eulerAngles.x + 360f;
				}
				if (showLog[(int)_partsType])
				{
					UnityEngine.Debug.Log("[" + _partsType.ToString() + "]：X軸：プラス方向に回転 -> " + motionLoadDatas[(int)_partsType].blendEulerAngles.x.ToString());
				}
			}
			break;
		case BodyPartsList.SHOULDER_L:
		case BodyPartsList.ARM_L:
		case BodyPartsList.LEG_L:
			if (quaternion2.eulerAngles.x > num)
			{
				motionLoadDatas[(int)_partsType].blendEulerAngles.x = quaternion2.eulerAngles.x;
				if (showLog[(int)_partsType])
				{
					UnityEngine.Debug.Log("[" + _partsType.ToString() + "]：X軸：マイナス方向に回転 -> " + motionLoadDatas[(int)_partsType].blendEulerAngles.x.ToString());
				}
				break;
			}
			if (quaternion.eulerAngles.x < 180f)
			{
				motionLoadDatas[(int)_partsType].blendEulerAngles.x = quaternion2.eulerAngles.x;
			}
			else
			{
				motionLoadDatas[(int)_partsType].blendEulerAngles.x = 360f + quaternion2.eulerAngles.x;
			}
			if (showLog[(int)_partsType])
			{
				UnityEngine.Debug.Log("[" + _partsType.ToString() + "]：X軸：プラス方向に回転 -> " + motionLoadDatas[(int)_partsType].blendEulerAngles.x.ToString());
			}
			break;
		case BodyPartsList.SHOULDER_R:
		case BodyPartsList.ARM_R:
		case BodyPartsList.LEG_R:
			if (quaternion2.eulerAngles.x > num)
			{
				motionLoadDatas[(int)_partsType].blendEulerAngles.x = quaternion2.eulerAngles.x;
				if (showLog[(int)_partsType])
				{
					UnityEngine.Debug.Log("[" + _partsType.ToString() + "]：X軸：マイナス方向に回転 -> " + motionLoadDatas[(int)_partsType].blendEulerAngles.x.ToString());
				}
				break;
			}
			if (quaternion.eulerAngles.x < 180f)
			{
				motionLoadDatas[(int)_partsType].blendEulerAngles.x = quaternion2.eulerAngles.x;
			}
			else
			{
				motionLoadDatas[(int)_partsType].blendEulerAngles.x = 360f + quaternion2.eulerAngles.x;
			}
			if (showLog[(int)_partsType])
			{
				UnityEngine.Debug.Log("[" + _partsType.ToString() + "]：X軸：プラス方向に回転 -> " + motionLoadDatas[(int)_partsType].blendEulerAngles.x.ToString());
			}
			break;
		}
	}
	private void SetRotateBlend_Y(BodyPartsList _partsType)
	{
		Quaternion quaternion = Quaternion.Euler(motionLoadDatas[(int)_partsType].prevEulerAngles);
		Quaternion quaternion2 = Quaternion.Euler(motionLoadDatas[(int)_partsType].eulerAnglesData[motionLoadDatas[(int)_partsType].motionLoadNoList[1]]);
		if (showLog[(int)_partsType])
		{
			UnityEngine.Debug.Log("[" + _partsType.ToString() + "]元のY角度：" + motionLoadDatas[(int)_partsType].prevEulerAngles.y.ToString());
			UnityEngine.Debug.Log("[" + _partsType.ToString() + "]指定Y角度：" + motionLoadDatas[(int)_partsType].eulerAnglesData[motionLoadDatas[(int)_partsType].motionLoadNoList[1]].y.ToString());
		}
		if (motionLoadDatas[(int)_partsType].isStopRotateAngle[1])
		{
			motionLoadDatas[(int)_partsType].blendEulerAngles.y = motionLoadDatas[(int)_partsType].prevEulerAngles.y;
			if (showLog[(int)_partsType])
			{
				UnityEngine.Debug.Log("[" + _partsType.ToString() + "]のY軸は回転が許可されていません");
			}
			return;
		}
		bool flag = quaternion2.eulerAngles.y > 180f;
		float num = 0f;
		if (motionLoadDatas[(int)_partsType].prevEulerAngles.y == motionLoadDatas[(int)_partsType].eulerAnglesData[motionLoadDatas[(int)_partsType].motionLoadNoList[1]].y)
		{
			if (showLog[(int)_partsType])
			{
				UnityEngine.Debug.Log("Y軸が同じ角度です");
			}
			return;
		}
		num = ((!(quaternion.eulerAngles.y > 180f)) ? (quaternion.eulerAngles.y + 180f) : (quaternion.eulerAngles.y - 180f));
		if (flag)
		{
			if ((uint)_partsType > 8u)
			{
				return;
			}
			if (quaternion2.eulerAngles.y > num)
			{
				if (quaternion.eulerAngles.y < 180f)
				{
					motionLoadDatas[(int)_partsType].blendEulerAngles.y = quaternion2.eulerAngles.y - 360f;
				}
				else
				{
					motionLoadDatas[(int)_partsType].blendEulerAngles.y = quaternion2.eulerAngles.y;
				}
				if (showLog[(int)_partsType])
				{
					UnityEngine.Debug.Log("[" + _partsType.ToString() + "]：Y軸：マイナス方向に回転 -> " + motionLoadDatas[(int)_partsType].blendEulerAngles.y.ToString());
				}
			}
			else
			{
				motionLoadDatas[(int)_partsType].blendEulerAngles.y = quaternion2.eulerAngles.y;
				if (showLog[(int)_partsType])
				{
					UnityEngine.Debug.Log("[" + _partsType.ToString() + "]：Y軸：プラス方向に回転 -> " + motionLoadDatas[(int)_partsType].blendEulerAngles.y.ToString());
				}
			}
		}
		else
		{
			if ((uint)_partsType > 8u)
			{
				return;
			}
			if (quaternion2.eulerAngles.y > num)
			{
				if (quaternion.eulerAngles.y < 180f)
				{
					motionLoadDatas[(int)_partsType].blendEulerAngles.y = quaternion2.eulerAngles.y - 360f;
				}
				else
				{
					motionLoadDatas[(int)_partsType].blendEulerAngles.y = quaternion2.eulerAngles.y;
				}
				if (showLog[(int)_partsType])
				{
					UnityEngine.Debug.Log("[" + _partsType.ToString() + "]：Y軸：マイナス方向に回転 -> " + motionLoadDatas[(int)_partsType].blendEulerAngles.y.ToString());
				}
				return;
			}
			if (quaternion.eulerAngles.y < 180f)
			{
				motionLoadDatas[(int)_partsType].blendEulerAngles.y = quaternion2.eulerAngles.y;
			}
			else if (quaternion.eulerAngles.y > 180f)
			{
				motionLoadDatas[(int)_partsType].blendEulerAngles.y = quaternion2.eulerAngles.y + 360f;
			}
			else
			{
				motionLoadDatas[(int)_partsType].blendEulerAngles.y = quaternion2.eulerAngles.y;
			}
			if (showLog[(int)_partsType])
			{
				UnityEngine.Debug.Log("[" + _partsType.ToString() + "]：Y軸：プラス方向に回転 -> " + motionLoadDatas[(int)_partsType].blendEulerAngles.y.ToString());
			}
		}
	}
	private void SetRotateBlend_Z(BodyPartsList _partsType)
	{
		Quaternion quaternion = Quaternion.Euler(motionLoadDatas[(int)_partsType].prevEulerAngles);
		Quaternion quaternion2 = Quaternion.Euler(motionLoadDatas[(int)_partsType].eulerAnglesData[motionLoadDatas[(int)_partsType].motionLoadNoList[1]]);
		if (showLog[(int)_partsType])
		{
			UnityEngine.Debug.Log("[" + _partsType.ToString() + "]元のZ角度：" + motionLoadDatas[(int)_partsType].prevEulerAngles.z.ToString());
			UnityEngine.Debug.Log("[" + _partsType.ToString() + "]指定Z角度：" + motionLoadDatas[(int)_partsType].eulerAnglesData[motionLoadDatas[(int)_partsType].motionLoadNoList[1]].z.ToString());
		}
		if (motionLoadDatas[(int)_partsType].isStopRotateAngle[2])
		{
			motionLoadDatas[(int)_partsType].blendEulerAngles.z = motionLoadDatas[(int)_partsType].prevEulerAngles.z;
			if (showLog[(int)_partsType])
			{
				UnityEngine.Debug.Log("[" + _partsType.ToString() + "]のZ軸は回転が許可されていません");
			}
			return;
		}
		bool flag = quaternion2.eulerAngles.z > 180f;
		float num = 0f;
		if (motionLoadDatas[(int)_partsType].prevEulerAngles.z == motionLoadDatas[(int)_partsType].eulerAnglesData[motionLoadDatas[(int)_partsType].motionLoadNoList[1]].z)
		{
			if (showLog[(int)_partsType])
			{
				UnityEngine.Debug.Log("Z軸が同じ角度です");
			}
			return;
		}
		num = ((!(quaternion.eulerAngles.z > 180f)) ? (quaternion.eulerAngles.z + 180f) : (quaternion.eulerAngles.z - 180f));
		if (flag)
		{
			if ((uint)_partsType > 8u)
			{
				return;
			}
			if (quaternion2.eulerAngles.z > num)
			{
				if (quaternion.eulerAngles.z < 180f)
				{
					motionLoadDatas[(int)_partsType].blendEulerAngles.z = quaternion2.eulerAngles.z - 360f;
				}
				else
				{
					motionLoadDatas[(int)_partsType].blendEulerAngles.z = quaternion2.eulerAngles.z;
				}
				if (showLog[(int)_partsType])
				{
					UnityEngine.Debug.Log("[" + _partsType.ToString() + "]：Z軸：マイナス方向に回転 -> " + motionLoadDatas[(int)_partsType].blendEulerAngles.z.ToString());
				}
			}
			else
			{
				motionLoadDatas[(int)_partsType].blendEulerAngles.z = quaternion2.eulerAngles.z;
				if (showLog[(int)_partsType])
				{
					UnityEngine.Debug.Log("[" + _partsType.ToString() + "]：Z軸：プラス方向に回転 -> " + motionLoadDatas[(int)_partsType].blendEulerAngles.z.ToString());
				}
			}
		}
		else
		{
			if ((uint)_partsType > 8u)
			{
				return;
			}
			if (quaternion2.eulerAngles.z > num)
			{
				if (quaternion.eulerAngles.z < 180f)
				{
					motionLoadDatas[(int)_partsType].blendEulerAngles.z = quaternion2.eulerAngles.z - 360f;
				}
				else
				{
					motionLoadDatas[(int)_partsType].blendEulerAngles.z = quaternion2.eulerAngles.z;
				}
				if (showLog[(int)_partsType])
				{
					UnityEngine.Debug.Log("[" + _partsType.ToString() + "]：Z軸：マイナス方向に回転 -> " + motionLoadDatas[(int)_partsType].blendEulerAngles.z.ToString());
				}
				return;
			}
			if (quaternion.eulerAngles.z < 180f)
			{
				motionLoadDatas[(int)_partsType].blendEulerAngles.z = quaternion2.eulerAngles.z;
			}
			else if (quaternion.eulerAngles.z > 180f)
			{
				motionLoadDatas[(int)_partsType].blendEulerAngles.z = quaternion2.eulerAngles.z + 360f;
			}
			else
			{
				motionLoadDatas[(int)_partsType].blendEulerAngles.z = quaternion2.eulerAngles.z;
			}
			if (showLog[(int)_partsType])
			{
				UnityEngine.Debug.Log("[" + _partsType.ToString() + "]：Z軸：プラス方向に回転 -> " + motionLoadDatas[(int)_partsType].blendEulerAngles.z.ToString());
			}
		}
	}
	private void RotateQuaternion(BodyPartsList _partsType, Vector3 _vec)
	{
		Quaternion quaternion = Quaternion.RotateTowards(Quaternion.Euler(motionLoadDatas[(int)_partsType].prevEulerAngles), Quaternion.Euler(_vec), float.PositiveInfinity);
		bodyParts.Parts(_partsType).localEulerAngles = quaternion.eulerAngles;
	}
	private bool IsAllMotionLoadComplete()
	{
		int num = 0;
		for (int i = 0; i < 9; i++)
		{
			int num2 = 0;
			for (int j = 0; j < motionLoadDatas[i].isCompleteLoad.Length; j++)
			{
				if (motionLoadDatas[i].isCompleteLoad[j])
				{
					num2++;
				}
			}
			if (num2 == motionLoadDatas[i].isCompleteLoad.Length)
			{
				num++;
			}
		}
		return num == 9;
	}
}
