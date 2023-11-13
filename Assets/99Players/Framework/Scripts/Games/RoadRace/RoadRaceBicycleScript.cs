using UnityEngine;
public class RoadRaceBicycleScript : MonoBehaviour
{
	public enum BicycleParts
	{
		BASE,
		HANDLE,
		BRAKE_LEFT,
		BRAKE_RIGHT,
		WHEEL_FRONT,
		WHEEL_BACK,
		CRANK_LEFT,
		CRANK_RIGHT,
		PEDAL_LEFT,
		PEDAL_RIGHT,
		MAX
	}
	[SerializeField]
	[Header("自転車本体を設定")]
	public Transform baseBicycle;
	private MeshRenderer[] renderers;
	[SerializeField]
	[Header("ハンドルを設定")]
	public Transform handle;
	[SerializeField]
	[Header("持ち手アンカ\u30fc")]
	private Transform handleAnchor;
	[SerializeField]
	[Header("ブレ\u30fcキを設定")]
	public Transform[] brakes;
	[SerializeField]
	[Header("タイヤを設定")]
	public Transform[] wheels;
	[SerializeField]
	[Header("クランクを設定")]
	public Transform[] cranks;
	[SerializeField]
	[Header("ペダルを設定")]
	public Transform[] pedals;
	[SerializeField]
	[Header("ペダルアンカ\u30fcを設定")]
	public Transform[] pedalAnchors;
	[SerializeField]
	[Header("キャラの両足を設定")]
	public Transform[] charaFoots;
	[SerializeField]
	[Header("自転車の中心座標を設定")]
	public Transform centerAnchor;
	[SerializeField]
	[Header("自転車の乗る座標を設定")]
	private Transform rideAnchor;
	[SerializeField]
	[Header("自転車の長さを設定")]
	private float length;
	public MeshRenderer[] Renderers
	{
		get
		{
			if (renderers == null || renderers.Length == 0)
			{
				renderers = new MeshRenderer[10];
				for (int i = 0; i < renderers.Length; i++)
				{
					renderers[i] = GetBicycleTransform((BicycleParts)i).GetComponent<MeshRenderer>();
				}
			}
			return renderers;
		}
	}
	public Transform HandleAnchor => handleAnchor;
	public float Length => length * base.transform.localScale.z;
	public void SetMaterial(Material _mat)
	{
		for (int i = 0; i < Renderers.Length; i++)
		{
			Renderers[i].material = _mat;
		}
	}
	public float GetLengthData()
	{
		return length;
	}
	public Vector3 GetCenterAnchor()
	{
		return centerAnchor.position;
	}
	public Vector3 GetRideAnchor()
	{
		return rideAnchor.position;
	}
	public Transform GetBicycleTransform(BicycleParts _bicycleParts)
	{
		switch (_bicycleParts)
		{
		case BicycleParts.BASE:
			return baseBicycle;
		case BicycleParts.HANDLE:
			return handle;
		case BicycleParts.BRAKE_LEFT:
			return brakes[0];
		case BicycleParts.BRAKE_RIGHT:
			return brakes[1];
		case BicycleParts.WHEEL_FRONT:
			return wheels[0];
		case BicycleParts.WHEEL_BACK:
			return wheels[1];
		case BicycleParts.CRANK_LEFT:
			return cranks[0];
		case BicycleParts.CRANK_RIGHT:
			return cranks[1];
		case BicycleParts.PEDAL_LEFT:
			return pedals[0];
		case BicycleParts.PEDAL_RIGHT:
			return pedals[1];
		default:
			return baseBicycle;
		}
	}
}
