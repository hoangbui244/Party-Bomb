using System;
using UnityEngine;
public class ShortTrack_CharaLeanTweenMotionData
{
	[Serializable]
	public struct PoseData
	{
		[Header("モ\u30fcションの往復ル\u30fcプ化")]
		public bool Loop;
		[Header("モ\u30fcションの反復ル\u30fcプ化")]
		public bool motionRepeat;
		[Header("ポ\u30fcズ情報：位置")]
		public Vector3[] pose_Pos;
		[Header("ポ\u30fcズ情報：角度")]
		public Vector3[] pose_Angle;
		[Header("ポ\u30fcズ情報：スケ\u30fcル")]
		public Vector3[] pose_Scale;
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
	}
	private static readonly float INIT_ANIMATION_TIME = 0.5f;
	private static readonly float DashStandby_Time = 0.5f;
	private static readonly float Dash_Time = 0.3f;
	private static readonly float FullPowerDash_Time = 0.15f;
	private static readonly float SlowDash_Time = 0.5f;
	private static readonly float Curve_Time = 0.4f;
	private static readonly float FullCurve_Time = 0.3f;
	private static readonly float Tired_Time = 0.6f;
	private static readonly float TiredCurve_Time = 0.6f;
	private static MotionData defaultMotionData;
	private static MotionData dashStandbyMotionData;
	private static MotionData dashMotionData;
	private static MotionData fullPowerDashMotionData;
	private static MotionData slowDashMotionData;
	private static MotionData curveMotionData;
	private static MotionData fullPowerCurveMotionData;
	private static MotionData tiredMotionData;
	private static MotionData tiredCurveMotionData;
	public static void CreateMotionData()
	{
		SetDefaultMotionData();
		SetDashStandbyMotionData();
		SetDashMotionData();
		SetFullPowerDashData();
		SetSlowDashData();
		SetCurveMotionData();
		SetFullPowerCurveMotionDataMotionData();
		SetTiredMotionDataMotionData();
		SetTiredCurveMotionData();
	}
	private static void SetDefaultMotionData()
	{
		defaultMotionData.motionTime = INIT_ANIMATION_TIME;
		defaultMotionData.pose_HEAD.pose_Pos = new Vector3[1]
		{
			new Vector3(0f, 0.1735753f, 0f)
		};
		defaultMotionData.pose_HEAD.pose_Angle = new Vector3[1]
		{
			Vector3.zero
		};
		defaultMotionData.pose_HEAD.pose_Scale = new Vector3[1]
		{
			Vector3.one
		};
		defaultMotionData.pose_BODY.pose_Pos = new Vector3[1]
		{
			Vector3.zero
		};
		defaultMotionData.pose_BODY.pose_Angle = new Vector3[1]
		{
			Vector3.zero
		};
		defaultMotionData.pose_BODY.pose_Scale = new Vector3[1]
		{
			Vector3.one
		};
		defaultMotionData.pose_HIP.pose_Pos = new Vector3[1]
		{
			new Vector3(0f, 0.05483828f, 0f)
		};
		defaultMotionData.pose_HIP.pose_Angle = new Vector3[1]
		{
			Vector3.zero
		};
		defaultMotionData.pose_HIP.pose_Scale = new Vector3[1]
		{
			Vector3.one
		};
		defaultMotionData.pose_SHOULDER_L.pose_Pos = new Vector3[1]
		{
			new Vector3(-0.1511168f, 0.1297733f, 0f)
		};
		defaultMotionData.pose_SHOULDER_L.pose_Angle = new Vector3[1]
		{
			Vector3.zero
		};
		defaultMotionData.pose_SHOULDER_L.pose_Scale = new Vector3[1]
		{
			Vector3.one
		};
		defaultMotionData.pose_SHOULDER_R.pose_Pos = new Vector3[1]
		{
			new Vector3(0.1511168f, 0.1297733f, 0f)
		};
		defaultMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[1]
		{
			Vector3.zero
		};
		defaultMotionData.pose_SHOULDER_R.pose_Scale = new Vector3[1]
		{
			Vector3.one
		};
		defaultMotionData.pose_ARM_L.pose_Pos = new Vector3[1]
		{
			new Vector3(0.006473546f, -0.02895849f, 0f)
		};
		defaultMotionData.pose_ARM_L.pose_Angle = new Vector3[1]
		{
			Vector3.zero
		};
		defaultMotionData.pose_ARM_L.pose_Scale = new Vector3[1]
		{
			Vector3.one
		};
		defaultMotionData.pose_ARM_R.pose_Pos = new Vector3[1]
		{
			new Vector3(-0.006473546f, -0.02895849f, 0f)
		};
		defaultMotionData.pose_ARM_R.pose_Angle = new Vector3[1]
		{
			Vector3.zero
		};
		defaultMotionData.pose_ARM_R.pose_Scale = new Vector3[1]
		{
			Vector3.one
		};
		defaultMotionData.pose_LEG_L.pose_Pos = new Vector3[1]
		{
			new Vector3(-0.054f, -0.0483f, 0f)
		};
		defaultMotionData.pose_LEG_L.pose_Angle = new Vector3[1]
		{
			Vector3.zero
		};
		defaultMotionData.pose_LEG_L.pose_Scale = new Vector3[1]
		{
			Vector3.one
		};
		defaultMotionData.pose_LEG_R.pose_Pos = new Vector3[1]
		{
			new Vector3(0.054f, -0.0483f, 0f)
		};
		defaultMotionData.pose_LEG_R.pose_Angle = new Vector3[1]
		{
			Vector3.zero
		};
		defaultMotionData.pose_LEG_R.pose_Scale = new Vector3[1]
		{
			Vector3.one
		};
	}
	public static MotionData GetDefaultMotionData()
	{
		return defaultMotionData;
	}
	private static void SetDashStandbyMotionData()
	{
		dashStandbyMotionData = defaultMotionData;
		dashStandbyMotionData.motionTime = DashStandby_Time;
		dashStandbyMotionData.pose_HEAD.pose_Pos = new Vector3[1]
		{
			new Vector3(0f, 0.1735753f, 0f)
		};
		dashStandbyMotionData.pose_HEAD.pose_Angle = new Vector3[1]
		{
			new Vector3(330f, 350f, 15f)
		};
		dashStandbyMotionData.pose_BODY.pose_Pos = new Vector3[1]
		{
			new Vector3(0f, 0f, 0f)
		};
		dashStandbyMotionData.pose_BODY.pose_Angle = new Vector3[1]
		{
			new Vector3(15f, 330f, 4.419457E-07f)
		};
		dashStandbyMotionData.pose_HIP.pose_Pos = new Vector3[1]
		{
			new Vector3(0f, 0.05483828f, 0f)
		};
		dashStandbyMotionData.pose_HIP.pose_Angle = new Vector3[1]
		{
			new Vector3(30f, 45f, 0f)
		};
		dashStandbyMotionData.pose_SHOULDER_L.pose_Pos = new Vector3[1]
		{
			new Vector3(-0.1511168f, 0.1297733f, 0f)
		};
		dashStandbyMotionData.pose_SHOULDER_L.pose_Angle = new Vector3[1]
		{
			new Vector3(315f, 0f, 330f)
		};
		dashStandbyMotionData.pose_ARM_L.pose_Pos = new Vector3[1]
		{
			new Vector3(0.006473546f, -0.02895849f, 0f)
		};
		dashStandbyMotionData.pose_ARM_L.pose_Angle = new Vector3[1]
		{
			new Vector3(0f, 0f, 30f)
		};
		dashStandbyMotionData.pose_SHOULDER_R.pose_Pos = new Vector3[1]
		{
			new Vector3(0.1511168f, 0.1297733f, 0f)
		};
		dashStandbyMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[1]
		{
			new Vector3(330f, -9.85853E-07f, 45f)
		};
		dashStandbyMotionData.pose_ARM_R.pose_Pos = new Vector3[1]
		{
			new Vector3(-0.006473546f, -0.02895849f, 0f)
		};
		dashStandbyMotionData.pose_ARM_R.pose_Angle = new Vector3[1]
		{
			new Vector3(315f, 0f, 0f)
		};
		dashStandbyMotionData.pose_LEG_L.pose_Pos = new Vector3[1]
		{
			new Vector3(-0.054f, -0.0483f, 0f)
		};
		dashStandbyMotionData.pose_LEG_L.pose_Angle = new Vector3[1]
		{
			new Vector3(330f, 0f, 0f)
		};
		dashStandbyMotionData.pose_LEG_R.pose_Pos = new Vector3[1]
		{
			new Vector3(0.054f, -0.0483f, 0f)
		};
		dashStandbyMotionData.pose_LEG_R.pose_Angle = new Vector3[1]
		{
			new Vector3(330f, 30f, 0f)
		};
	}
	public static MotionData GetDashStandbyMotionData()
	{
		return dashStandbyMotionData;
	}
	private static void SetDashMotionData()
	{
		dashMotionData = defaultMotionData;
		dashMotionData.motionTime = Dash_Time;
		dashMotionData.pose_HEAD.pose_Pos = new Vector3[1]
		{
			new Vector3(0f, 0.1735753f, 0f)
		};
		dashMotionData.pose_HEAD.pose_Angle = new Vector3[1]
		{
			new Vector3(300f, 0f, 0f)
		};
		dashMotionData.pose_BODY.pose_Pos = new Vector3[1]
		{
			new Vector3(0f, 0f, 0f)
		};
		dashMotionData.pose_BODY.pose_Angle = new Vector3[1]
		{
			new Vector3(30f, 0f, 0f)
		};
		dashMotionData.pose_HIP.pose_Pos = new Vector3[1]
		{
			new Vector3(0f, 0.05483828f, 0f)
		};
		dashMotionData.pose_HIP.pose_Angle = new Vector3[1]
		{
			new Vector3(30f, 0f, 0f)
		};
		dashMotionData.pose_SHOULDER_L.pose_Pos = new Vector3[1]
		{
			new Vector3(-0.1511168f, 0.1297733f, 0f)
		};
		dashMotionData.pose_SHOULDER_L.pose_Angle = new Vector3[2]
		{
			new Vector3(270f, 0f, 0f),
			new Vector3(0f, 0f, 0f)
		};
		dashMotionData.pose_SHOULDER_L.Loop = true;
		dashMotionData.pose_ARM_L.pose_Pos = new Vector3[1]
		{
			new Vector3(0.006473546f, -0.02895849f, 0f)
		};
		dashMotionData.pose_ARM_L.pose_Angle = new Vector3[1]
		{
			new Vector3(315f, 0f, 0f)
		};
		dashMotionData.pose_SHOULDER_R.pose_Pos = new Vector3[1]
		{
			new Vector3(0.1511168f, 0.1297733f, 0f)
		};
		dashMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[2]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(270f, 0f, 0f)
		};
		dashMotionData.pose_SHOULDER_R.Loop = true;
		dashMotionData.pose_ARM_R.pose_Pos = new Vector3[1]
		{
			new Vector3(-0.006473546f, -0.02895849f, 0f)
		};
		dashMotionData.pose_ARM_R.pose_Angle = new Vector3[1]
		{
			new Vector3(315f, 0f, 0f)
		};
		dashMotionData.pose_LEG_L.pose_Pos = new Vector3[2]
		{
			new Vector3(-0.0652f, -0.0657f, -0.0433f),
			new Vector3(-0.0543f, -0.0248f, 0.0317f)
		};
		dashMotionData.pose_LEG_L.pose_Angle = new Vector3[2]
		{
			new Vector3(29.57459f, -9.816718E-06f, 340.924683f),
			new Vector3(330f, 0f, 0f)
		};
		dashMotionData.pose_LEG_L.Loop = true;
		dashMotionData.pose_LEG_R.pose_Pos = new Vector3[2]
		{
			new Vector3(0.0543f, -0.0248f, 0.0317f),
			new Vector3(0.0652f, -0.0657f, -0.0433f)
		};
		dashMotionData.pose_LEG_R.pose_Angle = new Vector3[2]
		{
			new Vector3(330f, 0f, 0f),
			new Vector3(29.57459f, -9.816718E-06f, 19.07531f)
		};
		dashMotionData.pose_LEG_R.Loop = true;
	}
	public static MotionData GetDashMotionData()
	{
		return dashMotionData;
	}
	private static void SetFullPowerDashData()
	{
		fullPowerDashMotionData = defaultMotionData;
		fullPowerDashMotionData.motionTime = FullPowerDash_Time;
		fullPowerDashMotionData.pose_HEAD.pose_Pos = new Vector3[1]
		{
			new Vector3(0f, 0.1735753f, 0f)
		};
		fullPowerDashMotionData.pose_HEAD.pose_Angle = new Vector3[1]
		{
			new Vector3(300f, 0f, 0f)
		};
		fullPowerDashMotionData.pose_BODY.pose_Pos = new Vector3[1]
		{
			new Vector3(0f, 0f, 0f)
		};
		fullPowerDashMotionData.pose_BODY.pose_Angle = new Vector3[1]
		{
			new Vector3(45f, 0f, 0f)
		};
		fullPowerDashMotionData.pose_HIP.pose_Pos = new Vector3[1]
		{
			new Vector3(0f, 0.05483828f, 0f)
		};
		fullPowerDashMotionData.pose_HIP.pose_Angle = new Vector3[1]
		{
			new Vector3(30f, 0f, 0f)
		};
		fullPowerDashMotionData.pose_SHOULDER_L.pose_Pos = new Vector3[1]
		{
			new Vector3(-0.1511168f, 0.1297733f, 0f)
		};
		fullPowerDashMotionData.pose_SHOULDER_L.pose_Angle = new Vector3[2]
		{
			new Vector3(270f, 0f, 0f),
			new Vector3(0f, 0f, 0f)
		};
		fullPowerDashMotionData.pose_SHOULDER_L.Loop = true;
		fullPowerDashMotionData.pose_SHOULDER_L.Loop = true;
		fullPowerDashMotionData.pose_ARM_L.pose_Pos = new Vector3[1]
		{
			new Vector3(0.006473546f, -0.02895849f, 0f)
		};
		fullPowerDashMotionData.pose_ARM_L.pose_Angle = new Vector3[1]
		{
			new Vector3(315f, 0f, 0f)
		};
		fullPowerDashMotionData.pose_SHOULDER_R.pose_Pos = new Vector3[1]
		{
			new Vector3(0.1511168f, 0.1297733f, 0f)
		};
		fullPowerDashMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[2]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(270f, 0f, 0f)
		};
		fullPowerDashMotionData.pose_SHOULDER_R.Loop = true;
		fullPowerDashMotionData.pose_ARM_R.pose_Pos = new Vector3[1]
		{
			new Vector3(-0.006473546f, -0.02895849f, 0f)
		};
		fullPowerDashMotionData.pose_ARM_R.pose_Angle = new Vector3[1]
		{
			new Vector3(315f, 0f, 0f)
		};
		fullPowerDashMotionData.pose_LEG_L.pose_Pos = new Vector3[2]
		{
			new Vector3(-0.0652f, -0.0657f, -0.0433f),
			new Vector3(-0.0543f, -0.0248f, 0.0317f)
		};
		fullPowerDashMotionData.pose_LEG_L.pose_Angle = new Vector3[2]
		{
			new Vector3(29.57459f, -9.816718E-06f, 340.924683f),
			new Vector3(330f, 0f, 0f)
		};
		fullPowerDashMotionData.pose_LEG_L.Loop = true;
		fullPowerDashMotionData.pose_LEG_R.pose_Pos = new Vector3[2]
		{
			new Vector3(0.0543f, -0.0248f, 0.0317f),
			new Vector3(0.0652f, -0.0657f, -0.0433f)
		};
		fullPowerDashMotionData.pose_LEG_R.pose_Angle = new Vector3[2]
		{
			new Vector3(330f, 0f, 0f),
			new Vector3(29.57459f, -9.816718E-06f, 19.07531f)
		};
		fullPowerDashMotionData.pose_LEG_R.Loop = true;
	}
	public static MotionData GetFullPowerDashMotionData()
	{
		return fullPowerDashMotionData;
	}
	private static void SetSlowDashData()
	{
		slowDashMotionData = defaultMotionData;
		slowDashMotionData.motionTime = SlowDash_Time;
		slowDashMotionData.pose_HEAD.pose_Pos = new Vector3[1]
		{
			new Vector3(0f, 0.1735753f, 0f)
		};
		slowDashMotionData.pose_HEAD.pose_Angle = new Vector3[1]
		{
			new Vector3(315f, 0f, 0f)
		};
		slowDashMotionData.pose_BODY.pose_Pos = new Vector3[1]
		{
			new Vector3(0f, 0f, 0f)
		};
		slowDashMotionData.pose_BODY.pose_Angle = new Vector3[1]
		{
			new Vector3(15f, 0f, 0f)
		};
		slowDashMotionData.pose_HIP.pose_Pos = new Vector3[1]
		{
			new Vector3(0f, 0.05483828f, 0f)
		};
		slowDashMotionData.pose_HIP.pose_Angle = new Vector3[1]
		{
			new Vector3(30f, 0f, 0f)
		};
		slowDashMotionData.pose_SHOULDER_L.pose_Pos = new Vector3[1]
		{
			new Vector3(-0.1511168f, 0.1297733f, 0f)
		};
		slowDashMotionData.pose_SHOULDER_L.pose_Angle = new Vector3[2]
		{
			new Vector3(270f, 0f, 0f),
			new Vector3(0f, 0f, 0f)
		};
		slowDashMotionData.pose_SHOULDER_L.Loop = true;
		slowDashMotionData.pose_SHOULDER_L.Loop = true;
		slowDashMotionData.pose_ARM_L.pose_Pos = new Vector3[1]
		{
			new Vector3(0.006473546f, -0.02895849f, 0f)
		};
		slowDashMotionData.pose_ARM_L.pose_Angle = new Vector3[1]
		{
			new Vector3(315f, 0f, 0f)
		};
		slowDashMotionData.pose_SHOULDER_R.pose_Pos = new Vector3[1]
		{
			new Vector3(0.1511168f, 0.1297733f, 0f)
		};
		slowDashMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[2]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(270f, 0f, 0f)
		};
		slowDashMotionData.pose_SHOULDER_R.Loop = true;
		slowDashMotionData.pose_ARM_R.pose_Pos = new Vector3[1]
		{
			new Vector3(-0.006473546f, -0.02895849f, 0f)
		};
		slowDashMotionData.pose_ARM_R.pose_Angle = new Vector3[1]
		{
			new Vector3(315f, 0f, 0f)
		};
		slowDashMotionData.pose_LEG_L.pose_Pos = new Vector3[2]
		{
			new Vector3(-0.0652f, -0.0657f, -0.0433f),
			new Vector3(-0.0543f, -0.0248f, 0.0317f)
		};
		slowDashMotionData.pose_LEG_L.pose_Angle = new Vector3[2]
		{
			new Vector3(29.57459f, -9.816718E-06f, 340.924683f),
			new Vector3(330f, 0f, 0f)
		};
		slowDashMotionData.pose_LEG_L.Loop = true;
		slowDashMotionData.pose_LEG_R.pose_Pos = new Vector3[2]
		{
			new Vector3(0.0543f, -0.0248f, 0.0317f),
			new Vector3(0.0652f, -0.0657f, -0.0433f)
		};
		slowDashMotionData.pose_LEG_R.pose_Angle = new Vector3[2]
		{
			new Vector3(330f, 0f, 0f),
			new Vector3(29.57459f, -9.816718E-06f, 19.07531f)
		};
		slowDashMotionData.pose_LEG_R.Loop = true;
	}
	public static MotionData GetSlowDashMotionData()
	{
		return slowDashMotionData;
	}
	private static void SetCurveMotionData()
	{
		curveMotionData = defaultMotionData;
		curveMotionData.motionTime = Curve_Time;
		curveMotionData.pose_HEAD.pose_Pos = new Vector3[1]
		{
			new Vector3(0f, 0.1735753f, 0f)
		};
		curveMotionData.pose_HEAD.pose_Angle = new Vector3[1]
		{
			new Vector3(330f, 0f, 0f)
		};
		curveMotionData.pose_BODY.pose_Pos = new Vector3[1]
		{
			new Vector3(0f, 0f, 0f)
		};
		curveMotionData.pose_BODY.pose_Angle = new Vector3[1]
		{
			new Vector3(30f, 0f, 15f)
		};
		curveMotionData.pose_HIP.pose_Pos = new Vector3[1]
		{
			new Vector3(0f, 0.05483828f, 0f)
		};
		curveMotionData.pose_HIP.pose_Angle = new Vector3[1]
		{
			new Vector3(15f, 0f, 0f)
		};
		curveMotionData.pose_SHOULDER_L.pose_Pos = new Vector3[1]
		{
			new Vector3(-0.1511168f, 0.1297733f, 0f)
		};
		curveMotionData.pose_SHOULDER_L.pose_Angle = new Vector3[2]
		{
			new Vector3(315f, 0f, 0f),
			new Vector3(0f, 0f, 0f)
		};
		curveMotionData.pose_SHOULDER_L.Loop = true;
		curveMotionData.pose_SHOULDER_R.pose_Pos = new Vector3[1]
		{
			new Vector3(0.1511168f, 0.1297733f, 0f)
		};
		curveMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[2]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(315f, 0f, 0f)
		};
		curveMotionData.pose_SHOULDER_R.Loop = true;
		curveMotionData.pose_ARM_L.pose_Pos = new Vector3[1]
		{
			new Vector3(0.006473546f, -0.02895849f, 0f)
		};
		curveMotionData.pose_ARM_L.pose_Angle = new Vector3[1]
		{
			new Vector3(330f, 0f, 0f)
		};
		curveMotionData.pose_ARM_R.pose_Pos = new Vector3[1]
		{
			new Vector3(-0.006473546f, -0.02895849f, 0f)
		};
		curveMotionData.pose_ARM_R.pose_Angle = new Vector3[1]
		{
			new Vector3(330f, 0f, 0f)
		};
		curveMotionData.pose_LEG_L.pose_Pos = new Vector3[2]
		{
			new Vector3(-0.054f, -0.0483f, 0f),
			new Vector3(-0.054f, -0.052f, -0.051f)
		};
		curveMotionData.pose_LEG_L.pose_Angle = new Vector3[2]
		{
			new Vector3(345f, 354f, 0f),
			new Vector3(60f, 0f, 0f)
		};
		curveMotionData.pose_LEG_L.Loop = true;
		curveMotionData.pose_LEG_R.pose_Pos = new Vector3[2]
		{
			new Vector3(0.054f, -0.052f, -0.051f),
			new Vector3(0.054f, -0.0483f, 0f)
		};
		curveMotionData.pose_LEG_R.pose_Angle = new Vector3[2]
		{
			new Vector3(60f, 0f, 0f),
			new Vector3(345f, 345f, 0f)
		};
		curveMotionData.pose_LEG_R.Loop = true;
	}
	public static MotionData GetCurveMotionData()
	{
		return curveMotionData;
	}
	private static void SetFullPowerCurveMotionDataMotionData()
	{
		fullPowerCurveMotionData = defaultMotionData;
		fullPowerCurveMotionData.motionTime = FullCurve_Time;
		fullPowerCurveMotionData.pose_HEAD.pose_Pos = new Vector3[1]
		{
			new Vector3(0f, 0.1735753f, 0f)
		};
		fullPowerCurveMotionData.pose_HEAD.pose_Angle = new Vector3[1]
		{
			new Vector3(315f, 0f, 0f)
		};
		fullPowerCurveMotionData.pose_BODY.pose_Pos = new Vector3[1]
		{
			new Vector3(0f, 0f, 0f)
		};
		fullPowerCurveMotionData.pose_BODY.pose_Angle = new Vector3[1]
		{
			new Vector3(30f, -4.436338E-06f, 15f)
		};
		fullPowerCurveMotionData.pose_HIP.pose_Pos = new Vector3[1]
		{
			new Vector3(0f, 0.05483828f, 0f)
		};
		fullPowerCurveMotionData.pose_HIP.pose_Angle = new Vector3[1]
		{
			new Vector3(30f, 0f, 18.10101f)
		};
		fullPowerCurveMotionData.pose_SHOULDER_L.pose_Pos = new Vector3[1]
		{
			new Vector3(-0.1511168f, 0.1297733f, 0f)
		};
		fullPowerCurveMotionData.pose_SHOULDER_L.pose_Angle = new Vector3[1]
		{
			new Vector3(272.2112f, 0f, 296.7843f)
		};
		fullPowerCurveMotionData.pose_SHOULDER_R.pose_Pos = new Vector3[1]
		{
			new Vector3(0.1511168f, 0.1297733f, 0f)
		};
		fullPowerCurveMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[2]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(315f, 0f, 0f)
		};
		fullPowerCurveMotionData.pose_SHOULDER_R.Loop = true;
		fullPowerCurveMotionData.pose_ARM_L.pose_Pos = new Vector3[1]
		{
			new Vector3(0.006473546f, -0.02895849f, 0f)
		};
		fullPowerCurveMotionData.pose_ARM_L.pose_Angle = new Vector3[1]
		{
			new Vector3(-1.306155E-06f, 1.030415E-05f, 355.524f)
		};
		fullPowerCurveMotionData.pose_ARM_R.pose_Pos = new Vector3[1]
		{
			new Vector3(-0.006473546f, -0.02895849f, 0f)
		};
		fullPowerCurveMotionData.pose_ARM_R.pose_Angle = new Vector3[1]
		{
			new Vector3(315f, 0f, 0f)
		};
		fullPowerCurveMotionData.pose_LEG_L.pose_Pos = new Vector3[2]
		{
			new Vector3(-0.054f, -0.037f, 0.019f),
			new Vector3(-0.054f, -0.062f, -0.044f)
		};
		fullPowerCurveMotionData.pose_LEG_L.pose_Angle = new Vector3[2]
		{
			new Vector3(330f, 0f, 0f),
			new Vector3(18.98691f, 1.084294E-05f, 7.899665E-06f)
		};
		fullPowerCurveMotionData.pose_LEG_L.Loop = true;
		fullPowerCurveMotionData.pose_LEG_R.pose_Pos = new Vector3[2]
		{
			new Vector3(0.054f, -0.051f, -0.044f),
			new Vector3(0.044f, -0.039f, 0.025f)
		};
		fullPowerCurveMotionData.pose_LEG_R.pose_Angle = new Vector3[2]
		{
			new Vector3(-7.000942E-05f, -1.366038E-05f, 7.897413E-06f),
			new Vector3(322.1554f, -2.108276E-05f, 342.5909f)
		};
		fullPowerCurveMotionData.pose_LEG_R.Loop = true;
	}
	public static MotionData GetFullPowerCurveMotionData()
	{
		return fullPowerCurveMotionData;
	}
	private static void SetTiredMotionDataMotionData()
	{
		tiredMotionData = defaultMotionData;
		tiredMotionData.motionTime = Tired_Time;
		tiredMotionData.pose_HEAD.pose_Pos = new Vector3[1]
		{
			new Vector3(0f, 0.1735753f, 0f)
		};
		tiredMotionData.pose_HEAD.pose_Angle = new Vector3[1]
		{
			new Vector3(315f, 0f, 0f)
		};
		tiredMotionData.pose_BODY.pose_Pos = new Vector3[1]
		{
			new Vector3(0f, 0f, 0f)
		};
		tiredMotionData.pose_BODY.pose_Angle = new Vector3[2]
		{
			new Vector3(15f, 0f, 5f),
			new Vector3(15f, 0f, -5f)
		};
		tiredMotionData.pose_BODY.Loop = true;
		tiredMotionData.pose_HIP.pose_Pos = new Vector3[1]
		{
			new Vector3(0f, 0.05483828f, 0f)
		};
		tiredMotionData.pose_HIP.pose_Angle = new Vector3[1]
		{
			new Vector3(30f, 0f, 0f)
		};
		tiredMotionData.pose_SHOULDER_L.pose_Pos = new Vector3[1]
		{
			new Vector3(-0.1511168f, 0.1297733f, 0f)
		};
		tiredMotionData.pose_SHOULDER_L.pose_Angle = new Vector3[2]
		{
			new Vector3(270f, 0f, 0f),
			new Vector3(0f, 0f, 0f)
		};
		tiredMotionData.pose_SHOULDER_L.Loop = true;
		tiredMotionData.pose_ARM_L.pose_Pos = new Vector3[1]
		{
			new Vector3(0.006473546f, -0.02895849f, 0f)
		};
		tiredMotionData.pose_ARM_L.pose_Angle = new Vector3[1]
		{
			new Vector3(350f, 0f, 0f)
		};
		tiredMotionData.pose_SHOULDER_R.pose_Pos = new Vector3[1]
		{
			new Vector3(0.1511168f, 0.1297733f, 0f)
		};
		tiredMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[2]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(270f, 0f, 0f)
		};
		tiredMotionData.pose_SHOULDER_R.Loop = true;
		tiredMotionData.pose_ARM_R.pose_Pos = new Vector3[1]
		{
			new Vector3(-0.006473546f, -0.02895849f, 0f)
		};
		tiredMotionData.pose_ARM_R.pose_Angle = new Vector3[1]
		{
			new Vector3(350f, 0f, 0f)
		};
		tiredMotionData.pose_LEG_L.pose_Pos = new Vector3[2]
		{
			new Vector3(-0.0652f, -0.0657f, -0.0433f),
			new Vector3(-0.0543f, -0.0248f, 0.0317f)
		};
		tiredMotionData.pose_LEG_L.pose_Angle = new Vector3[2]
		{
			new Vector3(29.57459f, -9.816718E-06f, 340.924683f),
			new Vector3(330f, 0f, 0f)
		};
		tiredMotionData.pose_LEG_L.Loop = true;
		tiredMotionData.pose_LEG_R.pose_Pos = new Vector3[2]
		{
			new Vector3(0.054f, -0.037f, 0.019f),
			new Vector3(0.054f, -0.051f, -0.044f)
		};
		tiredMotionData.pose_LEG_R.pose_Angle = new Vector3[2]
		{
			new Vector3(330f, 0f, 0f),
			new Vector3(29.57459f, -9.816718E-06f, 19.07531f)
		};
		tiredMotionData.pose_LEG_R.Loop = true;
	}
	public static MotionData GetTiredMotionData()
	{
		return tiredMotionData;
	}
	private static void SetTiredCurveMotionData()
	{
		tiredCurveMotionData = defaultMotionData;
		tiredCurveMotionData.motionTime = TiredCurve_Time;
		tiredCurveMotionData.pose_HEAD.pose_Pos = new Vector3[1]
		{
			new Vector3(0f, 0.1735753f, 0f)
		};
		tiredCurveMotionData.pose_HEAD.pose_Angle = new Vector3[1]
		{
			new Vector3(330f, 0f, 0f)
		};
		tiredCurveMotionData.pose_BODY.pose_Pos = new Vector3[1]
		{
			new Vector3(0f, 0f, 0f)
		};
		tiredCurveMotionData.pose_BODY.pose_Angle = new Vector3[2]
		{
			new Vector3(30f, 0f, 15f),
			new Vector3(30f, 0f, 5f)
		};
		tiredCurveMotionData.pose_BODY.Loop = true;
		tiredCurveMotionData.pose_HIP.pose_Pos = new Vector3[1]
		{
			new Vector3(0f, 0.05483828f, 0f)
		};
		tiredCurveMotionData.pose_HIP.pose_Angle = new Vector3[1]
		{
			new Vector3(15f, 0f, 0f)
		};
		tiredCurveMotionData.pose_SHOULDER_L.pose_Pos = new Vector3[1]
		{
			new Vector3(-0.1511168f, 0.1297733f, 0f)
		};
		tiredCurveMotionData.pose_SHOULDER_L.pose_Angle = new Vector3[2]
		{
			new Vector3(315f, 0f, 0f),
			new Vector3(0f, 0f, 0f)
		};
		tiredCurveMotionData.pose_SHOULDER_L.Loop = true;
		tiredCurveMotionData.pose_SHOULDER_L.Loop = true;
		tiredCurveMotionData.pose_SHOULDER_R.pose_Pos = new Vector3[1]
		{
			new Vector3(0.1511168f, 0.1297733f, 0f)
		};
		tiredCurveMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[2]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(315f, 0f, 0f)
		};
		tiredCurveMotionData.pose_SHOULDER_R.Loop = true;
		tiredCurveMotionData.pose_ARM_L.pose_Pos = new Vector3[1]
		{
			new Vector3(0.006473546f, -0.02895849f, 0f)
		};
		tiredCurveMotionData.pose_ARM_L.pose_Angle = new Vector3[1]
		{
			new Vector3(330f, 0f, 0f)
		};
		tiredCurveMotionData.pose_ARM_R.pose_Pos = new Vector3[1]
		{
			new Vector3(-0.006473546f, -0.02895849f, 0f)
		};
		tiredCurveMotionData.pose_ARM_R.pose_Angle = new Vector3[1]
		{
			new Vector3(330f, 0f, 0f)
		};
		tiredCurveMotionData.pose_LEG_L.pose_Pos = new Vector3[2]
		{
			new Vector3(-0.054f, -0.0483f, 0f),
			new Vector3(-0.054f, -0.052f, -0.051f)
		};
		tiredCurveMotionData.pose_LEG_L.pose_Angle = new Vector3[2]
		{
			new Vector3(345f, 354f, 0f),
			new Vector3(60f, 0f, 0f)
		};
		tiredCurveMotionData.pose_LEG_L.Loop = true;
		tiredCurveMotionData.pose_LEG_R.pose_Pos = new Vector3[2]
		{
			new Vector3(0.054f, -0.052f, -0.051f),
			new Vector3(0.054f, -0.0483f, 0f)
		};
		tiredCurveMotionData.pose_LEG_R.pose_Angle = new Vector3[2]
		{
			new Vector3(60f, 0f, 0f),
			new Vector3(345f, 345f, 0f)
		};
		tiredCurveMotionData.pose_LEG_R.Loop = true;
	}
	public static MotionData GetTiredCurveMotionData()
	{
		return tiredCurveMotionData;
	}
}
