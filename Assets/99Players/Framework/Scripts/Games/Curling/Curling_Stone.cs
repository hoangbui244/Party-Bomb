using UnityEngine;
public class Curling_Stone : MonoBehaviour
{
	private Curling_GameManager.Team team;
	[SerializeField]
	[Header("Rigidbody")]
	private Rigidbody rigid;
	[SerializeField]
	[Header("石オブジェクト")]
	private GameObject stoneObj;
	[SerializeField]
	[Header("石オブジェクトの大きさの半分")]
	private float stoneObjectHalfSize;
	[SerializeField]
	[Header("MeshRenderer")]
	private MeshRenderer meshRenderer;
	private Transform originParent;
	[SerializeField]
	[Header("空気抵抗")]
	private float RIGID_DRAG;
	[SerializeField]
	[Header("空気抵抗が０になるための必要な入力回数")]
	private int DRAG_ZERO_NEED_INPUT_CNT;
	[SerializeField]
	[Header("投げる準備する時の座標")]
	private Vector3 prepForPos;
	[SerializeField]
	[Header("投げる準備をするときの角度")]
	private Vector3 prepForRot;
	[SerializeField]
	[Header("こするキャラのアンカ\u30fc")]
	private Transform[] arraySweepCharaAnchor;
	[SerializeField]
	[Header("こするキャラの追従をやめるアンカ\u30fc")]
	private Transform[] arraySweepMoveEndAnchor;
	private Vector3 curveDir;
	[SerializeField]
	[Header("曲がるパワ\u30fc（投げるキャラ用）")]
	private float curvePower_Throw;
	[SerializeField]
	[Header("曲がるパワ\u30fc（こするキャラ用）")]
	private float curvePower_Sweep;
	private bool isSweepCurve;
	[SerializeField]
	[Header("曲がり始めるまでの時間")]
	private float CURVE_START_TIME;
	private float curveStartTime;
	private bool isFailure;
	private Vector3 prevPos;
	private Vector3 nowPos;
	private float rotDir;
	[SerializeField]
	[Header("カ\u30fcブさせたときの回転速度")]
	private float CURVE_ROT_SPEED;
	[SerializeField]
	[Header("自然の回転速度")]
	private float FREE_ROT_SPEED;
	private int pointIdx;
	[SerializeField]
	[Range(0f, 1f)]
	[Header("他の石に接触した時の自身のmagnitudeの補正値")]
	private float colStoneDiffMag;
	[SerializeField]
	[Header("他の石に接触した時の相手の石に与える力")]
	private float colStonePower;
	private bool isThrow;
	public void Init(Curling_GameManager.Team _team)
	{
		team = _team;
		originParent = base.transform.parent;
		pointIdx = -1;
	}
	public void SetMaterial(Material _mat)
	{
		meshRenderer.material = _mat;
	}
	public void InitPlay()
	{
		stoneObj.transform.localEulerAngles = Vector3.zero;
		rigid.isKinematic = true;
		rigid.drag = RIGID_DRAG;
		base.transform.position = Vector3.zero;
		base.gameObject.SetActive(value: false);
		curveDir = Vector3.zero;
		curveStartTime = 0f;
		isFailure = false;
		rotDir = 0f;
		isSweepCurve = false;
	}
	public void UpdateMetthod()
	{
		prevPos = nowPos;
		if (rigid.velocity.magnitude < 0.01f)
		{
			rigid.velocity = Vector3.zero;
			curveDir = Vector3.zero;
			if (!isFailure && SingletonCustom<Curling_CurlingRinkManager>.Instance.GetToHogLine_Back() >= base.transform.position.z - stoneObjectHalfSize)
			{
				SetIsFailure();
			}
		}
		else
		{
			if (curveStartTime < CURVE_START_TIME)
			{
				curveStartTime += Time.deltaTime;
			}
			else if (isSweepCurve)
			{
				rigid.AddForce(curveDir * curvePower_Sweep * rigid.velocity.magnitude, ForceMode.Force);
			}
			else
			{
				rigid.AddForce(curveDir * curvePower_Throw * rigid.velocity.magnitude, ForceMode.Force);
			}
			if (curveDir != Vector3.zero)
			{
				rotDir = curveDir.x;
				stoneObj.transform.AddLocalEulerAnglesY(rotDir * rigid.velocity.magnitude * Time.deltaTime * CURVE_ROT_SPEED);
			}
			else
			{
				Vector3 vector = base.transform.position - prevPos;
				if (vector.x != 0f)
				{
					rotDir = Mathf.Sign(vector.x);
				}
				stoneObj.transform.AddLocalEulerAnglesY(rotDir * rigid.velocity.magnitude * Time.deltaTime * FREE_ROT_SPEED);
			}
			if (!isFailure && SingletonCustom<Curling_CurlingRinkManager>.Instance.GetToBackLine_Back() <= base.transform.position.z - stoneObjectHalfSize)
			{
				SetIsFailure();
			}
		}
		nowPos = base.transform.position;
	}
	public Curling_GameManager.Team GetTeam()
	{
		return team;
	}
	public Rigidbody GetRigid()
	{
		return rigid;
	}
	public Vector3 RigidVelocity()
	{
		return rigid.velocity;
	}
	public float GetStoneObjectHalfSize()
	{
		return stoneObjectHalfSize;
	}
	public void SetRigidDrag(Vector3 _stickDir, int _cnt)
	{
		if (_stickDir != Vector3.zero)
		{
			isSweepCurve = true;
			curveDir = _stickDir;
		}
		_cnt = Mathf.Clamp(_cnt, 0, DRAG_ZERO_NEED_INPUT_CNT);
		int num = (DRAG_ZERO_NEED_INPUT_CNT - _cnt) / DRAG_ZERO_NEED_INPUT_CNT;
		rigid.drag = RIGID_DRAG * (float)num;
	}
	public Transform[] GetArraySweepCharaAnchor()
	{
		return arraySweepCharaAnchor;
	}
	public Transform[] GetArraySweepMoveEndAnchor()
	{
		return arraySweepMoveEndAnchor;
	}
	public bool GetIsFailure()
	{
		return isFailure;
	}
	public void SetIsFailure()
	{
		isFailure = true;
	}
	public bool GetIsThrow()
	{
		return isThrow;
	}
	public bool CheckSameBeforePointIdx(int _pointIdx)
	{
		return pointIdx == _pointIdx;
	}
	public void SetPointIdx(int _pointIdx)
	{
		pointIdx = _pointIdx;
	}
	public void PrepForThrow(Transform _parent)
	{
		base.transform.parent = _parent;
		base.transform.localPosition = prepForPos;
		base.transform.localEulerAngles = prepForRot;
		base.gameObject.SetActive(value: true);
	}
	public void Throw(Vector3 _curveDir, Vector3 _dir, float _power)
	{
		rigid.isKinematic = false;
		base.transform.parent = originParent;
		rigid.AddForce(_dir * _power, ForceMode.Impulse);
		curveDir = _curveDir;
		isThrow = true;
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Ball")
		{
			Curling_Stone component = collision.gameObject.GetComponent<Curling_Stone>();
			if (component != null)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_curling_stone");
				Vector3 force = (component.transform.position - base.transform.position).normalized * rigid.velocity.magnitude * (1f + colStoneDiffMag) * colStonePower;
				component.GetRigid().AddForce(force, ForceMode.Impulse);
			}
		}
	}
}
