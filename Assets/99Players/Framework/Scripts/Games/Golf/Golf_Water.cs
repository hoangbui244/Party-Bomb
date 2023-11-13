using UnityEngine;
public class Golf_Water : MonoBehaviour
{
	[SerializeField]
	[Header("水しぶきのエフェクト")]
	private ParticleSystem rippleEffect;
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Ball"))
		{
			if (SingletonCustom<Golf_GameManager>.Instance.GetState() != Golf_GameManager.State.CALC_POINT)
			{
				SingletonCustom<Golf_BallManager>.Instance.GetTurnPlayerBall().SetIsOB();
				Vector3 position = other.ClosestPointOnBounds(base.transform.position);
				position.y = rippleEffect.transform.position.y;
				rippleEffect.transform.position = position;
				rippleEffect.Play();
				SingletonCustom<AudioManager>.Instance.SePlay("se_golf_water_in");
				SingletonCustom<Golf_GameManager>.Instance.CalcPoint();
			}
		}
		else
		{
			other.GetComponent<Golf_PredictionBall>().SetIsCheckExit();
		}
	}
}
