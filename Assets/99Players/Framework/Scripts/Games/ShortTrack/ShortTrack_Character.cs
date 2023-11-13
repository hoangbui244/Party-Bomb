using Cinemachine;
using System;
using UnityEngine;
public class ShortTrack_Character : MonoBehaviour {
    public enum BodyPartsList {
        HEAD,
        BODY,
        HIP,
        SHOULDER_L,
        SHOULDER_R,
        ARM_L,
        ARM_R,
        LEG_L,
        LEG_R
    }
    [Serializable]
    public struct BodyParts {
        [SerializeField]
        [Header("体アンカ\u30fc")]
        public GameObject bodyAnchor;
        public MeshRenderer[] rendererList;
        public Transform Parts(BodyPartsList _parts) {
            return rendererList[(int)_parts].transform;
        }
        public Transform Parts(int _parts) {
            return rendererList[_parts].transform;
        }
    }
    public enum AnimationType {
        Standby,
        Move,
        DashStandby,
        Dash,
        FullPowerDash,
        SlowDash,
        Curve,
        FullCurve,
        Tired,
        TiredCurve
    }
    private enum CPU_MOVE_STATE {
        LEFT_MOVE,
        RIGHT_MOVE
    }
    [SerializeField]
    [Header("足元のスパ\u30fcク")]
    private ParticleSystem LegIce;
    [SerializeField]
    [Header("手元にあるスパ\u30fcク")]
    private ParticleSystem ArmIce;
    [SerializeField]
    [Header("風が出るエフェクト")]
    private ParticleSystem MaxSpeed;
    [SerializeField]
    [Header("スリップストリ\u30fcム中のエフェクト")]
    private ParticleSystem slipStreamEffect;
    [SerializeField]
    private ShortTrack_Slipstream slipStream;
    [SerializeField]
    [Header("最高速度出たときに出る集中線")]
    private ShortTrack_ConcentratedLine concentratedLine;
    [SerializeField]
    [Header("スタミナ切れたときの汗")]
    private ParticleSystem noStaminaSweatEffect;
    [SerializeField]
    [Header("スリップストリ\u30fcムの頭のポジション")]
    private Transform HeadPos;
    [SerializeField]
    [Header("キャラスタイル")]
    private CharacterStyle charaStyle;
    [SerializeField]
    [Header("体のパ\u30fcツ")]
    private BodyParts bodyParts;
    private bool isStyleFat;
    private float staminaPoint = 100f;
    private const float STAMINA_MAX_POINT = 100f;
    private const float STAMINA_INCREASE = 10f;
    private const float STAMINA_DECREASE = 2f;
    public bool outStamina;
    public bool isPlayer;
    private int playerNum;
    private bool isRunnerStart;
    public bool isCurve;
    private CinemachineDollyCart dollyCart;
    private float slipStreamRotTime;
    [SerializeField]
    [Header("キャラクタ\u30fcのモ\u30fcション処理")]
    private ShortTrack_CharaLeanTweenMotion motion;
    private AnimationType currentAnimType;
    private Transform lookTarget;
    private SHORTTRACK.AiStrength aiStrength;
    private float ADD_ACCEL_SPEED_CPU;
    private const float ADD_ACCEL_SPEED_CPU_WEAK = 0.15f;
    private const float ADD_ACCEL_SPEED_CPU_COMMON = 0.2f;
    private const float ADD_ACCEL_SPEED_CPU_STRONG = 0.25f;
    private float MAX_ACCEL_SPEED_CPU;
    private const float MAX_ACCEL_SPEED_CPU_WEAK = 8f;
    private const float MAX_ACCEL_SPEED_CPU_COMMON = 9f;
    private const float MAX_ACCEL_SPEED_CPU_STRONG = 10f;
    private float INPUT_ACCEL_SPEED_INTERVAL_CPU;
    private const float INPUT_ACCEL_SPEED_INTERVAL_CPU_WEAK = 0.15f;
    private const float INPUT_ACCEL_SPEED_INTERVAL_CPU_COMMON = 0.1f;
    private const float INPUT_ACCEL_SPEED_INTERVAL_CPU_STRONG = 0f;
    private float inputAccelSpeedInterval;
    private float CPU_RANDDM_DATA_CHANGE_TIME;
    private const float CPU_RANDDM_DATA_CHANGE_TIME_MIN = 1.5f;
    private const float CPU_RANDDM_DATA_CHANGE_TIME_MAX = 3f;
    private float cpuRanddmDataChangeTime;
    private float currentCheckMoveIntervalTime;
    private const float CHECK_MOVE_TIME = 1f;
    private Vector3 moveVec_CPU = Vector3.zero;
    private CPU_MOVE_STATE moveState_CPU;
    private const float CHARA_DEFAULT_RUN_SPEED = 5f;
    private const float CHARA_NORMAL_RUN_SPEED = 10f;
    private const float CHARA_FULL_POWER_RUN_SPEED = 15f;
    private float addjustRankSpeed;
    private const float ADD_BUTTON_INPUT_ACCEL_SPEED = 0.25f;
    private const float MAX_BUTTON_INPUT_ACCEL_SPEED = 10f;
    private const float REMAIN_BUTTON_INPUT_ACCEL_TIME = 0.25f;
    private const float REDUCE_BUTTON_INPUT_ACCEL_SPEED = 1f;
    private float currentRemainButtonInputAccelTime;
    private float buttonInputAccelCharaSpeed;
    private const float MAX_CURVE_SPEED = 3f;
    private const float ADD_CURVE_SPEED = 0.02f;
    private float curveAccelCharaSpeed;
    public float CurveCenter;
    private const float MaxCurveGage = 1f;
    private const float MinCurveGage = 0f;
    private bool isSlipStream;
    private const float SLIP_STEAM_UNTIL_TIME = 0.25f;
    private float slipSteamUtilTime;
    private const float MAX_SLIP_STREAM_ACCEL_SPEED = 2.5f;
    private const float ADD_SLIP_STREAM_ACCEL_SPEED = 0.25f;
    private const float REMAIN_SLIP_STREAM_ACCEL_TIME = 0.45f;
    private const float REDUCE_SLIP_STREAM_ACCEL_SPEED = 1f;
    private float currentRemainSlipStreamAccelTime;
    private float slipStreamAccelCharaSpeed;
    private const float MOVE_SPEED = 3f;
    private const float MOVE_LIMIT_LEFT = -3f;
    private const float MOVE_LIMIT_RIGHT = 3f;
    private float runAnimationSpeed = 100f;
    private float runAnimationTime;
    private int playSeRunCnt;
    private float runSeInterval;
    private bool isChangeAnimationNeutral;
    private Vector3 prevPos;
    private Vector3 nowPos;
    private float moveSePlayTimeInterval;
    private bool isAllowMove_Left;
    private bool isAllowMove_Right;
    private bool isAllowMove_Front;
    private bool isLimitMove_Left;
    private bool isLimitMove_Right;
    [SerializeField]
    [Header("前方のチェックコライダ\u30fc")]
    private ShortTrack_CheckCollision frontCheckCollider;
    [SerializeField]
    [Header("左側のチェックコライダ\u30fc")]
    private ShortTrack_CheckCollision leftCheckCollider;
    [SerializeField]
    [Header("右側のチェックコライダ\u30fc")]
    private ShortTrack_CheckCollision rightCheckCollider;
    [SerializeField]
    [Header("スリップストリ\u30fcム用のチェックコライダ\u30fc")]
    private ShortTrack_CheckCollision slipStreamCheckCollider;
    private float frontCollisionSpeed;
    [SerializeField]
    [Header("カ\u30fcソル画像")]
    private SpriteRenderer cursorSprite;
    [SerializeField]
    [Header("ベ\u30fcスサイズ")]
    private Vector3 baseScale;
    public static float SCALE_ANI_SPEED = 6f;
    private float scaleAniTime;
    [SerializeField]
    [Header("鏡見たいな奴")]
    private MeshRenderer Reflection;
    [SerializeField]
    [Header("床に打つ映るキャラクタ\u30fc画像")]
    private Texture[] ReflectionSprite;
    private bool isWaitAddLapCnt;
    public float StaminaPoint => staminaPoint;
    public float StaminaMaxPoint => 100f;
    public float Stamina_Decrease => 2f;
    public int PlayerNum => playerNum;
    public bool IsRunnerStart {
        get {
            return isRunnerStart;
        }
        set {
            isRunnerStart = value;
        }
    }
    public float chara_full_power_run_speed => 15f;
    public float CharaSpeed {
        get {
            return buttonInputAccelCharaSpeed;
        }
        set {
            buttonInputAccelCharaSpeed = value;
        }
    }
    public void Init(int _playerNum) {
        staminaPoint = 100f;
        playerNum = _playerNum;
        aiStrength = (SHORTTRACK.AiStrength)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
        SetCPURandomData(_isInit: true);
        motion.Init();
        SetAnimation(AnimationType.Standby);
        ChangeCursorSprite();
        NonActiveCursor();
        ActiveCursor();
        addjustRankSpeed = 1f;
        frontCheckCollider.Init(this);
        leftCheckCollider.Init(this);
        rightCheckCollider.Init(this);
        slipStreamCheckCollider.Init(this);
    }
    private void SetCPURandomData(bool _isInit = false) {
        if (_isInit || cpuRanddmDataChangeTime > CPU_RANDDM_DATA_CHANGE_TIME) {
            cpuRanddmDataChangeTime = 0f;
            CPU_RANDDM_DATA_CHANGE_TIME = UnityEngine.Random.Range(1.5f, 3f);
            switch (aiStrength) {
                case SHORTTRACK.AiStrength.WEAK:
                    ADD_ACCEL_SPEED_CPU = UnityEngine.Random.Range(71f / (452f * (float)Math.PI), 0.25f);
                    ADD_ACCEL_SPEED_CPU = Mathf.Clamp(ADD_ACCEL_SPEED_CPU, 0f, 0.25f);
                    MAX_ACCEL_SPEED_CPU = UnityEngine.Random.Range(7f, 9f);
                    MAX_ACCEL_SPEED_CPU = Mathf.Clamp(MAX_ACCEL_SPEED_CPU, 0f, 10f);
                    INPUT_ACCEL_SPEED_INTERVAL_CPU = UnityEngine.Random.Range(71f / (452f * (float)Math.PI), 0.25f);
                    INPUT_ACCEL_SPEED_INTERVAL_CPU = Mathf.Clamp(INPUT_ACCEL_SPEED_INTERVAL_CPU, 0f, 0.25f);
                    break;
                case SHORTTRACK.AiStrength.COMMON:
                    ADD_ACCEL_SPEED_CPU = UnityEngine.Random.Range(0.1f, 0.3f);
                    ADD_ACCEL_SPEED_CPU = Mathf.Clamp(ADD_ACCEL_SPEED_CPU, 0f, 0.25f);
                    MAX_ACCEL_SPEED_CPU = UnityEngine.Random.Range(8f, 10f);
                    MAX_ACCEL_SPEED_CPU = Mathf.Clamp(MAX_ACCEL_SPEED_CPU, 0f, 10f);
                    INPUT_ACCEL_SPEED_INTERVAL_CPU = UnityEngine.Random.Range(0f, 0.2f);
                    INPUT_ACCEL_SPEED_INTERVAL_CPU = Mathf.Clamp(INPUT_ACCEL_SPEED_INTERVAL_CPU, 0f, 0.2f);
                    break;
                case SHORTTRACK.AiStrength.STRONG:
                    ADD_ACCEL_SPEED_CPU = UnityEngine.Random.Range(0.15f, 0.35f);
                    ADD_ACCEL_SPEED_CPU = Mathf.Clamp(ADD_ACCEL_SPEED_CPU, 0f, 0.25f);
                    MAX_ACCEL_SPEED_CPU = UnityEngine.Random.Range(9f, 11f);
                    MAX_ACCEL_SPEED_CPU = Mathf.Clamp(MAX_ACCEL_SPEED_CPU, 0f, 10f);
                    INPUT_ACCEL_SPEED_INTERVAL_CPU = UnityEngine.Random.Range(-0.1f, 0.1f);
                    INPUT_ACCEL_SPEED_INTERVAL_CPU = Mathf.Clamp(INPUT_ACCEL_SPEED_INTERVAL_CPU, 0f, 0.1f);
                    break;
            }
        } else {
            cpuRanddmDataChangeTime += Time.deltaTime;
        }
    }
    public void UpdateMethod() {
        UpdateAnimation();
        UpdateEffect();
        concentratedLine.UpdateMethod();
        UpdateCursorAnimation();
        SetCPURandomData();
        if (!isRunnerStart) {
            return;
        }
        SetAddJustRankSpeed();
        if (SHORTTRACK.MCM.PData[playerNum].isGoal) {
            CheckLimitMove_Front();
            if (!(dollyCart.m_Path != null) || !isAllowMove_Front) {
                return;
            }
            if (buttonInputAccelCharaSpeed > 0f) {
                buttonInputAccelCharaSpeed -= Time.deltaTime;
                if (buttonInputAccelCharaSpeed < 0f) {
                    buttonInputAccelCharaSpeed = 0f;
                }
            }
            if (buttonInputAccelCharaSpeed > 0f) {
                buttonInputAccelCharaSpeed -= Time.deltaTime;
                if (buttonInputAccelCharaSpeed < 0f) {
                    buttonInputAccelCharaSpeed = 0f;
                }
            }
            if (slipStreamAccelCharaSpeed > 0f) {
                slipStreamAccelCharaSpeed -= Time.deltaTime;
                if (slipStreamAccelCharaSpeed < 0f) {
                    slipStreamAccelCharaSpeed = 0f;
                }
            }
            if (curveAccelCharaSpeed > 0f) {
                curveAccelCharaSpeed -= Time.deltaTime;
                if (curveAccelCharaSpeed < 0f) {
                    curveAccelCharaSpeed = 0f;
                }
            }
            float num = (5f + buttonInputAccelCharaSpeed + slipStreamAccelCharaSpeed + curveAccelCharaSpeed) * addjustRankSpeed;
            dollyCart.m_Position = Mathf.Clamp(dollyCart.m_Position + num * Time.deltaTime, 0f, dollyCart.m_Path.PathLength);
            return;
        }
        CheckLimitMove_Front();
        CheckLimitMove_Left();
        CheckLimitMove_Right();
        base.transform.SetLocalPositionX(Mathf.Clamp(base.transform.localPosition.x, -3f, 3f));
        base.transform.SetLocalPositionZ(0f);
        if (currentRemainButtonInputAccelTime > 0.25f) {
            if (outStamina) {
                buttonInputAccelCharaSpeed -= 2f * Time.deltaTime;
            } else {
                buttonInputAccelCharaSpeed -= 1f * Time.deltaTime;
            }
            if (buttonInputAccelCharaSpeed < 0f) {
                buttonInputAccelCharaSpeed = 0f;
            }
        } else {
            currentRemainButtonInputAccelTime += Time.deltaTime;
        }
        SetIsSlipStream();
        if (isSlipStream) {
            float slipStreamAccelSpeed = slipStreamAccelCharaSpeed + 0.25f * Time.deltaTime;
            SetSlipStreamAccelSpeed(slipStreamAccelSpeed);
        } else if (currentRemainSlipStreamAccelTime > 0.45f) {
            slipStreamAccelCharaSpeed -= 1f * Time.deltaTime;
            if (slipStreamAccelCharaSpeed < 0f) {
                slipStreamAccelCharaSpeed = 0f;
            }
        } else {
            currentRemainSlipStreamAccelTime += Time.deltaTime;
        }
        if (dollyCart.m_Path != null) {
            if (isAllowMove_Front) {
                float num2 = (5f + buttonInputAccelCharaSpeed + slipStreamAccelCharaSpeed + curveAccelCharaSpeed) * addjustRankSpeed;
                dollyCart.m_Position = Mathf.Clamp(dollyCart.m_Position + num2 * Time.deltaTime, 0f, dollyCart.m_Path.PathLength);
            } else {
                dollyCart.m_Position = Mathf.Clamp(dollyCart.m_Position + frontCollisionSpeed / 4f * Time.deltaTime, 0f, dollyCart.m_Path.PathLength);
            }
            if (GetNowDistancePer() > 0.999f) {
                AddLapCnt();
            }
        }
        if ((double)staminaPoint <= 0.5) {
            outStamina = true;
        } else if (staminaPoint >= 30f) {
            outStamina = false;
        }
        SHORTTRACK.MCM.SetMovePathDistance(playerNum, dollyCart.m_Position);
    }
    public void StartStay() {
        SetAnimation(AnimationType.DashStandby, _isMotion: true);
    }
    public void SetMainCharaStyle(int _userType) {
        charaStyle.SetGameStyle(GS_Define.GameType.MOLE_HAMMER, _userType);
        Reflection.material.SetTexture("_MainTex", ReflectionSprite[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[_userType]]);
    }
    public void SetCharaFacial(StyleTextureManager.FacialType _facialType, bool _isResetFacial = false) {
        charaStyle.SetFacial(_facialType);
        if (_isResetFacial) {
            Invoke("ResetCharaFacial", 0.5f);
        }
    }
    public void ResetCharaFacial() {
        charaStyle.SetFacial(StyleTextureManager.FacialType.DEFAULT);
    }
    private void UpdateEffect() {
        if (SHORTTRACK.MCM.PData[playerNum].isGoal) {
            StopSlipStreamEffect();
            StopMaxSpeedEffect();
            StopSweatEffet();
            return;
        }
        PlayLegIceEffect();
        if (isCurve) {
            slipStreamRotTime = Mathf.Clamp(slipStreamRotTime + Time.deltaTime / 0.1f, 0f, 1f);
            if (base.transform.localPosition.x <= -1.5f) {
                PlayArmIceEffect();
            } else {
                StopArmIceEffect();
            }
        } else {
            slipStreamRotTime = Mathf.Clamp(slipStreamRotTime - Time.deltaTime / 0.1f, 0f, 1f);
            StopArmIceEffect();
            if (GetTotalSpeed() >= 15f) {
                PlayMaxSpeedEffect();
            } else {
                StopMaxSpeedEffect();
            }
        }
        if (isSlipStream) {
            PlaySlipStreamEffect();
        } else {
            StopSlipStreamEffect();
        }
        if (outStamina) {
            PlaySweatEffect();
        } else if (!outStamina) {
            StopSweatEffet();
        }
    }
    private void SlipStreamEffectRot() {
        slipStreamEffect.gameObject.transform.localRotation = Quaternion.Lerp(new Quaternion(1.862645E-08f, 0.9971102f, 0.07596853f, -5.960464E-08f), new Quaternion(-0.2444033f, 0.9524336f, 0.03985435f, 0.1776198f), slipStreamRotTime);
        slipStream.transform.localRotation = slipStreamEffect.transform.localRotation * Quaternion.Euler(0f, 180f, 0f);
        if (currentAnimType == AnimationType.Curve || currentAnimType == AnimationType.TiredCurve) {
            HeadPos.transform.localPosition = Vector3.Lerp(new Vector3(-0.003f, 0.267f, 0.182f), new Vector3(-0.134f, 0.267f, 0.182f), slipStreamRotTime);
        }
        if (!isCurve) {
            HeadPos.transform.localPosition = Vector3.Lerp(new Vector3(-0.003f, 0.267f, 0.182f), new Vector3(-0.134f, 0.267f, 0.182f), slipStreamRotTime);
        }
    }
    public void PlaySlipStreamEffect() {
        slipStream.Play();
    }
    public void StopSlipStreamEffect() {
        slipStream.Stop();
    }
    private void PlayMaxSpeedEffect() {
        if (!MaxSpeed.isPlaying) {
            MaxSpeed.Play();
            concentratedLine.SpeedLineStart();
        }
    }
    private void StopMaxSpeedEffect() {
        if (!MaxSpeed.isStopped) {
            MaxSpeed.Stop();
            concentratedLine.SpeedLineEnd();
        }
    }
    private void PlayLegIceEffect() {
        if (!LegIce.isPlaying) {
            LegIce.Play();
        }
    }
    private void StopLegIceEffect() {
        if (!LegIce.isStopped) {
            LegIce.Stop();
        }
    }
    private void PlayArmIceEffect() {
        if (!ArmIce.isPlaying) {
            ArmIce.Play();
        }
    }
    private void StopArmIceEffect() {
        if (!ArmIce.isStopped) {
            ArmIce.Stop();
        }
    }
    private void PlaySweatEffect() {
        if (!noStaminaSweatEffect.isPlaying) {
            noStaminaSweatEffect.Play();
        }
    }
    private void StopSweatEffet() {
        if (!noStaminaSweatEffect.isStopped) {
            noStaminaSweatEffect.Stop();
        }
    }
    private void UpdateAnimation() {
        if (!isRunnerStart) {
            return;
        }
        if (outStamina) {
            if (isCurve) {
                SetAnimation(AnimationType.TiredCurve);
            } else {
                SetAnimation(AnimationType.Tired);
            }
        } else if (isCurve) {
            if (base.transform.localPosition.x <= -1.5f) {
                SetAnimation(AnimationType.FullCurve);
            } else {
                SetAnimation(AnimationType.Curve);
            }
        } else if (GetTotalSpeed() >= 14.5f) {
            SetAnimation(AnimationType.FullPowerDash);
            if (isPlayer) {
                SingletonCustom<AudioManager>.Instance.SePlay("se_shorttrack_maxspeed");
            }
        } else if (GetTotalSpeed() >= 10f) {
            SetAnimation(AnimationType.Dash);
        } else {
            SetAnimation(AnimationType.SlowDash);
        }
    }
    public void SetAnimation(AnimationType _animType, bool _isMotion = false) {
        if (_animType != currentAnimType) {
            currentAnimType = _animType;
            switch (currentAnimType) {
                case AnimationType.Move:
                    break;
                case AnimationType.Standby:
                    StandbyAnimation(_isMotion);
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
                case AnimationType.SlowDash:
                    SlowDashAnimation(_isMotion);
                    break;
                case AnimationType.Curve:
                    CurveAnimation(_isMotion);
                    break;
                case AnimationType.FullCurve:
                    FullCurveAnimation(_isMotion);
                    break;
                case AnimationType.Tired:
                    TiredAnimation(_isMotion);
                    break;
                case AnimationType.TiredCurve:
                    TiredCurveAnimation(_isMotion);
                    break;
            }
        }
    }
    private void StandbyAnimation(bool _isMotion) {
        ShortTrack_CharaLeanTweenMotionData.MotionData defaultMotionData = ShortTrack_CharaLeanTweenMotionData.GetDefaultMotionData();
        if (!_isMotion) {
            defaultMotionData.motionTime = 0f;
        }
        motion.StartMotion(defaultMotionData);
    }
    private void DashStandbyAnimation(bool _isMotion) {
        ShortTrack_CharaLeanTweenMotionData.MotionData dashStandbyMotionData = ShortTrack_CharaLeanTweenMotionData.GetDashStandbyMotionData();
        if (!_isMotion) {
            dashStandbyMotionData.motionTime = 0f;
        }
        motion.StartMotion(dashStandbyMotionData);
    }
    private void DashAnimation(bool _isMotion) {
        if (currentAnimType == AnimationType.Dash) {
            ShortTrack_CharaLeanTweenMotionData.MotionData dashMotionData = ShortTrack_CharaLeanTweenMotionData.GetDashMotionData();
            if (_isMotion) {
                dashMotionData.motionTime = 0f;
            }
            motion.StartMotion(dashMotionData);
        }
    }
    private void FullPowerDashAnimation(bool _isMotion) {
        if (currentAnimType == AnimationType.FullPowerDash) {
            ShortTrack_CharaLeanTweenMotionData.MotionData fullPowerDashMotionData = ShortTrack_CharaLeanTweenMotionData.GetFullPowerDashMotionData();
            if (_isMotion) {
                fullPowerDashMotionData.motionTime = 0f;
            }
            motion.StartMotion(fullPowerDashMotionData);
        }
    }
    private void SlowDashAnimation(bool _isMotion) {
        if (currentAnimType == AnimationType.SlowDash) {
            ShortTrack_CharaLeanTweenMotionData.MotionData slowDashMotionData = ShortTrack_CharaLeanTweenMotionData.GetSlowDashMotionData();
            if (_isMotion) {
                slowDashMotionData.motionTime = 0f;
            }
            motion.StartMotion(slowDashMotionData);
        }
    }
    private void CurveAnimation(bool _isMotion) {
        if (currentAnimType == AnimationType.Curve) {
            ShortTrack_CharaLeanTweenMotionData.MotionData curveMotionData = ShortTrack_CharaLeanTweenMotionData.GetCurveMotionData();
            if (_isMotion) {
                curveMotionData.motionTime = 0f;
            }
            motion.StartMotion(curveMotionData);
        }
    }
    private void FullCurveAnimation(bool _isMotion) {
        if (currentAnimType == AnimationType.FullCurve) {
            ShortTrack_CharaLeanTweenMotionData.MotionData fullPowerCurveMotionData = ShortTrack_CharaLeanTweenMotionData.GetFullPowerCurveMotionData();
            if (_isMotion) {
                fullPowerCurveMotionData.motionTime = 0f;
            }
            motion.StartMotion(fullPowerCurveMotionData);
        }
    }
    private void TiredAnimation(bool _isMotion) {
        if (currentAnimType == AnimationType.Tired) {
            ShortTrack_CharaLeanTweenMotionData.MotionData tiredMotionData = ShortTrack_CharaLeanTweenMotionData.GetTiredMotionData();
            if (_isMotion) {
                tiredMotionData.motionTime = 0f;
            }
            motion.StartMotion(tiredMotionData);
        }
    }
    private void TiredCurveAnimation(bool _isMotion) {
        if (currentAnimType == AnimationType.TiredCurve) {
            ShortTrack_CharaLeanTweenMotionData.MotionData tiredCurveMotionData = ShortTrack_CharaLeanTweenMotionData.GetTiredCurveMotionData();
            if (_isMotion) {
                tiredCurveMotionData.motionTime = 0f;
            }
            motion.StartMotion(tiredCurveMotionData);
        }
    }
    public void SetDollyCart(CinemachineDollyCart _dollyCart) {
        dollyCart = _dollyCart;
        dollyCart.m_Path = SHORTTRACK.MCM.GetCinemachineSmoothPath();
        dollyCart.m_Position = 0f;
        SHORTTRACK.MCM.SetMovePathDistance(playerNum, dollyCart.m_Position);
    }
    public void AddAccelSpeed() {
        buttonInputAccelCharaSpeed += 0.25f;
        if (buttonInputAccelCharaSpeed > 10f) {
            buttonInputAccelCharaSpeed = 10f;
        }
        currentRemainButtonInputAccelTime = 0f;
    }
    private void SetAddJustRankSpeed() {
        float num = SHORTTRACK.MCM.GetAddJustRankSpped(playerNum) - addjustRankSpeed;
        if (num == 0f) {
            return;
        }
        num = Mathf.Sign(num);
        addjustRankSpeed += num * Time.deltaTime;
        if (num > 0f) {
            if (addjustRankSpeed > SHORTTRACK.MCM.GetAddJustRankSpped(playerNum)) {
                addjustRankSpeed = SHORTTRACK.MCM.GetAddJustRankSpped(playerNum);
            }
        } else if (num < 0f && addjustRankSpeed < SHORTTRACK.MCM.GetAddJustRankSpped(playerNum)) {
            addjustRankSpeed = SHORTTRACK.MCM.GetAddJustRankSpped(playerNum);
        }
    }
    public float GetTotalSpeed() {
        return (5f + buttonInputAccelCharaSpeed + slipStreamAccelCharaSpeed + curveAccelCharaSpeed) * addjustRankSpeed;
    }
    public void SetSlipStreamAccelSpeed(float _accelSpeed) {
        slipStreamAccelCharaSpeed = _accelSpeed;
        if (slipStreamAccelCharaSpeed > 2.5f) {
            slipStreamAccelCharaSpeed = 2.5f;
        }
        currentRemainSlipStreamAccelTime = 0f;
    }
    public void InitMovePathProcess() {
        base.transform.SetLocalEulerAnglesY(0f);
    }
    public void PathMoveStart() {
        SetAnimation(AnimationType.Dash, _isMotion: true);
        isRunnerStart = true;
    }
    public void Move(Vector3 _moveDir) {
        Vector3 vector = _moveDir;
        if ((isAllowMove_Right || !(_moveDir.x > 0f)) && (isAllowMove_Left || !(_moveDir.x < 0f))) {
            vector.y = 0f;
            vector.z = 0f;
            vector.x = vector.x * 3f * Time.deltaTime;
            base.transform.SetLocalPositionX(Mathf.Clamp(base.transform.localPosition.x + vector.x, -3f, 3f));
        }
    }
    private void CheckLimitMove_Front() {
        if (frontCheckCollider.GetIsCollision()) {
            isAllowMove_Front = true;
            return;
        }
        if (isAllowMove_Front) {
            frontCollisionSpeed = (5f + buttonInputAccelCharaSpeed + slipStreamAccelCharaSpeed + curveAccelCharaSpeed) * addjustRankSpeed;
        }
        isAllowMove_Front = false;
    }
    private void CheckLimitMove_Left() {
        isLimitMove_Left = (base.transform.localPosition.x == -3f);
        isAllowMove_Left = leftCheckCollider.GetIsCollision();
    }
    private void CheckLimitMove_Right() {
        isLimitMove_Right = (base.transform.localPosition.x == 3f);
        isAllowMove_Right = rightCheckCollider.GetIsCollision();
    }
    public void AddAccelSpeed_CPU() {
        if (inputAccelSpeedInterval > INPUT_ACCEL_SPEED_INTERVAL_CPU) {
            inputAccelSpeedInterval = 0f;
            if (!outStamina) {
                buttonInputAccelCharaSpeed += ADD_ACCEL_SPEED_CPU;
                CPUStaminaDecrease();
            }
            if (buttonInputAccelCharaSpeed > MAX_ACCEL_SPEED_CPU) {
                buttonInputAccelCharaSpeed = MAX_ACCEL_SPEED_CPU;
            }
            currentRemainButtonInputAccelTime = 0f;
        } else {
            inputAccelSpeedInterval += Time.deltaTime;
            StaminaRecovery();
            if (aiStrength != 0) {
                buttonInputAccelCharaSpeed -= 1f * Time.deltaTime;
            } else {
                buttonInputAccelCharaSpeed -= 2f / 3f * Time.deltaTime;
            }
        }
    }
    public void CPUMove() {
        if (currentCheckMoveIntervalTime > 1f) {
            currentCheckMoveIntervalTime = 0f;
            if (!isAllowMove_Front) {
                if (isAllowMove_Right && !isLimitMove_Right) {
                    moveVec_CPU = new Vector3(1f, 0f, 0f);
                    moveState_CPU = CPU_MOVE_STATE.RIGHT_MOVE;
                }
            } else if (isAllowMove_Left && !isLimitMove_Left) {
                moveVec_CPU = new Vector3(-1f, 0f, 0f);
                moveState_CPU = CPU_MOVE_STATE.LEFT_MOVE;
            }
        } else {
            currentCheckMoveIntervalTime += Time.deltaTime;
        }
        if (moveState_CPU == CPU_MOVE_STATE.LEFT_MOVE && !isAllowMove_Left) {
            moveVec_CPU = Vector3.zero;
        }
        if (moveState_CPU == CPU_MOVE_STATE.RIGHT_MOVE && !isAllowMove_Right) {
            moveVec_CPU = Vector3.zero;
        }
        base.transform.SetLocalPositionX(Mathf.Clamp(base.transform.localPosition.x + moveVec_CPU.x * 3f * Time.deltaTime, -3f, 3f));
    }
    public float GetNowDistancePer() {
        return dollyCart.m_Position / dollyCart.m_Path.PathLength;
    }
    private void AddLapCnt() {
        if (!isWaitAddLapCnt) {
            SHORTTRACK.MCM.AddLapCnt(playerNum);
            isWaitAddLapCnt = true;
            LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate {
                isWaitAddLapCnt = false;
            });
        }
    }
    private void SetIsSlipStream() {
        if (!slipStreamCheckCollider.GetIsCollision()) {
            if (slipSteamUtilTime > 0.25f) {
                isSlipStream = true;
            } else {
                slipSteamUtilTime += Time.deltaTime;
            }
        } else {
            isSlipStream = false;
            slipSteamUtilTime = 0f;
        }
    }
    public void ActiveCursor() {
        if (SHORTTRACK.MCM.IsNowRunnerControlePlayer(this)) {
            cursorSprite.gameObject.SetActive(value: true);
        }
    }
    public void NonActiveCursor() {
        cursorSprite.gameObject.SetActive(value: false);
    }
    private void ChangeCursorSprite() {
        if (SHORTTRACK.MCM.IsNowRunnerControlePlayer(this)) {
            SpriteRenderer spriteRenderer = cursorSprite;
            SAManager instance = SingletonCustom<SAManager>.Instance;
            int userType = SHORTTRACK.MCM.PData[playerNum].userType;
            spriteRenderer.sprite = instance.GetSprite(SAType.Common, "tex_cursor_" + userType.ToString());
        }
    }
    private void UpdateCursorAnimation() {
        if (cursorSprite.gameObject.activeSelf) {
            scaleAniTime += Time.deltaTime * SCALE_ANI_SPEED;
            cursorSprite.transform.localScale = baseScale * (1f - Mathf.Sin(scaleAniTime) * 0.12f);
        }
    }
    public void StaminaRecovery() {
        staminaPoint = Mathf.Clamp(StaminaPoint + 10f * Time.deltaTime, 0f, 100f);
    }
    public void StaminaDecrease() {
        staminaPoint = Mathf.Clamp(StaminaPoint - 2f, 0f, 100f);
    }
    private void CPUStaminaDecrease() {
        switch (aiStrength) {
            case SHORTTRACK.AiStrength.WEAK:
                staminaPoint = Mathf.Clamp(StaminaPoint - 2f, 0f, 100f);
                break;
            case SHORTTRACK.AiStrength.COMMON:
                staminaPoint = Mathf.Clamp(StaminaPoint - 1.6f, 0f, 100f);
                break;
            case SHORTTRACK.AiStrength.STRONG:
                staminaPoint = Mathf.Clamp(StaminaPoint - 1f, 0f, 100f);
                break;
        }
    }
    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("CheckPoint")) {
            return;
        }
        ShortTrack_CurvePoint component = other.gameObject.GetComponent<ShortTrack_CurvePoint>();
        if (component != null) {
            switch (component.GetCurvePointType()) {
                case ShortTrack_CurvePoint.CurvePointType.IN:
                    isCurve = true;
                    break;
                case ShortTrack_CurvePoint.CurvePointType.OUT:
                    isCurve = false;
                    break;
            }
        }
    }
}
