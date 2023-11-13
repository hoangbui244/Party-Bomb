using UnityEngine;
public class Golf_ViewPlayerIcon : MonoBehaviour
{
	private void OnTriggerEnter(Collider col)
	{
		if (SingletonCustom<Golf_GameManager>.Instance.GetState() == Golf_GameManager.State.BALL_FLY && SingletonCustom<Golf_GameManager>.Instance.GetGameCnt() + 1 != 1)
		{
			UnityEngine.Debug.Log("col.gameObject " + col.gameObject.name);
			if (col.gameObject.layer == LayerMask.NameToLayer("Ball"))
			{
				SingletonCustom<Golf_CursorManager>.Instance.SetAllPlayerIconActive(_isFade: true, _isActive: true);
			}
		}
	}
}
