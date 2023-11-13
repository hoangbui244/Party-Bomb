using UnityEngine;
public class FlyingSquirrelRace_ObstacleObject_CallCollision : MonoBehaviour
{
	[SerializeField]
	[Header("衝突判定の関数を呼ぶタ\u30fcゲット")]
	private FlyingSquirrelRace_ObstacleObject callTarget;
	[SerializeField]
	[Header("タ\u30fcゲットを非表示にするかどうかのフラグ")]
	private bool isTargetHide;
	private void OnTriggerEnter(Collider other)
	{
		callTarget.OnTriggerEnterMethod(other, isTargetHide);
	}
}
