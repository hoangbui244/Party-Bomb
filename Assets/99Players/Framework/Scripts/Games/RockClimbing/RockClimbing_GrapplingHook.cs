using System;
using System.Collections.Generic;
using UnityEngine;
public class RockClimbing_GrapplingHook : MonoBehaviour
{
	public enum State
	{
		NONE,
		THROW,
		HOOK
	}
	private RockClimbing_Player player;
	private Rigidbody rigid;
	private State state;
	private Transform originParent;
	private Vector3 originPos;
	private Vector3 originRot;
	private Vector3 throwOriginPos;
	private Vector3 throwTargetPos;
	[SerializeField]
	[Header("鉤オブジェクトのコライダ\u30fc")]
	private SphereCollider hookCollider;
	private float hookColliderRadius;
	[SerializeField]
	[Header("縄アンカ\u30fc配列")]
	private Transform[] arrayRopeLineAnchor;
	private List<Transform> ropeLineList = new List<Transform>();
	private float[] arrayRopeLerpDistance;
	private int distanceIdx;
	[SerializeField]
	[Header("鉤オブジェクト")]
	private GameObject hookObj;
	[SerializeField]
	[Header("縄アンカ\u30fcのLineRenderer")]
	private LineRenderer ropeLineRenderer;
	private Collider[] arrayOverLapCollider = new Collider[4];
	private int checkOverLapLayerMask;
	private string[] checkOverLapMaskNameList = new string[1]
	{
		"Default"
	};
	public void Init(RockClimbing_Player _player)
	{
		state = State.NONE;
		player = _player;
		rigid = GetComponent<Rigidbody>();
		originParent = base.transform.parent;
		originPos = base.transform.localPosition;
		originRot = base.transform.localEulerAngles;
		hookColliderRadius = hookCollider.transform.lossyScale.x * hookCollider.radius;
		ropeLineList.Add(arrayRopeLineAnchor[0]);
		ropeLineList.Add(arrayRopeLineAnchor[arrayRopeLineAnchor.Length - 1]);
		arrayRopeLerpDistance = new float[arrayRopeLineAnchor.Length];
		float num = SingletonCustom<RockClimbing_PlayerManager>.Instance.GetThrowGrapplingHookDistance() / (float)arrayRopeLineAnchor.Length;
		for (int i = 0; i < arrayRopeLineAnchor.Length; i++)
		{
			arrayRopeLerpDistance[i] = (float)i * num;
		}
		ropeLineRenderer.positionCount = ropeLineList.Count;
		ropeLineRenderer.enabled = false;
		ropeLineRenderer.material = SingletonCustom<RockClimbing_PlayerManager>.Instance.GetGrapplingHookMat(SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)player.GetUserType()]);
		distanceIdx = 0;
		checkOverLapLayerMask = LayerMask.GetMask(checkOverLapMaskNameList);
	}
	public void UpdateMethod()
	{
		if (state != State.THROW)
		{
			return;
		}
		base.transform.position = Vector3.MoveTowards(base.transform.position, throwTargetPos, SingletonCustom<RockClimbing_PlayerManager>.Instance.GetThrowGrapplingHookSpeed() * Time.deltaTime);
		Vector3 normalized = (throwTargetPos - base.transform.position).normalized;
		base.transform.LookAt(throwTargetPos);
		if (base.transform.position == throwTargetPos)
		{
			Hook();
			return;
		}
		if (distanceIdx < arrayRopeLerpDistance.Length - 1 && CalcManager.Length(throwOriginPos, base.transform.position) >= arrayRopeLerpDistance[distanceIdx + 1])
		{
			distanceIdx++;
			if (!ropeLineList.Contains(arrayRopeLineAnchor[distanceIdx]))
			{
				ropeLineList.Insert(ropeLineList.Count - 1, arrayRopeLineAnchor[distanceIdx]);
				ropeLineRenderer.positionCount = ropeLineList.Count;
			}
		}
		arrayRopeLineAnchor[arrayRopeLineAnchor.Length - 1].position = player.GetHaveGrapplingHook().position;
		for (int i = 0; i < ropeLineList.Count; i++)
		{
			ropeLineRenderer.SetPosition(i, ropeLineList[i].position);
		}
	}
	public State GetState()
	{
		return state;
	}
	public float GetHookColliderRadius()
	{
		return hookColliderRadius;
	}
	public Transform GetRopeEndAnchor()
	{
		return ropeLineList[ropeLineList.Count - 1];
	}
	public void Throw(Vector3 _throwPos)
	{
		state = State.THROW;
		base.transform.parent = player.GetThrowGrapplingHookAnchor();
		base.transform.localEulerAngles = Vector3.zero;
		throwOriginPos = base.transform.position;
		throwTargetPos = _throwPos;
		ropeLineRenderer.enabled = true;
	}
	private void Hook()
	{
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, hookColliderRadius, arrayOverLapCollider, checkOverLapLayerMask);
		if (num <= 0)
		{
			return;
		}
		RockClimbing_GrapplingHookPoint rockClimbing_GrapplingHookPoint = null;
		for (int i = 0; i < num; i++)
		{
			rockClimbing_GrapplingHookPoint = arrayOverLapCollider[i].GetComponent<RockClimbing_GrapplingHookPoint>();
			if (rockClimbing_GrapplingHookPoint != null && rockClimbing_GrapplingHookPoint.CheckClimbPlayerType(player.GetPlayerNo()))
			{
				break;
			}
		}
		if (rockClimbing_GrapplingHookPoint != null)
		{
			state = State.HOOK;
			float num2 = Mathf.Sign(throwTargetPos.z - throwOriginPos.z);
			UnityEngine.Debug.Log("引っかけ角度\u3000" + num2.ToString());
			LeanTween.rotateLocal(hookObj, new Vector3(num2 * 35f, 0f, 0f), 0.2f);
			if (!player.GetIsCpu())
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_alpineskiing_stock");
			}
			ropeLineList.Clear();
			ropeLineList.Add(arrayRopeLineAnchor[0]);
			ropeLineList.Add(arrayRopeLineAnchor[arrayRopeLineAnchor.Length - 1]);
			ropeLineRenderer.positionCount = ropeLineList.Count;
			for (int j = 0; j < ropeLineList.Count; j++)
			{
				ropeLineRenderer.SetPosition(j, ropeLineList[j].position);
			}
			rockClimbing_GrapplingHookPoint.SetColliderActive(_isActive: false);
			RockClimbing_ClimbOnFoundation climbOnFoundation = rockClimbing_GrapplingHookPoint.GetClimbOnFoundation();
			player.SetClimbOnFoundation(climbOnFoundation);
			climbOnFoundation.GetClimbOnCollider(player.GetPlayerNo()).SetColliderActive(_isActive: true);
			player.GrapplingHook();
		}
	}
	public void SetCollectRope()
	{
		Vector3 diffPos = ropeLineList[0].position - ropeLineList[ropeLineList.Count - 1].position;
		LeanTween.value(base.gameObject, 0f, 1f, SingletonCustom<RockClimbing_PlayerManager>.Instance.GetCollectRopeTime()).setOnUpdate(delegate(float _value)
		{
			ropeLineList[ropeLineList.Count - 1].position = ropeLineList[0].position - diffPos * (1f - _value);
			for (int i = 0; i < ropeLineList.Count; i++)
			{
				ropeLineRenderer.SetPosition(i, ropeLineList[i].position);
			}
		}).setOnComplete((Action)delegate
		{
			state = State.NONE;
			ropeLineRenderer.enabled = false;
			base.transform.parent = originParent;
			base.transform.localPosition = originPos;
			base.transform.localEulerAngles = originRot;
			hookObj.transform.localEulerAngles = Vector3.zero;
		});
	}
	private void OnDrawGizmos()
	{
	}
}
