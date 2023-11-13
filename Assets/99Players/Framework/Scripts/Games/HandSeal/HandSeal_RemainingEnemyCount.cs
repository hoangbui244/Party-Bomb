using UnityEngine;
public class HandSeal_RemainingEnemyCount : MonoBehaviour
{
	[SerializeField]
	[Header("対象のHandSeal_Hand")]
	private HandSeal_Hand hand;
	private int allEnemyCount;
	private int nowKillCount;
	[SerializeField]
	[Header("SpriteNumbersスクリプト")]
	private SpriteNumbers spriteNumbers;
	private void Start()
	{
		allEnemyCount = SingletonCustom<HandSeal_EnemyManager>.Instance.AllEnemyCount();
		nowKillCount = 0;
		spriteNumbers.Set(allEnemyCount);
	}
	private void Update()
	{
		if (base.gameObject.activeInHierarchy && nowKillCount < hand.TotalKillCount)
		{
			nowKillCount = hand.TotalKillCount;
			spriteNumbers.Set(allEnemyCount - nowKillCount);
		}
	}
}
