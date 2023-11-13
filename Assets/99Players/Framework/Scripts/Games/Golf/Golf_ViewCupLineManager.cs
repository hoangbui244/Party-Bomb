using UnityEngine;
public class Golf_ViewCupLineManager : SingletonCustom<Golf_ViewCupLineManager>
{
	[SerializeField]
	[Header("ラインを表示するクラス")]
	private Golf_ViewCupLine viewCupLine;
	private float LINE_RAY_MAX_CNT_DISTANCE;
	private float LINE_RAY_MIN_CNT_DISTANCE;
	[SerializeField]
	[Header("ラインを表示するためのRayを打つ最大個数")]
	private int LINE_RAY_MAX_CNT;
	[SerializeField]
	[Header("ラインを表示するまでの待機時間")]
	private float VIEW_LINE_WAIT_TIME;
	[SerializeField]
	[Header("ラインを表示する時間")]
	private float VIEW_LINE_TIME;
	[SerializeField]
	[Header("ラインの補正座標")]
	private Vector3 DIFF_LINE_POS;
	public void Init()
	{
		viewCupLine.Init();
		Vector3 cupPos = SingletonCustom<Golf_FieldManager>.Instance.GetHole().GetCupPos();
		cupPos.y = 0f;
		Vector3 position = SingletonCustom<Golf_BallManager>.Instance.GetTurnPlayerBall().transform.position;
		position.y = 0f;
		LINE_RAY_MAX_CNT_DISTANCE = CalcManager.Length(cupPos, position);
		LINE_RAY_MIN_CNT_DISTANCE = LINE_RAY_MAX_CNT_DISTANCE * 0.25f;
	}
	public void InitPlay()
	{
		viewCupLine.InitPlay();
	}
	public void UpdateMethod()
	{
		if (SingletonCustom<Golf_GameManager>.Instance.GetState() == Golf_GameManager.State.VIEW_CUP_LINE)
		{
			viewCupLine.UpdateMethod();
		}
	}
	public void Show()
	{
		viewCupLine.Show();
	}
	public int GetLineRayMaxCnt()
	{
		return LINE_RAY_MAX_CNT;
	}
	public float GetChangeRayCnt(float _distance)
	{
		return ClampDistance(_distance, LINE_RAY_MIN_CNT_DISTANCE, LINE_RAY_MAX_CNT_DISTANCE) / LINE_RAY_MAX_CNT_DISTANCE * (float)LINE_RAY_MAX_CNT;
	}
	public float ClampDistance(float _distance, float _min, float _max)
	{
		return Mathf.Clamp(_distance, _min, _max);
	}
	public float GetViewLineWaitTime()
	{
		return VIEW_LINE_WAIT_TIME;
	}
	public float GetViewLineTime()
	{
		return VIEW_LINE_TIME;
	}
	public Vector3 GetDiffLinePos()
	{
		return DIFF_LINE_POS;
	}
}
