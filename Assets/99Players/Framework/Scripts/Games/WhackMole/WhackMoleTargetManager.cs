using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class WhackMoleTargetManager : SingletonCustom<WhackMoleTargetManager>
{
	[Serializable]
	public struct MoleData
	{
		public int holeNo;
		public HoleType holeType;
		public MoleType moleType;
		public float startGameTime;
		public float moveSpeed;
		public float stayTime;
		public MoleData(int _holeNo, MoleType _moleType, float _startGameTime, float _moveSpeed, float _stayTime)
		{
			holeNo = _holeNo;
			holeType = (HoleType)_holeNo;
			moleType = _moleType;
			startGameTime = _startGameTime;
			moveSpeed = _moveSpeed;
			stayTime = _stayTime;
		}
		public MoleData(HoleType _holeType, MoleType _moleType, float _startGameTime, float _moveSpeed, float _stayTime)
		{
			holeNo = (int)_holeType;
			holeType = _holeType;
			moleType = _moleType;
			startGameTime = _startGameTime;
			moveSpeed = _moveSpeed;
			stayTime = _stayTime;
		}
	}
	public enum MoleType
	{
		N,
		R
	}
	public enum HoleType
	{
		U_L,
		U_C,
		U_R,
		M_L,
		M_C,
		M_R,
		D_L,
		D_C,
		D_R
	}
	public const int MOLE_NORMAL_POINT = 50;
	public const int MOLE_RARE_POINT = 150;
	private const float SPEED_A = 3f;
	private const float SPEED_B = 6f;
	private const float SPEED_C = 8f;
	private const float TIME_A = 1.5f;
	private const float TIME_B = 1f;
	private const float TIME_C = 0.75f;
	private const float TIME_D = 7f;
	[SerializeField]
	private WhackMoleTargetGroup[] targetGroups;
	private List<MoleData> moleDataList;
	private static readonly MoleData[] MOLE_DATA_NEW_A = new MoleData[67]
	{
		new MoleData(3, MoleType.N, 0f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 1f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 2f, 3f, 1.5f),
		new MoleData(6, MoleType.R, 3.5f, 6f, 1f),
		new MoleData(0, MoleType.N, 4f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 5f, 3f, 1.5f),
		new MoleData(4, MoleType.N, 5f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 7f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 8f, 6f, 1f),
		new MoleData(8, MoleType.N, 9f, 8f, 0.75f),
		new MoleData(7, MoleType.N, 9.5f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 11f, 3f, 1.5f),
		new MoleData(6, MoleType.R, 12f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 14f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 14f, 6f, 1f),
		new MoleData(4, MoleType.N, 15f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 17f, 3f, 1.5f),
		new MoleData(3, MoleType.R, 17.5f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 19f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 20f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 21f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 22f, 3f, 1.5f),
		new MoleData(4, MoleType.R, 23.5f, 8f, 0.75f),
		new MoleData(8, MoleType.N, 24f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 25f, 6f, 1f),
		new MoleData(7, MoleType.R, 25f, 6f, 1f),
		new MoleData(2, MoleType.N, 27f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 28f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 29f, 3f, 1.5f),
		new MoleData(1, MoleType.R, 29.5f, 6f, 1f),
		new MoleData(8, MoleType.N, 31f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 32f, 3f, 1.5f),
		new MoleData(7, MoleType.R, 34f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 34f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 35f, 6f, 1f),
		new MoleData(0, MoleType.N, 37f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 37f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 37.5f, 8f, 0.75f),
		new MoleData(2, MoleType.N, 42f, 6f, 1.5f),
		new MoleData(6, MoleType.N, 42.5f, 6f, 1f),
		new MoleData(4, MoleType.N, 43f, 6f, 1.5f),
		new MoleData(1, MoleType.R, 44f, 6f, 1f),
		new MoleData(3, MoleType.N, 44f, 6f, 1.5f),
		new MoleData(7, MoleType.N, 45f, 6f, 1f),
		new MoleData(8, MoleType.R, 46f, 6f, 1f),
		new MoleData(5, MoleType.N, 46.5f, 6f, 1.5f),
		new MoleData(1, MoleType.N, 47.5f, 8f, 0.75f),
		new MoleData(2, MoleType.N, 48f, 6f, 1f),
		new MoleData(6, MoleType.R, 48f, 6f, 1f),
		new MoleData(4, MoleType.R, 49f, 6f, 1.5f),
		new MoleData(3, MoleType.N, 49f, 6f, 1.5f),
		new MoleData(0, MoleType.R, 50f, 6f, 1f),
		new MoleData(7, MoleType.N, 51f, 6f, 1.5f),
		new MoleData(2, MoleType.N, 52f, 6f, 1f),
		new MoleData(5, MoleType.R, 52f, 6f, 1f),
		new MoleData(8, MoleType.R, 52.5f, 6f, 1.5f),
		new MoleData(4, MoleType.N, 53f, 6f, 1.5f),
		new MoleData(1, MoleType.N, 53.5f, 6f, 1.5f),
		new MoleData(3, MoleType.N, 53.5f, 6f, 1f),
		new MoleData(6, MoleType.N, 54.5f, 8f, 0.75f),
		new MoleData(0, MoleType.N, 55.5f, 6f, 1.5f),
		new MoleData(2, MoleType.R, 55.5f, 6f, 1f),
		new MoleData(7, MoleType.N, 56.5f, 6f, 1.5f),
		new MoleData(5, MoleType.N, 57f, 6f, 1.5f),
		new MoleData(3, MoleType.N, 57.5f, 6f, 1f),
		new MoleData(4, MoleType.R, 58f, 6f, 1f),
		new MoleData(8, MoleType.N, 58f, 6f, 1f)
	};
	private static readonly MoleData[] MOLE_DATA_NEW_B = new MoleData[67]
	{
		new MoleData(2, MoleType.N, 0f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 1f, 3f, 1.5f),
		new MoleData(4, MoleType.N, 2f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 3f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 4f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 4.5f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 6f, 6f, 1f),
		new MoleData(1, MoleType.N, 8f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 8f, 3f, 1.5f),
		new MoleData(8, MoleType.R, 9f, 8f, 0.75f),
		new MoleData(7, MoleType.N, 10f, 6f, 1f),
		new MoleData(3, MoleType.N, 10f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 12f, 3f, 1.5f),
		new MoleData(6, MoleType.R, 13f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 14f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 16f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 16f, 3f, 1.5f),
		new MoleData(4, MoleType.R, 17.5f, 6f, 1f),
		new MoleData(5, MoleType.N, 18.5f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 19f, 3f, 1.5f),
		new MoleData(8, MoleType.R, 21f, 6f, 1f),
		new MoleData(6, MoleType.N, 23f, 3f, 1.5f),
		new MoleData(4, MoleType.N, 23f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 24f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 25f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 26f, 6f, 1f),
		new MoleData(0, MoleType.N, 27f, 3f, 1.5f),
		new MoleData(5, MoleType.R, 27.5f, 8f, 0.75f),
		new MoleData(6, MoleType.N, 29f, 3f, 1.5f),
		new MoleData(8, MoleType.R, 30f, 6f, 1f),
		new MoleData(1, MoleType.N, 30.5f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 32f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 34f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 34f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 35f, 8f, 0.75f),
		new MoleData(1, MoleType.N, 36f, 3f, 1.5f),
		new MoleData(0, MoleType.R, 37.5f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 38f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 41f, 6f, 1f),
		new MoleData(4, MoleType.N, 42f, 6f, 1.5f),
		new MoleData(1, MoleType.N, 43f, 6f, 1f),
		new MoleData(7, MoleType.R, 44f, 8f, 0.75f),
		new MoleData(2, MoleType.N, 44f, 6f, 1.5f),
		new MoleData(8, MoleType.N, 44f, 6f, 1.5f),
		new MoleData(3, MoleType.N, 45f, 6f, 1f),
		new MoleData(6, MoleType.R, 46f, 6f, 1f),
		new MoleData(7, MoleType.R, 46.5f, 8f, 0.75f),
		new MoleData(5, MoleType.N, 47f, 6f, 1.5f),
		new MoleData(4, MoleType.N, 48f, 6f, 1.5f),
		new MoleData(0, MoleType.R, 48f, 6f, 1.5f),
		new MoleData(8, MoleType.N, 49f, 6f, 1f),
		new MoleData(6, MoleType.N, 49.5f, 6f, 1.5f),
		new MoleData(1, MoleType.R, 50f, 6f, 1f),
		new MoleData(7, MoleType.N, 51f, 6f, 1f),
		new MoleData(2, MoleType.N, 51f, 6f, 1f),
		new MoleData(4, MoleType.R, 52f, 6f, 1.5f),
		new MoleData(3, MoleType.N, 53f, 6f, 1.5f),
		new MoleData(6, MoleType.N, 53.5f, 6f, 1f),
		new MoleData(8, MoleType.N, 54f, 6f, 1.5f),
		new MoleData(5, MoleType.N, 54f, 6f, 1.5f),
		new MoleData(7, MoleType.R, 55f, 6f, 1.5f),
		new MoleData(1, MoleType.R, 56f, 6f, 1f),
		new MoleData(0, MoleType.N, 56f, 6f, 1.5f),
		new MoleData(2, MoleType.N, 57f, 6f, 1f),
		new MoleData(4, MoleType.N, 57f, 6f, 1f),
		new MoleData(8, MoleType.R, 58f, 6f, 1.5f),
		new MoleData(5, MoleType.N, 58f, 6f, 1.5f)
	};
	private static readonly MoleData[] MOLE_DATA_NEW_C = new MoleData[67]
	{
		new MoleData(6, MoleType.N, 0f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 1f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 2f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 3f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 4f, 3f, 1.5f),
		new MoleData(3, MoleType.R, 5f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 6f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 8f, 6f, 1f),
		new MoleData(5, MoleType.N, 8f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 9f, 3f, 1.5f),
		new MoleData(6, MoleType.R, 9f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 11.5f, 6f, 1f),
		new MoleData(4, MoleType.N, 12.5f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 13f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 14.5f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 15f, 3f, 1.5f),
		new MoleData(2, MoleType.R, 16f, 6f, 1f),
		new MoleData(5, MoleType.N, 17f, 3f, 1.5f),
		new MoleData(4, MoleType.N, 18f, 3f, 1.5f),
		new MoleData(2, MoleType.R, 20f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 21f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 23f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 23f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 24f, 8f, 0.75f),
		new MoleData(7, MoleType.R, 25f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 26f, 6f, 1f),
		new MoleData(4, MoleType.N, 26f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 28.5f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 29f, 3f, 1.5f),
		new MoleData(5, MoleType.R, 30f, 8f, 0.75f),
		new MoleData(1, MoleType.N, 31f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 32f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 32.5f, 6f, 1f),
		new MoleData(5, MoleType.N, 35f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 35f, 6f, 1f),
		new MoleData(6, MoleType.R, 36f, 6f, 1f),
		new MoleData(0, MoleType.N, 37f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 37.5f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 42f, 6f, 1.5f),
		new MoleData(5, MoleType.N, 42f, 6f, 1.5f),
		new MoleData(7, MoleType.N, 43f, 8f, 0.75f),
		new MoleData(0, MoleType.R, 44f, 6f, 1.5f),
		new MoleData(3, MoleType.R, 44f, 6f, 1.5f),
		new MoleData(6, MoleType.N, 45f, 6f, 1.5f),
		new MoleData(8, MoleType.N, 45f, 6f, 1.5f),
		new MoleData(2, MoleType.N, 46f, 6f, 1f),
		new MoleData(4, MoleType.R, 46.5f, 6f, 1.5f),
		new MoleData(7, MoleType.N, 47f, 6f, 1f),
		new MoleData(5, MoleType.N, 47.5f, 6f, 1f),
		new MoleData(1, MoleType.N, 48f, 6f, 1.5f),
		new MoleData(3, MoleType.N, 49f, 6f, 1f),
		new MoleData(6, MoleType.R, 50f, 6f, 1.5f),
		new MoleData(2, MoleType.N, 50f, 6f, 1f),
		new MoleData(8, MoleType.R, 51f, 6f, 1f),
		new MoleData(0, MoleType.N, 51f, 6f, 1.5f),
		new MoleData(4, MoleType.N, 52f, 6f, 1f),
		new MoleData(1, MoleType.N, 53f, 6f, 1f),
		new MoleData(3, MoleType.R, 53f, 8f, 0.75f),
		new MoleData(2, MoleType.R, 54f, 6f, 1f),
		new MoleData(7, MoleType.N, 54f, 6f, 1.5f),
		new MoleData(4, MoleType.N, 55f, 6f, 1.5f),
		new MoleData(8, MoleType.R, 56f, 6f, 1f),
		new MoleData(3, MoleType.N, 56f, 6f, 1f),
		new MoleData(5, MoleType.N, 57f, 6f, 1.5f),
		new MoleData(1, MoleType.R, 57f, 6f, 1.5f),
		new MoleData(2, MoleType.N, 57f, 6f, 1f),
		new MoleData(7, MoleType.N, 58f, 6f, 1.5f)
	};
	private static readonly MoleData[] MOLE_DATA_NEW_D = new MoleData[67]
	{
		new MoleData(8, MoleType.N, 0f, 3f, 1.5f),
		new MoleData(4, MoleType.N, 1f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 2f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 3f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 4f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 5f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 5f, 3f, 1.5f),
		new MoleData(4, MoleType.N, 7f, 3f, 1.5f),
		new MoleData(7, MoleType.R, 9f, 6f, 1f),
		new MoleData(2, MoleType.N, 9f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 10f, 3f, 1.5f),
		new MoleData(0, MoleType.R, 11.5f, 8f, 0.75f),
		new MoleData(7, MoleType.N, 12f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 12.5f, 6f, 1f),
		new MoleData(8, MoleType.N, 14.5f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 16f, 3f, 1.5f),
		new MoleData(5, MoleType.R, 16f, 6f, 1f),
		new MoleData(7, MoleType.N, 17f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 18f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 20f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 20.5f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 21f, 3f, 1.5f),
		new MoleData(2, MoleType.R, 23f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 24f, 6f, 1f),
		new MoleData(8, MoleType.N, 25f, 8f, 0.75f),
		new MoleData(4, MoleType.N, 27f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 27f, 3f, 1.5f),
		new MoleData(0, MoleType.R, 28f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 30f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 30f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 31f, 6f, 1f),
		new MoleData(5, MoleType.R, 32f, 8f, 0.75f),
		new MoleData(3, MoleType.N, 33f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 34.5f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 35f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 35.5f, 6f, 1f),
		new MoleData(6, MoleType.R, 37f, 6f, 1f),
		new MoleData(7, MoleType.N, 38f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 42f, 8f, 0.75f),
		new MoleData(0, MoleType.N, 42f, 6f, 1f),
		new MoleData(4, MoleType.R, 43f, 6f, 1.5f),
		new MoleData(7, MoleType.N, 43f, 6f, 1.5f),
		new MoleData(3, MoleType.N, 44f, 6f, 1.5f),
		new MoleData(1, MoleType.N, 44.5f, 6f, 1.5f),
		new MoleData(8, MoleType.N, 45f, 6f, 1f),
		new MoleData(0, MoleType.R, 46f, 6f, 1.5f),
		new MoleData(6, MoleType.R, 46.5f, 6f, 1f),
		new MoleData(5, MoleType.N, 47f, 6f, 1.5f),
		new MoleData(2, MoleType.N, 48f, 6f, 1f),
		new MoleData(8, MoleType.N, 48f, 6f, 1f),
		new MoleData(1, MoleType.R, 49f, 6f, 1.5f),
		new MoleData(4, MoleType.N, 50f, 6f, 1f),
		new MoleData(0, MoleType.N, 50f, 6f, 1f),
		new MoleData(5, MoleType.N, 50.5f, 6f, 1f),
		new MoleData(6, MoleType.R, 51f, 6f, 1.5f),
		new MoleData(2, MoleType.N, 52f, 6f, 1.5f),
		new MoleData(3, MoleType.N, 53f, 6f, 1.5f),
		new MoleData(8, MoleType.R, 53f, 6f, 1f),
		new MoleData(7, MoleType.R, 54f, 6f, 1f),
		new MoleData(5, MoleType.N, 54f, 8f, 0.75f),
		new MoleData(4, MoleType.N, 55f, 6f, 1.5f),
		new MoleData(6, MoleType.N, 56f, 6f, 1f),
		new MoleData(1, MoleType.R, 56f, 6f, 1.5f),
		new MoleData(0, MoleType.N, 56.5f, 6f, 1.5f),
		new MoleData(2, MoleType.R, 57f, 6f, 1f),
		new MoleData(5, MoleType.N, 58f, 6f, 1.5f),
		new MoleData(3, MoleType.N, 58f, 6f, 1.5f)
	};
	private static readonly MoleData[] MOLE_DATA_NEW_A_REVERSE = new MoleData[67]
	{
		new MoleData(5, MoleType.N, 0f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 1f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 2f, 3f, 1.5f),
		new MoleData(2, MoleType.R, 3.5f, 6f, 1f),
		new MoleData(8, MoleType.N, 4f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 5f, 3f, 1.5f),
		new MoleData(4, MoleType.N, 5f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 7f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 8f, 6f, 1f),
		new MoleData(0, MoleType.N, 9f, 8f, 0.75f),
		new MoleData(1, MoleType.N, 9.5f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 11f, 3f, 1.5f),
		new MoleData(2, MoleType.R, 12f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 14f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 14f, 6f, 1f),
		new MoleData(4, MoleType.N, 15f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 17f, 3f, 1.5f),
		new MoleData(5, MoleType.R, 17.5f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 19f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 20f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 21f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 22f, 3f, 1.5f),
		new MoleData(4, MoleType.R, 23.5f, 8f, 0.75f),
		new MoleData(0, MoleType.N, 24f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 25f, 6f, 1f),
		new MoleData(1, MoleType.R, 25f, 6f, 1f),
		new MoleData(6, MoleType.N, 27f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 28f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 29f, 3f, 1.5f),
		new MoleData(7, MoleType.R, 29.5f, 6f, 1f),
		new MoleData(0, MoleType.N, 31f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 32f, 3f, 1.5f),
		new MoleData(1, MoleType.R, 34f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 34f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 35f, 6f, 1f),
		new MoleData(8, MoleType.N, 37f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 37f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 37.5f, 8f, 0.75f),
		new MoleData(6, MoleType.N, 42f, 6f, 1.5f),
		new MoleData(2, MoleType.N, 42.5f, 6f, 1f),
		new MoleData(4, MoleType.N, 43f, 6f, 1.5f),
		new MoleData(7, MoleType.R, 44f, 6f, 1f),
		new MoleData(5, MoleType.N, 44f, 6f, 1.5f),
		new MoleData(1, MoleType.N, 45f, 6f, 1f),
		new MoleData(0, MoleType.R, 46f, 6f, 1f),
		new MoleData(3, MoleType.N, 46.5f, 6f, 1.5f),
		new MoleData(7, MoleType.N, 47.5f, 8f, 0.75f),
		new MoleData(6, MoleType.N, 48f, 6f, 1f),
		new MoleData(2, MoleType.R, 48f, 6f, 1f),
		new MoleData(4, MoleType.R, 49f, 6f, 1.5f),
		new MoleData(5, MoleType.N, 49f, 6f, 1.5f),
		new MoleData(8, MoleType.R, 50f, 6f, 1f),
		new MoleData(1, MoleType.N, 51f, 6f, 1.5f),
		new MoleData(6, MoleType.N, 52f, 6f, 1f),
		new MoleData(3, MoleType.R, 52f, 6f, 1f),
		new MoleData(0, MoleType.R, 52.5f, 6f, 1.5f),
		new MoleData(4, MoleType.N, 53f, 6f, 1.5f),
		new MoleData(7, MoleType.N, 53.5f, 6f, 1.5f),
		new MoleData(5, MoleType.N, 53.5f, 6f, 1f),
		new MoleData(2, MoleType.N, 54.5f, 8f, 0.75f),
		new MoleData(8, MoleType.N, 55.5f, 6f, 1.5f),
		new MoleData(6, MoleType.R, 55.5f, 6f, 1f),
		new MoleData(1, MoleType.N, 56.5f, 6f, 1.5f),
		new MoleData(3, MoleType.N, 57f, 6f, 1.5f),
		new MoleData(5, MoleType.N, 57.5f, 6f, 1f),
		new MoleData(4, MoleType.R, 58f, 6f, 1f),
		new MoleData(0, MoleType.N, 58f, 6f, 1f)
	};
	private static readonly MoleData[] MOLE_DATA_NEW_B_REVERSE = new MoleData[67]
	{
		new MoleData(6, MoleType.N, 0f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 1f, 3f, 1.5f),
		new MoleData(4, MoleType.N, 2f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 3f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 4f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 4.5f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 6f, 6f, 1f),
		new MoleData(7, MoleType.N, 8f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 8f, 3f, 1.5f),
		new MoleData(0, MoleType.R, 9f, 8f, 0.75f),
		new MoleData(1, MoleType.N, 10f, 6f, 1f),
		new MoleData(5, MoleType.N, 10f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 12f, 3f, 1.5f),
		new MoleData(2, MoleType.R, 13f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 14f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 16f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 16f, 3f, 1.5f),
		new MoleData(4, MoleType.R, 17.5f, 6f, 1f),
		new MoleData(3, MoleType.N, 18.5f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 19f, 3f, 1.5f),
		new MoleData(0, MoleType.R, 21f, 6f, 1f),
		new MoleData(2, MoleType.N, 23f, 3f, 1.5f),
		new MoleData(4, MoleType.N, 23f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 24f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 25f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 26f, 6f, 1f),
		new MoleData(8, MoleType.N, 27f, 3f, 1.5f),
		new MoleData(3, MoleType.R, 27.5f, 8f, 0.75f),
		new MoleData(2, MoleType.N, 29f, 3f, 1.5f),
		new MoleData(0, MoleType.R, 30f, 6f, 1f),
		new MoleData(7, MoleType.N, 30.5f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 32f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 34f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 34f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 35f, 8f, 0.75f),
		new MoleData(7, MoleType.N, 36f, 3f, 1.5f),
		new MoleData(8, MoleType.R, 37.5f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 38f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 41f, 6f, 1f),
		new MoleData(4, MoleType.N, 42f, 6f, 1.5f),
		new MoleData(7, MoleType.N, 43f, 6f, 1f),
		new MoleData(1, MoleType.R, 44f, 8f, 0.75f),
		new MoleData(6, MoleType.N, 44f, 6f, 1.5f),
		new MoleData(0, MoleType.N, 44f, 6f, 1.5f),
		new MoleData(5, MoleType.N, 45f, 6f, 1f),
		new MoleData(2, MoleType.R, 46f, 6f, 1f),
		new MoleData(1, MoleType.R, 46.5f, 8f, 0.75f),
		new MoleData(3, MoleType.N, 47f, 6f, 1.5f),
		new MoleData(4, MoleType.N, 48f, 6f, 1.5f),
		new MoleData(8, MoleType.R, 48f, 6f, 1.5f),
		new MoleData(0, MoleType.N, 49f, 6f, 1f),
		new MoleData(2, MoleType.N, 49.5f, 6f, 1.5f),
		new MoleData(7, MoleType.R, 50f, 6f, 1f),
		new MoleData(1, MoleType.N, 51f, 6f, 1f),
		new MoleData(6, MoleType.N, 51f, 6f, 1f),
		new MoleData(4, MoleType.R, 52f, 6f, 1.5f),
		new MoleData(5, MoleType.N, 53f, 6f, 1.5f),
		new MoleData(2, MoleType.N, 53.5f, 6f, 1f),
		new MoleData(0, MoleType.N, 54f, 6f, 1.5f),
		new MoleData(3, MoleType.N, 54f, 6f, 1.5f),
		new MoleData(1, MoleType.R, 55f, 6f, 1.5f),
		new MoleData(7, MoleType.R, 56f, 6f, 1f),
		new MoleData(8, MoleType.N, 56f, 6f, 1.5f),
		new MoleData(6, MoleType.N, 57f, 6f, 1f),
		new MoleData(4, MoleType.N, 57f, 6f, 1f),
		new MoleData(0, MoleType.R, 58f, 6f, 1.5f),
		new MoleData(3, MoleType.N, 58f, 6f, 1.5f)
	};
	private static readonly MoleData[] MOLE_DATA_NEW_C_REVERSE = new MoleData[67]
	{
		new MoleData(2, MoleType.N, 0f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 1f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 2f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 3f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 4f, 3f, 1.5f),
		new MoleData(5, MoleType.R, 5f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 6f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 8f, 6f, 1f),
		new MoleData(3, MoleType.N, 8f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 9f, 3f, 1.5f),
		new MoleData(2, MoleType.R, 9f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 11.5f, 6f, 1f),
		new MoleData(4, MoleType.N, 12.5f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 13f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 14.5f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 15f, 3f, 1.5f),
		new MoleData(6, MoleType.R, 16f, 6f, 1f),
		new MoleData(3, MoleType.N, 17f, 3f, 1.5f),
		new MoleData(4, MoleType.N, 18f, 3f, 1.5f),
		new MoleData(6, MoleType.R, 20f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 21f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 23f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 23f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 24f, 8f, 0.75f),
		new MoleData(1, MoleType.R, 25f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 26f, 6f, 1f),
		new MoleData(4, MoleType.N, 26f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 28.5f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 29f, 3f, 1.5f),
		new MoleData(3, MoleType.R, 30f, 8f, 0.75f),
		new MoleData(7, MoleType.N, 31f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 32f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 32.5f, 6f, 1f),
		new MoleData(3, MoleType.N, 35f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 35f, 6f, 1f),
		new MoleData(2, MoleType.R, 36f, 6f, 1f),
		new MoleData(8, MoleType.N, 37f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 37.5f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 42f, 6f, 1.5f),
		new MoleData(3, MoleType.N, 42f, 6f, 1.5f),
		new MoleData(1, MoleType.N, 43f, 8f, 0.75f),
		new MoleData(8, MoleType.R, 44f, 6f, 1.5f),
		new MoleData(5, MoleType.R, 44f, 6f, 1.5f),
		new MoleData(2, MoleType.N, 45f, 6f, 1.5f),
		new MoleData(0, MoleType.N, 45f, 6f, 1.5f),
		new MoleData(6, MoleType.N, 46f, 6f, 1f),
		new MoleData(4, MoleType.R, 46.5f, 6f, 1.5f),
		new MoleData(1, MoleType.N, 47f, 6f, 1f),
		new MoleData(3, MoleType.N, 47.5f, 6f, 1f),
		new MoleData(7, MoleType.N, 48f, 6f, 1.5f),
		new MoleData(5, MoleType.N, 49f, 6f, 1f),
		new MoleData(2, MoleType.R, 50f, 6f, 1.5f),
		new MoleData(6, MoleType.N, 50f, 6f, 1f),
		new MoleData(0, MoleType.R, 51f, 6f, 1f),
		new MoleData(8, MoleType.N, 51f, 6f, 1.5f),
		new MoleData(4, MoleType.N, 52f, 6f, 1f),
		new MoleData(7, MoleType.N, 53f, 6f, 1f),
		new MoleData(5, MoleType.R, 53f, 8f, 0.75f),
		new MoleData(6, MoleType.R, 54f, 6f, 1f),
		new MoleData(1, MoleType.N, 54f, 6f, 1.5f),
		new MoleData(4, MoleType.N, 55f, 6f, 1.5f),
		new MoleData(0, MoleType.R, 56f, 6f, 1f),
		new MoleData(5, MoleType.N, 56f, 6f, 1f),
		new MoleData(3, MoleType.N, 57f, 6f, 1.5f),
		new MoleData(7, MoleType.R, 57f, 6f, 1.5f),
		new MoleData(6, MoleType.N, 57f, 6f, 1f),
		new MoleData(1, MoleType.N, 58f, 6f, 1.5f)
	};
	private static readonly MoleData[] MOLE_DATA_NEW_D_REVERSE = new MoleData[67]
	{
		new MoleData(0, MoleType.N, 0f, 3f, 1.5f),
		new MoleData(4, MoleType.N, 1f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 2f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 3f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 4f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 5f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 5f, 3f, 1.5f),
		new MoleData(4, MoleType.N, 7f, 3f, 1.5f),
		new MoleData(1, MoleType.R, 9f, 6f, 1f),
		new MoleData(6, MoleType.N, 9f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 10f, 3f, 1.5f),
		new MoleData(8, MoleType.R, 11.5f, 8f, 0.75f),
		new MoleData(1, MoleType.N, 12f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 12.5f, 6f, 1f),
		new MoleData(0, MoleType.N, 14.5f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 16f, 3f, 1.5f),
		new MoleData(3, MoleType.R, 16f, 6f, 1f),
		new MoleData(1, MoleType.N, 17f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 18f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 20f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 20.5f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 21f, 3f, 1.5f),
		new MoleData(6, MoleType.R, 23f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 24f, 6f, 1f),
		new MoleData(0, MoleType.N, 25f, 8f, 0.75f),
		new MoleData(4, MoleType.N, 27f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 27f, 3f, 1.5f),
		new MoleData(8, MoleType.R, 28f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 30f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 30f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 31f, 6f, 1f),
		new MoleData(3, MoleType.R, 32f, 8f, 0.75f),
		new MoleData(5, MoleType.N, 33f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 34.5f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 35f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 35.5f, 6f, 1f),
		new MoleData(2, MoleType.R, 37f, 6f, 1f),
		new MoleData(1, MoleType.N, 38f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 42f, 8f, 0.75f),
		new MoleData(8, MoleType.N, 42f, 6f, 1f),
		new MoleData(4, MoleType.R, 43f, 6f, 1.5f),
		new MoleData(1, MoleType.N, 43f, 6f, 1.5f),
		new MoleData(5, MoleType.N, 44f, 6f, 1.5f),
		new MoleData(7, MoleType.N, 44.5f, 6f, 1.5f),
		new MoleData(0, MoleType.N, 45f, 6f, 1f),
		new MoleData(8, MoleType.R, 46f, 6f, 1.5f),
		new MoleData(2, MoleType.R, 46.5f, 6f, 1f),
		new MoleData(3, MoleType.N, 47f, 6f, 1.5f),
		new MoleData(6, MoleType.N, 48f, 6f, 1f),
		new MoleData(0, MoleType.N, 48f, 6f, 1f),
		new MoleData(7, MoleType.R, 49f, 6f, 1.5f),
		new MoleData(4, MoleType.N, 50f, 6f, 1f),
		new MoleData(8, MoleType.N, 50f, 6f, 1f),
		new MoleData(3, MoleType.N, 50.5f, 6f, 1f),
		new MoleData(2, MoleType.R, 51f, 6f, 1.5f),
		new MoleData(6, MoleType.N, 52f, 6f, 1.5f),
		new MoleData(5, MoleType.N, 53f, 6f, 1.5f),
		new MoleData(0, MoleType.R, 53f, 6f, 1f),
		new MoleData(1, MoleType.R, 54f, 6f, 1f),
		new MoleData(3, MoleType.N, 54f, 8f, 0.75f),
		new MoleData(4, MoleType.N, 55f, 6f, 1.5f),
		new MoleData(2, MoleType.N, 56f, 6f, 1f),
		new MoleData(7, MoleType.R, 56f, 6f, 1.5f),
		new MoleData(8, MoleType.N, 56.5f, 6f, 1.5f),
		new MoleData(6, MoleType.R, 57f, 6f, 1f),
		new MoleData(3, MoleType.N, 58f, 6f, 1.5f),
		new MoleData(5, MoleType.N, 58f, 6f, 1.5f)
	};
	private static readonly MoleData[] MOLE_DATA_TEST2 = new MoleData[63]
	{
		new MoleData(2, MoleType.N, 0f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 1f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 2f, 3f, 1.5f),
		new MoleData(2, MoleType.R, 3.5f, 6f, 1f),
		new MoleData(0, MoleType.N, 4f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 5f, 3f, 1.5f),
		new MoleData(4, MoleType.N, 5f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 7f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 8f, 6f, 1f),
		new MoleData(8, MoleType.N, 9f, 8f, 0.75f),
		new MoleData(7, MoleType.N, 9.5f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 11f, 3f, 1.5f),
		new MoleData(6, MoleType.R, 12f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 14f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 14f, 6f, 1f),
		new MoleData(4, MoleType.N, 15f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 17f, 3f, 1.5f),
		new MoleData(3, MoleType.R, 17.5f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 19f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 20f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 21f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 22f, 3f, 1.5f),
		new MoleData(4, MoleType.R, 23.5f, 8f, 0.75f),
		new MoleData(8, MoleType.N, 24f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 25f, 6f, 1f),
		new MoleData(7, MoleType.N, 25f, 6f, 1f),
		new MoleData(2, MoleType.R, 27f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 28f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 29f, 3f, 1.5f),
		new MoleData(1, MoleType.R, 29.5f, 6f, 1f),
		new MoleData(8, MoleType.N, 31f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 32f, 3f, 1.5f),
		new MoleData(7, MoleType.R, 34f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 34f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 35f, 6f, 1f),
		new MoleData(0, MoleType.N, 37f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 37f, 3f, 1.5f),
		new MoleData(3, MoleType.R, 37.5f, 8f, 0.75f),
		new MoleData(2, MoleType.N, 42f, 6f, 1f),
		new MoleData(6, MoleType.N, 42.5f, 6f, 1f),
		new MoleData(4, MoleType.N, 43f, 6f, 1f),
		new MoleData(1, MoleType.R, 43.5f, 6f, 1f),
		new MoleData(3, MoleType.N, 49f, 6f, 1f),
		new MoleData(7, MoleType.N, 49.5f, 6f, 1f),
		new MoleData(8, MoleType.R, 50f, 6f, 1f),
		new MoleData(5, MoleType.N, 50f, 6f, 1f),
		new MoleData(1, MoleType.N, 51f, 6f, 1f),
		new MoleData(2, MoleType.N, 51f, 6f, 1f),
		new MoleData(6, MoleType.R, 52f, 6f, 1f),
		new MoleData(4, MoleType.R, 52.5f, 6f, 1f),
		new MoleData(3, MoleType.N, 53f, 6f, 1f),
		new MoleData(0, MoleType.R, 53f, 6f, 1f),
		new MoleData(7, MoleType.N, 54f, 6f, 1f),
		new MoleData(2, MoleType.N, 54f, 6f, 1f),
		new MoleData(5, MoleType.R, 54.5f, 6f, 1f),
		new MoleData(8, MoleType.R, 55f, 6f, 1f),
		new MoleData(4, MoleType.N, 55.5f, 6f, 1f),
		new MoleData(1, MoleType.N, 56f, 6f, 1f),
		new MoleData(3, MoleType.N, 56f, 6f, 1f),
		new MoleData(6, MoleType.N, 57f, 6f, 1f),
		new MoleData(0, MoleType.N, 57f, 6f, 1f),
		new MoleData(2, MoleType.R, 58f, 6f, 1f),
		new MoleData(7, MoleType.R, 58f, 6f, 1f)
	};
	private static readonly MoleData[] MOLE_DATA_A = new MoleData[58]
	{
		new MoleData(2, MoleType.N, 0f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 1f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 2f, 3f, 1.5f),
		new MoleData(2, MoleType.R, 3.5f, 6f, 1f),
		new MoleData(0, MoleType.N, 4f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 5f, 3f, 1.5f),
		new MoleData(4, MoleType.N, 5f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 7f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 8f, 6f, 1f),
		new MoleData(8, MoleType.N, 9f, 8f, 0.75f),
		new MoleData(7, MoleType.N, 9.5f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 11f, 3f, 1.5f),
		new MoleData(6, MoleType.R, 12f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 14f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 14f, 6f, 1f),
		new MoleData(4, MoleType.N, 15f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 17f, 3f, 1.5f),
		new MoleData(3, MoleType.R, 17.5f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 19f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 20f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 21f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 22f, 3f, 1.5f),
		new MoleData(4, MoleType.R, 23.5f, 8f, 0.75f),
		new MoleData(8, MoleType.N, 24f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 25f, 6f, 1f),
		new MoleData(7, MoleType.N, 25f, 6f, 1f),
		new MoleData(2, MoleType.R, 27f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 28f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 29f, 3f, 1.5f),
		new MoleData(1, MoleType.R, 29.5f, 6f, 1f),
		new MoleData(8, MoleType.N, 31f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 32f, 3f, 1.5f),
		new MoleData(7, MoleType.R, 34f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 34f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 35f, 6f, 1f),
		new MoleData(0, MoleType.N, 37f, 3f, 1.5f),
		new MoleData(3, MoleType.R, 37.5f, 8f, 0.75f),
		new MoleData(7, MoleType.N, 39f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 40f, 6f, 1f),
		new MoleData(6, MoleType.N, 41f, 3f, 1.5f),
		new MoleData(4, MoleType.N, 42f, 3f, 1.5f),
		new MoleData(1, MoleType.R, 43.5f, 6f, 1f),
		new MoleData(3, MoleType.N, 44f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 45f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 45f, 3f, 1.5f),
		new MoleData(6, MoleType.R, 47f, 3f, 1.5f),
		new MoleData(4, MoleType.N, 47.5f, 6f, 1f),
		new MoleData(3, MoleType.N, 48f, 3f, 1.5f),
		new MoleData(0, MoleType.R, 48f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 52f, 8f, 7f),
		new MoleData(1, MoleType.N, 52f, 8f, 7f),
		new MoleData(2, MoleType.R, 52f, 8f, 7f),
		new MoleData(3, MoleType.R, 52f, 8f, 7f),
		new MoleData(4, MoleType.N, 52f, 8f, 7f),
		new MoleData(5, MoleType.R, 52f, 8f, 7f),
		new MoleData(6, MoleType.R, 52f, 8f, 7f),
		new MoleData(7, MoleType.N, 52f, 8f, 7f),
		new MoleData(8, MoleType.N, 52f, 8f, 7f)
	};
	private static readonly MoleData[] MOLE_DATA_B = new MoleData[60]
	{
		new MoleData(7, MoleType.N, 0f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 1f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 2f, 3f, 1.5f),
		new MoleData(4, MoleType.N, 3f, 3f, 1.5f),
		new MoleData(0, MoleType.R, 4f, 6f, 1f),
		new MoleData(8, MoleType.N, 4f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 6.5f, 3f, 1.5f),
		new MoleData(4, MoleType.N, 7f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 8f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 9f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 10f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 12f, 3f, 1.5f),
		new MoleData(5, MoleType.R, 11.5f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 13f, 6f, 1f),
		new MoleData(2, MoleType.N, 14f, 3f, 1.5f),
		new MoleData(0, MoleType.R, 15f, 6f, 1f),
		new MoleData(1, MoleType.N, 16f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 17f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 18f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 19f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 19f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 21f, 3f, 1.5f),
		new MoleData(1, MoleType.R, 22f, 6f, 1f),
		new MoleData(0, MoleType.N, 23f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 25f, 3f, 1.5f),
		new MoleData(7, MoleType.R, 24.5f, 3f, 1.5f),
		new MoleData(4, MoleType.N, 26.5f, 6f, 1f),
		new MoleData(3, MoleType.N, 27f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 29f, 8f, 0.75f),
		new MoleData(6, MoleType.N, 29f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 30f, 3f, 1.5f),
		new MoleData(7, MoleType.R, 31.5f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 32f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 33f, 3f, 1.5f),
		new MoleData(5, MoleType.R, 34f, 6f, 1f),
		new MoleData(2, MoleType.N, 35f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 36f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 37f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 38f, 3f, 1.5f),
		new MoleData(3, MoleType.R, 39f, 8f, 0.75f),
		new MoleData(7, MoleType.N, 39.5f, 3f, 1.5f),
		new MoleData(0, MoleType.R, 40f, 6f, 1f),
		new MoleData(2, MoleType.N, 42f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 43f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 44f, 6f, 1f),
		new MoleData(3, MoleType.R, 45f, 6f, 1f),
		new MoleData(5, MoleType.N, 46f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 47f, 8f, 0.75f),
		new MoleData(4, MoleType.N, 47f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 48f, 3f, 1.5f),
		new MoleData(6, MoleType.R, 48f, 3f, 1.5f),
		new MoleData(0, MoleType.R, 52f, 8f, 7f),
		new MoleData(1, MoleType.R, 52f, 8f, 7f),
		new MoleData(2, MoleType.R, 52f, 8f, 7f),
		new MoleData(3, MoleType.N, 52f, 8f, 7f),
		new MoleData(4, MoleType.N, 52f, 8f, 7f),
		new MoleData(5, MoleType.N, 52f, 8f, 7f),
		new MoleData(6, MoleType.N, 52f, 8f, 7f),
		new MoleData(7, MoleType.N, 52f, 8f, 7f),
		new MoleData(8, MoleType.R, 52f, 8f, 7f)
	};
	private static readonly MoleData[] MOLE_DATA_C = new MoleData[60]
	{
		new MoleData(7, MoleType.N, 0f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 1f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 2f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 3f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 4f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 5f, 6f, 1f),
		new MoleData(6, MoleType.N, 6f, 3f, 1.5f),
		new MoleData(8, MoleType.R, 7f, 3f, 1.5f),
		new MoleData(4, MoleType.N, 8f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 8f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 10.5f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 12f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 12f, 3f, 1.5f),
		new MoleData(2, MoleType.R, 12.5f, 6f, 1f),
		new MoleData(4, MoleType.N, 14f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 15f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 16f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 17f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 18f, 3f, 1.5f),
		new MoleData(5, MoleType.R, 19f, 6f, 1f),
		new MoleData(8, MoleType.N, 20f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 22f, 3f, 1.5f),
		new MoleData(6, MoleType.R, 22f, 6f, 1f),
		new MoleData(2, MoleType.N, 23f, 3f, 1.5f),
		new MoleData(4, MoleType.N, 23f, 8f, 0.75f),
		new MoleData(5, MoleType.N, 25f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 26f, 3f, 1.5f),
		new MoleData(1, MoleType.R, 27f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 28.5f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 28.5f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 30f, 3f, 1.5f),
		new MoleData(8, MoleType.R, 32f, 6f, 1f),
		new MoleData(1, MoleType.N, 32f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 33f, 3f, 1.5f),
		new MoleData(5, MoleType.R, 34f, 8f, 0.75f),
		new MoleData(2, MoleType.N, 35.5f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 36f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 37f, 6f, 1f),
		new MoleData(1, MoleType.N, 38f, 3f, 1.5f),
		new MoleData(7, MoleType.R, 39f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 40f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 41f, 3f, 1.5f),
		new MoleData(3, MoleType.R, 42f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 43f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 44f, 8f, 0.75f),
		new MoleData(1, MoleType.N, 44.5f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 45f, 3f, 1.5f),
		new MoleData(3, MoleType.R, 47f, 6f, 1f),
		new MoleData(8, MoleType.N, 47f, 6f, 1f),
		new MoleData(4, MoleType.N, 48f, 3f, 1.5f),
		new MoleData(2, MoleType.R, 48f, 6f, 1f),
		new MoleData(0, MoleType.R, 52f, 8f, 7f),
		new MoleData(1, MoleType.R, 52f, 8f, 7f),
		new MoleData(2, MoleType.N, 52f, 8f, 7f),
		new MoleData(3, MoleType.N, 52f, 8f, 7f),
		new MoleData(4, MoleType.R, 52f, 8f, 7f),
		new MoleData(5, MoleType.N, 52f, 8f, 7f),
		new MoleData(6, MoleType.N, 52f, 8f, 7f),
		new MoleData(7, MoleType.N, 52f, 8f, 7f),
		new MoleData(8, MoleType.R, 52f, 8f, 7f)
	};
	private static readonly MoleData[] MOLE_DATA_D = new MoleData[60]
	{
		new MoleData(6, MoleType.N, 0f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 1f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 2f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 3f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 5f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 5f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 6f, 3f, 1.5f),
		new MoleData(0, MoleType.R, 7.5f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 8f, 3f, 1.5f),
		new MoleData(4, MoleType.N, 9f, 6f, 1f),
		new MoleData(1, MoleType.N, 10f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 11f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 12f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 12f, 3f, 1.5f),
		new MoleData(2, MoleType.R, 14f, 6f, 1f),
		new MoleData(8, MoleType.N, 15f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 16f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 17f, 6f, 1f),
		new MoleData(4, MoleType.R, 17.5f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 19f, 3f, 1.5f),
		new MoleData(5, MoleType.R, 20f, 8f, 0.75f),
		new MoleData(2, MoleType.N, 21.5f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 22f, 3f, 1.5f),
		new MoleData(3, MoleType.N, 23f, 3f, 1.5f),
		new MoleData(8, MoleType.R, 25f, 6f, 1f),
		new MoleData(1, MoleType.N, 25f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 26f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 27f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 28f, 3f, 1.5f),
		new MoleData(2, MoleType.R, 28.5f, 8f, 0.75f),
		new MoleData(3, MoleType.N, 30f, 3f, 1.5f),
		new MoleData(0, MoleType.N, 31.5f, 3f, 1.5f),
		new MoleData(7, MoleType.N, 32f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 32f, 3f, 1.5f),
		new MoleData(5, MoleType.R, 34f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 35f, 6f, 1f),
		new MoleData(6, MoleType.N, 36f, 6f, 1f),
		new MoleData(4, MoleType.N, 38f, 3f, 1.5f),
		new MoleData(1, MoleType.R, 38f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 39f, 3f, 1.5f),
		new MoleData(3, MoleType.R, 39.5f, 6f, 1f),
		new MoleData(7, MoleType.N, 41f, 3f, 1.5f),
		new MoleData(6, MoleType.N, 42.5f, 3f, 1.5f),
		new MoleData(8, MoleType.N, 43f, 3f, 1.5f),
		new MoleData(1, MoleType.R, 44f, 3f, 1.5f),
		new MoleData(2, MoleType.N, 45f, 6f, 1f),
		new MoleData(4, MoleType.N, 45f, 6f, 1f),
		new MoleData(7, MoleType.N, 47f, 3f, 1.5f),
		new MoleData(0, MoleType.R, 47f, 3f, 1.5f),
		new MoleData(5, MoleType.N, 48f, 3f, 1.5f),
		new MoleData(1, MoleType.N, 48f, 8f, 0.75f),
		new MoleData(0, MoleType.N, 52f, 8f, 7f),
		new MoleData(1, MoleType.R, 52f, 8f, 7f),
		new MoleData(2, MoleType.N, 52f, 8f, 7f),
		new MoleData(3, MoleType.R, 52f, 8f, 7f),
		new MoleData(4, MoleType.N, 52f, 8f, 7f),
		new MoleData(5, MoleType.N, 52f, 8f, 7f),
		new MoleData(6, MoleType.N, 52f, 8f, 7f),
		new MoleData(7, MoleType.R, 52f, 8f, 7f),
		new MoleData(8, MoleType.R, 52f, 8f, 7f)
	};
	public void Init()
	{
		for (int i = 0; i < targetGroups.Length; i++)
		{
			targetGroups[i].Init(SingletonCustom<WhackMoleCharacterManager>.Instance.GetPlayerNo(i), i);
		}
		moleDataList = new List<MoleData>();
		moleDataList.AddRange(GetRandomMoleData());
	}
	public void SecondGroupInit()
	{
		for (int i = 0; i < targetGroups.Length; i++)
		{
			targetGroups[i].SecondGroupInit(SingletonCustom<WhackMoleCharacterManager>.Instance.GetPlayerNo(i), i);
		}
		moleDataList = new List<MoleData>();
		moleDataList.AddRange(GetRandomMoleData());
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < targetGroups.Length; i++)
		{
			targetGroups[i].UpdateMethod();
		}
		float gameTime = SingletonCustom<WhackMoleGameManager>.Instance.GameTime;
		for (int j = 0; j < moleDataList.Count; j++)
		{
			if (moleDataList[j].startGameTime < gameTime)
			{
				for (int k = 0; k < targetGroups.Length; k++)
				{
					targetGroups[k].ShowMole(moleDataList[j]);
				}
				moleDataList.RemoveAt(j);
				j--;
			}
		}
	}
	public void PlayFeverEffect()
	{
		for (int i = 0; i < targetGroups.Length; i++)
		{
			targetGroups[i].PlayHoleEffect();
		}
	}
	public void WhackReceive(int _charaNo, int _holeNo)
	{
		targetGroups[_charaNo].WhackCheck(_holeNo);
	}
	public float GetMoleTopPosY()
	{
		return targetGroups[0].transform.position.y;
	}
	private MoleData[] GetRandomMoleData()
	{
		switch (UnityEngine.Random.Range(0, 8))
		{
		case 0:
			return MOLE_DATA_NEW_A;
		case 1:
			return MOLE_DATA_NEW_B;
		case 2:
			return MOLE_DATA_NEW_C;
		case 3:
			return MOLE_DATA_NEW_D;
		case 4:
			return MOLE_DATA_NEW_A_REVERSE;
		case 5:
			return MOLE_DATA_NEW_B_REVERSE;
		case 6:
			return MOLE_DATA_NEW_C_REVERSE;
		case 7:
			return MOLE_DATA_NEW_D_REVERSE;
		default:
			return null;
		}
	}
	private MoleData[] GetRandomCreateMoleData()
	{
		MoleData[] array = new MoleData[67];
		int num = 19;
		List<int> source = new List<int>
		{
			1,
			2,
			2,
			2,
			2,
			2,
			2,
			3,
			3
		};
		source = (from a in source
			orderby Guid.NewGuid()
			select a).ToList();
		if (source[4] == 3)
		{
			do
			{
				int index = UnityEngine.Random.Range(0, source.Count);
				int value = source[index];
				source[index] = source[4];
				source[4] = value;
			}
			while (source[4] == 3);
		}
		List<int> list = new List<int>();
		for (int i = 0; i < source.Count; i++)
		{
			for (int j = 0; j < source[i]; j++)
			{
				list.Add(i);
			}
		}
		list = (from a in list
			orderby Guid.NewGuid()
			select a).ToList();
		for (int k = 2; k < list.Count; k++)
		{
			while (list[k] == list[k - 1] || list[k] == list[k - 2])
			{
				int index2 = UnityEngine.Random.Range(0, list.Count);
				int value2 = list[k];
				list[k] = list[index2];
				list[index2] = value2;
			}
		}
		List<MoleType> list2 = new List<MoleType>();
		List<int> list3 = new List<int>();
		for (int l = 0; l < num; l++)
		{
			list2.Add(MoleType.N);
		}
		list3.Add(UnityEngine.Random.Range(4, 10));
		list3.Add(UnityEngine.Random.Range(10, 15));
		list3.Add(UnityEngine.Random.Range(15, 19));
		for (int m = 0; m < list3.Count; m++)
		{
			list2[list3[m]] = MoleType.R;
		}
		List<float> list4 = new List<float>();
		List<float> list5 = new List<float>();
		for (int n = 0; n < num; n++)
		{
			list4.Add(n);
			if (n > 8)
			{
				list5.Add(0f);
			}
		}
		list5.Add(0.5f);
		list5.Add(0.5f);
		list5.Add(1f);
		list5.Add(1f);
		list5.Add(-0.5f);
		list5.Add(-1f);
		list5 = (from a in list5
			orderby Guid.NewGuid()
			select a).ToList();
		for (int num2 = 0; num2 < 3; num2++)
		{
			list5.Add(0f);
			float value3 = list5[num2];
			list5[num2] = list5[list5.Count - 1];
			list5[list5.Count - 1] = value3;
		}
		for (int num3 = 0; num3 < num; num3++)
		{
			List<float> list6 = list4;
			int index3 = num3;
			list6[index3] += list5[num3];
		}
		for (int num4 = num - 1; num4 > 0; num4--)
		{
			if (list4[num4] < list4[num4 - 1])
			{
				float value4 = list4[num4];
				list4[num4] = list4[num4 - 1];
				list4[num4 - 1] = value4;
			}
		}
		List<float> list7 = new List<float>();
		List<float> list8 = new List<float>();
		for (int num5 = 0; num5 < num; num5++)
		{
			list7.Add(3f);
			list8.Add(1.5f);
		}
		int index4 = list3[UnityEngine.Random.Range(0, list3.Count)];
		list7[index4] = 8f;
		list8[index4] = 0.75f;
		index4 = list3[UnityEngine.Random.Range(0, list3.Count)];
		list7[index4] = 6f;
		list8[index4] = 1f;
		if (15 <= index4 && index4 < 19)
		{
			int index5 = UnityEngine.Random.Range(5, 10);
			list7[index5] = 6f;
			list8[index5] = 1f;
			index5 = UnityEngine.Random.Range(10, 15);
			list7[index5] = 6f;
			list8[index5] = 1f;
		}
		else if (10 <= index4 && index4 < 15)
		{
			int index6 = UnityEngine.Random.Range(5, 10);
			list7[index6] = 6f;
			list8[index6] = 1f;
			index6 = UnityEngine.Random.Range(15, 19);
			list7[index6] = 6f;
			list8[index6] = 1f;
		}
		else
		{
			int index7 = UnityEngine.Random.Range(10, 15);
			list7[index7] = 6f;
			list8[index7] = 1f;
			index7 = UnityEngine.Random.Range(15, 19);
			list7[index7] = 6f;
			list8[index7] = 1f;
		}
		for (int num6 = 0; num6 < num; num6++)
		{
			array[num6] = new MoleData(list[num6], list2[num6], list4[num6], list7[num6], list8[num6]);
		}
		int num7 = 19;
		source = new List<int>
		{
			1,
			1,
			2,
			2,
			2,
			2,
			3,
			3,
			3
		};
		source = (from a in source
			orderby Guid.NewGuid()
			select a).ToList();
		if (source[4] != 1)
		{
			do
			{
				int index8 = UnityEngine.Random.Range(0, source.Count);
				int value5 = source[index8];
				source[index8] = source[4];
				source[4] = value5;
			}
			while (source[4] != 1);
		}
		list = new List<int>();
		for (int num8 = 0; num8 < source.Count; num8++)
		{
			for (int num9 = 0; num9 < source[num8]; num9++)
			{
				list.Add(num8);
			}
		}
		list = (from a in list
			orderby Guid.NewGuid()
			select a).ToList();
		for (int num10 = 2; num10 < list.Count; num10++)
		{
			while (list[num10] == list[num10 - 1] || list[num10] == list[num10 - 2])
			{
				int index9 = UnityEngine.Random.Range(0, list.Count);
				int value6 = list[num10];
				list[num10] = list[index9];
				list[index9] = value6;
			}
		}
		list2 = new List<MoleType>();
		list3 = new List<int>();
		for (int num11 = 0; num11 < num7; num11++)
		{
			list2.Add(MoleType.N);
		}
		list3.Add(UnityEngine.Random.Range(0, 5));
		list3.Add(UnityEngine.Random.Range(5, 10));
		list3.Add(UnityEngine.Random.Range(10, 15));
		list3.Add(UnityEngine.Random.Range(15, 19));
		for (int num12 = 0; num12 < list3.Count; num12++)
		{
			list2[list3[num12]] = MoleType.R;
		}
		list4 = new List<float>();
		list5 = new List<float>();
		for (int num13 = 0; num13 < num7; num13++)
		{
			list4.Add(20 + num13);
			if (num13 > 5)
			{
				list5.Add(0f);
			}
		}
		list5.Add(0.5f);
		list5.Add(1f);
		list5.Add(1f);
		list5.Add(-0.5f);
		list5.Add(-0.5f);
		list5.Add(-1f);
		list5 = (from a in list5
			orderby Guid.NewGuid()
			select a).ToList();
		for (int num14 = 0; num14 < num7; num14++)
		{
			List<float> list6 = list4;
			int index3 = num14;
			list6[index3] += list5[num14];
		}
		for (int num15 = num7 - 1; num15 > 0; num15--)
		{
			if (list4[num15] < list4[num15 - 1])
			{
				float value7 = list4[num15];
				list4[num15] = list4[num15 - 1];
				list4[num15 - 1] = value7;
			}
		}
		list7 = new List<float>();
		list8 = new List<float>();
		for (int num16 = 0; num16 < num7; num16++)
		{
			list7.Add(3f);
			list8.Add(1.5f);
		}
		list3 = (from a in list3
			orderby Guid.NewGuid()
			select a).ToList();
		index4 = list3[0];
		int index10 = list3[1];
		list7[index4] = 6f;
		list8[index4] = 1f;
		list7[index10] = 8f;
		list8[index10] = 0.75f;
		for (int num17 = 0; num17 < list3.Count; num17++)
		{
			int num18 = 0;
			num18 = ((0 <= list3[num17] && list3[num17] < 5) ? UnityEngine.Random.Range(0, 5) : ((5 <= list3[num17] && list3[num17] < 10) ? UnityEngine.Random.Range(5, 10) : ((10 > list3[num17] || list3[num17] >= 15) ? UnityEngine.Random.Range(15, 19) : UnityEngine.Random.Range(10, 15))));
			if (num17 == 2)
			{
				list7[num18] = 8f;
				list8[num18] = 0.75f;
			}
			else
			{
				list7[num18] = 6f;
				list8[num18] = 1f;
			}
		}
		for (int num19 = 0; num19 < num7; num19++)
		{
			array[19 + num19] = new MoleData(list[num19], list2[num19], list4[num19], list7[num19], list8[num19]);
		}
		int num20 = 29;
		source = new List<int>
		{
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			4,
			4
		};
		source = (from a in source
			orderby Guid.NewGuid()
			select a).ToList();
		if (source[4] == 4)
		{
			do
			{
				int index11 = UnityEngine.Random.Range(0, source.Count);
				int value8 = source[index11];
				source[index11] = source[4];
				source[4] = value8;
			}
			while (source[4] == 4);
		}
		list = new List<int>();
		for (int num21 = 0; num21 < source.Count; num21++)
		{
			for (int num22 = 0; num22 < source[num21]; num22++)
			{
				list.Add(num21);
			}
		}
		list = (from a in list
			orderby Guid.NewGuid()
			select a).ToList();
		for (int num23 = 2; num23 < list.Count; num23++)
		{
			while (list[num23] == list[num23 - 1] || list[num23] == list[num23 - 2])
			{
				int index12 = UnityEngine.Random.Range(0, list.Count);
				int value9 = list[num23];
				list[num23] = list[index12];
				list[index12] = value9;
			}
		}
		list2 = new List<MoleType>();
		list3 = new List<int>();
		for (int num24 = 0; num24 < num20; num24++)
		{
			list2.Add(MoleType.N);
		}
		list3.Add(UnityEngine.Random.Range(0, 4));
		list3.Add(UnityEngine.Random.Range(4, 8));
		list3.Add(UnityEngine.Random.Range(8, 11));
		list3.Add(UnityEngine.Random.Range(11, 14));
		list3.Add(UnityEngine.Random.Range(14, 17));
		list3.Add(UnityEngine.Random.Range(17, 20));
		list3.Add(UnityEngine.Random.Range(20, 23));
		list3.Add(UnityEngine.Random.Range(23, 26));
		list3.Add(UnityEngine.Random.Range(26, 29));
		for (int num25 = 0; num25 < list3.Count; num25++)
		{
			list2[list3[num25]] = MoleType.R;
		}
		list4 = new List<float>();
		list5 = new List<float>();
		float num26 = 42f;
		for (int num27 = 0; num27 < num20; num27++)
		{
			list4.Add(num26);
			if (num27 % 5 == 1 || num27 % 5 == 2 || num27 % 5 == 4)
			{
				num26 += 1f;
			}
			if (num27 > 4)
			{
				list5.Add(0f);
			}
		}
		list5.Add(0.5f);
		list5.Add(-0.5f);
		list5.Add(-0.5f);
		list5.Add(-1f);
		list5.Add(-1f);
		list5 = (from a in list5
			orderby Guid.NewGuid()
			select a).ToList();
		for (int num28 = 0; num28 < num20; num28++)
		{
			List<float> list6 = list4;
			int index3 = num28;
			list6[index3] += list5[num28];
		}
		for (int num29 = num20 - 1; num29 > 0; num29--)
		{
			if (list4[num29] < list4[num29 - 1])
			{
				float value10 = list4[num29];
				list4[num29] = list4[num29 - 1];
				list4[num29 - 1] = value10;
			}
		}
		list7 = new List<float>();
		list8 = new List<float>();
		for (int num30 = 0; num30 < num20; num30++)
		{
			if (num30 < 2)
			{
				list7.Add(8f);
				list8.Add(0.75f);
			}
			else if (num30 < 14)
			{
				list7.Add(6f);
				list8.Add(1f);
			}
			else
			{
				list7.Add(6f);
				list8.Add(1.5f);
			}
		}
		for (int num31 = 0; num31 < 50; num31++)
		{
			int index13 = UnityEngine.Random.Range(0, num20);
			int index14 = UnityEngine.Random.Range(0, num20);
			float value11 = list7[index13];
			list7[index13] = list7[index14];
			list7[index14] = value11;
			value11 = list8[index13];
			list8[index13] = list8[index14];
			list8[index14] = value11;
		}
		for (int num32 = 0; num32 < num20; num32++)
		{
			array[38 + num32] = new MoleData(list[num32], list2[num32], list4[num32], list7[num32], list8[num32]);
		}
		return array;
	}
	public List<int> GetAiTargetNoList_Normal(int _charaNo)
	{
		return targetGroups[_charaNo].SearchAiTargetNoList_Normal();
	}
	public List<int> GetAiTargetNoList_Rare(int _charaNo)
	{
		return targetGroups[_charaNo].SearchAiTargetNoList_Rare();
	}
	private void ExportMoleDataScriptLog(MoleData[] _datas)
	{
		string text = "private static readonly MoleData[] MOLE_DATA = new MoleData[] {\n";
		for (int i = 0; i < _datas.Length; i++)
		{
			text += "new MoleData(";
			text = text + _datas[i].holeNo.ToString() + ", MoleType.";
			text = text + _datas[i].moleType.ToString() + ", ";
			text = text + _datas[i].startGameTime.ToString("0.0") + "f, ";
			text = ((!(_datas[i].moveSpeed < 3.1f)) ? ((!(_datas[i].moveSpeed < 6.1f)) ? (text + "SPEED_C, ") : (text + "SPEED_B, ")) : (text + "SPEED_A, "));
			text = ((!(_datas[i].stayTime < 0.85f)) ? ((!(_datas[i].stayTime < 1.1f)) ? ((!(_datas[i].stayTime < 1.6f)) ? (text + "TIME_D)") : (text + "TIME_A)")) : (text + "TIME_B)")) : (text + "TIME_C)"));
			text = ((i >= _datas.Length - 1) ? (text + "\n};") : (text + ",\n"));
		}
		UnityEngine.Debug.Log(text);
	}
	private void ExportMoleDataScriptLog_ReverseHoleNo(MoleData[] _datas)
	{
		UnityEngine.Debug.Log("ExportMoleDataScriptLog_ReverseHoleNo");
		for (int i = 0; i < _datas.Length; i++)
		{
			_datas[i].holeNo = 9 - _datas[i].holeNo - 1;
			_datas[i].holeType = (HoleType)_datas[i].holeNo;
		}
		ExportMoleDataScriptLog(_datas);
	}
	private void MoleDataAnalysis(MoleData[] _datas)
	{
		int[] array = new int[9];
		int[] array2 = new int[9];
		for (int i = 0; i < _datas.Length; i++)
		{
			array[_datas[i].holeNo]++;
			if (_datas[i].moleType == MoleType.R)
			{
				array2[_datas[i].holeNo]++;
			}
		}
		UnityEngine.Debug.Log("もぐらの出現分布");
		UnityEngine.Debug.Log(array[0].ToString() + "," + array[1].ToString() + "," + array[2].ToString());
		UnityEngine.Debug.Log(array[3].ToString() + "," + array[4].ToString() + "," + array[5].ToString());
		UnityEngine.Debug.Log(array[6].ToString() + "," + array[7].ToString() + "," + array[8].ToString());
		UnityEngine.Debug.Log("レアの出現分布");
		UnityEngine.Debug.Log(array2[0].ToString() + "," + array2[1].ToString() + "," + array2[2].ToString());
		UnityEngine.Debug.Log(array2[3].ToString() + "," + array2[4].ToString() + "," + array2[5].ToString());
		UnityEngine.Debug.Log(array2[6].ToString() + "," + array2[7].ToString() + "," + array2[8].ToString());
		UnityEngine.Debug.Log("出現時間の被りのチェック");
		int num = 0;
		for (int j = 0; j < 9; j++)
		{
			float num2 = -10f;
			for (int k = 0; k < _datas.Length; k++)
			{
				if (j == _datas[k].holeNo)
				{
					float startGameTime = _datas[k].startGameTime;
					float num3 = startGameTime + _datas[k].stayTime + 1f;
					num3 = ((_datas[k].moveSpeed < 3.5f) ? (num3 + 1.4f) : ((!(_datas[k].moveSpeed < 6.5f)) ? (num3 + 0.5f) : (num3 + 0.7f)));
					if (startGameTime < num2)
					{
						UnityEngine.Debug.Log((k + 1).ToString() + "番目");
						UnityEngine.Debug.Log("ホ\u30fcル番号：" + j.ToString());
						UnityEngine.Debug.Log("出現時間：" + startGameTime.ToString());
						num++;
					}
					num2 = num3;
				}
			}
		}
		UnityEngine.Debug.Log("被り回数：" + num.ToString());
	}
}
