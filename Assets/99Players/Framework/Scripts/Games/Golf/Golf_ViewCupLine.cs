using System;
using System.Collections.Generic;
using UnityEngine;
public class Golf_ViewCupLine : MonoBehaviour
{
	[SerializeField]
	[Header("LineRenderer")]
	private LineRenderer line;
	private List<Vector3> posList = new List<Vector3>();
	public Vector3 posListVec;
	public float diffTime;
	public float time;
	public bool isWaitViewLine;
	protected int fieldLayerMask;
	private readonly string[] FIELD_LAYER_NAME = new string[1]
	{
		"BackGround"
	};
	public void Init()
	{
		fieldLayerMask = LayerMask.GetMask(FIELD_LAYER_NAME);
		diffTime = SingletonCustom<Golf_ViewCupLineManager>.Instance.GetViewLineTime() / (float)SingletonCustom<Golf_ViewCupLineManager>.Instance.GetLineRayMaxCnt();
		line.gameObject.SetActive(value: false);
	}
	public void InitPlay()
	{
		isWaitViewLine = false;
		posList.Clear();
		line.positionCount = 0;
		posListVec = Vector3.zero;
		time = 0f;
		line.gameObject.SetActive(value: false);
	}
	public void UpdateMethod()
	{
		if (!isWaitViewLine)
		{
			return;
		}
		time += Time.deltaTime;
		time = Mathf.Clamp(time, 0f, diffTime);
		float num = time / diffTime;
		line.SetPosition(line.positionCount - 1, posList[line.positionCount - 1] + posListVec.normalized * (num * posListVec.magnitude));
		if (time >= diffTime)
		{
			time = 0f;
			if (line.positionCount + 1 == SingletonCustom<Golf_ViewCupLineManager>.Instance.GetLineRayMaxCnt())
			{
				SingletonCustom<Golf_GameManager>.Instance.CalcPoint();
				return;
			}
			line.positionCount++;
			line.SetPosition(line.positionCount - 1, posList[line.positionCount - 1]);
			posListVec = posList[line.positionCount - 1] - posList[line.positionCount - 2];
		}
	}
	public void Show()
	{
		LeanTween.delayedCall(base.gameObject, SingletonCustom<Golf_ViewCupLineManager>.Instance.GetViewLineWaitTime(), (Action)delegate
		{
			isWaitViewLine = true;
			Vector3 cupPos = SingletonCustom<Golf_FieldManager>.Instance.GetHole().GetCupPos();
			cupPos.y = 0f;
			Vector3 position = SingletonCustom<Golf_BallManager>.Instance.GetTurnPlayerBall().transform.position;
			position.y = 0f;
			int lineRayMaxCnt = SingletonCustom<Golf_ViewCupLineManager>.Instance.GetLineRayMaxCnt();
			UnityEngine.Debug.Log("rayCnt " + lineRayMaxCnt.ToString());
			Vector3 vector = cupPos - position;
			float num = vector.magnitude / (float)lineRayMaxCnt;
			for (int i = 0; i < lineRayMaxCnt; i++)
			{
				if (Physics.Raycast(position + vector.normalized * ((float)i * num), Vector3.down, out RaycastHit hitInfo, float.PositiveInfinity, fieldLayerMask))
				{
					posList.Add(hitInfo.point + SingletonCustom<Golf_ViewCupLineManager>.Instance.GetDiffLinePos());
				}
			}
			line.positionCount = 2;
			line.SetPosition(line.positionCount - 2, posList[line.positionCount - 2]);
			line.SetPosition(line.positionCount - 1, posList[line.positionCount - 1]);
			posListVec = posList[line.positionCount - 1] - posList[line.positionCount - 2];
			line.gameObject.SetActive(value: true);
		});
	}
}
