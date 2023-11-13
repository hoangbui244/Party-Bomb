using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_OvalCoin : FlyingSquirrelRace_StageObject
{
	[SerializeField]
	[DisplayName("獲得スコア")]
	private int score = 100;
	public int Score => score;
	private void Update()
	{
		base.transform.AddLocalEulerAnglesY(100f * Time.deltaTime);
	}
	private void OnTriggerEnter(Collider other)
	{
		if (TryGetPlayer(other, out FlyingSquirrelRace_Player player) && !player.IsGoal)
		{
			player.CollectCoin(this);
			base.gameObject.SetActive(value: false);
		}
	}
}
