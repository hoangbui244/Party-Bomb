using UnityEngine;
using UnityEngine.UI;
public class Hidden_LoopRawImage : MonoBehaviour
{
	public RawImage mFade;
	private void Update()
	{
		if (mFade.gameObject.activeSelf)
		{
			mFade.uvRect = new Rect((mFade.uvRect.x + Time.unscaledDeltaTime * 0.04f) % 1f, (mFade.uvRect.y + Time.unscaledDeltaTime * 0.04f) % 1f, mFade.uvRect.width, mFade.uvRect.height);
		}
	}
}
