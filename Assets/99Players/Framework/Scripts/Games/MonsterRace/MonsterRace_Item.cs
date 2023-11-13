using System;
using System.Collections;
using UnityEngine;
public class MonsterRace_Item : MonoBehaviour
{
	[SerializeField]
	[Header("何秒で表示する？")]
	private float appear = 0.3f;
	[SerializeField]
	[Header("何秒で消える？")]
	private float vanishTime = 0.2f;
	[SerializeField]
	[Header("何秒後に出現するか")]
	private float maxRespawnTime = 10f;
	[SerializeField]
	[Header("出現するときのアニメ\u30fcション")]
	private AnimationCurve appearAni;
	[SerializeField]
	[Header("消える時のスケ\u30fcルのアニメ\u30fcション")]
	private AnimationCurve vanishAni;
	private BoxCollider collider;
	private Collider playerColl;
	private Vector3 objectScale;
	private float scaleMoveTime = 1f;
	private MonsterRace_CarScript playerScript;
	private void Start()
	{
		objectScale = base.transform.localScale;
		collider = GetComponent<BoxCollider>();
	}
	private void Update()
	{
		base.gameObject.transform.localScale = objectScale * scaleMoveTime;
		base.gameObject.transform.Rotate(new Vector3(0f, 2.5f, 0f));
	}
	private void Vanish()
	{
		LeanTween.value(base.gameObject, 0f, 1f, vanishTime).setOnUpdate(delegate(float volume)
		{
			scaleMoveTime = vanishAni.Evaluate(volume);
		});
		LeanTween.delayedCall(base.gameObject, 0.01f, (Action)delegate
		{
			collider.enabled = false;
		});
	}
	private IEnumerator Appear()
	{
		yield return new WaitForSeconds(maxRespawnTime);
		LeanTween.value(base.gameObject, 1f, 0f, appear).setOnUpdate(delegate(float volume)
		{
			scaleMoveTime = vanishAni.Evaluate(volume);
		});
		collider.enabled = true;
		playerColl = null;
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && !(playerColl != null))
		{
			playerColl = other;
			playerScript = other.transform.parent.GetComponent<MonsterRace_CarScript>();
			playerScript.StaminaCount = Mathf.Clamp(playerScript.StaminaCount + 1, 0, playerScript.MaxStaminaCount);
			if (playerScript.IsPlayer && !playerScript.IsGoal && !SingletonCustom<MonsterRace_GameManager>.Instance.IsGameEnd)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_good");
			}
			Vanish();
			StartCoroutine(Appear());
		}
	}
}
