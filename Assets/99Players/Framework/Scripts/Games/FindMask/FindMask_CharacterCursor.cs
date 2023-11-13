using UnityEngine;
public class FindMask_CharacterCursor : MonoBehaviour
{
	[SerializeField]
	[Header("キャラスタイル")]
	private CharacterStyle charaStyle;
	[SerializeField]
	[Header("カ\u30fcソルテクスチャ")]
	private Texture[] charaCursors;
	[SerializeField]
	[Header("カ\u30fcソルのメッシュレンダラ\u30fc")]
	private MeshRenderer cursorMeshRenderer;
	[SerializeField]
	[Header("選択判定用のレイを出す位置")]
	private Transform rayStartTrans;
	[SerializeField]
	[Header("カ\u30fcソルの移動可能範囲")]
	private Transform[] cursorLimit;
	private FindMask_MaskData nowSelectMask;
	private FindMask_MaskData lastSelectMask;
	private int cursorRayDistance = 20;
	private float CURSOR_SPEED = 40f;
	public void Init()
	{
		base.transform.localPosition = Vector3.zero;
		SetCharaCursors(SingletonCustom<FindMask_GameManager>.Instance.ArrayPlayerElement[SingletonCustom<FindMask_GameManager>.Instance.CurrentTurnNum]);
	}
	public void CursorMove(Vector3 _moveDir)
	{
		Vector3 a = _moveDir * CURSOR_SPEED;
		base.transform.localPosition += a * Time.deltaTime;
		Vector3 position = cursorLimit[0].transform.position;
		Vector3 position2 = cursorLimit[1].transform.position;
		Vector3 position3 = base.transform.position;
		position3.x = Mathf.Clamp(position3.x, position.x, position2.x);
		position3.y = Mathf.Clamp(position3.y, position.y, position2.y);
		base.transform.position = position3;
	}
	public void SetCursorActive(bool _active)
	{
		base.gameObject.SetActive(_active);
	}
	public void SetCharaCursors(int _charaNo)
	{
		cursorMeshRenderer.material.SetTexture("_MainTex", charaCursors[_charaNo]);
	}
	public GameObject GetCursor()
	{
		return base.gameObject;
	}
	public Vector3 GetCursorPos()
	{
		return base.transform.localPosition;
	}
	public FindMask_MaskData GetSelectMaskeObj()
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(new Ray(rayStartTrans.position, Vector3.forward), out hitInfo, cursorRayDistance) && hitInfo.transform.GetComponent<FindMask_MaskData>() != null && !hitInfo.transform.GetComponent<FindMask_MaskData>().IsSelect)
		{
			return hitInfo.transform.GetComponent<FindMask_MaskData>();
		}
		return null;
	}
	public int GetSelectMaskeObjNum()
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(new Ray(rayStartTrans.position, Vector3.forward), out hitInfo, cursorRayDistance) && hitInfo.transform.GetComponent<FindMask_MaskData>() != null && !hitInfo.transform.GetComponent<FindMask_MaskData>().IsFindPair)
		{
			return hitInfo.transform.GetComponent<FindMask_MaskData>().ObjNo;
		}
		return -1;
	}
}
