using System;
using System.Collections;
using UnityEngine;
public class SnowBoard_SkiBoard : MonoBehaviour {
    public enum MoveCursorDirection {
        UP,
        RIGHT,
        LEFT,
        DOWN
    }
    public enum SkiBoardProcessType {
        STANDBY,
        SLIDING,
        GOAL
    }
    public enum CharacterPositionType {
        LEFT,
        RIGHT
    }
    public enum CameraPosType {
        NEAR,
        NORMAL,
        DISTANT
    }
    [SerializeField]
    [Header("対象のSnowBoard_Player")]
    public SnowBoard_Player player;
    [SerializeField]
    [Header("キャラオブジェクト位置移動用(Y軸回転)")]
    public GameObject CharacterAnchor_Yrot;
    [SerializeField]
    [Header("キャラオブジェクト(X軸回転)")]
    public GameObject characterAnchor_Xrot;
    [SerializeField]
    [Header("キャラオブジェクト(Z軸回転)")]
    public GameObject characterAnchor_Zrot;
    [SerializeField]
    [Header("キャラオブジェクト(回転アクション)")]
    public GameObject characterAnchor_Action;
    [SerializeField]
    [Header("キャラオブジェクト")]
    public GameObject characterObj;
    [SerializeField]
    [Header("搭乗キャラの管理スクリプト")]
    private SnowBoard_Character character;
    [SerializeField]
    [Header("移動時エフェクト(左向き)")]
    private ParticleSystem psMoveSmokeLeft;
    [SerializeField]
    [Header("移動時エフェクト(右向き)")]
    private ParticleSystem psMoveSmokeRight;
    [SerializeField]
    [Header("移動時エフェクトの生成量")]
    private float psMoveSmokeRate = 1f;
    [SerializeField]
    [Header("移動時トレイルエフェクト(前)")]
    private TrailRenderer trMoveFront;
    [SerializeField]
    [Header("移動時トレイルエフェクト(後)")]
    private TrailRenderer trMoveBack;
    [SerializeField]
    [Header("ドリフトエフェクト（左向き_左回転）")]
    private ParticleSystem psDriftLeft_LeftTurn;
    [SerializeField]
    [Header("ドリフトエフェクト（左向き_右回転）")]
    private ParticleSystem psDriftLeft_RightTurn;
    [SerializeField]
    [Header("ドリフトエフェクト（右向き_左回転）")]
    private ParticleSystem psDriftRight_LeftTurn;
    [SerializeField]
    [Header("ドリフトエフェクト（右向き_右回転）")]
    private ParticleSystem psDriftRight_RightTurn;
    [SerializeField]
    [Header("トリックエフェクト（縦回転_前）")]
    private ParticleSystem psTrickZrotFront;
    [SerializeField]
    [Header("トリックエフェクト（縦回転_後）")]
    private ParticleSystem psTrickZrotBack;
    [SerializeField]
    [Header("トリックエフェクト（横回転_前）")]
    private ParticleSystem psTrickYrotFront;
    [SerializeField]
    [Header("トリックエフェクト（横回転_後）")]
    private ParticleSystem psTrickYrotBack;
    [SerializeField]
    [Header("レ\u30fcルアクションエフェクト")]
    private ParticleSystem psRailAction;
    [SerializeField]
    [Header("加速エフェクト渦")]
    private ParticleSystem psSpeedEffectWhirl;
    [SerializeField]
    [Header("加速度")]
    public float acceleration = 2f;
    [SerializeField]
    [Header("方向の変えやすさ")]
    public float directionChangeSpeed = 12f;
    [SerializeField]
    [Header("ドリフトになる最小の角度")]
    public float driftAngle = 25f;
    [SerializeField]
    [Header("SnowBoard_Camera")]
    private SnowBoard_Camera cameraEffect;
    [SerializeField]
    [Header("SnowBoard_Animation")]
    private SnowBoard_Animation anime;
    [SerializeField]
    [Header("プレイヤ\u30fc番号")]
    private int playerNo;
    private Rigidbody rb;
    private Vector3 moveVector;
    private Vector3 localMoveVector;
    private bool isDriftInput;
    private bool isGround;
    private float driftTime;
    private Vector3 driftDir;
    private float addOnSpeed;
    private float addAccelSpeed;
    private SnowBoard_Define.UserType userType;
    public SkiBoardProcessType processType;
    public CharacterPositionType positionType;
    private float speed;
    private bool direction;
    private bool isInputKeep;
    private MoveCursorDirection beforeInput;
    private WaitForSeconds inputKeeptTime = new WaitForSeconds(0.5f);
    private bool isAdvanceZone;
    private bool isJumpStandby;
    private bool isJumpInput;
    private bool isJumpAction;
    public int jumpActionNum;
    private bool isRail;
    private Transform railTarget;
    private bool isRaidStandby;
    private bool isRaid;
    private float accelGauge;
    private bool isAccel;
    private bool isAccelEnd;
    private bool isTurn;
    private float turnTime;
    private bool isNoAction;
    private int id;
    private CameraPosType cameraPos;
    public int PlayerNo => playerNo;
    public bool IsGround => isGround;
    public bool IsRail => isRail;
    public float AccelGauge => accelGauge;
    public bool IsTurn => isTurn;
    public CameraPosType CameraPos => cameraPos;
    public void PlayerInit(SnowBoard_Player _player) {
        userType = _player.UserType;
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        ProcessTypeChange(SkiBoardProcessType.STANDBY);
        positionType = CharacterPositionType.RIGHT;
        anime.SetAnim(SnowBoard_Animation.AnimType.SLIDING_RIGHT);
        if (cameraEffect != null) {
            cameraEffect.Init();
        }
        cameraPos = CameraPosType.NORMAL;
    }
    public void StartMethod() {
        ProcessTypeChange(SkiBoardProcessType.SLIDING);
        anime.SetAnim(SnowBoard_Animation.AnimType.SLIDING_RIGHT);
        isGround = true;
        trMoveFront.emitting = true;
        trMoveBack.emitting = true;
    }
    public void UpdateMethod() {
        if (!isInputKeep && beforeInput != 0) {
            beforeInput = MoveCursorDirection.UP;
            LeanTween.cancel(characterAnchor_Zrot);
            LeanTween.rotateZ(characterAnchor_Zrot, 0f, 0.5f);
        }
        if (isGround && !isJumpStandby && !isJumpInput && !isRail && Vector3.Angle(moveVector, characterAnchor_Xrot.transform.forward) > 90f) {
            if (positionType == CharacterPositionType.LEFT) {
                PositionTypeChange(CharacterPositionType.RIGHT);
            } else if (positionType == CharacterPositionType.RIGHT) {
                PositionTypeChange(CharacterPositionType.LEFT);
            }
        }
    }
    private void FixedUpdate() {
        CharacterAnchor_Yrot.transform.position = base.transform.position;
        if (processType == SkiBoardProcessType.SLIDING) {
            if (isGround) {
                if (!character.isForward && isAdvanceZone) {
                    rb.AddForce(-character.angleAcc * acceleration);
                } else {
                    rb.AddForce(character.angleAcc * acceleration);
                }
                if (isAdvanceZone) {
                    rb.AddForce(CharacterAnchor_Yrot.transform.up * 7f);
                }
                moveVector = rb.velocity.normalized;
                if (Vector3.Angle(characterAnchor_Xrot.transform.forward, moveVector) >= driftAngle) {
                    if (Vector3.Cross(moveVector, characterAnchor_Xrot.transform.forward).y < 0f) {
                        direction = true;
                    } else {
                        direction = false;
                    }
                    SnowBoard_Define.UserType userType2 = player.UserType;
                    if (!isDriftInput) {
                        driftDir = moveVector;
                        isDriftInput = true;
                    }
                    if (rb.velocity.sqrMagnitude >= Mathf.Pow(5f + addOnSpeed + addAccelSpeed, 2f)) {
                        while (rb.velocity.sqrMagnitude >= Mathf.Pow(5f + addOnSpeed + addAccelSpeed, 2f)) {
                            rb.velocity *= 0.98f;
                        }
                    }
                    if (character.isForward || isAdvanceZone) {
                        rb.AddForce(characterAnchor_Xrot.transform.forward * (rb.velocity.sqrMagnitude * Mathf.Clamp(driftTime * driftTime, 0f, 2f)) * 0.7f);
                    }
                    driftTime = Mathf.Clamp(driftTime + Time.fixedDeltaTime, 0f, 3f);
                    if (character.isForward || isTurn) {
                        rb.AddForce(characterAnchor_Xrot.transform.forward * rb.velocity.sqrMagnitude * 0.5f);
                    }
                    if (isTurn && !isRail && rb.velocity.sqrMagnitude >= 2.05f) {
                        if (positionType == CharacterPositionType.LEFT) {
                            if (direction) {
                                psDriftLeft_RightTurn.Emit(5);
                            } else {
                                psDriftLeft_LeftTurn.Emit(5);
                            }
                        } else if (direction) {
                            psDriftRight_RightTurn.Emit(5);
                        } else {
                            psDriftRight_LeftTurn.Emit(5);
                        }
                    }
                } else {
                    driftTime = 0f;
                    isDriftInput = false;
                }
                if (isRail) {
                    rb.velocity = (railTarget.position - base.gameObject.transform.position).normalized * rb.velocity.magnitude * 1.2f;
                    if (trMoveFront.emitting) {
                        trMoveFront.emitting = false;
                    }
                    if (trMoveBack.emitting) {
                        trMoveBack.emitting = false;
                    }
                    if (psMoveSmokeRight.isPlaying) {
                        psMoveSmokeRight.Stop();
                    }
                    if (psMoveSmokeLeft.isPlaying) {
                        psMoveSmokeLeft.Stop();
                    }
                    if (!isAccel && !isAccelEnd) {
                        accelGauge = Mathf.Clamp(accelGauge + Time.fixedDeltaTime * 0.5f, 0f, 1f);
                    }
                    if (SnowBoard_Define.GM.IsDuringGame() && player.UserType <= SnowBoard_Define.UserType.PLAYER_4 && !SingletonCustom<AudioManager>.Instance.IsSePlaying("se_alpineskiing_slide_2")) {
                        SingletonCustom<AudioManager>.Instance.SePlay("se_alpineskiing_slide_2", _loop: true);
                    }
                } else {
                    if (!isAccel && !isAccelEnd) {
                        accelGauge = Mathf.Clamp(accelGauge + Time.fixedDeltaTime * 0.1f, 0f, 1f);
                    }
                    if (!trMoveFront.emitting) {
                        trMoveFront.emitting = true;
                    }
                    if (!trMoveBack.emitting) {
                        trMoveBack.emitting = true;
                    }
                    if (positionType == CharacterPositionType.LEFT) {
                        //??psMoveSmokeLeft.emission.rateOverTime = Mathf.Clamp(rb.velocity.sqrMagnitude * psMoveSmokeRate, 0f, 180f);
                        if (psMoveSmokeRight.isPlaying) {
                            psMoveSmokeRight.Stop();
                        }
                        if (psMoveSmokeLeft.isStopped) {
                            psMoveSmokeLeft.Play();
                        }
                        anime.SetAnim(SnowBoard_Animation.AnimType.SLIDING_LEFT);
                    } else {
                        //??psMoveSmokeRight.emission.rateOverTime = Mathf.Clamp(rb.velocity.sqrMagnitude * psMoveSmokeRate, 0f, 180f);
                        if (psMoveSmokeLeft.isPlaying) {
                            psMoveSmokeLeft.Stop();
                        }
                        if (psMoveSmokeRight.isStopped) {
                            psMoveSmokeRight.Play();
                        }
                        anime.SetAnim(SnowBoard_Animation.AnimType.SLIDING_RIGHT);
                    }
                }
                if (rb.velocity.sqrMagnitude >= Mathf.Pow(5f + addOnSpeed + addAccelSpeed, 2f)) {
                    while (rb.velocity.sqrMagnitude >= Mathf.Pow(5f + addOnSpeed + addAccelSpeed, 2f)) {
                        rb.velocity *= 0.98f;
                    }
                }
                if (SnowBoard_Define.GM.IsDuringGame() && player.UserType <= SnowBoard_Define.UserType.PLAYER_4 && !SingletonCustom<AudioManager>.Instance.IsSePlaying("se_alpineskiing_slide")) {
                    SingletonCustom<AudioManager>.Instance.SePlay("se_alpineskiing_slide", _loop: true);
                }
                if (SnowBoard_Define.PM.CheckPlayerIsGround() && SingletonCustom<AudioManager>.Instance.IsSePlaying("se_snowboard_air")) {
                    SingletonCustom<AudioManager>.Instance.SeStop("se_snowboard_air");
                }
            } else {
                if (psMoveSmokeRight.isPlaying) {
                    psMoveSmokeRight.Stop();
                }
                if (psMoveSmokeLeft.isPlaying) {
                    psMoveSmokeLeft.Stop();
                }
                if (isJumpInput) {
                    rb.AddForce(CharacterAnchor_Yrot.transform.up * 6f);
                }
                if (!isAccel && isJumpAction && !isAccelEnd) {
                    accelGauge = Mathf.Clamp(accelGauge + Time.fixedDeltaTime * 0.5f, 0f, 1f);
                }
                if (rb.velocity.sqrMagnitude >= Mathf.Pow(7f + addOnSpeed + addAccelSpeed, 2f)) {
                    while (rb.velocity.sqrMagnitude >= Mathf.Pow(7f + addOnSpeed + addAccelSpeed, 2f)) {
                        rb.velocity *= 0.98f;
                    }
                }
                if (!SnowBoard_Define.PM.CheckPlayerIsGround() && SingletonCustom<AudioManager>.Instance.IsSePlaying("se_alpineskiing_slide")) {
                    SingletonCustom<AudioManager>.Instance.SeStop("se_alpineskiing_slide");
                }
                if (SnowBoard_Define.GM.IsDuringGame() && player.UserType <= SnowBoard_Define.UserType.PLAYER_4 && !SingletonCustom<AudioManager>.Instance.IsSePlaying("se_snowboard_air")) {
                    SingletonCustom<AudioManager>.Instance.SePlay("se_snowboard_air", _loop: true);
                }
            }
            if (addOnSpeed > 0f) {
                addOnSpeed -= Time.deltaTime * 1.5f;
            }
            addOnSpeed = Mathf.Clamp(addOnSpeed, 0f, 3.5f);
            if (addAccelSpeed > 0f) {
                addAccelSpeed -= Time.deltaTime * 2f;
                addAccelSpeed = Mathf.Clamp(addAccelSpeed, 0f, 3f);
            }
            if (isAccel) {
                addAccelSpeed = 3f;
            }
        } else if (processType == SkiBoardProcessType.GOAL) {
            if (psMoveSmokeRight.isPlaying) {
                psMoveSmokeRight.Stop();
            }
            if (psMoveSmokeLeft.isPlaying) {
                psMoveSmokeLeft.Stop();
            }
            if (rb.velocity.sqrMagnitude > 0.01f) {
                rb.velocity *= 0.99f;
            } else {
                rb.velocity *= 0f;
            }
        } else if (processType == SkiBoardProcessType.STANDBY) {
            if (psMoveSmokeRight.isPlaying) {
                psMoveSmokeRight.Stop();
            }
            if (psMoveSmokeLeft.isPlaying) {
                psMoveSmokeLeft.Stop();
            }
            rb.velocity *= 0f;
        }
    }
    public void MoveCursor(MoveCursorDirection _dir, float _input) {
        switch (_dir) {
            case MoveCursorDirection.UP:
                jumpActionNum = 4;
                if (!isGround && !isJumpAction && !isRail && !isNoAction && isJumpStandby) {
                    JumpAction();
                }
                break;
            case MoveCursorDirection.RIGHT:
                jumpActionNum = 0;
                if (!isGround && !isJumpAction && !isRail && !isNoAction && isJumpStandby) {
                    JumpAction();
                }
                CurveMove(_input);
                if (beforeInput != MoveCursorDirection.RIGHT) {
                    LeanTween.cancel(characterAnchor_Zrot);
                    if (isTurn) {
                        LeanTween.rotateZ(characterAnchor_Zrot, -25f, 0.5f);
                    } else {
                        LeanTween.rotateZ(characterAnchor_Zrot, -15f, 1f);
                    }
                    isInputKeep = true;
                    beforeInput = MoveCursorDirection.RIGHT;
                }
                StopCoroutine(isInputKeepWait());
                StartCoroutine(isInputKeepWait());
                break;
            case MoveCursorDirection.LEFT:
                jumpActionNum = 1;
                if (!isGround && !isJumpAction && !isRail && !isNoAction && isJumpStandby) {
                    JumpAction();
                }
                CurveMove(_input);
                if (beforeInput != MoveCursorDirection.LEFT) {
                    LeanTween.cancel(characterAnchor_Zrot);
                    if (isTurn) {
                        LeanTween.rotateZ(characterAnchor_Zrot, 25f, 0.5f);
                    } else {
                        LeanTween.rotateZ(characterAnchor_Zrot, 15f, 1f);
                    }
                    isInputKeep = true;
                    beforeInput = MoveCursorDirection.LEFT;
                }
                StopCoroutine(isInputKeepWait());
                StartCoroutine(isInputKeepWait());
                break;
            case MoveCursorDirection.DOWN:
                jumpActionNum = 5;
                if (!isGround && !isJumpAction && !isRail && !isNoAction && isJumpStandby) {
                    JumpAction();
                }
                break;
        }
    }
    public void ActionInput() {
        if (processType == SkiBoardProcessType.SLIDING && isGround && !isRail && !isRaidStandby && !isAccel && accelGauge > 0f) {
            Speedup();
        }
    }
    public void ProcessTypeChange(SkiBoardProcessType _type) {
        switch (_type) {
            case SkiBoardProcessType.STANDBY:
                processType = SkiBoardProcessType.STANDBY;
                break;
            case SkiBoardProcessType.SLIDING:
                if (processType == SkiBoardProcessType.STANDBY) {
                    processType = SkiBoardProcessType.SLIDING;
                }
                break;
            case SkiBoardProcessType.GOAL:
                if (cameraEffect != null) {
                    cameraEffect.RadialBlur(active: false);
                }
                processType = SkiBoardProcessType.GOAL;
                break;
        }
    }
    public void PositionTypeChange(CharacterPositionType _type) {
        switch (_type) {
            case CharacterPositionType.LEFT:
                positionType = CharacterPositionType.LEFT;
                anime.SetAnim(SnowBoard_Animation.AnimType.SLIDING_LEFT);
                CharacterAnchor_Yrot.transform.rotation *= Quaternion.Euler(0f, 180f, 0f);
                characterObj.transform.rotation *= Quaternion.Euler(0f, 180f, 0f);
                break;
            case CharacterPositionType.RIGHT:
                positionType = CharacterPositionType.RIGHT;
                anime.SetAnim(SnowBoard_Animation.AnimType.SLIDING_RIGHT);
                CharacterAnchor_Yrot.transform.rotation *= Quaternion.Euler(0f, 180f, 0f);
                characterObj.transform.rotation *= Quaternion.Euler(0f, 180f, 0f);
                break;
        }
    }
    public void CameraPosTypeChange(bool _set) {
        if (_set) {
            switch (cameraPos) {
                case CameraPosType.NEAR:
                    cameraPos = CameraPosType.NORMAL;
                    break;
                case CameraPosType.NORMAL:
                    cameraPos = CameraPosType.DISTANT;
                    break;
                case CameraPosType.DISTANT:
                    cameraPos = CameraPosType.NEAR;
                    break;
            }
        } else {
            switch (cameraPos) {
                case CameraPosType.NEAR:
                    cameraPos = CameraPosType.DISTANT;
                    break;
                case CameraPosType.NORMAL:
                    cameraPos = CameraPosType.NEAR;
                    break;
                case CameraPosType.DISTANT:
                    cameraPos = CameraPosType.NORMAL;
                    break;
            }
        }
    }
    public void IsTurnChange(bool _set) {
        if (_set) {
            isTurn = true;
        } else {
            isTurn = false;
        }
    }
    private void CurveMove(float _input) {
        if (processType != SkiBoardProcessType.SLIDING) {
            return;
        }
        if (isGround) {
            if (isTurn) {
                CharacterAnchor_Yrot.transform.forward = Vector3.Lerp(CharacterAnchor_Yrot.transform.forward, Quaternion.Euler(0f, _input * directionChangeSpeed * 1.5f * Time.deltaTime, 0f) * CharacterAnchor_Yrot.transform.forward, 0.1f);
            } else {
                CharacterAnchor_Yrot.transform.forward = Vector3.Lerp(CharacterAnchor_Yrot.transform.forward, Quaternion.Euler(0f, _input * directionChangeSpeed * Time.deltaTime, 0f) * CharacterAnchor_Yrot.transform.forward, 0.1f);
            }
        } else {
            CharacterAnchor_Yrot.transform.forward = Vector3.Lerp(CharacterAnchor_Yrot.transform.forward, Quaternion.Euler(0f, _input * directionChangeSpeed * 0.5f * Time.deltaTime, 0f) * CharacterAnchor_Yrot.transform.forward, 0.1f);
        }
    }
    private void Speedup() {
        isAccel = true;
        addAccelSpeed = 3f;
        rb.velocity *= 1f + accelGauge;
        id = LeanTween.value(base.gameObject, accelGauge, 0f, accelGauge * 2f).setOnUpdate(delegate (float val) {
            accelGauge = val;
        }).setOnComplete((Action)delegate {
            isAccel = false;
            isAccelEnd = true;
            StartCoroutine(IsAccelEndWait());
            if (cameraEffect != null) {
                cameraEffect.RadialBlur(active: false);
            }
        })
            .id;
        if (cameraEffect != null) {
            cameraEffect.RadialBlur(active: true);
        }
        if (SnowBoard_Define.GM.IsDuringGame() && player.UserType <= SnowBoard_Define.UserType.PLAYER_4) {
            SingletonCustom<AudioManager>.Instance.SePlay("se_alpineskiing_accel", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
            SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)userType, HidVibration.VibrationType.Strong, accelGauge * 2f);
        }
    }
    private void JumpAction() {
        UnityEngine.Debug.Log("ジャンプアクション実行");
        isJumpAction = true;
        switch (jumpActionNum) {
            case 0:
                if (isAccel) {
                    LeanTween.rotateAroundLocal(characterAnchor_Action, Vector3.up, 720f, 1f).setEase(LeanTweenType.easeOutQuad);
                    if (direction) {
                        if (player.UserType <= SnowBoard_Define.UserType.PLAYER_4) {
                            SnowBoard_Define.UIM.TrickUICall((int)player.UserType, SnowBoard_UserUIData.TrickType.Frontside720);
                        }
                    } else if (player.UserType <= SnowBoard_Define.UserType.PLAYER_4) {
                        SnowBoard_Define.UIM.TrickUICall((int)player.UserType, SnowBoard_UserUIData.TrickType.Backside720);
                    }
                    if (SnowBoard_Define.GM.IsDuringGame() && player.UserType <= SnowBoard_Define.UserType.PLAYER_4) {
                        StartCoroutine(DoubleVibration());
                    }
                } else {
                    LeanTween.rotateAroundLocal(characterAnchor_Action, Vector3.up, 360f, 1f).setEase(LeanTweenType.easeOutQuad);
                    if (direction) {
                        if (player.UserType <= SnowBoard_Define.UserType.PLAYER_4) {
                            SnowBoard_Define.UIM.TrickUICall((int)player.UserType, SnowBoard_UserUIData.TrickType.Frontside360);
                        }
                    } else if (player.UserType <= SnowBoard_Define.UserType.PLAYER_4) {
                        SnowBoard_Define.UIM.TrickUICall((int)player.UserType, SnowBoard_UserUIData.TrickType.Backside360);
                    }
                    if (SnowBoard_Define.GM.IsDuringGame() && player.UserType <= SnowBoard_Define.UserType.PLAYER_4) {
                        SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)userType, HidVibration.VibrationType.Common, 0.5f);
                    }
                }
                psTrickYrotFront.Play();
                psTrickYrotBack.Play();
                break;
            case 1:
                if (isAccel) {
                    LeanTween.rotateAroundLocal(characterAnchor_Action, Vector3.up, -720f, 1f).setEase(LeanTweenType.easeOutQuad);
                    if (direction) {
                        if (player.UserType <= SnowBoard_Define.UserType.PLAYER_4) {
                            SnowBoard_Define.UIM.TrickUICall((int)player.UserType, SnowBoard_UserUIData.TrickType.Backside720);
                        }
                    } else if (player.UserType <= SnowBoard_Define.UserType.PLAYER_4) {
                        SnowBoard_Define.UIM.TrickUICall((int)player.UserType, SnowBoard_UserUIData.TrickType.Frontside720);
                    }
                    if (SnowBoard_Define.GM.IsDuringGame() && player.UserType <= SnowBoard_Define.UserType.PLAYER_4) {
                        StartCoroutine(DoubleVibration());
                    }
                } else {
                    LeanTween.rotateAroundLocal(characterAnchor_Action, Vector3.up, -360f, 1f).setEase(LeanTweenType.easeOutQuad);
                    if (direction) {
                        if (player.UserType <= SnowBoard_Define.UserType.PLAYER_4) {
                            SnowBoard_Define.UIM.TrickUICall((int)player.UserType, SnowBoard_UserUIData.TrickType.Backside360);
                        }
                    } else if (player.UserType <= SnowBoard_Define.UserType.PLAYER_4) {
                        SnowBoard_Define.UIM.TrickUICall((int)player.UserType, SnowBoard_UserUIData.TrickType.Frontside360);
                    }
                    if (SnowBoard_Define.GM.IsDuringGame() && player.UserType <= SnowBoard_Define.UserType.PLAYER_4) {
                        SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)userType, HidVibration.VibrationType.Common, 0.5f);
                    }
                }
                psTrickYrotFront.Play();
                psTrickYrotBack.Play();
                break;
            case 2:
                LeanTween.rotateAroundLocal(characterAnchor_Action, Vector3.forward, 720f, 1f).setEase(LeanTweenType.easeOutQuad);
                break;
            case 3:
                LeanTween.rotateAroundLocal(characterAnchor_Action, Vector3.forward, -360f, 1f).setEase(LeanTweenType.easeOutQuad);
                break;
            case 4:
                if (isAccel) {
                    LeanTween.rotateAroundLocal(characterAnchor_Action, Vector3.right, 720f, 1f).setEase(LeanTweenType.easeOutQuad);
                    if (player.UserType <= SnowBoard_Define.UserType.PLAYER_4) {
                        SnowBoard_Define.UIM.TrickUICall((int)player.UserType, SnowBoard_UserUIData.TrickType.WFrontflip);
                    }
                    if (SnowBoard_Define.GM.IsDuringGame() && player.UserType <= SnowBoard_Define.UserType.PLAYER_4) {
                        StartCoroutine(DoubleVibration());
                    }
                } else {
                    LeanTween.rotateAroundLocal(characterAnchor_Action, Vector3.right, 360f, 1f).setEase(LeanTweenType.easeOutQuad);
                    if (player.UserType <= SnowBoard_Define.UserType.PLAYER_4) {
                        SnowBoard_Define.UIM.TrickUICall((int)player.UserType, SnowBoard_UserUIData.TrickType.Frontflip);
                    }
                    if (SnowBoard_Define.GM.IsDuringGame() && player.UserType <= SnowBoard_Define.UserType.PLAYER_4) {
                        SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)userType, HidVibration.VibrationType.Common, 0.5f);
                    }
                }
                psTrickZrotFront.Play();
                psTrickZrotBack.Play();
                break;
            case 5:
                if (isAccel) {
                    LeanTween.rotateAroundLocal(characterAnchor_Action, Vector3.right, -720f, 1f).setEase(LeanTweenType.easeOutQuad);
                    if (player.UserType <= SnowBoard_Define.UserType.PLAYER_4) {
                        SnowBoard_Define.UIM.TrickUICall((int)player.UserType, SnowBoard_UserUIData.TrickType.WBackflip);
                    }
                    if (SnowBoard_Define.GM.IsDuringGame() && player.UserType <= SnowBoard_Define.UserType.PLAYER_4) {
                        StartCoroutine(DoubleVibration());
                    }
                } else {
                    LeanTween.rotateAroundLocal(characterAnchor_Action, Vector3.right, -360f, 1f).setEase(LeanTweenType.easeOutQuad);
                    if (player.UserType <= SnowBoard_Define.UserType.PLAYER_4) {
                        SnowBoard_Define.UIM.TrickUICall((int)player.UserType, SnowBoard_UserUIData.TrickType.Backflip);
                    }
                    if (SnowBoard_Define.GM.IsDuringGame() && player.UserType <= SnowBoard_Define.UserType.PLAYER_4) {
                        SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)userType, HidVibration.VibrationType.Common, 0.5f);
                    }
                }
                psTrickZrotFront.Play();
                psTrickZrotBack.Play();
                break;
        }
        LeanTween.rotateLocal(characterAnchor_Action, new Vector3(0f, 0f, 0f), 0.2f).setDelay(1f).setOnComplete((Action)delegate {
            isJumpAction = false;
        });
        if (SnowBoard_Define.GM.IsDuringGame() && player.UserType <= SnowBoard_Define.UserType.PLAYER_4) {
            SingletonCustom<AudioManager>.Instance.SePlay("se_snowboard_trick", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
        }
        if (isAccel) {
            isAccel = false;
            LeanTween.cancel(id);
            if (cameraEffect != null) {
                cameraEffect.RadialBlur(active: false);
            }
        }
        isAccelEnd = false;
    }
    private void RailAction(bool _set) {
        LeanTween.cancel(characterAnchor_Action);
        if (isJumpAction) {
            isJumpAction = false;
            LeanTween.rotateLocal(characterAnchor_Action, new Vector3(0f, 0f, 0f), 0.1f);
        }
        if (_set) {
            switch (positionType) {
                case CharacterPositionType.LEFT:
                    LeanTween.rotateY(characterAnchor_Action, 90f, 0.5f).setDelay(0.1f);
                    break;
                case CharacterPositionType.RIGHT:
                    LeanTween.rotateY(characterAnchor_Action, -90f, 0.5f).setDelay(0.1f);
                    break;
            }
        } else {
            LeanTween.rotateLocal(characterAnchor_Action, new Vector3(0f, 0f, 0f), 0.5f);
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (processType != SkiBoardProcessType.SLIDING) {
            return;
        }
        if (other.name.Contains("GoalChecker")) {
            ProcessTypeChange(SkiBoardProcessType.GOAL);
            LeanTween.cancel(characterAnchor_Zrot);
            LeanTween.rotateZ(characterAnchor_Zrot, 0f, 0.5f);
            int num = 0;
            for (int i = 0; i < SnowBoard_Define.PM.UserData_Group1.Length; i++) {
                if (SnowBoard_Define.PM.UserData_Group1[playerNo].goalTime > SnowBoard_Define.PM.UserData_Group1[i].goalTime) {
                    num++;
                }
            }
            if (SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType == GameSettingManager.GameProgressType.SINGLE && SnowBoard_Define.GM.IsEightRun) {
                switch (num) {
                    case 0:
                    case 1:
                        player.characterStyle.SetMainCharacterFaceDiff((int)userType, StyleTextureManager.MainCharacterFaceType.HAPPY);
                        anime.SetAnim(SnowBoard_Animation.AnimType.WINNER);
                        break;
                    case 2:
                    case 3:
                        player.characterStyle.SetMainCharacterFaceDiff((int)userType, StyleTextureManager.MainCharacterFaceType.SMILE);
                        anime.SetAnim(SnowBoard_Animation.AnimType.JOY);
                        break;
                    case 4:
                    case 5:
                        player.characterStyle.SetMainCharacterFaceDiff((int)userType, StyleTextureManager.MainCharacterFaceType.NORMAL);
                        anime.SetAnim(SnowBoard_Animation.AnimType.FIGHT);
                        break;
                    case 6:
                    case 7:
                        player.characterStyle.SetMainCharacterFaceDiff((int)userType, StyleTextureManager.MainCharacterFaceType.SAD);
                        anime.SetAnim(SnowBoard_Animation.AnimType.SAD);
                        break;
                }
            } else {
                switch (num) {
                    case 0:
                        player.characterStyle.SetMainCharacterFaceDiff((int)userType, StyleTextureManager.MainCharacterFaceType.HAPPY);
                        anime.SetAnim(SnowBoard_Animation.AnimType.WINNER);
                        break;
                    case 1:
                        player.characterStyle.SetMainCharacterFaceDiff((int)userType, StyleTextureManager.MainCharacterFaceType.SMILE);
                        anime.SetAnim(SnowBoard_Animation.AnimType.JOY);
                        break;
                    case 2:
                        player.characterStyle.SetMainCharacterFaceDiff((int)userType, StyleTextureManager.MainCharacterFaceType.NORMAL);
                        anime.SetAnim(SnowBoard_Animation.AnimType.FIGHT);
                        break;
                    case 3:
                        player.characterStyle.SetMainCharacterFaceDiff((int)userType, StyleTextureManager.MainCharacterFaceType.SAD);
                        anime.SetAnim(SnowBoard_Animation.AnimType.SAD);
                        break;
                }
            }
            if (!SnowBoard_Define.GM.IsGameEnd()) {
                if (player.UserType <= SnowBoard_Define.UserType.PLAYER_4) {
                    SingletonCustom<AudioManager>.Instance.SePlay("se_cracker", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
                    if (SnowBoard_Define.PM.PlayerOnlyOneCheck()) {
                        SnowBoard_Define.GM.StartForcedGoal();
                    }
                }
                SnowBoard_Define.PM.SetGoalTime(userType, SnowBoard_Define.GM.GetGameTime());
                if (SnowBoard_Define.PM.PlayerGoalCheck()) {
                    SnowBoard_Define.GM.GameEnd();
                }
            }
        }
        if (other.name.Contains("Rail")) {
            railTarget = other.gameObject.transform;
        }
    }
    private void OnTriggerStay(Collider other) {
        if (other.name.Contains("AdvanceZone") && !isAdvanceZone) {
            isAdvanceZone = true;
        }
        if (other.name.Contains("Jump")) {
            if (!isJumpStandby) {
                isJumpStandby = true;
                character.isAngleUpdate = false;
                trMoveFront.emitting = false;
                trMoveBack.emitting = false;
            }
            if (isJumpStandby && !isJumpInput) {
                UnityEngine.Debug.Log("ジャンプ入力");
                isJumpInput = true;
            }
            trMoveFront.emitting = false;
            trMoveBack.emitting = false;
        }
        if (other.name.Contains("Rail")) {
            isRail = true;
            RailAction(_set: true);
        }
        if (!other.name.Contains("RidePoint")) {
            return;
        }
        if (!isRaidStandby) {
            isRaidStandby = true;
        }
        if (!isRaid) {
            isRaid = true;
            if (player.UserType <= SnowBoard_Define.UserType.PLAYER_4) {
                SnowBoard_Define.UIM.TrickUICall((int)player.UserType, SnowBoard_UserUIData.TrickType.Rail);
            }
            if (isAccel) {
                isAccel = false;
                LeanTween.cancel(id);
                if (cameraEffect != null) {
                    cameraEffect.RadialBlur(active: false);
                }
            }
            isAccelEnd = false;
        }
        if (isRaid) {
            if (!isRail) {
                railTarget = other.gameObject.transform;
                isRail = true;
            }
            trMoveFront.emitting = false;
            trMoveBack.emitting = false;
            psRailAction.Play();
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.name.Contains("AdvanceZone") && isAdvanceZone) {
            isAdvanceZone = false;
        }
        if (other.name.Contains("Jump") && isJumpStandby) {
            if (isJumpInput) {
                isJumpInput = false;
            } else {
                isJumpStandby = false;
                character.isAngleUpdate = true;
            }
        }
        if (other.name.Contains("Rail")) {
            if (isRail) {
                isRail = false;
            }
            RailAction(_set: false);
            trMoveFront.emitting = true;
            trMoveBack.emitting = true;
            psRailAction.Stop();
            if (!SnowBoard_Define.PM.CheckPlayerIsRail() && SingletonCustom<AudioManager>.Instance.IsSePlaying("se_alpineskiing_slide_2")) {
                SingletonCustom<AudioManager>.Instance.SeStop("se_alpineskiing_slide_2");
            }
        }
        if (other.name.Contains("RidePoint")) {
            if (isRaidStandby) {
                isRaidStandby = false;
            }
            if (isRaid) {
                isRaid = false;
            }
            if (isRail) {
                isRail = false;
            }
        }
    }
    private void OnCollisionStay(Collision collision) {
        if (!isGround && collision.gameObject.layer == LayerMask.NameToLayer("Field")) {
            isGround = true;
            isJumpInput = false;
            isJumpStandby = false;
            character.isAngleUpdate = true;
            driftTime = 0f;
            isDriftInput = false;
            trMoveFront.emitting = true;
            trMoveBack.emitting = true;
            if (SnowBoard_Define.GM.IsDuringGame() && player.UserType <= SnowBoard_Define.UserType.PLAYER_4) {
                SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)userType, HidVibration.VibrationType.Strong);
            }
        }
    }
    private void OnCollisionExit(Collision collision) {
        if (isGround && collision.gameObject.layer == LayerMask.NameToLayer("Field")) {
            isGround = false;
            trMoveFront.emitting = false;
            trMoveBack.emitting = false;
            if (player.UserType <= SnowBoard_Define.UserType.PLAYER_4 && isAccel) {
                SingletonCustom<HidVibration>.Instance.StopVibration((int)userType);
            }
        }
    }
    private IEnumerator isInputKeepWait() {
        yield return inputKeeptTime;
        isInputKeep = false;
    }
    private IEnumerator isNoActionWait() {
        yield return new WaitForSeconds(1f);
        isNoAction = false;
    }
    private IEnumerator IsAccelEndWait() {
        yield return new WaitForSeconds(1.5f);
        isAccelEnd = false;
    }
    private IEnumerator DoubleVibration() {
        SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)userType, HidVibration.VibrationType.Common, 0.2f);
        yield return new WaitForSeconds(0.2f);
        SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)userType, HidVibration.VibrationType.Common, 0.2f);
    }
}
