using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SwordFight_CharacterScript : MonoBehaviour {
    public enum ActionState {
        STANDARD,
        MOVE,
        ATTACK,
        DEFENCE,
        REPEL,
        FREEZE,
        DEATH,
        DODGE,
        JUMP
    }
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
        public MeshFilter[] filterList;
        public Transform Parts(BodyPartsList _parts) {
            return rendererList[(int)_parts].transform;
        }
        public Transform Parts(int _parts) {
            return rendererList[_parts].transform;
        }
    }
    public delegate void AiActionMethod();
    public enum NowPosStatus {
        SAFE,
        CAUTION,
        DANGER
    }
    [Serializable]
    public struct DebugShowData {
        public bool searchVicinityCharacter;
        public bool frontSearchCharacter;
        public bool transformRightDir;
        public bool knockBackDir;
        public DebugShowData(bool _searchVicinityCharacter, bool _frontSearchCharacter, bool _transformRightDir, bool _knockBackDir) {
            searchVicinityCharacter = _searchVicinityCharacter;
            frontSearchCharacter = _frontSearchCharacter;
            transformRightDir = _transformRightDir;
            knockBackDir = _knockBackDir;
        }
    }
    private const float JUMP_POWER = 490f;
    private const float ADD_GRAVITY = 400f;
    public const float WALK_SPEED = 0.6f;
    public const float RUN_SPEED = 1f;
    private const float ATTACK_WALK_SPEED = 0.2f;
    private const int DAMAGE_LIMIT_COUNT = 6;
    private const float FREEZE_TIME_CONST = 0.3f;
    private const float INVINCIBLE_TIME_CONST = 0.45f;
    private const float REPEL_TIME_CONST = 0.5f;
    private const float RE_ATTACK_INTERVAL_TIME_CONST = 0.25f;
    private Vector3[] calcVec = new Vector3[2];
    private Vector3 rot;
    private ActionState actionState;
    [SerializeField]
    [Header("オブジェクト")]
    private Transform obj;
    private Vector3 defPos;
    private Vector3 prevPos;
    private Vector3 nowPos;
    private Vector3 gameStartStandbyPos;
    private float controlInterval;
    private bool isInvincible;
    private int damageCount;
    [SerializeField]
    [Header("走るエフェクト")]
    private ParticleSystem runEffect;
    [SerializeField]
    [Header("汗エフェクト")]
    private ParticleSystem sweatEffect;
    [SerializeField]
    [Header("吹き飛ばしエフェクト")]
    private ParticleSystem[] breakEffect;
    [SerializeField]
    [Header("ジャンプエフェクト")]
    private ParticleSystem psJump;
    private float triggerCheckInterval;
    [SerializeField]
    [Header("体のパ\u30fcツ")]
    private BodyParts bodyParts;
    [SerializeField]
    [Header("RigidBody")]
    private Rigidbody rigid;
    [SerializeField]
    [Header("CapsuleCollider")]
    private CapsuleCollider charaCollider;
    [SerializeField]
    [Header("ヘッドギア")]
    private GameObject headgear;
    [SerializeField]
    [Header("ソ\u30fcド")]
    private SwordFight_Sword sword;
    [SerializeField]
    [Header("モ\u30fcションデ\u30fcタ")]
    private SwordFight_MotionData motionData;
    private float runAnimationSpeed = 20f;
    private float runAnimationTime;
    private float attackAnimationTime;
    private int playSeRunCnt;
    private float runSeInterval;
    private bool isChangeAnimationNeutral;
    private float actionChangeInterval;
    [SerializeField]
    [Header("モ\u30fcションの開始：デバッグモ\u30fcド時有効")]
    private bool isStartAnimation;
    [SerializeField]
    [Header("モ\u30fcションの停止：デバッグモ\u30fcド時有効")]
    private bool isStopAnimation;
    [SerializeField]
    [Header("モ\u30fcションのル\u30fcプ化：デバッグモ\u30fcド時有効")]
    private bool isLoopAnimationMode;
    [SerializeField]
    [Header("確認用のモ\u30fcション指定：デバッグモ\u30fcド時有効")]
    private SwordFight_MotionData.MotionType motionType;
    [SerializeField]
    [Header("モ\u30fcションデ\u30fcタ読み込み：デバッグモ\u30fcド時有効")]
    private SwordFight_MotionData.MotionType loadMotionType;
    [SerializeField]
    [Header("モ\u30fcションの読み込み：デバッグモ\u30fcド時有効")]
    private bool isLoadMotionData;
    private bool isDuringAnimation;
    private bool isWinnerPlayer;
    [SerializeField]
    [Header("キャラクタ\u30fcの方向キャスト用のアンカ\u30fc")]
    private Transform characterDirectionCastAnchor;
    private List<AiActionMethod> aiActionMethod = new List<AiActionMethod>();
    private float aiActionTime;
    private bool isInitAiAction;
    private bool isCallAiAction;
    private RaycastHit raycastHit;
    private SwordFight_CharacterScript frontCharacter;
    private int playerNo;
    private int charaNo;
    private int teamNo;
    private new string name;
    private int uniformNumber;
    private float reAttackTimeValue;
    private float reAttackTimeValueMax = 1f;
    private const float continusAttackInputTime = 0.2f;
    private float currentContinusAttackInputTime;
    private float moveSpeed = 300f;
    private float moveSpeedMax = 0.2f;
    private Transform originAnchor;
    private float charaBodySize;
    private float charaHeight;
    private float moveSpeedParamCorr = 1f;
    private int nowUseDeffenceCount;
    private bool isFirstAttack;
    private bool isSecondAttack;
    private bool isLastAttack;
    [SerializeField]
    [Header("ヘッドギアのMeshRenderer")]
    private MeshRenderer headgearMeshRenderer;
    [SerializeField]
    [Header("ソ\u30fcドのMeshRenderer")]
    private MeshRenderer swordMeshRenderer;
    [SerializeField]
    [Header("ドロップ用ソ\u30fcド")]
    private MeshRenderer dropSwordMeshRenderer;
    [SerializeField]
    [Header("プレイヤ\u30fcごとに設定するマテリアル")]
    private Material[] playerMaterial;
    [SerializeField]
    [Header("CPU１に設定するマテリアル")]
    private Material cpuMaterial_1;
    [SerializeField]
    [Header("CPU１に設定するマテリアル")]
    private Material cpuMaterial_2;
    [SerializeField]
    [Header("キャラスタイル")]
    private CharacterStyle charaStyle;
    protected float actionInterval;
    [SerializeField]
    [Header("オブジェクト基点")]
    protected Transform objPivot;
    [SerializeField]
    [Header("回避用アンカ\u30fc")]
    protected Transform dodgeAnchor;
    [SerializeField]
    [Header("ロ\u30fcカル重力値")]
    private Vector3 localGravity = new Vector3(0f, -9.81f, 0f);
    [SerializeField]
    [Header("背中ニンジャ剣（標準：結合分）")]
    private GameObject defaultCarrySword;
    [SerializeField]
    [Header("背中ニンジャ剣（分割）")]
    private MeshRenderer[] arrayCarrySword;
    protected float DODGE_ACTION_TIME = 0.35f;
    protected float DODGE_COOL_TIME = 0.6f;
    private bool isDodgeJump;
    private float dodgeTime;
    private static readonly float HP_MAX = 100f;
    private float hp = HP_MAX;
    private Vector3 blowVec;
    private GameObject opponent;
    private Vector3 jumpTarget;
    private bool isJumpInput;
    private float backTime;
    private Vector3 knockBackDebugDir = Vector3.zero;
    private SwordFight_Define.AiStrength aiStrength;
    private float STOP_CHECK_DISTANCE = 1.1f;
    private float AVOID_ROT = 15f;
    private float runTime;
    private string AiState;
    private float freezeTime;
    private float invincibleTime;
    private float repelTime;
    private float RUN_SPEED_DATA;
    private float runSpeed;
    private float ATTACK_INTERVAL_DATA;
    private int DEFFENCE_PER_DATA;
    private int DEFFENCE_COUNTER_PER_DATA;
    private SwordFight_CharacterScript targetCharacter;
    private float searchVisinityRange = 15f;
    [SerializeField]
    private float frontSearchCharacterRange = 5f;
    private float currentDeffenceTime;
    private bool aiContinusAttack_2nd;
    private bool aiContinusAttack_Last;
    private int[] randomAttackNum = new int[3];
    private float nowSearchAngle;
    public DebugShowData debugShowData = new DebugShowData(_searchVicinityCharacter: false, _frontSearchCharacter: false, _transformRightDir: false, _knockBackDir: false);
    [Header("AI行動")]
    public string debugActionState;
    public bool IsCpu => playerNo >= SingletonCustom<GameSettingManager>.Instance.PlayerNum;
    public float HpScale => hp / HP_MAX;
    public bool IsJumpInput => isJumpInput;
    public bool IsMoveDown => rigid.velocity.y < 0f;
    public int PlayerNo {
        get {
            return playerNo;
        }
        set {
            playerNo = value;
        }
    }
    public int CharaNo {
        get {
            return charaNo;
        }
        set {
            charaNo = value;
        }
    }
    public int TeamNo {
        get {
            return teamNo;
        }
        set {
            teamNo = value;
        }
    }
    public int NowUseDeffenceCount => nowUseDeffenceCount;
    public bool IsFirstAttack => isFirstAttack;
    public bool IsSecondAttack => isSecondAttack;
    public bool IsLastAttack => isLastAttack;
    protected SwordFight_MainCharacterManager MCM => SingletonCustom<SwordFight_MainCharacterManager>.Instance;
    protected SwordFight_CpuArtificialIntelligence CpuAi => SingletonCustom<SwordFight_MainCharacterManager>.Instance.CpuAi;
    protected SwordFight_FieldManager FM => SingletonCustom<SwordFight_FieldManager>.Instance;
    public void GameStartInit(int _charaNo, int _playerNo, int _teamNo, Transform _originAnchor, float _reAttackTime) {
        rigid.maxAngularVelocity = 490f;
        charaBodySize = charaCollider.radius * obj.localScale.x * 2f;
        charaHeight = charaCollider.height * obj.localScale.y;
        charaNo = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[_playerNo];
        UnityEngine.Debug.Log("キャラ番号：" + charaNo.ToString());
        playerNo = _playerNo;
        UnityEngine.Debug.Log("プレイヤ\u30fc番号：" + charaNo.ToString());
        teamNo = _teamNo;
        ChangeUniform();
        nowUseDeffenceCount = 0;
        runAnimationTime = 0f;
        playSeRunCnt = 0;
        originAnchor = _originAnchor;
        base.transform.localPosition = originAnchor.localPosition;
        defPos = (nowPos = (prevPos = base.transform.position));
        base.transform.rotation = originAnchor.rotation;
        SettingLayer("Character");
        SettingCharaParameter();
        SettingGameStartPos(_originAnchor.position);
        InvincibleAnimationStop();
        isWinnerPlayer = false;
        CharacterFaceChange_Normal();
        reAttackTimeValueMax = 0.25f;
        hp = HP_MAX;
        sword.Init();
        runEffect.Clear();
        UnityEngine.Debug.Log(base.gameObject.name + "のゲ\u30fcム開始設定を初期化");
        base.transform.SetLocalEulerAnglesX(0f);
    }
    public void RoundStartInit() {
        if (!SwordFight_Define.IS_TEAM_MODE && SingletonCustom<GameSettingManager>.Instance.IsSinglePlay && !SingletonCustom<SwordFight_MainCharacterManager>.Instance.IsPlayer(playerNo)) {
            ChangeRivalUniform(SwordFight_Define.GetNowWinningNum());
        }
        nowUseDeffenceCount = 0;
        runAnimationTime = 0f;
        playSeRunCnt = 0;
        base.transform.localPosition = originAnchor.localPosition;
        defPos = (nowPos = (prevPos = base.transform.position));
        base.transform.rotation = originAnchor.rotation;
        SettingGameStartPos(originAnchor.position);
        InvincibleAnimationStop();
        isWinnerPlayer = false;
        CharacterFaceChange_Normal();
        UnityEngine.Debug.Log(base.gameObject.name + "のラウンド開始設定を初期化");
    }
    public void SetOpponent() {
        for (int i = 0; i < SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList.Length; i++) {
            if (SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList[i] != this) {
                opponent = SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList[i].gameObject;
            }
        }
    }
    public void RoundContinueStartInit() {
        nowUseDeffenceCount = 0;
        runAnimationTime = 0f;
        playSeRunCnt = 0;
        base.transform.localPosition = originAnchor.localPosition;
        defPos = (nowPos = (prevPos = base.transform.position));
        base.transform.rotation = originAnchor.rotation;
        SettingGameStartPos(originAnchor.position);
        InvincibleAnimationStop();
        UnityEngine.Debug.Log(base.gameObject.name + "のラウンド続行設定を初期化");
    }
    public void UpdateMethod() {
        triggerCheckInterval -= Time.deltaTime;
        UpdateDebugActionState();
        prevPos = nowPos;
        nowPos = base.transform.position;
        if (rigid.velocity.sqrMagnitude >= 100f) {
            rigid.velocity *= 0f;
        }
        if (SingletonCustom<SwordFight_MainGameManager>.Instance.IsAnimationDebugMode) {
            if (isStartAnimation) {
                isStartAnimation = false;
                isDuringAnimation = isLoopAnimationMode;
                attackAnimationTime = motionData.GetMotionData(motionType).motionTime;
                actionState = ActionState.ATTACK;
                MotionAnimation(motionData.GetMotionData(motionType), null);
            }
            if (isStopAnimation) {
                isStopAnimation = false;
                isDuringAnimation = false;
                actionState = ActionState.STANDARD;
                ResetCharaMotion();
            }
            if (isLoadMotionData) {
                isLoadMotionData = false;
                SetStartMotionData(motionData.GetMotionData(loadMotionType));
            }
        }
        if (attackAnimationTime > 0f) {
            attackAnimationTime -= Time.deltaTime;
            return;
        }
        if (SingletonCustom<SwordFight_MainGameManager>.Instance.IsAnimationDebugMode) {
            if (isDuringAnimation) {
                actionState = ActionState.ATTACK;
                ResetCharaMotion();
                MotionAnimation(motionData.GetMotionData(motionType), null);
            }
        } else {
            switch (actionState) {
                case ActionState.ATTACK:
                    if (attackAnimationTime <= 0f && (isFirstAttack || isSecondAttack || isLastAttack)) {
                        currentContinusAttackInputTime += Time.deltaTime;
                        if (currentContinusAttackInputTime > 0.2f) {
                            currentContinusAttackInputTime = 0f;
                            actionState = ActionState.STANDARD;
                            reAttackTimeValue = 0f;
                            ResetCharaMotion();
                        }
                        return;
                    }
                    break;
                case ActionState.FREEZE:
                    sword.DisableTrail();
                    AiFreeze();
                    break;
                case ActionState.REPEL:
                    AiRepel();
                    break;
                case ActionState.DEATH:
                    AiDeath();
                    blowVec = rigid.velocity;
                    break;
                case ActionState.DODGE:
                    DodgeAction();
                    break;
            }
        }
        reAttackTimeValue += Time.deltaTime;
        controlInterval += Time.deltaTime;
        runSeInterval -= Time.deltaTime;
        CalcManager.mCalcVector3 = opponent.transform.position;
        CalcManager.mCalcVector3.y = base.transform.position.y;
        base.transform.LookAt(CalcManager.mCalcVector3);
        rigid.MoveRotation(Quaternion.LookRotation(CalcManager.mCalcVector3 - base.transform.position));
    }
    public void FixedUpdate() {
        SetLocalGravity();
    }
    private void SetLocalGravity() {
        rigid.AddForce(localGravity, ForceMode.Acceleration);
    }
    protected virtual void DodgeAction() {
        actionInterval -= Time.deltaTime;
        DodgeAnimation();
        if (actionInterval <= 0f) {
            ResetObjPosition();
            actionState = ActionState.STANDARD;
        }
    }
    public void Dodge(Vector3 _dir) {
        if ((!CheckActionState(ActionState.STANDARD) && !CheckActionState(ActionState.MOVE)) || Time.time - dodgeTime < DODGE_COOL_TIME) {
            return;
        }
        UnityEngine.Debug.Log("回避");
        dodgeTime = Time.time + DODGE_ACTION_TIME;
        ResetObjPosition();
        ResetMotion();
        rigid.isKinematic = false;
        isDodgeJump = false;
        bool num = Mathf.Abs(_dir.x) < 0.1f && Mathf.Abs(_dir.z) < 0.1f;
        SettingLayer("DodgeCharacter");
        rigid.constraints = (RigidbodyConstraints)84;
        obj.parent = dodgeAnchor;
        if (!num) {
            Move(_dir, 1f, 1f, _moveRot: false);
            dodgeAnchor.SetLocalEulerAnglesZ(90f);
            obj.SetLocalPosition(0f, -0.7f, 0f);
            LeanTween.value(0f, 1f, DODGE_ACTION_TIME).setOnUpdate((Action<float>)delegate {
                Move(_dir, 1.25f, 1f, _moveRot: false, _isStateChange: false);
            });
            LeanTween.rotateAroundLocal(dodgeAnchor.gameObject, Vector3.up, -270f, 0f);
            LeanTween.rotateAroundLocal(dodgeAnchor.gameObject, Vector3.up, -180f, DODGE_ACTION_TIME).setEaseLinear().setOnComplete((Action)delegate {
                SettingLayer("Character");
                rigid.constraints = (RigidbodyConstraints)80;
                dodgeAnchor.SetLocalEulerAngles(0f, 0f, 0f);
                obj.parent = objPivot;
                obj.SetLocalPosition(0f, 0f, 0f);
            });
            MoveRot(_dir, _immediate: true);
            actionInterval = DODGE_ACTION_TIME;
        } else {
            bool flag = UnityEngine.Random.Range(0, 2) == 1;
            flag = false;
            foreach (Transform item in obj) {
                if (item.gameObject.name == "Arm_L_Anchor" || item.gameObject.name == "Arm_R_Anchor") {
                    item.SetLocalEulerAngles(0f, 0f, 0f);
                }
            }
            sword.transform.SetLocalEulerAngles(45f, 180f, 180f);
            if (flag) {
                isDodgeJump = true;
                rigid.constraints = (RigidbodyConstraints)80;
                rigid.AddForce(base.transform.up * 400f, ForceMode.Impulse);
                actionInterval = 0.9f;
            } else {
                LeanTween.rotateAround(obj.gameObject, Vector3.up, 360f, 0.6f).setOnComplete((Action)delegate {
                    SettingLayer("Character");
                    rigid.constraints = (RigidbodyConstraints)80;
                    dodgeAnchor.SetLocalEulerAngles(0f, 0f, 0f);
                    obj.parent = objPivot;
                    obj.SetLocalPosition(0f, 0f, 0f);
                });
                actionInterval = 0.6f;
            }
        }
        SingletonCustom<AudioManager>.Instance.SePlay("se_snowball_steal");
        actionState = ActionState.DODGE;
    }
    private void DodgeAnimation() {
        bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAnglesX(0f);
        bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAnglesX(0f);
        if (isDodgeJump) {
            bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(0f, 0f, 225f + Mathf.Sin((actionInterval - 0.2f) * (float)Math.PI * 2f) * 35f);
            bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(0f, 0f, 135f - Mathf.Sin((actionInterval - 0.2f) * (float)Math.PI * 2f) * 35f);
        } else {
            bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(0f, 0f, 180f);
            bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(0f, 0f, 180f);
        }
        bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, 0f);
        bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(0f, 0f, 0f);
    }
    protected void ResetMotion() {
        bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(0f, 0f, 0f);
        bodyParts.Parts(BodyPartsList.HIP).SetLocalPosition(0f, 0f, 0f);
        bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(0f, 0f, 0f);
        bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(0f, 0f, 0f);
        bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(0f, 0f, 0f);
        bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(0f, 0f, 0f);
        bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(0f, 0f, 0f);
        bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, 0f);
        bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(0f, 0f, 0f);
    }
    public void ResetObjPosition() {
        obj.parent = objPivot;
        obj.SetLocalPosition(0f, 0f, 0f);
        obj.SetLocalEulerAngles(0f, 0f, 0f);
        base.transform.SetEulerAngles(0f, base.transform.rotation.eulerAngles.y, 0f);
        sword.transform.SetLocalEulerAngles(0f, 0f, 0f);
        ResetMotion();
        if (isJumpInput) {
            rigid.constraints = RigidbodyConstraints.None;
        } else {
            rigid.constraints = (RigidbodyConstraints)80;
        }
    }
    private void ChangeUniform() {
        headgearMeshRenderer.material = playerMaterial[charaNo];
        swordMeshRenderer.material = playerMaterial[charaNo];
        dropSwordMeshRenderer.material = playerMaterial[charaNo];
        charaStyle.SetGameStyle(GS_Define.GameType.BOMB_ROULETTE, playerNo);
        if (defaultCarrySword.activeSelf) {
            defaultCarrySword.SetActive(value: false);
            for (int i = 0; i < arrayCarrySword.Length; i++) {
                arrayCarrySword[i].material = charaStyle.StyleMat;
                arrayCarrySword[i].gameObject.SetActive(value: true);
            }
        }
    }
    private void ChangeRivalUniform(int _defeatedNumber) {
        List<int> list = new List<int>();
        for (int i = 0; i < SingletonCustom<SwordFight_MainCharacterManager>.Instance.UniformTexNoArray.Length; i++) {
            list.Add(SingletonCustom<SwordFight_MainCharacterManager>.Instance.UniformTexNoArray[i].texNo);
        }
        switch (aiStrength) {
        }
    }
    public void AddGravity(float _mag = 1f) {
    }
    public void Wait() {
        if (actionState != ActionState.ATTACK) {
            actionState = ActionState.STANDARD;
        }
        rigid.velocity = new Vector3(0f, rigid.velocity.y, 0f);
    }
    public void Move(Vector3 _moveDir, float _moveSpeed, float _speedMag = 1f, bool _moveRot = true, bool _isStateChange = true) {
        if (!(Time.timeScale < 1f)) {
            if (_isStateChange && actionState != ActionState.ATTACK) {
                actionState = ActionState.MOVE;
            }
            Vector3 a = _moveDir;
            a.y = 0f;
            if (actionState == ActionState.ATTACK) {
                _moveSpeed = 0.2f;
            } else if (_moveSpeed <= 0.3f) {
                _moveSpeed = 0.3f;
            } else if (_moveSpeed <= 0.6f) {
                _moveSpeed = 0.6f;
            } else if (!isJumpInput && Time.frameCount % 5 == 0) {
                runEffect.Emit(1);
            }
            a *= moveSpeed * _moveSpeed * _speedMag;
            rigid.AddForce(a + Vector3.down, ForceMode.Acceleration);
            if (rigid.velocity.magnitude > moveSpeedMax * _speedMag) {
                rigid.velocity = new Vector3(rigid.velocity.normalized.x * moveSpeedMax * _speedMag, rigid.velocity.y, rigid.velocity.normalized.z * moveSpeedMax * _speedMag);
            }
            if (_moveRot) {
                MoveRot(_moveDir);
            }
            MoveAnimation();
            CalcManager.mCalcVector3 = opponent.transform.position;
            CalcManager.mCalcVector3.y = base.transform.position.y;
            base.transform.LookAt(CalcManager.mCalcVector3);
            rigid.MoveRotation(Quaternion.LookRotation(CalcManager.mCalcVector3 - base.transform.position));
        }
    }
    private void MoveRot(Vector3 _moveDir, bool _immediate = false) {
        if (CalcManager.Length(nowPos, prevPos) > 0.01f) {
            calcVec[0] = _moveDir;
            rot.x = 0f;
            rot.y = CalcManager.Rot(calcVec[0], CalcManager.AXIS.Y);
            rot.z = 0f;
            if (_immediate) {
                rigid.MoveRotation(Quaternion.Euler(rot));
            } else {
                rigid.MoveRotation(Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(rot), 20f * Time.deltaTime));
            }
        }
    }
    public void VerticalSlashAnimation() {
        if (!(attackAnimationTime > 0f) && CheckAttackAllow() && (!isFirstAttack || !isSecondAttack || !isLastAttack)) {
            actionState = ActionState.ATTACK;
            if (!isFirstAttack) {
                VerticalSlash_1st_SwingStandby();
            } else if (!isSecondAttack) {
                VerticalSlash_2nd_SwingStandby();
                currentContinusAttackInputTime = 0f;
            } else if (!isLastAttack) {
                VerticalSlash_Last_SwingStandby();
                currentContinusAttackInputTime = 0f;
            }
        }
    }
    private void VerticalSlash_1st_SwingStandby() {
        attackAnimationTime = motionData.GetMotionData(SwordFight_MotionData.MotionType.VerticalSlash_1st_SwingStandby).motionTime;
        MotionAnimation(motionData.GetMotionData(SwordFight_MotionData.MotionType.VerticalSlash_1st_SwingStandby), VerticalSlash_1st_Swing);
    }
    private void VerticalSlash_1st_Swing() {
        rigid.constraints = (RigidbodyConstraints)80;
        sword.SetVerticalSwingFlg(_swingFlg: true);
        SingletonCustom<AudioManager>.Instance.SePlay("se_sword_fight_swing");
        attackAnimationTime = motionData.GetMotionData(SwordFight_MotionData.MotionType.VerticalSlash_1st_Slash).motionTime;
        MotionAnimation(motionData.GetMotionData(SwordFight_MotionData.MotionType.VerticalSlash_1st_Slash), delegate {
            LeanTween.delayedCall(base.gameObject, 0.14f, (Action)delegate {
                if (attackAnimationTime <= 0f) {
                    sword.SetVerticalSwingFlg(_swingFlg: false);
                    sword.SetHorizontalLeftSwingFlg(_swingFlg: false);
                    sword.SetHorizontalRightSwingFlg(_swingFlg: false);
                }
            });
            isFirstAttack = true;
        });
    }
    private void VerticalSlash_2nd_SwingStandby() {
        attackAnimationTime = motionData.GetMotionData(SwordFight_MotionData.MotionType.VerticalSlash_2nd_SwingStandby).motionTime;
        MotionAnimation(motionData.GetMotionData(SwordFight_MotionData.MotionType.VerticalSlash_2nd_SwingStandby), VerticalSlash_2nd_Swing);
    }
    private void VerticalSlash_2nd_Swing() {
        rigid.constraints = (RigidbodyConstraints)80;
        sword.SetVerticalSwingFlg(_swingFlg: true);
        SingletonCustom<AudioManager>.Instance.SePlay("se_sword_fight_swing", _loop: false, 0f, 1f, 1.2f);
        attackAnimationTime = motionData.GetMotionData(SwordFight_MotionData.MotionType.VerticalSlash_2nd_Slash).motionTime;
        MotionAnimation(motionData.GetMotionData(SwordFight_MotionData.MotionType.VerticalSlash_2nd_Slash), delegate {
            LeanTween.delayedCall(base.gameObject, 0.15f, (Action)delegate {
                if (attackAnimationTime <= 0f) {
                    sword.SetVerticalSwingFlg(_swingFlg: false);
                    sword.SetHorizontalLeftSwingFlg(_swingFlg: false);
                    sword.SetHorizontalRightSwingFlg(_swingFlg: false);
                }
            });
            isSecondAttack = true;
        });
    }
    private void VerticalSlash_Last_SwingStandby() {
        attackAnimationTime = motionData.GetMotionData(SwordFight_MotionData.MotionType.VerticalSlash_Last_SwingStandby).motionTime;
        MotionAnimation(motionData.GetMotionData(SwordFight_MotionData.MotionType.VerticalSlash_Last_SwingStandby), VerticalSlash_Last_Swing);
    }
    private void VerticalSlash_Last_Swing() {
        rigid.constraints = (RigidbodyConstraints)80;
        sword.SetVerticalSwingFlg(_swingFlg: true);
        SingletonCustom<AudioManager>.Instance.SePlay("se_sword_fight_swing", _loop: false, 0f, 1f, 1.4f);
        attackAnimationTime = motionData.GetMotionData(SwordFight_MotionData.MotionType.VerticalSlash_Last_Slash).motionTime;
        MotionAnimation(motionData.GetMotionData(SwordFight_MotionData.MotionType.VerticalSlash_Last_Slash), delegate {
            LeanTween.delayedCall(base.gameObject, 0.15f, (Action)delegate {
                if (attackAnimationTime <= 0f) {
                    sword.SetVerticalSwingFlg(_swingFlg: false);
                    sword.SetHorizontalLeftSwingFlg(_swingFlg: false);
                    sword.SetHorizontalRightSwingFlg(_swingFlg: false);
                }
            });
            isLastAttack = true;
        });
    }
    public void HorizontalRightSlashAnimation() {
        if (!(attackAnimationTime > 0f) && CheckAttackAllow() && (!isFirstAttack || !isSecondAttack || !isLastAttack)) {
            actionState = ActionState.ATTACK;
            if (!isFirstAttack) {
                HorizontalRightSlash_1st_SwingStandby();
            } else if (!isSecondAttack) {
                HorizontalRightSlash_2nd_SwingStandby();
                currentContinusAttackInputTime = 0f;
            } else if (!isLastAttack) {
                HorizontalRightSlash_Last_SwingStandby();
                currentContinusAttackInputTime = 0f;
            }
        }
    }
    private void HorizontalRightSlash_1st_SwingStandby() {
        attackAnimationTime = motionData.GetMotionData(SwordFight_MotionData.MotionType.HorizontalSlash_1st_SwingStandby).motionTime;
        MotionAnimation(motionData.GetMotionData(SwordFight_MotionData.MotionType.HorizontalSlash_1st_SwingStandby), HorizontalRightSlash_1st_Swing);
    }
    private void HorizontalRightSlash_1st_Swing() {
        rigid.constraints = (RigidbodyConstraints)80;
        sword.SetHorizontalRightSwingFlg(_swingFlg: true);
        SingletonCustom<AudioManager>.Instance.SePlay("se_sword_fight_swing");
        attackAnimationTime = motionData.GetMotionData(SwordFight_MotionData.MotionType.HorizontalSlash_1st_Slash).motionTime;
        MotionAnimation(motionData.GetMotionData(SwordFight_MotionData.MotionType.HorizontalSlash_1st_Slash), delegate {
            sword.SetVerticalSwingFlg(_swingFlg: false);
            sword.SetHorizontalLeftSwingFlg(_swingFlg: false);
            sword.SetHorizontalRightSwingFlg(_swingFlg: false);
            isFirstAttack = true;
        });
    }
    private void HorizontalRightSlash_2nd_SwingStandby() {
        attackAnimationTime = motionData.GetMotionData(SwordFight_MotionData.MotionType.HorizontalSlash_2nd_SwingStandby).motionTime;
        MotionAnimation(motionData.GetMotionData(SwordFight_MotionData.MotionType.HorizontalSlash_2nd_SwingStandby), HorizontalRightSlash_2nd_Swing);
    }
    private void HorizontalRightSlash_2nd_Swing() {
        rigid.constraints = (RigidbodyConstraints)80;
        sword.SetHorizontalRightSwingFlg(_swingFlg: true);
        SingletonCustom<AudioManager>.Instance.SePlay("se_sword_fight_swing", _loop: false, 0f, 1f, 1.2f);
        attackAnimationTime = motionData.GetMotionData(SwordFight_MotionData.MotionType.HorizontalSlash_2nd_Slash).motionTime;
        MotionAnimation(motionData.GetMotionData(SwordFight_MotionData.MotionType.HorizontalSlash_2nd_Slash), delegate {
            sword.SetVerticalSwingFlg(_swingFlg: false);
            sword.SetHorizontalLeftSwingFlg(_swingFlg: false);
            sword.SetHorizontalRightSwingFlg(_swingFlg: false);
            isSecondAttack = true;
        });
    }
    private void HorizontalRightSlash_Last_SwingStandby() {
        attackAnimationTime = motionData.GetMotionData(SwordFight_MotionData.MotionType.HorizontalSlash_Last_SwingStandby).motionTime;
        MotionAnimation(motionData.GetMotionData(SwordFight_MotionData.MotionType.HorizontalSlash_Last_SwingStandby), HorizontalRightSlash_Last_Swing);
    }
    private void HorizontalRightSlash_Last_Swing() {
        rigid.constraints = (RigidbodyConstraints)80;
        sword.SetHorizontalRightSwingFlg(_swingFlg: true);
        SingletonCustom<AudioManager>.Instance.SePlay("se_sword_fight_swing", _loop: false, 0f, 1f, 1.4f);
        attackAnimationTime = motionData.GetMotionData(SwordFight_MotionData.MotionType.HorizontalSlash_Last_Slash).motionTime;
        MotionAnimation(motionData.GetMotionData(SwordFight_MotionData.MotionType.HorizontalSlash_Last_Slash), delegate {
            sword.SetVerticalSwingFlg(_swingFlg: false);
            sword.SetHorizontalLeftSwingFlg(_swingFlg: false);
            sword.SetHorizontalRightSwingFlg(_swingFlg: false);
            isLastAttack = true;
        });
    }
    private void ResetAttackData() {
        if (actionState != ActionState.FREEZE && actionState != ActionState.REPEL && actionState != ActionState.DEATH) {
            sword.SetVerticalSwingFlg(_swingFlg: false);
            sword.SetHorizontalLeftSwingFlg(_swingFlg: false);
            sword.SetHorizontalRightSwingFlg(_swingFlg: false);
            isFirstAttack = false;
            isSecondAttack = false;
            isLastAttack = false;
        }
    }
    private void SetStartMotionData(SwordFight_MotionData.MotionData _motionData) {
        bodyParts.Parts(BodyPartsList.HEAD).localPosition = _motionData.pose_HEAD.pose_Pos;
        bodyParts.Parts(BodyPartsList.HEAD).localEulerAngles = _motionData.pose_HEAD.pose_Angle;
        bodyParts.Parts(BodyPartsList.HEAD).localScale = _motionData.pose_HEAD.pose_Scale;
        bodyParts.Parts(BodyPartsList.BODY).localPosition = _motionData.pose_BODY.pose_Pos;
        bodyParts.Parts(BodyPartsList.BODY).localEulerAngles = _motionData.pose_BODY.pose_Angle;
        bodyParts.Parts(BodyPartsList.BODY).localScale = _motionData.pose_BODY.pose_Scale;
        bodyParts.Parts(BodyPartsList.HIP).localPosition = _motionData.pose_HIP.pose_Pos;
        bodyParts.Parts(BodyPartsList.HIP).localEulerAngles = _motionData.pose_HIP.pose_Angle;
        bodyParts.Parts(BodyPartsList.HIP).localScale = _motionData.pose_HIP.pose_Scale;
        bodyParts.Parts(BodyPartsList.SHOULDER_L).localPosition = _motionData.pose_SHOULDER_L.pose_Pos;
        bodyParts.Parts(BodyPartsList.SHOULDER_L).localEulerAngles = _motionData.pose_SHOULDER_L.pose_Angle;
        bodyParts.Parts(BodyPartsList.SHOULDER_L).localScale = _motionData.pose_SHOULDER_L.pose_Scale;
        bodyParts.Parts(BodyPartsList.SHOULDER_R).localPosition = _motionData.pose_SHOULDER_R.pose_Pos;
        bodyParts.Parts(BodyPartsList.SHOULDER_R).localEulerAngles = _motionData.pose_SHOULDER_R.pose_Angle;
        bodyParts.Parts(BodyPartsList.SHOULDER_R).localScale = _motionData.pose_SHOULDER_R.pose_Scale;
        bodyParts.Parts(BodyPartsList.ARM_L).localPosition = _motionData.pose_ARM_L.pose_Pos;
        bodyParts.Parts(BodyPartsList.ARM_L).localEulerAngles = _motionData.pose_ARM_L.pose_Angle;
        bodyParts.Parts(BodyPartsList.ARM_L).localScale = _motionData.pose_ARM_L.pose_Scale;
        bodyParts.Parts(BodyPartsList.ARM_R).localPosition = _motionData.pose_ARM_R.pose_Pos;
        bodyParts.Parts(BodyPartsList.ARM_R).localEulerAngles = _motionData.pose_ARM_R.pose_Angle;
        bodyParts.Parts(BodyPartsList.ARM_R).localScale = _motionData.pose_ARM_R.pose_Scale;
        bodyParts.Parts(BodyPartsList.LEG_L).localPosition = _motionData.pose_LEG_L.pose_Pos;
        bodyParts.Parts(BodyPartsList.LEG_L).localEulerAngles = _motionData.pose_LEG_L.pose_Angle;
        bodyParts.Parts(BodyPartsList.LEG_L).localScale = _motionData.pose_LEG_L.pose_Scale;
        bodyParts.Parts(BodyPartsList.LEG_R).localPosition = _motionData.pose_LEG_R.pose_Pos;
        bodyParts.Parts(BodyPartsList.LEG_R).localEulerAngles = _motionData.pose_LEG_R.pose_Angle;
        bodyParts.Parts(BodyPartsList.LEG_R).localScale = _motionData.pose_LEG_R.pose_Scale;
        sword.transform.localPosition = _motionData.pose_SWORD.pose_Pos;
        sword.transform.localEulerAngles = _motionData.pose_SWORD.pose_Angle;
        sword.transform.localScale = _motionData.pose_SWORD.pose_Scale;
    }
    private void MotionAnimation(SwordFight_MotionData.MotionData _motionData, Action _callback) {
        if (actionState != ActionState.ATTACK) {
            StopMotionAnimation();
            return;
        }
        LeanTween.moveLocal(bodyParts.Parts(BodyPartsList.HEAD).gameObject, _motionData.pose_HEAD.pose_Pos, attackAnimationTime);
        LeanTween.rotateLocal(bodyParts.Parts(BodyPartsList.HEAD).gameObject, _motionData.pose_HEAD.pose_Angle, attackAnimationTime);
        LeanTween.scale(bodyParts.Parts(BodyPartsList.HEAD).gameObject, _motionData.pose_HEAD.pose_Scale, attackAnimationTime);
        LeanTween.moveLocal(bodyParts.Parts(BodyPartsList.BODY).gameObject, _motionData.pose_BODY.pose_Pos, attackAnimationTime);
        LeanTween.rotateLocal(bodyParts.Parts(BodyPartsList.BODY).gameObject, _motionData.pose_BODY.pose_Angle, attackAnimationTime);
        LeanTween.scale(bodyParts.Parts(BodyPartsList.BODY).gameObject, _motionData.pose_BODY.pose_Scale, attackAnimationTime);
        LeanTween.moveLocal(bodyParts.Parts(BodyPartsList.HIP).gameObject, _motionData.pose_HIP.pose_Pos, attackAnimationTime);
        LeanTween.rotateLocal(bodyParts.Parts(BodyPartsList.HIP).gameObject, _motionData.pose_HIP.pose_Angle, attackAnimationTime);
        LeanTween.scale(bodyParts.Parts(BodyPartsList.HIP).gameObject, _motionData.pose_HIP.pose_Scale, attackAnimationTime);
        LeanTween.moveLocal(bodyParts.Parts(BodyPartsList.SHOULDER_L).gameObject, _motionData.pose_SHOULDER_L.pose_Pos, attackAnimationTime);
        LeanTween.rotateLocal(bodyParts.Parts(BodyPartsList.SHOULDER_L).gameObject, _motionData.pose_SHOULDER_L.pose_Angle, attackAnimationTime);
        LeanTween.scale(bodyParts.Parts(BodyPartsList.SHOULDER_L).gameObject, _motionData.pose_SHOULDER_L.pose_Scale, attackAnimationTime);
        LeanTween.moveLocal(bodyParts.Parts(BodyPartsList.SHOULDER_R).gameObject, _motionData.pose_SHOULDER_R.pose_Pos, attackAnimationTime);
        LeanTween.rotateLocal(bodyParts.Parts(BodyPartsList.SHOULDER_R).gameObject, _motionData.pose_SHOULDER_R.pose_Angle, attackAnimationTime);
        LeanTween.scale(bodyParts.Parts(BodyPartsList.SHOULDER_R).gameObject, _motionData.pose_SHOULDER_R.pose_Scale, attackAnimationTime);
        LeanTween.moveLocal(bodyParts.Parts(BodyPartsList.ARM_L).gameObject, _motionData.pose_ARM_L.pose_Pos, attackAnimationTime);
        LeanTween.rotateLocal(bodyParts.Parts(BodyPartsList.ARM_L).gameObject, _motionData.pose_ARM_L.pose_Angle, attackAnimationTime);
        LeanTween.scale(bodyParts.Parts(BodyPartsList.ARM_L).gameObject, _motionData.pose_ARM_L.pose_Scale, attackAnimationTime);
        LeanTween.moveLocal(bodyParts.Parts(BodyPartsList.ARM_R).gameObject, _motionData.pose_ARM_R.pose_Pos, attackAnimationTime);
        LeanTween.rotateLocal(bodyParts.Parts(BodyPartsList.ARM_R).gameObject, _motionData.pose_ARM_R.pose_Angle, attackAnimationTime);
        LeanTween.scale(bodyParts.Parts(BodyPartsList.ARM_R).gameObject, _motionData.pose_ARM_R.pose_Scale, attackAnimationTime);
        LeanTween.moveLocal(bodyParts.Parts(BodyPartsList.LEG_L).gameObject, _motionData.pose_LEG_L.pose_Pos, attackAnimationTime);
        LeanTween.rotateLocal(bodyParts.Parts(BodyPartsList.LEG_L).gameObject, _motionData.pose_LEG_L.pose_Angle, attackAnimationTime);
        LeanTween.scale(bodyParts.Parts(BodyPartsList.LEG_L).gameObject, _motionData.pose_LEG_L.pose_Scale, attackAnimationTime);
        LeanTween.moveLocal(bodyParts.Parts(BodyPartsList.LEG_R).gameObject, _motionData.pose_LEG_R.pose_Pos, attackAnimationTime);
        LeanTween.rotateLocal(bodyParts.Parts(BodyPartsList.LEG_R).gameObject, _motionData.pose_LEG_R.pose_Angle, attackAnimationTime);
        LeanTween.scale(bodyParts.Parts(BodyPartsList.LEG_R).gameObject, _motionData.pose_LEG_R.pose_Scale, attackAnimationTime);
        LeanTween.moveLocal(sword.gameObject, _motionData.pose_SWORD.pose_Pos, attackAnimationTime);
        LeanTween.rotateLocal(sword.gameObject, _motionData.pose_SWORD.pose_Angle, attackAnimationTime);
        LeanTween.scale(sword.gameObject, _motionData.pose_SWORD.pose_Scale, attackAnimationTime);
        if (actionState == ActionState.ATTACK) {
            if (_callback != null) {
                LeanTween.delayedCall(attackAnimationTime, _callback);
            }
        } else {
            StopMotionAnimation();
        }
    }
    private void StopMotionAnimation() {
        LeanTween.cancel(bodyParts.Parts(BodyPartsList.HEAD).gameObject);
        LeanTween.cancel(bodyParts.Parts(BodyPartsList.BODY).gameObject);
        LeanTween.cancel(bodyParts.Parts(BodyPartsList.HIP).gameObject);
        LeanTween.cancel(bodyParts.Parts(BodyPartsList.SHOULDER_L).gameObject);
        LeanTween.cancel(bodyParts.Parts(BodyPartsList.SHOULDER_R).gameObject);
        LeanTween.cancel(bodyParts.Parts(BodyPartsList.ARM_L).gameObject);
        LeanTween.cancel(bodyParts.Parts(BodyPartsList.ARM_R).gameObject);
        LeanTween.cancel(bodyParts.Parts(BodyPartsList.LEG_L).gameObject);
        LeanTween.cancel(bodyParts.Parts(BodyPartsList.LEG_R).gameObject);
        LeanTween.cancel(sword.gameObject);
    }
    private void MoveAnimation() {
        bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAnglesX(Mathf.Sin(runAnimationTime * (float)Math.PI * 2f) * 30f);
        bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAnglesX(Mathf.Sin(runAnimationTime * (float)Math.PI * 2f) * -30f);
        if (CalcManager.Length(nowPos, prevPos) > 0.01f) {
            runAnimationTime += CalcManager.Length(nowPos, prevPos) * runAnimationSpeed * Time.deltaTime;
            if (runAnimationTime >= (float)playSeRunCnt * 0.5f) {
                playSeRunCnt++;
                PlaySeRun();
            }
            if (runAnimationTime >= 1f) {
                runAnimationTime = 0f;
                playSeRunCnt = 1;
            }
            isChangeAnimationNeutral = false;
        } else {
            if (isChangeAnimationNeutral) {
                return;
            }
            if (runAnimationTime <= 0.25f) {
                runAnimationTime -= 1f * Time.deltaTime;
                if (runAnimationTime <= 0f) {
                    runAnimationTime = 0f;
                    isChangeAnimationNeutral = true;
                }
            } else if (runAnimationTime <= 0.5f) {
                runAnimationTime += 1f * Time.deltaTime;
                if (runAnimationTime >= 0.5f) {
                    runAnimationTime = 0.5f;
                    isChangeAnimationNeutral = true;
                }
            } else if (runAnimationTime <= 0.75f) {
                runAnimationTime -= 1f * Time.deltaTime;
                if (runAnimationTime <= 0.5f) {
                    runAnimationTime = 0.5f;
                    isChangeAnimationNeutral = true;
                }
            } else {
                runAnimationTime += 1f * Time.deltaTime;
                if (runAnimationTime >= 1f) {
                    runAnimationTime = 1f;
                    isChangeAnimationNeutral = true;
                }
            }
        }
    }
    private void PlaySeRun() {
        if (MCM.CheckControlChara(this)) {
            SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 0.2f);
        }
    }
    public void DeffenceAnimation() {
        actionState = ActionState.DEFENCE;
        nowUseDeffenceCount++;
        damageCount = 0;
        Deffence();
    }
    public void DeffenceRotateAnimation(Vector3 _moveDir) {
        if (_moveDir.x != 0f || _moveDir.y != 0f || _moveDir.z != 0f) {
            calcVec[0] = _moveDir;
            rot.x = 0f;
            rot.y = CalcManager.Rot(calcVec[0], CalcManager.AXIS.Y);
            rot.z = 0f;
            rigid.MoveRotation(Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(rot), 20f * Time.deltaTime));
        }
    }
    private void Deffence() {
        ResetCharaMotion();
        SetStartMotionData(motionData.GetMotionData(SwordFight_MotionData.MotionType.Deffence));
    }
    public void ResetDeffenceMotion() {
        if (actionState != ActionState.FREEZE && actionState != ActionState.REPEL) {
            ResetCharaMotion();
            actionState = ActionState.STANDARD;
        }
    }
    public bool CheckUseDeffence() {
        return nowUseDeffenceCount < SwordFight_Define.DEFFENCE_USE_COUNT;
    }
    public void SetVibration() {
        if (!IsCpu) {
            SingletonCustom<HidVibration>.Instance.SetCommonVibration(playerNo);
        }
    }
    public void KnockBackAnimation(SwordFight_CharacterScript _attackChara, float _knockBackPower, bool _isVerticalAttack, bool _isHorizontalLeftAttack, bool _isHorizontalRightAttack, bool _isFirstAttack, bool _isSecondAttack, bool _isLastAttack, float _damage) {
        if (isInvincible || actionState == ActionState.DEATH || actionState == ActionState.DODGE) {
            return;
        }
        damageCount++;
        hp = Mathf.Clamp(hp - _damage, 0f, HP_MAX);
        if (damageCount >= 6) {
            bool isInvincible2 = isInvincible;
        }
        if (!IsCpu) {
            SingletonCustom<HidVibration>.Instance.SetCommonVibration(playerNo);
        }
        sweatEffect.Stop();
        sword.SetVerticalSwingFlg(_swingFlg: false);
        sword.SetHorizontalLeftSwingFlg(_swingFlg: false);
        sword.SetHorizontalRightSwingFlg(_swingFlg: false);
        isFirstAttack = false;
        isSecondAttack = false;
        isLastAttack = false;
        ResetCharaMotion();
        CharacterFaceChange_Sad();
        rigid.constraints = RigidbodyConstraints.FreezeRotation;
        rigid.velocity = new Vector3(0f, rigid.velocity.y, 0f);
        rigid.drag = 0.5f;
        if (_isVerticalAttack) {
            CalcManager.mCalcVector3 = -(_attackChara.GetPos() - GetPos()).normalized;
            CalcManager.mCalcVector3.y = 0f;
            if (hp <= 0f) {
                CalcManager.mCalcVector3 *= 2f;
            }
            rigid.AddForce(CalcManager.mCalcVector3 * _knockBackPower, ForceMode.Acceleration);
        } else if (_isHorizontalRightAttack) {
            CalcManager.mCalcVector3 = _attackChara.GetTransform().forward - _attackChara.GetTransform().right;
            CalcManager.mCalcVector3 -= _attackChara.GetTransform().right;
            CalcManager.mCalcVector3.y = 0f;
            if (hp <= 0f) {
                CalcManager.mCalcVector3 *= 2f;
            }
            rigid.AddForce(CalcManager.mCalcVector3 * (_knockBackPower * 0.5f), ForceMode.Acceleration);
        }
        knockBackDebugDir = CalcManager.mCalcVector3;
        bodyParts.Parts(2).SetLocalEulerAngles(-45f, 0f, 0f);
        for (int i = 0; i < breakEffect.Length; i++) {
            breakEffect[i].Play();
        }
        if (_isSecondAttack) {
            SingletonCustom<AudioManager>.Instance.SePlay("se_attack_hit_3");
        } else if (_isFirstAttack) {
            SingletonCustom<AudioManager>.Instance.SePlay("se_attack_hit_2");
        } else {
            SingletonCustom<AudioManager>.Instance.SePlay("se_attack_hit_1");
        }
        freezeTime = 0.3f;
        actionState = ActionState.FREEZE;
        sword.DisableTrail();
        if (hp <= 0f) {
            DeathAnimation();
        }
    }
    public void RepelAnimation() {
        if (!IsCpu) {
            SingletonCustom<HidVibration>.Instance.SetCommonVibration(playerNo);
        }
        sword.SetVerticalSwingFlg(_swingFlg: false);
        sword.SetHorizontalLeftSwingFlg(_swingFlg: false);
        sword.SetHorizontalRightSwingFlg(_swingFlg: false);
        isFirstAttack = false;
        isSecondAttack = false;
        isLastAttack = false;
        rigid.velocity = new Vector3(0f, rigid.velocity.y, 0f);
        ResetCharaMotion();
        bodyParts.Parts(2).SetLocalEulerAngles(-45f, 0f, 0f);
        rigid.constraints = RigidbodyConstraints.FreezeRotation;
        //??sweatEffect.main.loop = true;
        sweatEffect.Play();
        SingletonCustom<AudioManager>.Instance.SePlay("se_kick");
        repelTime = 0.5f;
        actionState = ActionState.REPEL;
    }
    public void DeathAnimation() {
        ResetCharaMotion();
        if (CheckCharacterFrontFloor()) {
            obj.transform.SetLocalEulerAngles(45f, 0f, 0f);
        } else {
            obj.transform.SetLocalEulerAngles(-45f, 0f, 0f);
        }
        sword.Drop();
        rigid.drag = 10f;
        rigid.constraints = RigidbodyConstraints.None;
        for (int i = 0; i < breakEffect.Length; i++) {
            breakEffect[i].Play();
        }
        SingletonCustom<AudioManager>.Instance.SePlay("se_chara_break");
        freezeTime = 0.3f;
        actionState = ActionState.DEATH;
        blowVec = rigid.velocity;
        CharacterFaceChange_Sad();
        sword.SetDeath();
    }
    private bool CheckCharacterFrontFloor() {
        int num = 0;
        int num2 = 0;
        for (nowSearchAngle = -45f; nowSearchAngle < 45f; nowSearchAngle += 1f) {
            Vector3 direction = Quaternion.Euler(new Vector3(0f, nowSearchAngle, 0f)) * characterDirectionCastAnchor.forward;
            if (Physics.Raycast(characterDirectionCastAnchor.position, direction, out raycastHit, float.PositiveInfinity, LayerMask.GetMask("NoHit"))) {
                if (raycastHit.collider.gameObject.GetComponent<SwordFight_Floor>() != null) {
                    num++;
                } else {
                    num2++;
                }
            }
        }
        if (num <= num2) {
            return false;
        }
        return true;
    }
    private IEnumerator InvincibleFlashedAnimation(Action _callBack = null) {
        isInvincible = true;
        bool _flashed = false;
        for (invincibleTime = 0.45f; invincibleTime > 0f; invincibleTime -= 0.2f) {
            if (_flashed) {
                for (int i = 0; i < bodyParts.rendererList.Length; i++) {
                    bodyParts.rendererList[i].material.color = new Color(0.63f, 0.63f, 0.63f, 1f);
                }
                sword.GetComponent<MeshRenderer>().material.color = new Color(0.63f, 0.63f, 0.63f, 1f);
                headgear.GetComponent<MeshRenderer>().material.color = new Color(0.63f, 0.63f, 0.63f, 1f);
                _flashed = false;
            } else {
                for (int j = 0; j < bodyParts.rendererList.Length; j++) {
                    bodyParts.rendererList[j].material.color = new Color(0.63f, 0.63f, 0.63f, 0.4f);
                }
                sword.GetComponent<MeshRenderer>().material.color = new Color(0.63f, 0.63f, 0.63f, 0.4f);
                headgear.GetComponent<MeshRenderer>().material.color = new Color(0.63f, 0.63f, 0.63f, 0.4f);
                _flashed = true;
            }
            yield return new WaitForSeconds(0.2f);
        }
        InvincibleAnimationStop();
    }
    private void InvincibleAnimationStop() {
        isInvincible = false;
        damageCount = 0;
        for (int i = 0; i < bodyParts.rendererList.Length; i++) {
            bodyParts.rendererList[i].material.color = new Color(0.63f, 0.63f, 0.63f, 1f);
        }
        sword.GetComponent<MeshRenderer>().material.color = new Color(0.63f, 0.63f, 0.63f, 1f);
        headgear.GetComponent<MeshRenderer>().material.color = new Color(0.63f, 0.63f, 0.63f, 1f);
    }
    public void CharacterFaceChange_Normal() {
        if (!isWinnerPlayer) {
            charaStyle.SetMainCharacterFaceDiff(playerNo, StyleTextureManager.MainCharacterFaceType.NORMAL);
        }
    }
    public void CharacterFaceChange_Happy() {
        rigid.drag = 10f;
        charaStyle.SetMainCharacterFaceDiff(playerNo, StyleTextureManager.MainCharacterFaceType.HAPPY);
        isWinnerPlayer = true;
    }
    public void CharacterFaceChange_Sad() {
        if (!isWinnerPlayer) {
            charaStyle.SetMainCharacterFaceDiff(playerNo, StyleTextureManager.MainCharacterFaceType.SAD);
        }
    }
    public void ResetCharaMotion() {
        obj.transform.SetLocalEulerAngles(0f, 0f, 0f);
        SetStartMotionData(motionData.GetMotionData(SwordFight_MotionData.MotionType.Default));
        StopMotionAnimation();
        rigid.drag = 0f;
        sweatEffect.Stop();
        if (isJumpInput) {
            rigid.constraints = RigidbodyConstraints.None;
        } else {
            rigid.constraints = RigidbodyConstraints.FreezeRotation;
        }
        CharacterFaceChange_Normal();
        ResetAttackData();
    }
    public void ResetDamageCount() {
        damageCount = 0;
    }
    public void ResetPosData() {
        nowPos = (prevPos = base.transform.position);
    }
    public void ResetControlInterval() {
        controlInterval = 0f;
    }
    private void SettingCharaParameter() {
        moveSpeed *= moveSpeedParamCorr;
        moveSpeedMax *= moveSpeedParamCorr;
    }
    public void SetActionState(ActionState _state) {
        actionState = _state;
    }
    public void ChangeCharacterDisplayActive(bool _isHide) {
        bodyParts.bodyAnchor.SetActive(_isHide);
    }
    public void SetControlInterval(float _time) {
        controlInterval = _time;
    }
    public bool CheckCharaHide() {
        return !bodyParts.bodyAnchor.activeSelf;
    }
    public bool CheckObj(GameObject _obj) {
        if (!(_obj == obj.gameObject)) {
            return base.gameObject == _obj;
        }
        return true;
    }
    public bool CheckActionState(ActionState _state) {
        return actionState == _state;
    }
    public bool CheckAttackAllow() {
        return reAttackTimeValue >= reAttackTimeValueMax;
    }
    public Rigidbody GetRigid() {
        return rigid;
    }
    public Vector3 GetMoveVec() {
        return nowPos - prevPos;
    }
    public Transform GetTransform() {
        return base.transform;
    }
    public Vector3 GetPos(bool _isLocal = false) {
        if (_isLocal) {
            return base.transform.localPosition;
        }
        return base.transform.position;
    }
    public CapsuleCollider GetCollider() {
        return charaCollider;
    }
    public float GetCharaBodySize() {
        return charaBodySize;
    }
    public float GetCharaHeight() {
        return charaHeight;
    }
    public Vector3 GetFormationPos(bool _local = true, bool _half = false, bool _deffense = true) {
        Vector3 vector = originAnchor.localPosition;
        if (!_half) {
            vector.z *= 2f;
        }
        if (!_local) {
            vector = originAnchor.parent.TransformPoint(vector);
        }
        return vector;
    }
    public ActionState GetActionState() {
        return actionState;
    }
    public float GetControlInterval() {
        return controlInterval;
    }
    public string GetName() {
        return name;
    }
    public int GetUniformNumber() {
        return uniformNumber;
    }
    public float GetAttackTimePer() {
        return reAttackTimeValue;
    }
    public Vector3 ConvertLocalPos(Vector3 _pos) {
        return base.transform.InverseTransformPoint(_pos);
    }
    public Vector3 ConvertLocalVec(Vector3 _vec) {
        return _vec;
    }
    public Vector3 ConvertWordVec(Vector3 _vec) {
        return _vec;
    }
    public void AiInit(SwordFight_Define.AiStrength _aiStrength, float _runSpeed, float _attackInterval, int _deffencePer, int _deffenceCounterPer) {
        aiStrength = _aiStrength;
        RUN_SPEED_DATA = _runSpeed;
        ATTACK_INTERVAL_DATA = _attackInterval;
        DEFFENCE_PER_DATA = _deffencePer;
        DEFFENCE_COUNTER_PER_DATA = _deffenceCounterPer;
        reAttackTimeValueMax = 0.25f + _attackInterval;
    }
    public void SettingGameStartPos(Vector3 _pos) {
        ResetCharaMotion();
        actionState = ActionState.STANDARD;
        gameStartStandbyPos = _pos;
        base.transform.position = gameStartStandbyPos;
        ResetPosData();
        base.transform.LookAt(SingletonCustom<SwordFight_FieldManager>.Instance.GetAnchors().CenterAnchor.position);
    }
    public void AiStandby() {
    }
    private void AiFreeze() {
        freezeTime -= Time.deltaTime;
        if (freezeTime <= 0f) {
            actionState = ActionState.STANDARD;
            ResetCharaMotion();
            if (playerNo != -1) {
                AiStandby();
            }
        }
    }
    public void AiRepel() {
        repelTime -= Time.deltaTime;
        if (repelTime <= 0f) {
            actionState = ActionState.STANDARD;
            ResetCharaMotion();
            if (playerNo != -1) {
                AiStandby();
            }
        }
    }
    public void AiDeath() {
        InvincibleAnimationStop();
    }
    public void AiSearchTarget() {
        SetSerachVicinityCharacter();
        rigid.velocity = new Vector3(0f, rigid.velocity.y, 0f);
        if (targetCharacter != null) {
            actionState = ActionState.MOVE;
        }
    }
    public void AiMoveToTarget() {
        if (targetCharacter.GetActionState() == ActionState.DEATH) {
            actionState = ActionState.STANDARD;
            targetCharacter = null;
        } else {
            RunTowardTarget(targetCharacter.GetPos(), RUN_SPEED_DATA);
        }
    }
    public void EndUpdate() {
        if (actionState == ActionState.DEATH) {
            SetLocalGravity();
            rigid.constraints = RigidbodyConstraints.None;
        } else {
            rigid.angularVelocity *= 0.9f;
            rigid.velocity *= 0.9f;
        }
    }
    public void SetAiContinusAttackData(bool _isSecondAttackOrder, bool _isLastAttackOrder) {
        aiContinusAttack_2nd = _isSecondAttackOrder;
        aiContinusAttack_Last = _isLastAttackOrder;
        rigid.velocity = new Vector3(0f, rigid.velocity.y, 0f);
        actionState = ActionState.ATTACK;
        for (int i = 0; i < randomAttackNum.Length; i++) {
            randomAttackNum[i] = UnityEngine.Random.Range(0, 100);
        }
    }
    public void AiAttack() {
        if (targetCharacter.GetActionState() == ActionState.DEATH) {
            targetCharacter = null;
            return;
        }
        SwordFight_Define.AiStrength aiStrength = this.aiStrength;
        if ((uint)(aiStrength - 1) <= 3u) {
            AiMoveToTarget();
        }
        aiStrength = this.aiStrength;
        if ((uint)(aiStrength - 2) <= 2u) {
            LookTargetCharacter();
        }
        if (attackAnimationTime > 0f) {
            return;
        }
        int num = isFirstAttack ? ((!isSecondAttack) ? 1 : 2) : 0;
        if (!isFirstAttack) {
            aiStrength = this.aiStrength;
            if ((uint)(aiStrength - 3) <= 1u) {
                if (targetCharacter.CheckNowPosStatus(NowPosStatus.DANGER) && CheckTargetNowPosStatusAdvantage()) {
                    VerticalSlashAnimation();
                } else if (!CheckTargetNowPosStatusAdvantage()) {
                    HorizontalRightSlashAnimation();
                } else if (randomAttackNum[num] < 50) {
                    HorizontalRightSlashAnimation();
                } else {
                    VerticalSlashAnimation();
                }
            } else if (randomAttackNum[num] < 50) {
                HorizontalRightSlashAnimation();
            } else {
                VerticalSlashAnimation();
            }
        } else if (!isSecondAttack && aiContinusAttack_2nd) {
            aiStrength = this.aiStrength;
            if ((uint)(aiStrength - 3) <= 1u) {
                if (targetCharacter.CheckNowPosStatus(NowPosStatus.DANGER) && CheckTargetNowPosStatusAdvantage()) {
                    VerticalSlashAnimation();
                } else if (!CheckTargetNowPosStatusAdvantage()) {
                    HorizontalRightSlashAnimation();
                } else if (randomAttackNum[num] < 50) {
                    HorizontalRightSlashAnimation();
                } else {
                    VerticalSlashAnimation();
                }
            } else if (randomAttackNum[num] < 50) {
                HorizontalRightSlashAnimation();
            } else {
                VerticalSlashAnimation();
            }
        } else {
            if (isLastAttack || !aiContinusAttack_Last) {
                return;
            }
            aiStrength = this.aiStrength;
            if ((uint)(aiStrength - 3) <= 1u) {
                if (targetCharacter.CheckNowPosStatus(NowPosStatus.DANGER) && CheckTargetNowPosStatusAdvantage()) {
                    VerticalSlashAnimation();
                } else if (!CheckTargetNowPosStatusAdvantage()) {
                    HorizontalRightSlashAnimation();
                } else if (randomAttackNum[num] < 50) {
                    HorizontalRightSlashAnimation();
                } else {
                    VerticalSlashAnimation();
                }
            } else if (randomAttackNum[num] < 50) {
                HorizontalRightSlashAnimation();
            } else {
                VerticalSlashAnimation();
            }
        }
    }
    public void AiJump() {
        Jump();
    }
    public void AiDeffence() {
        Dodge(new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f)));
    }
    public void AiWait() {
        if (!(attackAnimationTime > 0f)) {
            ResetCharaMotion();
            actionState = ActionState.STANDARD;
        }
    }
    public bool RunTowardTarget(Vector3 _pos, float _maxSpeed, float _speedMag = 1f) {
        if (actionState == ActionState.ATTACK) {
            _maxSpeed *= 0.75f;
        }
        if (CalcManager.Length(_pos, GetPos()) > STOP_CHECK_DISTANCE) {
            if (backTime > 0f) {
                backTime -= Time.deltaTime;
                if (IsJumpInput) {
                    Move(-(jumpTarget - GetPos()).normalized, Mathf.Min(CalcManager.Length(_pos, GetPos()), _maxSpeed), _speedMag);
                } else {
                    Move(-(_pos - GetPos()).normalized, Mathf.Min(CalcManager.Length(_pos, GetPos()), _maxSpeed), _speedMag);
                }
            } else {
                if (CalcManager.Length(_pos, GetPos()) > STOP_CHECK_DISTANCE && CalcManager.Length(_pos, GetPos()) < STOP_CHECK_DISTANCE * 1.5f && UnityEngine.Random.Range(0, 100) <= 5) {
                    backTime = UnityEngine.Random.Range(0.35f, 0.55f);
                }
                if (IsJumpInput) {
                    Move((jumpTarget - GetPos()).normalized, Mathf.Min(CalcManager.Length(_pos, GetPos()), _maxSpeed), _speedMag);
                } else {
                    Move((_pos - GetPos()).normalized, Mathf.Min(CalcManager.Length(_pos, GetPos()), _maxSpeed), _speedMag);
                }
            }
            return false;
        }
        if (CalcManager.Length(_pos, GetPos()) < STOP_CHECK_DISTANCE * 0.5f) {
            Move(-(_pos - GetPos()).normalized, Mathf.Min(CalcManager.Length(_pos, GetPos()), _maxSpeed), _speedMag);
        }
        return true;
    }
    public bool RunTargetCertainTime(Vector3 _pos, float _time, float _moveSpeed) {
        runTime += Time.deltaTime;
        if (runTime <= _time) {
            Move((_pos - GetPos()).normalized, _moveSpeed);
            return true;
        }
        runTime = 0f;
        return false;
    }
    public void LookForward() {
        rigid.MoveRotation(Quaternion.Euler(0f, 180f, 0f));
    }
    public void LookTargetCharacter() {
        CalcManager.mCalcVector3 = targetCharacter.GetPos();
        CalcManager.mCalcVector3.y = base.transform.position.y;
        calcVec[0] = (targetCharacter.GetPos() - GetPos()).normalized;
        rot.x = 0f;
        rot.y = CalcManager.Rot(calcVec[0], CalcManager.AXIS.Y);
        rot.z = 0f;
        rigid.MoveRotation(Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(rot), 20f * Time.deltaTime));
    }
    public void LookCursorDir() {
        MCM.GetCursor(playerNo).transform.parent = base.transform.parent;
        base.transform.parent = MCM.GetCursor(playerNo).transform;
        base.transform.SetLocalEulerAnglesY(0f);
        base.transform.parent = MCM.GetCursor(playerNo).transform.parent;
        base.transform.SetLocalScale(1f, 1f, 1f);
        MCM.GetCursor(playerNo).transform.parent = base.transform;
        MCM.GetCursor(playerNo).transform.SetLocalEulerAnglesY(0f);
        MCM.GetCursor(playerNo).transform.SetLocalScale(1f, 1f, 1f);
    }
    private void SetSerachVicinityCharacter() {
        int layer = obj.gameObject.layer;
        SettingLayer("NoHit");
        RaycastHit[] array = Physics.SphereCastAll(base.transform.position, GetCharaBodySize() * searchVisinityRange, base.transform.forward, LayerMask.GetMask("Character"));
        List<SwordFight_CharacterScript> list = new List<SwordFight_CharacterScript>();
        for (int i = 0; i < array.Length; i++) {
            if (!(array[i].collider.gameObject.GetComponent<SwordFight_CharacterScript>() != null) || array[i].collider.gameObject.GetComponent<SwordFight_CharacterScript>().charaNo == charaNo) {
                continue;
            }
            if (SwordFight_Define.IS_TEAM_MODE) {
                if (array[i].collider.gameObject.GetComponent<SwordFight_CharacterScript>().teamNo != teamNo && array[i].collider.gameObject.GetComponent<SwordFight_CharacterScript>().GetActionState() != ActionState.DEATH) {
                    list.Add(array[i].collider.gameObject.GetComponent<SwordFight_CharacterScript>());
                }
            } else if (array[i].collider.gameObject.GetComponent<SwordFight_CharacterScript>().GetActionState() != ActionState.DEATH) {
                list.Add(array[i].collider.gameObject.GetComponent<SwordFight_CharacterScript>());
            }
        }
        SwordFight_CharacterScript swordFight_CharacterScript = null;
        foreach (SwordFight_CharacterScript item in list) {
            if (swordFight_CharacterScript == null) {
                swordFight_CharacterScript = item;
            } else if (Vector3.Distance(base.transform.position, item.GetPos()) < Vector3.Distance(base.transform.position, swordFight_CharacterScript.GetPos())) {
                swordFight_CharacterScript = item;
            }
        }
        obj.gameObject.layer = layer;
        targetCharacter = swordFight_CharacterScript;
    }
    public bool IsTargetCharaActive() {
        return targetCharacter != null;
    }
    public SwordFight_CharacterScript GetTargetChara() {
        return targetCharacter;
    }
    public float GetTargetDistance() {
        return Vector3.Distance(base.transform.position, targetCharacter.GetPos());
    }
    public bool CheckCharacterFront() {
        int layer = obj.gameObject.layer;
        SettingLayer("NoHit");
        if (Physics.SphereCast(base.transform.position + new Vector3(0f, 1f, 0f), GetCharaBodySize() * frontSearchCharacterRange, base.transform.forward, out raycastHit, LayerMask.GetMask("Character"))) {
            if (debugShowData.frontSearchCharacter) {
                UnityEngine.Debug.Log("キャラ取得：" + raycastHit.collider.gameObject.name);
            }
            obj.gameObject.layer = layer;
            if (raycastHit.collider.gameObject.GetComponent<SwordFight_CharacterScript>() != null && raycastHit.collider.gameObject.GetComponent<SwordFight_CharacterScript>() != this) {
                frontCharacter = MCM.GetChara(raycastHit.collider.gameObject);
                return true;
            }
            return false;
        }
        obj.gameObject.layer = layer;
        return false;
    }
    public bool CheckTargetAngleCharacter(float startAngle, float endAngle) {
        int layer = obj.gameObject.layer;
        SettingLayer("NoHit");
        for (nowSearchAngle = startAngle; nowSearchAngle < endAngle; nowSearchAngle += 1f) {
            Vector3 direction = Quaternion.Euler(new Vector3(0f, nowSearchAngle, 0f)) * base.transform.forward;
            if (Physics.Raycast(base.transform.position + new Vector3(0f, 1f, 0f), direction, out raycastHit, LayerMask.GetMask("Character"))) {
                if (debugShowData.frontSearchCharacter) {
                    UnityEngine.Debug.Log("キャラ取得：" + raycastHit.collider.gameObject.name);
                }
                if (raycastHit.collider.gameObject.GetComponent<SwordFight_CharacterScript>() != null && raycastHit.collider.gameObject.GetComponent<SwordFight_CharacterScript>() != this) {
                    obj.gameObject.layer = layer;
                    return true;
                }
            }
        }
        obj.gameObject.layer = layer;
        return false;
    }
    public bool CheckAir() {
        return GetPos(_isLocal: true).y >= GetCharaHeight() * 0.15f;
    }
    public bool CheckNowPosStatus(NowPosStatus _status) {
        float num = Vector3.Distance(SingletonCustom<SwordFight_MainCharacterManager>.Instance.GetCenterAreaAnchor().position, base.transform.position);
        switch (_status) {
            case NowPosStatus.SAFE:
                if (num < 0.75f) {
                    return true;
                }
                return false;
            case NowPosStatus.CAUTION:
                if (num > 0.75f && num < 1.5f) {
                    return true;
                }
                return false;
            case NowPosStatus.DANGER:
                if (num > 1.5f) {
                    return true;
                }
                return false;
            default:
                UnityEngine.Debug.Log("[" + base.gameObject.name + "]エリア中心からのプレイヤ\u30fcの距離：" + num.ToString());
                return false;
        }
    }
    public bool CheckTargetNowPosStatusAdvantage() {
        float num = Vector3.Distance(SingletonCustom<SwordFight_MainCharacterManager>.Instance.GetCenterAreaAnchor().position, base.transform.position);
        float num2 = Vector3.Distance(SingletonCustom<SwordFight_MainCharacterManager>.Instance.GetCenterAreaAnchor().position, targetCharacter.transform.position);
        if (num < num2) {
            return true;
        }
        return false;
    }
    public bool CheckAiAction(AiActionMethod _action, int _index = 0) {
        if (aiActionMethod.Count > 0) {
            return _action == aiActionMethod[_index];
        }
        return false;
    }
    public void ShowCharacter(bool _show = true) {
        bodyParts.bodyAnchor.SetActive(_show);
    }
    public void Jump() {
        if (!isJumpInput) {
            rigid.velocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
            rigid.AddForce(base.transform.up * 0.55f * 1.75f, ForceMode.Impulse);
            rigid.constraints = RigidbodyConstraints.None;
            isJumpInput = true;
            jumpTarget = base.transform.position + new Vector3(UnityEngine.Random.Range(-2.5f, 2.5f), 0f, UnityEngine.Random.Range(-2.5f, 2.5f));
            SingletonCustom<AudioManager>.Instance.SePlay("se_sasuke_jump");
            psJump.Play();
        }
    }
    public void SettingLayer(string _layer) {
        obj.gameObject.layer = SwordFight_Define.ConvertLayerNo(_layer);
    }
    public void StopRunEffect() {
    }
    private void UpdateDebugActionState() {
        if (aiActionMethod != null && aiActionMethod.Count > 0 && aiActionMethod[aiActionMethod.Count - 1] != null) {
            debugActionState = aiActionMethod[aiActionMethod.Count - 1].Method.Name;
        } else {
            debugActionState = "行動が設定されていません : No." + charaNo.ToString();
        }
    }
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Field")) {
            rigid.constraints = (RigidbodyConstraints)80;
            isJumpInput = false;
        }
    }
    private void OnDestroy() {
        LeanTween.cancel(base.gameObject);
    }
    private void OnDrawGizmos() {
        if (!(rigid == null)) {
            if (debugShowData.frontSearchCharacter) {
                Gizmos.color = ColorPalet.green;
                Vector3 direction = Quaternion.Euler(new Vector3(0f, nowSearchAngle, 0f)) * base.transform.forward;
                Gizmos.DrawRay(base.transform.position + new Vector3(0f, 1f, 0f), direction);
            }
            if (debugShowData.transformRightDir) {
                Gizmos.color = ColorPalet.yellow;
                Gizmos.DrawRay(GetPos(), GetTransform().right);
            }
            if (debugShowData.searchVicinityCharacter) {
                Gizmos.color = ColorPalet.cyan;
                Gizmos.DrawWireSphere(base.transform.position, GetCharaBodySize() * searchVisinityRange);
            }
            if (debugShowData.knockBackDir) {
                Gizmos.color = ColorPalet.magenta;
                Gizmos.DrawRay(GetPos(), knockBackDebugDir);
            }
        }
    }
}
