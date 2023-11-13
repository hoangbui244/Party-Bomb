using UnityEngine;
public class Golf_OB : MonoBehaviour
{
	private void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.layer == LayerMask.NameToLayer("Ball"))
		{
			if (SingletonCustom<Golf_GameManager>.Instance.GetState() != Golf_GameManager.State.CALC_POINT)
			{
				SingletonCustom<Golf_BallManager>.Instance.GetTurnPlayerBall().SetIsOB();
				SingletonCustom<Golf_GameManager>.Instance.CalcPoint();
			}
		}
		else
		{
			col.GetComponent<Golf_PredictionBall>().SetIsCheckExit();
		}
	}
}
