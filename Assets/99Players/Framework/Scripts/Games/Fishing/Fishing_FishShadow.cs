using System;
using System.Collections;
using UnityEngine;
public class Fishing_FishShadow : MonoBehaviour
{
	[Serializable]
	public struct FishData
	{
		public int fishId;
		public FishingDefinition.FishType fishType;
		public FishingDefinition.FishSizeType fishSizeType;
	}
	[Serializable]
	private struct FishingAreaData
	{
		public Vector3 areaData_Min;
		public Vector3 areaData_Max;
	}
	public enum FishState
	{
		MOVE_AREA_IN,
		WAIT,
		MOVE_NEXT_POINT,
		BITE_MODE,
		ESCAPE,
		MOVE_AREA_OUT
	}
	[SerializeField]
	[Header("魚影の画像")]
	private SpriteRenderer fishShadowSprite;
	[SerializeField]
	[Header("魚影アニメ\u30fcション")]
	private Animator fishShadowAnimationCtrl;
	[SerializeField]
	[Header("ギズモ表示")]
	private bool isGizmos;
	private int animationHash = Animator.StringToHash("WaitMode");
	private FishData fishData;
	private FishState fishState;
	private const float FISH_SIZE_SCALE_SMALL = 0.15f;
	private const float FISH_SIZE_SCALE_NEDIUM = 0.25f;
	private const float FISH_SIZE_SCALE_LARGE = 0.35f;
	private const float FISH_SHADOW_ANIMATION_DEF_SPEED = 1f;
	private const float FISH_SHADOW_ANIMATION_WAIT_SPEED = 0.25f;
	[SerializeField]
	[Header("RigidBody")]
	private Rigidbody rigid;
	private FishingAreaData fishingAreaData_MoveAround;
	private FishingAreaData fishingAreaData_Out;
	private const float MOVE_SPEED_MAX_NORMAL = 0.2f;
	private const float MOVE_SPEED_MAX_FAST = 0.4f;
	private float moveSpeed = 0.1f;
	private float moveSpeedMax = 0.2f;
	private Vector3 prevPos;
	private Vector3 nowPos;
	private Vector3[] calcVec = new Vector3[2];
	private Vector3 rot;
	private float STOP_CHECK_DISTANCE = 0.01f;
	private Vector3 moveTargetPoint = Vector3.zero;
	private Fishing_RodUki biteUki;
	private bool createInAreaMoveTargetPoint;
	private const float MOVE_WAIT_INTERVAL = 10f;
	private float moveTargetWaitIntervalTime;
	private const float MOVE_NEXT_POINT_INTERVAL = 5f;
	private float moveNextPointIntervalTime;
	private bool isActive;
	private const float FISH_CIRCLE_INTERVAL_SPACE = 0.125f;
	private float distFromToTarget;
	private Vector3 moveVec;
	private float escapeTime;
	private const float ESCAPE_TIME = 3f;
	private const float FISH_SHADOW_FADE_IN_TIME = 0.5f;
	public void Init(int _fishId, Fishing_FishData _fishData, Vector3 _moveAreaMin, Vector3 _moveAreaMax)
	{
		fishData.fishId = _fishId;
		fishData.fishType = _fishData.fishType;
		fishData.fishSizeType = _fishData.fishSizeType;
		switch (fishData.fishSizeType)
		{
		case FishingDefinition.FishSizeType.Small:
			base.transform.SetLocalScale(0.15f, 0.15f, 0.15f);
			break;
		case FishingDefinition.FishSizeType.Medium:
			base.transform.SetLocalScale(0.25f, 0.25f, 0.25f);
			break;
		case FishingDefinition.FishSizeType.Large:
			base.transform.SetLocalScale(0.35f, 0.35f, 0.35f);
			break;
		case FishingDefinition.FishSizeType.Garbage:
			base.transform.SetLocalScale(0.15f, 0.15f, 0.15f);
			break;
		}
		if (fishData.fishType == FishingDefinition.FishType.Iwana)
		{
			moveSpeedMax = 0.4f;
		}
		else
		{
			moveSpeedMax = 0.2f;
		}
		fishingAreaData_MoveAround.areaData_Min = _moveAreaMin;
		fishingAreaData_MoveAround.areaData_Max = _moveAreaMax;
		fishState = FishState.MOVE_AREA_IN;
		isActive = true;
		fishShadowAnimationCtrl.SetBool(animationHash, value: false);
	}
	public void UpdateMethod()
	{
		prevPos = nowPos;
		nowPos = base.transform.localPosition;
		switch (fishState)
		{
		case FishState.MOVE_AREA_IN:
			FishMoveAreaInProcess();
			break;
		case FishState.WAIT:
			FishWaitProcess();
			break;
		case FishState.MOVE_NEXT_POINT:
			FishMoveNextPointProcess();
			break;
		case FishState.BITE_MODE:
			FishBiteModeProcess();
			break;
		case FishState.ESCAPE:
			FishEscapeProcess();
			break;
		}
		moveTargetPoint.y = 0f;
		distFromToTarget = Vector3.Distance(new Vector3(base.transform.position.x, 0f, base.transform.position.z), moveTargetPoint);
	}
	private void FishMoveAreaInProcess()
	{
		if (!createInAreaMoveTargetPoint)
		{
			moveTargetPoint = new Vector3(UnityEngine.Random.Range(fishingAreaData_MoveAround.areaData_Min.x, fishingAreaData_MoveAround.areaData_Max.x), 0f, UnityEngine.Random.Range(fishingAreaData_MoveAround.areaData_Min.z, fishingAreaData_MoveAround.areaData_Max.z));
			createInAreaMoveTargetPoint = true;
		}
		else if (MoveTarget())
		{
			fishState = FishState.WAIT;
			fishShadowAnimationCtrl.SetBool(animationHash, value: true);
		}
		else if (moveNextPointIntervalTime < 0f)
		{
			fishState = FishState.WAIT;
			fishShadowAnimationCtrl.SetBool(animationHash, value: true);
			moveNextPointIntervalTime = 5f;
		}
		else
		{
			moveNextPointIntervalTime -= Time.deltaTime;
		}
	}
	private void FishWaitProcess()
	{
		rigid.velocity = Vector3.zero;
		if (moveTargetWaitIntervalTime < 0f)
		{
			moveTargetPoint = new Vector3(UnityEngine.Random.Range(fishingAreaData_MoveAround.areaData_Min.x, fishingAreaData_MoveAround.areaData_Max.x), 0f, UnityEngine.Random.Range(fishingAreaData_MoveAround.areaData_Min.z, fishingAreaData_MoveAround.areaData_Max.z));
			moveTargetWaitIntervalTime = 10f;
			fishState = FishState.MOVE_NEXT_POINT;
			fishShadowAnimationCtrl.SetBool(animationHash, value: false);
		}
		else
		{
			moveTargetWaitIntervalTime -= Time.deltaTime;
		}
	}
	private void FishMoveNextPointProcess()
	{
		if (MoveTarget())
		{
			fishState = FishState.WAIT;
			fishShadowAnimationCtrl.SetBool(animationHash, value: true);
		}
		else if (moveNextPointIntervalTime < 0f)
		{
			fishState = FishState.WAIT;
			fishShadowAnimationCtrl.SetBool(animationHash, value: true);
			moveNextPointIntervalTime = 5f;
		}
		else
		{
			moveNextPointIntervalTime -= Time.deltaTime;
		}
	}
	private void FishBiteModeProcess()
	{
		moveTargetPoint = biteUki.GetUkiAnchor().position + GetCirclePosition(GetFromToAngle(biteUki.GetUkiAnchor().position, base.transform.position), 0.125f);
		if (MoveTarget(_moveRot: false))
		{
			rigid.velocity = Vector3.zero;
		}
		MoveRot(biteUki.GetUkiAnchor().position - base.transform.position);
		if (!biteUki.IsFishingMode())
		{
			SetStopBiteMode();
		}
	}
	private void FishEscapeProcess()
	{
		if (escapeTime < 0f)
		{
			rigid.velocity = Vector3.zero;
			fishState = FishState.WAIT;
			fishShadowAnimationCtrl.SetBool(0, value: true);
		}
		else
		{
			MoveTarget();
			escapeTime -= Time.deltaTime;
		}
	}
	private bool MoveTarget(bool _moveRot = true)
	{
		if (distFromToTarget > STOP_CHECK_DISTANCE)
		{
			Move(moveTargetPoint - base.transform.position, 1f, _moveRot);
			return false;
		}
		return true;
	}
	public void Move(Vector3 _moveDir, float _speedMag = 1f, bool _moveRot = true)
	{
		moveVec = _moveDir;
		moveVec.y = 0f;
		moveVec *= moveSpeed * _speedMag;
		rigid.AddForce(moveVec + Vector3.down, ForceMode.Impulse);
		if (rigid.velocity.magnitude > moveSpeedMax * _speedMag)
		{
			rigid.velocity = rigid.velocity.normalized * moveSpeedMax * _speedMag;
		}
		if (_moveRot)
		{
			MoveRot(_moveDir);
		}
	}
	private void MoveRot(Vector3 _moveDir, bool _immediate = false)
	{
		calcVec[0] = _moveDir;
		rot.x = 0f;
		rot.y = CalcManager.Rot(calcVec[0], CalcManager.AXIS.Y);
		rot.z = 0f;
		if (_immediate)
		{
			rigid.MoveRotation(Quaternion.Euler(rot));
		}
		else
		{
			rigid.MoveRotation(Quaternion.Lerp(base.transform.localRotation, Quaternion.Euler(rot), 5f * Time.deltaTime));
		}
	}
	public FishState GetFishState()
	{
		return fishState;
	}
	public int GetFishID()
	{
		return fishData.fishId;
	}
	public FishingDefinition.FishType GetFishType()
	{
		return fishData.fishType;
	}
	public FishingDefinition.FishSizeType GetFishSizeType()
	{
		return fishData.fishSizeType;
	}
	public Vector3 GetPos(bool _isLocal = false)
	{
		if (!_isLocal)
		{
			return base.transform.position;
		}
		return base.transform.localPosition;
	}
	public void SetFishShadowInactive()
	{
		isActive = false;
		base.gameObject.SetActive(value: false);
	}
	public void SetStartBiteMode(Fishing_RodUki _ukiTrans)
	{
		fishState = FishState.BITE_MODE;
		biteUki = _ukiTrans;
	}
	public void SetStopBiteMode()
	{
		fishState = FishState.ESCAPE;
		escapeTime = 3f;
		moveTargetPoint = biteUki.GetUkiAnchor().position + GetCirclePosition(GetFromToAngle(biteUki.GetUkiAnchor().position, base.transform.position), 0.5f);
		biteUki = null;
	}
	public bool IsActive()
	{
		return isActive;
	}
	private Vector3 GetCirclePosition(float _angle, float _radius)
	{
		return new Vector3(Mathf.Cos(_angle * ((float)Math.PI / 180f)) * _radius, 0f, Mathf.Sin(_angle * ((float)Math.PI / 180f)) * _radius);
	}
	private float GetFromToAngle(Vector3 p1, Vector3 p2)
	{
		return Mathf.Atan2(p2.z - p1.z, p2.x - p1.x) * 57.29578f;
	}
	public void FishShadowFadeIn()
	{
		fishShadowSprite.SetAlpha(0f);
		StartCoroutine(SetAlphaColor(fishShadowSprite, 0.5f));
	}
	private IEnumerator SetAlphaColor(SpriteRenderer _renderer, float _fadeTime)
	{
		float time = 0f;
		while (time < _fadeTime)
		{
			Color color = _renderer.color;
			color.a = Mathf.Lerp(0f, 1f, time / _fadeTime);
			_renderer.color = color;
			time += Time.deltaTime;
			yield return null;
		}
	}
	private void OnDrawGizmos()
	{
		if (!isGizmos)
		{
			return;
		}
		if (biteUki != null)
		{
			Gizmos.DrawLine(base.transform.position, biteUki.GetUkiAnchor().position);
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(biteUki.GetUkiAnchor().position + GetCirclePosition(GetFromToAngle(biteUki.GetUkiAnchor().position, base.transform.position), 0.125f), 0.2f);
			Gizmos.color = Color.yellow;
			for (int i = 0; i < 360; i++)
			{
				Gizmos.DrawWireSphere(biteUki.GetUkiAnchor().position + GetCirclePosition(i, 0.125f), 0.01f);
			}
		}
		Gizmos.color = Color.red;
		Gizmos.DrawLine(base.transform.position, moveTargetPoint);
	}
}
