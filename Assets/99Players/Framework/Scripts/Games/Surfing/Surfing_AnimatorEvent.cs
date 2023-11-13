using UnityEngine;
public class Surfing_AnimatorEvent : MonoBehaviour
{
	[SerializeField]
	[Header("対象のSurfing_Player")]
	public Surfing_Player player;
	[SerializeField]
	[Header("水かきエフェクト(右手)")]
	private ParticleSystem psRight;
	[SerializeField]
	[Header("水かきエフェクト(左手)")]
	private ParticleSystem psLeft;
	private float sePitch;
	public void RightEffect()
	{
		psRight.Emit(10);
		if (player.UserType <= Surfing_Define.UserType.PLAYER_4)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_surfing_weber", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
		}
	}
	public void LeftEffect()
	{
		psLeft.Emit(10);
		if (player.UserType <= Surfing_Define.UserType.PLAYER_4)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_surfing_weber", _loop: false, 0f, 1f, 0.9f, 0f, _overlap: true);
		}
	}
}
