using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_Player : DecoratedMonoBehaviour
{
	[SerializeField]
	[DisplayName("レティクル")]
	private Shuriken_PlayerReticle reticle;
	[SerializeField]
	[DisplayName("手元の手裏剣")]
	private Shuriken_DisplayShuriken displayShuriken;
	[SerializeField]
	[DisplayName("手裏剣投げアクション")]
	private Shuriken_ThrowAction throwAction;
	[Header("デバッグ用UI")]
	[SerializeField]
	[Disable(true)]
	[DisplayName("プレイヤ\u30fc番号")]
	private int playerNo;
	[SerializeField]
	[Disable(true)]
	[DisplayName("操作ユ\u30fcザ\u30fc")]
	private Shuriken_Definition.ControlUser controlUser;
	public int PlayerNo => playerNo;
	public bool IsCpu => playerNo >= SingletonCustom<GameSettingManager>.Instance.PlayerNum;
	public Shuriken_Definition.ControlUser ControlUser => controlUser;
	public int Score
	{
		get;
		private set;
	}
	public void Initialize(int no, Shuriken_Definition.ControlUser user)
	{
		playerNo = no;
		controlUser = user;
		Score = 0;
		reticle.Initialize(no);
		displayShuriken.Initialize(no);
		throwAction.Initialize(no);
	}
	public void PostInitialize()
	{
		reticle.PostInitialize();
	}
	public void UpdateMethod()
	{
		reticle.UpdateMethod();
		throwAction.UpdateMethod();
	}
	public void AddScore(int score)
	{
		Score += score;
		SingletonMonoBehaviour<Shuriken_UI>.Instance.SetScore(Score, playerNo);
	}
}
