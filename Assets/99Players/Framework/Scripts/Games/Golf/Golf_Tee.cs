using UnityEngine;
public class Golf_Tee : MonoBehaviour
{
	[SerializeField]
	[Header("ボ\u30fcルを置くアンカ\u30fc")]
	private Transform putBallAnchor;
	public Vector3 GetPutBallPos()
	{
		return putBallAnchor.position;
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(putBallAnchor.position, 0.05f);
	}
}
