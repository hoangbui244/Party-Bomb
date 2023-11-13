using UnityEngine;
public class SnowBoard_Character : MonoBehaviour
{
	[SerializeField]
	[Header("キャラの親アンカ\u30fc")]
	public GameObject characterAnchor_Xrot;
	[SerializeField]
	[Header("足元の地面座標取得用アンカ\u30fc")]
	private Transform downAnchor;
	[SerializeField]
	[Header("前方の地面座標取得用アンカ\u30fc")]
	private Transform forwardAnchor;
	private RaycastHit Hit;
	private Vector3 angle;
	private Quaternion targetAngle;
	private float angleY;
	private int layerMask = 1048576;
	public Vector3 angleAcc;
	public bool isForward;
	public bool isAngleUpdate = true;
	private void Update()
	{
		if (Physics.Raycast(forwardAnchor.position, Vector3.down, out Hit, 10f, layerMask))
		{
			angle = Hit.point - downAnchor.position;
		}
		UnityEngine.Debug.DrawRay(forwardAnchor.position, Vector3.down * 10f, Color.red);
		targetAngle = Quaternion.LookRotation(angle, Vector3.up);
		angleY = targetAngle.eulerAngles.x;
		if (angleY <= 180f)
		{
			angleY = Mathf.Clamp(angleY, 0f, 30f);
		}
		else
		{
			angleY = Mathf.Clamp(angleY, 330f, 360f);
		}
		if (isAngleUpdate)
		{
			LeanTween.cancel(characterAnchor_Xrot);
			LeanTween.rotateX(characterAnchor_Xrot, angleY, 0.1f);
		}
		if (Hit.point.y <= downAnchor.position.y + 0.05f)
		{
			isForward = true;
			angleAcc = (Hit.point - downAnchor.position).normalized;
		}
		else
		{
			isForward = false;
			angleAcc = -(Hit.point - downAnchor.position).normalized;
		}
	}
}
