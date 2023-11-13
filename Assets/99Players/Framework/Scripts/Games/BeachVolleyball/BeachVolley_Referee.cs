using UnityEngine;
public class BeachVolley_Referee : MonoBehaviour
{
	[SerializeField]
	[Header("身体パ\u30fcツ")]
	private Transform[] bodyParts;
	private float neckRotSpeed = 10f;
	private float maxRot = 60f;
	private float neutralInterval;
	private void Update()
	{
		if (BeachVolley_Define.MGM.CheckInPlay())
		{
			Vector3 vector = (BeachVolley_Define.BM.GetBallPos() - base.transform.position).normalized;
			vector.y = 0f;
			float num = AngleVec(new Vector3(base.transform.forward.x, base.transform.forward.z, 0f), new Vector3(vector.x, vector.z, 0f));
			if (num > maxRot)
			{
				vector = ((!(base.transform.InverseTransformPoint(base.transform.position + vector).x > 0f)) ? CalcManager.PosRotation2D(vector, Vector3.zero, num - maxRot, CalcManager.AXIS.Y) : CalcManager.PosRotation2D(vector, Vector3.zero, 0f - (num - maxRot), CalcManager.AXIS.Y));
			}
			bodyParts[0].LookAt(bodyParts[0].transform.position + vector);
			neutralInterval = 0.25f;
		}
		else if (neutralInterval > 0f)
		{
			neutralInterval -= Time.deltaTime;
		}
		else
		{
			bodyParts[0].SetLocalEulerAnglesY(Mathf.LerpAngle(bodyParts[0].localEulerAngles.y, 0f, neckRotSpeed * Time.deltaTime));
		}
	}
	public static float AngleVec(Vector3 _vec1, Vector3 _vec2)
	{
		return Mathf.Acos((_vec1.x * _vec2.x + _vec1.y * _vec2.y) / (_vec1.magnitude * _vec2.magnitude)) * 57.29578f;
	}
}
