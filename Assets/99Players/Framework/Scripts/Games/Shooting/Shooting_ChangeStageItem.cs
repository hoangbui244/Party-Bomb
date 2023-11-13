using UnityEngine;
public class Shooting_ChangeStageItem : MonoBehaviour
{
	[SerializeField]
	[Header("3dカメラ")]
	private Camera worldCamera;
	[SerializeField]
	[Header("消しゴムくん")]
	private GameObject objKeshigomuKun;
	[SerializeField]
	[Header("いちごちゃん")]
	private GameObject objIchigoChan;
	[SerializeField]
	[Header("プレイヤ\u30fc")]
	private Shooting_Controller[] arrayPlayerController;
	[SerializeField]
	[Header("プレイヤ\u30fcの照準Z軸アンカ\u30fc")]
	private Transform controllerLookZAnchor;
	public Camera WorldCamera => worldCamera;
	public GameObject ObjKeshigomuKun => objKeshigomuKun;
	public GameObject ObjIchigoChan => objIchigoChan;
	public Shooting_Controller[] ArrayPlayerController => arrayPlayerController;
	public Transform ControllerLookZAnchor => controllerLookZAnchor;
}
