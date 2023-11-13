using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_ThrowAction : DecoratedMonoBehaviour
{
	[SerializeField]
	[DisplayName("レティクル")]
	private Shuriken_PlayerReticle reticle;
	[SerializeField]
	[DisplayName("手元の手裏剣")]
	private Shuriken_DisplayShuriken displayShuriken;
	[SerializeField]
	[DisplayName("コンフィグ")]
	private Shuriken_ThrowingShurikenConfig config;
	private int playerNo;
	private Shuriken_Definition.ControlUser controlUser;
	private int throwCount;
	private float nextTime;
	public void Initialize(int no)
	{
		playerNo = no;
		nextTime = 0f;
		controlUser = SingletonMonoBehaviour<Shuriken_GameMain>.Instance.ControlUsers[playerNo];
	}
	public void UpdateMethod()
	{
		switch (controlUser)
		{
		case Shuriken_Definition.ControlUser.Player1:
		case Shuriken_Definition.ControlUser.Player2:
		case Shuriken_Definition.ControlUser.Player3:
		case Shuriken_Definition.ControlUser.Player4:
			ThrowForPlayer(controlUser);
			break;
		case Shuriken_Definition.ControlUser.Cpu1:
		case Shuriken_Definition.ControlUser.Cpu2:
		case Shuriken_Definition.ControlUser.Cpu3:
		case Shuriken_Definition.ControlUser.AutoPlay:
			ThrowForAI();
			break;
		}
	}
	public float GetTimeRequiredToReach(Vector3 point)
	{
		return Vector3.Distance(displayShuriken.Position, point) / config.ThrowPower;
	}
	private void ThrowForPlayer(Shuriken_Definition.ControlUser user)
	{
		if (!(nextTime > Time.time) && SingletonMonoBehaviour<Shuriken_Input>.Instance.IsPressDownButtonA(user))
		{
			nextTime = Time.time + config.ThrowInterval;
			Throw();
		}
	}
	private void ThrowForAI()
	{
		if (!(nextTime > Time.time) && reticle.IsAimTarget())
		{
			Throw();
			nextTime = Time.time + config.ThrowInterval;
		}
	}
	private void Throw()
	{
		if (playerNo < SingletonCustom<GameSettingManager>.Instance.PlayerNum)
		{
			SingletonCustom<HidVibration>.Instance.SetCommonVibration(playerNo);
		}
		if (Physics.Raycast(reticle.ScreenPointToRay(), out RaycastHit hitInfo, 60f))
		{
			Vector3 point = hitInfo.point;
			ThrowShuriken(displayShuriken.Position, point);
			reticle.DiscardTargetIfNeeded();
		}
		else
		{
			UnityEngine.Debug.LogError("何にも当たらなかった");
		}
		SingletonMonoBehaviour<Shuriken_Audio>.Instance.PlaySfx(config.ThrowSfxNames[throwCount % config.ThrowSfxNames.Length], playerNo);
		throwCount++;
		displayShuriken.Hide();
		this.CoffeeBreak().DelayCall(config.ThrowInterval * 0.5f, delegate
		{
			displayShuriken.Show(config.ThrowInterval * 0.5f);
		}).Start();
	}
	private void ThrowShuriken(Vector3 origin, Vector3 point)
	{
		SingletonMonoBehaviour<Shuriken_ThrowingShurikenCache>.Instance.GetShuriken().Throw(throwVector: (point - origin).normalized, playerNo: playerNo, originPoint: origin);
	}
}
