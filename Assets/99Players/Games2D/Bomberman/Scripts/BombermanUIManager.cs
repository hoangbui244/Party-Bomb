using UnityEngine;

namespace io.ninenine.players.party2d.games.bomberman {
	/// <summary>
	/// 
	/// </summary>
	public class BombermanUIManager : SingletonCustom<BombermanUIManager> {
		/// <summary>
		/// 
		/// </summary>
		[SerializeField]
		private CommonUIManager commonUIManager;
		/// <summary>
		/// 
		/// </summary>
		private bool isShowSkip;
		/// <summary>
		/// 
		/// </summary>
		public bool IsShowSkip => isShowSkip;
		/// <summary>
		/// 
		/// </summary>
		public void Init() {
			commonUIManager.Init(CommonUIManager.UIType.SideUI_Rank);
			commonUIManager.SetTime(SingletonCustom<BombermanGameManager>.Instance.GameTime);
			commonUIManager.AddButtonExplanation(0, SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.BLOW_AWAY_TANK, 0), CommonButtonExplanationUI.ButtonType.None, CommonButtonExplanationUI.ButtonType.LStick);
			commonUIManager.AddButtonExplanation(1, SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.BLOW_AWAY_TANK, 1), CommonButtonExplanationUI.ButtonType.None, CommonButtonExplanationUI.ButtonType.A);
			commonUIManager.SetButtonExplanation();
			commonUIManager.SetButtonExplanation();
			for (int i = 0; i < SingletonCustom<BombermanPlayerManager>.Instance.UserNum; i++) {
				commonUIManager.ShowPlayerNo(i, SingletonCustom<BombermanPlayerManager>.Instance.Players[i].transform.position, SingletonCustom<BombermanGameManager>.Instance.GetCamera(), 150f);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public void UpdateMethod() {
			commonUIManager.SetTime(Mathf.Min(SingletonCustom<BombermanGameManager>.Instance.GameTime, 599f));
			if (!isShowSkip && SingletonCustom<BombermanPlayerManager>.Instance.CheckAliveCpuOnly() && SingletonCustom<BombermanPlayerManager>.Instance.GetAlivePlayerNum() >= 2) {
				isShowSkip = true;
				SingletonCustom<CommonUIManager>.Instance.ShowSkipUI();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="_playerIdx"></param>
		/// <param name="_rank"></param>
		public void SetRank(int _playerIdx, int _rank) {
			commonUIManager.SetRank(_playerIdx, _rank);
		}
	}
}