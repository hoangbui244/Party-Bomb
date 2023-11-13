using UnityEngine;
public class Curling_PredictStone : MonoBehaviour
{
	[SerializeField]
	[Header("予測する石")]
	private Curling_Stone predictStone;
	[SerializeField]
	[Header("予測する石のマテリアル")]
	private Material predictMat;
	[SerializeField]
	[Header("最初の座標（補正）")]
	private Vector3 originPos_diff;
	[SerializeField]
	[Header("予測する時間")]
	private float predictTime;
	[SerializeField]
	[Header("fixedDeltaTime補正")]
	private float fixedDeltaTime_Diff;
	[SerializeField]
	[Header("予測するパワ\u30fc値")]
	private float[] predictThrowPower;
	private Vector3 originPos;
	private Vector3[] maxDistancePos = new Vector3[2];
	public void Init()
	{
		Physics.autoSimulation = false;
		for (int i = 0; i < 2; i++)
		{
			predictStone.GetRigid().velocity = Vector3.zero;
			predictStone.GetRigid().angularVelocity = Vector3.zero;
			predictStone.GetRigid().position = SingletonCustom<Curling_GameManager>.Instance.GetThrowStone().transform.position + SingletonCustom<Curling_GameManager>.Instance.GetThrowPlayer().transform.forward * SingletonCustom<Curling_GameManager>.Instance.GetThrowMoveDistance() + originPos_diff;
			originPos = predictStone.GetRigid().position;
			predictStone.GetRigid().AddForce(SingletonCustom<Curling_GameManager>.Instance.GetThrowPlayer().transform.forward * (SingletonCustom<Curling_GameManager>.Instance.GetStoneThrowBasePower() + SingletonCustom<Curling_GameManager>.Instance.GetStoneThrowDiffMaxPower() * predictThrowPower[i]), ForceMode.Impulse);
			for (float num = 0f; num < predictTime; num += Time.fixedUnscaledDeltaTime * fixedDeltaTime_Diff)
			{
				Physics.Simulate(Time.fixedUnscaledDeltaTime * fixedDeltaTime_Diff);
			}
			maxDistancePos[i] = predictStone.transform.position;
		}
		base.gameObject.SetActive(value: false);
		Physics.autoSimulation = true;
	}
	public Vector3 GetOriginPos()
	{
		return originPos;
	}
	public Vector3 GetMaxPos(int _idx)
	{
		return maxDistancePos[_idx];
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		for (int i = 0; i < 2; i++)
		{
			Gizmos.DrawWireSphere(originPos, 0.25f);
			Gizmos.DrawWireSphere(maxDistancePos[i], 0.25f);
			Gizmos.DrawLine(originPos, maxDistancePos[i]);
		}
	}
}
