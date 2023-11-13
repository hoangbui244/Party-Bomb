using UnityEngine;
public class MakingPotion_1PCameraSetting : MonoBehaviour
{
	[SerializeField]
	[Header("シングル時のロ\u30fcカル位置と角度(Euler)")]
	private Vector3 singleLocalPos;
	[SerializeField]
	private Vector3 singleLocalEuler;
	[SerializeField]
	[Header("マルチ時のロ\u30fcカル位置と角度(Euler)")]
	private Vector3 multiLocalPos;
	[SerializeField]
	private Vector3 multiLocalEuler;
	private void Start()
	{
		if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
		{
			ChangeSingleTrans();
		}
		else
		{
			ChangeMultiTrans();
		}
	}
	public void ChangeSingleTrans()
	{
		base.transform.localPosition = singleLocalPos;
		base.transform.localEulerAngles = singleLocalEuler;
	}
	public void ChangeMultiTrans()
	{
		base.transform.localPosition = multiLocalPos;
		base.transform.localEulerAngles = multiLocalEuler;
	}
	public void ChangeSingleRect()
	{
		Camera component = GetComponent<Camera>();
		component.rect = MakingPotion_GameManager.SINGLE_CAMERA_RECT[0];
		component.depth = 1f;
	}
	public void ChangeMultiRect()
	{
		Camera component = GetComponent<Camera>();
		component.rect = MakingPotion_GameManager.MULTI_CAMERA_RECT[0];
		component.depth = 0f;
	}
}
