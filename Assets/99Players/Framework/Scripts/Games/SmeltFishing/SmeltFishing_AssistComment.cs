using System.Collections;
using UnityEngine;
using UnityEngine.Extension;
public class SmeltFishing_AssistComment : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer[] assistComments;
	[SerializeField]
	[DisplayName("フェ\u30fcドイン時間")]
	private float fadeInTime = 0.05f;
	[SerializeField]
	[DisplayName("表示時間")]
	private float displayDuration = 3f;
	[SerializeField]
	[DisplayName("フェ\u30fcドアウト時間")]
	private float fadeOutTime = 0.05f;
	private SpriteRenderer current;
	private bool[] assistCommentsShown;
	private bool isPlaying;
	public void Init()
	{
		SpriteRenderer[] array = assistComments;
		foreach (SpriteRenderer spriteRenderer in array)
		{
			if (!(spriteRenderer == null))
			{
				spriteRenderer.SetAlpha(0f);
				spriteRenderer.enabled = false;
			}
		}
		assistCommentsShown = new bool[assistComments.Length];
		current = null;
	}
	public void ShowAssistComment(int assistIndex, bool isForceHide = false)
	{
		if (current != null || assistCommentsShown[assistIndex])
		{
			return;
		}
		if (assistIndex >= assistComments.Length)
		{
			UnityEngine.Debug.LogError($"指定されたアシストコメントのインデックスが範囲外です index:{assistIndex}");
			return;
		}
		assistCommentsShown[assistIndex] = true;
		current = assistComments[assistIndex];
		if (current != null)
		{
			current.enabled = true;
			StartCoroutine(DisplayAssistCommentWithFading(current, isForceHide));
		}
	}
	public void HideAssistComment()
	{
		if (!(current == null))
		{
			StartCoroutine(DisplayAssistCommentWithHideFading(current));
		}
	}
	public void ForceHideAssistComment()
	{
		if (current != null)
		{
			StopAllCoroutines();
			current.enabled = false;
			current = null;
		}
	}
	public bool IsOnceAssistComment(int assistIndex)
	{
		UnityEngine.Debug.Log("assistCommentsShown[" + assistIndex.ToString() + "] : " + assistCommentsShown[assistIndex].ToString());
		return assistCommentsShown[assistIndex];
	}
	private IEnumerator DisplayAssistCommentWithFading(SpriteRenderer comment, bool isForceHide)
	{
		float elapsed2 = 0f;
		while (elapsed2 < fadeInTime)
		{
			elapsed2 += Time.deltaTime;
			comment.SetAlpha(elapsed2 / fadeInTime);
			yield return null;
		}
		comment.SetAlpha(1f);
		if (!isForceHide)
		{
			yield return new WaitForSeconds(displayDuration);
			elapsed2 = 0f;
			while (elapsed2 < fadeOutTime)
			{
				elapsed2 += Time.deltaTime;
				comment.SetAlpha(1f - elapsed2 / fadeOutTime);
				yield return null;
			}
			comment.SetAlpha(0f);
			comment.enabled = false;
			current = null;
		}
	}
	private IEnumerator DisplayAssistCommentWithHideFading(SpriteRenderer comment)
	{
		float elapsed = 0f;
		while (elapsed < fadeOutTime)
		{
			elapsed += Time.deltaTime;
			comment.SetAlpha(1f - elapsed / fadeOutTime);
			yield return null;
		}
		comment.SetAlpha(0f);
		comment.enabled = false;
		current = null;
	}
}
