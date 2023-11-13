using GamepadInput;
using System;
using System.Collections.Generic;
using UnityEngine;
public class ArenaBattlePlayer : MonoBehaviour {
    public enum TREE_TOP_CLIM_ANIM_STATE {
        ANIM_1,
        ANIM_2,
        ANIM_3
    }
    public enum State {
        APPEARANCE,
        DEFAULT,
        JUMP,
        JUMP_WAIT,
        SWORD_ATTACK_0,
        SWORD_ATTACK_1,
        SWORD_ATTACK_2,
        SWORD_ATTACK_SP,
        ATTACK_WAIT,
        MAGIC_CAST,
        DODGE,
        MAGIC_ATTACK,
        SHURIKEN_ATTACK,
        CLIMBING,
        CLIBING_TOP,
        WATER_FALL,
        SCROLL_DEATH,
        DEAD,
        VICTORY_WAIT,
        VICTORY,
        KNOCK_BACK,
        GOAL
    }
    [SerializeField]
    [Header("スタイル")]
    private CharacterStyle style;
    [SerializeField]
    [Header("リジッドボディ")]
    private Rigidbody rigid;
    [SerializeField]
    [Header("アニメ\u30fcション")]
    private ArenaBattlePlayer_Animation anim;
    [SerializeField]
    [Header("ロ\u30fcカル重力値")]
    private Vector3 localGravity = new Vector3(0f, -9.81f, 0f);
    [SerializeField]
    [Header("ジャンプエフェクト")]
    private ParticleSystem psJump;
    [SerializeField]
    [Header("突撃エフェクト")]
    private ParticleSystem psDash;
    [SerializeField]
    [Header("装備：剣")]
    private ArenaBattlePlayerSword sword;
    [SerializeField]
    [Header("魔法準備エフェクト")]
    private ParticleSystem psMagicCast;
    [SerializeField]
    [Header("体の各部位")]
    private CharacterParts characterParts;
    [SerializeField]
    [Header("魔法弾")]
    private ArenaBattleMagicBullet magicBullet;
    [SerializeField]
    [Header("ヒットエフェクト")]
    private ParticleSystem psHitEffect;
    [SerializeField]
    [Header("ダメ\u30fcジエフェクト")]
    private ParticleSystem psDamage;
    [SerializeField]
    [Header("やられたエフェクト")]
    private ParticleSystem psDeadEffect;
    [SerializeField]
    [Header("魔法カラ\u30fc設定")]
    private Gradient[] arrayMagicColor;
    [SerializeField]
    [Header("コライダ\u30fc")]
    private CapsuleCollider col;
    [SerializeField]
    [Header("勝利エフェクト")]
    private ParticleSystem psVictory;
    [SerializeField]
    [Header("吹き飛びエフェクト")]
    private ParticleSystem psBlowAway;
    [SerializeField]
    [Header("登場位置アンカ\u30fc")]
    private Transform anchorAppearancePos;
    [SerializeField]
    [Header("必殺エフェクト")]
    private ParticleSystem[] arraySpEffect;
    [SerializeField]
    [Header("必殺技爆破エフェクト")]
    private ParticleSystem[] arrayHitEffect;
    [SerializeField]
    [Header("必殺技状態エフェクト")]
    private ParticleSystem psSpStandby;
    [SerializeField]
    [Header("AI")]
    private ArenaBattlePlayerAI ai;
    private readonly float LOOK_SPEED = 11f;
    public readonly float MOVE_SPEED_MAX = 2.75f;
    private readonly float MOVE_SPEED_SCALE = 15f;
    private readonly float ATTENUATION_SCALE = 0.85f;
    private readonly float SE_RUN_TIME = 0.55f;
    private int playerIdx = -1;
    private JoyConManager.AXIS_INPUT axisInput;
    private Vector3 moveForce;
    private float moveSpeed;
    private bool isJump;
    private bool isJumpInput;
    private float jumpInputTime;
    private int score;
    private float hp = 1f;
    private float runAnimationSpeed = 20f;
    private float runAnimationTime;
    private Vector3 prevPos;
    private Vector3 nowPos;
    private int playSeRunCnt;
    private int goalNo = -1;
    private bool isAddInput;
    private bool isDodge;
    private float deadOutCameraTime = 1f;
    private Vector3 deadPos;
    private float blowAwayMoveScale = 0.5f;
    private float blowPosY;
    private State currentState = State.DEFAULT;
    private float waitTime;
    private readonly float ATTACK_WAIT = 0.25f;
    private readonly float RESPAWN_TIME = 1.5f;
    private float waitRespawnTime;
    private float hitStopTime;
    private List<MeshRenderer> listBlinkMesh = new List<MeshRenderer>();
    private float specialPower;
    private Quaternion tempRot;
    private Vector3 prevDir;
    private Vector3 rotVec;
    private RaycastHit rayHit;
    private Vector3 attackDir;
    private Vector3 knockBackDir;
    private int climbingCount;
    private Transform[] climbTopAnchor;
    private bool isMagicCast;
    private Material[] tempMats;
    private int layerMask = 1048576;
    public float Hp => hp;
    public float Special => specialPower;
    public bool IsDodge => isDodge;
    public Vector3 DeadPos => deadPos;
    public State CurrentState => currentState;
    public CharacterStyle Style => style;
    public float MoveSpeed => moveSpeed;
    public bool IsJump => currentState == State.JUMP;
    public Rigidbody Rigid => rigid;
    public int Score {
        get {
            return score;
        }
        set {
            score = value;
        }
    }
    public bool IsCpu {
        get;
        set;
    }
    public int PlayerIdx => playerIdx;
    public int GoalNo => goalNo;
    public void Init(int _playerIdx) {
        playerIdx = _playerIdx;
        IsCpu = (playerIdx >= SingletonCustom<GameSettingManager>.Instance.PlayerNum);
        ai.Init();
        moveSpeed = 0f;
        prevDir = moveForce;
        style.SetGameStyle(GS_Define.GameType.BLOW_AWAY_TANK, IsCpu ? (4 + (playerIdx - SingletonCustom<GameSettingManager>.Instance.PlayerNum)) : playerIdx);
        anim.SetAnim(ArenaBattlePlayer_Animation.AnimType.STANDBY);
        playSeRunCnt = 0;
        nowPos = (prevPos = base.transform.position);
        sword.Init();
        //??psMagicCast.colorOverLifetime.color = arrayMagicColor[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[IsCpu ? (4 + (PlayerIdx - SingletonCustom<GameSettingManager>.Instance.PlayerNum)) : PlayerIdx]];
        for (int i = 0; i < arraySpEffect.Length; i++) {
            //??arraySpEffect[i].colorOverLifetime.color = arrayMagicColor[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[IsCpu ? (4 + (PlayerIdx - SingletonCustom<GameSettingManager>.Instance.PlayerNum)) : PlayerIdx]];
        }
        for (int j = 0; j < arrayHitEffect.Length; j++) {
            //??arrayHitEffect[j].colorOverLifetime.color = arrayMagicColor[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[IsCpu ? (4 + (PlayerIdx - SingletonCustom<GameSettingManager>.Instance.PlayerNum)) : PlayerIdx]];
        }
        //??psSpStandby.colorOverLifetime.color = arrayMagicColor[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[IsCpu ? (4 + (PlayerIdx - SingletonCustom<GameSettingManager>.Instance.PlayerNum)) : PlayerIdx]];
        MeshRenderer[] meshList = style.GetMeshList(GS_Define.GameType.BLOW_AWAY_TANK, IsCpu ? (4 + (playerIdx - SingletonCustom<GameSettingManager>.Instance.PlayerNum)) : playerIdx);
        for (int k = 0; k < meshList.Length; k++) {
            listBlinkMesh.Add(meshList[k]);
        }
        listBlinkMesh.Add(sword.GetMesh());
    }
    public void Appearance() {
        SetState(State.APPEARANCE);
    }
    public void AddSp() {
        specialPower = Mathf.Clamp(specialPower + 0.1f, 0f, 1f);
        if (specialPower >= 1f && !psSpStandby.isPlaying) {
            psSpStandby.Play();
            SetVibration();
        }
    }
    public void OnAttackStart() {
        sword.AttackStart(currentState == State.SWORD_ATTACK_SP);
        switch (currentState) {
            case State.ATTACK_WAIT:
            case State.MAGIC_CAST:
            case State.DODGE:
                break;
            case State.SWORD_ATTACK_0:
                SingletonCustom<AudioManager>.Instance.SePlay("se_sword_0", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
                break;
            case State.SWORD_ATTACK_1:
                SingletonCustom<AudioManager>.Instance.SePlay("se_sword_1", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
                break;
            case State.SWORD_ATTACK_2:
                SingletonCustom<AudioManager>.Instance.SePlay("se_sword_2", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
                break;
            case State.SWORD_ATTACK_SP:
                SingletonCustom<AudioManager>.Instance.SePlay("se_barrier", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
                SingletonCustom<AudioManager>.Instance.SePlay("se_magic_bullet", _loop: false, 0f, 1f, 1f, 0.1f, _overlap: true);
                for (int i = 0; i < arraySpEffect.Length; i++) {
                    arraySpEffect[i].Play();
                }
                specialPower = 0f;
                break;
            case State.MAGIC_ATTACK: {
                    ArenaBattleMagicBullet arenaBattleMagicBullet = UnityEngine.Object.Instantiate(magicBullet, base.transform.parent);
                    arenaBattleMagicBullet.gameObject.SetActive(value: true);
                    arenaBattleMagicBullet.transform.SetPosition(psMagicCast.transform.position.x, base.transform.position.y + 0.3f, psMagicCast.transform.position.z);
                    arenaBattleMagicBullet.Init(style.transform.forward, arrayMagicColor[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[IsCpu ? (4 + (PlayerIdx - SingletonCustom<GameSettingManager>.Instance.PlayerNum)) : PlayerIdx]]);
                    isMagicCast = false;
                    SingletonCustom<AudioManager>.Instance.SePlay("se_magic_bullet", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
                    break;
                }
        }
    }
    public void OnDodgeEnd() {
        if (CurrentState != State.DODGE) {
            return;
        }
        anim.SetBool(ArenaBattlePlayer_Animation.AnimType.DODGE, _enable: false);
        State state = currentState;
        if ((uint)(state - 18) > 1u) {
            if (isJump) {
                SetState(State.JUMP);
            } else {
                anim.SetAnim(ArenaBattlePlayer_Animation.AnimType.STANDBY);
                SetState(State.DEFAULT);
            }
        }
        isDodge = false;
    }
    public void OnAttackEnd() {
        sword.AttackEnd(currentState == State.SWORD_ATTACK_SP);
        switch (CurrentState) {
            case State.SWORD_ATTACK_0:
                if (isJump) {
                    anim.SetAnim(ArenaBattlePlayer_Animation.AnimType.JUMP);
                    SetState(State.JUMP);
                    anim.SetBool(ArenaBattlePlayer_Animation.AnimType.SWORD_ATTACK_0, _enable: false);
                    break;
                }
                anim.SetBool(ArenaBattlePlayer_Animation.AnimType.SWORD_ATTACK_0, _enable: false);
                if (IsCpu) {
                    isAddInput = ai.IsSwordStage2();
                }
                if (isAddInput) {
                    attackDir = style.transform.forward;
                    SetState(State.SWORD_ATTACK_1);
                    anim.SetBool(ArenaBattlePlayer_Animation.AnimType.SWORD_ATTACK_1, _enable: true);
                } else {
                    waitTime = 0.15f;
                    SetState(State.ATTACK_WAIT);
                }
                break;
            case State.SWORD_ATTACK_1:
                anim.SetBool(ArenaBattlePlayer_Animation.AnimType.SWORD_ATTACK_1, _enable: false);
                if (IsCpu) {
                    isAddInput = ai.IsSwordStage3();
                }
                if (isAddInput) {
                    attackDir = style.transform.forward;
                    SetState(State.SWORD_ATTACK_2);
                    anim.SetBool(ArenaBattlePlayer_Animation.AnimType.SWORD_ATTACK_2, _enable: true);
                    psDash.Play();
                } else {
                    waitTime = 0.1f;
                    SetState(State.ATTACK_WAIT);
                }
                break;
            case State.SWORD_ATTACK_2:
                anim.SetBool(ArenaBattlePlayer_Animation.AnimType.SWORD_ATTACK_2, _enable: false);
                anim.SetAnim(ArenaBattlePlayer_Animation.AnimType.STANDBY);
                SetState(State.ATTACK_WAIT);
                rigid.angularVelocity = Vector3.zero;
                rigid.velocity = Vector3.zero;
                waitTime = 0.25f;
                psDash.Stop();
                break;
            case State.SWORD_ATTACK_SP:
                anim.SetAnim(ArenaBattlePlayer_Animation.AnimType.STANDBY);
                SetState(State.ATTACK_WAIT);
                rigid.angularVelocity = Vector3.zero;
                rigid.velocity = Vector3.zero;
                waitTime = 0.25f;
                for (int i = 0; i < arraySpEffect.Length; i++) {
                    arraySpEffect[i].Stop();
                }
                break;
            case State.MAGIC_ATTACK:
                anim.SetBool(ArenaBattlePlayer_Animation.AnimType.MAGIC_CAST, _enable: false);
                anim.SetBool(ArenaBattlePlayer_Animation.AnimType.MAGIC_ATTACK, _enable: false);
                if (isJump) {
                    SetState(State.JUMP);
                } else {
                    anim.SetAnim(ArenaBattlePlayer_Animation.AnimType.STANDBY);
                    SetState(State.ATTACK_WAIT);
                }
                rigid.angularVelocity = Vector3.zero;
                rigid.velocity = Vector3.zero;
                waitTime = 0.25f;
                break;
        }
        isAddInput = false;
    }
    public void OnVictory() {
        moveSpeed = 0f;
        moveForce = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        rigid.velocity = Vector3.zero;
        waitTime = 1.15f;
        anim.SetAnimSpeed(0f);
        anim.SetBool(ArenaBattlePlayer_Animation.AnimType.JUMP, _enable: false);
        anim.SetBool(ArenaBattlePlayer_Animation.AnimType.SWORD_ATTACK_0, _enable: false);
        anim.SetBool(ArenaBattlePlayer_Animation.AnimType.SWORD_ATTACK_1, _enable: false);
        anim.SetBool(ArenaBattlePlayer_Animation.AnimType.SWORD_ATTACK_2, _enable: false);
        anim.SetBool(ArenaBattlePlayer_Animation.AnimType.MAGIC_CAST, _enable: false);
        anim.SetBool(ArenaBattlePlayer_Animation.AnimType.MAGIC_ATTACK, _enable: false);
        anim.SetBool(ArenaBattlePlayer_Animation.AnimType.DODGE, _enable: false);
        knockBackDir = Vector3.zero;
        psDash.Stop();
        psVictory.Play();
        SetState(State.VICTORY_WAIT);
    }
    public void UpdateMethod() {
        prevPos = nowPos;
        nowPos = base.transform.position;
        if (rigid.velocity.sqrMagnitude >= 300f) {
            rigid.velocity *= 0f;
        }
        if (IsCpu) {
            ai.UpdateMethod();
            switch (currentState) {
                case State.DEFAULT:
                    moveForce = ai.UpdateMove();
                    if (moveForce.magnitude < 0.0400000028f) {
                        moveSpeed = Mathf.Clamp(moveSpeed * ATTENUATION_SCALE, 0f, MOVE_SPEED_MAX);
                    } else if (moveSpeed < MOVE_SPEED_MAX) {
                        moveSpeed = Mathf.Clamp(moveSpeed + Time.deltaTime * MOVE_SPEED_SCALE, MOVE_SPEED_MAX * 0.3f, MOVE_SPEED_MAX);
                    }
                    if (CalcManager.Length(nowPos, prevPos) > 0.01f) {
                        runAnimationTime += CalcManager.Length(nowPos, prevPos) * runAnimationSpeed * Time.deltaTime;
                        if (runAnimationTime >= (float)playSeRunCnt * 0.15f) {
                            playSeRunCnt++;
                            PlaySeRun();
                        }
                        if (runAnimationTime >= 1f) {
                            runAnimationTime = 0f;
                            playSeRunCnt = 1;
                        }
                    }
                    if (ai.IsSwordStage1()) {
                        if (specialPower >= 1f) {
                            AttackSp();
                        } else {
                            AttackSword();
                        }
                    } else if (ai.IsMagic()) {
                        MagicCast();
                    } else if (ai.IsDodge()) {
                        Dodge();
                    } else if (ai.IsJump()) {
                        Jump();
                    }
                    break;
                case State.MAGIC_CAST:
                    AttackMagic();
                    break;
                case State.JUMP:
                    moveForce = ai.UpdateMove();
                    if (moveForce.magnitude < 0.0400000028f) {
                        moveSpeed = Mathf.Clamp(moveSpeed * ATTENUATION_SCALE, 0f, MOVE_SPEED_MAX);
                    } else if (moveSpeed < MOVE_SPEED_MAX) {
                        moveSpeed = Mathf.Clamp(moveSpeed + Time.deltaTime * MOVE_SPEED_SCALE, MOVE_SPEED_MAX * 0.3f, MOVE_SPEED_MAX);
                    }
                    if (ai.IsMagic()) {
                        MagicCast();
                    } else if (ai.IsDodge()) {
                        Dodge();
                    }
                    if (isJumpInput) {
                        jumpInputTime += Time.deltaTime;
                        if (jumpInputTime < 0.25f) {
                            rigid.AddForce(base.transform.up * 0.022f, ForceMode.Impulse);
                        }
                    }
                    break;
                case State.SWORD_ATTACK_0:
                case State.SWORD_ATTACK_1:
                case State.SWORD_ATTACK_2:
                    moveForce = ai.UpdateMove();
                    if (moveForce.magnitude < 0.0400000028f) {
                        moveSpeed = Mathf.Clamp(moveSpeed * ATTENUATION_SCALE, 0f, MOVE_SPEED_MAX);
                    } else if (moveSpeed < MOVE_SPEED_MAX) {
                        moveSpeed = Mathf.Clamp(moveSpeed + Time.deltaTime * MOVE_SPEED_SCALE, MOVE_SPEED_MAX * 0.3f, MOVE_SPEED_MAX);
                    }
                    break;
            }
        } else {
            switch (currentState) {
                case State.DEFAULT:
                    UpdateMoveForce();
                    if (CalcManager.Length(nowPos, prevPos) > 0.01f) {
                        runAnimationTime += CalcManager.Length(nowPos, prevPos) * runAnimationSpeed * Time.deltaTime;
                        if (runAnimationTime >= (float)playSeRunCnt * 0.15f) {
                            playSeRunCnt++;
                            PlaySeRun();
                        }
                        if (runAnimationTime >= 1f) {
                            runAnimationTime = 0f;
                            playSeRunCnt = 1;
                        }
                    }
                    if (!isJump && SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.B)) {
                        Jump();
                    } else if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.A)) {
                        AttackSword();
                    } else if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.Y)) {
                        if (specialPower >= 1f) {
                            AttackSp();
                        } else {
                            MagicCast();
                        }
                    } else if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.X)) {
                        Dodge();
                    }
                    break;
                case State.MAGIC_CAST:
                    UpdateMoveForce();
                    psMagicCast.Emit(1);
                    if (SingletonCustom<JoyConManager>.Instance.GetButtonUp(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.Y)) {
                        AttackMagic();
                    }
                    break;
                case State.MAGIC_ATTACK:
                    if (isMagicCast) {
                        psMagicCast.Emit(1);
                    }
                    break;
                case State.JUMP:
                    UpdateMoveForce();
                    if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.A)) {
                        AttackSword();
                        isJump = true;
                    } else if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.Y)) {
                        MagicCast();
                    } else if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.X)) {
                        Dodge();
                    }
                    if (isJumpInput) {
                        jumpInputTime += Time.deltaTime;
                        if (jumpInputTime < 0.25f) {
                            rigid.AddForce(base.transform.up * 0.022f, ForceMode.Impulse);
                        }
                    }
                    break;
                case State.SWORD_ATTACK_0:
                    UpdateMoveForce();
                    if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.A)) {
                        isAddInput = true;
                    }
                    break;
                case State.SWORD_ATTACK_1:
                    UpdateMoveForce();
                    if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.A)) {
                        isAddInput = true;
                    }
                    break;
                case State.SWORD_ATTACK_2:
                    UpdateMoveForce();
                    break;
                case State.SHURIKEN_ATTACK:
                    UpdateMoveForce();
                    waitTime -= Time.deltaTime;
                    if (waitTime <= 0f) {
                        if (isJump) {
                            anim.SetAnim(ArenaBattlePlayer_Animation.AnimType.JUMP);
                            SetState(State.JUMP);
                        } else {
                            anim.SetAnim(ArenaBattlePlayer_Animation.AnimType.STANDBY);
                            SetState(State.DEFAULT);
                        }
                    }
                    break;
                case State.CLIMBING:
                    if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.A)) {
                        Climbing();
                    }
                    break;
            }
        }
        switch (currentState) {
            case State.GOAL:
                rigid.velocity = new Vector3(rigid.velocity.x * 0.95f, rigid.velocity.y, rigid.velocity.z * 0.95f);
                moveSpeed = Mathf.Clamp(moveSpeed * ATTENUATION_SCALE, 0f, MOVE_SPEED_MAX);
                break;
            case State.WATER_FALL:
                waitRespawnTime -= Time.deltaTime;
                if (waitRespawnTime <= 0f) {
                    Respawn();
                }
                break;
            case State.SCROLL_DEATH:
                waitRespawnTime -= Time.deltaTime;
                if (waitRespawnTime <= 0f) {
                    Respawn();
                }
                break;
            case State.VICTORY_WAIT:
                waitTime -= Time.deltaTime;
                if (waitTime <= 0f) {
                    anim.SetTrigger(ArenaBattlePlayer_Animation.AnimType.VICTORY);
                    style.SetMainCharacterFaceDiff(IsCpu ? (4 + (playerIdx - SingletonCustom<GameSettingManager>.Instance.PlayerNum)) : playerIdx, StyleTextureManager.MainCharacterFaceType.HAPPY);
                    for (int i = 0; i < arraySpEffect.Length; i++) {
                        arraySpEffect[i].Stop();
                    }
                    SetState(State.VICTORY);
                    SingletonCustom<AudioManager>.Instance.SePlay("se_colosseum_win", _loop: false, 0f, 1f, 1f, 0.33f);
                }
                moveSpeed = 0f;
                moveForce = Vector3.zero;
                rigid.angularVelocity = Vector3.zero;
                rigid.velocity = Vector3.zero;
                break;
            case State.VICTORY:
                moveSpeed = 0f;
                moveForce = Vector3.zero;
                rigid.angularVelocity = Vector3.zero;
                rigid.velocity = Vector3.zero;
                break;
            case State.ATTACK_WAIT:
                waitTime -= Time.deltaTime;
                if (waitTime <= 0f) {
                    anim.SetAnim(ArenaBattlePlayer_Animation.AnimType.STANDBY);
                    SetState(State.DEFAULT);
                }
                break;
            case State.DEAD:
                if (deadOutCameraTime > 0f) {
                    deadOutCameraTime -= Time.deltaTime;
                }
                break;
            case State.JUMP_WAIT:
                waitTime -= Time.deltaTime;
                if (waitTime <= 0f) {
                    anim.SetAnim(ArenaBattlePlayer_Animation.AnimType.STANDBY);
                    SetState(State.DEFAULT);
                }
                break;
            case State.KNOCK_BACK:
                waitTime -= Time.deltaTime;
                if (waitTime <= 0f) {
                    anim.SetAnim(ArenaBattlePlayer_Animation.AnimType.STANDBY);
                    SetState(State.DEFAULT);
                }
                break;
        }
        State state = currentState;
        if ((uint)(state - 15) > 1u && state != State.GOAL) {
            CheckScrollDeath();
        }
        anim.SetAnimSpeed(moveSpeed);
        if (moveSpeed > 0.1f && Time.frameCount % 10 == 0 && Time.timeScale > 0f) {
            anim.EmitMoveEffct(1);
        }
    }
    private void Jump() {
        anim.SetAnim(ArenaBattlePlayer_Animation.AnimType.JUMP);
        anim.SetBool(ArenaBattlePlayer_Animation.AnimType.JUMP, _enable: true);
        SingletonCustom<AudioManager>.Instance.SePlay("se_sasuke_jump");
        SetState(State.JUMP);
        psJump.Play();
        isJumpInput = true;
        jumpInputTime = 0f;
        rigid.velocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
        rigid.AddForce(base.transform.up * 0.35f, ForceMode.Impulse);
    }
    private void AttackSword() {
        anim.SetAnim(ArenaBattlePlayer_Animation.AnimType.SWORD_ATTACK_0);
        anim.SetBool(ArenaBattlePlayer_Animation.AnimType.SWORD_ATTACK_0, _enable: true);
        attackDir = style.transform.forward;
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        waitTime = ATTACK_WAIT;
        SetState(State.SWORD_ATTACK_0);
        moveSpeed = 0f;
    }
    private void AttackSp() {
        psSpStandby.Stop();
        psJump.Play();
        anim.SetTrigger(ArenaBattlePlayer_Animation.AnimType.SWORD_ATTACK_SP);
        attackDir = style.transform.forward;
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        waitTime = ATTACK_WAIT;
        SetState(State.SWORD_ATTACK_SP);
        moveSpeed = 0f;
    }
    private void MagicCast() {
        anim.SetBool(ArenaBattlePlayer_Animation.AnimType.MAGIC_CAST, _enable: true);
        if (currentState == State.JUMP) {
            isJump = true;
        }
        SetState(State.MAGIC_CAST);
        waitTime = 0.05f;
        SingletonCustom<AudioManager>.Instance.SePlay("se_magic_cast", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
    }
    private void Dodge() {
        anim.SetBool(ArenaBattlePlayer_Animation.AnimType.DODGE, _enable: true);
        attackDir = style.transform.forward * 1.15f;
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        moveSpeed = 0f;
        if (currentState == State.JUMP) {
            isJump = true;
        }
        SetState(State.DODGE);
        isDodge = true;
        SingletonCustom<AudioManager>.Instance.SePlay("se_oni_dodge");
    }
    private void Dead() {
        SingletonCustom<ArenaBattleGameManager>.Instance.SetTime(playerIdx);
        SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
        SingletonCustom<AudioManager>.Instance.SePlay("se_handseal_earthcrack");
        psDeadEffect.Play();
        psBlowAway.Play();
        sword.AttackEnd();
        col.enabled = false;
        rigid.isKinematic = true;
        anim.SetTrigger(ArenaBattlePlayer_Animation.AnimType.DAMAGE);
        anim.SetBool(ArenaBattlePlayer_Animation.AnimType.DEAD, _enable: true);
        SetState(State.DEAD);
        UnityEngine.Debug.Log("distance★" + Vector3.Distance(base.transform.position, SingletonCustom<ArenaBattleFieldManager>.Instance.AnchorCenter.position).ToString());
        deadPos = base.transform.position;
    }
    private void AttackMagic() {
        anim.SetBool(ArenaBattlePlayer_Animation.AnimType.MAGIC_ATTACK, _enable: true);
        SetState(State.MAGIC_ATTACK);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        isMagicCast = true;
        moveSpeed = 0f;
    }
    private void AttackShuriken() {
        anim.SetAnim(ArenaBattlePlayer_Animation.AnimType.SWORD_ATTACK_0);
        attackDir = moveForce;
        waitTime = ATTACK_WAIT;
        SetState(State.SHURIKEN_ATTACK);
    }
    private void Climbing() {
        rigid.isKinematic = false;
        climbingCount++;
        ClimbingAnimation(0.1f, climbingCount % 2 == 1);
        rigid.AddForce(Vector3.up * 250f, ForceMode.Acceleration);
        if (!IsCpu) {
            SingletonCustom<AudioManager>.Instance.SePlay("se_blow");
        }
        LeanTween.cancel(base.gameObject);
        LeanTween.delayedCall(base.gameObject, 0.1f, (Action)delegate {
            rigid.velocity = Vector3.zero;
        });
        if (base.transform.position.y >= climbTopAnchor[0].position.y) {
            SetState(State.CLIBING_TOP);
            RecursiveTreeTopClimbingAnimation(TREE_TOP_CLIM_ANIM_STATE.ANIM_1);
        }
    }
    public void OnGoal() {
    }
    private void Respawn() {
    }
    private void CheckScrollDeath() {
    }
    private void RecursiveTreeTopClimbingAnimation(TREE_TOP_CLIM_ANIM_STATE _goalAnimState) {
        TreeTopClimbingAnimation(0.25f, _goalAnimState);
        LeanTween.delayedCall(base.gameObject, 0.25f, (Action)delegate {
            _goalAnimState++;
            if ((int)_goalAnimState < Enum.GetValues(typeof(TREE_TOP_CLIM_ANIM_STATE)).Length) {
                RecursiveTreeTopClimbingAnimation(_goalAnimState);
            } else {
                SetState(State.DEFAULT);
                anim.SetAnim(ArenaBattlePlayer_Animation.AnimType.STANDBY);
                rigid.velocity = Vector3.zero;
                rigid.angularVelocity = Vector3.zero;
                rigid.isKinematic = false;
            }
        });
    }
    public void SetVibration() {
        if (!IsCpu) {
            SingletonCustom<HidVibration>.Instance.SetCommonVibration(playerIdx);
        }
    }
    public float GetAttackDamage() {
        switch (currentState) {
            case State.SWORD_ATTACK_0:
                return 0.05f;
            case State.SWORD_ATTACK_1:
                return 0.065f;
            case State.SWORD_ATTACK_2:
                return 0.1f;
            case State.SWORD_ATTACK_SP:
                return 0.125f;
            default:
                return 0f;
        }
    }
    public void KnockBack(Vector3 _dir, float _pow, float _damage) {
        switch (currentState) {
            case State.GOAL:
                return;
            case State.CLIMBING:
            case State.CLIBING_TOP:
                _pow *= 0.1f;
                break;
        }
        if (currentState != State.VICTORY_WAIT && currentState != State.VICTORY) {
            if (!IsCpu) {
                SingletonCustom<HidVibration>.Instance.SetCommonVibration(playerIdx);
            }
            psHitEffect.transform.position = base.transform.position;
            psHitEffect.Play();
            _dir.y = 0f;
            knockBackDir = _dir * _pow;
            SingletonCustom<AudioManager>.Instance.SePlay("se_damage", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
            hp = Mathf.Clamp(hp -= _damage, 0f, 1f);
            anim.SetTrigger(ArenaBattlePlayer_Animation.AnimType.DAMAGE);
            LeanTween.value(base.gameObject, 0f, 1f, 0.2f).setOnUpdate(delegate (float _value) {
                for (int k = 0; k < listBlinkMesh.Count; k++) {
                    tempMats = listBlinkMesh[k].materials;
                    for (int l = 0; l < tempMats.Length; l++) {
                        tempMats[l].EnableKeyword("_EMISSION");
                        tempMats[l].SetColor("_EmissionColor", Color.white * _value);
                    }
                    listBlinkMesh[k].materials = tempMats;
                }
            }).setOnComplete((Action)delegate {
                LeanTween.value(base.gameObject, 1f, 0f, 0.2f).setOnUpdate(delegate (float _value) {
                    for (int i = 0; i < listBlinkMesh.Count; i++) {
                        tempMats = listBlinkMesh[i].materials;
                        for (int j = 0; j < tempMats.Length; j++) {
                            tempMats[j].EnableKeyword("_EMISSION");
                            tempMats[j].SetColor("_EmissionColor", Color.white * _value);
                        }
                        listBlinkMesh[i].materials = tempMats;
                    }
                });
            });
            waitTime = 0.25f;
            anim.SetBool(ArenaBattlePlayer_Animation.AnimType.JUMP, _enable: false);
            anim.SetBool(ArenaBattlePlayer_Animation.AnimType.SWORD_ATTACK_0, _enable: false);
            anim.SetBool(ArenaBattlePlayer_Animation.AnimType.SWORD_ATTACK_1, _enable: false);
            anim.SetBool(ArenaBattlePlayer_Animation.AnimType.SWORD_ATTACK_2, _enable: false);
            anim.SetBool(ArenaBattlePlayer_Animation.AnimType.MAGIC_CAST, _enable: false);
            anim.SetBool(ArenaBattlePlayer_Animation.AnimType.MAGIC_ATTACK, _enable: false);
            anim.SetBool(ArenaBattlePlayer_Animation.AnimType.DODGE, _enable: false);
            psDash.Stop();
            sword.AttackEnd();
            SetState(State.KNOCK_BACK);
            if (hp <= 0f) {
                knockBackDir = _dir * 5.5f;
                Dead();
            } else {
                HitStop();
                psDamage.Play();
            }
        }
    }
    public void SetClimbing(Transform[] _topAnchor, Transform _posAnchor) {
        climbTopAnchor = _topAnchor;
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        SetState(State.CLIMBING);
        anim.SetAnim(ArenaBattlePlayer_Animation.AnimType.CLIMBING);
        base.transform.SetLocalEulerAnglesY(270f);
        base.transform.SetPositionZ(_posAnchor.position.z);
        ReadyClimbingAnimation(0.25f);
        bool isCpu = IsCpu;
    }
    public void AnimPorRotReset() {
        characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD).transform.localPosition = new Vector3(0f, 0.1735753f, 0f);
        characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD).transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY).transform.localPosition = new Vector3(0f, 0f, 0f);
        characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY).transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HIP).transform.localPosition = new Vector3(0f, 0.05483828f, 0f);
        characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HIP).transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L).transform.localPosition = new Vector3(-0.1511168f, 0.1297733f, 0f);
        characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L).transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R).transform.localPosition = new Vector3(0.1511168f, 0.1297733f, 0f);
        characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R).transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_L).transform.localPosition = new Vector3(0.006473546f, -0.02895849f, 0f);
        characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_L).transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_R).transform.localPosition = new Vector3(-0.006473546f, -0.02895849f, 0f);
        characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_R).transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L).transform.localPosition = new Vector3(-0.054f, -0.0483f, 0f);
        characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L).transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R).transform.localPosition = new Vector3(0.054f, -0.0483f, 0f);
        characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R).transform.localEulerAngles = new Vector3(0f, 0f, 0f);
    }
    public void FallClimbing() {
        AnimPorRotReset();
        anim.SetAnim(ArenaBattlePlayer_Animation.AnimType.JUMP);
        SetState(State.JUMP);
        isJumpInput = false;
        jumpInputTime = 0f;
        rigid.isKinematic = false;
        isJump = true;
    }
    private void UpdateMoveForce() {
        CalcManager.mCalcVector2 = ArenaBattleControllerManager.GetStickDir(playerIdx);
        if (CalcManager.mCalcVector2.magnitude < 0.0400000028f) {
            moveSpeed = Mathf.Clamp(moveSpeed * ATTENUATION_SCALE, 0f, MOVE_SPEED_MAX);
            if (anim.CurrentAnimType == ArenaBattlePlayer_Animation.AnimType.DASH && moveSpeed <= 0.5f) {
                anim.SetAnim(ArenaBattlePlayer_Animation.AnimType.STANDBY);
            }
        } else {
            ArenaBattlePlayer_Animation.AnimType currentAnimType = anim.CurrentAnimType;
            if (currentAnimType == ArenaBattlePlayer_Animation.AnimType.STANDBY) {
            }
            if (moveSpeed < MOVE_SPEED_MAX) {
                moveSpeed = Mathf.Clamp(moveSpeed + Time.deltaTime * MOVE_SPEED_SCALE, MOVE_SPEED_MAX * 0.3f, MOVE_SPEED_MAX);
            }
            moveForce.x = CalcManager.mCalcVector2.x;
            moveForce.z = CalcManager.mCalcVector2.y;
            moveForce.y *= 0.95f;
            moveForce = moveForce.normalized;
        }
        if (Mathf.Abs(Vector3.Angle(prevDir, moveForce)) >= 90f) {
            moveSpeed *= 0.1f;
        }
        prevDir = moveForce;
    }
    public void FixedUpdate() {
        SetLocalGravity();
        if (hitStopTime > 0f) {
            hitStopTime -= Time.deltaTime;
            rigid.angularVelocity = Vector3.zero;
            rigid.velocity = Vector3.zero;
            return;
        }
        switch (currentState) {
            case State.APPEARANCE:
                rigid.velocity = new Vector3(0f, rigid.velocity.y, 0f);
                moveSpeed = MOVE_SPEED_MAX * 0.45f;
                moveForce = (anchorAppearancePos.position - base.transform.position).normalized;
                anim.SetAnimSpeed(moveSpeed);
                rigid.MovePosition(rigid.transform.position + new Vector3((moveForce * moveSpeed).x, rigid.velocity.y, (moveForce * moveSpeed).z) * Time.deltaTime);
                nowPos = base.transform.position;
                prevPos = nowPos;
                if (Vector3.Distance(base.transform.position, anchorAppearancePos.position) <= 0.1f) {
                    moveSpeed = 0f;
                    moveForce = Vector3.zero;
                    anim.SetAnimSpeed(0f);
                    SetState(State.DEFAULT);
                    return;
                }
                break;
            case State.DEFAULT:
            case State.JUMP:
            case State.SHURIKEN_ATTACK:
            case State.GOAL:
                rigid.velocity = new Vector3(0f, rigid.velocity.y, 0f);
                rigid.MovePosition(rigid.transform.position + new Vector3((moveForce * moveSpeed).x, rigid.velocity.y, (moveForce * moveSpeed).z) * Time.deltaTime);
                break;
            case State.MAGIC_CAST:
                rigid.velocity = new Vector3(0f, rigid.velocity.y, 0f);
                rigid.MovePosition(rigid.transform.position + new Vector3((moveForce * moveSpeed).x, rigid.velocity.y, (moveForce * moveSpeed).z) * 0.25f * Time.deltaTime);
                break;
            case State.SWORD_ATTACK_0:
                if (isJump) {
                    rigid.velocity = new Vector3(0f, rigid.velocity.y, 0f);
                    rigid.MovePosition(rigid.transform.position + new Vector3((moveForce * moveSpeed).x, rigid.velocity.y, (moveForce * moveSpeed).z) * Time.deltaTime);
                } else {
                    rigid.velocity = new Vector3((attackDir * 2.5f).x, rigid.velocity.y, (attackDir * 2.5f).z);
                }
                break;
            case State.SWORD_ATTACK_1:
                rigid.velocity = new Vector3((attackDir * 3f).x, rigid.velocity.y, (attackDir * 3f).z);
                break;
            case State.SWORD_ATTACK_2:
                rigid.velocity = new Vector3((attackDir * 5f).x, rigid.velocity.y, (attackDir * 5f).z);
                break;
            case State.DODGE:
                rigid.velocity = new Vector3((attackDir * 3.75f).x, rigid.velocity.y, (attackDir * 3.75f).z);
                break;
            case State.WATER_FALL:
                rigid.velocity *= 0.77f;
                break;
            case State.DEAD: {
                    blowAwayMoveScale = Mathf.Clamp(blowAwayMoveScale + Time.deltaTime, 0f, 1.5f);
                    CalcManager.mCalcVector3.x = base.transform.position.x + knockBackDir.x * Time.deltaTime * blowAwayMoveScale;
                    CalcManager.mCalcVector3.y = base.transform.position.y + (3.25f + blowPosY) * Time.deltaTime * blowAwayMoveScale;
                    CalcManager.mCalcVector3.z = base.transform.position.z + knockBackDir.z * Time.deltaTime * blowAwayMoveScale;
                    rigid.MovePosition(CalcManager.mCalcVector3);
                    Quaternion rhs = Quaternion.Euler(new Vector3(0f, 720f, 0f) * Time.deltaTime);
                    rigid.MoveRotation(rigid.rotation * rhs);
                    return;
                }
        }
        rigid.velocity += knockBackDir;
        knockBackDir *= 0.8f;
        switch (currentState) {
            case State.DEFAULT:
            case State.JUMP:
            case State.SWORD_ATTACK_0:
            case State.SWORD_ATTACK_1:
            case State.SWORD_ATTACK_2:
            case State.MAGIC_CAST:
            case State.SHURIKEN_ATTACK:
                if (currentState == State.DEFAULT) {
                    if (rigid.velocity.y <= -3f) {
                        anim.SetAnim(ArenaBattlePlayer_Animation.AnimType.JUMP);
                    } else {
                        if (anim.CurrentAnimType == ArenaBattlePlayer_Animation.AnimType.JUMP) {
                            anim.SetAnim(ArenaBattlePlayer_Animation.AnimType.STANDBY);
                        }
                        if (isJump) {
                            isJump = false;
                        }
                    }
                }
                if (moveForce.magnitude >= 0.01f) {
                    rotVec.x = moveForce.x;
                    rotVec.z = moveForce.z;
                    tempRot = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Quaternion.Euler(0f, 0f, 0f) * rotVec), Time.deltaTime * LOOK_SPEED);
                    if (tempRot != Quaternion.identity) {
                        base.transform.rotation = tempRot;
                        rigid.MoveRotation(tempRot);
                    }
                }
                break;
            case State.GOAL:
                tempRot = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Quaternion.Euler(0f, -90f, 0f) * new Vector3(0f, 0f, -1f)), Time.deltaTime * LOOK_SPEED);
                if (tempRot != Quaternion.identity) {
                    base.transform.rotation = tempRot;
                    rigid.MoveRotation(tempRot);
                }
                break;
            case State.VICTORY:
                tempRot = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Quaternion.Euler(0f, 0f, 0f) * new Vector3(0f, 0f, -1f)), Time.deltaTime * LOOK_SPEED);
                if (tempRot != Quaternion.identity) {
                    base.transform.rotation = tempRot;
                    rigid.MoveRotation(tempRot);
                }
                break;
        }
    }
    private void PlaySeRun() {
        if (!IsCpu) {
            SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 0.25f);
        }
    }
    public void OnJumpAiCall() {
        moveSpeed = MOVE_SPEED_MAX;
    }
    public bool IsCameraIn() {
        if (hp > 0f) {
            return true;
        }
        if (deadOutCameraTime > 0f) {
            return true;
        }
        return false;
    }
    public void HitStop() {
        anim.HitStop();
        hitStopTime = 0.035f;
    }
    public void SetState(State _state) {
        if (currentState != _state) {
            State currentState2 = currentState;
            currentState = _state;
            switch (currentState) {
                case State.DEFAULT:
                    sword.AttackEnd();
                    break;
                case State.JUMP_WAIT:
                    waitTime = 0.025f;
                    anim.SetBool(ArenaBattlePlayer_Animation.AnimType.JUMP, _enable: false);
                    SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 0.25f);
                    break;
                case State.WATER_FALL:
                    score = (int)((float)score * 0.5f);
                    waitRespawnTime = RESPAWN_TIME;
                    break;
                case State.SCROLL_DEATH:
                    waitRespawnTime = RESPAWN_TIME;
                    break;
            }
        }
    }
    private void SetLocalGravity() {
        if (currentState != State.CLIMBING) {
            rigid.AddForce(localGravity, ForceMode.Acceleration);
        }
    }
    private void OnCollisionEnter(Collision collision) {
        if (currentState != State.WATER_FALL && collision.gameObject.layer == LayerMask.NameToLayer("Water")) {
            SetState(State.WATER_FALL);
            AnimPorRotReset();
            anim.SetAnim(ArenaBattlePlayer_Animation.AnimType.DROWN);
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
            style.SetMainCharacterFaceDiff(IsCpu ? (4 + (playerIdx - SingletonCustom<GameSettingManager>.Instance.PlayerNum)) : playerIdx, StyleTextureManager.MainCharacterFaceType.SAD);
            if (!IsCpu) {
                SingletonCustom<HidVibration>.Instance.SetCommonVibration(playerIdx);
            }
            SingletonCustom<AudioManager>.Instance.SePlay("se_water_collision_2");
        } else {
            if (currentState != State.JUMP || collision.gameObject.layer != LayerMask.NameToLayer("Character")) {
                return;
            }
            int num = 0;
            while (true) {
                if (num < collision.contacts.Length) {
                    if (base.transform.position.y + 0.02f >= collision.contacts[num].point.y) {
                        break;
                    }
                    num++;
                    continue;
                }
                return;
            }
            State currentState2 = currentState;
            anim.SetAnim(ArenaBattlePlayer_Animation.AnimType.STANDBY);
            SetState(State.JUMP_WAIT);
        }
    }
    private void OnCollisionStay(Collision collision) {
        if (currentState != State.JUMP || collision.gameObject.layer != LayerMask.NameToLayer("Field") || !(Mathf.Abs(rigid.velocity.y) <= 0.5f)) {
            return;
        }
        ContactPoint[] contacts = collision.contacts;
        int num = 0;
        while (true) {
            if (num < contacts.Length) {
                ContactPoint contactPoint = contacts[num];
                if (contactPoint.point.y < base.transform.position.y) {
                    break;
                }
                num++;
                continue;
            }
            return;
        }
        State currentState2 = currentState;
        anim.SetAnim(ArenaBattlePlayer_Animation.AnimType.STANDBY);
        SetState(State.JUMP_WAIT);
    }
    private void Animation(Transform _parts, Vector3 _pos, Vector3 _euler, float _animTime) {
        LeanTween.cancel(_parts.gameObject);
        LeanTween.moveLocal(_parts.gameObject, _pos, _animTime);
        LeanTween.rotateLocal(_parts.gameObject, _euler, _animTime);
    }
    private void AnimationRotateAround(Transform _parts, Vector3 _pos, Vector3 _dir, float _angle, float _animTime) {
        LeanTween.cancel(_parts.gameObject);
        LeanTween.moveLocal(_parts.gameObject, _pos, _animTime);
        LeanTween.rotateAround(_parts.gameObject, _dir, _angle, _animTime);
    }
    public void ReadyClimbingAnimation(float _animTime) {
        Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.17f, 0f), new Vector3(340f, 0f, 0f), _animTime);
        Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), _animTime);
        Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HIP), new Vector3(0f, 0.055f, 0f), new Vector3(355f, 0f, 0f), _animTime);
        Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.13f, 0.116f, 0.097f), new Vector3(45f, 300f, 145f), _animTime);
        Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.13f, 0.173f, 0.103f), new Vector3(315f, 240f, 145f), _animTime);
        Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_L), new Vector3(0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 0f), _animTime);
        Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_R), new Vector3(-0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 0f), _animTime);
        Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.08f, -0.015f, 0.09f), new Vector3(0f, 0f, 0f), _animTime);
        Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.055f, -0.055f, 0.08f), new Vector3(0f, 0f, 0f), _animTime);
    }
    public void ClimbingAnimation(float _animTime, bool isOdd) {
        if (!isOdd) {
            Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.17f, 0f), new Vector3(340f, 0f, 0f), _animTime);
            Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.13f, 0.116f, 0.097f), new Vector3(45f, 300f, 145f), _animTime);
            Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.13f, 0.173f, 0.103f), new Vector3(315f, 240f, 145f), _animTime);
            Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.08f, -0.015f, 0.09f), new Vector3(0f, 0f, 0f), _animTime);
            Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.055f, -0.055f, 0.08f), new Vector3(0f, 0f, 0f), _animTime);
        } else {
            Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.17f, 0f), new Vector3(350f, 0f, 0f), _animTime);
            Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.13f, 0.173f, 0.103f), new Vector3(45f, 300f, 145f), _animTime);
            Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.13f, 0.116f, 0.097f), new Vector3(315f, 240f, 145f), _animTime);
            Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.055f, -0.055f, 0.08f), new Vector3(0f, 0f, 0f), _animTime);
            Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.08f, -0.015f, 0.09f), new Vector3(0f, 0f, 0f), _animTime);
        }
    }
    public void ReadySideMoveAnimation(float _animTime) {
        Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.12f, 0.125f, 0.097f), new Vector3(45f, 300f, 145f), _animTime);
        Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.12f, 0.125f, 0.097f), new Vector3(315f, 240f, 145f), _animTime);
        Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.095f, -0.04f, 0.09f), new Vector3(0f, 0f, 0f), _animTime);
        Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.095f, -0.04f, 0.09f), new Vector3(0f, 0f, 0f), _animTime);
    }
    public void SideMoveAnimation(float _animTime, bool isLeftMove) {
        if (isLeftMove) {
            Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.15f, 0.125f, 0.097f), new Vector3(45f, 300f, 145f), _animTime);
            Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.12f, 0.155f, 0.097f), new Vector3(315f, 240f, 145f), _animTime);
            Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.105f, -0.04f, 0.09f), new Vector3(0f, 0f, 0f), _animTime);
            Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.065f, -0.04f, 0.09f), new Vector3(0f, 0f, 0f), _animTime);
        } else {
            Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.12f, 0.155f, 0.097f), new Vector3(45f, 300f, 145f), _animTime);
            Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.15f, 0.125f, 0.097f), new Vector3(315f, 240f, 145f), _animTime);
            Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.065f, -0.04f, 0.09f), new Vector3(0f, 0f, 0f), _animTime);
            Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.105f, -0.04f, 0.09f), new Vector3(0f, 0f, 0f), _animTime);
        }
    }
    private void TreeTopClimbingAnimation(float _animTime, TREE_TOP_CLIM_ANIM_STATE _climbAnimState) {
        switch (_climbAnimState) {
            case TREE_TOP_CLIM_ANIM_STATE.ANIM_1:
                CalcManager.mCalcVector3.x = base.transform.localPosition.x;
                CalcManager.mCalcVector3.y = base.transform.parent.InverseTransformPoint(climbTopAnchor[0].position).y;
                CalcManager.mCalcVector3.z = base.transform.parent.InverseTransformPoint(climbTopAnchor[0].position).z;
                Animation(base.transform, CalcManager.mCalcVector3, new Vector3(0f, -90f, 0f), _animTime);
                Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.1735753f, 0f), new Vector3(349.2066f, 0f, 0f), _animTime);
                Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0.002f, -0.013f), new Vector3(359.3358f, 0f, 0f), _animTime);
                Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HIP), new Vector3(0f, 0.05356598f, 0.01595052f), new Vector3(1.530573f, 0f, 0f), _animTime);
                Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.151f, 0.127f, 0.04f), new Vector3(348.2313f, 63.27095f, 267.2055f), _animTime);
                Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.151f, 0.123f, 0.052f), new Vector3(356.7374f, 301.0549f, 116.8664f), _animTime);
                Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_L), new Vector3(0.006473546f, -0.02895849f, 0f), new Vector3(316.0092f, 3.994534f, 6.859902f), _animTime);
                Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_R), new Vector3(-0.006473546f, -0.02895849f, 0f), new Vector3(319.6358f, 30.74282f, 316.3231f), _animTime);
                Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.068f, -0.04833f, 0f), new Vector3(348.5809f, 0.3663634f, 0.5334841f), _animTime);
                Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.054f, -0.0483f, 0f), new Vector3(0f, 0f, 0f), _animTime);
                break;
            case TREE_TOP_CLIM_ANIM_STATE.ANIM_2:
                CalcManager.mCalcVector3.x = base.transform.localPosition.x;
                CalcManager.mCalcVector3.y = base.transform.parent.InverseTransformPoint(climbTopAnchor[1].position).y;
                CalcManager.mCalcVector3.z = base.transform.parent.InverseTransformPoint(climbTopAnchor[1].position).z;
                Animation(base.transform, CalcManager.mCalcVector3, new Vector3(0f, -90f, 0f), _animTime);
                Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.175f, 0f), new Vector3(340.5f, 0f, 0f), _animTime);
                Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0.005f, -0.015f), new Vector3(43.77412f, 0f, 0f), _animTime);
                Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HIP), new Vector3(0f, 0.04087448f, 0.01260412f), new Vector3(1.943285f, 0f, 0f), _animTime);
                Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.151f, 0.122f, 0.034f), new Vector3(336f, 73f, 235f), _animTime);
                Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.151f, 0.127f, 0.04f), new Vector3(350f, 295f, 95f), _animTime);
                Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_L), new Vector3(0.006473546f, -0.02895849f, 0f), new Vector3(315f, 5f, 6.86f), _animTime);
                Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_R), new Vector3(0f, -0.029f, 0f), new Vector3(315f, 30f, 0f), _animTime);
                Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.112f, -0.021f, 0.054f), new Vector3(8.567815f, 318.7467f, 291.7385f), _animTime);
                Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.054f, -0.047f, 0.015f), new Vector3(332.2637f, 0f, 0f), _animTime);
                break;
            case TREE_TOP_CLIM_ANIM_STATE.ANIM_3:
                CalcManager.mCalcVector3.x = base.transform.localPosition.x;
                CalcManager.mCalcVector3.y = base.transform.parent.InverseTransformPoint(climbTopAnchor[2].position).y;
                CalcManager.mCalcVector3.z = base.transform.parent.InverseTransformPoint(climbTopAnchor[2].position).z;
                Animation(base.transform, CalcManager.mCalcVector3, new Vector3(0f, -90f, 0f), _animTime);
                Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.1735753f, 0f), new Vector3(0f, 0f, 0f), _animTime);
                Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), _animTime);
                Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HIP), new Vector3(0f, 0.05483828f, 0f), new Vector3(0f, 0f, 0f), _animTime);
                Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.1511168f, 0.1297733f, 0f), new Vector3(0f, 0f, 0f), _animTime);
                Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.1511168f, 0.1297733f, 0f), new Vector3(0f, 0f, 0f), _animTime);
                Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_L), new Vector3(0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 0f), _animTime);
                Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_R), new Vector3(-0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 0f), _animTime);
                Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.054f, -0.0483f, 0f), new Vector3(0f, 0f, 0f), _animTime);
                Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.054f, -0.0483f, 0f), new Vector3(0f, 0f, 0f), _animTime);
                break;
        }
    }
    private void AfterRankGoalAnimation(int _rank) {
        LeanTween.rotateY(base.gameObject, base.transform.eulerAngles.y + 15f, 1f);
        LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate {
            LeanTween.rotateY(base.gameObject, base.transform.eulerAngles.y - 30f, 2f).setLoopPingPong();
        });
        switch (_rank) {
            case 1:
                LeanTween.rotateX(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L).gameObject, base.transform.localEulerAngles.x + 45f, 0.5f).setLoopPingPong();
                LeanTween.rotateX(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R).gameObject, base.transform.localEulerAngles.x + 45f, 0.5f).setLoopPingPong();
                break;
            case 2:
                LeanTween.rotateX(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R).gameObject, base.transform.localEulerAngles.x + 45f, 0.5f).setLoopPingPong();
                break;
        }
    }
    private void OnDestroy() {
        LeanTween.cancel(style.gameObject);
    }
}
