using UnityEngine;
public class DragonBattleGimmickMover : MonoBehaviour
{
	[SerializeField]
	[Header("移動量")]
	private Vector3 move;
	public void Start()
	{
		LeanTween.moveLocal(base.gameObject, base.transform.localPosition + move, 2f).setLoopPingPong();
	}
	private void OnDestroy()
	{
		LeanTween.cancel(base.gameObject);
	}
}
