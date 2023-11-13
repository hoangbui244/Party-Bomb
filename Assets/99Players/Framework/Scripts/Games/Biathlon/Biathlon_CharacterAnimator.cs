using UnityEngine;
public class Biathlon_CharacterAnimator : MonoBehaviour
{
	private static readonly int IsRunHash = Animator.StringToHash("IsRun");
	private static readonly int IsUphillHash = Animator.StringToHash("IsUphill");
	private static readonly int IsDownhillHash = Animator.StringToHash("IsDownhill");
	private static readonly int CycleOffset = Animator.StringToHash("CycleOffset");
	private static readonly int HardRunHash = Animator.StringToHash("HardRun");
	[SerializeField]
	private Animator animator;
	private Biathlon_Character playingCharacter;
	public float AnimationSpeed
	{
		get;
		set;
	} = 1f;
	public bool IsHardRun => animator.GetCurrentAnimatorStateInfo(0).IsName("HardRun");
	public void Init(Biathlon_Character character)
	{
		playingCharacter = character;
		float value = Mathf.Lerp(0f, 0.5f, UnityEngine.Random.value);
		animator.SetFloat(CycleOffset, value);
	}
	public void UpdateMethod()
	{
		bool isPlayer = playingCharacter.IsPlayer;
	}
	public void PlayRun()
	{
		animator.SetBool(IsRunHash, value: true);
	}
	public void StopRun()
	{
		animator.SetBool(IsRunHash, value: false);
	}
	public void EnterDownhill()
	{
		animator.SetBool(IsDownhillHash, value: true);
	}
	public void ExitDownhill()
	{
		animator.SetBool(IsDownhillHash, value: false);
	}
	public void EnterUphill()
	{
		animator.SetBool(IsUphillHash, value: true);
	}
	public void ExitUphill()
	{
		animator.SetBool(IsUphillHash, value: false);
	}
	public void PoseStandShooting()
	{
		animator.Play("StandShooting");
	}
	public void PoseLieDownShooting()
	{
		animator.Play("LieDownShooting");
	}
	public void StopPose()
	{
		animator.Play("Idle");
	}
}
