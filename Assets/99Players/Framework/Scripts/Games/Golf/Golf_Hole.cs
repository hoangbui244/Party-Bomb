using UnityEngine;
public class Golf_Hole : MonoBehaviour
{
	[Header("デバッグ：フィ\u30fcルドの種類を固定するかどうか")]
	public bool isDebugHoleField;
	[Header("デバッグ：フィ\u30fcルドの種類を固定")]
	public int debugHoleFieldIdx;
	[SerializeField]
	[Header("フィ\u30fcルド")]
	private Golf_Hole_Field[] arrayHoleField;
	private Golf_Hole_Field holeField;
	[SerializeField]
	[Header("生成したフィ\u30fcルドを格納するル\u30fcト")]
	private Transform holeFieldRoot;
	[SerializeField]
	[Header("右側のティ\u30fcマ\u30fcク")]
	private GameObject rightTeeMark;
	[SerializeField]
	[Header("左側のティ\u30fcマ\u30fcク")]
	private GameObject leftTeeMark;
	private Vector3 readyCharaPos;
	private readonly float TEE_CENTER_TO_BALL_DISTANCE = 0.5f;
	private readonly float BALL_TO_CHARA_DISTANCE = 0.925f;
	private Vector3 readyBallPosToCupVec;
	private float readyBallPosToCupDistance;
	[SerializeField]
	[Header("生成するフィ\u30fcルドのGizmos用メッシュ")]
	private Mesh holeField_Gizmos;
	public void Init()
	{
		isDebugHoleField = false;
		int num = 0;
		do
		{
			num = UnityEngine.Random.Range(0, arrayHoleField.Length);
		}
		while (num == Golf_Define.BEFORE_HOLE_FIELD_IDX);
		Golf_Define.BEFORE_HOLE_FIELD_IDX = num;
		holeField = UnityEngine.Object.Instantiate(arrayHoleField[num], holeFieldRoot);
		Vector3 position = rightTeeMark.transform.position;
		position.y += 0.25f;
		Vector3 position2 = leftTeeMark.transform.position;
		position2.y += 0.25f;
		Vector3 vector = (position + position2) / 2f;
		SingletonCustom<Golf_FieldManager>.Instance.SetTeeMarkCenterPos(vector);
		Vector3 vector2 = vector - holeField.GetCupPos();
		vector2.y = 0f;
		Vector3 vector3 = vector + vector2.normalized * TEE_CENTER_TO_BALL_DISTANCE;
		if (Physics.Raycast(vector3, Vector3.down, out RaycastHit hitInfo, 1f))
		{
			vector3 = hitInfo.point;
			SingletonCustom<Golf_FieldManager>.Instance.SetTeePos(vector3 + new Vector3(0f, -0.1f, 0f));
		}
		vector2 = vector - vector3;
		vector3 += Quaternion.Euler(new Vector3(0f, -90f, 0f)) * (vector2.normalized * BALL_TO_CHARA_DISTANCE);
		Vector3 vector4;
		if (Physics.Raycast(vector3 + new Vector3(0f, 1f, 0f), Vector3.down, out hitInfo, 2f))
		{
			vector3 = hitInfo.point;
			vector4 = vector3;
			UnityEngine.Debug.Log("posTmp : " + vector4.ToString());
		}
		readyCharaPos = vector3;
		vector4 = readyCharaPos;
		UnityEngine.Debug.Log("charapos " + vector4.ToString());
		Vector3 readyBallPos = SingletonCustom<Golf_FieldManager>.Instance.GetReadyBallPos();
		readyBallPos.y = 0f;
		Vector3 cupPos = holeField.GetCupPos();
		cupPos.y = 0f;
		readyBallPosToCupVec = cupPos - readyBallPos;
		readyBallPosToCupDistance = SingletonCustom<Golf_FieldManager>.Instance.GetConversionYardDistance(CalcManager.Length(readyBallPos, cupPos));
	}
	public Vector3 GetCupPos()
	{
		return holeField.GetCupPos();
	}
	public float GetRequiredPower()
	{
		return holeField.GetRequiredPower();
	}
	public Vector3 GetReadyCharaPos()
	{
		return readyCharaPos;
	}
	public Vector3 GetReadyBallPosToCupVec()
	{
		return readyBallPosToCupVec;
	}
	public float GetReadyBallPosToCupDistance()
	{
		return readyBallPosToCupDistance;
	}
	private void OnDrawGizmos()
	{
		if (holeField_Gizmos != null)
		{
			Gizmos.DrawWireMesh(holeField_Gizmos, holeFieldRoot.position);
		}
		if (!(rightTeeMark == null) && !(leftTeeMark == null) && !(holeField == null))
		{
			Vector3 position = rightTeeMark.transform.position;
			position.y += 0.25f;
			Vector3 position2 = leftTeeMark.transform.position;
			position2.y += 0.25f;
			Gizmos.color = Color.black;
			Gizmos.DrawLine(position, position2);
			Vector3 vector = (position + position2) / 2f;
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(vector, 0.1f);
			Vector3 vector2 = vector + (vector - holeField.GetCupPos()).normalized * TEE_CENTER_TO_BALL_DISTANCE;
			Vector3 vector3 = vector2;
			Gizmos.DrawLine(vector, vector2);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(vector2, 0.1f);
			Gizmos.DrawRay(vector2, Vector3.down);
			if (Physics.Raycast(vector2, Vector3.down, out RaycastHit hitInfo, 2f))
			{
				vector2 = hitInfo.point;
				Vector3 center = vector2 + new Vector3(0f, -0.1f, 0f);
				Gizmos.color = Color.magenta;
				Gizmos.DrawWireSphere(center, 0.1f);
			}
			Vector3 vector4 = vector - vector2;
			Vector3 vector5 = vector2 + Quaternion.Euler(new Vector3(0f, -90f, 0f)) * (vector4.normalized * BALL_TO_CHARA_DISTANCE);
			if (Physics.Raycast(vector5 + new Vector3(0f, 1f, 0f), Vector3.down, out hitInfo, 2f))
			{
				vector5 = hitInfo.point;
				Gizmos.color = Color.white;
				Gizmos.DrawWireSphere(vector5, 0.1f);
			}
			Gizmos.DrawLine(new Vector3(vector5.x, vector3.y, vector5.z), vector3);
		}
	}
}
