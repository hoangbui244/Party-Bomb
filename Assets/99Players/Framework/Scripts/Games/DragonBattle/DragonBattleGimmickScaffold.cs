using UnityEngine;
public class DragonBattleGimmickScaffold : MonoBehaviour
{
	private Vector3 initPos;
	private float waitTime;
	[SerializeField]
	[Header("連動エフェクト")]
	private ParticleSystem[] arrayEffect;
	private void Awake()
	{
		initPos = base.transform.localPosition;
	}
	private void Update()
	{
		if (waitTime > 0f)
		{
			waitTime -= Time.deltaTime;
		}
	}
	public void OnCollisionEnter(Collision collision)
	{
		if (!(waitTime > 0f))
		{
			LeanTween.cancel(base.gameObject);
			base.transform.SetLocalPositionY(initPos.y - 0.1f);
			LeanTween.moveLocalY(base.gameObject, initPos.y, 0.65f).setEaseOutBounce();
			for (int i = 0; i < arrayEffect.Length; i++)
			{
				arrayEffect[i].Emit(1);
			}
			waitTime = 0.5f;
		}
	}
	private void OnDestroy()
	{
		LeanTween.cancel(base.gameObject);
	}
}
