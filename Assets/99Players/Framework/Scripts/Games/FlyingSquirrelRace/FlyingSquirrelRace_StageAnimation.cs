using System.Collections;
using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_StageAnimation : MonoBehaviour
{
	[SerializeField]
	private Transform startStand;
	[SerializeField]
	private Transform endStand;
	[SerializeField]
	private FlyingSquirrelRace_Backgrounds backgrounds;
	[SerializeField]
	private float scrollSpeed = 1f;
	[SerializeField]
	private float jumpTime = 0.5f;
	[SerializeField]
	private float jumpHeight = 0.3f;
	[SerializeField]
	private float jumpScrollSpeed = 1f;
	[SerializeField]
	private float fallTime = 0.5f;
	[SerializeField]
	private float fallHeight = 2f;
	[SerializeField]
	private float breakingFactor = 1f;
	[SerializeField]
	private float fallSpeedWhenGoal = 1f;
	private float bgScrollSpeed = 2f;
	private bool endStandMoving;
	public void Initialize()
	{
		backgrounds.transform.LocalPosition((Vector3 v) => v.Y((float y) => y - 2f));
		endStand.LocalPosition((Vector3 v) => v.Y(0f));
	}
	public IEnumerator PlayStartAnimation()
	{
		float elapsed2 = 0f;
		while (elapsed2 < jumpTime)
		{
			elapsed2 += Time.deltaTime;
			float y = Mathf.PingPong(Mathf.Clamp01(elapsed2 / jumpTime), 0.5f) * jumpHeight * 2f;
			startStand.LocalPosition((Vector3 v) => v.X((float x) => x - jumpScrollSpeed * Time.deltaTime));
			startStand.LocalPosition((Vector3 v) => v.Y(0f - y));
			backgrounds.UpdateMethod(scrollSpeed);
			yield return null;
		}
		elapsed2 = 0f;
		while (elapsed2 < fallTime)
		{
			elapsed2 += Time.deltaTime;
			float t = elapsed2 / fallTime;
			float y2 = Mathf.Lerp(0f, fallTime, t);
			startStand.LocalPosition((Vector3 v) => v.Y(y2));
			startStand.LocalPosition((Vector3 v) => v.X((float x) => x - 2f * Time.deltaTime));
			backgrounds.transform.LocalPosition((Vector3 v) => v.Y(Mathf.Lerp(-2f, 0f, t)));
			backgrounds.UpdateMethod(scrollSpeed * 2f);
			yield return null;
		}
		backgrounds.transform.LocalPosition((Vector3 v) => v.Y(0f));
		while (SingletonMonoBehaviour<FlyingSquirrelRace_GameMain>.Instance.GameState != FlyingSquirrelRace_Definition.GameState.DuringGame)
		{
			startStand.LocalPosition((Vector3 v) => v.X((float x) => x - 2f * Time.deltaTime));
			backgrounds.UpdateMethod(scrollSpeed);
			yield return null;
		}
		elapsed2 = 0f;
		while (elapsed2 < 3f)
		{
			elapsed2 += Time.deltaTime;
			startStand.LocalPosition((Vector3 v) => v.X((float x) => x - 2f * Time.deltaTime));
			yield return null;
		}
		startStand.gameObject.SetActive(value: false);
	}
	public void PlayEndStandPreGoalAnimation()
	{
		if (!endStandMoving)
		{
			endStandMoving = true;
			StartCoroutine(PlayEndStandPreGoalAnimationInternal());
		}
	}
	private IEnumerator PlayEndStandPreGoalAnimationInternal()
	{
		while (endStandMoving)
		{
			endStand.LocalPosition((Vector3 v) => v.X((float x) => x - bgScrollSpeed * Time.deltaTime));
			yield return null;
		}
	}
	public void StopEndStandPreGoalAnimation()
	{
		endStandMoving = false;
	}
	public IEnumerator PlayPreGoalAnimation()
	{
		FlyingSquirrelRace_Stage component = GetComponent<FlyingSquirrelRace_Stage>();
		yield return component.TargetBaseSpeed();
	}
	public IEnumerator PlayGoalAnimation0()
	{
		Vector3 endPosition = endStand.localPosition;
		while (endPosition.x > 1f)
		{
			endStand.LocalPosition((Vector3 v) => v.X((float x) => x - bgScrollSpeed * Time.deltaTime));
			endPosition = endStand.localPosition;
			yield return null;
		}
		GetComponent<FlyingSquirrelRace_Stage>().StopStageScroll();
	}
	public IEnumerator PlayGoalAnimation1()
	{
		bool loop = true;
		while (loop)
		{
			loop = false;
			if (bgScrollSpeed > 0f)
			{
				bgScrollSpeed -= 2f * Time.deltaTime * breakingFactor;
				endStand.LocalPosition((Vector3 v) => v.X((float x) => x - bgScrollSpeed * Time.deltaTime));
				loop = true;
			}
			else
			{
				bgScrollSpeed = 0f;
			}
			if (endStand.localPosition.y < 0.75f)
			{
				endStand.LocalPosition((Vector3 v) => v.Y((float y) => y + 0.75f * Time.deltaTime * fallSpeedWhenGoal));
				backgrounds.UpdateGoalAnimation(0.5f * Time.deltaTime * fallSpeedWhenGoal);
				loop = ((byte)((loop ? 1 : 0) | 1) != 0);
			}
			yield return null;
		}
	}
	private IEnumerator BackgroundsUpdate()
	{
		while (SingletonMonoBehaviour<FlyingSquirrelRace_GameMain>.Instance.GameState != FlyingSquirrelRace_Definition.GameState.DuringGame)
		{
			backgrounds.UpdateMethod(bgScrollSpeed);
			yield return null;
		}
	}
}
