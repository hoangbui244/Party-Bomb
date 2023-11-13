using System;
using UnityEngine;

namespace io.ninenine.players.party2d.games.bomberman {
	/// <summary>
	/// 
	/// </summary>
	public class BombermanAIManager : SingletonCustom<BombermanAIManager> {
		/// <summary>
		/// 
		/// </summary>
		[Serializable]
		public struct AiDataList {
			/// <summary>
			/// 
			/// </summary>
			public Transform anchor;
		}
		/// <summary>
		/// 
		/// </summary>
		[SerializeField]
		private AiDataList[] aiDataList;
		/// <summary>
		/// 
		/// </summary>
		private int dataNo;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="_no"></param>
		public void Init(int _no) {
			dataNo = _no;
			aiDataList[dataNo].anchor.gameObject.SetActive(value: true);
		}		
	}
}