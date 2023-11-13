using UnityEngine;
public class SmartBall_GimmickSwitch : MonoBehaviour
{
	private bool gimmickStartUp;
	public bool GimmickStartUp => gimmickStartUp;
	public void EndGimmickMove()
	{
		gimmickStartUp = false;
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.name == "SmartBall_Ball(Clone)")
		{
			gimmickStartUp = true;
		}
	}
	private void OnCollisionStay(Collision collision)
	{
		if (!gimmickStartUp && collision.transform.name == "SmartBall_Ball(Clone)")
		{
			gimmickStartUp = true;
		}
	}
	private void OnCollisionExit(Collision collision)
	{
		if (collision.transform.name == "SmartBall_Ball(Clone)")
		{
			gimmickStartUp = false;
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.transform.name == "SmartBall_Ball(Clone)")
		{
			gimmickStartUp = true;
		}
	}
}
