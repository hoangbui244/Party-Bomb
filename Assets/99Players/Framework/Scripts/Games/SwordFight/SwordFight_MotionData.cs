using System;
using UnityEngine;
public class SwordFight_MotionData : MonoBehaviour
{
	public enum MotionType
	{
		Default,
		VerticalSlash_1st_SwingStandby,
		VerticalSlash_1st_Slash,
		VerticalSlash_2nd_SwingStandby,
		VerticalSlash_2nd_Slash,
		VerticalSlash_Last_SwingStandby,
		VerticalSlash_Last_Slash,
		HorizontalSlash_1st_SwingStandby,
		HorizontalSlash_1st_Slash,
		HorizontalSlash_2nd_SwingStandby,
		HorizontalSlash_2nd_Slash,
		HorizontalSlash_Last_SwingStandby,
		HorizontalSlash_Last_Slash,
		Deffence,
		NONE
	}
	[Serializable]
	public struct PoseData
	{
		[Header("ポ\u30fcズ情報：位置")]
		public Vector3 pose_Pos;
		[Header("ポ\u30fcズ情報：角度")]
		public Vector3 pose_Angle;
		[Header("ポ\u30fcズ情報：スケ\u30fcル")]
		public Vector3 pose_Scale;
	}
	[Serializable]
	public struct MotionData
	{
		[Header("モ\u30fcション時間")]
		public float motionTime;
		[Header("頭のポ\u30fcズ情報")]
		public PoseData pose_HEAD;
		[Header("体のポ\u30fcズ情報")]
		public PoseData pose_BODY;
		[Header("腰のポ\u30fcズ情報")]
		public PoseData pose_HIP;
		[Header("左肩のポ\u30fcズ情報")]
		public PoseData pose_SHOULDER_L;
		[Header("右肩のポ\u30fcズ情報")]
		public PoseData pose_SHOULDER_R;
		[Header("左腕のポ\u30fcズ情報")]
		public PoseData pose_ARM_L;
		[Header("右腕のポ\u30fcズ情報")]
		public PoseData pose_ARM_R;
		[Header("左足のポ\u30fcズ情報")]
		public PoseData pose_LEG_L;
		[Header("右足のポ\u30fcズ情報")]
		public PoseData pose_LEG_R;
		[Header("剣のポ\u30fcズ情報")]
		public PoseData pose_SWORD;
	}
	[SerializeField]
	[Header("デフォルトのモ\u30fcションデ\u30fcタ")]
	private MotionData motionData_Default;
	[SerializeField]
	[Header("1st縦斬りの振りかぶりモ\u30fcションデ\u30fcタ")]
	private MotionData motionData_VerticalSlash_1st_SwingStandBy;
	[SerializeField]
	[Header("1st縦斬りの振り下ろしモ\u30fcションデ\u30fcタ")]
	private MotionData motionData_VerticalSlash_1st_Slash;
	[SerializeField]
	[Header("2nd縦斬りの振りかぶりモ\u30fcションデ\u30fcタ")]
	private MotionData motionData_VerticalSlash_2nd_SwingStandBy;
	[SerializeField]
	[Header("2nd縦斬りの振り下ろしモ\u30fcションデ\u30fcタ")]
	private MotionData motionData_VerticalSlash_2nd_Slash;
	[SerializeField]
	[Header("Last縦斬りの振りかぶりモ\u30fcションデ\u30fcタ")]
	private MotionData motionData_VerticalSlash_Last_SwingStandBy;
	[SerializeField]
	[Header("Last縦斬りの振り下ろしモ\u30fcションデ\u30fcタ")]
	private MotionData motionData_VerticalSlash_Last_Slash;
	[SerializeField]
	[Header("1st横斬りの振りかぶりモ\u30fcションデ\u30fcタ")]
	private MotionData motionData_HorizontalSlash_1st_SwingStandBy;
	[SerializeField]
	[Header("1st横斬りの振り下ろしモ\u30fcションデ\u30fcタ")]
	private MotionData motionData_HorizontalSlash_1st_Slash;
	[SerializeField]
	[Header("2nd横斬りの振りかぶりモ\u30fcションデ\u30fcタ")]
	private MotionData motionData_HorizontalSlash_2nd_SwingStandBy;
	[SerializeField]
	[Header("2nd横斬りの振り下ろしモ\u30fcションデ\u30fcタ")]
	private MotionData motionData_HorizontalSlash_2nd_Slash;
	[SerializeField]
	[Header("Last横斬りの振りかぶりモ\u30fcションデ\u30fcタ")]
	private MotionData motionData_HorizontalSlash_Last_SwingStandBy;
	[SerializeField]
	[Header("Last横斬りの振り下ろしモ\u30fcションデ\u30fcタ")]
	private MotionData motionData_HorizontalSlash_Last_Slash;
	[SerializeField]
	[Header("防御のモ\u30fcションデ\u30fcタ")]
	private MotionData motionData_Deffence;
	public MotionData GetMotionData(MotionType _type)
	{
		switch (_type)
		{
		case MotionType.VerticalSlash_1st_SwingStandby:
			return motionData_VerticalSlash_1st_SwingStandBy;
		case MotionType.VerticalSlash_1st_Slash:
			return motionData_VerticalSlash_1st_Slash;
		case MotionType.VerticalSlash_2nd_SwingStandby:
			return motionData_VerticalSlash_2nd_SwingStandBy;
		case MotionType.VerticalSlash_2nd_Slash:
			return motionData_VerticalSlash_2nd_Slash;
		case MotionType.VerticalSlash_Last_SwingStandby:
			return motionData_VerticalSlash_Last_SwingStandBy;
		case MotionType.VerticalSlash_Last_Slash:
			return motionData_VerticalSlash_Last_Slash;
		case MotionType.HorizontalSlash_1st_SwingStandby:
			return motionData_HorizontalSlash_1st_SwingStandBy;
		case MotionType.HorizontalSlash_1st_Slash:
			return motionData_HorizontalSlash_1st_Slash;
		case MotionType.HorizontalSlash_2nd_SwingStandby:
			return motionData_HorizontalSlash_2nd_SwingStandBy;
		case MotionType.HorizontalSlash_2nd_Slash:
			return motionData_HorizontalSlash_2nd_Slash;
		case MotionType.HorizontalSlash_Last_SwingStandby:
			return motionData_HorizontalSlash_Last_SwingStandBy;
		case MotionType.HorizontalSlash_Last_Slash:
			return motionData_HorizontalSlash_Last_Slash;
		case MotionType.Deffence:
			return motionData_Deffence;
		default:
			return motionData_Default;
		}
	}
}
