using UnityEngine;
public class Golf_CupIn : MonoBehaviour
{
	[SerializeField]
	[Header("カップイン時のエフェクト")]
	private ParticleSystem cupInEffect;
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Ball"))
		{
			Golf_Ball turnPlayerBall = SingletonCustom<Golf_BallManager>.Instance.GetTurnPlayerBall();
			if (!turnPlayerBall.GetIsCupIn())
			{
				turnPlayerBall.SetIsCupIn();
				cupInEffect.Play();
				SingletonCustom<AudioManager>.Instance.SePlay("se_golf_cup_in");
				turnPlayerBall.Stop();
			}
		}
		else
		{
			other.GetComponent<Golf_PredictionBall>().SetIsCheckExit();
		}
	}
}
