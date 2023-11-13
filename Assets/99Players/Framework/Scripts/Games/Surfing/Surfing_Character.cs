using UnityEngine;
public class Surfing_Character : MonoBehaviour
{
	[SerializeField]
	[Header("対応するSurfing_Surfer")]
	private Surfing_Surfer surfer;
	[SerializeField]
	[Header("キャラの親アンカ\u30fc")]
	private GameObject characterAnchor;
	[SerializeField]
	[Header("進行方向取得するオブジェクト")]
	private Transform angleAccAnchor;
	[SerializeField]
	[Header("前方_水面取得用Rayアンカ\u30fc")]
	private Transform frontRayAnchor;
	[SerializeField]
	[Header("後方_水面取得用Rayアンカ\u30fc")]
	private Transform backRayAnchor;
	private RaycastHit frontHit;
	private RaycastHit backHit;
	private Vector3 angle;
	private Quaternion targetAngle;
	private float angleY;
	private int layerMask = 1048576;
	public Vector3 angleAcc;
	public bool isForward;
	public bool isAngleUpdate = true;
	private int id_init;
	private int ownlayerMask;
	private void Start()
	{
		ownlayerMask = base.gameObject.layer;
		UnityEngine.Debug.Log("ownlayerMask = " + ownlayerMask.ToString());
		layerMask = (0x100000 | (1 << ownlayerMask));
	}
	public void UpdateMethod()
	{
		LeanTween.cancel(id_init);
		if (surfer.IsGround && !surfer.IsJump)
		{
			if (Physics.Raycast(frontRayAnchor.position, -frontRayAnchor.up, out frontHit, 1f, layerMask, QueryTriggerInteraction.Ignore))
			{
				UnityEngine.Debug.DrawRay(frontRayAnchor.position, -frontRayAnchor.up * frontHit.distance, Color.yellow);
				if (Physics.Raycast(backRayAnchor.position, -backRayAnchor.up, out backHit, 1f, layerMask, QueryTriggerInteraction.Ignore))
				{
					UnityEngine.Debug.DrawRay(backRayAnchor.position, -backRayAnchor.up * backHit.distance, Color.red);
					angle = frontHit.point - backHit.point;
					targetAngle = Quaternion.LookRotation(angle, characterAnchor.transform.up);
					characterAnchor.transform.localRotation = Quaternion.Lerp(characterAnchor.transform.localRotation, targetAngle, 0.2f);
					if (Mathf.Abs(angle.y) <= 0.2f)
					{
						id_init = LeanTween.rotateZ(characterAnchor, 0f, 0.5f).id;
					}
				}
			}
			else
			{
				angle.x = 0f;
				angle.y = characterAnchor.transform.eulerAngles.y;
				angle.z = 0f;
				id_init = LeanTween.rotateLocal(characterAnchor, angle, 0.5f).id;
			}
		}
		else
		{
			angle.x = 0f;
			angle.y = characterAnchor.transform.eulerAngles.y;
			angle.z = 0f;
			id_init = LeanTween.rotateLocal(characterAnchor, angle, 0.5f).id;
		}
		angleAcc = angleAccAnchor.forward;
	}
	public bool IsFacingTop()
	{
		if (characterAnchor.transform.localEulerAngles.y < 45f || characterAnchor.transform.localEulerAngles.y > 315f)
		{
			return true;
		}
		return false;
	}
	public bool IsFacingUnder()
	{
		if (characterAnchor.transform.localEulerAngles.y > 135f && characterAnchor.transform.localEulerAngles.y < 225f)
		{
			return true;
		}
		return false;
	}
}
