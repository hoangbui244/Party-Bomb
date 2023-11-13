using UnityEngine;
public class CheckLeanTweenScene : MonoBehaviour {
    public enum AnimationType {
        Wait,
        Move,
        Happy,
        HappyJump,
        Sadly,
        Excite,
        GutsPose1,
        GutsPose2,
        DashStandby,
        Dash,
        FullPowerDash,
        BeforeRunner_BatonPass,
        NextRunner_Wait,
        NextRunner_BatonReceive,
        SlowDash,
        LookAtLastAnchor,
        Sit,
        RopeHoldSit,
        StandUp,
        Pull,
        RodSetUp,
        RodCast,
        MoveInWater,
        Scoop,
        CatchCrawfish,
        BucketPutOnBalance
    }
    [SerializeField]
    [Header("キャラクタ\u30fcのモ\u30fcション処理")]
    private CharaLeanTweenMotion motion;
    private AnimationType currentAnimType;
    private float guiButtonOffset_X = 180f;
    private float guiButtonOffset_Y = 60f;
    private void Awake() {
        CharaLeanTweenMotionData.CreateMotionData();
        motion.Init();
    }
    private void OnGUI() {
        GUI.skin.button.fontSize = 20;
        if (GUI.Button(new Rect(20f, 20f, 150f, 50f), "Wait")) {
            SetAnimation(AnimationType.Wait, _isMotion: true);
        }
        if (GUI.Button(new Rect(20f + guiButtonOffset_X, 20f, 150f, 50f), "Dash")) {
            SetAnimation(AnimationType.Dash, _isMotion: true);
        }
        if (GUI.Button(new Rect(20f + guiButtonOffset_X * 2f, 20f, 150f, 50f), "DashStandby")) {
            SetAnimation(AnimationType.DashStandby, _isMotion: true);
        }
        if (GUI.Button(new Rect(20f + guiButtonOffset_X * 3f, 20f, 150f, 50f), "Move")) {
            SetAnimation(AnimationType.Move, _isMotion: true);
        }
        if (GUI.Button(new Rect(20f + guiButtonOffset_X * 4f, 20f, 150f, 50f), "BeforeRunner_BatonPass")) {
            SetAnimation(AnimationType.BeforeRunner_BatonPass, _isMotion: true);
        }
        if (GUI.Button(new Rect(20f + guiButtonOffset_X * 5f, 20f, 150f, 50f), "NextRunner_Wait")) {
            SetAnimation(AnimationType.NextRunner_Wait, _isMotion: true);
        }
        if (GUI.Button(new Rect(20f + guiButtonOffset_X * 6f, 20f, 150f, 50f), "NextRunner_BatonReceive")) {
            SetAnimation(AnimationType.NextRunner_BatonReceive, _isMotion: true);
        }
        if (GUI.Button(new Rect(20f + guiButtonOffset_X * 7f, 20f, 150f, 50f), "Emote_Happy")) {
            SetAnimation(AnimationType.Happy, _isMotion: true);
        }
        if (GUI.Button(new Rect(20f + guiButtonOffset_X * 8f, 20f, 150f, 50f), "Emote_Sadly")) {
            SetAnimation(AnimationType.Sadly, _isMotion: true);
        }
        if (GUI.Button(new Rect(20f + guiButtonOffset_X * 9f, 20f, 150f, 50f), "SlowDash")) {
            SetAnimation(AnimationType.SlowDash, _isMotion: true);
        }
        if (GUI.Button(new Rect(20f, 20f + guiButtonOffset_Y, 150f, 50f), "FullPowerDash")) {
            SetAnimation(AnimationType.FullPowerDash, _isMotion: true);
        }
        if (GUI.Button(new Rect(20f + guiButtonOffset_X, 20f + guiButtonOffset_Y, 150f, 50f), "SitDown")) {
            SetAnimation(AnimationType.Sit, _isMotion: true);
        }
        if (GUI.Button(new Rect(20f + guiButtonOffset_X * 2f, 20f + guiButtonOffset_Y, 150f, 50f), "SitDown_HaveRope")) {
            SetAnimation(AnimationType.RopeHoldSit, _isMotion: true);
        }
        if (GUI.Button(new Rect(20f + guiButtonOffset_X * 3f, 20f + guiButtonOffset_Y, 150f, 50f), "PullRope")) {
            SetAnimation(AnimationType.Pull, _isMotion: true);
        }
        if (GUI.Button(new Rect(20f + guiButtonOffset_X * 4f, 20f + guiButtonOffset_Y, 150f, 50f), "StandUp")) {
            SetAnimation(AnimationType.StandUp, _isMotion: true);
        }
        if (GUI.Button(new Rect(20f + guiButtonOffset_X * 5f, 20f + guiButtonOffset_Y, 150f, 50f), "HappyJump")) {
            SetAnimation(AnimationType.HappyJump, _isMotion: true);
        }
        if (GUI.Button(new Rect(20f + guiButtonOffset_X * 6f, 20f + guiButtonOffset_Y, 150f, 50f), "RodSetUp")) {
            SetAnimation(AnimationType.RodSetUp, _isMotion: true);
        }
        if (GUI.Button(new Rect(20f + guiButtonOffset_X * 7f, 20f + guiButtonOffset_Y, 150f, 50f), "RodCast")) {
            SetAnimation(AnimationType.RodCast, _isMotion: true);
        }
        if (GUI.Button(new Rect(20f + guiButtonOffset_X * 8f, 20f + guiButtonOffset_Y, 150f, 50f), "MoveInWater")) {
            SetAnimation(AnimationType.MoveInWater, _isMotion: true);
        }
        if (GUI.Button(new Rect(20f + guiButtonOffset_X * 9f, 20f + guiButtonOffset_Y, 150f, 50f), "Scoop")) {
            SetAnimation(AnimationType.Scoop, _isMotion: true);
        }
        if (GUI.Button(new Rect(20f, 20f + guiButtonOffset_Y * 2f, 150f, 50f), "CatchCrawfish")) {
            SetAnimation(AnimationType.CatchCrawfish, _isMotion: true);
        }
        if (GUI.Button(new Rect(20f + guiButtonOffset_X, 20f + guiButtonOffset_Y * 2f, 150f, 50f), "BucketPutOnBalance")) {
            SetAnimation(AnimationType.BucketPutOnBalance, _isMotion: true);
        }
        if (GUI.Button(new Rect(20f + guiButtonOffset_X * 2f, 20f + guiButtonOffset_Y * 2f, 150f, 50f), "Excite")) {
            SetAnimation(AnimationType.Excite, _isMotion: true);
        }
        if (GUI.Button(new Rect(20f + guiButtonOffset_X * 3f, 20f + guiButtonOffset_Y * 2f, 150f, 50f), "GutsPose1")) {
            SetAnimation(AnimationType.GutsPose1, _isMotion: true);
        }
        if (GUI.Button(new Rect(20f + guiButtonOffset_X * 4f, 20f + guiButtonOffset_Y * 2f, 150f, 50f), "GutsPose1")) {
            SetAnimation(AnimationType.GutsPose2, _isMotion: true);
        }
    }
    public void SetAnimation(AnimationType _animType, bool _isMotion = false) {
        if (_animType != currentAnimType) {
            currentAnimType = _animType;
            switch (currentAnimType) {
                case AnimationType.Wait:
                    WaitAnimation(_isMotion);
                    break;
                case AnimationType.Move:
                    MoveAnimation(_isMotion);
                    break;
                case AnimationType.DashStandby:
                    DashStandbyAnimation(_isMotion);
                    break;
                case AnimationType.Dash:
                    DashAnimation(_isMotion);
                    break;
                case AnimationType.FullPowerDash:
                    FullPowerDashAnimation(_isMotion);
                    break;
                case AnimationType.NextRunner_Wait:
                    NextRunnerWaitAnimation(_isMotion);
                    break;
                case AnimationType.NextRunner_BatonReceive:
                    NextRunnerReceiveBatonAnimation(_isMotion);
                    break;
                case AnimationType.BeforeRunner_BatonPass:
                    BeforeRunner_BatonPassAnimation(_isMotion);
                    break;
                case AnimationType.SlowDash:
                    SlowDashAnimation(_isMotion);
                    break;
                case AnimationType.LookAtLastAnchor:
                    WaitAnimation(_isMotion);
                    break;
                case AnimationType.Happy:
                    HappyAnimation(_isMotion);
                    break;
                case AnimationType.Sadly:
                    SadlyAnimation(_isMotion);
                    break;
                case AnimationType.Sit:
                    SitAnimation(_isMotion);
                    break;
                case AnimationType.RopeHoldSit:
                    RopeHoldSitAnimation(_isMotion);
                    break;
                case AnimationType.StandUp:
                    StandUpAnimation(_isMotion);
                    break;
                case AnimationType.Pull:
                    RopePullAnimation(_isMotion);
                    break;
                case AnimationType.HappyJump:
                    HappyJumpAnimation(_isMotion);
                    break;
                case AnimationType.RodSetUp:
                    RodSetUpAnimation(_isMotion);
                    break;
                case AnimationType.RodCast:
                    RodCastAnimation(_isMotion);
                    break;
                case AnimationType.MoveInWater:
                    MoveInWaterAnimation(_isMotion);
                    break;
                case AnimationType.Scoop:
                    ScoopAnimation(_isMotion);
                    break;
                case AnimationType.CatchCrawfish:
                    CatchCrawfishAnimation(_isMotion);
                    break;
                case AnimationType.BucketPutOnBalance:
                    BucketPutOnBalanceAnimation(_isMotion);
                    break;
                case AnimationType.Excite:
                    ExciteAnimation(_isMotion);
                    break;
                case AnimationType.GutsPose1:
                    GutsPose1Animation(_isMotion);
                    break;
                case AnimationType.GutsPose2:
                    GutsPose2Animation(_isMotion);
                    break;
            }
        }
    }
    private void WaitAnimation(bool _isMotion) {
        CharaLeanTweenMotionData.MotionData defaultMotionData = CharaLeanTweenMotionData.GetDefaultMotionData();
        if (!_isMotion) {
            defaultMotionData.motionTime = 0f;
        }
        motion.StartMotion(defaultMotionData);
    }
    private void MoveAnimation(bool _isMotion) {
        if (currentAnimType == AnimationType.Move) {
            CharaLeanTweenMotionData.MotionData moveMotionData = CharaLeanTweenMotionData.GetMoveMotionData();
            if (!_isMotion) {
                moveMotionData.motionTime = 0f;
            }
            motion.StartMotion(moveMotionData);
        }
    }
    private void HappyAnimation(bool _isMotion) {
        if (currentAnimType == AnimationType.Happy) {
            CharaLeanTweenMotionData.MotionData happyMotionData = CharaLeanTweenMotionData.GetHappyMotionData();
            if (!_isMotion) {
                happyMotionData.motionTime = 0f;
            }
            motion.StartMotion(happyMotionData);
        }
    }
    private void HappyJumpAnimation(bool _isMotion) {
        if (currentAnimType == AnimationType.HappyJump) {
            CharaLeanTweenMotionData.MotionData happyJumpMotionData = CharaLeanTweenMotionData.GetHappyJumpMotionData();
            if (!_isMotion) {
                happyJumpMotionData.motionTime = 0f;
            }
            motion.StartMotion(happyJumpMotionData);
        }
    }
    private void SadlyAnimation(bool _isMotion) {
        if (currentAnimType == AnimationType.Sadly) {
            CharaLeanTweenMotionData.MotionData sadlyMotionData = CharaLeanTweenMotionData.GetSadlyMotionData();
            if (!_isMotion) {
                sadlyMotionData.motionTime = 0f;
            }
            motion.StartMotion(sadlyMotionData);
        }
    }
    private void ExciteAnimation(bool _isMotion) {
        if (currentAnimType == AnimationType.Excite) {
            CharaLeanTweenMotionData.MotionData exciteMotionData = CharaLeanTweenMotionData.GetExciteMotionData();
            if (!_isMotion) {
                exciteMotionData.motionTime = 0f;
            }
            motion.StartMotion(exciteMotionData);
        }
    }
    private void GutsPose1Animation(bool _isMotion) {
        if (currentAnimType == AnimationType.GutsPose1) {
            CharaLeanTweenMotionData.MotionData gutsPose1MotionData = CharaLeanTweenMotionData.GetGutsPose1MotionData();
            if (!_isMotion) {
                gutsPose1MotionData.motionTime = 0f;
            }
            motion.StartMotion(gutsPose1MotionData);
        }
    }
    private void GutsPose2Animation(bool _isMotion) {
        if (currentAnimType == AnimationType.GutsPose2) {
            CharaLeanTweenMotionData.MotionData gutsPose2MotionData = CharaLeanTweenMotionData.GetGutsPose2MotionData();
            if (!_isMotion) {
                gutsPose2MotionData.motionTime = 0f;
            }
            motion.StartMotion(gutsPose2MotionData);
        }
    }
    private void DashStandbyAnimation(bool _isMotion) {
        CharaLeanTweenMotionData.MotionData dashStandbyMotionData = CharaLeanTweenMotionData.GetDashStandbyMotionData();
        if (!_isMotion) {
            dashStandbyMotionData.motionTime = 0f;
        }
        motion.StartMotion(dashStandbyMotionData);
    }
    private void DashAnimation(bool _isMotion) {
        if (currentAnimType == AnimationType.Dash) {
            CharaLeanTweenMotionData.MotionData dashMotionData = CharaLeanTweenMotionData.GetDashMotionData();
            if (!_isMotion) {
                dashMotionData.motionTime = 0f;
            }
            motion.StartMotion(dashMotionData);
        }
    }
    private void FullPowerDashAnimation(bool _isMotion) {
        if (currentAnimType == AnimationType.FullPowerDash) {
            CharaLeanTweenMotionData.MotionData fullPowerDashMotionData = CharaLeanTweenMotionData.GetFullPowerDashMotionData();
            if (!_isMotion) {
                fullPowerDashMotionData.motionTime = 0f;
            }
            motion.StartMotion(fullPowerDashMotionData);
        }
    }
    private void NextRunnerWaitAnimation(bool _isMotion) {
        CharaLeanTweenMotionData.MotionData nextRunnnerWaitMotionData = CharaLeanTweenMotionData.GetNextRunnnerWaitMotionData();
        if (!_isMotion) {
            nextRunnnerWaitMotionData.motionTime = 0f;
        }
        motion.StartMotion(nextRunnnerWaitMotionData);
    }
    private void NextRunnerReceiveBatonAnimation(bool _isMotion) {
        CharaLeanTweenMotionData.MotionData nextRunnerReceiveBatonMotionData = CharaLeanTweenMotionData.GetNextRunnerReceiveBatonMotionData();
        if (!_isMotion) {
            nextRunnerReceiveBatonMotionData.motionTime = 0f;
        }
        motion.StartMotion(nextRunnerReceiveBatonMotionData);
    }
    private void BeforeRunner_BatonPassAnimation(bool _isMotion) {
        CharaLeanTweenMotionData.MotionData batonPassMotionData = CharaLeanTweenMotionData.GetBatonPassMotionData();
        if (!_isMotion) {
            batonPassMotionData.motionTime = 0f;
        }
        motion.StartMotion(batonPassMotionData);
    }
    private void SlowDashAnimation(bool _isMotion) {
        if (currentAnimType == AnimationType.SlowDash) {
            CharaLeanTweenMotionData.MotionData slowDashMotionData = CharaLeanTweenMotionData.GetSlowDashMotionData();
            if (!_isMotion) {
                slowDashMotionData.motionTime = 0f;
            }
            motion.StartMotion(slowDashMotionData);
        }
    }
    private void SitAnimation(bool _isMotion) {
        CharaLeanTweenMotionData.MotionData sitMotionData = CharaLeanTweenMotionData.GetSitMotionData();
        if (!_isMotion) {
            sitMotionData.motionTime = 0f;
        }
        motion.StartMotion(sitMotionData);
    }
    private void RopeHoldSitAnimation(bool _isMotion) {
        CharaLeanTweenMotionData.MotionData ropeHoldSitMotionData_Right = CharaLeanTweenMotionData.GetRopeHoldSitMotionData_Right();
        if (!_isMotion) {
            ropeHoldSitMotionData_Right.motionTime = 0f;
        }
        motion.StartMotion(ropeHoldSitMotionData_Right);
    }
    private void StandUpAnimation(bool _isMotion) {
        CharaLeanTweenMotionData.MotionData standUpMotionData = CharaLeanTweenMotionData.GetStandUpMotionData();
        if (!_isMotion) {
            standUpMotionData.motionTime = 0f;
        }
        motion.StartMotion(standUpMotionData);
    }
    private void RopePullAnimation(bool _isMotion) {
        if (currentAnimType == AnimationType.Pull) {
            CharaLeanTweenMotionData.MotionData pullRopeMotionData_Right = CharaLeanTweenMotionData.GetPullRopeMotionData_Right();
            if (!_isMotion) {
                pullRopeMotionData_Right.motionTime = 0f;
            }
            motion.StartMotion(pullRopeMotionData_Right);
        }
    }
    private void RodSetUpAnimation(bool _isMotion) {
        if (currentAnimType == AnimationType.RodSetUp) {
            CharaLeanTweenMotionData.MotionData rodSetUpMotionData = CharaLeanTweenMotionData.GetRodSetUpMotionData();
            if (!_isMotion) {
                rodSetUpMotionData.motionTime = 0f;
            }
            motion.StartMotion(rodSetUpMotionData);
        }
    }
    private void RodCastAnimation(bool _isMotion) {
        if (currentAnimType == AnimationType.RodCast) {
            CharaLeanTweenMotionData.MotionData rodCastMotionData = CharaLeanTweenMotionData.GetRodCastMotionData();
            if (!_isMotion) {
                rodCastMotionData.motionTime = 0f;
            }
            motion.StartMotion(rodCastMotionData);
        }
    }
    private void MoveInWaterAnimation(bool _isMotion) {
        if (currentAnimType == AnimationType.MoveInWater) {
            CharaLeanTweenMotionData.MotionData moveInWaterMotionData = CharaLeanTweenMotionData.GetMoveInWaterMotionData();
            if (!_isMotion) {
                moveInWaterMotionData.motionTime = 0f;
            }
            motion.StartMotion(moveInWaterMotionData);
        }
    }
    private void ScoopAnimation(bool _isMotion) {
        if (currentAnimType == AnimationType.Scoop) {
            CharaLeanTweenMotionData.MotionData scoopMotionData = CharaLeanTweenMotionData.GetScoopMotionData();
            if (!_isMotion) {
                scoopMotionData.motionTime = 0f;
            }
            motion.StartMotion(scoopMotionData);
        }
    }
    private void CatchCrawfishAnimation(bool _isMotion) {
        if (currentAnimType == AnimationType.CatchCrawfish) {
            CharaLeanTweenMotionData.MotionData catchCrawfishMotionData = CharaLeanTweenMotionData.GetCatchCrawfishMotionData();
            if (!_isMotion) {
                catchCrawfishMotionData.motionTime = 0f;
            }
            motion.StartMotion(catchCrawfishMotionData);
        }
    }
    private void BucketPutOnBalanceAnimation(bool _isMotion) {
        if (currentAnimType == AnimationType.BucketPutOnBalance) {
            CharaLeanTweenMotionData.MotionData bucketPutOnBalanceMotionData = CharaLeanTweenMotionData.GetBucketPutOnBalanceMotionData();
            if (!_isMotion) {
                bucketPutOnBalanceMotionData.motionTime = 0f;
            }
            motion.StartMotion(bucketPutOnBalanceMotionData);
        }
    }
}
