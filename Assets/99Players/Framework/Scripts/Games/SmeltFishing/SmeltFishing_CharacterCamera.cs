using System.Collections;
using UnityEngine;
public class SmeltFishing_CharacterCamera : MonoBehaviour
{
	private enum State
	{
		Follow,
		Fixed
	}
	private static readonly Rect[] SinglePlayerRects = new Rect[4]
	{
		new Rect(0f, 0f, 1f, 1f),
		new Rect(0.667f, 0.667f, 0.333f, 0.333f),
		new Rect(0.667f, 0.3334f, 0.333f, 0.333f),
		new Rect(0.667f, 0f, 0.333f, 0.333f)
	};
	private static readonly Rect[] SinglePlayerWithCpuRects = new Rect[4]
	{
		new Rect(0f, 0f, 0.666f, 1f),
		new Rect(0.667f, 0.667f, 0.333f, 0.333f),
		new Rect(0.667f, 0.334f, 0.333f, 0.333f),
		new Rect(0.667f, 0f, 0.333f, 0.333f)
	};
	private static readonly Rect[] MultiPlayerRects = new Rect[4]
	{
		new Rect(0f, 0.5f, 0.5f, 0.5f),
		new Rect(0.5f, 0.5f, 0.5f, 0.5f),
		new Rect(0f, 0f, 0.5f, 0.5f),
		new Rect(0.5f, 0f, 0.5f, 0.5f)
	};
	[SerializeField]
	private Camera camera;
	[SerializeField]
	private SmeltFishing_CharacterCameraConfig config;
	private SmeltFishing_Character playingCharacter;
	private Transform playerTransform;
	private Vector3 targetPosition;
	private State state;
	public bool InTransition
	{
		get;
		private set;
	}
	public void Init(SmeltFishing_Character character)
	{
		state = State.Follow;
		playingCharacter = character;
		playerTransform = playingCharacter.transform;
		UpdateViewPort();
		FollowCharacterImmediate();
	}
	private void FollowCharacterImmediate()
	{
		Vector3 perspectiveCameraPosition = config.PerspectiveCameraPosition;
		targetPosition = playerTransform.position + perspectiveCameraPosition;
		base.transform.position = targetPosition;
	}
	public void FollowCharacter()
	{
		Vector3 perspectiveCameraPosition = config.PerspectiveCameraPosition;
		targetPosition = playerTransform.position + perspectiveCameraPosition;
		base.transform.position = Vector3.Lerp(base.transform.position, targetPosition, config.FollowDistanceDelta * Time.deltaTime);
	}
	public void UpdateViewPort()
	{
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			camera.rect = SinglePlayerWithCpuRects[playingCharacter.PlayerNo];
		}
		else
		{
			camera.rect = MultiPlayerRects[playingCharacter.PlayerNo];
		}
	}
	public void SetAsFollowCamera()
	{
		state = State.Follow;
		StartCoroutine(TransitionToFollowCamera());
	}
	public void SetAsFixedCamera()
	{
		state = State.Fixed;
		StartCoroutine(TransitionToFixedCamera());
	}
	private IEnumerator TransitionToFollowCamera()
	{
		InTransition = true;
		Vector3 endPos = playerTransform.position + config.PerspectiveCameraPosition;
		Vector3 endAngles = config.PerspectiveCameraEulerAngles;
		Vector3 startPos = base.transform.position;
		Vector3 startAngles = base.transform.localEulerAngles;
		float elapsed = 0f;
		while (elapsed < config.TransitionDuration)
		{
			elapsed += Time.deltaTime;
			float t = Mathf.Clamp01(elapsed / config.TransitionDuration);
			base.transform.position = Vector3.Lerp(startPos, endPos, t);
			base.transform.localEulerAngles = Vector3.Lerp(startAngles, endAngles, t);
			yield return null;
		}
		base.transform.position = endPos;
		base.transform.localEulerAngles = endAngles;
		InTransition = false;
	}
	private IEnumerator TransitionToFixedCamera()
	{
		InTransition = true;
		Vector3 endPos = playerTransform.position + config.ContactCameraPosition;
		Vector3 endAngles = config.ContactCameraEulerAngles;
		Vector3 startPos = base.transform.position;
		Vector3 startAngles = base.transform.localEulerAngles;
		float elapsed = 0f;
		while (elapsed < config.TransitionDuration)
		{
			elapsed += Time.deltaTime;
			float t = Mathf.Clamp01(elapsed / config.TransitionDuration);
			base.transform.position = Vector3.Lerp(startPos, endPos, t);
			base.transform.localEulerAngles = Vector3.Lerp(startAngles, endAngles, t);
			yield return null;
		}
		base.transform.position = endPos;
		base.transform.localEulerAngles = endAngles;
		InTransition = false;
	}
	public float ConfigTransitionDuration()
	{
		return config.TransitionDuration;
	}
}
