using System;
using UnityEngine;
public class CharaLeanTweenMotionData {
    [Serializable]
    public struct PoseData {
        [Header("モ\u30fcションの往復ル\u30fcプ化")]
        public bool motionPingPong;
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
    public struct MotionData {
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
    private static readonly float WAIT_ANIMATION_TIME = 0.5f;
    private static readonly float MOVE_ANIMATION_TIME = 0.2f;
    private static readonly float HAPPY_ANIMATION_TIME = 0.5f;
    private static readonly float HAPPY_JUMP_ANIMATION_TIME = 0.25f;
    private static readonly float SADLY_ANIMATION_TIME = 0.5f;
    private static readonly float EXCITE_ANIMATION_TIME = 0.5f;
    private static readonly float GUTS_POSE_1_ANIMATION_TIME = 0.2f;
    private static readonly float GUTS_POSE_2_ANIMATION_TIME = 0.2f;
    private static readonly float SIT_ANIMATION_TIME = 0.5f;
    private static readonly float STANDUP_ANIMATION_TIME = 0.5f;
    private static readonly float ROPE_PULL_ANIMATION_TIME = 0.25f;
    private static readonly float ROPE_ORIGIN_HAND_ANIMATION_TIME = 0.5f;
    private static readonly float DASH_STANDBY_ANIMATION_TIME = 0.2f;
    private static readonly float DASH_ANIMATION_TIME = 0.2f;
    private static readonly float FULL_POWER_DASH_ANIMATION_TIME = 0.15f;
    private static readonly float NEXT_RUNNER_WAIT_ANIMATION_TIME = 0.2f;
    private static readonly float SLOW_DASH_ANIMATION_TIME = 0.4f;
    private static readonly float FISHING_WAIT_ANIMATION_TIME = 0.5f;
    private static readonly float ROD_CAST_ANIMATION_TIME = 0.2f;
    private static readonly float ROD_SET_UP_ANIMATION_TIME = 0.75f;
    private static readonly float FISH_CATCH_ANIMATION_TIME = 0.2f;
    private static readonly float ROD_CANCEL_ANIMATION_TIME = 0.2f;
    private static readonly float CRAWFISH_CATCHING_MOVE_IN_WATER_ANIMATION_TIME = 0.5f;
    private static readonly float CRAWFISH_CATCHING_SCOOP_NET_ANIMATION_TIME = 0.15f;
    private static readonly float CRAWFISH_CATCHING_CATCH_CRAWFISH_ANIMATION_TIME = 0.3f;
    private static readonly float CRAWFISH_CATCHING_BUCKET_PUT_ON_BALANCE_ANIMATION_TIME = 0.3f;
    private static MotionData defaultMotionData;
    private static MotionData moveMotionData;
    private static MotionData happyMotionData;
    private static MotionData happyJumpMotionData;
    private static MotionData sadlyMotionData;
    private static MotionData exciteMotionData;
    private static MotionData gutsPose1MotionData;
    private static MotionData gutsPose2MotionData;
    private static MotionData ropeHoldSitMotionData_Right;
    private static MotionData ropeHoldSitMotionData_Left;
    private static MotionData pullRopeMotionData_Right;
    private static MotionData pullRopeMotionData_Left;
    private static MotionData sitMotionData;
    private static MotionData dashStandbyMotionData;
    private static MotionData dashMotionData;
    private static MotionData fullPowerDashMotionData;
    private static MotionData standUpMotionData;
    private static MotionData nextRunnerWaitMotionData;
    private static MotionData nextRunnerReceiveBatonMotionData;
    private static MotionData batonPassMotionData;
    private static MotionData slowDashMotionData;
    private static MotionData fishingWaitMotionData;
    private static MotionData sitAndFishingMotionData;
    private static MotionData rodCastMotionData;
    private static MotionData rodSetUpMotionData;
    private static MotionData rodCancelMotionData;
    private static MotionData fishingFightMotionData;
    private static MotionData fishingMotionData;
    private static MotionData moveInWaterMotionData;
    private static MotionData scoopMotionData;
    private static MotionData catchCrawfishMotionData;
    private static MotionData bucketPutOnBalanceMotionData;
    public static void CreateMotionData() {
        SetDefaultMotionData();
        SetMoveMotionData();
        SetHappyMotionData();
        SetHappyJumpMotionData();
        SetSadlyMotionData();
        SetExciteMotionData();
        SetGutsPose1MotionData();
        SetGutsPose2MotionData();
        SetSitMotionData();
        SetRopeHoldSitMotionData();
        SetStandUpMotionData();
        SetPullRopeMotionData();
        SetDashStandbyMotionData();
        SetDashMotionData();
        SetFullPowerDashMotionData();
        SetNextRunnerWaitMotionData();
        SetNextRunnerReceiveBatonMotionData();
        SetBatonPassMotionData();
        SetSlowDashMotionData();
        SetFishingWaitMotionData();
        SetSitAndFishingMotionData();
        SetRodSetUpMotionData();
        SetRodCastMotionData();
        SetFishingFightMotionData();
        SetFishingMotionData();
        SetRodCancelMotionData();
        SetMoveInWaterMotionData();
        SetScoopMotionData();
        SetCatchCrawfishMotionData();
        SetBucketPutOnBalanceMotionData();
    }
    private static void SetDefaultMotionData() {
        defaultMotionData.motionTime = WAIT_ANIMATION_TIME;
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
    public static MotionData GetDefaultMotionData() {
        return defaultMotionData;
    }
    private static void SetMoveMotionData() {
        moveMotionData = defaultMotionData;
        moveMotionData.motionTime = MOVE_ANIMATION_TIME;
        moveMotionData.pose_SHOULDER_L.pose_Angle = new Vector3[2]
        {
            new Vector3(-40f, 0f, 0f),
            new Vector3(40f, 0f, 0f)
        };
        moveMotionData.pose_SHOULDER_L.motionPingPong = true;
        moveMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[2]
        {
            new Vector3(40f, 0f, 0f),
            new Vector3(-40f, 0f, 0f)
        };
        moveMotionData.pose_SHOULDER_R.motionPingPong = true;
        moveMotionData.pose_LEG_R.pose_Angle = new Vector3[2]
        {
            new Vector3(-40f, 0f, 0f),
            new Vector3(40f, 0f, 0f)
        };
        moveMotionData.pose_LEG_R.motionPingPong = true;
        moveMotionData.pose_LEG_L.pose_Angle = new Vector3[2]
        {
            new Vector3(40f, 0f, 0f),
            new Vector3(-40f, 0f, 0f)
        };
        moveMotionData.pose_LEG_L.motionPingPong = true;
    }
    public static MotionData GetMoveMotionData() {
        return moveMotionData;
    }
    private static void SetHappyMotionData() {
        happyMotionData = defaultMotionData;
        happyMotionData.motionTime = HAPPY_ANIMATION_TIME;
        happyMotionData.pose_BODY.pose_Angle = new Vector3[3]
        {
            new Vector3(10f, 0f, 0f),
            new Vector3(-10f, 0f, 0f),
            new Vector3(-15f, 0f, 0f)
        };
        happyMotionData.pose_BODY.motionPingPong = true;
        happyMotionData.pose_SHOULDER_L.pose_Angle = new Vector3[3]
        {
            new Vector3(320f, 0f, 0f),
            new Vector3(220f, 0f, 0f),
            new Vector3(210f, 0f, 0f)
        };
        happyMotionData.pose_SHOULDER_L.motionPingPong = true;
        happyMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[3]
        {
            new Vector3(320f, 0f, 0f),
            new Vector3(220f, 0f, 0f),
            new Vector3(210f, 0f, 0f)
        };
        happyMotionData.pose_SHOULDER_R.motionPingPong = true;
    }
    public static MotionData GetHappyMotionData() {
        return happyMotionData;
    }
    private static void SetHappyJumpMotionData() {
        happyJumpMotionData = defaultMotionData;
        happyJumpMotionData.motionTime = HAPPY_JUMP_ANIMATION_TIME;
        happyJumpMotionData.pose_HIP.pose_Pos = new Vector3[2]
        {
            new Vector3(0f, 0.12f, 0f),
            new Vector3(0f, 0.05483828f, 0f)
        };
        happyJumpMotionData.pose_HIP.motionPingPong = true;
        happyJumpMotionData.pose_SHOULDER_L.pose_Angle = new Vector3[2]
        {
            new Vector3(320f, 0f, 0f),
            new Vector3(220f, 0f, 0f)
        };
        happyJumpMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[2]
        {
            new Vector3(320f, 0f, 0f),
            new Vector3(220f, 0f, 0f)
        };
    }
    public static MotionData GetHappyJumpMotionData() {
        return happyJumpMotionData;
    }
    private static void SetSadlyMotionData() {
        sadlyMotionData = defaultMotionData;
        sadlyMotionData.motionTime = SADLY_ANIMATION_TIME;
        sadlyMotionData.pose_BODY.pose_Angle = new Vector3[1]
        {
            new Vector3(15f, 0f, 0f)
        };
    }
    public static MotionData GetSadlyMotionData() {
        return sadlyMotionData;
    }
    private static void SetExciteMotionData() {
        exciteMotionData = defaultMotionData;
        exciteMotionData.motionTime = EXCITE_ANIMATION_TIME;
        exciteMotionData.pose_BODY.pose_Angle = new Vector3[2]
        {
            new Vector3(5f, 5f, 0f),
            new Vector3(5f, 355f, 0f)
        };
        exciteMotionData.pose_BODY.motionPingPong = true;
        exciteMotionData.pose_SHOULDER_L.pose_Angle = new Vector3[1]
        {
            new Vector3(310f, 0f, 0f)
        };
        exciteMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[1]
        {
            new Vector3(310f, 0f, 0f)
        };
        exciteMotionData.pose_ARM_L.pose_Angle = new Vector3[1]
        {
            new Vector3(310f, 0f, 0f)
        };
        exciteMotionData.pose_ARM_R.pose_Angle = new Vector3[1]
        {
            new Vector3(310f, 0f, 0f)
        };
    }
    public static MotionData GetExciteMotionData() {
        return exciteMotionData;
    }
    private static void SetGutsPose1MotionData() {
        gutsPose1MotionData = defaultMotionData;
        gutsPose1MotionData.motionTime = GUTS_POSE_1_ANIMATION_TIME;
        gutsPose1MotionData.pose_BODY.pose_Angle = new Vector3[2]
        {
            new Vector3(355f, 0f, 0f),
            new Vector3(5f, 0f, 0f)
        };
        gutsPose1MotionData.pose_SHOULDER_R.pose_Pos = new Vector3[2]
        {
            new Vector3(0.15f, 0.165f, 0.08f),
            new Vector3(0.15f, 0.09f, 0.08f)
        };
        gutsPose1MotionData.pose_SHOULDER_R.pose_Angle = new Vector3[2]
        {
            new Vector3(275f, 0f, 0f),
            new Vector3(280f, 0f, 0f)
        };
        gutsPose1MotionData.pose_ARM_R.pose_Angle = new Vector3[2]
        {
            new Vector3(300f, 0f, 0f),
            new Vector3(270f, 0f, 0f)
        };
        gutsPose1MotionData.pose_SHOULDER_L.pose_Pos = new Vector3[1]
        {
            new Vector3(-0.15f, 0.13f, 0.08f)
        };
        gutsPose1MotionData.pose_SHOULDER_L.pose_Angle = new Vector3[1]
        {
            new Vector3(320f, 0f, 0f)
        };
        gutsPose1MotionData.pose_ARM_L.pose_Angle = new Vector3[1]
        {
            new Vector3(290f, 0f, 0f)
        };
    }
    public static MotionData GetGutsPose1MotionData() {
        return gutsPose1MotionData;
    }
    private static void SetGutsPose2MotionData() {
        gutsPose2MotionData = defaultMotionData;
        gutsPose2MotionData.motionTime = GUTS_POSE_2_ANIMATION_TIME;
        gutsPose2MotionData.pose_BODY.pose_Angle = new Vector3[2]
        {
            new Vector3(10f, 0f, 0f),
            new Vector3(350f, 0f, 0f)
        };
        gutsPose2MotionData.pose_SHOULDER_R.pose_Pos = new Vector3[2]
        {
            new Vector3(0.15f, 0.06f, 0.08f),
            new Vector3(0.15f, 0.15f, 0.07f)
        };
        gutsPose2MotionData.pose_SHOULDER_R.pose_Angle = new Vector3[3]
        {
            new Vector3(320f, 0f, 0f),
            new Vector3(220f, 0f, 0f),
            new Vector3(210f, 0f, 0f)
        };
        gutsPose2MotionData.pose_ARM_R.pose_Angle = new Vector3[2]
        {
            new Vector3(270f, 0f, 0f),
            new Vector3(0f, 0f, 0f)
        };
        gutsPose2MotionData.pose_SHOULDER_L.pose_Pos = new Vector3[1]
        {
            new Vector3(-0.15f, 0.13f, 0.08f)
        };
        gutsPose2MotionData.pose_SHOULDER_L.pose_Angle = new Vector3[1]
        {
            new Vector3(320f, 0f, 0f)
        };
        gutsPose2MotionData.pose_ARM_L.pose_Angle = new Vector3[1]
        {
            new Vector3(290f, 0f, 0f)
        };
    }
    public static MotionData GetGutsPose2MotionData() {
        return gutsPose2MotionData;
    }
    private static void SetRopeHoldSitMotionData() {
        ropeHoldSitMotionData_Right = defaultMotionData;
        ropeHoldSitMotionData_Left = defaultMotionData;
        ropeHoldSitMotionData_Right.motionTime = SIT_ANIMATION_TIME;
        ropeHoldSitMotionData_Left.motionTime = SIT_ANIMATION_TIME;
        ropeHoldSitMotionData_Right.pose_SHOULDER_L.pose_Pos = new Vector3[1]
        {
            new Vector3(-0.03f, 0.13f, 0.13f)
        };
        ropeHoldSitMotionData_Right.pose_SHOULDER_L.pose_Angle = new Vector3[1]
        {
            new Vector3(270f, 0f, 0f)
        };
        ropeHoldSitMotionData_Right.pose_SHOULDER_R.pose_Pos = new Vector3[1]
        {
            new Vector3(0.1f, 0.2f, 0.15f)
        };
        ropeHoldSitMotionData_Right.pose_SHOULDER_R.pose_Angle = new Vector3[1]
        {
            new Vector3(270f, 0f, 0f)
        };
        ropeHoldSitMotionData_Right.pose_HIP.pose_Angle = new Vector3[1]
        {
            new Vector3(0f, 30f, 0f)
        };
        ropeHoldSitMotionData_Right.pose_LEG_L.pose_Pos = new Vector3[1]
        {
            new Vector3(-0.054f, -0.014f, 0.094f)
        };
        ropeHoldSitMotionData_Right.pose_LEG_L.pose_Angle = new Vector3[1]
        {
            new Vector3(-90f, 0f, 0f)
        };
        ropeHoldSitMotionData_Right.pose_LEG_R.pose_Pos = new Vector3[1]
        {
            new Vector3(0.054f, -0.014f, 0.094f)
        };
        ropeHoldSitMotionData_Right.pose_LEG_R.pose_Angle = new Vector3[1]
        {
            new Vector3(-90f, 0f, 0f)
        };
        ropeHoldSitMotionData_Left.pose_SHOULDER_L.pose_Pos = new Vector3[1]
        {
            new Vector3(-0.07f, 0.18f, 0.13f)
        };
        ropeHoldSitMotionData_Left.pose_SHOULDER_L.pose_Angle = new Vector3[1]
        {
            new Vector3(270f, 0f, 0f)
        };
        ropeHoldSitMotionData_Left.pose_SHOULDER_R.pose_Pos = new Vector3[1]
        {
            new Vector3(0.03f, 0.12f, 0.13f)
        };
        ropeHoldSitMotionData_Left.pose_SHOULDER_R.pose_Angle = new Vector3[1]
        {
            new Vector3(270f, 0f, 0f)
        };
        ropeHoldSitMotionData_Left.pose_HIP.pose_Angle = new Vector3[1]
        {
            new Vector3(0f, -30f, 0f)
        };
        ropeHoldSitMotionData_Left.pose_LEG_L.pose_Pos = new Vector3[1]
        {
            new Vector3(-0.054f, -0.014f, 0.094f)
        };
        ropeHoldSitMotionData_Left.pose_LEG_L.pose_Angle = new Vector3[1]
        {
            new Vector3(-90f, 0f, 0f)
        };
        ropeHoldSitMotionData_Left.pose_LEG_R.pose_Pos = new Vector3[1]
        {
            new Vector3(0.054f, -0.014f, 0.094f)
        };
        ropeHoldSitMotionData_Left.pose_LEG_R.pose_Angle = new Vector3[1]
        {
            new Vector3(-90f, 0f, 0f)
        };
    }
    public static MotionData GetRopeHoldSitMotionData_Right() {
        return ropeHoldSitMotionData_Right;
    }
    public static MotionData GetRopeHoldSitMotionData_Left() {
        return ropeHoldSitMotionData_Left;
    }
    private static void SetPullRopeMotionData() {
        pullRopeMotionData_Right = defaultMotionData;
        pullRopeMotionData_Left = defaultMotionData;
        pullRopeMotionData_Right.motionTime = ROPE_PULL_ANIMATION_TIME;
        pullRopeMotionData_Left.motionTime = ROPE_PULL_ANIMATION_TIME;
        pullRopeMotionData_Right.pose_SHOULDER_L.pose_Pos = new Vector3[2]
        {
            new Vector3(0.01f, 0.13f, 0.13f),
            new Vector3(-0.03f, 0.13f, 0.13f)
        };
        pullRopeMotionData_Right.pose_SHOULDER_L.pose_Angle = new Vector3[2]
        {
            new Vector3(290f, 0f, 0f),
            new Vector3(270f, 0f, 0f)
        };
        pullRopeMotionData_Right.pose_SHOULDER_L.motionPingPong = true;
        pullRopeMotionData_Right.pose_SHOULDER_R.pose_Pos = new Vector3[2]
        {
            new Vector3(0.13f, 0.16f, 0.14f),
            new Vector3(0.1f, 0.2f, 0.15f)
        };
        pullRopeMotionData_Right.pose_SHOULDER_R.pose_Angle = new Vector3[2]
        {
            new Vector3(290f, 0f, 0f),
            new Vector3(270f, 0f, 0f)
        };
        pullRopeMotionData_Right.pose_SHOULDER_R.motionPingPong = true;
        pullRopeMotionData_Right.pose_BODY.pose_Angle = new Vector3[2]
        {
            new Vector3(-20f, 0f, 0f),
            new Vector3(10f, 0f, 0f)
        };
        pullRopeMotionData_Right.pose_BODY.motionPingPong = true;
        pullRopeMotionData_Right.pose_HIP.pose_Angle = new Vector3[1]
        {
            new Vector3(0f, 30f, 0f)
        };
        pullRopeMotionData_Right.pose_LEG_L.pose_Pos = new Vector3[1]
        {
            new Vector3(-0.054f, -0.0483f, 0f)
        };
        pullRopeMotionData_Right.pose_LEG_L.pose_Angle = new Vector3[1]
        {
            new Vector3(0f, 0f, 0f)
        };
        pullRopeMotionData_Right.pose_LEG_R.pose_Pos = new Vector3[1]
        {
            new Vector3(0.054f, -0.0483f, 0f)
        };
        pullRopeMotionData_Right.pose_LEG_R.pose_Angle = new Vector3[1]
        {
            new Vector3(0f, 0f, 0f)
        };
        pullRopeMotionData_Left.pose_SHOULDER_L.pose_Pos = new Vector3[2]
        {
            new Vector3(-0.1f, 0.15f, 0.12f),
            new Vector3(-0.07f, 0.18f, 0.13f)
        };
        pullRopeMotionData_Left.pose_SHOULDER_L.pose_Angle = new Vector3[2]
        {
            new Vector3(290f, 0f, 0f),
            new Vector3(270f, 0f, 0f)
        };
        pullRopeMotionData_Left.pose_SHOULDER_L.motionPingPong = true;
        pullRopeMotionData_Left.pose_SHOULDER_R.pose_Pos = new Vector3[2]
        {
            new Vector3(-0.01f, 0.12f, 0.13f),
            new Vector3(0.03f, 0.12f, 0.13f)
        };
        pullRopeMotionData_Left.pose_SHOULDER_R.pose_Angle = new Vector3[2]
        {
            new Vector3(290f, 0f, 0f),
            new Vector3(270f, 0f, 0f)
        };
        pullRopeMotionData_Left.pose_SHOULDER_R.motionPingPong = true;
        pullRopeMotionData_Left.pose_BODY.pose_Angle = new Vector3[2]
        {
            new Vector3(-20f, 0f, 0f),
            new Vector3(10f, 0f, 0f)
        };
        pullRopeMotionData_Left.pose_BODY.motionPingPong = true;
        pullRopeMotionData_Left.pose_HIP.pose_Angle = new Vector3[1]
        {
            new Vector3(0f, -30f, 0f)
        };
        pullRopeMotionData_Left.pose_LEG_L.pose_Pos = new Vector3[1]
        {
            new Vector3(-0.054f, -0.0483f, 0f)
        };
        pullRopeMotionData_Left.pose_LEG_L.pose_Angle = new Vector3[1]
        {
            new Vector3(0f, 0f, 0f)
        };
        pullRopeMotionData_Left.pose_LEG_R.pose_Pos = new Vector3[1]
        {
            new Vector3(0.054f, -0.0483f, 0f)
        };
        pullRopeMotionData_Left.pose_LEG_R.pose_Angle = new Vector3[1]
        {
            new Vector3(0f, 0f, 0f)
        };
    }
    public static MotionData GetPullRopeMotionData_Right() {
        return pullRopeMotionData_Right;
    }
    public static MotionData GetPullRopeMotionData_Left() {
        return pullRopeMotionData_Left;
    }
    private static void SetSitMotionData() {
        sitMotionData = defaultMotionData;
        sitMotionData.motionTime = SIT_ANIMATION_TIME;
        sitMotionData.pose_SHOULDER_L.pose_Pos = new Vector3[1]
        {
            new Vector3(-0.15f, 0.13f, 0.12f)
        };
        sitMotionData.pose_SHOULDER_L.pose_Angle = new Vector3[1]
        {
            new Vector3(310f, 40f, 0f)
        };
        sitMotionData.pose_SHOULDER_R.pose_Pos = new Vector3[1]
        {
            new Vector3(0.15f, 0.13f, 0.12f)
        };
        sitMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[1]
        {
            new Vector3(310f, 320f, 0f)
        };
        sitMotionData.pose_LEG_L.pose_Pos = new Vector3[1]
        {
            new Vector3(-0.054f, 0.06f, 0.155f)
        };
        sitMotionData.pose_LEG_L.pose_Angle = new Vector3[1]
        {
            new Vector3(0f, 0f, 0f)
        };
        sitMotionData.pose_LEG_R.pose_Pos = new Vector3[1]
        {
            new Vector3(0.054f, 0.06f, 0.155f)
        };
        sitMotionData.pose_LEG_R.pose_Angle = new Vector3[1]
        {
            new Vector3(0f, 0f, 0f)
        };
    }
    public static MotionData GetSitMotionData() {
        return sitMotionData;
    }
    private static void SetDashStandbyMotionData() {
        dashStandbyMotionData = defaultMotionData;
        dashStandbyMotionData.motionTime = DASH_STANDBY_ANIMATION_TIME;
        dashStandbyMotionData.pose_BODY.pose_Angle = new Vector3[1]
        {
            new Vector3(6.4f, 0f, 0f)
        };
        dashStandbyMotionData.pose_HIP.pose_Angle = new Vector3[1]
        {
            new Vector3(5f, 0f, 0f)
        };
        dashStandbyMotionData.pose_SHOULDER_L.pose_Angle = new Vector3[1]
        {
            new Vector3(60f, 0f, 0f)
        };
        dashStandbyMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[1]
        {
            new Vector3(300f, 0f, 0f)
        };
        dashStandbyMotionData.pose_ARM_L.pose_Angle = new Vector3[1]
        {
            new Vector3(310f, 0f, 0f)
        };
        dashStandbyMotionData.pose_ARM_R.pose_Angle = new Vector3[1]
        {
            new Vector3(310f, 0f, 0f)
        };
        dashStandbyMotionData.pose_LEG_L.pose_Angle = new Vector3[1]
        {
            new Vector3(330f, 0f, 0f)
        };
        dashStandbyMotionData.pose_LEG_R.pose_Angle = new Vector3[1]
        {
            new Vector3(30f, 0f, 0f)
        };
    }
    public static MotionData GetDashStandbyMotionData() {
        return dashStandbyMotionData;
    }
    private static void SetDashMotionData() {
        dashMotionData = defaultMotionData;
        dashMotionData.motionTime = DASH_ANIMATION_TIME;
        dashMotionData.pose_BODY.pose_Angle = new Vector3[1]
        {
            new Vector3(2f, 0f, 0f)
        };
        dashMotionData.pose_HIP.pose_Angle = new Vector3[2]
        {
            new Vector3(0f, 4f, 0f),
            new Vector3(0f, -4f, 0f)
        };
        dashMotionData.pose_HIP.motionPingPong = true;
        dashMotionData.pose_SHOULDER_L.pose_Angle = new Vector3[2]
        {
            new Vector3(-40f, 0f, 0f),
            new Vector3(40f, 0f, 0f)
        };
        dashMotionData.pose_SHOULDER_L.motionPingPong = true;
        dashMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[2]
        {
            new Vector3(40f, 0f, 0f),
            new Vector3(-40f, 0f, 0f)
        };
        dashMotionData.pose_SHOULDER_R.motionPingPong = true;
        dashMotionData.pose_ARM_L.pose_Angle = new Vector3[1]
        {
            new Vector3(310f, 0f, 0f)
        };
        dashMotionData.pose_ARM_R.pose_Angle = new Vector3[1]
        {
            new Vector3(310f, 0f, 0f)
        };
        dashMotionData.pose_LEG_L.pose_Angle = new Vector3[2]
        {
            new Vector3(-45f, 0f, 0f),
            new Vector3(45f, 0f, 0f)
        };
        dashMotionData.pose_LEG_L.motionPingPong = true;
        dashMotionData.pose_LEG_R.pose_Angle = new Vector3[2]
        {
            new Vector3(45f, 0f, 0f),
            new Vector3(-45f, 0f, 0f)
        };
        dashMotionData.pose_LEG_R.motionPingPong = true;
    }
    public static MotionData GetDashMotionData() {
        return dashMotionData;
    }
    private static void SetFullPowerDashMotionData() {
        fullPowerDashMotionData = defaultMotionData;
        fullPowerDashMotionData.motionTime = FULL_POWER_DASH_ANIMATION_TIME;
        fullPowerDashMotionData.pose_BODY.pose_Angle = new Vector3[1]
        {
            new Vector3(2f, 0f, 0f)
        };
        fullPowerDashMotionData.pose_HIP.pose_Angle = new Vector3[2]
        {
            new Vector3(15f, 8f, 0f),
            new Vector3(15f, -8f, 0f)
        };
        fullPowerDashMotionData.pose_HIP.motionPingPong = true;
        fullPowerDashMotionData.pose_SHOULDER_L.pose_Angle = new Vector3[2]
        {
            new Vector3(-75f, 0f, 0f),
            new Vector3(75f, 0f, 0f)
        };
        fullPowerDashMotionData.pose_SHOULDER_L.motionPingPong = true;
        fullPowerDashMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[2]
        {
            new Vector3(75f, 0f, 0f),
            new Vector3(-75f, 0f, 0f)
        };
        fullPowerDashMotionData.pose_SHOULDER_R.motionPingPong = true;
        fullPowerDashMotionData.pose_ARM_L.pose_Angle = new Vector3[1]
        {
            new Vector3(310f, 0f, 0f)
        };
        fullPowerDashMotionData.pose_ARM_R.pose_Angle = new Vector3[1]
        {
            new Vector3(310f, 0f, 0f)
        };
        fullPowerDashMotionData.pose_LEG_L.pose_Angle = new Vector3[2]
        {
            new Vector3(-75f, 0f, 0f),
            new Vector3(75f, 0f, 0f)
        };
        fullPowerDashMotionData.pose_LEG_L.motionPingPong = true;
        fullPowerDashMotionData.pose_LEG_R.pose_Angle = new Vector3[2]
        {
            new Vector3(75f, 0f, 0f),
            new Vector3(-75f, 0f, 0f)
        };
        fullPowerDashMotionData.pose_LEG_R.motionPingPong = true;
    }
    public static MotionData GetFullPowerDashMotionData() {
        return fullPowerDashMotionData;
    }
    private static void SetStandUpMotionData() {
        standUpMotionData = defaultMotionData;
        standUpMotionData.motionTime = STANDUP_ANIMATION_TIME;
    }
    public static MotionData GetStandUpMotionData() {
        return standUpMotionData;
    }
    private static void SetNextRunnerWaitMotionData() {
        nextRunnerWaitMotionData = defaultMotionData;
        nextRunnerWaitMotionData.motionTime = NEXT_RUNNER_WAIT_ANIMATION_TIME;
        nextRunnerWaitMotionData.pose_HEAD.pose_Angle = new Vector3[1]
        {
            new Vector3(0f, 270f, 0f)
        };
        nextRunnerWaitMotionData.pose_BODY.pose_Angle = new Vector3[1]
        {
            new Vector3(6.4f, 350f, 0f)
        };
        nextRunnerWaitMotionData.pose_HIP.pose_Angle = new Vector3[1]
        {
            new Vector3(5f, 0f, 0f)
        };
        dashStandbyMotionData.pose_SHOULDER_L.pose_Angle = new Vector3[1]
        {
            new Vector3(60f, 0f, 0f)
        };
        dashStandbyMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[1]
        {
            new Vector3(300f, 0f, 0f)
        };
        dashStandbyMotionData.pose_ARM_L.pose_Angle = new Vector3[1]
        {
            new Vector3(310f, 0f, 0f)
        };
        dashStandbyMotionData.pose_ARM_R.pose_Angle = new Vector3[1]
        {
            new Vector3(310f, 0f, 0f)
        };
        dashStandbyMotionData.pose_LEG_L.pose_Angle = new Vector3[1]
        {
            new Vector3(330f, 0f, 0f)
        };
        dashStandbyMotionData.pose_LEG_R.pose_Angle = new Vector3[1]
        {
            new Vector3(30f, 0f, 0f)
        };
    }
    public static MotionData GetNextRunnnerWaitMotionData() {
        return nextRunnerWaitMotionData;
    }
    private static void SetNextRunnerReceiveBatonMotionData() {
        nextRunnerReceiveBatonMotionData = defaultMotionData;
        nextRunnerReceiveBatonMotionData.motionTime = DASH_ANIMATION_TIME;
        nextRunnerReceiveBatonMotionData.pose_HEAD.pose_Angle = new Vector3[1]
        {
            new Vector3(0f, 10f, 0f)
        };
        nextRunnerReceiveBatonMotionData.pose_BODY.pose_Angle = new Vector3[1]
        {
            new Vector3(0f, 330f, 0f)
        };
        nextRunnerReceiveBatonMotionData.pose_SHOULDER_L.pose_Angle = new Vector3[1]
        {
            new Vector3(90f, 0f, 0f)
        };
        nextRunnerReceiveBatonMotionData.pose_ARM_L.pose_Angle = new Vector3[1]
        {
            new Vector3(0f, 0f, 0f)
        };
        nextRunnerReceiveBatonMotionData.pose_LEG_L.pose_Angle = new Vector3[2]
        {
            new Vector3(-45f, 0f, 0f),
            new Vector3(45f, 0f, 0f)
        };
        nextRunnerReceiveBatonMotionData.pose_LEG_L.motionPingPong = true;
        nextRunnerReceiveBatonMotionData.pose_LEG_R.pose_Angle = new Vector3[2]
        {
            new Vector3(45f, 0f, 0f),
            new Vector3(-45f, 0f, 0f)
        };
        nextRunnerReceiveBatonMotionData.pose_LEG_R.motionPingPong = true;
    }
    public static MotionData GetNextRunnerReceiveBatonMotionData() {
        return nextRunnerReceiveBatonMotionData;
    }
    private static void SetBatonPassMotionData() {
        batonPassMotionData = defaultMotionData;
        batonPassMotionData.motionTime = DASH_ANIMATION_TIME;
        batonPassMotionData.pose_BODY.pose_Angle = new Vector3[1]
        {
            new Vector3(2f, 0f, 0f)
        };
        batonPassMotionData.pose_HIP.pose_Angle = new Vector3[1]
        {
            new Vector3(3f, 4f, 0f)
        };
        batonPassMotionData.pose_SHOULDER_L.pose_Angle = new Vector3[2]
        {
            new Vector3(-40f, 0f, 0f),
            new Vector3(40f, 0f, 0f)
        };
        batonPassMotionData.pose_SHOULDER_L.motionPingPong = true;
        batonPassMotionData.pose_ARM_L.pose_Angle = new Vector3[1]
        {
            new Vector3(310f, 0f, 0f)
        };
        batonPassMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[1]
        {
            new Vector3(260f, 0f, 0f)
        };
        batonPassMotionData.pose_ARM_R.pose_Angle = new Vector3[1]
        {
            new Vector3(0f, 0f, 0f)
        };
        batonPassMotionData.pose_LEG_L.pose_Angle = new Vector3[2]
        {
            new Vector3(-45f, 0f, 0f),
            new Vector3(45f, 0f, 0f)
        };
        batonPassMotionData.pose_LEG_L.motionPingPong = true;
        batonPassMotionData.pose_LEG_R.pose_Angle = new Vector3[2]
        {
            new Vector3(45f, 0f, 0f),
            new Vector3(-45f, 0f, 0f)
        };
        batonPassMotionData.pose_LEG_R.motionPingPong = true;
    }
    public static MotionData GetBatonPassMotionData() {
        return batonPassMotionData;
    }
    private static void SetSlowDashMotionData() {
        slowDashMotionData = defaultMotionData;
        slowDashMotionData.motionTime = SLOW_DASH_ANIMATION_TIME;
        slowDashMotionData.pose_BODY.pose_Angle = new Vector3[1]
        {
            new Vector3(0f, 0f, 0f)
        };
        slowDashMotionData.pose_HIP.pose_Angle = new Vector3[1]
        {
            new Vector3(0f, 0f, 0f)
        };
        slowDashMotionData.pose_SHOULDER_L.pose_Angle = new Vector3[2]
        {
            new Vector3(40f, 0f, 0f),
            new Vector3(-40f, 0f, 0f)
        };
        slowDashMotionData.pose_SHOULDER_L.motionPingPong = true;
        slowDashMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[2]
        {
            new Vector3(-40f, 0f, 0f),
            new Vector3(40f, 0f, 0f)
        };
        slowDashMotionData.pose_SHOULDER_R.motionPingPong = true;
        slowDashMotionData.pose_ARM_L.pose_Angle = new Vector3[1]
        {
            new Vector3(310f, 0f, 0f)
        };
        slowDashMotionData.pose_ARM_R.pose_Angle = new Vector3[1]
        {
            new Vector3(310f, 0f, 0f)
        };
        slowDashMotionData.pose_LEG_L.pose_Angle = new Vector3[2]
        {
            new Vector3(-45f, 0f, 0f),
            new Vector3(45f, 0f, 0f)
        };
        slowDashMotionData.pose_LEG_L.motionPingPong = true;
        slowDashMotionData.pose_LEG_R.pose_Angle = new Vector3[2]
        {
            new Vector3(45f, 0f, 0f),
            new Vector3(-45f, 0f, 0f)
        };
        slowDashMotionData.pose_LEG_R.motionPingPong = true;
    }
    public static MotionData GetSlowDashMotionData() {
        return slowDashMotionData;
    }
    private static void SetFishingWaitMotionData() {
        fishingWaitMotionData.motionTime = FISHING_WAIT_ANIMATION_TIME;
        fishingWaitMotionData.pose_HEAD.pose_Pos = new Vector3[1]
        {
            new Vector3(0f, 0.1735753f, 0f)
        };
        fishingWaitMotionData.pose_HEAD.pose_Angle = new Vector3[1]
        {
            Vector3.zero
        };
        fishingWaitMotionData.pose_HEAD.pose_Scale = new Vector3[1]
        {
            Vector3.one
        };
        fishingWaitMotionData.pose_BODY.pose_Pos = new Vector3[1]
        {
            Vector3.zero
        };
        fishingWaitMotionData.pose_BODY.pose_Angle = new Vector3[1]
        {
            Vector3.zero
        };
        fishingWaitMotionData.pose_BODY.pose_Scale = new Vector3[1]
        {
            Vector3.one
        };
        fishingWaitMotionData.pose_HIP.pose_Pos = new Vector3[1]
        {
            new Vector3(0f, 0.134f, 0f)
        };
        fishingWaitMotionData.pose_HIP.pose_Angle = new Vector3[1]
        {
            Vector3.zero
        };
        fishingWaitMotionData.pose_HIP.pose_Scale = new Vector3[1]
        {
            Vector3.one
        };
        fishingWaitMotionData.pose_SHOULDER_L.pose_Pos = new Vector3[1]
        {
            new Vector3(-0.1511168f, 0.1297733f, 0f)
        };
        fishingWaitMotionData.pose_SHOULDER_L.pose_Angle = new Vector3[1]
        {
            Vector3.zero
        };
        fishingWaitMotionData.pose_SHOULDER_L.pose_Scale = new Vector3[1]
        {
            Vector3.one
        };
        fishingWaitMotionData.pose_SHOULDER_R.pose_Pos = new Vector3[1]
        {
            new Vector3(0.1511168f, 0.1297733f, 0f)
        };
        fishingWaitMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[1]
        {
            new Vector3(280f, 0f, 0f)
        };
        fishingWaitMotionData.pose_SHOULDER_R.pose_Scale = new Vector3[1]
        {
            Vector3.one
        };
        fishingWaitMotionData.pose_ARM_L.pose_Pos = new Vector3[1]
        {
            new Vector3(0.006473546f, -0.02895849f, 0f)
        };
        fishingWaitMotionData.pose_ARM_L.pose_Angle = new Vector3[1]
        {
            Vector3.zero
        };
        fishingWaitMotionData.pose_ARM_L.pose_Scale = new Vector3[1]
        {
            Vector3.one
        };
        fishingWaitMotionData.pose_ARM_R.pose_Pos = new Vector3[1]
        {
            new Vector3(-0.006473546f, -0.02895849f, 0f)
        };
        fishingWaitMotionData.pose_ARM_R.pose_Angle = new Vector3[1]
        {
            new Vector3(310f, 0f, 0f)
        };
        fishingWaitMotionData.pose_ARM_R.pose_Scale = new Vector3[1]
        {
            Vector3.one
        };
        fishingWaitMotionData.pose_LEG_L.pose_Pos = new Vector3[1]
        {
            new Vector3(-0.054f, -0.0483f, 0f)
        };
        fishingWaitMotionData.pose_LEG_L.pose_Angle = new Vector3[1]
        {
            Vector3.zero
        };
        fishingWaitMotionData.pose_LEG_L.pose_Scale = new Vector3[1]
        {
            Vector3.one
        };
        fishingWaitMotionData.pose_LEG_R.pose_Pos = new Vector3[1]
        {
            new Vector3(0.054f, -0.0483f, 0f)
        };
        fishingWaitMotionData.pose_LEG_R.pose_Angle = new Vector3[1]
        {
            Vector3.zero
        };
        fishingWaitMotionData.pose_LEG_R.pose_Scale = new Vector3[1]
        {
            Vector3.one
        };
    }
    public static MotionData GetFishingWaitMotionData() {
        return fishingWaitMotionData;
    }
    private static void SetSitAndFishingMotionData() {
        sitAndFishingMotionData = fishingWaitMotionData;
        sitAndFishingMotionData.motionTime = ROD_CAST_ANIMATION_TIME;
        sitAndFishingMotionData.pose_BODY.pose_Angle = new Vector3[1]
        {
            new Vector3(5f, 0f, 0f)
        };
        sitAndFishingMotionData.pose_HEAD.pose_Angle = new Vector3[1]
        {
            new Vector3(6f, 0f, 0f)
        };
        sitAndFishingMotionData.pose_SHOULDER_R.pose_Pos = new Vector3[1]
        {
            new Vector3(0.1511168f, 0.133f, 0.035f)
        };
        sitAndFishingMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[1]
        {
            new Vector3(322.5f, 0f, 0f)
        };
        sitAndFishingMotionData.pose_ARM_R.pose_Pos = new Vector3[1]
        {
            new Vector3(-0.006473546f, -0.02895849f, 0f)
        };
        sitAndFishingMotionData.pose_ARM_R.pose_Angle = new Vector3[1]
        {
            new Vector3(343f, 0f, 0f)
        };
        sitAndFishingMotionData.pose_SHOULDER_R.pose_Pos = new Vector3[1]
        {
            new Vector3(0.1511168f, 0.133f, 0.035f)
        };
        sitAndFishingMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[1]
        {
            new Vector3(344f, 340.3f, 4.21f)
        };
        sitAndFishingMotionData.pose_LEG_L.pose_Pos = new Vector3[1]
        {
            new Vector3(-0.054f, -0.004f, 0.101f)
        };
        sitAndFishingMotionData.pose_LEG_L.pose_Angle = new Vector3[1]
        {
            new Vector3(311f, 0f, 0f)
        };
        sitAndFishingMotionData.pose_LEG_R.pose_Pos = new Vector3[1]
        {
            new Vector3(0.054f, -0.004f, 0.101f)
        };
        sitAndFishingMotionData.pose_LEG_R.pose_Angle = new Vector3[1]
        {
            new Vector3(311f, 0f, 0f)
        };
    }
    public static MotionData GetShitAndFishingMotionData() {
        return sitAndFishingMotionData;
    }
    private static void SetRodCastMotionData() {
        rodCastMotionData = fishingWaitMotionData;
        rodCastMotionData.motionTime = ROD_CAST_ANIMATION_TIME;
        rodCastMotionData.pose_BODY.pose_Angle = new Vector3[2]
        {
            new Vector3(345f, 10f, 0f),
            new Vector3(5f, 0f, 0f)
        };
        rodCastMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[2]
        {
            new Vector3(300f, 0f, 0f),
            new Vector3(310f, 0f, 0f)
        };
        rodCastMotionData.pose_ARM_R.pose_Angle = new Vector3[2]
        {
            new Vector3(290f, 0f, 0f),
            new Vector3(10f, 0f, 0f)
        };
    }
    public static MotionData GetRodCastMotionData() {
        return rodCastMotionData;
    }
    private static void SetRodSetUpMotionData() {
        rodSetUpMotionData = fishingWaitMotionData;
        rodSetUpMotionData.motionTime = ROD_SET_UP_ANIMATION_TIME;
        rodSetUpMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[3]
        {
            new Vector3(312f, 0f, 0f),
            new Vector3(309f, 0f, 0f),
            new Vector3(311f, 0f, 0f)
        };
        rodSetUpMotionData.pose_SHOULDER_R.motionPingPong = true;
        rodSetUpMotionData.pose_ARM_R.pose_Angle = new Vector3[1]
        {
            Vector3.zero
        };
    }
    public static MotionData GetRodSetUpMotionData() {
        return rodSetUpMotionData;
    }
    private static void SetRodCancelMotionData() {
        rodCancelMotionData = fishingWaitMotionData;
        rodCancelMotionData.motionTime = ROD_CANCEL_ANIMATION_TIME;
        rodCancelMotionData.pose_BODY.pose_Angle = new Vector3[2]
        {
            new Vector3(345f, 10f, 0f),
            new Vector3(0f, 0f, 0f)
        };
        rodCancelMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[2]
        {
            new Vector3(300f, 0f, 0f),
            new Vector3(280f, 0f, 0f)
        };
        rodCancelMotionData.pose_ARM_R.pose_Angle = new Vector3[2]
        {
            new Vector3(290f, 0f, 0f),
            new Vector3(310f, 0f, 0f)
        };
    }
    public static MotionData GetRodCancelMotionData() {
        return rodCancelMotionData;
    }
    private static void SetFishingFightMotionData() {
        fishingFightMotionData = fishingWaitMotionData;
        fishingFightMotionData.motionTime = FISH_CATCH_ANIMATION_TIME;
        fishingFightMotionData.pose_SHOULDER_L.pose_Pos = new Vector3[1]
        {
            new Vector3(-0.075f, 0.1f, 0.1f)
        };
        fishingFightMotionData.pose_SHOULDER_L.pose_Angle = new Vector3[1]
        {
            new Vector3(270f, 35f, 0f)
        };
        fishingFightMotionData.pose_SHOULDER_R.pose_Pos = new Vector3[1]
        {
            new Vector3(0.075f, 0.13f, 0.1f)
        };
        fishingFightMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[1]
        {
            new Vector3(320f, 325f, 0f)
        };
        fishingFightMotionData.pose_ARM_R.pose_Angle = new Vector3[1]
        {
            new Vector3(345f, 0f, 0f)
        };
    }
    public static MotionData GetFishingFightMotionData() {
        return fishingFightMotionData;
    }
    private static void SetFishingMotionData() {
        fishingMotionData = fishingWaitMotionData;
        fishingMotionData.motionTime = FISH_CATCH_ANIMATION_TIME;
        fishingMotionData.pose_BODY.pose_Angle = new Vector3[2]
        {
            new Vector3(350f, 0f, 0f),
            new Vector3(0f, 0f, 0f)
        };
        fishingMotionData.pose_SHOULDER_L.pose_Angle = new Vector3[1]
        {
            new Vector3(300f, 0f, 0f)
        };
        fishingMotionData.pose_ARM_L.pose_Angle = new Vector3[1]
        {
            new Vector3(330f, 0f, 0f)
        };
        fishingMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[1]
        {
            new Vector3(300f, 0f, 0f)
        };
        fishingMotionData.pose_ARM_R.pose_Angle = new Vector3[1]
        {
            new Vector3(340f, 0f, 0f)
        };
    }
    public static MotionData GetFishingMotionData() {
        return fishingMotionData;
    }
    private static void SetMoveInWaterMotionData() {
        moveInWaterMotionData = defaultMotionData;
        moveInWaterMotionData.motionTime = CRAWFISH_CATCHING_MOVE_IN_WATER_ANIMATION_TIME;
        moveInWaterMotionData.pose_BODY.pose_Angle = new Vector3[2]
        {
            new Vector3(0f, 350f, 0f),
            new Vector3(0f, 10f, 0f)
        };
        moveInWaterMotionData.pose_BODY.motionPingPong = true;
        moveInWaterMotionData.pose_SHOULDER_L.pose_Angle = new Vector3[2]
        {
            new Vector3(0f, 20f, 270f),
            new Vector3(0f, 350f, 270f)
        };
        moveInWaterMotionData.pose_SHOULDER_L.motionPingPong = true;
        moveInWaterMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[2]
        {
            new Vector3(0f, 35f, 90f),
            new Vector3(0f, 350f, 90f)
        };
        moveInWaterMotionData.pose_SHOULDER_R.motionPingPong = true;
        moveInWaterMotionData.pose_ARM_L.pose_Angle = new Vector3[1]
        {
            new Vector3(280f, 0f, 0f)
        };
        moveInWaterMotionData.pose_ARM_R.pose_Angle = new Vector3[1]
        {
            new Vector3(330f, 0f, 0f)
        };
        moveInWaterMotionData.pose_LEG_R.pose_Angle = new Vector3[2]
        {
            new Vector3(-40f, 0f, 0f),
            new Vector3(40f, 0f, 0f)
        };
        moveInWaterMotionData.pose_LEG_R.motionPingPong = true;
        moveInWaterMotionData.pose_LEG_L.pose_Angle = new Vector3[2]
        {
            new Vector3(40f, 0f, 0f),
            new Vector3(-40f, 0f, 0f)
        };
        moveInWaterMotionData.pose_LEG_L.motionPingPong = true;
    }
    public static MotionData GetMoveInWaterMotionData() {
        return moveInWaterMotionData;
    }
    private static void SetScoopMotionData() {
        scoopMotionData = defaultMotionData;
        scoopMotionData.motionTime = CRAWFISH_CATCHING_SCOOP_NET_ANIMATION_TIME;
        scoopMotionData.pose_BODY.pose_Angle = new Vector3[2]
        {
            new Vector3(5f, 345f, 0f),
            new Vector3(0f, 10f, 0f)
        };
        scoopMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[2]
        {
            new Vector3(10f, 0f, 0f),
            new Vector3(350f, 0f, 0f)
        };
        scoopMotionData.pose_ARM_R.pose_Angle = new Vector3[2]
        {
            new Vector3(25f, 0f, 0f),
            new Vector3(290f, 0f, 0f)
        };
    }
    public static MotionData GetScoopMotionData() {
        return scoopMotionData;
    }
    private static void SetCatchCrawfishMotionData() {
        catchCrawfishMotionData = defaultMotionData;
        catchCrawfishMotionData.motionTime = CRAWFISH_CATCHING_CATCH_CRAWFISH_ANIMATION_TIME;
        catchCrawfishMotionData.pose_BODY.pose_Angle = new Vector3[5]
        {
            new Vector3(10f, 0f, 0f),
            new Vector3(335f, 0f, 0f),
            new Vector3(330f, 0f, 0f),
            new Vector3(335f, 0f, 0f),
            new Vector3(0f, 0f, 0f)
        };
        catchCrawfishMotionData.pose_SHOULDER_L.pose_Angle = new Vector3[5]
        {
            new Vector3(320f, 0f, 0f),
            new Vector3(220f, 0f, 0f),
            new Vector3(210f, 0f, 0f),
            new Vector3(220f, 0f, 0f),
            new Vector3(320f, 0f, 0f)
        };
        catchCrawfishMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[5]
        {
            new Vector3(320f, 0f, 0f),
            new Vector3(220f, 0f, 0f),
            new Vector3(210f, 0f, 0f),
            new Vector3(220f, 0f, 0f),
            new Vector3(320f, 0f, 0f)
        };
    }
    public static MotionData GetCatchCrawfishMotionData() {
        return catchCrawfishMotionData;
    }
    private static void SetBucketPutOnBalanceMotionData() {
        bucketPutOnBalanceMotionData = defaultMotionData;
        bucketPutOnBalanceMotionData.motionTime = CRAWFISH_CATCHING_BUCKET_PUT_ON_BALANCE_ANIMATION_TIME;
        bucketPutOnBalanceMotionData.pose_BODY.pose_Angle = new Vector3[2]
        {
            new Vector3(0f, 340f, 0f),
            new Vector3(0f, 330f, 0f)
        };
        bucketPutOnBalanceMotionData.pose_SHOULDER_R.pose_Pos = new Vector3[1]
        {
            new Vector3(0.15f, 0.14f, 0.07f)
        };
        bucketPutOnBalanceMotionData.pose_SHOULDER_R.pose_Angle = new Vector3[1]
        {
            new Vector3(280f, 0f, 0f)
        };
    }
    public static MotionData GetBucketPutOnBalanceMotionData() {
        return bucketPutOnBalanceMotionData;
    }
}
