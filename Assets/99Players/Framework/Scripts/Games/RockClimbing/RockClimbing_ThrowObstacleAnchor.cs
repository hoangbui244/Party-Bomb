using System;
using UnityEngine;
public class RockClimbing_ThrowObstacleAnchor : MonoBehaviour
{
	[SerializeField]
	[Header("障子が開蹴る時に動かすＸ座標")]
	private float SHOJI_MOVE_X;
	[SerializeField]
	[Header("左側の障子")]
	private GameObject leftShoji;
	private float originLeftShojiPosX;
	[SerializeField]
	[Header("右側の障子")]
	private GameObject rightShoji;
	private float originRightShojiPosX;
	[SerializeField]
	[Header("忍者のキャラを配置するアンカ\u30fc")]
	private Transform[] arrayCharaAnchor;
	private bool isOpen;
	private bool isComplete;
	public void Init()
	{
		originLeftShojiPosX = leftShoji.transform.localPosition.x;
		originRightShojiPosX = rightShoji.transform.localPosition.x;
	}
	public void OpenShoji(float _time)
	{
		isOpen = true;
		LeanTween.moveLocalX(leftShoji, originLeftShojiPosX + SHOJI_MOVE_X, _time).setEaseOutBounce();
		LeanTween.moveLocalX(rightShoji, originRightShojiPosX - SHOJI_MOVE_X, _time).setEaseOutBounce();
		LeanTween.delayedCall(base.gameObject, _time, (Action)delegate
		{
			isComplete = true;
		});
	}
	public void CloseShoji(float _time)
	{
		isOpen = false;
		LeanTween.moveLocalX(leftShoji, originLeftShojiPosX, _time);
		LeanTween.moveLocalX(rightShoji, originRightShojiPosX, _time);
		LeanTween.delayedCall(base.gameObject, _time, (Action)delegate
		{
			isComplete = false;
		});
	}
	public Transform GetCharaAnchor(Vector3 _pos)
	{
		int num = 0;
		float num2 = Mathf.Abs(arrayCharaAnchor[num].transform.position.x - _pos.x);
		for (int i = 1; i < arrayCharaAnchor.Length; i++)
		{
			float num3 = Mathf.Abs(arrayCharaAnchor[i].transform.position.x - _pos.x);
			if (num2 < num3)
			{
				num2 = num3;
				num = i;
			}
		}
		return arrayCharaAnchor[num];
	}
	public void StopThrowObstacle()
	{
		if (isOpen)
		{
			if (!isComplete)
			{
				LeanTween.cancel(leftShoji);
				LeanTween.cancel(rightShoji);
			}
			CloseShoji(0.25f);
		}
	}
}
