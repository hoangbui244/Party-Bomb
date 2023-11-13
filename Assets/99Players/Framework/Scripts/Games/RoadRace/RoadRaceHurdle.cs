using UnityEngine;
public class RoadRaceHurdle : MonoBehaviour
{
	[SerializeField]
	private Rigidbody rigid;
	[SerializeField]
	private Collider[] collider;
	[SerializeField]
	private GameObject aiJumpObj;
	private void Update()
	{
		if (Mathf.Abs(base.transform.localEulerAngles.x - 270f) < 5f)
		{
			base.transform.SetLocalEulerAnglesX(270f);
			rigid.isKinematic = true;
			for (int i = 0; i < collider.Length; i++)
			{
				collider[i].enabled = false;
			}
			aiJumpObj.SetActive(value: false);
			base.enabled = false;
		}
	}
}
