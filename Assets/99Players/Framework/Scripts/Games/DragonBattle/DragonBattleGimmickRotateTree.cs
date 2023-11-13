using UnityEngine;
public class DragonBattleGimmickRotateTree : MonoBehaviour
{
	[SerializeField]
	[Header("回転量")]
	private float speed;
	private Rigidbody rigid;
	private Vector3 eulerAngleVelocity;
	private Quaternion deltaRotation;
	private void Start()
	{
		base.transform.SetLocalEulerAnglesY(UnityEngine.Random.Range(0f, 360f));
		rigid = GetComponent<Rigidbody>();
		eulerAngleVelocity = new Vector3(0f, speed, 0f);
	}
	private void FixedUpdate()
	{
		deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.fixedDeltaTime);
		rigid.MoveRotation(rigid.rotation * deltaRotation);
	}
}
