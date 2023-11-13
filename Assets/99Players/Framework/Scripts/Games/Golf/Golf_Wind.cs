using UnityEngine;
public class Golf_Wind : MonoBehaviour
{
	private void OnTriggerStay(Collider other)
	{
		if (!(other.tag == "Ball"))
		{
			return;
		}
		if (other.gameObject.layer == LayerMask.NameToLayer("Ball"))
		{
			SingletonCustom<Golf_BallManager>.Instance.GetTurnPlayerBall().SetWindForce();
			return;
		}
		Golf_PredictionBall component = other.GetComponent<Golf_PredictionBall>();
		if (component.GetCurrentPredictionType() == Golf_PredictionBall.PredictionType.Wind)
		{
			component.SetWindForce();
		}
	}
}
