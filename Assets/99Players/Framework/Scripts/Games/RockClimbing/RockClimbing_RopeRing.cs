using UnityEngine;
public class RockClimbing_RopeRing : MonoBehaviour
{
	private RockClimbing_Player player;
	[SerializeField]
	[Header("ラインをつなげるアンカ\u30fc")]
	private Transform connectAnchor;
	[SerializeField]
	[Header("地面との補正高さ")]
	private float diffGroundHeight;
	[SerializeField]
	[Header("斜面の時の補正高さ")]
	private float diffSlopeHeight;
	private int layerMask;
	private string[] MaskNameList = new string[1]
	{
		"Field"
	};
	private Transform originParent;
	private Vector3 originPos;
	private Vector3 originRot;
	[SerializeField]
	[Header("縄の輪っかオブジェクト")]
	private MeshRenderer ropeRingObj;
	[SerializeField]
	[Header("縄の終点アンカ\u30fcにつなげるLineRenderer")]
	private LineRenderer ropeLineRenderer;
	public void Init(RockClimbing_Player _player)
	{
		player = _player;
		originParent = base.transform.parent;
		originPos = base.transform.localPosition;
		originRot = base.transform.localEulerAngles;
		ropeLineRenderer.positionCount = 2;
		ropeLineRenderer.enabled = false;
		Material grapplingHookMat = SingletonCustom<RockClimbing_PlayerManager>.Instance.GetGrapplingHookMat(SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)player.GetUserType()]);
		ropeRingObj.material = grapplingHookMat;
		ropeLineRenderer.material = grapplingHookMat;
		layerMask = LayerMask.GetMask(MaskNameList);
	}
	public void PutOnGround()
	{
		base.transform.parent = player.GetThrowGrapplingHookAnchor();
		Vector3 position = player.GetGrapplingHook().GetRopeEndAnchor().position;
		UnityEngine.Debug.DrawLine(position, position + Vector3.down * 5f, Color.white, 10f);
		if (Physics.Raycast(position, Vector3.down, out RaycastHit hitInfo, 5f, layerMask))
		{
			float d = diffGroundHeight;
			float num = Vector3.Angle(Vector3.up, hitInfo.normal);
			if (player.GetClimbOnFoundation().GetClimbOnFoundationType() > RockClimbing_ClimbingWallManager.ClimbOnFoundationType.Roof_1)
			{
				num *= -1f;
				d = diffSlopeHeight;
			}
			UnityEngine.Debug.Log("angle " + num.ToString());
			base.transform.localEulerAngles = new Vector3(num, 0f, 0f);
			base.transform.position = hitInfo.point + Quaternion.Euler(base.transform.up) * (Vector3.up * d);
			ropeLineRenderer.enabled = true;
			ropeLineRenderer.SetPosition(0, position);
			ropeLineRenderer.SetPosition(1, connectAnchor.position);
			return;
		}
		position.x = player.transform.position.x;
		if (Physics.Raycast(position, Vector3.down, out hitInfo, 5f, layerMask))
		{
			float d2 = diffGroundHeight;
			float num2 = Vector3.Angle(Vector3.up, hitInfo.normal);
			if (player.GetClimbOnFoundation().GetClimbOnFoundationType() > RockClimbing_ClimbingWallManager.ClimbOnFoundationType.Roof_1)
			{
				num2 *= -1f;
				d2 = diffSlopeHeight;
			}
			UnityEngine.Debug.Log("angle " + num2.ToString());
			base.transform.localEulerAngles = new Vector3(num2, 0f, 0f);
			base.transform.position = hitInfo.point + Quaternion.Euler(base.transform.up) * (Vector3.up * d2);
			ropeLineRenderer.enabled = true;
			ropeLineRenderer.SetPosition(0, position);
			ropeLineRenderer.SetPosition(1, connectAnchor.position);
		}
	}
	public void SetCollectRope()
	{
		ropeLineRenderer.enabled = false;
		base.transform.parent = originParent;
		base.transform.localPosition = originPos;
		base.transform.localEulerAngles = originRot;
	}
}
