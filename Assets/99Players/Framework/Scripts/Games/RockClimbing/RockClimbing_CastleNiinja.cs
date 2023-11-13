using UnityEngine;
public class RockClimbing_CastleNiinja : MonoBehaviour
{
	private RockClimbing_Character character;
	[SerializeField]
	[Header("障害物を持つ部分")]
	private Transform haveObstacle;
	public void Init()
	{
		character = GetComponent<RockClimbing_Character>();
		character.Init(null);
	}
	public Transform GetHaveObstacle()
	{
		return haveObstacle;
	}
	public void SetMaterial(Material _mat)
	{
		character.SetNinjaMaterial(_mat);
	}
	public void SetThrow(Vector3 _pos, Vector3 _target)
	{
		base.transform.position = _pos;
		Vector3 forward = _target - base.transform.position;
		forward.y = 0f;
		base.transform.rotation = Quaternion.LookRotation(forward);
	}
	public void ReadyThrowAnimation(float _animTime)
	{
		character.NinjaReadyThrowAnimation(_animTime);
	}
	public void ThrowAnimation(float _animTime)
	{
		character.NinjaThrowAnimation(_animTime);
	}
	public void ResetAnimation(float _animTime)
	{
		character.ResetAnimation(_animTime);
	}
}
