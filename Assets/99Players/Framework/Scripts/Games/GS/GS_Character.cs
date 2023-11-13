using System;
using UnityEngine;
public class GS_Character : MonoBehaviour
{
	[SerializeField]
	[Header("パスを表示")]
	private bool pathVisible;
	[SerializeField]
	[Header("パス")]
	private Transform[] path;
	private int pathIdx;
	private bool isNextArrow = true;
	private float addTime;
	private float prevX;
	public SpriteRenderer TargetObj
	{
		get;
		set;
	}
	public void Play()
	{
		prevX = TargetObj.transform.position.x;
		pathIdx = UnityEngine.Random.Range(0, path.Length);
		isNextArrow = (UnityEngine.Random.Range(0, 2) == 0);
		TargetObj.transform.position = path[pathIdx].position;
		NextPoint();
	}
	public void NextPoint()
	{
		if (isNextArrow)
		{
			pathIdx++;
			if (pathIdx >= path.Length)
			{
				isNextArrow = false;
				pathIdx = path.Length - 1;
				addTime = UnityEngine.Random.Range(0.25f, 0.75f);
				TargetObj.GetComponent<SimpleAnim>().SetIdx(1, addTime);
				LeanTween.delayedCall(TargetObj.gameObject, addTime, (Action)delegate
				{
					NextPoint();
				});
				return;
			}
		}
		else
		{
			pathIdx--;
			if (pathIdx < 0)
			{
				isNextArrow = true;
				pathIdx = 0;
				addTime = UnityEngine.Random.Range(0.25f, 0.75f);
				TargetObj.GetComponent<SimpleAnim>().SetIdx(1, addTime);
				LeanTween.delayedCall(TargetObj.gameObject, addTime, (Action)delegate
				{
					NextPoint();
				});
				return;
			}
		}
		LeanTween.move(TargetObj.gameObject, path[pathIdx], UnityEngine.Random.Range(3f, 5f)).setOnComplete((Action)delegate
		{
			NextPoint();
		});
		TargetObj.flipX = !isNextArrow;
	}
	public void Stop()
	{
		if (TargetObj != null)
		{
			LeanTween.cancel(TargetObj.gameObject);
			TargetObj = null;
		}
	}
	private void OnDrawGizmos()
	{
		if (path == null || !pathVisible)
		{
			return;
		}
		Gizmos.color = Color.blue;
		for (int i = 0; i < path.Length; i++)
		{
			if (i > 0)
			{
				Gizmos.DrawLine(path[i - 1].position, path[i].position);
			}
			Gizmos.DrawWireSphere(path[i].position, 15f);
		}
		Gizmos.color = Color.white;
	}
}
